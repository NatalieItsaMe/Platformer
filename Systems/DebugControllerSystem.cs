using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using MonoGameGum;
using nkast.Aether.Physics2D.Dynamics;
using Platformer.Component;
using Platformer.UI;
using System.Linq;

namespace Platformer.Systems
{
    internal class DebugControllerSystem : EntityUpdateSystem
    {
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<KeyboardController> _playerControllers;

        public DebugControllerSystem() : base(Aspect.One(typeof(Body), typeof(KeyboardController)))
        {
        }

        public OrthographicCamera Camera { get; set; }
        public PhysicsSystem PhysicsSystem { get; set; }
        public Matrix CameraToPhysicsMatrix { get; set; }
        public GumService GumUI => GumService.Default;

        public override void Initialize(IComponentMapperService mapperService)
        {
            _bodies = mapperService.GetMapper<Body>();
            _playerControllers = mapperService.GetMapper<KeyboardController>();
        }

        public override void Update(GameTime gameTime)
        {
            if (GumUI.Cursor.WindowOver != null)
                return;

            if (GumUI.Cursor.PrimaryClick)
                OutputBodyReport(GumUI.Cursor.X, GumUI.Cursor.Y);
        }

        private void OutputBodyReport(float x, float y)
        {
            Vector2 worldMouse = Vector2.Transform( Camera.ScreenToWorld(x, y), CameraToPhysicsMatrix);
            var bodiesUnderMouse = PhysicsSystem.GetBodiesAt(worldMouse.X, worldMouse.Y);

            if (!bodiesUnderMouse.Any())
                return;

            foreach (var body in bodiesUnderMouse)
            {
                var dialog = new BodyEditorPanel(body);
                dialog.AddToRoot();
                dialog.Visual.Dock(Gum.Wireframe.Dock.Right);

                if (_playerControllers.TryGet((int)body.Tag, out var playerController))
                    dialog.AddPlayerControls(playerController); 
            }
        }
    }
}
