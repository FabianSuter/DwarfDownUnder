using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DwarfDownUnder.GameObjects;

public class Dwarf
{
    // States of dwarf
    private enum DwarfState
    {
        IdleF,  // Facing player
        WalkF,  // Walking toward player
        IdleB,  // Facing away from player
        WalkB,  // Walking away from player
        IdleL,  // Facing left
        WalkL,  // Walking left
        IdleR,  // Facing right
        WalkR   // Walking right
    }

    // Current state of dwarf
    private DwarfState _currState = DwarfState.IdleF;

    // Next state of dwarf
    private DwarfState _nextState;

    // A constant value that represents the amount of time to wait between movement updates.
    private static readonly TimeSpan s_movementTime = TimeSpan.FromMilliseconds(100);

    // The amount of time that has elapsed since the last movement update.
    private TimeSpan _movementTimer;

    // Normalized value (0-1) representing progress between movement ticks for visual interpolation
    private float _movementProgress;

    // The next direction to apply to the dwarf during the next movement update.
    private Vector2 _nextDirection;

    // The number of pixels to move the dwarf during the movement cycle.
    private float _stride;

    // The AnimatedSprite used when drawing the dwarf
    private AnimatedSprite _sprite;

    // Sprite dict for different movement directions
    private Dictionary<object, AnimatedSprite> _spriteDict = [];

    // Buffer to queue inputs input by player during input polling.
    private Queue<Vector2> _inputBuffer;

    // The maximum size of the buffer queue.
    private const int MAX_BUFFER_SIZE = 2;

    // Current dwarf position
    public Vector2 At;

    // Position dwarf is moving to
    // public Vector2 To;

    /// <summary>
    /// Creates a new Dwarf using the specified animated sprite.
    /// </summary>
    /// <param name="sprite">The AnimatedSprite to use when drawing the dwarf.</param>
    public Dwarf(AnimatedSprite sprite)
    {
        var key = "idle-front";
        _spriteDict.Add(key, sprite);
        _sprite = _spriteDict[key]; // Set initial sprite as default

        // _sprite = sprite;
    }

    /// <summary>
    /// Adds animations of the dwarf
    /// </summary>
    /// <param name="key">Key for the added animation. Use normalized vectors</param>
    /// <param name="sprite">The AnimatedSprite to use when drawing the dwarf</param>
    public void AddAnimation(object key, AnimatedSprite sprite)
    {
        _spriteDict.Add(key, sprite);
    }

    /// <summary>
    /// Initializes the dwarf, can be used to reset it back to an initial state.
    /// </summary>
    /// <param name="startingPosition">The position the dwarf should start at.</param>
    /// <param name="stride">The total number of pixels to move the head segment during each movement cycle.</param>
    public void Initialize(Vector2 startingPosition, float stride)
    {
        // Set the stride
        _stride = stride;

        // Create the initial position
        At = startingPosition;

        // Zero out the movement timer.
        _movementTimer = TimeSpan.Zero;

        // Initialize the input buffer
        // _inputBuffer = new Queue<Vector2>(MAX_BUFFER_SIZE);
    }

    private void HandleInput()
    {
        switch (_currState)
        {
            // If idle and move sync, set next state to walk forward
            // If idle andother move, set to idle in the direction of movement
            // Else remain idle forward
            case DwarfState.IdleF:
                if (GameController.MoveDown())
                {
                    _nextState = DwarfState.WalkF;
                }
                else if (GameController.MoveUp())
                {
                    _nextState = DwarfState.IdleB;
                }
                else if (GameController.MoveLeft())
                {
                    _nextState = DwarfState.IdleL;
                }
                else if (GameController.MoveRight())
                {
                    _nextState = DwarfState.IdleR;
                }
                else
                {
                    _nextState = DwarfState.IdleF;
                }
                break;
            case DwarfState.IdleB:
                if (GameController.MoveDown())
                {
                    _nextState = DwarfState.IdleF;
                }
                else if (GameController.MoveUp())
                {
                    _nextState = DwarfState.WalkB;
                }
                else if (GameController.MoveLeft())
                {
                    _nextState = DwarfState.IdleL;
                }
                else if (GameController.MoveRight())
                {
                    _nextState = DwarfState.IdleR;
                }
                else
                {
                    _nextState = DwarfState.IdleB;
                }
                break;
            case DwarfState.IdleL:
                if (GameController.MoveDown())
                {
                    _nextState = DwarfState.IdleF;
                }
                else if (GameController.MoveUp())
                {
                    _nextState = DwarfState.IdleB;
                }
                else if (GameController.MoveLeft())
                {
                    _nextState = DwarfState.WalkL;
                }
                else if (GameController.MoveRight())
                {
                    _nextState = DwarfState.IdleR;
                }
                else
                {
                    _nextState = DwarfState.IdleL;
                }
                break;
            case DwarfState.IdleR:
                if (GameController.MoveDown())
                {
                    _nextState = DwarfState.IdleF;
                }
                else if (GameController.MoveUp())
                {
                    _nextState = DwarfState.IdleB;
                }
                else if (GameController.MoveLeft())
                {
                    _nextState = DwarfState.IdleL;
                }
                else if (GameController.MoveRight())
                {
                    _nextState = DwarfState.WalkR;
                }
                else
                {
                    _nextState = DwarfState.IdleR;
                }
                break;

            // If walking and move sync, remain in walk
            // If walking and other move, set to idle in the direction of movement
            // Else set to idle in the direction of movement
            case DwarfState.WalkF:
                if (GameController.MoveDown())
                {
                    _nextState = DwarfState.WalkF;
                }
                else if (GameController.MoveUp())
                {
                    _nextState = DwarfState.IdleB;
                }
                else if (GameController.MoveLeft())
                {
                    _nextState = DwarfState.IdleL;
                }
                else if (GameController.MoveRight())
                {
                    _nextState = DwarfState.WalkR;
                }
                else
                {
                    _nextState = DwarfState.IdleF;
                }
                break;
            case DwarfState.WalkB:
                if (GameController.MoveDown())
                {
                    _nextState = DwarfState.IdleF;
                }
                else if (GameController.MoveUp())
                {
                    _nextState = DwarfState.WalkB;
                }
                else if (GameController.MoveLeft())
                {
                    _nextState = DwarfState.IdleL;
                }
                else if (GameController.MoveRight())
                {
                    _nextState = DwarfState.WalkR;
                }
                else
                {
                    _nextState = DwarfState.IdleB;
                }
                break;
            case DwarfState.WalkL:
                if (GameController.MoveDown())
                {
                    _nextState = DwarfState.IdleF;
                }
                else if (GameController.MoveUp())
                {
                    _nextState = DwarfState.IdleB;
                }
                else if (GameController.MoveLeft())
                {
                    _nextState = DwarfState.WalkL;
                }
                else if (GameController.MoveRight())
                {
                    _nextState = DwarfState.WalkR;
                }
                else
                {
                    _nextState = DwarfState.IdleL;
                }
                break;
            case DwarfState.WalkR:
                if (GameController.MoveDown())
                {
                    _nextState = DwarfState.IdleF;
                }
                else if (GameController.MoveUp())
                {
                    _nextState = DwarfState.IdleB;
                }
                else if (GameController.MoveLeft())
                {
                    _nextState = DwarfState.WalkL;
                }
                else if (GameController.MoveRight())
                {
                    _nextState = DwarfState.WalkR;
                }
                else
                {
                    _nextState = DwarfState.IdleR;
                }
                break;
            default:
                break;
        }
    }

    private void Move()
    {
        Vector2 potentialNextDirection = Vector2.Zero;

        switch (_nextState)
        {
            case DwarfState.IdleF:
                _sprite = _spriteDict["idle-front"];
                break;
            case DwarfState.WalkF:
                potentialNextDirection = Vector2.UnitY;
                _sprite = _spriteDict["walk-front"];
                break;
            case DwarfState.IdleB:
                _sprite = _spriteDict["idle-back"];
                break;
            case DwarfState.WalkB:
                potentialNextDirection = -Vector2.UnitY;
                _sprite = _spriteDict["walk-back"];
                break;
            case DwarfState.IdleL:
                _sprite = _spriteDict["idle-right"];
                break;
            case DwarfState.WalkL:
                potentialNextDirection = -Vector2.UnitX;
                _sprite = _spriteDict["walk-right"];
                break;
            case DwarfState.IdleR:
                _sprite = _spriteDict["idle-right"];
                break;
            case DwarfState.WalkR:
                potentialNextDirection = Vector2.UnitX;
                _sprite = _spriteDict["walk-right"];
                break;
            default:
                break;
        }

        // Update position
        At += potentialNextDirection * _stride;

        // Update current state to next state
        _currState = _nextState;
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
        // _movementProgress = (float)(_movementTimer.TotalSeconds / s_movementTime.TotalSeconds);
    }

    /// <summary>
    /// Draws the dwarf.
    /// </summary>
    public void Draw()
    {
        // Calculate the visual position of the dwarf at the moment by
        // lerping between its "at" and "to" position by the movement
        // offset lerp amount
        // Vector2 pos = Vector2.Lerp(At, To, _movementProgress);

        // Draw the dwarf sprite at the calculated visual position
        if (_currState == DwarfState.WalkL || _currState == DwarfState.IdleL)
        {
            _sprite.Draw(Core.SpriteBatch, At, SpriteEffects.FlipHorizontally);
        }
        else
        {
            _sprite.Draw(Core.SpriteBatch, At);
        }
    }

    /// <summary>
    /// Returns a Circle value that represents collision bounds of the dwarf.
    /// </summary>
    /// <returns>A Circle value.</returns>
    public Circle GetBounds()
    {
        // Vector2 pos = Vector2.Lerp(At, To, _movementProgress);

        // Create the bounds using the calculated visual position of the head.
        Circle bounds = new Circle(
            (int)(At.X + (_sprite.Width * 0.5f)),
            (int)(At.Y + (_sprite.Height * 0.5f)),
            (int)(_sprite.Width * 0.5f)
        );

        return bounds;
    }
}
