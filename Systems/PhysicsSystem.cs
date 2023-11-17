using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Box2DSharp.Dynamics;
using Platformer.Component;
using System.Linq;
using Box2DSharp.Collision.Shapes;
using System;
using MonoGame.Extended.Tiled;
using System.Collections.Generic;
using System.Numerics;

namespace Platformer
{
    internal class PhysicsSystem : EntityUpdateSystem
    {
        public System.Numerics.Vector2 Gravity = new System.Numerics.Vector2(0, 21f);
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
            foreach (var entity in ActiveEntities)
            {
                if (_grounded.Has(entity) && Box2DWorld.BodyList.Single(body => (int)body.UserData == entity).IsAwake)
                    _grounded.Delete(entity);
            }

            Box2DWorld.Step(gameTime.GetElapsedSeconds(), 6, 2);
        }

        public Body AddTiledMapObject(TiledMapObject obj)
        {
                BodyDef bodyDef = new BodyDef()
                {
                    Position = obj.Position.ToNumerics(),
                    Angle = obj.Rotation * (float)Math.PI / 180f,
                    BodyType = obj.Type switch
                    {
                        "Static" => BodyType.StaticBody,
                        "Kinematic" => BodyType.KinematicBody,
                        _ => BodyType.DynamicBody
                    }
                };
                Body body = Box2DWorld.CreateBody(bodyDef);

                FixtureDef fixture = new();
                if (obj.Properties != null)
                {
                    if (obj.Properties.ContainsKey("friction"))
                        fixture.Friction = float.Parse(obj.Properties["friction"]);
                    if (obj.Properties.ContainsKey("restitution"))
                        fixture.Restitution = float.Parse(obj.Properties["restitution"]);
                    if (obj.Properties.ContainsKey("density"))
                        fixture.Density = float.Parse(obj.Properties["density"]);
                    if (obj.Properties.ContainsKey("filter"))
                        fixture.Filter = new Filter(); //TODO work out filtering later
                }

                if (obj is TiledMapPolygonObject)
                {
                    TiledMapPolygonObject polygon = (TiledMapPolygonObject)obj;

                    if (polygon.Points.Length == 2)
                    {
                        EdgeShape shape = new();
                        shape.SetTwoSided(polygon.Points[0].ToNumerics(), polygon.Points[1].ToNumerics());

                        fixture.Shape = shape;
                    }
                    else if (polygon.Points.Length > 2)
                    {
                        PolygonShape shape = new();
                        shape.Set(polygon.Points.Select(p => new System.Numerics.Vector2(p.X, p.Y)).ToArray());

                        fixture.Shape = shape;
                    }
                }
                else if (obj is TiledMapEllipseObject)
                {
                    TiledMapEllipseObject ellipse = (TiledMapEllipseObject)obj;

                    CircleShape shape = new();
                    shape.Position = ellipse.Center.ToNumerics();
                    shape.Radius = ellipse.Radius.Length();

                    fixture.Shape = shape;
                }
                else
                {
                    PolygonShape shape = new();
                    shape.SetAsBox(obj.Size.Width, obj.Size.Height);

                    fixture.Shape = shape;
                }
                body.CreateFixture(fixture);
                return body;
        }


        public Body CreateBody(BodyDef bodyDef) => Box2DWorld.CreateBody(bodyDef);

        public void SetContactListener(IContactListener listener) => Box2DWorld.SetContactListener(listener);
    }
}
