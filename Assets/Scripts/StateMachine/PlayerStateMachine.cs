using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerStateMachine : MonoBehaviour
{
    // declare reference variables
    CharacterController _characterController;
    Animator _animator;
    PlayerInput _playerInput;
    PlayerManager _playerManager;

    // variables to store optimized setter/getter parameter IDs
    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash;
    int _jumpCountHash;
    int _isFallingHash;
    int _attackHash;

    // variables to store player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _currentRunMovement;
    Vector3 _appliedMovement;
    Vector3 _cameraRelativeMovement;
    bool _isMovementPressed;
    bool _isRunPressed;

    // constants
    float _rotationFactorPerFrame = 15.0f;
    float _runMultiplier = 5.0f;
    int _zero = 0;

    // gravity variable
    float initialGravity = -9.8f;

    // jumping variables
    bool _isJumpPressed = false;
    float _initialJumpVelocity;
    float _maxJumpHeight = 2.0f;
    float _maxJumpTime = 0.75f;
    bool _isJumping = false;
    bool _requireNewJumpPress = false;
    bool _doubleJump;
    int _jumpCount = 0;
    Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();
    Dictionary<int, float> _jumpGravities = new Dictionary<int, float>();
    Coroutine _currentJumpResetRoutine = null;

    bool _inRiver = false;

    // attacking variables
    bool _isAttackPressed;
    bool _isAttacking = false;
    bool _requireNewAttackPress = false;
    int _attackCount = 0;
    float _attackTimer = 0;
    float _timeToAttack = 1.1f;
    GameObject _attackArea = default;
    Vector3 _offset;
    Vector3 _spawnPosition;

    // attacking effect
    public GameObject _attackEffectPrefab;
    private GameObject _attackEffectInstance;

    // state variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerStateFactory States { get { return _states; } }
    public Animator Animator { get { return _animator; } }
    public CharacterController CharacterController { get { return _characterController; } }
    public Coroutine CurrentJumpResetRoutine { get { return _currentJumpResetRoutine; } set { _currentJumpResetRoutine = value; } }
    public Dictionary<int, float> InitialJumpVelocities { get { return _initialJumpVelocities; } }
    public Dictionary<int, float> JumpGravities { get { return _jumpGravities; } }
    public GameObject AttackArea { get { return _attackArea; } }
    public int JumpCount { get { return _jumpCount; } set { _jumpCount = value; } }
    public int AttackCount { get { return _attackCount; } set { _attackCount = value; } }
    public int IsWalkingHash { get { return _isWalkingHash; } }
    public int IsRunningHash { get { return _isRunningHash; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public int JumpCountHash { get { return _jumpCountHash; } }
    public int IsFallingHash { get { return _isFallingHash; } }
    public int AttackHash { get { return _attackHash; } }
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
    public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
    public bool IsJumping { set { _isJumping = value; } }
    public bool DoubleJump { get { return _doubleJump; } set { _doubleJump = value; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool IsAttackPressed { get { return _isAttackPressed; } }
    public bool InRiver { get { return _inRiver; } set { _inRiver = value; } }
    public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    public bool RequireNewAttackPress { get { return _requireNewAttackPress; } set { _requireNewAttackPress = value; } }
    public float AttackTimer { get { return _attackTimer; } set { _attackTimer = value; } }
    public float TimeToAttack { get { return _timeToAttack; } set { _timeToAttack = value; } }
    public float Gravity { get { return initialGravity; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
    public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
    public float RunMultiplier { get { return _runMultiplier; } set { _runMultiplier = value; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
    public Vector3 CameraRelativeMovement { get { return _cameraRelativeMovement; } set { _cameraRelativeMovement = value; } }


    void Awake()
    {
        // initially set reference variables
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerManager = GetComponent<PlayerManager>();
        Transform childTransform = transform.Find("Attack Range");

        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        // set the parameter hash references
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _jumpCountHash = Animator.StringToHash("jumpCount");
        _isFallingHash = Animator.StringToHash("isFalling");
        _attackHash = Animator.StringToHash("isAttacking");

        // set the player input callbacks
        _playerInput.CharacterControls.Move.started += OnMovementInput;
        _playerInput.CharacterControls.Move.canceled += OnMovementInput;
        _playerInput.CharacterControls.Move.performed += OnMovementInput;
        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;
        _playerInput.CharacterControls.Attack.started += OnAttack;
        _playerInput.CharacterControls.Attack.canceled += OnAttack;

        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        float initialGravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
        float secondJumpGravity = (-2 * (_maxJumpHeight * 1.5f)) / Mathf.Pow((timeToApex * 1.25f), 2);
        float secondJumpInitialVelocity = (2 * (_maxJumpHeight * 1.5f)) / (timeToApex * 1.25f);
        float thirdJumpGravity = (-2 * (_maxJumpHeight * 2f)) / Mathf.Pow((timeToApex * 1.5f), 2);
        float thirdJumpInitialVelocity = (2 * (_maxJumpHeight * 2f)) / (timeToApex * 1.5f);

        _initialJumpVelocities.Add(1, _initialJumpVelocity);
        _initialJumpVelocities.Add(2, secondJumpInitialVelocity);
        _initialJumpVelocities.Add(3, thirdJumpInitialVelocity);

        _jumpGravities.Add(0, initialGravity);
        _jumpGravities.Add(1, initialGravity);
        _jumpGravities.Add(2, secondJumpGravity);
        _jumpGravities.Add(3, thirdJumpGravity);
    }

    // Start is called before the first frame update
    void Start()
    {
        _attackArea = transform.GetChild(0).gameObject;
        _characterController.Move(_appliedMovement * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        _currentState.UpdateStates();

        if (!_playerManager.IsDead)
        {
            _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);
            _characterController.Move(_cameraRelativeMovement * Time.deltaTime);
        }
    }

    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        // store the Y value of the original vector to rotate
        float currentYValue = vectorToRotate.y;

        // get the forward and right directional vectors of the camera
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // remove the Y values to ignore upward/downward camera angles
        cameraForward.y = 0;
        cameraRight.y = 0;

        // re-normalize both vectors so they each have a magnitude of 1
        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        // rotate the X and Z VectorToRotate values to camera space
        Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

        // the sum of both products is the Vector3 in camera space
        Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
        vectorRotatedToCameraSpace.y = currentYValue;
        return vectorRotatedToCameraSpace;
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        // the change in position our character should point to
        positionToLookAt.x = _cameraRelativeMovement.x;
        positionToLookAt.y = _zero;
        positionToLookAt.z = _cameraRelativeMovement.z;

        // the current rotation of our character
        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed)
        {
            // creates a new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;
        _currentRunMovement.x = _currentMovementInput.x * _runMultiplier;
        _currentRunMovement.z = _currentMovementInput.y * _runMultiplier;
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requireNewJumpPress = false;
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        _isAttackPressed = context.ReadValueAsButton();
        _requireNewAttackPress = false;
    }

    void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }

    public void AttackVFXOn()
    {
        _offset = new Vector3(0, 0.5f, 0); // Adjust yOffset to your desired value
        _spawnPosition = transform.position + _offset;

        // Instantiate the effect if it hasn't been instantiated yet
        if (_attackEffectInstance == null)
        {
            _attackEffectInstance = Instantiate(_attackEffectPrefab, _spawnPosition, Quaternion.Euler(90f, 0f, 0f));
        }
        else
        {
            // Reposition the existing effect
            _attackEffectInstance.transform.position = _spawnPosition;
            _attackEffectInstance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            _attackEffectInstance.SetActive(true);
        }
    }

    public void AttackVFXOff()
    {
        // Deactivate the effect if it exists
        if (_attackEffectInstance != null)
        {
            _attackEffectInstance.SetActive(false);
        }
    }

    /*public void Knockback(Vector3 direction)
    {
        _knockBackCounter = _knockBackTime;

        _appliedMovement = direction * _knockBackForce;
    }*/
}
