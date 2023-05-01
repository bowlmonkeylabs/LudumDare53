using BML.ScriptableObjectCore.Scripts.Variables;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		public float SprintMaxAmount = 10f;
		public float SprintDecayRate = 3f;
		public float SprintRegenRate = 6.0f;
		public float SprintRegenDelay= 1f;
		public MMF_Player SprintStartFeedback;
		public MMF_Player SprintStopFeedback;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;
		[Tooltip("Factor for look acceleration setting")]
		[SerializeField] float lookAccelerationFactor = .0001f;
		[Tooltip("Rate of look acceleration")]
		[SerializeField] FloatReference LookAcceleration;
		[Tooltip("Curve for analog look input smoothing")]
		[SerializeField] AnimationCurve AnalogMovementCurve;

		[SerializeField, FoldoutGroup("Caffeine")] private float _caffeineMoveSpeedMultiplier;
		[SerializeField, FoldoutGroup("Caffeine")] private BoolReference _isCaffeinated;
		
		[SerializeField] private BoolReference _isPlayerInputDisabled;

		[SerializeField] private Vector3Variable _outputCurrentVelocity;
		[SerializeField] private BoolVariable _outputIsGrounded;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;
		
		//Sprint
		private float currentSprintAmount;
		private float lastSprintTime = Mathf.NegativeInfinity;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
		private PlayerInput _playerInput;
		private CharacterController _controller;
		private PlayerInputProcessor _input;
		private GameObject _mainCamera;
		private float previouRotSpeed = 0f;
		private bool sprinting = false;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get => _playerInput.currentControlScheme == "Keyboard&Mouse";
			
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
			
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<PlayerInputProcessor>();
			_playerInput = GetComponent<PlayerInput>();
			currentSprintAmount = SprintMaxAmount;
		}

		private void Start()
		{
			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void Update()
		{
			if (_isPlayerInputDisabled.Value)
				return;
			
			JumpAndGravity();
			GroundedCheck();
			Move();

			_outputCurrentVelocity.Value = _controller.velocity;
			_outputIsGrounded.Value = _controller.isGrounded;
		}

		private void LateUpdate()
		{
			if (_isPlayerInputDisabled.Value)
				return;
			
			CameraRotation();
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			//Don't multiply mouse input by Time.deltaTime
			float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
			
			float rotSpeed = RotationSpeed;
			float lookAcceleration = LookAcceleration.Value * lookAccelerationFactor;
			
			if (!IsCurrentDeviceMouse)
			{
				//For analog movement, dont interpolate linearly
				rotSpeed *= AnalogMovementCurve.Evaluate(_input.lookUnscaled.magnitude);
				
				float dummy = 0f;
				float rotSpeedAccelerated;

				//Accelerate to higher values but stop immediately
				if (rotSpeed > previouRotSpeed)
					rotSpeedAccelerated = Mathf.SmoothDamp(previouRotSpeed, rotSpeed, ref dummy, lookAcceleration);
				else
					rotSpeedAccelerated = rotSpeed;
				
				//Debug.Log($"prev: {previouRotSpeed} | target: {String.Format("{0:0.00}", rotSpeed)}");

				rotSpeed = rotSpeedAccelerated;
				previouRotSpeed = rotSpeedAccelerated;
			}
			else
			{
				float dummy = 0f;
				float rotSpeedAccelerated;

				//Accelerate to higher values but stop immediately
				if (rotSpeed > previouRotSpeed)
					rotSpeedAccelerated = Mathf.SmoothDamp(previouRotSpeed, rotSpeed, ref dummy, lookAcceleration);
				else
					rotSpeedAccelerated = rotSpeed;

				rotSpeed = rotSpeedAccelerated;
				previouRotSpeed = rotSpeedAccelerated;
			}

			if (Mathf.Approximately(0f, _input.look.magnitude))
				previouRotSpeed = 0f;
				
			
			_rotationVelocity = _input.look.x * rotSpeed * deltaTimeMultiplier;
			_cinemachineTargetPitch += _input.look.y * rotSpeed * deltaTimeMultiplier;
			

			// clamp our pitch rotation
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

			// Update Cinemachine camera target pitch
			CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
			
			// rotate the player left and right
			transform.Rotate(Vector3.up * _rotationVelocity);
		}

		private void Move()
		{
			if (_input.sprint && currentSprintAmount >= SprintDecayRate)
			{
				lastSprintTime = Time.time;
				currentSprintAmount -= SprintDecayRate * Time.deltaTime;
				if (!sprinting)
					SprintStartFeedback.PlayFeedbacks();
				sprinting = true;
			}
			else
			{
				if (sprinting)
				{
					SprintStopFeedback.PlayFeedbacks();
				}
					
				sprinting = false;
			}

			if (lastSprintTime + SprintRegenDelay < Time.time)
			{
				currentSprintAmount += SprintRegenDelay * Time.deltaTime;
			}

			currentSprintAmount = Mathf.Min(currentSprintAmount, SprintMaxAmount);

			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = sprinting ? SprintSpeed : MoveSpeed;
			targetSpeed *= _isCaffeinated.Value ? _caffeineMoveSpeedMultiplier : 1f;

				// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
    }
}