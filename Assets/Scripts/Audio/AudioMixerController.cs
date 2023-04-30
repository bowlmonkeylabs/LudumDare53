using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace BML.Scripts
{
    public class AudioMixerController : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private FloatReference _settingMasterVolume;

        private void Start()
        {
            SetMasterVolume(0f, _settingMasterVolume.Value);
        }

        private void OnEnable()
        {
            SetMasterVolume(0f, _settingMasterVolume.Value);
            _settingMasterVolume.Subscribe(SetMasterVolume);
        }
        
        private void OnDisable()
        {
            _settingMasterVolume.Unsubscribe(SetMasterVolume);
        }

        private void SetMasterVolume(float previousVolume, float newVolume)
        {
            float volume = ProcessVolume(newVolume);
            _audioMixer.SetFloat("MasterVolume", volume);
        }

        private float ProcessVolume(float volume)
        {
            float processedVolume;
            
            if (Mathf.Approximately(volume, 0f))
                processedVolume = -80f;
            else
                processedVolume = Mathf.Log10(volume) * 20f;

            return processedVolume;
        }
    }
}