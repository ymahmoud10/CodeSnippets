using UnityEngine;
using UnityEngine.AI;

namespace ObscureGames
{
    [RequireComponent(typeof(OGUnit))]
    public class OGControlledWalker : MonoBehaviour
    {
        public float rotationSpeed;
        public float strafingThreshold;
        
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private OGUnit unit;

        #region AnimatorParams

        private static readonly int AnimParamIsRunning = Animator.StringToHash("IsRunning");
        private static readonly int AnimParamIsStrafingRight = Animator.StringToHash("IsStrafingRight");
        private static readonly int AnimParamIsStrafingLeft = Animator.StringToHash("IsStrafingLeft");

        #endregion

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            unit = GetComponent<OGUnit>();
        }
        
        private void OnEnable()
        {
            unit.isControlledWalker = true;
            unit.controlledWalker = this;
        }
        
        private void OnDisable()
        {
            unit.isControlledWalker = false;
            unit.controlledWalker = null;
        }

        private void Update()
        {
            HandleRunningAnimation();
            HandleStrafingAnimation();
        }

        private void HandleStrafingAnimation()
        {
            // If walker is also an attacker
            if (unit.isControlledAttacker)
            {
                // If he has a target assigned
                if (unit.controlledAttacker.target)
                {
                    // Look (smoothly) towards the target
                    var dir = new Vector3(unit.controlledAttacker.target.position.x, transform.position.y,
                        unit.controlledAttacker.target.position.z) - transform.position;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir),
                        Time.deltaTime * rotationSpeed);

                    // Handle strafing
                    var angleToTarget = Vector3.Angle(navMeshAgent.destination - transform.position, transform.forward);
                    if (Mathf.Abs(angleToTarget) < strafingThreshold)
                    {
                        // Shouldn't strafe
                        animator.SetBool(AnimParamIsStrafingRight, false);
                        animator.SetBool(AnimParamIsStrafingLeft, false);
                    }
                    else
                    {
                        // Should strafe
                        if (Vector3.Dot(transform.right, navMeshAgent.destination - transform.position) > 0)
                        {
                            animator.SetBool(AnimParamIsStrafingRight, true);
                        }
                        else
                        {
                            animator.SetBool(AnimParamIsStrafingLeft, true);
                        }
                    }
                }
                else
                {
                    // If there's no target anymore then shouldn't strafe
                    animator.SetBool(AnimParamIsStrafingRight, false);
                    animator.SetBool(AnimParamIsStrafingLeft, false);
                }
            }
        }

        private void HandleRunningAnimation()
        {
            if (navMeshAgent.velocity.magnitude > 1)
            {
                animator.SetBool(AnimParamIsRunning, true);
            }
            else
            {
                animator.SetBool(AnimParamIsRunning, false);
                animator.SetBool(AnimParamIsStrafingRight, false);
                animator.SetBool(AnimParamIsStrafingLeft, false);
            }
        }

        public void SetDestination(Vector3 destination)
        {
            navMeshAgent.SetDestination(destination);
        }
    }
}
