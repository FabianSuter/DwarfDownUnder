using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;

namespace MonoGameLibrary.Graphics;

public class TileCollision
{
    // The tile layer that contains the collision data.
    private TiledMapTileLayer _collisionLayer;

    /// <summary>
    /// Creates a new TileCollision object that uses the specified tile layer from the given map
    /// </summary>
    /// <param name="map">The Tiled map that contains the collision layer.</param>
    public TileCollision(TiledMap map)
    {
        _collisionLayer = map.GetLayer<TiledMapTileLayer>("collisions");
    }

    /// <summary>
    /// Checks if target position is not blocked by collision tile
    /// </summary>
    /// <param name="targetPos">The target position to check for collision.</param>
    public bool CanMoveTo(Vector2 targetPos)
    {
        var TileX = (ushort)(targetPos.X / _collisionLayer.TileWidth);
        var TileY = (ushort)(targetPos.Y / _collisionLayer.TileHeight);
        if (_collisionLayer.TryGetTile(TileX, TileY, out TiledMapTile? tile))
        {
            if (tile.Value.GlobalIdentifier != 0) { return false; }
        }
        return true;
    }
}