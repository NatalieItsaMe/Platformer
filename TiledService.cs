using MonoGame.Extended.Tiled;
using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Box2DSharp.Dynamics;
using Platformer.Component;
using Box2DSharp.Collision.Shapes;

namespace Platformer
{
    public class TiledService
    {
        //public TiledMap Map { get; }
        //public Dictionary<int, TiledTileset> Tilesets { get; }
        //public Dictionary<int, Texture2D> TilesetTextures { get; } = new Dictionary<int, Texture2D>();

        //private int objectLayerIndex => Array.IndexOf(Map.Layers, Map.Layers.Single(layer => layer.type == TiledLayerType.ObjectLayer));

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

        //public TiledService(string tiledMapPath)
        //{
        //    // For loading maps in XML format
        //    Map = new TiledMap(tiledMapPath);
        //    Tilesets = Map.GetTiledTilesets(tiledMapPath); // DO NOT forget the / at the end
        //}

        //public TiledLayer[] GetObjectLayers() => Map.Layers.Where(layer => layer.type == TiledLayerType.ObjectLayer).ToArray();

        //public void DrawBackgroundLayers(SpriteBatch spriteBatch)
        //{
        //    for(int i = 0; i < objectLayerIndex; i ++)
        //    {
        //        DrawLayer(spriteBatch, Map.Layers[i]);
        //    }
        //}

        //public void DrawForegroundLayers(SpriteBatch spriteBatch)
        //{
        //    for(int i = objectLayerIndex; i < Map.Layers.Length; i++)
        //    {
        //        DrawLayer(spriteBatch, Map.Layers[i]);
        //    }
        //}

        //public void DrawLayer(SpriteBatch spriteBatch, TiledLayer layer)
        //{
        //    switch (layer.type)
        //    {
        //        case TiledLayerType.ObjectLayer:
        //            break;
        //        case TiledLayerType.TileLayer:
        //            DrawTileLayer(spriteBatch, layer);
        //            break;
        //        case TiledLayerType.ImageLayer:
        //            DrawImageLayer(spriteBatch, layer);
        //            break;
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
        //            var rect = GetSourceRect(mapTileset, tileset, gid);

        //            // Create destination and source rectangles
        //            var source = new Microsoft.Xna.Framework.Rectangle(rect.x, rect.y, rect.width, rect.height);
        //            var destination = new Microsoft.Xna.Framework.Rectangle(x, y, 1, 1);

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
        //                    rotation = System.Math.PI * .5f;
        //                    destination.X += Map.TileWidth;
        //                    break;

        //                case Transforms.Rotate_180:
        //                    rotation = System.Math.PI;
        //                    destination.X += Map.TileWidth;
        //                    destination.Y += Map.TileHeight;
        //                    break;

        //                case Transforms.Rotate_270:
        //                    rotation = System.Math.PI * 3 / 2;
        //                    destination.Y += Map.TileHeight;
        //                    break;

        //                case Transforms.Rotate_90AndFlip_H:
        //                    effects = SpriteEffects.FlipHorizontally;
        //                    rotation = System.Math.PI * .5f;
        //                    destination.X += Map.TileWidth;
        //                    break;

        //                default:
        //                    break;
        //            }

        //            // Render sprite at position tileX, tileY using the rect
        //            spriteBatch.Draw(tilesetTexture, destination, source, Microsoft.Xna.Framework.Color.White, (float)rotation, Vector2.Zero, effects, 0);
        //        }
        //    }
        //}

        //private TiledSourceRect GetSourceRect(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
        //{
        //    int id = gid - mapTileset.firstgid;
        //    int y = (id / tileset.Columns);
        //    int x = id % tileset.Columns;

        //    TiledSourceRect rect = new();
        //    rect.x = x * (tileset.TileWidth + tileset.Spacing) + tileset.Margin;
        //    rect.y = y * (tileset.TileHeight + tileset.Spacing) + tileset.Margin;
        //    rect.width = tileset.TileWidth;
        //    rect.height = tileset.TileHeight;

        //    return rect;
        //}

        //private void DrawImageLayer(SpriteBatch spriteBatch, TiledLayer layer)
        //{

        //}

        //internal void AddObjectLayersToPhysicsSystem(MonoGame.Extended.Entities.World world, PhysicsSystem physicsSystem)
        //{
        //    var objectLayers = GetObjectLayers();
        //    foreach (var objectLayer in objectLayers)
        //    {
        //        foreach (var obj in objectLayer.objects)
        //        {
        //            var entity = world.CreateEntity();

        //            switch (obj.type)
        //            {
        //                case "Body":
        //                    BodyDef bodyDef = new()
        //                    {
        //                        Position = new Vector2(obj.x / Map.TileWidth, obj.y / Map.TileHeight)
        //                    };

        //                    Shape shape;
        //                    if (obj.polygon != null)
        //                    {
        //                        PolygonShape polygon = new();
        //                        int count = obj.polygon.points.Length / 2;
        //                        Vector2[] vertices = new Vector2[count];
        //                        for (int i = 0; i < count; i ++)
        //                        {
        //                            vertices[i] = new Vector2(obj.polygon.points[i * 2] / Map.TileWidth, obj.polygon.points[i * 2 + 1] / Map.TileHeight);
        //                        }
        //                        polygon.Set(vertices, count);
        //                        shape = polygon;
        //                    }
        //                    else if (obj.ellipse != null || obj.point != null)
        //                    {
        //                        if (obj.width != obj.height)
        //                            throw new Exception($"Ellipses are not supported. Object with ID {obj.id} make width = height");
        //                        CircleShape circle = new();
        //                        circle.Radius = obj.width / Map.TileWidth;
        //                        shape = circle;
        //                    }
        //                    else
        //                    {
        //                        PolygonShape polygon = new PolygonShape();
        //                        polygon.SetAsBox(obj.width / Map.TileWidth, obj.height / Map.TileHeight);
        //                        shape = polygon;
        //                    }

        //                    FixtureDef definition = new();

        //                    foreach (var property in obj.properties)
        //                    {
        //                        switch (property.type)
        //                        {
        //                            case TiledPropertyType.String:
        //                                string s = property.value;
        //                                switch (property.name)
        //                                {
        //                                    case "BodyType":
        //                                        bodyDef.BodyType = s switch
        //                                        {
        //                                            "Kinematic" => BodyType.KinematicBody,
        //                                            "Dynamic" => BodyType.DynamicBody,
        //                                            _ => BodyType.StaticBody
        //                                        };
        //                                        break;
        //                                }
        //                                break;
        //                            case TiledPropertyType.Bool:
        //                                bool b = bool.Parse(property.value);
        //                                switch (property.name)
        //                                {
        //                                    case "IsSensor":
        //                                        definition.IsSensor = b;
        //                                        break;
        //                                }
        //                                break;
        //                            case TiledPropertyType.Color:
        //                                break;
        //                            case TiledPropertyType.File:
        //                                break;
        //                            case TiledPropertyType.Float:
        //                                float f = float.Parse(property.value);
        //                                switch (property.name)
        //                                {
        //                                    case "Density":
        //                                        definition.Density = f;
        //                                        break;
        //                                    case "Friction":
        //                                        definition.Friction = f;
        //                                        break;
        //                                    case "Restitution":
        //                                        definition.Restitution = f;
        //                                        break;
        //                                }
        //                                break;
        //                            case TiledPropertyType.Int:
        //                                break;
        //                            case TiledPropertyType.Object:
        //                                //TODO find the object in the pile of objects created
        //                                break;
        //                        }
        //                    }

        //                    Body body = physicsSystem.CreateBody(bodyDef);
        //                    body.CreateFixture(definition);
        //                    body.UserData = entity.Id;

        //                    entity.Attach(body);
        //                    break;
        //            }
        //            if(obj.name == "Player")
        //            {
        //                entity.Attach(new KeyboardMapping());
        //                entity.Attach(new CameraTarget(offset: new(0, -1), zoom: 16));
        //            }
        //        }
        //    }
        //}
    }
}