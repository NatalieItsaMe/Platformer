using Microsoft.Xna.Framework;

namespace Platformer.Component
{
    public class CameraTarget
    {
        public Vector2 Offset { get; set; } = new();
        public float Zoom { get; set; } = 1f;
        public int EntityID { get; set; }
        public Rectangle CameraBounds { get; set; }
        public Matrix? ScaleMatrix { get; set; } = null;
    }
}
