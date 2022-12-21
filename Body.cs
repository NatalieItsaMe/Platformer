using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class Body
    {
        public RectangleF Rectangle { get; set; }
        public BodyType Type { get; set; }
        public Vector2 Velocity { get; set; } = new Vector2();
        public Vector2 Acceleration { get; set; } = new Vector2();
        public Vector2 MaxSpeed { get; internal set; } = new Vector2();

        public enum BodyType { 
            Static, Kinematic
        }
    }
}
