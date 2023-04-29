using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Net
{
    public class Net : MonoBehaviour
    {
        public void AttemptNet(List<Collider> colliderList)
        {
            foreach (var col in colliderList)
            {
                AttemptNet(col);
            }
        }
        
        private void AttemptNet(Collider col)
        {
            Netable netable = col.GetComponent<Netable>();
            if (netable == null)
                return;
            
            netable.GetNetted();
        }
    }
}