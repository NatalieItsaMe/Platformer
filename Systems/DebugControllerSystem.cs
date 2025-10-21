using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using System.Diagnostics;
using Platformer.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer.Systems
{
    internal class DebugControllerSystem : UpdateSystem
    {
        public PlatformerGame Game { get; set; }
        public OrthographicCamera Camera { get; set; }
        public PhysicsSystem PhysicsSystem { get; set; }

        public DebugControllerSystem(PlatformerGame game)
        {
            Game = game;
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                Game.Exit();

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                OutputBodyReport(mouse.X, mouse.Y);
            }
        }

        private void OutputBodyReport(float x, float y)
        {
            Vector2 worldMouse = Camera.ScreenToWorld(x, y);
            var bodiesUnderMouse = PhysicsSystem.GetBodiesAt(worldMouse.X, worldMouse.Y);

            if (!bodiesUnderMouse.Any())
                return;

            Debug.WriteLine("------------LIST BEGIN-----------");
            foreach (var body in bodiesUnderMouse)
            {
                Debug.WriteLine($"---------entity: {body.Tag}");
                Debug.WriteLine($"          local: {body.GetLocalPoint(worldMouse)}");
                Debug.WriteLine($"       Position: {body.Position}");
                Debug.WriteLine($"      IsEnabled: {body.Enabled}");
                Debug.WriteLine($"        IsAwake: {body.Awake}");
                Debug.WriteLine($"           Mass: {body.Mass}");
                Debug.WriteLine($" LinearVelocity: {body.LinearVelocity}");
                Debug.WriteLine($"AngularVelocity: {body.AngularVelocity}");
                Debug.WriteLine($"-------Fixtures: {body.FixtureList.Count}");

                foreach (var fixture in body.FixtureList)
                {
                    Debug.WriteLine($"        Fixture: {fixture.Shape.ShapeType}");
                    Debug.WriteLine($"        Density: {fixture.Shape.Density}");
                    Debug.WriteLine($"       Friction: {fixture.Friction}");
                    Debug.WriteLine($"    Restitution: {fixture.Restitution}");
                }
            }
            Debug.WriteLine("------------LIST END-----------");
        }
    }
}
