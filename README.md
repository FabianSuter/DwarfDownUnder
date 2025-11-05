# DwarfDownUnder
## Info
This project serves as an assignment for the *Advanced C#* class at [University West](https://www.hv.se/) during the fall semester of 2025.

## How to Build & Run
- Install MonoGame on your device with the first two steps from [Getting Started](https://docs.monogame.net/articles/getting_started/index.html)
- Clone this repository to your device
- Install MonoGame.Extended on your device with the [Installation Guide](https://www.monogameextended.net/docs/getting-started/installation-monogame/), including the optional step with MGCB

After these steps you should be able to build and run the project.

**Important:** *Clone the repo **before** doing the Extended part*

## Declarations & Sources

### General
- This game was developed with the [MonoGame framework](https://monogame.net/)
- The majority of the MonoGameLibrary is from one of the [tutorials](https://docs.monogame.net/articles/tutorials/index.html)
- To handle Tiled maps (see below) and for other helpful additions [MonoGame Extended](https://docs.monogame.net/articles/tutorials/index.html) was also included

### Images
- Tileset as an asset pack from [lyime](https://lyime.itch.io/)
    - Resized background stuff to 32x32 px (originally 16x16)
    - World building done in [Tiled editor](https://www.mapeditor.org/)
- Background in title done by myself with [LibreSprite](https://libresprite.github.io/#!/)
- UI font png done with Hiero (see below)

### Audio
- Sound Effects as a free bundle from [Kronbits](https://kronbits.itch.io/retrosfx)
- Background song theme from [Luis Zuno](https://soundcloud.com/ansimuz/tracks), currently *Exploration*

### Fonts
- ~~Sprite font from [Joël Carrouché](https://www.1001fonts.com/users/joelcarrouche/), currently *Norse Regular*~~
- Sprite font *04b_30.ttf* from [dafont.com](https://www.dafont.com/04b-30.font) 
    - I originally wanted to change the font to Norse Regular but it causes crashes so far
    - Any other font crashes apparently too, Exception throws "*fontname*_0.xnb could not be found". Searching for error hasn't helped yet.
- Fonts in UI converted from *.ttf* to *.fnt* and *.png* with [Hiero](https://docs.flatredball.com/gum/gum-tool/gum-elements/text/use-custom-font#creating-fonts-with-hiero)

### UI
- This project uses the [Gum layout engine](https://docs.flatredball.com/gum), namely version `2025.8.3.3`