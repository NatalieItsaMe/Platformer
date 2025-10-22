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
    internal class CameraTargetSystem(OrthographicCamera camera) : EntityUpdateSystem(Aspect.All(typeof(CameraTarget)).One(typeof(Body), typeof(Transform), typeof(Point)))
    {
        public Rectangle WorldBounds { get; set; }
        public Matrix? ScaleMatrix { get; set; } = null;

        private ComponentMapper<CameraTarget> _cameraTargets;
        private ComponentMapper<Body> _bodies;

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

            if (ScaleMatrix.HasValue)
                position = Vector2.Transform(position, ScaleMatrix.Value);

            camera.Zoom = target.Zoom;
            camera.LerpToPosition(position);
            ClampCameraWithinBounds();
        }

        public void ClampCameraWithinBounds()
        {
            Vector2 d = new();
            if (camera.BoundingRectangle.Width > WorldBounds.Width)
            {
                d.X = (WorldBounds.Center.X - camera.BoundingRectangle.Center.X);
            }
            else
            {
                if (camera.BoundingRectangle.Left < WorldBounds.Left)
                {
                    d.X = (WorldBounds.Left - camera.BoundingRectangle.Left);
                }
                if (camera.BoundingRectangle.Right > WorldBounds.Right)
                {
                    d.X = (WorldBounds.Right - camera.BoundingRectangle.Right);
                }
            }
            if (camera.BoundingRectangle.Height > WorldBounds.Height)
            {
                d.Y = (WorldBounds.Center.Y - camera.BoundingRectangle.Center.Y);
            }
            else
            {
                if (camera.BoundingRectangle.Top < WorldBounds.Top)
                {
                    d.Y = (WorldBounds.Top - camera.BoundingRectangle.Top);
                }
                if (camera.BoundingRectangle.Bottom > WorldBounds.Bottom)
                {
                    d.Y = (WorldBounds.Bottom - camera.BoundingRectangle.Bottom);
                }
            }
            camera.Move(d);
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
