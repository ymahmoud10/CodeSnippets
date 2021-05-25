using Sirenix.OdinInspector;
using UnityEngine;

namespace ObscureGames
{
    public class OGUnit : MonoBehaviour
    {
        public enum UnitType
        {
            Character,
            Zombie,
        }

        public UnitType unitType;

        [ReadOnly, ShowIf("unitType", UnitType.Character)]
        public bool isControlledWalker;
        internal OGControlledWalker controlledWalker;
        
        [ReadOnly, ShowIf("unitType", UnitType.Character)]
        public bool isControlledAttacker;
        internal OGControlledAttacker controlledAttacker;

    }
}