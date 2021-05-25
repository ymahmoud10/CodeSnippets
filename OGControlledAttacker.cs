using UnityEngine;

namespace ObscureGames
{
    [RequireComponent(typeof(OGUnit))]
    public class OGControlledAttacker : MonoBehaviour
    {
        public Transform target;
        public float radius = 6f;
        public float shootingDelay = 1f;
        public float damage = 10f;
        public Transform bulletSpawnPoint;
        public Transform gunshotEffectPrefab;

        
        private OGUnit unit;
        private Animator animator;
        private float shootingTimeout;

        #region AnimatorParams
        
        private static readonly int AnimParamIsShooting = Animator.StringToHash("IsShooting");

        #endregion

        private void Awake()
        {
            unit = GetComponent<OGUnit>();
            animator = GetComponent<Animator>();
            shootingTimeout = shootingDelay;
        }
        
        private void OnEnable()
        {
            unit.isControlledAttacker = true;
            unit.controlledAttacker = this;
        }

        private void OnDisable()
        {
            unit.isControlledAttacker = false;
            unit.controlledAttacker = null;
        }

        void Update()
        {
            LookForTarget();

            if (target)
            {
                if (shootingTimeout < 0)
                {
                    Shoot();
                    shootingTimeout = shootingDelay;
                }
                else
                {
                    shootingTimeout -= Time.deltaTime;
                }
            }
            else
            {
                shootingTimeout = shootingDelay;
            }
        }
        
        private void LookForTarget()
        {
            var minDistance = float.MaxValue;
            OGUnit closestZombie = null;
            foreach (OGUnit zombie in BattleManager.Instance.zombies)
            {
                var thisDistance = Vector3.Distance(transform.position, zombie.transform.position);
                if (thisDistance < minDistance && thisDistance < radius)
                {
                    closestZombie = zombie;
                    minDistance = thisDistance;
                }
            }
        
            if (closestZombie)
            {
                target = closestZombie.transform;
                animator.SetBool(AnimParamIsShooting, true);
            }
            else
            {
                target = null;
                animator.SetBool(AnimParamIsShooting, false);
            }
        }
        
        private void Shoot()
        {
            var targetPosition = new Vector3(target.position.x, bulletSpawnPoint.position.y - 0.5f, target.position.z);
            Ray ray = new Ray(bulletSpawnPoint.position, targetPosition - bulletSpawnPoint.position);
            if (Physics.Raycast(ray, out var hit, 10f))
            {
                OGUnit hitUnit = hit.transform.GetComponent<OGUnit>();
                if (hitUnit && hitUnit.unitType == OGUnit.UnitType.Zombie)
                {
                    var effect = Instantiate(gunshotEffectPrefab, bulletSpawnPoint);
                    Destroy(effect.gameObject, 2f);
                    hit.transform.GetComponent<OGKillable>().GetHit(damage, hit.point, this);
                }
            }
            else
            {
                Debug.Log("Didn't hit");
            }
            
        }
    }
}