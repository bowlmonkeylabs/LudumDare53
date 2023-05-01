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

        public void TryReturn()
        {
            if (OnStoop)
                return;

            transform.position = house.PackageSpawnPoint.position;
            OnStoop = true;
            _onReturnPackage.Invoke();
        }

        public void DoDestroy(bool unassignFromPirate = true)
        {
            if(AssignedToPirate != null && unassignFromPirate) {
                AssignedToPirate.UnSetTargetPackage(this);
            }
            _onDroppedFeedbacks.StopFeedbacks();
            house.packages.Remove(this);
        }

        public void FixedUpdate() {
            if(OnStoop) {
                TimeOnStoop += Time.deltaTime;
            }
        }

        public void Drop()
        {
            IsDropped = true;
            _onDroppedFeedbacks.PlayFeedbacks();
        }
    }
}
