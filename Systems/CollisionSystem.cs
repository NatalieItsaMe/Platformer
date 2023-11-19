using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Tiled;
using System.Linq;
using Box2DSharp.Dynamics;

namespace Platformer
{
    internal class CollisionSystem : EntityUpdateSystem
    {
        private ComponentMapper<Body> _bodyMapper;
        private TiledMapObjectLayer _collisionLayer;

        public CollisionSystem(TiledMap tiledMap) : base(Aspect.All(typeof(Body)))
        {
            _collisionLayer = tiledMap.ObjectLayers.FirstOrDefault(l => l.Name.Equals("Collision", System.StringComparison.InvariantCultureIgnoreCase));
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _bodyMapper = mapperService.GetMapper<Body>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                Body body = _bodyMapper.Get(entity);


            }    
        }
    }
}
