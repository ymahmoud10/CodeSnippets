using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace ObscureGames
{
    [RequireComponent(typeof(OGUnit))]
    public class OGWalker : MonoBehaviour
    {

        #region General Configs

        [Title("General Configs:")]
        public float walkingSpeed;
        public float runningSpeed;
        [MinMaxSlider(0, 10, true)]
        public Vector2 timeInWaypoint = new Vector2(3, 6);
        public List<Transform> waypoints;

        [Space]
        [Title("Debugging")]
        public bool drawWaypoints = false;
        public Color color = Color.blue;


        #endregion

        #region Private

        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private int _waypointIndex;
        private float _currentTimeInWaypoint;
        private bool _atWaypoint;

        #endregion


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _waypointIndex = 0;
            _currentTimeInWaypoint = Random.Range(timeInWaypoint.x, timeInWaypoint.y);
            _atWaypoint = true;
            _navMeshAgent.speed = walkingSpeed;
        }
        
        void Update()
        {
            _animator.SetFloat("Speed", _navMeshAgent.velocity.magnitude);
            
            if (_atWaypoint)
            {
                // If currently at waypoint
                _currentTimeInWaypoint -= Time.deltaTime;

                if (_currentTimeInWaypoint <= 0)
                {
                    // Time to move to next waypoint
                    _navMeshAgent.SetDestination(waypoints[_waypointIndex].position);
                    _waypointIndex = (_waypointIndex + 1) % waypoints.Count;
                    _atWaypoint = false;
                }
            }


            if (Vector3.Distance(transform.position, _navMeshAgent.destination) <= _navMeshAgent.stoppingDistance && !_atWaypoint)
            {
                // Arrived to waypoint
                _atWaypoint = true;
                _currentTimeInWaypoint = Random.Range(timeInWaypoint.x, timeInWaypoint.y);
            }

            var attacker = GetComponent<OGAttacker>();
            if (attacker && attacker.target)
            {
                _navMeshAgent.speed = runningSpeed;
                _navMeshAgent.SetDestination(attacker.target.position);
            }

        }

        private void OnDrawGizmos()
        {
            if (drawWaypoints)
            {
                Gizmos.color = color;
                foreach (var waypoint in waypoints)
                {
                    Gizmos.DrawSphere(waypoint.position, 0.2f);
                }
            }
        }
    }
}
