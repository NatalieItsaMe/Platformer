using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System.Diagnostics;
using System.Linq;

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
                    var offset = (innerObject.Size - obj.Size).ToVector2() / 2f + innerObject.Position;
                    var shape = CreateShapeFromTiledObject(innerObject, offset);
                    var fixture = body.CreateFixture(shape);
                    ApplyTiledPropertiesToFixture(innerObject.Properties, ref fixture);
                }
            else
            {
                var shape = CreateShapeFromTiledObject(obj);
                var fixture = body.CreateFixture(shape);
                ApplyTiledPropertiesToFixture(obj.Properties, ref fixture);
            }
        }

        private static void ApplyTiledPropertiesToFixture(TiledMapProperties properties, ref Fixture fixture)
        {
            if (properties.TryGetValue(nameof(Fixture.Restitution), out string restitutionProperty))
                fixture.Restitution = float.Parse(restitutionProperty);
            if (properties.TryGetValue(nameof(Fixture.Friction), out string frictionProperty))
                fixture.Friction = float.Parse(frictionProperty);
            if (properties.TryGetValue(nameof(Fixture.IsSensor), out string isSensorProperty))
                fixture.IsSensor = bool.Parse(isSensorProperty);
        }

        private Shape CreateShapeFromTiledObject(TiledMapObject obj, Vector2 offset = new())
        {
            var density = obj.Properties.TryGetValue("Density", out TiledMapPropertyValue densityProperty)
                ? float.TryParse(densityProperty.Value, out float densityValue) ? densityValue : 1.0f
                : 1.0f;

            if (obj is TiledMapPolygonObject polygon)
            {
                //Concave polygons are not supported
                var vertices = new Vertices();
                vertices.AddRange(polygon.Points.Select(p => (p + offset) * _scale));
                var shape = new PolygonShape(vertices, density);
                return shape;
            }
            if (obj is TiledMapPolylineObject polyline)
            {
                Vector2 start = (polyline.Points[0] + offset) * _scale;
                Vector2 end = (polyline.Points[1] + offset) * _scale;
                var shape = new EdgeShape(start, end);
                //v0 and v3 are "ghost vertices", if the segment were to continue in either direction
                return shape;
            }
            if (obj is TiledMapEllipseObject ellipse)
            {
                var radius = ellipse.Radius * _scale;
                Debug.WriteLineIf(radius.X != radius.Y, "Ellipses are not supported!");

                var shape = new CircleShape(radius.X, density)
                {
                    Position = offset * _scale,
                    Radius = radius.X
                };

                return shape;
            }

            Vector2 h = obj.Size.ToVector2() * _scale / 2f;
            var vertices1 = new Vertices(4)
            {
                new(-h.X, -h.Y),
                new(-h.X, h.Y),
                new(h.X, h.Y),
                new(h.X, -h.Y)
            };

            var box = new PolygonShape(vertices1, density);

            return box;
        }
    }
}
