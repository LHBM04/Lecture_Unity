using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    #region Movement
    [Header("Movement")]
    [SerializeField]
    private Rigidbody _rigidbody;

    [Space, SerializeField]
    private float _moveSpeed;
    private Vector2 _moveInput;
    private Vector3 _currentHorizontalVelocity;

    public bool isMoved
    {
        get
        {
            return _moveInput.sqrMagnitude > 0.01f;
        }
    }

    [SerializeField]
    private float _runMultiplier;
    private bool _isRunning;

    [Space, SerializeField]
    private float _groundAcceleration;

    [SerializeField]
    private float _groundDeceleration;

    [SerializeField]
    private float _airAcceleration;

    [Space, SerializeField]
    private float _jumpForce;
    private bool _jumpRequested;
    
    [Space, SerializeField]
    private float _dodgeMultiplier;
    private bool _isDodging;

    [SerializeField]
    private float _dodgeTime;

    private Vector3 _dodgeDirection;

    private Coroutine _dodgeCoroutine;

    [Space, SerializeField]
    private float _lookSensitivity;
    private float _controlYaw;
    private float _moveReferenceYaw;

    [SerializeField]
    private float _moveRotationSpeed;

    public Quaternion CameraYawRotation
    {
        get
        {
            return Quaternion.Euler(0.0f, _controlYaw, 0.0f);
        }
    }

    [Space, SerializeField]
    private float _groundCheckDistance;
    public bool isGrounded
    {
        get
        {
            return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance);
        }
    }

    [Space, SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private Transform _bulletPosition;

    [Space, SerializeField]
    private GameObject _cartridgePrefab;

    [SerializeField]
    private Transform _cartridgePosition;

    private bool _shouldFire;
    private Coroutine _fireCoroutine;
    #endregion
    #region Health
    private bool _isDead;
    #endregion
    #region Animation
    [Header("Animation")]
    [SerializeField]
    private Animator _animator;

    [Space, SerializeField]
    private string _walkBool;
    private int _walkBoolHash;

    [SerializeField]
    private string _runBool;
    private int _runBoolHash;

    [SerializeField]
    private string _groundedBool;
    private int _groundedBoolHash;

    [SerializeField]
    private string _dodgeTrigger;
    private int _dodgeTriggerHash;

    [SerializeField]
    private string _deadTrigger;
    private int _deadTriggerHash;

    [SerializeField]
    private string _fireTrigger;
    private int _fireTriggerHash;
    #endregion

    private void Reset()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _moveSpeed = 5.0f;
        _runMultiplier = 1.5f;

        _groundAcceleration = 35.0f;
        _groundDeceleration = 45.0f;
        _airAcceleration = 12.0f;

        _jumpForce = 5.0f;
        
        _dodgeMultiplier = 3.0f;
        _dodgeTime = 0.35f;
        
        _lookSensitivity = 0.12f;
        _moveRotationSpeed = 18.0f;
        
        _groundCheckDistance = 0.3f;
    }

    private void Awake()
    {
        _rigidbody = _rigidbody ? _rigidbody : GetComponent<Rigidbody>();
        _animator = _animator ? _animator : GetComponentInChildren<Animator>();

        if (_animator != null)
        {
            _animator.applyRootMotion = false;

            _walkBoolHash = string.IsNullOrWhiteSpace(_walkBool) ? 0 : Animator.StringToHash(_walkBool);
            _runBoolHash = string.IsNullOrWhiteSpace(_runBool) ? 0 : Animator.StringToHash(_runBool);
            _groundedBoolHash = string.IsNullOrWhiteSpace(_groundedBool) ? 0 : Animator.StringToHash(_groundedBool);
            _dodgeTriggerHash = string.IsNullOrWhiteSpace(_dodgeTrigger) ? 0 : Animator.StringToHash(_dodgeTrigger);
            _deadTriggerHash = string.IsNullOrWhiteSpace(_deadTrigger) ? 0 : Animator.StringToHash(_deadTrigger);
            _fireTriggerHash = string.IsNullOrWhiteSpace(_fireTrigger) ? 0 : Animator.StringToHash(_fireTrigger);
        }

        _shouldFire = true;
        _controlYaw = transform.eulerAngles.y;
        _moveReferenceYaw = _controlYaw;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (_isDead)
        {
            return;
        }

        HandleAnimation();
    }

    private void FixedUpdate()
    {
        if (_isDead)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        if (isMoved)
        {
            _moveReferenceYaw = _controlYaw;
        }

        HandleVelocity();
        HandleRotation();
        _jumpRequested = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isDead)
        {
            return;
        }

        if (collision.collider.CompareTag("Enemy"))
        {
            _isDead = true;
            if (_animator != null && _deadTriggerHash != 0)
            {
                _animator.SetTrigger(_deadTriggerHash);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Gizmos.DrawLine();
    }
#endif

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_isDead)
        {
            _moveInput = Vector2.zero;
            return;
        }
        
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (_isDead)
        {
            return;
        }

        Vector2 lookInput = context.ReadValue<Vector2>();
        float yawDelta = lookInput.x * _lookSensitivity;
        _controlYaw += yawDelta;
        _moveReferenceYaw += yawDelta;

        if (!isMoved)
        {
            _moveReferenceYaw = _controlYaw;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (_isDead || _isDodging || !isGrounded)
        {
            _isRunning = false;
            return;
        }

        if (context.performed)
        {
            _isRunning = true;
        }
        else if (context.canceled)
        {
            _isRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_isDead || _isDodging || !isGrounded)
        {
            return;
        }

        if (context.performed)
        {
            _jumpRequested = true;
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!context.performed || _isDead || !isGrounded)
        {
            return;
        }

        Vector3 moveDirection = GetMoveDirection();
        _dodgeDirection = moveDirection.sqrMagnitude > 0.0f
            ? moveDirection
            : transform.forward;

        _isDodging = true;

        if (_animator != null && _dodgeTriggerHash != 0)
        {
            _animator.SetTrigger(_dodgeTriggerHash);
        }

        if (_dodgeCoroutine != null)
        {
            StopCoroutine(_dodgeCoroutine);
            _dodgeCoroutine = null;
        }

        _dodgeCoroutine = StartCoroutine(DodgeRoutine());
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!context.performed || _isDead || !isGrounded || !_shouldFire || _isDodging || isMoved)
        {
            return;
        }

        _shouldFire = false;

        if (_bulletPrefab == null || _bulletPosition == null)
        {
            _shouldFire = true;
            return;
        }

        if (_animator != null && _fireTriggerHash != 0)
        {
            _animator.SetTrigger(_fireTriggerHash);
        }

        if (_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
            _fireCoroutine = null;
        }

        _fireCoroutine = StartCoroutine(FireRoutine());
    }

    private void HandleVelocity()
    {
        Vector3 currentVelocity = _rigidbody.linearVelocity;

        if (_isDodging)
        {
            Vector3 dodgeVelocity = _dodgeDirection * (_moveSpeed * _dodgeMultiplier);
            _currentHorizontalVelocity = dodgeVelocity;
            _rigidbody.linearVelocity = new Vector3(dodgeVelocity.x, currentVelocity.y, dodgeVelocity.z);
        }
        else
        {
            Vector3 targetHorizontalVelocity = GetTargetHorizontalVelocity();
            bool grounded = isGrounded;
            float acceleration = grounded
                ? (isMoved ? _groundAcceleration : _groundDeceleration)
                : _airAcceleration;

            _currentHorizontalVelocity = Vector3.MoveTowards(
                _currentHorizontalVelocity,
                targetHorizontalVelocity,
                acceleration * Time.fixedDeltaTime);

            Vector3 velocity = new Vector3(_currentHorizontalVelocity.x, currentVelocity.y, _currentHorizontalVelocity.z);

            if (_jumpRequested)
            {
                velocity.y = _jumpForce;
            }

            _rigidbody.linearVelocity = velocity;
        }
    }

    private void HandleRotation()
    {
        if (_isDodging)
        {
            return;
        }

        Vector3 moveDirection = isMoved ? GetMoveDirection() : Vector3.zero;
        float targetYaw = _controlYaw;

        if (moveDirection.sqrMagnitude > 0.0f)
        {
            targetYaw = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
        }

        float rotationSpeed = _moveRotationSpeed > 0.0f ? _moveRotationSpeed : 18.0f;
        Quaternion targetRotation = Quaternion.Euler(0.0f, targetYaw, 0.0f);
        Quaternion rotation = Quaternion.Slerp(
            _rigidbody.rotation,
            targetRotation,
            1.0f - Mathf.Exp(-rotationSpeed * Time.fixedDeltaTime));

        _rigidbody.MoveRotation(rotation);
    }

    private void HandleAnimation()
    {
        if (_animator == null)
        {
            return;
        }

        if (_walkBoolHash != 0)
        {
            _animator.SetBool(_walkBoolHash, !_isDodging && isMoved);
        }

        if (_runBoolHash != 0)
        {
            _animator.SetBool(_runBoolHash, !_isDodging && _isRunning && isMoved);
        }

        if (_groundedBoolHash != 0)
        {
            _animator.SetBool(_groundedBoolHash, isGrounded);
        }
    }

    private IEnumerator DodgeRoutine()
    {
        float dodgeTime = _dodgeTime > 0.0f ? _dodgeTime : 0.35f;
        yield return new WaitForSeconds(dodgeTime);

        if (_animator != null && _dodgeTriggerHash != 0)
        {
            _animator.ResetTrigger(_dodgeTriggerHash);
        }

        _isDodging = false;
        _dodgeDirection = Vector3.zero;
        _currentHorizontalVelocity = GetTargetHorizontalVelocity();

        Vector3 currentVelocity = _rigidbody.linearVelocity;
        _rigidbody.linearVelocity = new Vector3(
            _currentHorizontalVelocity.x,
            currentVelocity.y,
            _currentHorizontalVelocity.z);

        _dodgeCoroutine = null;
    }

    private Vector3 GetTargetHorizontalVelocity()
    {
        if (!isMoved)
        {
            return Vector3.zero;
        }

        float speed = _isRunning
            ? _moveSpeed * _runMultiplier
            : _moveSpeed;

        return GetMoveDirection() * speed;
    }

    private Vector3 GetMoveDirection()
    {
        Vector3 localMove = new Vector3(_moveInput.x, 0.0f, _moveInput.y);
        localMove = Vector3.ClampMagnitude(localMove, 1.0f);

        if (localMove.sqrMagnitude <= 0.0f)
        {
            return Vector3.zero;
        }

        Quaternion referenceRotation = Quaternion.Euler(0.0f, _moveReferenceYaw, 0.0f);
        return (referenceRotation * localMove).normalized;
    }

    private IEnumerator FireRoutine()
    {
        yield return null;

        Instantiate(_bulletPrefab, _bulletPosition.position, Quaternion.LookRotation(transform.forward, Vector3.up));

        if (_cartridgePrefab != null && _cartridgePosition != null)
        {
            Instantiate(_cartridgePrefab, _cartridgePosition.position, _cartridgePosition.rotation);
        }

        if (_animator != null && !string.IsNullOrEmpty(_fireTrigger))
        {
            yield return new WaitWhile(() =>
            {
                return _animator.GetCurrentAnimatorStateInfo(0).IsName(_fireTrigger) &&
                       _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
            });

            if (_fireTriggerHash != 0)
            {
                _animator.ResetTrigger(_fireTriggerHash);
            }
        }

        _shouldFire = true;
        _fireCoroutine = null;
    }
}
