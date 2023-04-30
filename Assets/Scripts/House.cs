using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private float _maxTimeOnStoop = 20;

    [ReadOnly] public Transform PackageSpawnPoint;
    [ReadOnly] public List<Package> packages = new List<Package>();

    void Update() {
        packages.Where(p => p.OnStoop && p.TimeOnStoop >= _maxTimeOnStoop).ToList().ForEach(p => {
            packages.Remove(p);
            Destroy(p.gameObject);
        });
    }
}
