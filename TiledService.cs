using MonoGame.Extended.Tiled;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Platformer
{
    public class TiledService
    {
        //public TiledMap Map { get; }
        //public Dictionary<int, TiledTileset> Tilesets { get; }
        //public Dictionary<int, Texture2D> TilesetTextures { get; }

        //[Flags]
        //enum Transforms
        //{
        //    None = 0,
        //    Flip_H = 1 << 0,
        //    Flip_V = 1 << 1,
        //    Flip_D = 1 << 2,

        //    Rotate_90 = Flip_D | Flip_H,
        //    Rotate_180 = Flip_H | Flip_V,
        //    Rotate_270 = Flip_V | Flip_D,

        //    Rotate_90AndFlip_H = Flip_H | Flip_V | Flip_D,
        //}

        //public TiledService(string tiledMapPath, ContentManager Content)
        //{
        //    // For loading maps in XML format
        //    Map = new TiledMap(tiledMapPath);
        //    Tilesets = Map.GetTiledTilesets(Path.GetFullPath(tiledMapPath)); // DO NOT forget the / at the end
        //    TilesetTextures = new Dictionary<int, Texture2D>();
        //    foreach (var tileset in Tilesets)
        //    {
        //        TilesetTextures.Add(tileset.Key, Content.Load<Texture2D>(Path.GetFileNameWithoutExtension(tileset.Value.Image.source)));
        //    }
        //}

        //public TiledLayer[] GetObjectLayers() => Map.Layers.Where(layer => layer.type == TiledLayerType.ObjectLayer).ToArray();

        //public void DrawTileLayers(SpriteBatch spriteBatch)
        //{
        //    foreach (TiledLayer layer in Map.Layers)
        //    {
        //        switch (layer.type)
        //        {
        //            case TiledLayerType.ObjectLayer:
        //                continue;
        //            case TiledLayerType.TileLayer:
        //                DrawTileLayer(spriteBatch, layer);
        //                break;
        //            case TiledLayerType.ImageLayer:
        //                DrawImageLayer(spriteBatch, layer);
        //                break;
        //        }
        //    }
        //}

        //private void DrawTileLayer(SpriteBatch spriteBatch, TiledLayer layer)
        //{
        //    for (var y = 0; y < layer.height; y++)
        //    {
        //        for (var x = 0; x < layer.width; x++)
        //        {
        //            var index = (y * layer.width) + x; // Assuming the default render order is used which is from right to bottom
        //            var gid = layer.data[index]; // The tileset tile index
        //            var tileX = (x * Map.TileWidth);
        //            var tileY = (y * Map.TileHeight);

        //            // Gid 0 is used to tell there is no tile set
        //            if (gid == 0)
        //            {
        //                continue;
        //            }

        //            // Helper method to fetch the right TieldMapTileset instance. 
        //            // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property.
        //            var mapTileset = Map.GetTiledMapTileset(gid);

        //            // Retrieve the actual tileset based on the firstgid property of the connection object we retrieved just now
        //            var tileset = Tilesets[mapTileset.firstgid];
        //            var tilesetTexture = TilesetTextures[mapTileset.firstgid];

        //            // Use the connection object as well as the tileset to figure out the source rectangle.
        //            var rect = Map.GetSourceRect(mapTileset, tileset, gid);

        //            // Create destination and source rectangles
        //            var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
        //            var destination = new Rectangle(tileX, tileY, Map.TileWidth, Map.TileHeight);

        //            // You can use the helper methods to get information to handle flips and rotations
        //            Transforms tileTransforms = Transforms.None;
        //            if (Map.IsTileFlippedHorizontal(layer, x, y)) tileTransforms |= Transforms.Flip_H;
        //            if (Map.IsTileFlippedVertical(layer, x, y)) tileTransforms |= Transforms.Flip_V;
        //            if (Map.IsTileFlippedDiagonal(layer, x, y)) tileTransforms |= Transforms.Flip_D;

        //            SpriteEffects effects = SpriteEffects.None;
        //            double rotation = 0f;
        //            switch (tileTransforms)
        //            {
        //                case Transforms.Flip_H: effects = SpriteEffects.FlipHorizontally; break;
        //                case Transforms.Flip_V: effects = SpriteEffects.FlipVertically; break;

        //                case Transforms.Rotate_90:
        //                    rotation = Math.PI * .5f;
        //                    destination.X += Map.TileWidth;
        //                    break;

        //                case Transforms.Rotate_180:
        //                    rotation = Math.PI;
        //                    destination.X += Map.TileWidth;
        //                    destination.Y += Map.TileHeight;
        //                    break;

        //                case Transforms.Rotate_270:
        //                    rotation = Math.PI * 3 / 2;
        //                    destination.Y += Map.TileHeight;
        //                    break;

        //                case Transforms.Rotate_90AndFlip_H:
        //                    effects = SpriteEffects.FlipHorizontally;
        //                    rotation = Math.PI * .5f;
        //                    destination.X += Map.TileWidth;
        //                    break;

        //                default:
        //                    break;
        //            }

        //            // Render sprite at position tileX, tileY using the rect
        //            spriteBatch.Draw(tilesetTexture, destination, source, Color.White, (float)rotation, Vector2.Zero, effects, 0);
        //        }
        //    }
        //}

        //private void DrawImageLayer(SpriteBatch spriteBatch, TiledLayer layer)
        //{

        //}
    }
}