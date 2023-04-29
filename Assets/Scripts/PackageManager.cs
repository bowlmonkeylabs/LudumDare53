using System;
using System.Collections.Generic;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct PackageHouse {
    public GameObject spawnPoint;
    public List<Package> packages;
}

public struct PackageDelivery {
    public GameObject spawnPoint;
    public Package package;
}
namespace DefaultNamespace
{
    public class PackageManager : MonoBehaviour
    {
        [SerializeField] private TransformSceneReference _piratesContainer;
        [SerializeField] private TransformSceneReference _houseContainer;
        [SerializeField] private TransformSceneReference _packageContainer;

        [SerializeField] private GameObject _packagePrefab;

//add back for drone delivery
        // [SerializeField] private DynamicGameEvent _requestPackage;

        [SerializeField] private float _packageSpawnTime = 2;
        [SerializeField] private int _packageLimitPerHouse = 1;
        //add back for drone delivery
        // [SerializeField] private float _packageDeliveryTime = 1;

        [SerializeField, ReadOnly] private List<PackageHouse> _packagesAtEachHouse = new List<PackageHouse>();

        private Timer _packageSpawnTimer;
        //add back for drone delivery
        // private Timer _packageDeliverTimer;

        void Awake() {
            _packageSpawnTimer = new Timer(_packageSpawnTime);
            //add back for drone delivery
            // _packageDeliverTimer = new Timer(_packageDeliveryTime);
        }

        void Start() {
            foreach(Transform houseTransform in _houseContainer.Value) {
                var house = houseTransform.GetComponent<House>();
                if(house == null) {
                    continue;
                }
                _packagesAtEachHouse.Add(new PackageHouse {
                    spawnPoint = house.PackageSpawnPoint,
                    packages = new List<Package>()
                });
            }

            _packageSpawnTimer.Start();
        }

        void Update() {
            TrySpawnPackage();
            TryAssignPirateToPackage();
        }

        void FixedUpdate() {
            _packageSpawnTimer.Tick();
            //add back for drone delivery
            // _packageDeliverTimer.Tick();
        }

        private void TrySpawnPackage() {
            if(!_packageSpawnTimer.IsFinished) {
                return;
            }

            PackageHouse houseToSpawnAt = _packagesAtEachHouse.Find(ph => ph.packages.Count < _packageLimitPerHouse);

            if(!houseToSpawnAt.Equals(default(PackageHouse))) {
                GameObject packageGameObject = Instantiate<GameObject>(_packagePrefab, houseToSpawnAt.spawnPoint.transform.position, Quaternion.identity, _packageContainer.Value);

                Package package = packageGameObject.GetComponent<Package>();

                houseToSpawnAt.packages.Add(package);

                Debug.Log("Spawned package at: " + package.transform.position);

                //add back for drone delivery
                // packageGameObject.SetActive(false);
                // _requestPackage.Raise(new PackageDelivery {
                //     spawnPoint = houseToSpawnAt.spawnPoint,
                //     package = package
                // });
            }

            _packageSpawnTimer.Start();
        }

        private void TryAssignPirateToPackage() {
            Pirate pirate = null;
            foreach(Transform pirateTransform in _piratesContainer.Value) {
                pirate = pirateTransform.GetComponent<Pirate>();

                if(pirate.IsPatrolling) {
                    break;
                } else {
                    pirate = null;
                }
            }

            if(pirate != null) {
                Package package = null;
                _packagesAtEachHouse.ForEach(ph => {
                    package = ph.packages.Find(p => !p.PackageAssigned);

                    if(package != null) {
                        Debug.Log("Assigned package to pirate: " + pirate.gameObject.name);
                        pirate.SetTargetPackage(package.transform.position);
                        package.PackageAssigned = true;
                        return;
                    }
                });
//                 var packageHouse = _packagesAtEachHouse.Find(ph => ph.packages.Count > 0);
                
//                 if(!packageHouse.Equals(default(PackageHouse))) {
//                     // Debug.Log("Found assignable pirate: " + pirate.gameObject.name);
//                     Debug.Log("Found assignable house: " + packageHouse.spawnPoint.transform.position);
//                     //add back for drone delivery 
//                     //&& p.PackageDelivered
//                     Debug.Log(packageHouse.packages.Where(p => !p.PackageAssigned).ToList().Count);
//                     Package package = packageHouse.packages.Find(p => !p.PackageAssigned);
// // Debug.Log(package != null);
// // package = packageHouse.packages.Select(p => !p.PackageAssigned).ToList().Count > 0 ? packageHouse.packages.Select(p => !p.PackageAssigned).ToArray()[0] : null;
// // Debug.Log(package != null);
//                     if(package != null) {
//                         Debug.Log("Assigned package to pirate: " + pirate.gameObject.name);
//                         pirate.SetTargetPackage(package.transform.position);
//                         package.PackageAssigned = true;
//                     }
//                 }
            }
        }
    }
}