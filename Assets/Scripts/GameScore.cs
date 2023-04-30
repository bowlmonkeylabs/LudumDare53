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
    }
}