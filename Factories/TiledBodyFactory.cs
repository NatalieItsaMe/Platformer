using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Platformer.Factories
{
    internal class TiledBodyFactory(Vector2 scale)
    {
        private readonly Vector2 _scale = scale;

        public void BuildFixturesFromTiledObject(TiledMapObject obj, Body body)
        {
            if (obj is TiledMapTileObject tileObject && tileObject.Tile != null)
                foreach (var innerObject in tileObject.Tile.Objects)
                {
                    //offset points from the topleft (Tiled) to the center (Box2D)
                    var offset = (innerObject.Size - obj.Size).ToNumerics() / 2f + innerObject.Position.ToNumerics();
                    FixtureDef fixture = CreateFixtureDefFromTiledObject(innerObject, offset);
                    body.CreateFixture(fixture);
                }
            else
            {
                FixtureDef fixture = CreateFixtureDefFromTiledObject(obj);
                body.CreateFixture(fixture);
            }
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
                Vector2[] vertices = [.. polygon.Points.Select(p => (p.ToNumerics() + offset) * _scale)];
                shape.Set(vertices);
                return shape;
            }
            else if (obj is TiledMapPolylineObject polyline)
            {
                EdgeShape shape = new();
                Vector2 start = (polyline.Points[0].ToNumerics() + offset) * _scale;
                Vector2 end = (polyline.Points[1].ToNumerics() + offset) * _scale;
                //v0 and v3 are "ghost vertices", if the segment were to continue in either direction
                shape.SetOneSided(start, start, end, end);

                return shape;
            }
            else if (obj is TiledMapEllipseObject ellipse)
            {
                var size = ellipse.Radius * _scale;
                Debug.WriteLineIf(size.X != size.Y, "Ellipses are not supported!");

                CircleShape shape = new()
                {
                    Position = offset * _scale,
                    Radius = size.X
                };

                return shape;
            }

            PolygonShape box = new();
            Vector2 h = obj.Size.ToNumerics() * _scale / 2f;
            box.SetAsBox(h.X, h.Y, offset * _scale, 0);

            return box;
        }
    }
}
