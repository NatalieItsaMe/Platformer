using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer.Components
{
    internal class Collider
    {
        public ContactFilter contactFilter { get; set; }
        public BoundingBox bounds { get; internal set; }

        internal static RaycastHit[] Cast(Collider collider, Vector2 move, float v)
        {
            throw new NotImplementedException();
        }
    }
}
