using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DwarfDownUnder.GameObjects;

public class Spider
{
    private const float MOVEMENT_SPEED = 5.0f;

    // The velocity of the spider that defines the direction and how much in that
    // direction to update the spiders position each update cycle.
    private Vector2 _velocity;

    // The AnimatedSprite used when drawing the spider.
    private AnimatedSprite _sprite;

    // The sound effect to play when the spider bounces off the edge of the room.
    private SoundEffect _bounceSoundEffect;

    /// <summary>
    /// Gets or Sets the position of the spider.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Creates a new Spider using the specified animated sprite and sound effect.
    /// </summary>
    /// <param name="sprite">The AnimatedSprite ot use when drawing the spider.</param>
    /// <param name="bounceSoundEffect">The sound effect to play when the spider bounces off a wall.</param>
    public Spider(AnimatedSprite sprite, SoundEffect bounceSoundEffect)
    {
        _sprite = sprite;
        _bounceSoundEffect = bounceSoundEffect;
    }

    /// <summary>
    /// Randomizes the velocity of the spider.
    /// </summary>
    public void RandomizeVelocity()
    {
        // Generate a random angle
        float angle = (float)(Random.Shared.NextDouble() * MathHelper.TwoPi);

        // Convert the angle to a direction vector
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);

        // Multiply the direction vector by the movement speed to get the
        // final velocity
        _velocity = direction * MOVEMENT_SPEED;
    }

    /// <summary>
    /// Handles a bounce event when the spider collides with a wall or boundary.
    /// </summary>
    /// <param name="normal">The normal vector of the surface the spider is bouncing against.</param>
    public void Bounce(Vector2 normal)
    {
        Vector2 newPosition = Position;

        // Adjust the position based on the normal to prevent sticking to walls.
        if (normal.X != 0)
        {
            // We are bouncing off a vertical wall (left/right).
            // Move slightly away from the wall in the direction of the normal.
            newPosition.X += normal.X * (_sprite.Width * 0.1f);
        }

        if (normal.Y != 0)
        {
            // We are bouncing off a horizontal wall (top/bottom).
            // Move slightly way from the wall in the direction of the normal.
            newPosition.Y += normal.Y * (_sprite.Height * 0.1f);
        }

        // Apply the new position
        Position = newPosition;

        // Normalize before reflecting
        normal.Normalize();

        // Apply reflection based on the normal.
        _velocity = Vector2.Reflect(_velocity, normal);

        // Play the bounce sound effect.
        Core.Audio.PlaySoundEffect(_bounceSoundEffect);
    }

    /// <summary>
    /// Returns a Circle value that represents collision bounds of the spider.
    /// </summary>
    /// <returns>A Circle value.</returns>
    public Circle GetBounds()
    {
        int x = (int)(Position.X + _sprite.Width * 0.5f);
        int y = (int)(Position.Y + _sprite.Height * 0.5f);
        int radius = (int)(_sprite.Width * 0.25f);

        return new Circle(x, y, radius);
    }

    /// <summary>
    /// Updates the spider.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    public void Update(GameTime gameTime)
    {
        // Update the animated sprite
        _sprite.Update(gameTime);

        // Update the position of the spider based on the velocity.
        Position += _velocity;
    }

    /// <summary>
    /// Draws the spider.
    /// </summary>
    public void Draw()
    {
        _sprite.Draw(Core.SpriteBatch, Position);
    }
}