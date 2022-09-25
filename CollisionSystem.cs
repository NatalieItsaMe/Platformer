using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    internal class CollisionSystem : EntityUpdateSystem
    {
        private ComponentMapper<Physics> _physicsMapper;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<IShapeF> _shapeMapper;
        private TiledMapTileLayer _collisionLayer;

        public CollisionSystem(TiledMap tiledMap) : base(Aspect.All(typeof(Physics), typeof(Transform2), typeof(IShapeF)))
        {
            _collisionLayer = tiledMap.TileLayers.Single(l => l.Name == "Collision");
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _physicsMapper = mapperService.GetMapper<Physics>();
            _transformMapper = mapperService.GetMapper<Transform2>();
            _shapeMapper = mapperService.GetMapper<IShapeF>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                Physics physics = _physicsMapper.Get(entity);
                Transform2 transform = _transformMapper.Get(entity);
                var shape = _shapeMapper.Get(entity);


            }    
        }

        private ShapeCastHit Cast(IShapeF Shape, Vector2 Direction)
        {
            ShapeCastHit hit = new ShapeCastHit();

            foreach(var entity in ActiveEntities)
            {

            }


            return hit;
        }

        private class ShapeCastHit
        {

        }
    }
}
