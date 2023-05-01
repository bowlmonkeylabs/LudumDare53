using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using BML.ScriptableObjectCore.Scripts.Variables.ValueReferences;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BML.Scripts.Utils
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyVariableSetter : MonoBehaviour
    {
        [SerializeField, Required] private Rigidbody _rigidbody;

        [SerializeField] private FloatReference _drag;
        
        #region Unity lifecycle

        private void OnEnable()
        {
            if (_drag != null)
            {
                UpdateDrag();
                _drag.Subscribe(UpdateDrag);
            }
        }

        private void OnDisable()
        {
            if (_drag != null) _drag.Unsubscribe(UpdateDrag);
        }

        #endregion

        private void UpdateDrag()
        {
            Debug.Log($"Update drag ({_drag.Value})");
            _rigidbody.drag = _drag.Value;
        }
    }
}