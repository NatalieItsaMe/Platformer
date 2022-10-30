using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System.Collections.Generic;

namespace Platformer.Components
{
    internal class Physics
    {
        public Vector2 Velocity { get; set; } = new Vector2();
        public Vector2 Acceleration { get; set; } = new Vector2();

        public float terminalVelocity = 56;
        public float groundedVelocity = -3.125f;

        internal bool grounded = false;
    }
}
