using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Utility.UnityToBepu
{
    internal static class Capsule
    {
        internal static float ChangeSizeToBepu(Vector2 size) {
            return size.Y - 2 * size.X;
        }
    }
}
