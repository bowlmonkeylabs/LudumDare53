using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using UnityEngine;

namespace DefaultNamespace
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private TimerReference _levelTimer;
        [SerializeField] private IntReference _levelScore;
        [SerializeField] private GameScore _gameScore;

        #region Unity lifecycle

        private void Start()
        {
            _levelTimer.RestartTimer();
        }

        private void OnEnable()
        {
            _levelTimer.SubscribeFinished(RecordLevelScore);
        }

        private void OnDisable()
        {
            _levelTimer.UnsubscribeFinished(RecordLevelScore);
        }

        private void FixedUpdate()
        {
            _levelTimer.UpdateTime();
        }

        #endregion

        private void RecordLevelScore()
        {
            _gameScore.RecordScore(_levelScore.Value);
        }
    }
}