using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PlagueEngine.Rendering.Components
{
    interface IDeformable
    {
        void deform(Vector3 point);
    }
}
