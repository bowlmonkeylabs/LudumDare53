using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputProcessor : MonoBehaviour
    {
	    [Header("Character Input Values")]
	    public Vector2 move;
	    public Vector2 look;
	    public Vector2 lookUnscaled;
	    public Vector2 lookScaleMouse = new Vector2(0.005f, 0.005f);
	    public Vector2 lookScaleGamepad = new Vector2(12.5f, 12.5f);
        public bool jump;
        public bool sprint;

        public BoolVariable isPaused;
        
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Vector2Reference _mouseInput;
        [SerializeField] private Vector2Reference _moveInput;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;
        
        [SerializeField] private FloatReference _mouseSensitivity;
        
        private bool IsCurrentDeviceMouse
        {
	        get => playerInput.currentControlScheme == "Keyboard&Mouse";
			
        }

        #region Unity lifecycle

        private void OnEnable()
        {
	        isPaused.Subscribe(UpdateInputState);
        }
        
        private void OnDisable()
        {
	        isPaused.Unsubscribe(UpdateInputState);
        }

        #endregion

        #region Input Callbacks

        public void OnMove(InputValue value)
        {
	        Vector2 moveInput = value.Get<Vector2>();
	        MoveInput(moveInput);
        }

        public void OnLook(InputValue value)
        {
	        var inputValue = value.Get<Vector2>();
	        LookInput(inputValue);
        }

        public void OnJump(InputValue value)
        {
	        JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
	        SprintInput(value.isPressed);
        }

        public void OnPause(InputValue value)
        {
	        if (isPaused != null)
	        {
		        isPaused.Value = !isPaused.Value;
		        UpdateInputState();
	        }
        }

        #endregion
        
        public void MoveInput(Vector2 newMoveDirection)
        {
	        move = newMoveDirection;
	        _moveInput.Value = move;
        }
        
        public void LookInput(Vector2 newLookDirection)
        {
	        lookUnscaled = newLookDirection;

	        if (IsCurrentDeviceMouse)
		        look = lookUnscaled * lookScaleMouse * _mouseSensitivity.Value;
	        else
		        look = lookUnscaled * lookScaleGamepad * _mouseSensitivity.Value;

	        _mouseInput.Value = look;
        }
        
        public void JumpInput(bool newJumpState)
        {
	        jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
	        sprint = newSprintState;
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
	        SetCursorState(cursorLocked);
        }

        private void UpdateInputState()
        {
	        SetCursorState(!isPaused.Value);
	        playerInput.SwitchCurrentActionMap(isPaused.Value ? "UI" : "Player");
        }

        private void SetCursorState(bool newState)
        {
	        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

    }
}