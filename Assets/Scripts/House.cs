using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Variables;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace DefaultNamespace
{
    public class House : MonoBehaviour
    {
        [SerializeField] private IntVariable _score;

        [SerializeField] private float _maxTimeOnStoop = 20;
        [SerializeField] private int _positiveScoreOnHomeownerGrabPackage = 5;

        public Transform PackageSpawnPoint;
        [ReadOnly] public List<Package> packages = new List<Package>();

        void Update()
        {
            for (int i = 0; i < packages.Count; i++)
            {
                var package = packages[i];
                if (package.OnStoop && package.TimeOnStoop >= _maxTimeOnStoop)
                {
                    _score.Value += _positiveScoreOnHomeownerGrabPackage;
                    packages.Remove(package);
                    i--;
                    package.DoDestroy();
                }
            }
        }
    }
}
