using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ECS;
using MonoGame.Extended.ECS.Systems;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Platformer.Component;
using System.Linq;

namespace Platformer.Systems
{
    internal class CameraTargetSystem : EntityUpdateSystem
    {
        private ComponentMapper<CameraTarget> _cameraTargets;
        private ComponentMapper<Body> _bodies;
        private readonly OrthographicCamera _camera;
        
        public CameraTargetSystem(OrthographicCamera camera) : base(Aspect.All(typeof(CameraTarget)).One(typeof(Body), typeof(Transform), typeof(Point)))
        {
            _camera = camera;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _cameraTargets = mapperService.GetMapper<CameraTarget>();
            _bodies = mapperService.GetMapper<Body>();
        }

        public override void Update(GameTime gameTime)
        {
            int entityId = ActiveEntities.FirstOrDefault();
            if (!_cameraTargets.TryGet(entityId, out CameraTarget target))
                return;

            if (!_bodies.TryGet(entityId, out Body body))
                return;

#if DEBUG
            DebugUpdateTarget(target);
#endif

            Vector2 position = body.Position + target.Offset;

            if (target.ScaleMatrix.HasValue)
                position = Vector2.Transform(position, target.ScaleMatrix.Value);

            _camera.Zoom = target.Zoom;
            _camera.LerpToPosition(position);
            ClampCameraWithinBounds(target.CameraBounds);
        }

        public void ClampCameraWithinBounds(Rectangle bounds)
        {
            Vector2 d = new();
            if (_camera.BoundingRectangle.Width > bounds.Width)
            {
                d.X = (bounds.Center.X - _camera.BoundingRectangle.Center.X);
            }
            else
            {
                if (_camera.BoundingRectangle.Left < bounds.Left)
                {
                    d.X = (bounds.Left - _camera.BoundingRectangle.Left);
                }
                if (_camera.BoundingRectangle.Right > bounds.Right)
                {
                    d.X = (bounds.Right - _camera.BoundingRectangle.Right);
                }
            }
            if (_camera.BoundingRectangle.Height > bounds.Height)
            {
                d.Y = (bounds.Center.Y - _camera.BoundingRectangle.Center.Y);
            }
            else
            {
                if (_camera.BoundingRectangle.Top < bounds.Top)
                {
                    d.Y = (bounds.Top - _camera.BoundingRectangle.Top);
                }
                if (_camera.BoundingRectangle.Bottom > bounds.Bottom)
                {
                    d.Y = (bounds.Bottom - _camera.BoundingRectangle.Bottom);
                }
            }
            _camera.Move(d);
        }

        public static void DebugUpdateTarget(CameraTarget target)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Left))
                target.Offset -= Vector2.UnitX;

            if (keyboard.IsKeyDown(Keys.Right))
                target.Offset += Vector2.UnitX;

            if (keyboard.IsKeyDown(Keys.Down))
                target.Offset += Vector2.UnitY;

            if (keyboard.IsKeyDown(Keys.Up))
                target.Offset -= Vector2.UnitY;

            if (keyboard.IsKeyDown(Keys.PageDown))
                target.Zoom *= 0.9f;
            if (keyboard.IsKeyDown(Keys.PageUp))
                target.Zoom *= 1.1f;
        }
    }
}
