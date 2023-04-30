using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Package : MonoBehaviour
{
    // public bool PackageDelivered = false;
    public bool PackageAssigned = false;
    public bool PackageAwayFromHouse = false;
    public House house;

    public void TryReturn()
    {
        if (!PackageAwayFromHouse)
            return;

        transform.position = house.PackageSpawnPoint.position;
        PackageAwayFromHouse = false;
    }
}
