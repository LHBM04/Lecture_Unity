using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] 
    private Animator _animator;

    [SerializeField]
    private string _groundControlBool;
    private int _groundControlBoolHash;

    [SerializeField]
    private string _groundSpeedFloat;
    private int _groundSpeedFloatHash;

    [SerializeField] 
    private float _acceleration = 4.0f;

    private Vector2 _inputDirection = Vector2.zero;
    private float _currentSpeedParam = 0.0f;
    private bool _isRunningPressed = false;

    private void Reset()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        _animator = _animator ?? GetComponentInChildren<Animator>();
        _groundControlBoolHash = !string.IsNullOrEmpty(_groundControlBool) ? Animator.StringToHash(_groundControlBool) : 0;
        _groundSpeedFloatHash = !string.IsNullOrEmpty(_groundSpeedFloat) ? Animator.StringToHash(_groundSpeedFloat) : 0;
    }

    public void OnMove(InputValue value)
    {
        _inputDirection = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        _isRunningPressed = value.isPressed;
        Debug.Log($"Sprint Key State Changed: {_isRunningPressed}");
    }

    public void Update()
    {
        bool isMoving = _inputDirection.sqrMagnitude > 0.001f;

        _animator.SetBool(_groundControlBoolHash, isMoving);

        float targetSpeed = 0.0f;
        if (isMoving)
        {
            targetSpeed = _isRunningPressed ? 1.0f : 0.5f;
        }

        _currentSpeedParam = Mathf.MoveTowards(
            _currentSpeedParam,
            targetSpeed,
            _acceleration * Time.deltaTime
        );

        _animator.SetFloat(_groundSpeedFloatHash, _currentSpeedParam);
    }
}
