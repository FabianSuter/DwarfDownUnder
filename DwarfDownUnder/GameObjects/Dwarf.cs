using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DwarfDownUnder.GameObjects;

public class Dwarf
{
    // Current movement state
    private MovementState _movementState;

    // public Camera _camera { get; set; }

    // Buffer to queue inputs input by player during input polling.
    // private Queue<InputType> _inputBuffer;

    // The maximum size of the buffer queue.
    // private const int MAX_BUFFER_SIZE = 2;

    // A constant value that represents the amount of time to wait between movement updates.
    private static readonly TimeSpan s_movementTime = TimeSpan.FromMilliseconds(150);

    // The amount of time that has elapsed since the last movement update.
    private TimeSpan _movementTimer;

    // Normalized value (0-1) representing progress between movement ticks for visual interpolation
    // private float _movementProgress;

    // The number of pixels to move the dwarf during the movement cycle.
    private float _stride;

    /// <summary>
    /// The AnimatedSprite used when drawing the dwarf
    /// </summary>
    public AnimatedSprite _sprite;

    /// <summary>
    /// The dictionary of AnimatedSprites for the dwarf.
    /// </summary>
    public Dictionary<string, AnimatedSprite> _spriteDict = [];

    /// <summary>
    /// The position of the dwarf
    /// </summary>
    public Vector2 At;

    /// <summary>
    /// The TileCollision instance for handling tile-based collision detection.
    /// </summary>
    public TileCollision _tileColl;

    // Dwarf position before movement
    // public Vector2 From;

    // Position dwarf is moving to
    // public Vector2 To;

    /// <summary>
    /// Creates a new Dwarf and fills the dict with animations from the atlas. Default sprite is "idle-front".
    /// </summary>
    /// <param name="atlas">The TextureAtlas to pull the animations from</param>
    public Dwarf(TextureAtlas atlas)
    {
        // Create the sprites for the dwarf from the atlas.
        AnimatedSprite dwarfIdleFront = atlas.CreateAnimatedSprite("dwarfB-idle-front");
        dwarfIdleFront.Scale = new Vector2(2.0f, 2.0f);
        AnimatedSprite dwarfIdleBack = atlas.CreateAnimatedSprite("dwarfB-idle-back");
        dwarfIdleBack.Scale = new Vector2(2.0f, 2.0f);
        AnimatedSprite dwarfIdleRight = atlas.CreateAnimatedSprite("dwarfB-idle-right");
        dwarfIdleRight.Scale = new Vector2(2.0f, 2.0f);
        AnimatedSprite dwarfWalkFront = atlas.CreateAnimatedSprite("dwarfB-walk-front");
        dwarfWalkFront.Scale = new Vector2(2.0f, 2.0f);
        AnimatedSprite dwarfWalkBack = atlas.CreateAnimatedSprite("dwarfB-walk-back");
        dwarfWalkBack.Scale = new Vector2(2.0f, 2.0f);
        AnimatedSprite dwarfWalkRight = atlas.CreateAnimatedSprite("dwarfB-walk-right");
        dwarfWalkRight.Scale = new Vector2(2.0f, 2.0f);

        // Add the sprites to the sprite dictionary
        _spriteDict.Add("idle-front", dwarfIdleFront);
        _spriteDict.Add("idle-back", dwarfIdleBack);
        _spriteDict.Add("idle-right", dwarfIdleRight);
        _spriteDict.Add("walk-front", dwarfWalkFront);
        _spriteDict.Add("walk-back", dwarfWalkBack);
        _spriteDict.Add("walk-right", dwarfWalkRight);

        // Set initial sprite as default
        _sprite = _spriteDict["idle-front"];

        // Set initial state to down
        TransitionTo(new MovementDown());
    }

    /// <summary>
    /// Adds animations of the dwarf
    /// </summary>
    /// <param name="key">Key for the added animation. Use normalized vectors</param>
    /// <param name="sprite">The AnimatedSprite to use when drawing the dwarf</param>
    public void AddAnimation(string key, AnimatedSprite sprite)
    {
        _spriteDict.Add(key, sprite);
    }

    /// <summary>
    /// Initializes the dwarf, can be used to reset it back to an initial state.
    /// </summary>
    /// <param name="startingPosition">The position the dwarf should start at.</param>
    /// <param name="stride">The total number of pixels to move the dwarf during each movement cycle.</param>
    public void Initialize(Vector2 startingPosition, float stride, TiledMap tiledMap)
    {
        // Set the stride
        _stride = stride;

        // Initialize tile collision
        _tileColl = new TileCollision(tiledMap);

        // Create the initial position
        At = startingPosition;
        // From = At;
        // To = At;

        // Zero out the movement timer.
        _movementTimer = TimeSpan.Zero;

        // _camera = new Camera(startingPosition);
    }

    /// <summary>
    /// Transitions to a new movement state.
    /// </summary>
    /// <param name="state">The new MovementState to transition to.</param>
    public void TransitionTo(MovementState state)
    {
        this._movementState = state;
        this._movementState.SetMovementState(this);
    }

    private void UpdateDwarfState()
    {
        // Update the MovementState
        _movementState.UpdateState();
    }

    private void Move()
    {
        // Update sprite based on movement state
        _movementState.UpdateSprite();

        // Update position
        var targetPos = At + _movementState._nextDirection * _stride;
        // Check for collision, if false don't move
        if (_tileColl.CanMoveTo(targetPos))
        {
            At += _movementState._nextDirection * _stride;
        }
        // From = At;
        // To = At + potentialNextDirection * _stride;
    }

    /// <summary>
    /// Updates the dwarf.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    public void Update(GameTime gameTime)
    {
        // Update the animated sprite.
        _sprite.Update(gameTime);

        // Increment the movement timer by the frame elapsed time.
        _movementTimer += gameTime.ElapsedGameTime;

        // // Update the movement lerp offset amount
        // _movementProgress = (float)(_movementTimer.TotalSeconds / s_movementTime.TotalSeconds);

        // // Clamp the movement progress to the [0, 1] range
        // if (_movementProgress > 1f) _movementProgress = 1f;

        // // Update current position and camera
        // At = Vector2.Lerp(From, To, _movementProgress);
        // _camera.Update(gameTime, At);

        // If the movement timer has accumulated enough time to be greater than
        // the movement time threshold, then perform a full movement.
        if (_movementTimer >= s_movementTime)
        {
            _movementTimer -= s_movementTime;

            UpdateDwarfState();

            Move();
        }
    }

    /// <summary>
    /// Draws the dwarf.
    /// </summary>
    public void Draw()
    {
        // Draw the dwarf sprite at the calculated visual position
        if (_movementState is MovementLeft)
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
        // Create the bounds using the calculated visual position of the head.
        Circle bounds = new Circle(
            (int)(At.X + (_sprite.Width * 0.5f)),
            (int)(At.Y + (_sprite.Height * 0.5f)),
            (int)(_sprite.Width * 0.5f)
        );

        return bounds;
    }
}
