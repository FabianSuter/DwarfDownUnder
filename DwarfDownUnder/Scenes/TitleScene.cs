using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;

namespace DwarfDownUnder.Scenes;

public class TitleScene : Scene
{
    private const string DWARF_TEXT = "Dwarf";
    private const string DOWN_UNDER_TEXT = "Down Under";
    private const string PRESS_ENTER_TEXT = "Press Enter To Start";

    private SoundEffect _uiSoundEffect;
    private Panel _titleScreenButtonsPanel;
    private Panel _optionsPanel;
    private Button _optionsButton;
    private Button _optionsBackButton;


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

    private void CreateTitlePanel()
    {
        // Create a container to hold all of our buttons
        _titleScreenButtonsPanel = new Panel();
        _titleScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _titleScreenButtonsPanel.AddToRoot();

        var startButton = new Button();
        startButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        startButton.Visual.X = 50;
        startButton.Visual.Y = -12;
        startButton.Visual.Width = 70;
        startButton.Text = "Start";
        startButton.Click += HandleStartClicked;
        _titleScreenButtonsPanel.AddChild(startButton);

        _optionsButton = new Button();
        _optionsButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsButton.Visual.X = -50;
        _optionsButton.Visual.Y = -12;
        _optionsButton.Visual.Width = 70;
        _optionsButton.Text = "Options";
        _optionsButton.Click += HandleOptionsClicked;
        _titleScreenButtonsPanel.AddChild(_optionsButton);

        startButton.IsFocused = true;
    }

    private void HandleStartClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Change to the game scene to start the game.
        Core.ChangeScene(new GameScene());
    }

    private void HandleOptionsClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        
        // Set the title panel to be invisible.
        _titleScreenButtonsPanel.IsVisible = false;

        // Set the options panel to be visible.
        _optionsPanel.IsVisible = true;

        // Give the back button on the options panel focus.
        _optionsBackButton.IsFocused = true;
    }

    private void CreateOptionsPanel()
    {
        _optionsPanel = new Panel();
        _optionsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _optionsPanel.IsVisible = false;
        _optionsPanel.AddToRoot();

        var optionsText = new TextRuntime();
        optionsText.X = 10;
        optionsText.Y = 10;
        optionsText.Text = "OPTIONS";
        _optionsPanel.AddChild(optionsText);

        var musicSlider = new Slider();
        musicSlider.Anchor(Gum.Wireframe.Anchor.Top);
        musicSlider.Visual.Y = 30f;
        musicSlider.Minimum = 0;
        musicSlider.Maximum = 1;
        musicSlider.Value = Core.Audio.SongVolume;
        musicSlider.SmallChange = .1;
        musicSlider.LargeChange = .2;
        musicSlider.ValueChanged += HandleMusicSliderValueChanged;
        musicSlider.ValueChangeCompleted += HandleMusicSliderValueChangeCompleted;
        _optionsPanel.AddChild(musicSlider);

        var sfxSlider = new Slider();
        sfxSlider.Anchor(Gum.Wireframe.Anchor.Top);
        sfxSlider.Visual.Y = 93;
        sfxSlider.Minimum = 0;
        sfxSlider.Maximum = 1;
        sfxSlider.Value = Core.Audio.SoundEffectVolume;
        sfxSlider.SmallChange = .1;
        sfxSlider.LargeChange = .2;
        sfxSlider.ValueChanged += HandleSfxSliderChanged;
        sfxSlider.ValueChangeCompleted += HandleSfxSliderChangeCompleted;
        _optionsPanel.AddChild(sfxSlider);

        _optionsBackButton = new Button();
        _optionsBackButton.Text = "BACK";
        _optionsBackButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsBackButton.X = -28f;
        _optionsBackButton.Y = -10f;
        _optionsBackButton.Click += HandleOptionsButtonBack;
        _optionsPanel.AddChild(_optionsBackButton);
    }

    private void HandleSfxSliderChanged(object sender, EventArgs args)
    {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the
        // track.

        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global sound effect volume to the value of the slider.;
        Core.Audio.SoundEffectVolume = (float)slider.Value;
    }

    private void HandleSfxSliderChangeCompleted(object sender, EventArgs e)
    {
        // Play the UI Sound effect so the player can hear the difference in audio.
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleMusicSliderValueChanged(object sender, EventArgs args)
    {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the
        // track.

        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global song volume to the value of the slider.
        Core.Audio.SongVolume = (float)slider.Value;
    }

    private void HandleMusicSliderValueChangeCompleted(object sender, EventArgs args)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleOptionsButtonBack(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Set the title panel to be visible.
        _titleScreenButtonsPanel.IsVisible = true;

        // Set the options panel to be invisible.
        _optionsPanel.IsVisible = false;

        // Give the options button on the title panel focus since we are coming
        // back from the options screen.
        _optionsButton.IsFocused = true;
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from
        // a different screen:
        GumService.Default.Root.Children.Clear();

        CreateTitlePanel();
        CreateOptionsPanel();
    }

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

        InitializeUI();
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

        // Load the sound effect to play when ui actions occur.
        _uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");
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

        GumService.Default.Update(gameTime);
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

        if (_titleScreenButtonsPanel.IsVisible)
        {
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

        GumService.Default.Draw();
    }
}
