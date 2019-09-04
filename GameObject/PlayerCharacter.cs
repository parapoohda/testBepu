
using bepuphysics2_for_nelalen;
using Com.Nelalen.GameObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Nelalen.GameObject
{
    internal class PlayerCharacter : Character
    {
        private int charId;
        
        private int characterId { get { return charId; } set { charId = value; } }
        private int clientId;

        internal PlayerCharacter(int characterId, int clientId, string name,Bepu bepu, int unitId, System.Numerics.Vector3 startPosition) : base(unitId, name, bepu, startPosition)
        {
            collider.type = Collider.Type.PlayerCharacer;
            this.charId = characterId;
            this.clientId = clientId;
       
        }


        internal int GetClientId() {
            return clientId;
        }

        internal int GetCharId() {
            return charId;
        }



        /*internal void CharacterMove(Vector3 target, CharacterMoveType characterMoveType, float movementSpeed)
        {
            gate.CharacterMove(target, characterMoveType, movementSpeed);
        }*/
    }
}
