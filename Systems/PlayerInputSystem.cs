using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Platformer.Components;

namespace Platformer
{
    internal class PlayerInputSystem : EntityUpdateSystem
    {
        private Game _game;
        private ComponentMapper<KeyboardMapping> _keyboardMappings;
        private ComponentMapper<Physics> _physics;

        public PlayerInputSystem() : base(Aspect.All(typeof(Physics)).One(typeof(KeyboardMapping))) { }

        public PlayerInputSystem(Game game) : base(Aspect.All(typeof(Transform2), typeof(Physics)).One(typeof(KeyboardMapping)))
        {
            _game = game;
        }

        public Platformer Platformer { get; }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _keyboardMappings = mapperService.GetMapper<KeyboardMapping>();
            _physics = mapperService.GetMapper<Physics>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                Vector2 acceleration = _physics.Get(entity).Acceleration;
                if(_keyboardMappings.Has(entity))
                {
                    var state = Keyboard.GetState();
                    var mapping = _keyboardMappings.Get(entity);

                    if (state.IsKeyDown(mapping.Exit))
                        _game.Exit();

                    if (state.IsKeyDown(mapping.Up)) 
                        acceleration = acceleration.SetY(-8);
                    if (state.IsKeyDown(mapping.Down))
                        acceleration = acceleration.SetY(8);
                    if (state.IsKeyDown(mapping.Right))
                        acceleration = acceleration.SetX(8);
                    if (state.IsKeyDown(mapping.Left))
                        acceleration = acceleration.SetX(-8);

                    if (state.IsKeyUp(mapping.Up) && state.IsKeyUp(mapping.Down))
                        acceleration = acceleration.SetY(0);
                    if (state.IsKeyUp(mapping.Right) && state.IsKeyUp(mapping.Left))
                        acceleration = acceleration.SetX(0);

                    _physics.Get(entity).Acceleration = acceleration;
                    //LogDebugInfo(_physics.Get(entity).Acceleration, acceleration);
                }
            }
        }
    }
}
