using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Game_Score", menuName = "GameScore", order = 0)]
    public class GameScore : ScriptableObject
    {
        [Serializable]
        public class LevelScore
        {
            public int Score;
        }

        public List<LevelScore> LevelScores;
        public LevelScore BossScore;

        public void RecordScore(int score)
        {
            LevelScores.Add(new LevelScore { Score = score });
        }

        public void RecordBossScore(int score)
        {
            BossScore = new LevelScore { Score = score };
        }

        public void Reset()
        {
            if (LevelScores == null) LevelScores = new List<LevelScore>();
            else LevelScores.Clear();
        }
    }
}