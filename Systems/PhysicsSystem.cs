using Microsoft.Xna.Framework;
using Platformer.Component;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Box2DSharp.Dynamics;
using Box2DSharp.Collision.Shapes;
using System;
using System.Linq;
using Vector2 = System.Numerics.Vector2;
using System.Collections.Generic;

namespace Platformer
{
    internal class PhysicsSystem : EntityUpdateSystem
    {
        public Vector2 Gravity = new (0, 21f);
        private ComponentMapper<GroundedComponent> _grounded;
        private readonly Box2DSharp.Dynamics.World Box2DWorld;

        public PhysicsSystem() : base(Aspect.All(typeof(Body)))
        {
            Box2DWorld = new Box2DSharp.Dynamics.World(Gravity);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _grounded = mapperService.GetMapper<GroundedComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            ActiveEntities.Where(e => _grounded.Has(e) && Box2DWorld.BodyList.Where(b => (int)b.UserData == e).All(b => b.IsAwake))
                .ToList().ForEach(e => _grounded.Delete(e));

            Box2DWorld.Step(gameTime.GetElapsedSeconds(), 6, 2);
            Box2DWorld.ClearForces();
        }

        public Body CreateBodyFromTiledObject(TiledMapObject obj, Vector2 scale)
        {
            string bodyType;
            obj.Properties.TryGetValue("BodyType", out bodyType);
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
            Body body = Box2DWorld.CreateBody(bodyDef);

            if(obj is TiledMapTileObject tileObject && tileObject.Tile != null)
            {
                foreach(var innerObject in tileObject.Tile.Objects)
                {
                    FixtureDef fixture = new();
                    fixture.Shape = CreateShapeFromTiledObject(innerObject, scale);

                    SetPropertyValues(obj, ref fixture);
                    body.CreateFixture(fixture);
                }
            }
            else
            {
                FixtureDef fixture = new();
                fixture.Shape = CreateShapeFromTiledObject(obj, scale);

                SetPropertyValues(obj, ref fixture);
                body.CreateFixture(fixture);
            }

            return body;
        }

        private static Box2DSharp.Collision.Shapes.Shape CreateShapeFromTiledObject(TiledMapObject obj, Vector2 scale)
        {
            if (obj is TiledMapPolygonObject polygon)
            {
                //Concave polygons are not supported
                PolygonShape shape = new();
                shape.Set(polygon.Points.Select(p => p.ToNumerics() * scale).ToArray());
                return shape;
            }
            else if (obj is TiledMapPolylineObject polyline)
            {
                EdgeShape shape = new();
                Vector2 start = polyline.Points[0].ToNumerics() * scale;
                Vector2 end = polyline.Points[1].ToNumerics() * scale;
                Vector2 normal_start = (start + end) / 2f;
                Vector2 normal_end = - Vector2.UnitY * scale + normal_start;
                shape.SetOneSided(normal_start, start, end, normal_end);

                return shape;
            }
            else if (obj is TiledMapEllipseObject ellipse)
            {
                CircleShape shape = new()
                {
                    Radius = (ellipse.Radius * scale).Length()
                };

                return shape;
            }

            PolygonShape box = new();
            Vector2 h = obj.Size.ToNumerics() * scale / 2f;
            box.SetAsBox(h.X, h.Y, new(), 0);

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

        public Body CreateBody(BodyDef bodyDef) => Box2DWorld.CreateBody(bodyDef);

        public void SetContactListener(IContactListener listener) => Box2DWorld.SetContactListener(listener);

        internal Body[] GetBodiesAt(float x, float y)
        {
            Vector2 point = new(x, y);
            Box2DSharp.Collision.AABB aabb = new Box2DSharp.Collision.AABB(point, point);
            PointCallback callback = new PointCallback();
            Box2DWorld.QueryAABB(callback, aabb);
            return callback.hits.ToArray();
        }

        private class PointCallback : IQueryCallback
        {
            internal List<Body> hits = new();

            public bool QueryCallback(Fixture fixture)
            {
                hits.Add(fixture.Body);
                return true;
            }
        }
    }
}
