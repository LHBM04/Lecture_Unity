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
        // 애니메이터 업데이트 (단방향 구조 유지)
        if (_animator != null)
        {
            bool isMoving = _inputMovement.sqrMagnitude > 0.01f;
            _animator.SetBool(_movingBoolHash, isMoving);
            _animator.SetBool(_groundedBoolHash, _controller.isGrounded);

            float targetSpeedParam = isMoving ? (_isRunningPressed ? 1.0f : 0.5f) : 0f;
            _currentSpeedParam = Mathf.MoveTowards(_currentSpeedParam, targetSpeedParam, 5f * Time.deltaTime);
            _animator.SetFloat(_moveSpeedFloatHash, _currentSpeedParam);
        }
    }

    private void FixedUpdate()
    {
        float currentMoveSpeed = _moveSpeed * (_isRunningPressed ? _sprintSpeedMultiplier : 1f);
        Vector3 moveVector = Vector3.zero;

        var _cameraTransform = Camera.main.transform;
        // 1. 카메라 시점 기준 이동 벡터 생성
        if (_cameraTransform != null)
        {
            Vector3 camForward = _cameraTransform.forward;
            Vector3 camRight = _cameraTransform.right;

            // 수평 이동만을 위해 Y축 성분 제거
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // 카메라의 정면과 우측에 키보드 입력을 곱해 실제 이동 방향 도출
            moveVector = (camForward * _inputMovement.y) + (camRight * _inputMovement.x);
            if (moveVector.sqrMagnitude > 1f) moveVector.Normalize();
        }

        // 2. 물리 이동 연산 및 플레이어 회전
        if (_controller.isGrounded)
        {
            _direction.x = moveVector.x * currentMoveSpeed;
            _direction.z = moveVector.z * currentMoveSpeed;

            if (_direction.y < 0) _direction.y = -1f;

            // 3. 이동 입력이 있을 때만 가야 할 방향(moveVector)으로 플레이어를 회전
            if (_inputMovement.sqrMagnitude > 0.01f && moveVector.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveVector);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    2.0f * Time.fixedDeltaTime
                );
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

    public void OnMove(InputValue inputValue)
    {
        _inputMovement = inputValue.Get<Vector2>();
    }

    private Vector2 _inputLook;

    [SerializeField]
    private float _mouseSensitivity = 2.0f;

    public void OnLook(InputValue inputValue)
    {
        // _inputLook = inputValue.Get<Vector2>();
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