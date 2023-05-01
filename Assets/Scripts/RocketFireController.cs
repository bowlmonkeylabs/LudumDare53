using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class RocketFireController : MonoBehaviour
    {
        [SerializeField] private float _fireRocketCooldown;
        [SerializeField] private float _fireRocketDuration;
        [SerializeField] private GameObject _fireObj;
        private Timer _fireRocketCooldownTimer;
        private Timer _fireRocketDurationTimer;

        void Awake() {
            _fireObj.SetActive(false);
            _fireRocketCooldownTimer = new Timer(_fireRocketCooldown);
            _fireRocketDurationTimer = new Timer(_fireRocketDuration);
        }

        // Start is called before the first frame update
        void Start()
        {
            _fireRocketCooldownTimer.Start();
        }

        void Update() {
            if(_fireRocketDurationTimer.IsFinished) {
                _fireRocketCooldownTimer.Start();
                _fireObj.SetActive(false);
                return;
            }

            if(_fireRocketCooldownTimer.IsFinished) {
                _fireRocketDurationTimer.Start();
                _fireObj.SetActive(true);
                return;
            }
        }

        void FixedUpdate() {
            _fireRocketCooldownTimer.Tick();
            _fireRocketDurationTimer.Tick();
        }
    }
}
