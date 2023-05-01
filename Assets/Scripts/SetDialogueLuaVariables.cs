using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace BML.Scripts
{
    public class SetDialogueLuaVariables : MonoBehaviour
    {
        [SerializeField] private BoolVariable _wonPreviousDay;
        private void Start()
        {
            DialogueLua.SetVariable("WonPreviousDay", _wonPreviousDay.Value);
        }
    }
}