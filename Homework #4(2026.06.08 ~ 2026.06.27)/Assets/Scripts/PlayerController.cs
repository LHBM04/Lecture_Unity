using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] 
    private CharacterController _controller;

    [Space, SerializeField] 
    private float _gravity = 9.81f;

    [SerializeField] 
    private Vector3 _gravityDirection = Vector3.down;

    [Space, SerializeField] 
    private float _moveSpeed = 5f;
    [SerializeField] 
    private float _airDrag = 0.1f;
    [SerializeField] 
    private float _sprintSpeedMultiplier = 2f;
    [SerializeField] 
    private float _jumpForce = 5f;

    private Vector3 _direction;
    private Vector2 _inputMovement;

    [Space, SerializeField] 
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
    private string _moveDirectionFloat;
    private int _moveDirectionFloatHash;

    private float _currentSpeedParam = 0f;
    [SerializeField] 
    private bool _isRunningPressed;

    private void Reset()
    {
        _controller = GetComponent<CharacterController>();
        _gravity = 9.81f;
        _gravityDirection = Vector3.down;
        _airDrag = 0.1f;
        _sprintSpeedMultiplier = 2f;
        _jumpForce = 5f;
        _animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        _controller = _controller ?? GetComponent<CharacterController>();
        _animator = _animator ?? GetComponentInChildren<Animator>();
        
        _movingBoolHash = !string.IsNullOrEmpty(_movingBool) ? Animator.StringToHash(_movingBool) : 0;
        _groundedBoolHash = !string.IsNullOrEmpty(_groundedBool) ? Animator.StringToHash(_groundedBool) : 0;
        _moveSpeedFloatHash = !string.IsNullOrEmpty(_moveSpeedFloat) ? Animator.StringToHash(_moveSpeedFloat) : 0;
        _moveDirectionFloatHash = !string.IsNullOrEmpty(_moveDirectionFloat) ? Animator.StringToHash(_moveDirectionFloat) : 0;
    }

    private void Update()
    {
        _animator.SetBool(_groundedBoolHash, _controller.isGrounded);
        _animator.SetFloat("Vertical Velocity", _controller.velocity.y);

        _animator.SetBool(_movingBoolHash, _inputMovement.sqrMagnitude > 0.01f);

        float targetSpeedParam = _inputMovement.sqrMagnitude > 0.01f ? (_isRunningPressed ? 1.0f : 0.5f) : 0f;
        _currentSpeedParam = Mathf.MoveTowards(_currentSpeedParam, targetSpeedParam, 5f * Time.deltaTime);
        _animator.SetFloat(_moveSpeedFloatHash, _currentSpeedParam);

        if (_inputMovement.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(_inputMovement.x, _inputMovement.y) * Mathf.Rad2Deg;
            _animator.SetFloat(_moveDirectionFloatHash, targetAngle);
        }
    }

    private void FixedUpdate()
    {
        float currentMoveSpeed = _moveSpeed * (_isRunningPressed ? _sprintSpeedMultiplier : 1f);
        
        if (_controller.isGrounded)
        {
            _direction.x = _inputMovement.x * currentMoveSpeed;
            _direction.z = _inputMovement.y * currentMoveSpeed;

            if (_direction.y < 0) 
                _direction.y = -1f;
        }
        else
        {
            _direction.x = Mathf.Lerp(_direction.x, _inputMovement.x * currentMoveSpeed, _airDrag);
            _direction.z = Mathf.Lerp(_direction.z, _inputMovement.y * currentMoveSpeed, _airDrag);
        }

        _direction += _gravityDirection.normalized * (_gravity * Time.fixedDeltaTime);

        _controller.Move(_direction * Time.fixedDeltaTime);
    }

    public void OnMove(InputValue inputValue)
    {
        _inputMovement = inputValue.Get<Vector2>();
    }

    public void OnSprint(InputValue inputValue)
    {
        _isRunningPressed = inputValue.isPressed;
    }

    public void OnJump(InputValue inputValue)
    {
        if (_controller.isGrounded)
        {
            _direction.y = _jumpForce;
        }
    }
}