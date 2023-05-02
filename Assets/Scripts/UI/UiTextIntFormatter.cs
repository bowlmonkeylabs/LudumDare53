using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace BML.Scripts.UI
{
    public class UiTextIntFormatter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _formatString = "P0";
        [SerializeField] private IntVariable _variable;

        #if UNITY_EDITOR
        [Button]
        private void TestFormat()
        {
            Debug.Log(GetFormattedValue());
        }
        #endif

        private void Start()
        {
            UpdateText();
            _variable.Subscribe(UpdateText);
        }

        private void OnDestroy()
        {
            _variable.Unsubscribe(UpdateText);
        }

        protected string GetFormattedValue()
        {
            return _variable.Value.ToString(_formatString);
        }

        protected void UpdateText()
        {
            _text.text = GetFormattedValue();
        }
    }
}