using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DwarfDownUnder;

public class Game1 : Core
{
    // Sprite of dwarf
    private AnimatedSprite _dwarf;

    // Track position of dwarf
    private Vector2 _dwarfPosition;

    // Speed multiplier when moving
    private const float MOVE_SPEED = 5.0f;

    public Game1() : base("Dwarf Down Under", 1280, 720, false)
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
        _dwarf.Scale = new Vector2(3.0f, 3.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Update dwarf sprite
        _dwarf.Update(gameTime);

        // Check for keyboard input
        CheckKeyboardInput();

        base.Update(gameTime);
    }

        private void CheckKeyboardInput()
        {
            // Get the state of keyboard input
            KeyboardState keyboardState = Keyboard.GetState();

            // If the space key is held down, the movement speed increases by 1.5
            float speed = MOVE_SPEED;
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                speed *= 1.5f;
            }

            // If the W or Up keys are down, move the slime up on the screen.
            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                _dwarfPosition.Y -= speed;
            }

            // if the S or Down keys are down, move the slime down on the screen.
            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                _dwarfPosition.Y += speed;
            }

            // If the A or Left keys are down, move the slime left on the screen.
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                _dwarfPosition.X -= speed;
            }

            // If the D or Right keys are down, move the slime right on the screen.
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                _dwarfPosition.X += speed;
            }
        }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw dwarf sprite
        _dwarf.Draw(SpriteBatch, _dwarfPosition);

        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
