using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Utility.BepuToUnity
{
    internal static class Capsule
    {
        internal static Vector3 GetButtom(Vector3 position,Vector2 size) {
            return new Vector3(position.X, position.Y-(size.Y/2), position.Z);
        }
    }
}
