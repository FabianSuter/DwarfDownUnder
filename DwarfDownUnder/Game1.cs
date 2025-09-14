using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace DwarfDownUnder;

public class Game1 : Core
{
    // Sprite of dwarf
    private AnimatedSprite _dwarf;

    // Track position of dwarf
    private Vector2 _dwarfPosition;

    // Tilemap
    private Tilemap _tilemap;

    // Speed multiplier when moving
    private const float MOVE_SPEED = 5.0f;

    public Game1() : base("Dwarf Down Under", 1280, 800, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Load atlas texture
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        // Create dwarf sprite from atlas
        _dwarf = atlas.CreateAnimatedSprite("dwarfB-front-idle");
        _dwarf.Scale = new Vector2(2.0f, 2.0f);

        // Load tilemap
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(2.0f, 2.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        // Update dwarf sprite
        _dwarf.Update(gameTime);

        // Check for keyboard input
        CheckKeyboardInput();

        // Bounding rectangle for the screen, temporary
        Rectangle screenBounds = new Rectangle(
            0,
            0,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight
        );

        // Bounding circle for the dwarf
        Circle dwarfBounds = new Circle(
            (int)(_dwarfPosition.X + (_dwarf.Width * 0.5f)),
            (int)(_dwarfPosition.Y + (_dwarf.Height * 0.5f)),
            (int)(_dwarf.Width * 0.5f)
        );

        // Check if dwarf is out of screen bounds, and if so, move it back
        if (dwarfBounds.Left < screenBounds.Left)
        {
            _dwarfPosition.X = screenBounds.Left;
        }
        else if (dwarfBounds.Right > screenBounds.Right)
        {
            _dwarfPosition.X = screenBounds.Right - _dwarf.Width;
        }

        if (dwarfBounds.Top < screenBounds.Top)
        {
            _dwarfPosition.Y = screenBounds.Top;
        }
        else if (dwarfBounds.Bottom > screenBounds.Bottom)
        {
            _dwarfPosition.Y = screenBounds.Bottom - _dwarf.Height;
        }

        base.Update(gameTime);
    }

    private void CheckKeyboardInput()
    {
        // If the space key is held down, the movement speed increases by 1.5
        float speed = MOVE_SPEED;
        if (Input.Keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        // If the W or Up keys are down, move the slime up on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up))
        {
            _dwarfPosition.Y -= speed;
        }

        // if the S or Down keys are down, move the slime down on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down))
        {
            _dwarfPosition.Y += speed;
        }

        // If the A or Left keys are down, move the slime left on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left))
        {
            _dwarfPosition.X -= speed;
        }

        // If the D or Right keys are down, move the slime right on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right))
        {
            _dwarfPosition.X += speed;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw tilemap
        _tilemap.Draw(SpriteBatch);

        // Draw dwarf sprite
        _dwarf.Draw(SpriteBatch, _dwarfPosition);

        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
