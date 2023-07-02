using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Box2DSharp.Dynamics;

namespace Platformer
{
    internal class PhysicsSystem : EntityUpdateSystem
    {
        public System.Numerics.Vector2 Gravity = new System.Numerics.Vector2(0, 9.8f);

        private Box2DSharp.Dynamics.World Box2DWorld;

        public PhysicsSystem() : base(Aspect.All(typeof(Body)))
        {
            Box2DWorld = new Box2DSharp.Dynamics.World(Gravity);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
        }

        public override void Update(GameTime gameTime)
        {
            Box2DWorld.Step(gameTime.GetElapsedSeconds(), 6, 2);
                    
        }

        public Body CreateBody(BodyDef bodyDef)
        {
            return Box2DWorld.CreateBody(bodyDef);
        }
    }
}
