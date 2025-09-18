using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;

namespace DwarfDownUnder.Scenes;

public class GameScene : Scene
{
    // Speed multiplier when moving
    private const float MOVE_SPEED = 5.0f;

    // Sprite of dwarf
    private AnimatedSprite _dwarf;

    // Track position of dwarf
    private Vector2 _dwarfPosition;

    // Sprite of coin
    private Sprite _coin;

    // Track position of coin
    private Vector2 _coinPosition;

    // Tilemap
    private Tilemap _tilemap;

    // Define bounds of playing room
    private Rectangle _roomBounds;

    // SFX for collecting something
    private SoundEffect _collectSFX;

    // SpriteFont for displaying text
    private SpriteFont _font;

    // Tracks player score
    private int _score = 0;

    // Position to draw score text
    private Vector2 _scoreTextPosition;

    // Origin of score text
    private Vector2 _scoreTextOrigin;

    public override void Initialize()
    {
        // Also call LoadContent
        base.Initialize();

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen
        Core.ExitOnEscape = false;

        Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;

        _roomBounds = new Rectangle(
            (int)_tilemap.TileWidth,
            (int)_tilemap.TileHeight * 2,
            screenBounds.Width - (int)(_tilemap.TileWidth * 2),
            screenBounds.Height - (int)(_tilemap.TileHeight * 3)
        );

        // Set initial position of dwarf to center of screen
        int centerRow = _tilemap.Rows / 2;
        int centerCol = _tilemap.Columns / 2;
        _dwarfPosition = new Vector2(centerCol * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);

        // Initial coin position in top left corner
        _coinPosition = new Vector2(_roomBounds.Left, _roomBounds.Top);

        // Set score text aligned to left edge of room bounds and vertically centered on first tile
        _scoreTextPosition = new Vector2(_roomBounds.Left, _tilemap.TileHeight * 0.5f);

        // Set origin of score text left centered
        float scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
        _scoreTextOrigin = new Vector2(0, scoreTextYOrigin);
    }

    public override void LoadContent()
    {
        // Load atlas texture
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        // Create dwarf sprite from atlas
        _dwarf = atlas.CreateAnimatedSprite("dwarfB-front-idle");
        _dwarf.Scale = new Vector2(2.0f, 2.0f);

        // Create coin sprite from atlas
        _coin = atlas.CreateSprite("coin");
        _coin.Scale = new Vector2(2.0f, 2.0f);

        // Load tilemap
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(2.0f, 2.0f);

        // Load the collect sound effect.
        _collectSFX = Content.Load<SoundEffect>("audio/collect");

        // Load font
        _font = Core.Content.Load<SpriteFont>("fonts/norseRegular");
    }

    public override void Update(GameTime gameTime)
    {
        // Update dwarf sprite
        _dwarf.Update(gameTime);

        // Check for keyboard input
        CheckKeyboardInput();

        // Bounding circle for the dwarf
        Circle dwarfBounds = new Circle(
            (int)(_dwarfPosition.X + (_dwarf.Width * 0.5f)),
            (int)(_dwarfPosition.Y + (_dwarf.Height * 0.5f)),
            (int)(_dwarf.Width * 0.5f)
        );

        // Bounding circle for the coin
        Circle coinBounds = new Circle(
            (int)(_coinPosition.X + (_coin.Width * 0.5f)),
            (int)(_coinPosition.Y + (_coin.Height * 0.5f)),
            (int)(_coin.Width * 0.5f)
        );

        // Check if dwarf is out of room bounds, and if so, move it back
        if (dwarfBounds.Left < _roomBounds.Left)
        {
            _dwarfPosition.X = _roomBounds.Left;
        }
        else if (dwarfBounds.Right > _roomBounds.Right)
        {
            _dwarfPosition.X = _roomBounds.Right - _dwarf.Width;
        }

        if (dwarfBounds.Top < _roomBounds.Top)
        {
            _dwarfPosition.Y = _roomBounds.Top;
        }
        else if (dwarfBounds.Bottom > _roomBounds.Bottom)
        {
            _dwarfPosition.Y = _roomBounds.Bottom - _dwarf.Height;
        }

        // Check if dwarf collides with coin
        if (dwarfBounds.Intersects(coinBounds))
        {
            // Choose random row and column
            int column = Random.Shared.Next(1, _tilemap.Columns - 1);
            int row = Random.Shared.Next(2, _tilemap.Rows - 1);

            // Move coin to new position
            _coinPosition = new Vector2(column * _coin.Width, row * _coin.Height);

            // Play collect sfx
            Core.Audio.PlaySoundEffect(_collectSFX);

            // Increase score
            _score += 100;
        }
    }

    private void CheckKeyboardInput()
    {
        // Get reference to keyboard
        KeyboardInfo keyboard = Core.Input.Keyboard;

        // If the escape key is pressed, return to the title screen.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Core.ChangeScene(new TitleScene());
        }

        // If the space key is held down, the movement speed increases by 1.5
        float speed = MOVE_SPEED;
        if (keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        // If the W or Up keys are down, move the dwarf up on the screen.
        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
        {
            _dwarfPosition.Y -= speed;
        }

        // if the S or Down keys are down, move the dwarf down on the screen.
        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
        {
            _dwarfPosition.Y += speed;
        }

        // If the A or Left keys are down, move the dwarf left on the screen.
        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
        {
            _dwarfPosition.X -= speed;
        }

        // If the D or Right keys are down, move the dwarf right on the screen.
        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
        {
            _dwarfPosition.X += speed;
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw tilemap
        _tilemap.Draw(Core.SpriteBatch);

        // Draw dwarf sprite
        _dwarf.Draw(Core.SpriteBatch, _dwarfPosition);

        // Draw coin sprite
        _coin.Draw(Core.SpriteBatch, _coinPosition);

        // Draw score
        Core.SpriteBatch.DrawString(
            _font,              // font
            $"Score: {_score}", // text
            _scoreTextPosition, // position
            Color.White,        // color
            0.0f,               // rotation
            _scoreTextOrigin,   // origin
            1.0f,               // scale
            SpriteEffects.None, // effects
            0.0f                // layer depth
        );

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }
}
