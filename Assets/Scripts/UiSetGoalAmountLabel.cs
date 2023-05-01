using BML.ScriptableObjectCore.Scripts.Variables;
using UnityEngine;
using TMPro;

namespace DefaultNamespace
{
    public class UiSetGoalAmountLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private GameScore _goalScores;
        [SerializeField] private IntVariable _currentLevel;

        void OnEnable() {
            _text.text = "$" + _goalScores.LevelScores[_currentLevel.Value].Score;
        }
    }
}
