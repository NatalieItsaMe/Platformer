using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Platformer.Factories
{
    internal class TiledBodyFactory
    {
        private Vector2 Scale { get; }

        public TiledBodyFactory(Vector2 scale)
        {
            Scale = scale;
        }

        public void BuildFixturesFromTiledObject(TiledMapTileObject obj, Body body)
        {
            foreach (var innerObject in obj.Tile.Objects)
            {
                //offset points from the topleft (Tiled) to the center (Box2D)
                var offset = (innerObject.Size - obj.Size).ToNumerics() / 2f + innerObject.Position.ToNumerics();
                FixtureDef fixture = CreateFixtureDefFromTiledObject(innerObject, offset);
                body.CreateFixture(fixture);
            }
        }

        public void BuildFixturesFromTiledObject(TiledMapObject obj, Body body)
        {
            FixtureDef fixture = CreateFixtureDefFromTiledObject(obj);
            body.CreateFixture(fixture);
        }

        private FixtureDef CreateFixtureDefFromTiledObject(TiledMapObject obj, Vector2 offset = new())
        {
            obj.Properties.TryGetValue(nameof(FixtureDef), out string fixtureProperty);
            FixtureDef fixture = string.IsNullOrEmpty(fixtureProperty) ? new()
                : JsonConvert.DeserializeObject<FixtureDef>(fixtureProperty);
            fixture.Shape = CreateShapeFromTiledObject(obj, offset);
            return fixture;
        }

        private Shape CreateShapeFromTiledObject(TiledMapObject obj, Vector2 offset = new())
        {
            if (obj is TiledMapPolygonObject polygon)
            {
                //Concave polygons are not supported
                PolygonShape shape = new();
                shape.Set(polygon.Points.Select(p => (p.ToNumerics() + offset) * Scale).ToArray());
                return shape;
            }
            else if (obj is TiledMapPolylineObject polyline)
            {
                EdgeShape shape = new();
                Vector2 start = (polyline.Points[0].ToNumerics() + offset) * Scale;
                Vector2 end = (polyline.Points[1].ToNumerics() + offset) * Scale;
                //v0 and v3 are "ghost vertices", if the segment were to continue in either direction
                shape.SetOneSided(start, start, end, end);

                return shape;
            }
            else if (obj is TiledMapEllipseObject ellipse)
            {
                var size = ellipse.Radius * Scale;
                Debug.WriteLineIf(size.X != size.Y, "Ellipses are not supported!");

                CircleShape shape = new()
                {
                    Position = offset * Scale,
                    Radius = size.X
                };

                return shape;
            }

            PolygonShape box = new();
            Vector2 h = obj.Size.ToNumerics() * Scale / 2f;
            box.SetAsBox(h.X, h.Y, offset * Scale, 0);

            return box;
        }
    }
}
