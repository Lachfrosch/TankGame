using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using StarterAssets;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif

public class TankController : NetworkBehaviour
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;


    // input
#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private StarterAssetsInputs _input;

    // camera
    private GameObject _mainCamera;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private float _currentCameraDistance;
    public float zoomMaxDistance = 80f; //30
    public float zoomMinDistance = 50f; //10
    public float zoomSpeed = 0.02f;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // menue
    private MenuHandler _menuHandler;

    //Wheels
    public Transform[] leftWheels; // array of left wheels
    public Transform[] rightWheels; // array of right wheels

    //Turret
    public Transform turret;

    // player
    private float _speed;
    private float _verticalVelocity;

    // rigidbody
    Rigidbody rb;

    private void Awake()
    {

        rb = GetComponent<Rigidbody>();

        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        if (_cinemachineVirtualCamera == null)
        {
            _cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            _currentCameraDistance = _cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        _input = GetComponent<StarterAssetsInputs>();
    }

    public override void OnNetworkSpawn()
    {
        _menuHandler = FindAnyObjectByType<MenuHandler>();
        _menuHandler.SetMenu(MenuHandler.MenuIndex.HUD);
        base.OnNetworkSpawn();
        if (IsClient && IsOwner)
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.enabled = true;
            _cinemachineVirtualCamera.Follow = transform.GetChild(0);
            transform.position += GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    private void LateUpdate()
    {
        if (IsOwner)
        {
            CameraRotation();
            CameraZoom();
        }
    }

    private void CameraZoom()
    {
        var newDistance = _currentCameraDistance;
        if (_currentCameraDistance > zoomMinDistance && _input.zoom > 0)
        {
            newDistance--;
        }
        else if (_currentCameraDistance < zoomMaxDistance && _input.zoom < 0)
        {
            newDistance++;
        }

        if (_input.aiming)
        {
            _menuHandler.StartAim();
            _cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = 10;
        }
        else
        {
            _menuHandler.StopAim();
            _currentCameraDistance = newDistance;
            _cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = newDistance;
        }

    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        //if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        if (!LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            //float deltaTimeMultiplier = 1.0f;
            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;

            Vector3 eulerRotation = rb.transform.eulerAngles;
            // update turret rotations based on camera movement

            var rotationAboutAxis = Quaternion.Euler(transform.rotation.x, _cinemachineTargetYaw - transform.rotation.eulerAngles.y, transform.rotation.z);

            turret.localRotation = rotationAboutAxis;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
            //return true;
#else
			return false;
#endif
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // ratation of tank
        if (_input.move != Vector2.zero)
        {
            if (_input.move.x < 0)
            {
                transform.Rotate(0f, -0.5f, 0);
            }
            if (_input.move.x > 0)
            {
                transform.Rotate(0f, +0.5f, 0);
            }
        }

        Vector3 targetDirection = Vector3.zero;
        if (_input.move.y > 0)
        {
            targetDirection = transform.forward;
        }
        else if (_input.move.y < 0)
        {
            targetDirection = transform.forward * -1;
        }

        // move the player
        rb.MovePosition(rb.position + (targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime));

        // update wheel rotations based on tank movement
        if (_input.move.sqrMagnitude > 0.1)
        {

            float leftWheelRotation = (_input.move.x + _input.move.y) * _speed * Time.deltaTime * 360.0f / Mathf.PI;
            float rightWheelRotation = ((- _input.move.x) + _input.move.y) * _speed * Time.deltaTime * 360.0f / Mathf.PI;
            for (int i = 0; i < leftWheels.Length; i++)
            {
                Quaternion leftRotation = Quaternion.Euler(leftWheelRotation, 0.0f, 0.0f);
                Quaternion rightRotation = Quaternion.Euler(rightWheelRotation, 0.0f, 0.0f);
                leftWheels[i].rotation *= leftRotation;
                rightWheels[i].rotation *= rightRotation;
            }
        }

        if(_input.respawn)
        {
            PlayerRespawn();
        }
    }

    private void PlayerRespawn()
    {
        //transform.position = new Vector3(295f, 15f, 280f);
        transform.position = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
    }
}
