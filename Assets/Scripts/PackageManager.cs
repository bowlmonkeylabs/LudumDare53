using System;
using System.Collections.Generic;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            Debug.Log(System.DateTime.Now.Millisecond);
        }

        void Start() {
            _houseContainer.Value.GetComponentsInChildren<House>()
                .ForEach(h => {
                    _packagesAtEachHouse.Add(new PackageHouse {
                            spawnPoint = h.PackageSpawnPoint,
                            packages = new List<Package>()
                        }); 
                });

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

            PackageHouse houseToSpawnAt = _packagesAtEachHouse.OrderBy(p => UnityEngine.Random.value).FirstOrDefault(ph => ph.packages.Count < _packageLimitPerHouse);

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
            Pirate pirate = _piratesContainer.Value.GetComponentsInChildren<Pirate>()
                .Where(p => p.IsPatrolling)
                .OrderBy(p => UnityEngine.Random.value)
                .FirstOrDefault();

            if(pirate != null) {
                _packagesAtEachHouse.OrderBy(p => UnityEngine.Random.value)
                    .ForEach(ph => {
                        var package = ph.packages.Find(p => !p.PackageAssigned);

                        if(package != null) {
                            Debug.Log("Assigned package to pirate: " + pirate.gameObject.name);
                            pirate.SetTargetPackage(package.transform.position);
                            package.PackageAssigned = true;
                            return;
                        }
                    });
            }
        }
    }
}