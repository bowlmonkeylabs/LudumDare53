﻿using System;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class EntitySpawner : MonoBehaviour
    {
        [SerializeField] private TransformSceneReference _container;
        [SerializeField] private Transform _spawnPointContainer;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _maxCount = 6;
        [SerializeField] private float _spawnDelay = 5f;

        private float lastSpawnTime = Mathf.NegativeInfinity;

        private void Update()
        {
            if (lastSpawnTime + _spawnDelay > Time.time ||
                _container.Value.childCount >= _maxCount)
                return;

            Transform randomSpawnPoint = _spawnPointContainer.GetChild(Random.Range(0, _spawnPointContainer.childCount));
            Instantiate(_prefab, randomSpawnPoint.position, Quaternion.identity, _container.Value);
            lastSpawnTime = Time.time;
        }
    }
}