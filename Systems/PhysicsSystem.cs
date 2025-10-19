using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.ECS.Systems;
using nkast.Aether.Physics2D.Dynamics;
using Platformer.ContactListeners;
using System.Collections.Generic;

namespace Platformer.Systems
{
    internal class PhysicsSystem : UpdateSystem
    {
        public Vector2 Gravity = new (0, 21f);
        public World PhysicsWorld { get; }

        public PhysicsSystem()
        {
            PhysicsWorld = new World(Gravity);
        }

        public override void Update(GameTime gameTime)
        {
            PhysicsWorld.Step(gameTime.ElapsedGameTime);
            PhysicsWorld.ClearForces();
        }

        internal IEnumerable<Body> GetBodiesAt(float x, float y)
        {
            var point = new Vector2(x, y);
            var aabb = new nkast.Aether.Physics2D.Collision.AABB(point, point);

            var hits = new List<Body>();
            PhysicsWorld.QueryAABB(f =>
            {
                hits.Add(f.Body);
                return true;
            }, aabb);
            return hits;
        }

        public void RegisterContactListener(IContactListener contactListener)
        {
            PhysicsWorld.ContactManager.BeginContact += contactListener.BeginContact;
            PhysicsWorld.ContactManager.EndContact += contactListener.EndContact;
            PhysicsWorld.ContactManager.PreSolve += contactListener.PreSolve;
        }
    }
}
