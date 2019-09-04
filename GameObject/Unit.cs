
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Com.Nelalen.GameObject
{
    internal class Unit 
    {
        internal Collider collider;
        private string name;
        internal string Name => name;
        private int unitId;
        internal int UnitId => unitId;


        private System.Numerics.Vector2 size = new System.Numerics.Vector2(.25f, 1.5f);
        public System.Numerics.Vector2 Size => size;
        private System.Numerics.Vector3 startPosition;
        internal Unit(int unitId, string name, System.Numerics.Vector3 startPosition) {
            Console.WriteLine("start position");
            this.startPosition = new System.Numerics.Vector3(startPosition.X, startPosition.Y + (size.Y/2f), startPosition.Z);
            collider.type = Collider.Type.Unit;
            this.name = name;
            this.unitId = unitId;
        }
        
         internal System.Numerics.Vector3 GetStartPosition()
        {
           
            return this.startPosition;
        }
        internal string GetName() {
            return name;
        }

        public void SetBodyHandle(int bodyHandle) {
            //Console.WriteLine("bodyHandle: ", bodyHandle);
            collider.bodyHandle = bodyHandle;
        }
    }
}
