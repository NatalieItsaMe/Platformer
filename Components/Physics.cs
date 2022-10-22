using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Platformer.Components
{
    internal class Physics
    {
        public Vector2 Velocity { get; set; } = new Vector2();
        public Vector2 Acceleration { get; set; } = new Vector2();
    }
}
