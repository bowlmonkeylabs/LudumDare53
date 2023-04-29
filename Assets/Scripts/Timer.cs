using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private bool _isRunning;
    private bool _isFinished;
    private float _duration;
    private float _remainingTime;

    public bool IsFinished {
        get {
            return _isFinished;
        }
    }

    public bool IsRunning {
        get {
            return _isRunning;
        }
    }

    public Timer(float duration) {
        _duration = duration;
        Reset();
    }

    public void Start() {
        Reset();
        _isRunning = true;
    }

    public void Tick() {
        if(_isFinished || !_isRunning) {
            return;
        }

        _remainingTime -= Time.deltaTime;

        if(_remainingTime <= 0) {
            _isFinished = true;
        }
    }

    public void Reset() {
        _isRunning = false;
        _isFinished = false;
        _remainingTime = _duration;
    }
}
