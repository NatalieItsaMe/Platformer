using Microsoft.Xna.Framework;

namespace Platformer.Component
{
    public class CameraTarget
    {
        public Vector2 Offset { get; set; } = new();
        public float Zoom { get; set; } = 1f;
    }
}
