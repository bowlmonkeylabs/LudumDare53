using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace DefaultNamespace
{
    public class Pirate : MonoBehaviour
    {
        [SerializeField] private Patrol _patrol;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _captureTime = 3f;

        [ShowInInspector, ReadOnly] private PirateState pirateState = PirateState.Patrolling;

        private Vector3 destination;
        private Vector3 vanPosition;
        private float arriveAtPackageTime = Mathf.NegativeInfinity;

        [Serializable] 
        public enum PirateState
        {
            Patrolling,
            WalkingToPackage,
            CapturingPackage,
            TakingPackageToVan
        }

        private void Update()
        {
            switch (pirateState)
            {
                case (PirateState.Patrolling):
                    if (!_patrol.enabled) _patrol.enabled = true;
                    break;
                case (PirateState.WalkingToPackage):
                    if (HasReachedDestination())
                        ArriveAtPackage();
                    break;
                case (PirateState.CapturingPackage):
                    if (arriveAtPackageTime + _captureTime < Time.time)
                    {
                        GrabPackage();
                    }
                    break;
                case (PirateState.TakingPackageToVan):
                    if (HasReachedDestination())
                        CapturePackage();
                    break;
                default:
                    break;
            }
        }

        private bool HasReachedDestination()
        {
            return Vector3.Distance(transform.position, destination) < .5f;
        }

        public void SetTargetPackage(Vector3 packagePos)
        {
            pirateState = PirateState.WalkingToPackage;
            _patrol.enabled = false;
            destination = packagePos;
            _agent.SetDestination(packagePos);
        }

        private void ArriveAtPackage()
        {
            pirateState = PirateState.CapturingPackage;
            arriveAtPackageTime = Time.time;
        }

        private void GrabPackage()
        {
            pirateState = PirateState.TakingPackageToVan;
            
            //TODO: Get this van position dynamically
            _agent.SetDestination(vanPosition);
        }

        private void CapturePackage()
        {
            //TODO: Add logic for capture here
        }
    }
}