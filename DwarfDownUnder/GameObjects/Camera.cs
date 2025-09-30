using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using MonoGameLibrary;

namespace DwarfDownUnder.GameObjects;

public class Camera
{
    private OrthographicCamera _camera;
    private Vector2 _cameraPosition;

    public Camera(Vector2 position)
    {
        BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Core.Window, Core.GraphicsDevice, 640, 400);
        _camera = new OrthographicCamera(viewportAdapter);

        _cameraPosition = position;
        _camera.ZoomIn(1f);
        _camera.LookAt(_cameraPosition);
    }

    private Vector2 HandleInput()
    {
        Vector2 movementDir = Vector2.Zero;

        // Get current input
        if (GameController.MoveDown())
        {
            movementDir += Vector2.UnitY;
        }
        else if (GameController.MoveUp())
        {
            movementDir -= Vector2.UnitY;
        }
        else if (GameController.MoveLeft())
        {
            movementDir -= Vector2.UnitX;
        }
        else if (GameController.MoveRight())
        {
            movementDir += Vector2.UnitX;
        }

        return movementDir;
    }

    private void MoveCamera(GameTime gameTime)
    {
        float speed = 200f;
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 movementDir = HandleInput();

        _cameraPosition += movementDir * speed * deltaTime;
    }

    public void Update(GameTime gameTime, Vector2 position)
    {
        MoveCamera(gameTime);
        _camera.LookAt(position);
    }

    public Matrix GetViewMatrix()
    {
        return _camera.GetViewMatrix();
    }
}