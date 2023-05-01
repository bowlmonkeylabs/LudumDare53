using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class CheckRocketFire : MonoBehaviour
    {
        [SerializeField] private LayerMask _fireLayerMask;
        [SerializeField] private UnityEvent CheckSucceeded;

        public void Check() {
            if(Physics.Raycast(this.transform.position, Vector3.up, 1000, _fireLayerMask)) {
                CheckSucceeded.Invoke();
            }
        }
    }
}
