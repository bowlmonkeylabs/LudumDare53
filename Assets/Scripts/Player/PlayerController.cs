using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using BML.Scripts;
using BML.Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("References")] private Transform _mainCamera;
        [SerializeField, FoldoutGroup("References")] private Transform _firePoint;
        [SerializeField, FoldoutGroup("References")] private BoolReference _isPlayerInputDisabled;

        [SerializeField, FoldoutGroup("Interact")] private float _interactDistance = 5f;
        [SerializeField, FoldoutGroup("Interact")] private float _interactCastRadius = .25f;
        [SerializeField, FoldoutGroup("Interact")] private LayerMask _interactMask;

        [SerializeField, FoldoutGroup("NetGun")] private bool _hasNetGun;
        [SerializeField, FoldoutGroup("NetGun")] private GameObject _netPrefab;
        [SerializeField, FoldoutGroup("NetGun")] private GameObject netGunUIContainer;
        [SerializeField, FoldoutGroup("NetGun")] private Transform _netContainer;
        [SerializeField, FoldoutGroup("NetGun")] private float _netGunCooldown = .3f;
        [SerializeField, FoldoutGroup("NetGun")] private float _netGunForce = 1000f;
        [SerializeField, FoldoutGroup("NetGun")] private UnityEvent _onFireNet;

        [SerializeField, FoldoutGroup("Caffeine")] private IntReference _caffeineUsesRemaining;
        [SerializeField, FoldoutGroup("Caffeine")] private BoolReference _isCaffeinated;
        [SerializeField] private TimerReference _caffeineTimer;

        private float lastFireNetTime = Mathf.NegativeInfinity;

        #region Unity Lifecycle

        private void OnEnable()
        {
            _caffeineTimer.SubscribeFinished(DisableCaffeine);
        }

        private void OnDisable()
        {
            _caffeineTimer.UnsubscribeFinished(DisableCaffeine);
        }

        private void Update()
        {
            if (_hasNetGun && !netGunUIContainer.activeSelf)
            {
                netGunUIContainer.SetActive(true);
            }
            if (!_hasNetGun && netGunUIContainer.activeSelf)
            {
                netGunUIContainer.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            if (_caffeineTimer != null)
            {
                _caffeineTimer.UpdateTime();
            }
        }

        #endregion

        #region Input Callback

        private void OnPrimary(InputValue value)
        {
            if (_isPlayerInputDisabled.Value) return;
            if (!value.isPressed) return;
            
            if (_hasNetGun)
                TryFireNetGun();
            else
                TryInteract();
        }

        private void OnUseCaffeine(InputValue value)
        {
            if (_isPlayerInputDisabled.Value) return;
            if (!value.isPressed) return;
            
            TryUseCaffeine();
        }

        #endregion

        private void TryFireNetGun()
        {
            if (lastFireNetTime + _netGunCooldown > Time.time)
                return;
            
            GameObject net = Instantiate(_netPrefab, _firePoint.position, _mainCamera.rotation, _netContainer);
            net.GetComponent<Rigidbody>().AddForce(_netGunForce * _mainCamera.forward);
            lastFireNetTime = Time.time;
            _onFireNet.Invoke();
        }

        private void TryUseCaffeine()
        {
            Debug.Log($"TryUseCaffeine (IsCaffeinated {_isCaffeinated.Value}) (TimerStarted {_caffeineTimer.HasStarted}) (TimerFinished {_caffeineTimer.IsFinished})");
            if (!_isCaffeinated.Value && (!_caffeineTimer.HasStarted || _caffeineTimer.IsFinished))
            {
                _isCaffeinated.Value = true;
                Debug.Log("Start Caffeine");
                
                _caffeineTimer.RestartTimer();
            }
        }

        private void DisableCaffeine()
        {
            _isCaffeinated.Value = false;
            Debug.Log("End Caffeine");

            _caffeineTimer.ResetTimer();
        }

        private void TryInteract()
        {
            RaycastHit hit;
            
            if (Physics.SphereCast(_mainCamera.position, _interactCastRadius, _mainCamera.forward, out hit,
                _interactDistance, _interactMask, QueryTriggerInteraction.Ignore))
            {
                InteractionReceiver interactionReceiver = hit.collider.GetComponent<InteractionReceiver>();
                if (interactionReceiver == null)
                    return;

                interactionReceiver.ReceiveInteraction();
            }
        }
        
    }
}