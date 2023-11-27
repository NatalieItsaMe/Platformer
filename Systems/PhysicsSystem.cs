using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Joints;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;
using System.Collections.Generic;
using Vector2 = System.Numerics.Vector2;

namespace Platformer
{
    internal class PhysicsSystem : UpdateSystem
    {
        public Vector2 Gravity = new (0, 21f);
        public readonly World Box2DWorld;

        public PhysicsSystem()
        {
            Box2DWorld = new World(Gravity);
        }

        public override void Update(GameTime gameTime)
        {
            Box2DWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds, 6, 2);
            Box2DWorld.ClearForces();
        }

        public Body CreateBody(BodyDef bodyDef) => Box2DWorld.CreateBody(bodyDef);

        public Joint CreateJoint(JointDef jointDef) => Box2DWorld.CreateJoint(jointDef);

        public void SetContactListener(IContactListener listener) => Box2DWorld.SetContactListener(listener);

        public void SetDebugDrawer(IDrawer drawer) => Box2DWorld.SetDebugDrawer(drawer);

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
