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
        private ComponentMapper<Physics> _physics;

        public PlayerInputSystem() : base(Aspect.All(typeof(Transform2), typeof(Physics)).One(typeof(KeyboardMapping)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _keyboardMappings = mapperService.GetMapper<KeyboardMapping>();
            _transforms = mapperService.GetMapper<Transform2>();
            _physics = mapperService.GetMapper<Physics>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                var transform = _transforms.Get(entity);
                Vector2 acceleration = new Vector2();
                if(_keyboardMappings.Has(entity))
                {
                    var state = Keyboard.GetState();
                    var mapping = _keyboardMappings.Get(entity);

                    if (state.IsKeyDown(mapping.Up)) 
                        acceleration = acceleration.SetY(-1);
                    if (state.IsKeyDown(mapping.Down))
                        acceleration = acceleration.SetY(1);
                    if (state.IsKeyDown(mapping.Right))
                        acceleration = acceleration.SetX(1);
                    if (state.IsKeyDown(mapping.Left))
                        acceleration = acceleration.SetX(-1);

                    if (state.IsKeyUp(mapping.Up) && state.IsKeyUp(mapping.Down))
                        acceleration = acceleration.SetY(0);
                    if (state.IsKeyUp(mapping.Right) && state.IsKeyUp(mapping.Left))
                        acceleration = acceleration.SetX(0);

                    _physics.Get(entity).Acceleration = acceleration;
                    //LogDebugInfo(_physics.Get(entity).Acceleration, acceleration);
                }
            }
        }

        private void LogDebugInfo(params object[] objects)
        {
            for(int i= 0; i < objects.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine("Object " + i + ": " + objects[i].ToString());
            }
        }
    }
}
