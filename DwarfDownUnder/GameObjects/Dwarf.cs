using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DwarfDownUnder.GameObjects;

public class Dwarf
{
    // A constant value that represents the amount of time to wait between movement updates.
    private static readonly TimeSpan s_movementTime = TimeSpan.FromMilliseconds(200);

    // The amount of time that has elapsed since the last movement update.
    private TimeSpan _movementTimer;

    // Normalized value (0-1) representing progress between movement ticks for visual interpolation
    private float _movementProgress;

    // The next direction to apply to the dwarf chain during the next movement update.
    private Vector2 _nextDirection;

    // The number of pixels to move the dwarf during the movement cycle.
    private float _stride;

    // The AnimatedSprite used when drawing the dwarf
    private AnimatedSprite _sprite;

    // Buffer to queue inputs input by player during input polling.
    private Queue<Vector2> _inputBuffer;

    // The maximum size of the buffer queue.
    private const int MAX_BUFFER_SIZE = 2;

    // Current dwarf position
    public Vector2 At;

    // Position dwarf is moving to
    public Vector2 To;

    /// <summary>
    /// Creates a new Dwarf using the specified animated sprite.
    /// </summary>
    /// <param name="sprite">The AnimatedSprite to use when drawing the dwarf.</param>
    public Dwarf(AnimatedSprite sprite)
    {
        _sprite = sprite;
    }

    /// <summary>
    /// Initializes the dwarf, can be used to reset it back to an initial state.
    /// </summary>
    /// <param name="startingPosition">The position the dwarf should start at.</param>
    /// <param name="stride">The total number of pixels to move the head segment during each movement cycle.</param>
    public void Initialize(Vector2 startingPosition, float stride)
    {
        // Initialize the segment collection.
        // _segments = new List<DwarfSegment>();

        // Set the stride
        _stride = stride;

        // Create the initial head of the dwarf chain.
        // DwarfSegment head = new DwarfSegment();
        At = startingPosition;
        To = startingPosition + new Vector2(_stride, 0);
        _nextDirection = Vector2.UnitY;

        // Zero out the movement timer.
        _movementTimer = TimeSpan.Zero;

        // Initialize the input buffer
        _inputBuffer = new Queue<Vector2>(MAX_BUFFER_SIZE);
    }

    private void HandleInput()
    {
        // TODO: Maybe also handle sprites here?

        Vector2 potentialNextDirection = Vector2.Zero;

        if (GameController.MoveUp())
        {
            potentialNextDirection = -Vector2.UnitY;
        }
        else if (GameController.MoveDown())
        {
            potentialNextDirection = Vector2.UnitY;
        }
        else if (GameController.MoveLeft())
        {
            potentialNextDirection = -Vector2.UnitX;
        }
        else if (GameController.MoveRight())
        {
            potentialNextDirection = Vector2.UnitX;
        }

        // If a new direction was input, consider adding it to the buffer
        if (potentialNextDirection != Vector2.Zero && _inputBuffer.Count < MAX_BUFFER_SIZE)
        {
            // If the buffer is empty, validate against the current direction;
            // otherwise, validate against the last buffered direction
            // Vector2 validateAgainst = _inputBuffer.Count > 0 ?
            //                         _inputBuffer.Last() :
            //                         _segments[0].Direction;

            // Only allow direction change if it is not reversing the current
            // direction.  This prevents the dwarf from backing into itself
            // float dot = Vector2.Dot(potentialNextDirection, validateAgainst);
            // if (dot >= 0)
            // {
            _inputBuffer.Enqueue(potentialNextDirection);
            // }
        }
    }

    private void Move()
    {
        // Get the next direction from the input buffer, if available
        if (_inputBuffer.Count > 0)
        {
            _nextDirection = _inputBuffer.Dequeue();
        }

        // Update the head's "at" position to be where it was moving "to"
        At = To;

        // Update the head's "to" position to the next tile in the direction
        // it is moving.
        To = At + _nextDirection * _stride;
    }

    /// <summary>
    /// Updates the dwarf.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    public void Update(GameTime gameTime)
    {
        // Update the animated sprite.
        _sprite.Update(gameTime);

        // Handle any player input
        HandleInput();

        // Increment the movement timer by the frame elapsed time.
        _movementTimer += gameTime.ElapsedGameTime;

        // If the movement timer has accumulated enough time to be greater than
        // the movement time threshold, then perform a full movement.
        if (_movementTimer >= s_movementTime)
        {
            _movementTimer -= s_movementTime;
            Move();
        }

        // Update the movement lerp offset amount
        _movementProgress = (float)(_movementTimer.TotalSeconds / s_movementTime.TotalSeconds);
    }

    /// <summary>
    /// Draws the dwarf.
    /// </summary>
    public void Draw()
    {
        // Iterate through each segment and draw it
        // foreach (DwarfSegment segment in _segments)
        // {
            // Calculate the visual position of the segment at the moment by
            // lerping between its "at" and "to" position by the movement
            // offset lerp amount
            Vector2 pos = Vector2.Lerp(At, To, _movementProgress);

            // Draw the dwarf sprite at the calculated visual position of this
            // segment
            _sprite.Draw(Core.SpriteBatch, pos);
        // }
    }

    /// <summary>
    /// Returns a Circle value that represents collision bounds of the dwarf.
    /// </summary>
    /// <returns>A Circle value.</returns>
    public Circle GetBounds()
    {
        // DwarfSegment head = _segments[0];

        // Calculate the visual position of the head at the moment of this
        // method call by lerping between the "at" and "to" position by the
        // movement offset lerp amount
        Vector2 pos = Vector2.Lerp(At, To, _movementProgress);

        // Create the bounds using the calculated visual position of the head.
        Circle bounds = new Circle(
            (int)(pos.X + (_sprite.Width * 0.5f)),
            (int)(pos.Y + (_sprite.Height * 0.5f)),
            (int)(_sprite.Width * 0.5f)
        );

        return bounds;
    }
}
