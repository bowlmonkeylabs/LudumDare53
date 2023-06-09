using BML.ScriptableObjectCore.Scripts.Variables;
using DefaultNamespace;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntermissionLevelController : MonoBehaviour
{
    [SerializeField] private GameScore _gameScores;
    [SerializeField] private IntVariable _currentLevel;

    void Awake() {
        SetDialogCurrentLevel();
    }

    void Update() {
        SetDialogCurrentLevel();
    }

    public void LoadNextLevel() {
        //SceneManager.LoadScene("Level" + _currentLevel.Value);
        if (_currentLevel.Value >= 2)
        {
            SceneManager.LoadScene("RocketLaunch");
        }
        else
        {
            SceneManager.LoadScene("Level0");
        }
    }

    private void SetDialogCurrentLevel() {
        DialogueLua.SetVariable("CurrentLevel", _currentLevel.Value);
    }
}
