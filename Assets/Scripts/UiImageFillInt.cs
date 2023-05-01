using System.Collections;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.Variables;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UiImageFillInt : MonoBehaviour
    {
        [SerializeField] private IntVariable _currentAmount;
        [SerializeField] private GameScore _goalScores;
        [SerializeField] private IntVariable _currentLevel;
        [SerializeField] private Image _image;

        void OnEnable() {
            _currentAmount.Subscribe(SetFillAmount);
        }

        void OnDisable() {
            _currentAmount.Unsubscribe(SetFillAmount);
        }

        private void SetFillAmount() {
            _image.fillAmount = (float) _currentAmount.Value / (float) _goalScores.LevelScores[_currentLevel.Value].Score;
        }
    }
}
