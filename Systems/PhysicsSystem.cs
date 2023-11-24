using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;
using Box2DSharp.Dynamics;
using Vector2 = System.Numerics.Vector2;
using System.Collections.Generic;
using Box2DSharp.Dynamics.Joints;
using Microsoft.Xna.Framework.Graphics;
using Box2DSharp.Common;

namespace Platformer
{
    internal class PhysicsSystem : UpdateSystem, IDrawSystem
    {
        public Vector2 Gravity = new (0, 21f);
        public readonly World Box2DWorld;
        public SpriteBatch SpriteBatch 
        { 
            get => _spriteBatch; 
            set
            {
                if(_spriteBatch != value)
                {
                    _spriteBatch = value;
                    Box2DWorld.SetDebugDrawer(new Box2dDebugDrawer(value));
                }
            } 
        }

        private SpriteBatch _spriteBatch;

        public PhysicsSystem()
        {
            Box2DWorld = new World(Gravity);
        }

        public override void Update(GameTime gameTime)
        {
            Box2DWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds, 6, 2);
            Box2DWorld.ClearForces();
        }

        public void Draw(GameTime gameTime)
        {
            //_spriteBatch.Begin();
            Box2DWorld.DebugDraw();
            //_spriteBatch.End();
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
