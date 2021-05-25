using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace ObscureGames
{
    [RequireComponent(typeof(OGUnit))]
    public class OGKillable : MonoBehaviour
    {
        #region General Configs

        [Title("General Configs:")]
        public float healthPoints = 100;
        public bool isAlive = true;

        #endregion

        #region References
        
        [Space]
        [Title("References:")]
        public Transform bloodEffect;

        #endregion

        #region Private

        private Animator _animator;

        #endregion

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        public void GetHit(float damage, Vector3 point, OGControlledAttacker attacker)
        {
            var thisAttacker = GetComponent<OGAttacker>();
            if (thisAttacker)
            {
                thisAttacker.target = attacker.transform;
            }
            var targetPosition = new Vector3(transform.position.x, attacker.bulletSpawnPoint.position.y - 0.5f, transform.position.z);
            Quaternion rotation = Quaternion.LookRotation(targetPosition - attacker.bulletSpawnPoint.position);
            _animator.SetTrigger("GotHit");
            
            // Blood effect
            var blood = Instantiate(bloodEffect, point, rotation);
            Destroy(blood.gameObject, 1f);
            
            // TODO separate method
            healthPoints -= damage;
            if (healthPoints <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isAlive = false;
            var walker = GetComponent<OGWalker>();
            if (walker)
            {
                walker.GetComponent<NavMeshAgent>().enabled = false;
                walker.enabled = false;
            }
            _animator.SetTrigger("Died");
            BattleManager.Instance.zombies.Remove(GetComponent<OGUnit>());
            
        }
    }
}