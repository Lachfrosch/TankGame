using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using StarterAssets;
using Unity.Services.Authentication;
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

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 5.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 3.0f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

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

    // rigidbody
    Rigidbody rb;

    //Lobby Controller
    private LobbyController _lobbyController;

    // jumping
    private float _lastJump = 0.0f;

    // dampening
    private float _rayDistance = 1.0f;
    private float _fallDampening = 25.0f;
    private float _lastHit = 0.0f;

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
        _lobbyController = GameObject.FindGameObjectWithTag("LobbyController").GetComponent<LobbyController>();
        _lastJump = JumpTimeout;
    }

    public override void OnNetworkSpawn()
    {
        _menuHandler = FindAnyObjectByType<MenuHandler>();
        _menuHandler.SetMenu(MenuHandler.MenuIndex.HUD);
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            Debug.Log(AuthenticationService.Instance.PlayerId);
            GetComponent<PlayerHealth>().SetOwner(AuthenticationService.Instance.PlayerId);
        }
        if (IsClient && IsOwner)
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.enabled = true;
            _cinemachineVirtualCamera.Follow = transform.GetChild(0);
            Transform spawnpoint = SpawnManager.GetSpawnpoint();
            transform.position = spawnpoint.position;
            transform.rotation = spawnpoint.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            //Move();
            //Jump();
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            Move();
            Jump();
            Damp();
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
        
        _speed = targetSpeed;

        // ratation of tank
        if (_input.move != Vector2.zero)
        {
            if (_input.move.x < 0)
            {
                transform.Rotate(0f, -1.0f, 0);
            }
            if (_input.move.x > 0)
            {
                transform.Rotate(0f, +1.0f, 0);
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
        rb.MovePosition(transform.position + (targetDirection.normalized * (_speed * Time.deltaTime)));

        // update wheel rotations based on tank movement
        if (_input.move.sqrMagnitude > 0.1)
        {

            float leftWheelRotation = (_input.move.x + _input.move.y) * _speed * Time.deltaTime * 360.0f / Mathf.PI;
            float rightWheelRotation = ((-_input.move.x) + _input.move.y) * _speed * Time.deltaTime * 360.0f / Mathf.PI;
            for (int i = 0; i < leftWheels.Length; i++)
            {
                Quaternion leftRotation = Quaternion.Euler(leftWheelRotation, 0.0f, 0.0f);
                Quaternion rightRotation = Quaternion.Euler(rightWheelRotation, 0.0f, 0.0f);
                leftWheels[i].rotation *= leftRotation;
                rightWheels[i].rotation *= rightRotation;
            }
        }
    }

    private void Jump()
    {
        if (_input.jump && Grounded && JumpTimeout - _lastJump <= 0)
        {
            _lastJump = 0.0f;
            rb.AddForce(transform.up * JumpHeight, ForceMode.VelocityChange);
        }
        _lastJump += Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        Grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        Grounded = false;
    }

    private void Damp()
    {
        Ray ray = new Ray(rb.position, Vector3.down);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, _rayDistance);
        if (hit)
        {
            float distanceToGround = hitInfo.distance;
            if (_lastHit - distanceToGround >= 0)
            {
                float dampeningForce = (distanceToGround / _rayDistance) * _fallDampening;
                rb.AddForce(-rb.velocity * dampeningForce, ForceMode.Force);
            }
            _lastHit = distanceToGround;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner)
        {
            return;
        }

        var bullet = collision.collider.gameObject.GetComponent<BulletCollision>();
        if (bullet != null)
        {
            var temp = GetComponent<PlayerHealth>();
            CallClientRpcServerRpc(temp.TakeDamage(25), bullet.GetOwner());
        }
    }

    [ServerRpc]
    public void CallClientRpcServerRpc(bool kill, string bulletOwner)
    {
        HandleDmgClientRpc(kill, bulletOwner);
    }

    [ClientRpc]
    private void HandleDmgClientRpc(bool kill, string bulletOwner)
    {
        if (bulletOwner != AuthenticationService.Instance.PlayerId)
        {
            return;
        }
        if (kill)
        {
            //Give bulletOwner +1  kill
            _lobbyController.AddPlayerKill();
        }
        else
        {
            //Give bulletOwner +1 hit
            _lobbyController.AddPlayerHit();
        }
    }
}
