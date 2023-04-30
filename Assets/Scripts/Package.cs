using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace
{
    public class Package : MonoBehaviour
    {
        [ReadOnly] public Pirate AssignedToPirate = null;
        [ReadOnly] public bool OnStoop = true;
        [ReadOnly] public House house;
        [ReadOnly] public float TimeOnStoop;

        public void TryReturn()
        {
            if (OnStoop)
                return;

            transform.position = house.PackageSpawnPoint.position;
            OnStoop = true;
        }

        public void DoDestroy(bool unassignFromPirate = true)
        {
            if(AssignedToPirate != null && unassignFromPirate) {
                AssignedToPirate.UnSetTargetPackage(this);
            }
            house.packages.Remove(this);
            Destroy(this.gameObject);
        }

        public void FixedUpdate() {
            if(OnStoop) {
                TimeOnStoop += Time.deltaTime;
            }
        }
    }
}
