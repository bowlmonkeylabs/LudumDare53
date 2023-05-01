using System;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using BML.ScriptableObjectCore.Scripts.Variables;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using Sirenix.Utilities;
using System.Linq;

namespace DefaultNamespace
{
    public class Pirate : MonoBehaviour
    {
        [SerializeField] private Patrol _patrol;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private float _captureTime = 3f;
        [SerializeField] private IntVariable _score;
        [SerializeField] private TransformSceneReference _packageContainer;
        [SerializeField] private TransformSceneReference vanContainerSceneReference;
        [SerializeField] private int _positiveScoreOnDropPackage = 5;
        [SerializeField] EvaluateCurveVariable _negativeScoreOnGrabPackage;
        [SerializeField] private int _negativeScoreOnTakeToVan = 10;
        [SerializeField] private bool _destroyPackageOnDrop = false;

        [SerializeField] private DynamicGameEvent _onStartGrabPackage;
        [SerializeField] private DynamicGameEvent _onGrabPackage;
        [SerializeField] private DynamicGameEvent _onCapturePackage;
        [SerializeField] private DynamicGameEvent _onDropPackage;

        [ShowInInspector, ReadOnly] private PirateState pirateState = PirateState.Patrolling;

        public class OnGrabPackagePayload
        {
            public Vector3 Position;
        }

        public class OnCapturePackagePayload
        {
            
        }

        public class OnDropPackagePayload
        {
            public Vector3 Position;
        }

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

        private void Awake() {
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
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
            
            _onStartGrabPackage.Raise(new OnGrabPackagePayload()
            {
                Position = transform.position
            });
        }

        private void GrabPackage()
        {
            pirateState = PirateState.TakingPackageToVan;

            _grabbablePackage.OnStoop = false;
            _grabbablePackage.transform.parent = this.transform;

            // vanContainerSceneReference.Value.GetComponentsInChildren<Transform>()
            // .OrderBy(v => UnityEngine.Random.value);
            var van = vanContainerSceneReference.Value.GetChild((int) Mathf.Floor(UnityEngine.Random.value * vanContainerSceneReference.Value.childCount));
            _agent.SetDestination(van.position);

            _score.Value -= Mathf.RoundToInt(_negativeScoreOnGrabPackage.Value);
            
            _onGrabPackage.Raise(new OnGrabPackagePayload()
            {
                Position = transform.position
            });
        }

        private void CapturePackage()
        {
            //TODO: Add logic for capture here
            _grabbablePackage.DoDestroy(false);
            _score.Value -= _negativeScoreOnTakeToVan;
            
            _onCapturePackage.Raise(new OnCapturePackagePayload()
            {
                
            });
            
            Destroy(this.gameObject);
        }

        public void DropPackage()
        {
            if (_grabbablePackage == null)
                return;

            if(_destroyPackageOnDrop) {
                _grabbablePackage.DoDestroy();
                return;
            }

            _grabbablePackage.AssignedToPirate = null;
            _grabbablePackage.OnStoop = (pirateState == PirateState.CapturingPackage);
            _grabbablePackage.transform.parent = _packageContainer.Value;
            if (pirateState == PirateState.TakingPackageToVan)
            {
                _grabbablePackage.Drop();
            }
            _score.Value += _positiveScoreOnDropPackage;
            
            _onDropPackage.Raise(new OnDropPackagePayload()
            {
                Position = transform.position
            });
        }

        public void DestroySelf() {
            if (_grabbablePackage != null) {
                _grabbablePackage.DoDestroy(false);
            }

            Destroy(this.gameObject);
        }
    }
}
