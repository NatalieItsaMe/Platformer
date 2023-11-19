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
            //foreach (var entity in ActiveEntities)
            //{
            //    if (_grounded.Has(entity) && Box2DWorld.BodyList.Single(body => (int)body.UserData == entity).IsAwake)
            //        _grounded.Delete(entity);
            //}

            Box2DWorld.Step(gameTime.GetElapsedSeconds(), 6, 2);
            Box2DWorld.ClearForces();
        }

        public Body AddTiledMapObject(TiledMapObject obj, Vector2 scale)
        {
            string bodyType = obj.Properties.ContainsKey("BodyType")
                ? obj.Properties["BodyType"]
                : string.Empty;
            BodyDef bodyDef = new()
            {
                Position = (obj.Position.ToNumerics() + obj.Size.ToNumerics() / 2) * scale,
                BodyType = bodyType switch
                        {
                            "Kinematic" => BodyType.KinematicBody,
                            "Dynamic" => BodyType.DynamicBody,
                            _ => BodyType.StaticBody
                        }
            };
            Body body = Box2DWorld.CreateBody(bodyDef);

            if(obj is TiledMapTileObject tileObject)
            {
                foreach(var innerObject in tileObject.Tile.Objects)
                {
                    FixtureDef fixture = GetFixtureFromTiledObject(innerObject, scale);
                    body.CreateFixture(fixture);
                }
            }
            else
            {
                FixtureDef fixture = GetFixtureFromTiledObject(obj, scale);
                body.CreateFixture(fixture);
            }

            return body;
        }

        private FixtureDef GetFixtureFromTiledObject(TiledMapObject obj, Vector2 scale)
        {
            FixtureDef fixture = new();
            if (obj is TiledMapPolygonObject polygon)
            {
                if (polygon.Points.Length == 2)
                {
                    EdgeShape shape = new();
                    shape.SetTwoSided(polygon.Points[0].ToNumerics() * scale, polygon.Points[1].ToNumerics() * scale);

                    fixture.Shape = shape;
                }
                else if (polygon.Points.Length > 2)
                {
                    PolygonShape shape = new();
                    shape.Set(polygon.Points.Select(p => p.ToNumerics() * scale).ToArray());
                    fixture.Shape = shape;
                }
            }
            else if (obj is TiledMapEllipseObject ellipse)
            {
                CircleShape shape = new CircleShape()
                {
                    Radius = (ellipse.Radius * scale).Length()
                };

                fixture.Shape = shape;
            }
            else
            {
                PolygonShape shape = new();
                var hx = obj.Size.Width * scale.X / 2f;
                var hy = obj.Size.Height * scale.Y / 2f;
                shape.SetAsBox(hx, hy, new(), obj.Rotation * (float)Math.PI / 180f);

                fixture.Shape = shape;
            }

            foreach (var property in obj.Properties)
            {
                switch (property.Key)
                {
                    case "IsSensor":
                        fixture.IsSensor = bool.Parse(property.Value);
                        break;
                    case "Density":
                        fixture.Density = float.Parse(property.Value);
                        break;
                    case "Friction":
                        fixture.Friction = float.Parse(property.Value);
                        break;
                    case "Restitution":
                        fixture.Restitution = float.Parse(property.Value);
                        break;
                }
            }
            return fixture;
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
