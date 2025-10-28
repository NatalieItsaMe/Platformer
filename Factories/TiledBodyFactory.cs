using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Platformer.Systems;
using System;
using System.Diagnostics;
using System.Linq;

namespace Platformer.Factories
{
    internal class TiledBodyFactory(PhysicsSystem physicsSystem)
    {
        private Vector2 _scale;

        internal void SetTiledMap(TiledMap tiledMap)
        {
            _scale = tiledMap.GetScale();
        }

        public Body BuildBodyFromMapObject(TiledMapObject mapObject)
        {
            var properties = mapObject is TiledMapTileObject
                ? ((TiledMapTileObject)mapObject).Tile.Properties
                : mapObject.Properties;

            Vector2 position = (mapObject.Position.ToPoint() + mapObject.Size / 2f) * _scale;
            var rotation = mapObject.Rotation * (float)Math.PI / 180f;
            var bodyType = properties.TryGetValue(nameof(BodyType), out TiledMapPropertyValue bodyTypeString)
                ? Enum.Parse<BodyType>(bodyTypeString)
                : BodyType.Static;

            Body body = physicsSystem.CreateBody(position, rotation, bodyType);

            if (properties.TryGetValue(nameof(Body.FixedRotation), out string fixedRotation))
                body.FixedRotation = bool.Parse(fixedRotation);

            if (properties.TryGetValue(nameof(Body.AngularDamping), out string angularDamping))
                body.AngularDamping = float.Parse(angularDamping);

            if (mapObject is TiledMapTileObject tileObject)
                BuildFixturesFromTileObject(tileObject, body);
            else
                BuildFixturesFromMapObject(mapObject, body);
            return body;
        }

        public void BuildFixturesFromMapObject(TiledMapObject mapObject, Body body)
        {
            var shape = CreateShapeFromTiledObject(mapObject);
            var fixture = body.CreateFixture(shape);
            ApplyTiledPropertiesToFixture(mapObject.Properties, ref fixture);
        }

        public void BuildFixturesFromTileObject(TiledMapTileObject tileObject, Body body)
        {
            foreach (var innerObject in tileObject.Tile.Objects)
            {
                //offset points from the topleft (Tiled) to the center (Box2D)
                var offset = (innerObject.Size - tileObject.Size).ToVector2() / 2f + innerObject.Position;
                var shape = CreateShapeFromTiledObject(innerObject, offset);
                var fixture = body.CreateFixture(shape);
                ApplyTiledPropertiesToFixture(innerObject.Properties, ref fixture);
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
            var density = obj.Properties.TryGetValue(nameof(Shape.Density), out TiledMapPropertyValue densityProperty)
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
