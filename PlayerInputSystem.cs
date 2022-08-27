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
        private const Keys up = Keys.W, down = Keys.S, left = Keys.A, right = Keys.D;
        private ComponentMapper<InputListener> _inputListeners;
        private ComponentMapper<Transform2> _transforms;

        public PlayerInputSystem() : base(Aspect.All(typeof(InputListener), typeof(Transform2)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _inputListeners = mapperService.GetMapper<InputListener>();
            _transforms = mapperService.GetMapper<Transform2>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                var state = Keyboard.GetState();
                var transform = _transforms.Get(entity);

                if(state.IsKeyDown(up)) {

                }
            }
        }
    }
}
