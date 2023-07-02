using Box2DSharp.Dynamics;
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
        private ComponentMapper<Body> _bodies;

        public PlayerInputSystem() : base(Aspect.All(typeof(Body)).One(typeof(KeyboardMapping))) { }

        public PlayerInputSystem(Game game) : base(Aspect.All(typeof(Transform2), typeof(Body)).One(typeof(KeyboardMapping)))
        {
            _game = game;
        }

        public Platformer Platformer { get; }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _keyboardMappings = mapperService.GetMapper<KeyboardMapping>();
            _bodies = mapperService.GetMapper<Body>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                Body body = _bodies.Get(entity);
                if(_keyboardMappings.Has(entity))
                {
                    var state = Keyboard.GetState();
                    var mapping = _keyboardMappings.Get(entity);

                    if (state.IsKeyDown(mapping.Exit))
                        _game.Exit();

                    if (state.IsKeyDown(mapping.Up))
                        body.ApplyForceToCenter(new(0, -8), true);
                    if (state.IsKeyDown(mapping.Down))
                        body.ApplyForceToCenter(new(0, 8), true);
                    if (state.IsKeyDown(mapping.Right))
                        body.ApplyForceToCenter(new(8,0), true);
                    if (state.IsKeyDown(mapping.Left))
                        body.ApplyForceToCenter(new(-8,0), true);
                }
            }
        }
    }
}
