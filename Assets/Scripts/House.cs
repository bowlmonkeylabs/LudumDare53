using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace DefaultNamespace
{
    public class House : MonoBehaviour
    {
        [SerializeField] private float _maxTimeOnStoop = 20;

        public Transform PackageSpawnPoint;
        [ReadOnly] public List<Package> packages = new List<Package>();

        void Update() {
            packages.Where(p => p.OnStoop && p.TimeOnStoop >= _maxTimeOnStoop).ToList().ForEach(p => {
                p.DoDestroy();
            });
        }
    }
}
