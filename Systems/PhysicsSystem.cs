using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS.Systems;
using nkast.Aether.Physics2D.Dynamics;
using System.Collections.Generic;

namespace Platformer.Systems
{
    internal class PhysicsSystem : UpdateSystem
    {
        public Vector2 Gravity = new (0, 21f);
        public World Box2DWorld { get; }

        public PhysicsSystem()
        {
            Box2DWorld = new World(Gravity);
        }

        public override void Update(GameTime gameTime)
        {
            Box2DWorld.Step(gameTime.ElapsedGameTime);
            Box2DWorld.ClearForces();
        }

        internal IEnumerable<Body> GetBodiesAt(float x, float y)
        {
            var point = new Vector2(x, y);
            var aabb = new nkast.Aether.Physics2D.Collision.AABB(point, point);

            var hits = new List<Body>();
            Box2DWorld.QueryAABB(f =>
            {
                hits.Add(f.Body);
                return true;
            }, aabb);
            return hits;
        }
    }
}
