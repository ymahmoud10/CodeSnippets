using Sirenix.OdinInspector;
using UnityEngine;

namespace ObscureGames
{
    [RequireComponent(typeof(OGUnit))]
    public class OGAttacker : MonoBehaviour
    {
        #region General Configs

        [Title("General Configs:")]
        public float detectionRadius = 5f;
        public float detectionAngle = 60f;
        [DisableIf("@true")]
        public Transform target;

        [Space]
        [Title("Debugging")]
        public Color gizmosColor;
        public bool drawViewRange = false;
        
        #endregion

        #region Private

        

        #endregion
        
        void Update()
        {
            LookForTarget();
        }

        private void LookForTarget()
        {
            Transform newTarget = null;
            float minDistance = float.MaxValue;
            foreach (OGUnit character in BattleManager.Instance.characters)
            {
                float currDistance = Vector3.Distance(transform.position, character.transform.position);
                float angle = Mathf.Acos(Vector3.Dot(transform.forward,
                    (character.transform.position - transform.position).normalized)) * 180 / Mathf.PI;
                
                // If the character is in the detection radius AND angle.
                if (currDistance < Mathf.Min(detectionRadius, minDistance) && angle < detectionAngle)
                {
                    minDistance = currDistance;
                    newTarget = character.transform;
                }
            }

            if (newTarget)
            {
                target = newTarget;
            }
        }

        private void OnDrawGizmos()
        {
            if (drawViewRange)
            {
                Gizmos.color = gizmosColor;
                UnityEditor.Handles.color = gizmosColor;
                Vector3 offset = Vector3.up;
                Gizmos.DrawRay(transform.position + offset,
                    Quaternion.Euler(0, detectionAngle, 0) * transform.forward * detectionRadius);
                Gizmos.DrawRay(transform.position + offset,
                    Quaternion.Euler(0, -detectionAngle, 0) * transform.forward * detectionRadius);
                UnityEditor.Handles.DrawSolidArc(transform.position + offset, Vector3.up, transform.forward, detectionAngle,
                    detectionRadius);
                UnityEditor.Handles.DrawSolidArc(transform.position + offset, Vector3.up, transform.forward, -detectionAngle,
                    detectionRadius);
            }
        }
    }
}
