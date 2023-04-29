using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace.Net
{
    public class Netable : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnNet;
            
        public void GetNetted()
        {
            OnNet.Invoke();
        }
    }
}