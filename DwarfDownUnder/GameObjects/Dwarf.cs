using System;
using System.Collections.Generic;
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

    // Input types
    private enum InputType
    {
        Up, Down, Left, Right, None
    }

    // Current input being processed
    private InputType _currInput = InputType.None;

    // Next input taken from input buffer
    private InputType _nextInp;

    // Buffer to queue inputs input by player during input polling.
    private Queue<InputType> _inputBuffer;

    // The maximum size of the buffer queue.
    private const int MAX_BUFFER_SIZE = 2;

    // A constant value that represents the amount of time to wait between movement updates.
    private static readonly TimeSpan s_movementTime = TimeSpan.FromMilliseconds(150);

    // The amount of time that has elapsed since the last movement update.
    private TimeSpan _movementTimer;

    // Normalized value (0-1) representing progress between movement ticks for visual interpolation
    private float _movementProgress;

    // The number of pixels to move the dwarf during the movement cycle.
    private float _stride;

    // The AnimatedSprite used when drawing the dwarf
    private AnimatedSprite _sprite;

    // Sprite dict for different movement directions
    private Dictionary<object, AnimatedSprite> _spriteDict = [];

    // Current dwarf position
    public Vector2 At;

    // Dwarf position before movement
    public Vector2 From;

    // Position dwarf is moving to
    public Vector2 To;

    /// <summary>
    /// Creates a new Dwarf and fills the dict with animations from the atlas. Default sprite is "idle-front".
    /// </summary>
    /// <param name="atlas">The TextureAtlas to pull the animations from</param>
    public Dwarf(TextureAtlas atlas)
    {
        // Create the sprites for the dwarf from the atlas.
        AnimatedSprite dwarfIdleFront = atlas.CreateAnimatedSprite("dwarfB-idle-front");
        // dwarfIdleFront.Scale = new Vector2(2.0f, 2.0f);
        AnimatedSprite dwarfIdleBack = atlas.CreateAnimatedSprite("dwarfB-idle-back");
        // dwarfIdleBack.Scale = new Vector2(2.0f, 2.0f);
        AnimatedSprite dwarfIdleRight = atlas.CreateAnimatedSprite("dwarfB-idle-right");
        // dwarfIdleRight.Scale = new Vector2(2.0f, 2.0f);
        AnimatedSprite dwarfWalkFront = atlas.CreateAnimatedSprite("dwarfB-walk-front");
        // dwarfWalkFront.Scale = new Vector2(2.0f, 2.0f);
        AnimatedSprite dwarfWalkBack = atlas.CreateAnimatedSprite("dwarfB-walk-back");
        // dwarfWalkBack.Scale = new Vector2(2.0f, 2.0f);
        AnimatedSprite dwarfWalkRight = atlas.CreateAnimatedSprite("dwarfB-walk-right");
        // dwarfWalkRight.Scale = new Vector2(2.0f, 2.0f);

        // Add the sprites to the sprite dictionary
        _spriteDict.Add("idle-front", dwarfIdleFront);
        _spriteDict.Add("idle-back", dwarfIdleBack);
        _spriteDict.Add("idle-right", dwarfIdleRight);
        _spriteDict.Add("walk-front", dwarfWalkFront);
        _spriteDict.Add("walk-back", dwarfWalkBack);
        _spriteDict.Add("walk-right", dwarfWalkRight);

        // Set initial sprite as default
        _sprite = _spriteDict["idle-front"];
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
        _inputBuffer = new Queue<InputType>(MAX_BUFFER_SIZE);
    }

    private void HandleInput()
    {
        _currInput = InputType.None;
        // Get current input
        if (GameController.MoveDown())
        {
            _currInput = InputType.Down;
        }
        else if (GameController.MoveUp())
        {
            _currInput = InputType.Up;
        }
        else if (GameController.MoveLeft())
        {
            _currInput = InputType.Left;
        }
        else if (GameController.MoveRight())
        {
            _currInput = InputType.Right;
        }

        if (_inputBuffer.Count < MAX_BUFFER_SIZE)
        {
            _inputBuffer.Enqueue(_currInput);
        }
    }

    private void UpdateDwarfState()
    {
        if (_inputBuffer.Count > 0)
        {
            _nextInp = _inputBuffer.Dequeue();
        }
        switch (_currState)
            {
                // If idle and move sync, set next state to walk forward
                // If idle andother move, set to idle in the direction of movement
                // Else remain idle forward
                case DwarfState.IdleF:
                    if (_nextInp == InputType.Down)
                    {
                        _nextState = DwarfState.WalkF;
                    }
                    else if (_nextInp == InputType.Up)
                    {
                        _nextState = DwarfState.IdleB;
                    }
                    else if (_nextInp == InputType.Left)
                    {
                        _nextState = DwarfState.IdleL;
                    }
                    else if (_nextInp == InputType.Right)
                    {
                        _nextState = DwarfState.IdleR;
                    }
                    else
                    {
                        _nextState = DwarfState.IdleF;
                    }
                    break;
                case DwarfState.IdleB:
                    if (_nextInp == InputType.Down)
                    {
                        _nextState = DwarfState.IdleF;
                    }
                    else if (_nextInp == InputType.Up)
                    {
                        _nextState = DwarfState.WalkB;
                    }
                    else if (_nextInp == InputType.Left)
                    {
                        _nextState = DwarfState.IdleL;
                    }
                    else if (_nextInp == InputType.Right)
                    {
                        _nextState = DwarfState.IdleR;
                    }
                    else
                    {
                        _nextState = DwarfState.IdleB;
                    }
                    break;
                case DwarfState.IdleL:
                    if (_nextInp == InputType.Down)
                    {
                        _nextState = DwarfState.IdleF;
                    }
                    else if (_nextInp == InputType.Up)
                    {
                        _nextState = DwarfState.IdleB;
                    }
                    else if (_nextInp == InputType.Left)
                    {
                        _nextState = DwarfState.WalkL;
                    }
                    else if (_nextInp == InputType.Right)
                    {
                        _nextState = DwarfState.IdleR;
                    }
                    else
                    {
                        _nextState = DwarfState.IdleL;
                    }
                    break;
                case DwarfState.IdleR:
                    if (_nextInp == InputType.Down)
                    {
                        _nextState = DwarfState.IdleF;
                    }
                    else if (_nextInp == InputType.Up)
                    {
                        _nextState = DwarfState.IdleB;
                    }
                    else if (_nextInp == InputType.Left)
                    {
                        _nextState = DwarfState.IdleL;
                    }
                    else if (_nextInp == InputType.Right)
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
                    if (_nextInp == InputType.Down)
                    {
                        _nextState = DwarfState.WalkF;
                    }
                    else if (_nextInp == InputType.Up)
                    {
                        _nextState = DwarfState.IdleB;
                    }
                    else if (_nextInp == InputType.Left)
                    {
                        _nextState = DwarfState.IdleL;
                    }
                    else if (_nextInp == InputType.Right)
                    {
                        _nextState = DwarfState.WalkR;
                    }
                    else
                    {
                        _nextState = DwarfState.IdleF;
                    }
                    break;
                case DwarfState.WalkB:
                    if (_nextInp == InputType.Down)
                    {
                        _nextState = DwarfState.IdleF;
                    }
                    else if (_nextInp == InputType.Up)
                    {
                        _nextState = DwarfState.WalkB;
                    }
                    else if (_nextInp == InputType.Left)
                    {
                        _nextState = DwarfState.IdleL;
                    }
                    else if (_nextInp == InputType.Right)
                    {
                        _nextState = DwarfState.WalkR;
                    }
                    else
                    {
                        _nextState = DwarfState.IdleB;
                    }
                    break;
                case DwarfState.WalkL:
                    if (_nextInp == InputType.Down)
                    {
                        _nextState = DwarfState.IdleF;
                    }
                    else if (_nextInp == InputType.Up)
                    {
                        _nextState = DwarfState.IdleB;
                    }
                    else if (_nextInp == InputType.Left)
                    {
                        _nextState = DwarfState.WalkL;
                    }
                    else if (_nextInp == InputType.Right)
                    {
                        _nextState = DwarfState.WalkR;
                    }
                    else
                    {
                        _nextState = DwarfState.IdleL;
                    }
                    break;
                case DwarfState.WalkR:
                    if (_nextInp == InputType.Down)
                    {
                        _nextState = DwarfState.IdleF;
                    }
                    else if (_nextInp == InputType.Up)
                    {
                        _nextState = DwarfState.IdleB;
                    }
                    else if (_nextInp == InputType.Left)
                    {
                        _nextState = DwarfState.WalkL;
                    }
                    else if (_nextInp == InputType.Right)
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
        From = At;
        At += potentialNextDirection * _stride;
        To = At;

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

            UpdateDwarfState();

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
        // Calculate the visual position of the dwarf at the moment by
        // lerping between its "from" and "to" position by the movement
        // offset lerp amount
        Vector2 pos = Vector2.Lerp(From, To, _movementProgress);

        // Draw the dwarf sprite at the calculated visual position
        if (_currState == DwarfState.WalkL || _currState == DwarfState.IdleL)
        {
            _sprite.Draw(Core.SpriteBatch, pos, SpriteEffects.FlipHorizontally);
        }
        else
        {
            _sprite.Draw(Core.SpriteBatch, pos);
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
