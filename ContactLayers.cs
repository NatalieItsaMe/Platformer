using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    internal enum ContactLayers
    {
        NONE = 0b00000000,
        ENTITY = 0b00000001,
        WORLD = 0b00000010,
        ALLY = 0b00000100,
        ENEMY = 0b000001000,

    }
}
