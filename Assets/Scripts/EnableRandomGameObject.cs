using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class EnableRandomGameObject : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _gameObjects = new List<GameObject>();

        private void Start()
        {
            int randomIndex = Random.Range(0, _gameObjects.Count);
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                _gameObjects[i].SetActive(i == randomIndex);
            }
        }
    }
}