using System;
using DwarfDownUnder.GameObjects;
using DwarfDownUnder.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;

namespace DwarfDownUnder.Scenes;

public class GameScene : Scene
{
    private enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    // Reference to the dwarf.
    private Dwarf _dwarf;

    // Reference to the bat.
    private Bat _bat;

    // Defines the tilemap to draw.
    private Tilemap _tilemap;

    // Defines the bounds of the room that the dwarf and bat are contained within.
    private Rectangle _roomBounds;

    // The sound effect to play when the dwarf eats a bat.
    private SoundEffect _collectSoundEffect;

    // Tracks the players score.
    private int _score;

    private GameSceneUI _ui;

    private GameState _state;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen.
        Core.ExitOnEscape = false;

        // Create the room bounds by getting the bounds of the screen then
        // setting it in by 1 tile at left, bottom and right, and 2 tiles at the top
        Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
        _roomBounds = new Rectangle(
            (int)_tilemap.TileWidth,
            (int)_tilemap.TileHeight * 2,
            screenBounds.Width - (int)(_tilemap.TileWidth * 2),
            screenBounds.Height - (int)(_tilemap.TileHeight * 3)
        );

        // Subscribe to the dwarf's BodyCollision event so that a game over
        // can be triggered when this event is raised.
        // _dwarf.BodyCollision += OnDwarfBodyCollision;

        // Create any UI elements from the root element created in previous
        // scenes.
        GumService.Default.Root.Children.Clear();

        // Initialize the user interface for the game scene.
        InitializeUI();

        // Initialize a new game to be played.
        InitializeNewGame();
    }

    private void InitializeUI()
    {
        // Clear out any previous UI element incase we came here
        // from a different scene.
        GumService.Default.Root.Children.Clear();

        // Create the game scene ui instance.
        _ui = new GameSceneUI();

        // Subscribe to the events from the game scene ui.
        _ui.ResumeButtonClick += OnResumeButtonClicked;
        _ui.RetryButtonClick += OnRetryButtonClicked;
        _ui.QuitButtonClick += OnQuitButtonClicked;
    }

    private void OnResumeButtonClicked(object sender, EventArgs args)
    {
        // Change the game state back to playing.
        _state = GameState.Playing;
    }

    private void OnRetryButtonClicked(object sender, EventArgs args)
    {
        // Player has chosen to retry, so initialize a new game.
        InitializeNewGame();
    }

    private void OnQuitButtonClicked(object sender, EventArgs args)
    {
        // Player has chosen to quit, so return back to the title scene.
        Core.ChangeScene(new TitleScene());
    }

    private void InitializeNewGame()
    {
        // Calculate the position for the dwarf, which will be at the center
        // tile of the tile map.
        Vector2 dwarfPos = new Vector2();
        dwarfPos.X = (_tilemap.Columns / 2) * _tilemap.TileWidth;
        dwarfPos.Y = (_tilemap.Rows / 2) * _tilemap.TileHeight;

        // Initialize the dwarf.
        _dwarf.Initialize(dwarfPos, _tilemap.TileWidth);

        // Initialize the bat.
        _bat.RandomizeVelocity();
        PositionBatAwayFromDwarf();

        // Reset the score.
        _score = 0;

        // Set the game state to playing.
        _state = GameState.Playing;
    }

    public override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file.
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        // Create the tilemap from the XML configuration file.
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(2.0f, 2.0f);

        // Create the animated sprite for the dwarf from the atlas.
        AnimatedSprite dwarfAnimation = atlas.CreateAnimatedSprite("dwarf-animation");
        dwarfAnimation.Scale = new Vector2(2.0f, 2.0f);

        // Create the dwarf.
        _dwarf = new Dwarf(dwarfAnimation);

        // Create the animated sprite for the bat from the atlas.
        AnimatedSprite batAnimation = atlas.CreateAnimatedSprite("bat-animation");
        batAnimation.Scale = new Vector2(2.0f, 2.0f);

        // Load the bounce sound effect for the bat.
        SoundEffect bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Create the bat.
        _bat = new Bat(batAnimation, bounceSoundEffect);

        // Load the collect sound effect.
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");
    }

    public override void Update(GameTime gameTime)
    {
        // Ensure the UI is always updated.
        _ui.Update(gameTime);

        // If the game is in a game over state, immediately return back
        // here.
        if (_state == GameState.GameOver)
        {
            return;
        }

        // If the pause button is pressed, toggle the pause state.
        if (GameController.Pause())
        {
            TogglePause();
        }

        // At this point, if the game is paused, just return back early.
        if (_state == GameState.Paused)
        {
            return;
        }

        // Update the dwarf.
        _dwarf.Update(gameTime);

        // Update the bat.
        _bat.Update(gameTime);

        // Perform collision checks.
        CollisionChecks();
    }

    private void CollisionChecks()
    {
        // Capture the current bounds of the dwarf and bat.
        Circle dwarfBounds = _dwarf.GetBounds();
        Circle batBounds = _bat.GetBounds();

        // FIrst perform a collision check to see if the dwarf is colliding with
        // the bat, which means the dwarf kills the bat.
        if (dwarfBounds.Intersects(batBounds))
        {
            // Move the bat to a new position away from the dwarf.
            PositionBatAwayFromDwarf();

            // Randomize the velocity of the bat.
            _bat.RandomizeVelocity();

            // Tell the dwarf to grow.
            // _dwarf.Grow();

            // Increment the score.
            _score += 100;

            // Update the score display on the UI.
            _ui.UpdateScoreText(_score);

            // Play the collect sound effect.
            Core.Audio.PlaySoundEffect(_collectSoundEffect);
        }

        // Next check if the dwarf is colliding with the wall by validating if
        // it is within the bounds of the room.  If it is outside the room
        // bounds, then it collided with a wall which triggers a game over.
        if (dwarfBounds.Top < _roomBounds.Top ||
        dwarfBounds.Bottom > _roomBounds.Bottom ||
        dwarfBounds.Left < _roomBounds.Left ||
        dwarfBounds.Right > _roomBounds.Right)
        {
            GameOver();
            return;
        }

        // Finally, check if the bat is colliding with a wall by validating if
        // it is within the bounds of the room.  If it is outside the room
        // bounds, then it collided with a wall, and the bat should bounce
        // off of that wall.
        if (batBounds.Top < _roomBounds.Top)
        {
            _bat.Bounce(Vector2.UnitY);
        }
        else if (batBounds.Bottom > _roomBounds.Bottom)
        {
            _bat.Bounce(-Vector2.UnitY);
        }

        if (batBounds.Left < _roomBounds.Left)
        {
            _bat.Bounce(Vector2.UnitX);
        }
        else if (batBounds.Right > _roomBounds.Right)
        {
            _bat.Bounce(-Vector2.UnitX);
        }
    }

    private void PositionBatAwayFromDwarf()
    {
        // Calculate the position that is in the center of the bounds
        // of the room.
        float roomCenterX = _roomBounds.X + _roomBounds.Width * 0.5f;
        float roomCenterY = _roomBounds.Y + _roomBounds.Height * 0.5f;
        Vector2 roomCenter = new Vector2(roomCenterX, roomCenterY);

        // Get the bounds of the dwarf and calculate the center position.
        Circle dwarfBounds = _dwarf.GetBounds();
        Vector2 dwarfCenter = new Vector2(dwarfBounds.X, dwarfBounds.Y);

        // Calculate the distance vector from the center of the room to the
        // center of the dwarf.
        Vector2 centerToDwarf = dwarfCenter - roomCenter;

        // Get the bounds of the bat.
        Circle batBounds =_bat.GetBounds();

        // Calculate the amount of padding we will add to the new position of
        // the bat to ensure it is not sticking to walls
        int padding = batBounds.Radius * 2;

        // Calculate the new position of the bat by finding which component of
        // the center to dwarf vector (X or Y) is larger and in which direction.
        Vector2 newBatPosition = Vector2.Zero;
        if (Math.Abs(centerToDwarf.X) > Math.Abs(centerToDwarf.Y))
        {
            // The dwarf is closer to either the left or right wall, so the Y
            // position will be a random position between the top and bottom
            // walls.
            newBatPosition.Y = Random.Shared.Next(
                _roomBounds.Top + padding,
                _roomBounds.Bottom - padding
            );

            if (centerToDwarf.X > 0)
            {
                // The dwarf is closer to the right side wall, so place the
                // bat on the left side wall.
                newBatPosition.X = _roomBounds.Left + padding;
            }
            else
            {
                // The dwarf is closer to the left side wall, so place the
                // bat on the right side wall.
                newBatPosition.X = _roomBounds.Right - padding * 2;
            }
        }
        else
        {
            // The dwarf is closer to either the top or bottom wall, so the X
            // position will be a random position between the left and right
            // walls.
            newBatPosition.X = Random.Shared.Next(
                _roomBounds.Left + padding,
                _roomBounds.Right - padding
            );

            if (centerToDwarf.Y > 0)
            {
                // The dwarf is closer to the top wall, so place the bat on the
                // bottom wall.
                newBatPosition.Y = _roomBounds.Top + padding;
            }
            else
            {
                // The dwarf is closer to the bottom wall, so place the bat on
                // the top wall.
                newBatPosition.Y = _roomBounds.Bottom - padding * 2;
            }
        }

        // Assign the new bat position.
        _bat.Position = newBatPosition;
    }

    // private void OnDwarfBodyCollision(object sender, EventArgs args)
    // {
    //     GameOver();
    // }

    private void TogglePause()
    {
        if (_state == GameState.Paused)
        {
            // We're now unpausing the game, so hide the pause panel.
            _ui.HidePausePanel();

            // And set the state back to playing.
            _state = GameState.Playing;
        }
        else
        {
            // We're now pausing the game, so show the pause panel.
            _ui.ShowPausePanel();

            // And set the state to paused.
            _state = GameState.Paused;
        }
    }

    private void GameOver()
    {
        // Show the game over panel.
        _ui.ShowGameOverPanel();

        // Set the game state to game over.
        _state = GameState.GameOver;
    }

    public override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the tilemap
        _tilemap.Draw(Core.SpriteBatch);

        // Draw the dwarf.
        _dwarf.Draw();

        // Draw the bat.
        _bat.Draw();

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();

        // Draw the UI.
        _ui.Draw();
    }
}
