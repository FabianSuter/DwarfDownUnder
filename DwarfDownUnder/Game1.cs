using DwarfDownUnder.Scenes;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary;

namespace DwarfDownUnder;

public class Game1 : Core
{
    // Background music
    private Song _themeSong;

    public Game1() : base("Dwarf Down Under", 1280, 800, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();

        // Start playing background theme
        Audio.PlaySong(_themeSong);

        // Start the game with the title scene
        ChangeScene(new TitleScene());
    }

    protected override void LoadContent()
    {
        // Load the background theme music.
        _themeSong = Content.Load<Song>("audio/theme");
    }
}
