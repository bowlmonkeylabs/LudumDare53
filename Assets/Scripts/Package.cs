using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class Package : MonoBehaviour
    {
        [ReadOnly] public Pirate AssignedToPirate = null;
        [ReadOnly] public bool OnStoop = true;
        [ReadOnly] public House house;
        [ReadOnly] public float TimeOnStoop;
        [ReadOnly] public bool IsDropped = false;

        [SerializeField] private MMF_Player _onDroppedFeedbacks;

        [SerializeField] private UnityEvent _onReturnPackage;

        public delegate void OnPirateAssignedCallback();
        public event OnPirateAssignedCallback OnPirateAssigned;
        
        public delegate void OnPirateReachedCallback();
        public event OnPirateReachedCallback OnPirateReached;
        
        public delegate void OnPirateGrabbedCallback();
        public event OnPirateGrabbedCallback OnPirateGrabbed;
        
        public delegate void OnPirateCapturedCallback();
        public event OnPirateCapturedCallback OnPirateCaptured;
        
        public delegate void OnPlayerReturnedCallback();
        public event OnPlayerReturnedCallback OnPlayerReturned;

        public void TryReturn()
        {
            if (!IsDropped)
                return;
            
            _onDroppedFeedbacks.StopFeedbacks();
            _onDroppedFeedbacks.ResetFeedbacks();

            OnPlayerReturned?.Invoke();
            _onReturnPackage.Invoke();
            transform.position = house.PackageSpawnPoint.position;
            OnStoop = true;
            IsDropped = false;
        }

        public void DoDestroy(bool unassignFromPirate = true)
        {
            if(AssignedToPirate != null && unassignFromPirate) {
                AssignedToPirate.UnSetTargetPackage(this);
            }
            _onDroppedFeedbacks.StopFeedbacks();
            house.RemovePackage(this);
            Destroy(this.gameObject);
        }

        public void FixedUpdate() {
            if (OnStoop) {
                TimeOnStoop += Time.deltaTime;
            }
        }

        public void Assign(Pirate pirate)
        {
            AssignedToPirate = pirate;
            OnPirateAssigned?.Invoke();
        }

        public void StartGrab(Pirate pirate)
        {
            OnPirateReached?.Invoke();
        }

        public void Grab(Pirate pirate)
        {
            OnStoop = false;
            transform.parent = pirate.transform;
            OnPirateGrabbed?.Invoke();
            
        }

        public void Drop()
        {
            IsDropped = true;
            _onDroppedFeedbacks.PlayFeedbacks();
        }

        public void Capture()
        {
            OnPirateCaptured?.Invoke();
            
            DoDestroy(false);
        }
    }
}
