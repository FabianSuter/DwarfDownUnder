using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DwarfDownUnder;

public class Game1 : Core
{
    // TextureRegion of dwarf
    private TextureRegion _dwarf;

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
        Texture2D atlasTexture = Content.Load<Texture2D>("images/atlas");

        // Create TextureAtlas
        TextureAtlas atlas = new TextureAtlas(atlasTexture);

        // Add dwarf region to atlas
        atlas.AddRegion("dwarf", 0, 192, 48, 48);

        // Get dwarf region from atlas
        _dwarf = atlas.GetRegion("dwarf");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw dwarf texture
        _dwarf.Draw(SpriteBatch, Vector2.Zero, Color.White, 0.0f, Vector2.One, 4.0f, SpriteEffects.None, 0.0f);

        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
