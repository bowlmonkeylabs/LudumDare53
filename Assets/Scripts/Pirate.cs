using System;
using BML.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using Sirenix.Utilities;

namespace DefaultNamespace
{
    public class Pirate : MonoBehaviour
    {
        [SerializeField] private Patrol _patrol;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _captureTime = 3f;
        [SerializeField] private TransformSceneReference _packageContainer;
        [SerializeField] private TransformSceneReference vanSceneReference;

        [ShowInInspector, ReadOnly] private PirateState pirateState = PirateState.Patrolling;

        private float arriveAtPackageTime = Mathf.NegativeInfinity;
        private Package _grabbablePackage;

        [Serializable] 
        public enum PirateState
        {
            Patrolling,
            WalkingToPackage,
            CapturingPackage,
            TakingPackageToVan
        }

        public bool IsPatrolling {
            get {
                return pirateState == PirateState.Patrolling;
            }
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
            return Vector3.Distance(transform.position, _agent.destination) < .5f;
        }

        public void SetTargetPackage(Package package)
        {
            pirateState = PirateState.WalkingToPackage;
            _grabbablePackage = package;
            _patrol.enabled = false;
            _agent.SetDestination(_grabbablePackage.transform.position);
        }

        public void UnSetTargetPackage(Package package)
        {
            pirateState = PirateState.Patrolling;
            _grabbablePackage = null;
        }

        private void ArriveAtPackage()
        {
            pirateState = PirateState.CapturingPackage;
            arriveAtPackageTime = Time.time;
        }

        private void GrabPackage()
        {
            pirateState = PirateState.TakingPackageToVan;

            _grabbablePackage.OnStoop = false;
            _grabbablePackage.transform.parent = this.transform;
            
            //TODO: Get this van position dynamically
            _agent.SetDestination(vanSceneReference.Value.position);
        }

        private void CapturePackage()
        {
            //TODO: Add logic for capture here
            _grabbablePackage.DoDestroy();
            Destroy(this.gameObject);
        }

        public void DropPackage()
        {
            if (_grabbablePackage == null)
                return;

            _grabbablePackage.AssignedToPirate = null;
            _grabbablePackage.OnStoop = false;
            _grabbablePackage.transform.parent = _packageContainer.Value;
        }
    }
}
