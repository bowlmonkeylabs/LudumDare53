using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Variables;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace
{
    public class House : MonoBehaviour
    {
        [SerializeField] private IntVariable _score;

        [SerializeField] private float _maxTimeOnStoop = 20;
        [SerializeField] private int _positiveScoreOnHomeownerGrabPackage = 5;

        public Transform PackageSpawnPoint;
        [ReadOnly] public List<Package> packages = new List<Package>();

        private float _warningIndicatorUpdatePeriod = 1.5f;
        private float _warningIndicatorLastUpdateTime;
        private float _warningIndicatorNextUpdateTime;
        [SerializeField] private Transform _warningIndicator;
        [SerializeField] private MMF_Player _warningIndicatorUrgentFeedbacks;

        #region Unity lifecycle

        private void FixedUpdate()
        {
            for (int i = 0; i < packages.Count; i++)
            {
                var package = packages[i];
                if (package.OnStoop && package.TimeOnStoop >= _maxTimeOnStoop)
                {
                    _score.Value += _positiveScoreOnHomeownerGrabPackage;
                    RemovePackage(package);
                    i--;
                    package.DoDestroy();
                }
            }

            if (Time.time >= _warningIndicatorNextUpdateTime)
            {
                UpdateWarningIndicator();
            }
        }

        #endregion

        #region Public interface

        public void AddPackage(Package package)
        {
            package.OnPirateReached += UpdateWarningIndicator;
            package.OnPirateGrabbed += UpdateWarningIndicator;
            package.OnPirateCaptured += UpdateWarningIndicator;
            package.OnPlayerReturned += UpdateWarningIndicator;
            packages.Add(package);
        }

        public void RemovePackage(Package package)
        {
            package.OnPirateReached -= UpdateWarningIndicator;
            package.OnPirateGrabbed -= UpdateWarningIndicator;
            package.OnPirateCaptured -= UpdateWarningIndicator;
            package.OnPlayerReturned -= UpdateWarningIndicator;
            packages.Remove(package);
        }

        #endregion

        private void UpdateWarningIndicator()
        {
            _warningIndicatorLastUpdateTime = Time.time;
            _warningIndicatorNextUpdateTime = _warningIndicatorLastUpdateTime + _warningIndicatorUpdatePeriod +
                                              UnityEngine.Random.Range(0.01f, 0.3f);
            
            bool warningVisible = packages.Any(p => 
                p.AssignedToPirate != null 
                && (p.AssignedToPirate.State == Pirate.PirateState.CapturingPackage ||
                    p.AssignedToPirate.State == Pirate.PirateState.TakingPackageToVan));
            
            bool urgentWarningVisible = packages.Any(p =>
                p.AssignedToPirate != null && p.AssignedToPirate.State == Pirate.PirateState.TakingPackageToVan);
            
            _warningIndicator.gameObject.SetActive(warningVisible);
            if (warningVisible && urgentWarningVisible)
            {
                _warningIndicatorUrgentFeedbacks.PlayFeedbacks();
            }
            else
            {
                _warningIndicatorUrgentFeedbacks.StopFeedbacks();
                _warningIndicatorUrgentFeedbacks.ResetFeedbacks();
            }
            
        }
    }
}
