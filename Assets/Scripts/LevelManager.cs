using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private TimerReference _levelTimer;
        [SerializeField] private IntReference _currentLevel;
        [SerializeField] private IntReference _levelScore;
        [SerializeField] private BoolReference _wonPreviousDay;
        [SerializeField] private GameScore _gameScore;
        [SerializeField] private GameScore _goalScore;
        [SerializeField] private bool _isBossLevel = false;
        [SerializeField] private UnityEvent _onTimerEnd;

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
            if(_isBossLevel) {
                _gameScore.RecordBossScore(_levelScore.Value);
            } else {
                _gameScore.RecordScore(_levelScore.Value);
            }
            
            _wonPreviousDay.Value = (_gameScore.LevelScores[_currentLevel.Value].Score >=
                                         _goalScore.LevelScores[_currentLevel.Value].Score);

            _onTimerEnd.Invoke();
        }
    }
}