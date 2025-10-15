using System.Numerics;
using DwarfDownUnder;
using DwarfDownUnder.GameObjects;

/// <summary>
/// The base class for different movement states of the Dwarf.
/// </summary>
public abstract class MovementState
{
    // Reference to the Dwarf
    protected Dwarf _dwarf;

    /// <summary>
    /// The next direction the dwarf is moving in (unit vector)
    /// </summary>
    public Vector2 _nextDirection = Vector2.Zero;

    /// <summary>
    /// Indicates whether the dwarf is currently walking
    /// </summary>
    public bool isWalking = false;

    /// <summary>
    /// Sets the MovementState of the given dwarf
    /// </summary>
    /// <param name="dwarf">The dwarf whose movement state is being set</param>
    public void SetMovementState(Dwarf dwarf)
    {
        _dwarf = dwarf;
    }

    /// <summary>
    /// Updates the sprite for the current movement state. Does NOT update the animation, handle this separately.
    /// </summary>
    public abstract void UpdateSprite();

    /// <summary>
    /// Updates the state based on input and may set the next direction and isWalking flag, depending on user input.
    /// </summary>
    public abstract void UpdateState();
}

class MovementDown : MovementState
{
    /// <summary>
    /// Updates the sprite for the current movement state. Does NOT update the animation, handle this separately.
    /// </summary>
    public override void UpdateSprite()
    {
        if (isWalking)
        {
            _dwarf._sprite = _dwarf._spriteDict["walk-front"];
        }
        else
        {
            _dwarf._sprite = _dwarf._spriteDict["idle-front"];
        }
    }

    /// <summary>
    /// Updates the state based on input and may set the next direction and isWalking flag, depending on user input.
    /// </summary>
    public override void UpdateState()
    {
        isWalking = false;
        _nextDirection = Vector2.Zero;
        if (GameController.MoveDown())
        {
            isWalking = true;
            _nextDirection = Vector2.UnitY;
        }
        else if (GameController.MoveUp()) { _dwarf.TransitionTo(new MovementUp()); }
        else if (GameController.MoveLeft()) { _dwarf.TransitionTo(new MovementLeft()); }
        else if (GameController.MoveRight()) { _dwarf.TransitionTo(new MovementRight()); }
    }
}

class MovementLeft : MovementState
{
    /// <summary>
    /// Updates the sprite for the current movement state. Does NOT update the animation, handle this separately.
    /// </summary>
    public override void UpdateSprite()
    {
        if (isWalking)
        {
            _dwarf._sprite = _dwarf._spriteDict["walk-right"];
        }
        else
        {
            _dwarf._sprite = _dwarf._spriteDict["idle-right"];
        }
    }

    /// <summary>
    /// Updates the state based on input and may set the next direction and isWalking flag, depending on user input.
    /// </summary>
    public override void UpdateState()
    {
        isWalking = false;
        _nextDirection = Vector2.Zero;
        if (GameController.MoveLeft())
        {
            isWalking = true;
            _nextDirection = -Vector2.UnitX;
        }
        else if (GameController.MoveRight()) { _dwarf.TransitionTo(new MovementRight()); }
        else if (GameController.MoveUp()) { _dwarf.TransitionTo(new MovementUp()); }
        else if (GameController.MoveDown()) { _dwarf.TransitionTo(new MovementDown()); }
    }
}

class MovementRight : MovementState
{
    /// <summary>
    /// Updates the sprite for the current movement state. Does NOT update the animation, handle this separately.
    /// </summary>
    public override void UpdateSprite()
    {
        if (isWalking)
        {
            _dwarf._sprite = _dwarf._spriteDict["walk-right"];
        }
        else
        {
            _dwarf._sprite = _dwarf._spriteDict["idle-right"];
        }
    }

    /// <summary>
    /// Updates the state based on input and may set the next direction and isWalking flag, depending on user input.
    /// </summary>
    public override void UpdateState()
    {
        isWalking = false;
        _nextDirection = Vector2.Zero;
        if (GameController.MoveRight())
        {
            isWalking = true;
            _nextDirection = Vector2.UnitX;
        }
        else if (GameController.MoveLeft()) { _dwarf.TransitionTo(new MovementLeft()); }
        else if (GameController.MoveUp()) { _dwarf.TransitionTo(new MovementUp()); }
        else if (GameController.MoveDown()) { _dwarf.TransitionTo(new MovementDown()); }
    }
}

class MovementUp : MovementState
{
    /// <summary>
    /// Updates the sprite for the current movement state. Does NOT update the animation, handle this separately.
    /// </summary>
    public override void UpdateSprite()
    {
        if (isWalking)
        {
            _dwarf._sprite = _dwarf._spriteDict["walk-back"];
        }
        else
        {
            _dwarf._sprite = _dwarf._spriteDict["idle-back"];
        }
    }

    /// <summary>
    /// Updates the state based on input and may set the next direction and isWalking flag, depending on user input.
    /// </summary>
    public override void UpdateState()
    {
        isWalking = false;
        _nextDirection = Vector2.Zero;
        if (GameController.MoveUp())
        {
            isWalking = true;
            _nextDirection = -Vector2.UnitY;
        }
        else if (GameController.MoveDown()) { _dwarf.TransitionTo(new MovementDown()); }
        else if (GameController.MoveLeft()) { _dwarf.TransitionTo(new MovementLeft()); }
        else if (GameController.MoveRight()) { _dwarf.TransitionTo(new MovementRight()); }
    }
}