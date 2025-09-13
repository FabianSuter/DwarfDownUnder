using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace DwarfDownUnder;

public class Game1 : Core
{
    // MonoGame logo (temporary)
    private Texture2D _logo;

    public Game1() : base("Dwarf Down Under", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        // Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // use this.Content to load your game content here

        _logo = Content.Load<Texture2D>("images/logo");

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin();

        // Draw the logo texture
        SpriteBatch.Draw(_logo, Vector2.Zero, Color.White);

        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
