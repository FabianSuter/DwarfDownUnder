using Microsoft.Xna.Framework;

namespace DwarfDownUnder.GameObjects;

public struct DwarfSegment
{
    /// <summary>
    /// The position this dwarf segment is at before the movement cycle occurs.
    /// </summary>
    public Vector2 At;

    /// <summary>
    /// The position this dwarf segment should move to during the next movement cycle.
    /// </summary>
    public Vector2 To;

    /// <summary>
    /// The direction this dwarf segment is moving.
    /// </summary>
    public Vector2 Direction;

    /// <summary>
    /// The opposite direction this dwarf segment is moving.
    /// </summary>
    public Vector2 ReverseDirection => new Vector2(-Direction.X, -Direction.Y);
}
