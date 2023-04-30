using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Package : MonoBehaviour
{
    [ReadOnly] public bool AssignedToPirate = false;
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

    public void FixedUpdate() {
        if(OnStoop) {
            TimeOnStoop += Time.deltaTime;
        }
    }
}
