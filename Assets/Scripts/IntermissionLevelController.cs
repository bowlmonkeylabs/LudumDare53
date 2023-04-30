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
        DialogueLua.SetVariable("CurrentLevel", _currentLevel.Value);
    }

    public void LoadNextLevel() {
        SceneManager.LoadScene("Level" + _currentLevel.Value);
    }
}
