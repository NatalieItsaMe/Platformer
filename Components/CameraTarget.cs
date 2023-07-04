using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer.Components
{
    public class CameraTarget
    {
        public Vector2 offset;
        public float zoom;

        public CameraTarget(Vector2 offset = new(), float zoom = 1)
        {
            this.offset = offset;
            this.zoom = zoom;
        }
    }
}
