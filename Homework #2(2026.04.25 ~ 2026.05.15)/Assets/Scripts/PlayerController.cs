using UnityEngine;

/// <summary>
/// 플레이어의 이동 시뮬레이션을 구현합니다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private const float DefaultSensitivity = 2f;
    private const float DefaultGroundCheckDistance = 1.2f;

    /// <summary>
    /// 해당 플레이어의 Rigidbody.
    /// </summary>
    [Tooltip("해당 플레이어의 Rigidbody.")]
    [SerializeField]
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;

    /// <summary>
    /// 해당 플레이어의 Animator.
    /// </summary>
    [Tooltip("해당 플레이어의 Animator.")]
    [SerializeField]
    private Animator _animator;
    public Animator Animator => _animator;

    /// <summary>
    /// 해당 플레이어의 회전 민감도.
    /// </summary>
    [Header("Simulation")]
    [Tooltip("해당 플레이어의 회전 민감도.")]
    [SerializeField]
    private float sensitivity;

    /// <summary>
    /// 해당 플레이어의 바닥 체크 거리.
    /// </summary>
    [Tooltip("해당 플레이어의 바닥 체크 거리.")]
    [SerializeField]
    private float groundCheckDistance;

    private float simulatedRotation;

    /// <summary>
    /// 플레이어 기준 로컬 속도.
    /// </summary>
    public Vector3 Velocity { get; set; }

    /// <summary>
    /// 해당 플레이어가 땅 위에 있는가에 대한 여부.
    /// </summary>
    public bool IsGrounded
    {
        get
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            return Physics.Raycast(origin, Vector3.down, groundCheckDistance);
        }
    }

    private void Reset()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        sensitivity = DefaultSensitivity;
        groundCheckDistance = DefaultGroundCheckDistance;

        Velocity = Vector3.zero;
        simulatedRotation = 0f;
    }

    private void Awake()
    {
        _rigidbody = _rigidbody ?? GetComponent<Rigidbody>();
        _animator = _animator ?? GetComponent<Animator>();
        sensitivity = sensitivity > 0f ? sensitivity : DefaultSensitivity;
        groundCheckDistance = groundCheckDistance > 0f ? groundCheckDistance : DefaultGroundCheckDistance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Velocity = Vector3.zero;
        simulatedRotation = 0f;
    }

    private void FixedUpdate()
    {
        transform.Translate(Velocity * Time.fixedDeltaTime, Space.Self);
    }
}
