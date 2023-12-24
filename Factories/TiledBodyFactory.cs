using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using MonoGame.Extended.Tiled;
using System;
using System.Linq;
using System.Numerics;

namespace Platformer.Factories
{
    internal class TiledBodyFactory
    {
        private World Box2DWorld { get; }
        private TiledMap TiledMap { get; }

        public TiledBodyFactory(World box2dWorld, TiledMap tiledMap)
        {
            Box2DWorld = box2dWorld;
            TiledMap = tiledMap;
        }

        public Body CreateBodyFromTiledObject(TiledMapObject obj)
        {
            Vector2 scale = TiledMap.GetScale();
            obj.Properties.TryGetValue("BodyType", out string bodyType);
            BodyDef bodyDef = new()
            {
                Position = (obj.Position.ToNumerics() + obj.Size.ToNumerics() / 2) * scale,
                Angle = obj.Rotation * (float)Math.PI / 180f,
                BodyType = bodyType switch
                {
                    "Kinematic" => BodyType.KinematicBody,
                    "Dynamic" => BodyType.DynamicBody,
                    _ => BodyType.StaticBody
                }
            };
            if (obj.Properties.ContainsKey("AngularDamping"))
                bodyDef.AngularDamping = float.Parse(obj.Properties["AngularDamping"]);
            if (obj.Properties.ContainsKey("FixedRotation"))
                bodyDef.FixedRotation = bool.Parse(obj.Properties["FixedRotation"]);
            if (obj.Properties.ContainsKey("Bullet"))
                bodyDef.Bullet = bool.Parse(obj.Properties["Bullet"]);

            Body body = Box2DWorld.CreateBody(bodyDef);

            if (obj is TiledMapTileObject tileObject && tileObject.Tile != null)
            {
                foreach (var innerObject in tileObject.Tile.Objects)
                {
                    //Tiled reads object data from the top left
                    //Box2d builds fixtures from the center
                    //offset points from the topleft to the center
                    var offset = (innerObject.Size - obj.Size).ToNumerics() / 2 + innerObject.Position.ToNumerics();
                    FixtureDef fixture = new();
                    fixture.Shape = CreateShapeFromTiledObject(innerObject, offset);

                    SetPropertyValues(obj, ref fixture);
                    body.CreateFixture(fixture);
                }
            }
            else
            {
                FixtureDef fixture = new();
                fixture.Shape = CreateShapeFromTiledObject(obj);

                SetPropertyValues(obj, ref fixture);
                body.CreateFixture(fixture);
            }

            return body;
        }

        private Shape CreateShapeFromTiledObject(TiledMapObject obj, Vector2 offset = new())
        {
            var scale = TiledMap.GetScale();
            if (obj is TiledMapPolygonObject polygon)
            {
                //Concave polygons are not supported
                PolygonShape shape = new();
                shape.Set(polygon.Points.Select(p => (p.ToNumerics() + offset) * scale).ToArray());
                return shape;
            }
            else if (obj is TiledMapPolylineObject polyline)
            {
                EdgeShape shape = new();
                Vector2 start = (polyline.Points[0].ToNumerics() + offset) * scale;
                Vector2 end = (polyline.Points[1].ToNumerics() + offset) * scale;
                //v0 and v3 are "ghost vertices", if the segment were to continue in either direction
                shape.SetOneSided(start, start, end, end);

                return shape;
            }
            else if (obj is TiledMapEllipseObject ellipse)
            {
                CircleShape shape = new()
                {
                    Position = offset * scale,
                    Radius = (ellipse.Radius * scale).Length()
                };

                return shape;
            }

            PolygonShape box = new();
            Vector2 h = obj.Size.ToNumerics() * scale / 2f;
            box.SetAsBox(h.X, h.Y, offset * scale, 0);

            return box;
        }

        private static void SetPropertyValues(TiledMapObject obj, ref FixtureDef fixture)
        {
            if (obj.Properties.ContainsKey("Friction"))
                fixture.Friction = float.Parse(obj.Properties["Friction"]);
            if (obj.Properties.ContainsKey("Density"))
                fixture.Density = float.Parse(obj.Properties["Density"]);
            if (obj.Properties.ContainsKey("Restitution"))
                fixture.Restitution = float.Parse(obj.Properties["Restitution"]);
            if (obj.Properties.ContainsKey("IsSensor"))
                fixture.IsSensor = bool.Parse(obj.Properties["IsSensor"]);
        }
    }
}
