using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using TiledCS;
using System.Linq;
using Box2DSharp.Dynamics;

namespace Platformer
{
    internal class CollisionSystem : EntityUpdateSystem
    {
        private ComponentMapper<Body> _bodyMapper;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<IShapeF> _shapeMapper;
        private TiledLayer _collisionLayer;

        public CollisionSystem(TiledMap tiledMap) : base(Aspect.All(typeof(Body), typeof(Transform2), typeof(IShapeF)))
        {
            _collisionLayer = tiledMap.Layers.Single(layer => layer.name == "Collision");
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _bodyMapper = mapperService.GetMapper<Body>();
            _transformMapper = mapperService.GetMapper<Transform2>();
            _shapeMapper = mapperService.GetMapper<IShapeF>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                Body body = _bodyMapper.Get(entity);
                Transform2 transform = _transformMapper.Get(entity);
                var shape = _shapeMapper.Get(entity);


            }    
        }
    }
}
