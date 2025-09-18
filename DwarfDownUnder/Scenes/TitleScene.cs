using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace DwarfDownUnder.Scenes;

public class TitleScene : Scene
{
    private const string DWARF_TEXT = "Dwarf";
    private const string DOWN_UNDER_TEXT = "Down Under";
    private const string PRESS_ENTER_TEXT = "Press Enter To Start";

    // The font to use to render normal text.
    private SpriteFont _font;

    // The font used to render the title text.
    private SpriteFont _font5x;

    // The position to draw the dwarf text at.
    private Vector2 _dwarfTextPos;

    // The origin to set for the dwarf text.
    private Vector2 _dwarfTextOrigin;

    // The position to draw the downUnder text at.
    private Vector2 _downUnderTextPos;

    // The origin to set for the downUnder text.
    private Vector2 _downUnderTextOrigin;

    // The position to draw the press enter text at.
    private Vector2 _pressEnterPos;

    // The origin to set for the press enter text when drawing it.
    private Vector2 _pressEnterOrigin;

    // Texture for the background pattern
    private Texture2D _backgroundPattern;

    // Destination rectangle for the pattern to fill
    private Rectangle _backgroundDestination;

    // Offsetfor background pattern, to simulate scrolling
    private Vector2 _backgroundOffset;

    // Scrolling speed of the background pattern
    private float _scrollSpeed = 30.0f;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // While on the title screen, we can enable exit on escape so the player
        // can close the game by pressing the escape key.
        Core.ExitOnEscape = true;

        // Set the position and origin for the Dwarf text.
        Vector2 size = _font5x.MeasureString(DWARF_TEXT);
        _dwarfTextPos = new Vector2(640, 100);
        _dwarfTextOrigin = size * 0.5f;

        // Set the position and origin for the downUnder text.
        size = _font5x.MeasureString(DOWN_UNDER_TEXT);
        _downUnderTextPos = new Vector2(640, 240);
        _downUnderTextOrigin = size * 0.5f;

        // Set the position and origin for the press enter text.
        size = _font.MeasureString(PRESS_ENTER_TEXT);
        _pressEnterPos = new Vector2(640, 620);
        _pressEnterOrigin = size * 0.5f;

        // Initialize offset at zero
        _backgroundOffset = Vector2.Zero;

        // Pattern fills the entire screen
        _backgroundDestination = Core.GraphicsDevice.PresentationParameters.Bounds;
    }

    public override void LoadContent()
    {
        // Load the font for the standard text.
        _font = Core.Content.Load<SpriteFont>("fonts/norseRegular");

        // Load the font for the title text.
        _font5x = Content.Load<SpriteFont>("fonts/norseRegular_5x");

        // Load pattern texture
        // TODO: maybe a bigger pattern
        _backgroundPattern = Content.Load<Texture2D>("images/background-pattern");
    }

    public override void Update(GameTime gameTime)
    {
        // If the user presses enter, switch to the game scene.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            Core.ChangeScene(new GameScene());
        }

        // Update offsets to scroll down and right
        float offset = _scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _backgroundOffset.X -= offset;
        _backgroundOffset.Y -= offset;

        // Cap offset to the size of the pattern
        _backgroundOffset.X %= _backgroundPattern.Width;
        _backgroundOffset.Y %= _backgroundPattern.Height;
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        // Draw background pattern
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        Core.SpriteBatch.Draw(
            _backgroundPattern,             // texture  
            _backgroundDestination,         // destination rectangle
            new Rectangle(                  // source rectangle
                _backgroundOffset.ToPoint(),
                _backgroundDestination.Size
            ),
            Color.White * 0.5f              // color
        );
        Core.SpriteBatch.End();

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // The color to use for the drop shadow text.
        Color dropShadowColor = Color.Black * 0.5f;

        // Draw the Dwarf text slightly offset from it is original position and
        // with a transparent color to give it a drop shadow.
        Core.SpriteBatch.DrawString(
            _font5x,
            DWARF_TEXT,
            _dwarfTextPos + new Vector2(10, 10),
            dropShadowColor,
            0.0f,
            _dwarfTextOrigin,
            1.0f,
            SpriteEffects.None,
            1.0f
        );

        // Draw the Dwarf text on top of that at its original position.
        Core.SpriteBatch.DrawString(
            _font5x,
            DWARF_TEXT,
            _dwarfTextPos,
            Color.White,
            0.0f,
            _dwarfTextOrigin,
            1.0f,
            SpriteEffects.None,
            1.0f
        );

        // Draw the Down Under text slightly offset from it is original position and
        // with a transparent color to give it a drop shadow.
        Core.SpriteBatch.DrawString(
            _font5x,
            DOWN_UNDER_TEXT,
            _downUnderTextPos + new Vector2(10, 10),
            dropShadowColor,
            0.0f,
            _downUnderTextOrigin,
            1.0f,
            SpriteEffects.None,
            1.0f
        );

        // Draw the Down Under text on top of that at its original position.
        Core.SpriteBatch.DrawString(
            _font5x,
            DOWN_UNDER_TEXT,
            _downUnderTextPos,
            Color.White,
            0.0f,
            _downUnderTextOrigin,
            1.0f,
            SpriteEffects.None,
            1.0f
        );

        // Draw the press enter text.
        Core.SpriteBatch.DrawString(
            _font,
            PRESS_ENTER_TEXT,
            _pressEnterPos,
            Color.White,
            0.0f,
            _pressEnterOrigin,
            1.0f,
            SpriteEffects.None,
            0.0f
        );

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }
}
