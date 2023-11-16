using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using Platformer.Components;
using Platformer.Systems;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using World = MonoGame.Extended.Entities.World;

namespace Platformer
{
    public class Platformer : Game
    {
        private GraphicsDeviceManager _graphics;
        internal RenderSystem _renderSystem;
        private TiledService _tiledService;
        private PhysicsSystem _physicsSystem;
        private World _world;

        public Platformer()
        {
            Window.AllowUserResizing = true;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            string mapPath = Path.Combine(Directory.GetCurrentDirectory(), @"Assets\_tiled\sandbox.tmx");
            _tiledService = new TiledService(mapPath, Content);

            _physicsSystem = new PhysicsSystem();
            _renderSystem = new RenderSystem(GraphicsDevice);
            _world = new WorldBuilder()
                .AddSystem(_renderSystem)
                .AddSystem(new PlayerInputSystem(this))
                .AddSystem(_physicsSystem)
                .Build();

            _physicsSystem.SetContactListener(new GroundContactListener(_world));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            AddObjectLayerToPhysicsSystem();
        }

        private void AddObjectLayerToPhysicsSystem()
        {
            var objects = _tiledService.GetObjectLayers().Where(l => l.name == "Collision").SelectMany(l => l.objects);

            foreach (var obj in objects)
            {
                var entity = _world.CreateEntity();

                BodyDef bodyDef = new BodyDef()
                {
                    Position = new(obj.x, obj.y),
                    Angle = obj.rotation * (float)Math.PI / 180f,
                    BodyType = obj.type switch
                    {
                        "Static" => BodyType.StaticBody,
                        "Kinematic" => BodyType.KinematicBody,
                        _ => BodyType.DynamicBody
                    }
                };
                Body body = _physicsSystem.CreateBody(bodyDef);

                FixtureDef fixture = new();
                if (obj.properties != null)
                {
                    if (obj.properties.Any(p => p.name.Equals("friction", StringComparison.InvariantCultureIgnoreCase)))
                        fixture.Friction = float.Parse(obj.properties.First(p => p.name.Equals("friction", StringComparison.InvariantCultureIgnoreCase)).value);
                    if (obj.properties.Any(p => p.name.Equals("restitution", StringComparison.InvariantCultureIgnoreCase)))
                        fixture.Restitution = float.Parse(obj.properties.First(p => p.name.Equals("restitution", StringComparison.InvariantCultureIgnoreCase)).value);
                    if (obj.properties.Any(p => p.name.Equals("density", StringComparison.InvariantCultureIgnoreCase)))
                        fixture.Density = float.Parse(obj.properties.First(p => p.name.Equals("density", StringComparison.InvariantCultureIgnoreCase)).value);
                    if (obj.properties.Any(p => p.name.Equals("filter", StringComparison.InvariantCultureIgnoreCase)))
                        fixture.Filter = new Filter(); //TODO work out filtering later
                    if (obj.properties.Any(p => p.name.Equals("CameraTarget", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        entity.Attach(new CameraTarget());
                    }
                    if (obj.properties.Any(p => p.name.Equals("KeyboardMapping", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        entity.Attach(new KeyboardMapping());
                    }
                }

                if (obj.polygon != null)
                {
                    if (obj.polygon.points.Length % 2 != 0)
                        throw new Exception("polygon point size invalid");
                    else
                    {
                        var points = new System.Numerics.Vector2[obj.polygon.points.Length / 2];
                        for (int p = 0; p < obj.polygon.points.Length; p += 2)
                        {
                            var point = new System.Numerics.Vector2(obj.polygon.points[p], obj.polygon.points[p + 1]);
                            points.SetValue(point, p / 2);
                        }

                        if (points.Length == 2)
                        {
                            EdgeShape shape = new();
                            shape.SetTwoSided(points[0], points[1]);

                            fixture.Shape = shape;
                        }
                        else if (points.Length > 2)
                        {
                            PolygonShape shape = new();
                            shape.Set(points);

                            fixture.Shape = shape;
                        }
                    }
                }
                else if (obj.ellipse != null)
                {
                    CircleShape shape = new();
                    shape.Position = new(obj.x, obj.y);
                    shape.Radius = (obj.width + obj.height) / 4;

                    fixture.Shape = shape;
                }
                else
                {
                    PolygonShape shape = new();
                    shape.SetAsBox(obj.width, obj.height);

                    fixture.Shape = shape;
                }
                body.CreateFixture(fixture);
                body.UserData = entity.Id;
                entity.Attach(body);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            _world.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _world.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}