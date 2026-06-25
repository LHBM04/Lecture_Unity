using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PlayerInputActions _inputActions;

    [Header("Physics")]
    [SerializeField] 
    private CharacterController _controller;

    [Space, SerializeField]
    private float _gravity;

    [SerializeField]
    private Vector3 _gravityDirection;

    [Header("Camera")]
    [SerializeField]
    private Transform _cameraTransform;

    [Header("Status")]
    [SerializeField] 
    private float _moveSpeed;
    private float _moveSpeedParameter;

    [SerializeField] 
    private float _airDrag;

    [SerializeField]
    private float _sprintSpeedMultiplier;
    private bool _isRunningPressed;

    [SerializeField]
    private float _jumpForce;

    private Vector3 _direction;
    private Vector2 _inputMovement;

    [SerializeField]
    private float _rotationSmoothTime;
    private float _rotationVelocity;

    [Header("Weapons")]
    [SerializeField]
    private GameObject _staff;

    [SerializeField]
    private Transform _staffGripTransform;

    [SerializeField]
    private GameObject _pistol;

    [SerializeField]
    private Transform _pistolGripTransform;

    public enum WeaponType : byte
    {
        Staff,
        Pistol
    }

    private WeaponType _currentWeapon;

    [Header("Animations")]
    [SerializeField] 
    private Animator _animator;

    [SerializeField] 
    private string _movingBool;
    private int _movingBoolHash;

    [SerializeField] 
    private string _groundedBool;
    private int _groundedBoolHash;

    [SerializeField] 
    private string _moveSpeedFloat;
    private int _moveSpeedFloatHash;

    [SerializeField]
    private string _verticalVelocityFloat;
    private int _verticalVelocityFloatHash;

    [SerializeField]
    private string _rollTrigger;
    private int _rollTriggerHash;

    [SerializeField]
    private string _hurtTrigger;
    private int _hurtTriggerHash;

    [SerializeField]
    private string _deadTrigger;
    private int _deadTriggerHash;

    [SerializeField]
    private string _weaponInt;
    private int _weaponIntHash;

    [SerializeField]
    private string _attackTrigger;
    private int _attackTriggerHash;

    [HideInInspector]
    public bool isSpawned;

    private void Reset()
    {
        _controller = GetComponent<CharacterController>();

        _gravity = 9.81f;
        _gravityDirection = Vector3.down;
        
        _airDrag = 0.1f;
        _sprintSpeedMultiplier = 2f;
        _jumpForce = 5f;
        _currentWeapon = WeaponType.Staff;
        
        _animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        
        _controller = _controller ?? GetComponent<CharacterController>();
        
        _moveSpeedParameter = 0.0f;
        _currentWeapon = WeaponType.Staff;
        
        _animator = _animator ?? GetComponentInChildren<Animator>();
        _movingBoolHash = !string.IsNullOrEmpty(_movingBool) ? Animator.StringToHash(_movingBool) : 0;
        _groundedBoolHash = !string.IsNullOrEmpty(_groundedBool) ? Animator.StringToHash(_groundedBool) : 0;
        _moveSpeedFloatHash = !string.IsNullOrEmpty(_moveSpeedFloat) ? Animator.StringToHash(_moveSpeedFloat) : 0;
        _verticalVelocityFloatHash = !string.IsNullOrEmpty(_verticalVelocityFloat) ? Animator.StringToHash(_verticalVelocityFloat) : 0;
        _rollTriggerHash = !string.IsNullOrEmpty(_rollTrigger) ? Animator.StringToHash(_rollTrigger) : 0;
        _hurtTriggerHash = !string.IsNullOrEmpty(_hurtTrigger) ? Animator.StringToHash(_hurtTrigger) : 0;
        _deadTriggerHash = !string.IsNullOrEmpty(_deadTrigger) ? Animator.StringToHash(_deadTrigger) : 0;
        _weaponIntHash = !string.IsNullOrEmpty(_weaponInt) ? Animator.StringToHash(_weaponInt) : 0;
        _attackTriggerHash = !string.IsNullOrEmpty(_attackTrigger) ? Animator.StringToHash(_attackTrigger) : 0;

        AttachWeapons();
        SetWeapon(WeaponType.Staff);
    }

    private void OnEnable()
    {
        _inputActions.Player.Move.started += OnMoveStarted;
        _inputActions.Player.Move.performed += OnMovePerformed;
        _inputActions.Player.Move.canceled += OnMoveCanceled;
        
        _inputActions.Player.Sprint.started += OnSprintStarted;
        _inputActions.Player.Sprint.performed += OnSprintPerformed;
        _inputActions.Player.Sprint.canceled += OnSprintCanceled;
        
        _inputActions.Player.Jump.performed += OnJumpPerformed;
        _inputActions.Player.Roll.performed += OnRollPerformed;
        _inputActions.Player.Attack.performed += OnAttackPerformed;
        _inputActions.Player.Scroll.performed += OnScrollPerformed;
        
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
        
        _inputActions.Player.Move.started -= OnMoveStarted;
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCanceled;
        
        _inputActions.Player.Sprint.started -= OnSprintStarted;
        _inputActions.Player.Sprint.performed -= OnSprintPerformed;
        _inputActions.Player.Sprint.canceled -= OnSprintCanceled;
        
        _inputActions.Player.Jump.performed -= OnJumpPerformed;
        _inputActions.Player.Roll.performed -= OnRollPerformed;
        _inputActions.Player.Attack.performed -= OnAttackPerformed;
        _inputActions.Player.Scroll.performed -= OnScrollPerformed;
    }

    private void OnDestroy()
    {
        _inputActions.Dispose();
    }

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!isSpawned)
        {
            return;
        }

        if (_animator != null)
        {
            _animator.SetBool(_groundedBoolHash, _controller.isGrounded);

            const float threshold = 0.01f;
            if (_inputMovement.sqrMagnitude > threshold)
            {
                _animator.SetBool(_movingBoolHash, true);

                float targetSpeedParam = _isRunningPressed ? 1.0f : 0.5f;
                _moveSpeedParameter = Mathf.MoveTowards(_moveSpeedParameter, targetSpeedParam, 5f * Time.deltaTime);
                _animator.SetFloat(_moveSpeedFloatHash, _moveSpeedParameter);
            }
            else
            {
                _animator.SetBool(_movingBoolHash, false);
                _moveSpeedParameter = Mathf.MoveTowards(_moveSpeedParameter, 0f, 5f * Time.deltaTime);
                _animator.SetFloat(_moveSpeedFloatHash, _moveSpeedParameter);
            }

            _animator.SetFloat(_verticalVelocityFloatHash, _controller.velocity.y);
        }
    }

    private void FixedUpdate()
    {
        if (!isSpawned)
        {
            return;
        }

        float currentMoveSpeed = _moveSpeed * (_isRunningPressed ? _sprintSpeedMultiplier : 1f);
        Vector3 moveVector = Vector3.zero;

        if (_inputMovement.sqrMagnitude >= 0.01f)
        {
            float inputAngle = Mathf.Atan2(_inputMovement.x, _inputMovement.y) * Mathf.Rad2Deg;
            float targetAngle = inputAngle + _cameraTransform.eulerAngles.y;

            float smoothTargetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, _rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, smoothTargetAngle, 0.0f);

            moveVector = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
        }

        if (_controller.isGrounded)
        {
            _direction.x = moveVector.x * currentMoveSpeed;
            _direction.z = moveVector.z * currentMoveSpeed;

            if (_direction.y < 0)
            {
                _direction.y = -2f;
            }
        }
        else
        {
            _direction.x = Mathf.Lerp(_direction.x, moveVector.x * currentMoveSpeed, _airDrag);
            _direction.z = Mathf.Lerp(_direction.z, moveVector.z * currentMoveSpeed, _airDrag);
        }

        _direction += _gravityDirection.normalized * (_gravity * Time.fixedDeltaTime);
        _controller.Move(_direction * Time.fixedDeltaTime);
    }

    public void Ready()
    {
        isSpawned = true;
    }

    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        _inputMovement = context.ReadValue<Vector2>();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _inputMovement = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _inputMovement = Vector2.zero;
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        _isRunningPressed = true;
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        _isRunningPressed = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        _isRunningPressed = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        _direction.y = _jumpForce;
    }

    private void OnRollPerformed(InputAction.CallbackContext context)
    {
        if (_animator != null && _rollTriggerHash != 0)
        {
            _animator.SetTrigger(_rollTriggerHash);
        }
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (_animator != null)
        {
            if (_movingBoolHash != 0)
            {
                _animator.SetBool(_movingBoolHash, true);
            }
            if (_attackTriggerHash != 0)
            {
                _animator.SetTrigger(_attackTriggerHash);
            }
        }
    }

    private void OnScrollPerformed(InputAction.CallbackContext context)
    {
        float scrollDelta = context.ReadValue<Vector2>().y;

        if (Mathf.Approximately(scrollDelta, 0.0f))
        {
            return;
        }

        WeaponType nextWeapon = _currentWeapon == WeaponType.Staff ? WeaponType.Pistol : WeaponType.Staff;
        SetWeapon(nextWeapon);
    }

    private void SetWeapon(WeaponType weaponType)
    {
        _currentWeapon = weaponType;

        if (_animator != null && _weaponIntHash != 0)
        {
            _animator.SetInteger(_weaponIntHash, (int)_currentWeapon);
        }

        if (_staff != null)
        {
            _staff.SetActive(_currentWeapon == WeaponType.Staff);
        }

        if (_pistol != null)
        {
            _pistol.SetActive(_currentWeapon == WeaponType.Pistol);
        }
    }

    private void AttachWeapons()
    {
        AttachWeapon(_staff, _staffGripTransform);
        AttachWeapon(_pistol, _pistolGripTransform);
    }

    private void AttachWeapon(GameObject weapon, Transform weaponTransform)
    {
        if (weapon == null || weaponTransform == null)
        {
            return;
        }

        weapon.transform.SetParent(weaponTransform, false);
    }
}
