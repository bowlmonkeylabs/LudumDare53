using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class LevelPerformanceActions : MonoBehaviour
    {
        [SerializeField] private GameScore _gameScore;
        [SerializeField] private GameScore _gameScoreGoals;

        [SerializeField] private bool _useBossScore = false;

        [SerializeField] private int _thresholdOne = 0;
        [SerializeField] private int _thresholdTwo = 1;
        [SerializeField] private int _thresholdThree = 3;

        [SerializeField] private UnityEvent _thresholdOneAction;
        [SerializeField] private UnityEvent _thresholdTwoAction;
        [SerializeField] private UnityEvent _thresholdThreeAction;
        [SerializeField] private UnityEvent _defaultAction;

        public void PerformAction() {
            int thresholdReached = 0;

            for(int i = 0; i < _gameScoreGoals.LevelScores.Count; i++) {
                int scored = i < _gameScore.LevelScores.Count ? _gameScore.LevelScores[i].Score : 0;
                thresholdReached += scored >= _gameScoreGoals.LevelScores[i].Score ? 1 : 0;
            }

            if(_useBossScore && _gameScore.BossScore.Score < _gameScoreGoals.BossScore.Score) {
                thresholdReached = 0;
            }

            if(thresholdReached >= _thresholdThree) {
                _thresholdThreeAction.Invoke();
            } else if(thresholdReached >= _thresholdTwo) {
                _thresholdTwoAction.Invoke();
            } else if(thresholdReached >= _thresholdOne) {
                _thresholdOneAction.Invoke();
            } else {
                _defaultAction.Invoke();
            }
        }
    }
}
