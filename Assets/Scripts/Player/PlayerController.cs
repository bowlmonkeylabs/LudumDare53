using System;
using BML.Scripts;
using BML.Scripts.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("References")] private Transform _mainCamera;
        [SerializeField, FoldoutGroup("References")] private Transform _firePoint;

        [SerializeField, FoldoutGroup("Interact")] private float _interactDistance = 5f;
        [SerializeField, FoldoutGroup("Interact")] private float _interactCastRadius = .25f;
        [SerializeField, FoldoutGroup("Interact")] private LayerMask _interactMask;

        [SerializeField, FoldoutGroup("NetGun")] private bool _hasNetGun;
        [SerializeField, FoldoutGroup("NetGun")] private GameObject _netPrefab;
        [SerializeField, FoldoutGroup("NetGun")] private Transform _netContainer;
        [SerializeField, FoldoutGroup("NetGun")] private float _netGunCooldown = .3f;
        [SerializeField, FoldoutGroup("NetGun")] private float _netGunForce = 1000f;

        private float lastFireNetTime = Mathf.NegativeInfinity;

        #region Input Callback

        private void OnPrimary(InputValue value)
        {
            if (!value.isPressed) return;
            
            if (_hasNetGun)
                TryFireNetGun();
            else
                TryInteract();
        }

        #endregion

        private void TryFireNetGun()
        {
            if (lastFireNetTime + _netGunCooldown > Time.time)
                return;

            GameObject net = Instantiate(_netPrefab, _firePoint.position, _mainCamera.rotation, _netContainer);
            net.GetComponent<Rigidbody>().AddForce(_netGunForce * _mainCamera.forward);
            lastFireNetTime = Time.time;
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