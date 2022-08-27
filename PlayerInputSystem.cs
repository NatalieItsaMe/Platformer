using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Input.InputListeners;
using System;

namespace Platformer
{
    internal class PlayerInputSystem : EntityUpdateSystem
    {
        private ComponentMapper<KeyboardMapping> _keyboardMappings;
        private ComponentMapper<Transform2> _transforms;

        public PlayerInputSystem() : base(Aspect.All(typeof(Transform2)).One(typeof(KeyboardMapping)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _keyboardMappings = mapperService.GetMapper<KeyboardMapping>();
            _transforms = mapperService.GetMapper<Transform2>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                var transform = _transforms.Get(entity);
                if(_keyboardMappings.Has(entity))
                {
                    var state = Keyboard.GetState();
                    var mapping = _keyboardMappings.Get(entity);

                    if(state.IsKeyDown(mapping.Up)) 
                        transform.Position -= Vector2.UnitY;
                    if(state.IsKeyDown(mapping.Down))
                        transform.Position += Vector2.UnitY;
                    if (state.IsKeyDown(mapping.Right))
                        transform.Position += Vector2.UnitX;
                    if (state.IsKeyDown(mapping.Left))
                        transform.Position -= Vector2.UnitX;

                }
            }
        }
    }
}
