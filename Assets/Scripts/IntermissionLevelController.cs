using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntermissionLevelController : MonoBehaviour
{
    [SerializeField] private GameScore _gameScores;

    public void LoadNextLevel() {
        SceneManager.LoadScene("Level" + _gameScores.LevelScores.Count);
    }
}
