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
    private Rigidbody rb;

    /// <summary>
    /// 해당 플레이어의 Animator.
    /// </summary>
    [Tooltip("해당 플레이어의 Animator.")]
    [SerializeField]
    private Animator animator;

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
    /// 플레이어 Rigidbody 접근자.
    /// </summary>
    public Rigidbody Rigidbody => rb;

    /// <summary>
    /// 플레이어 Animator 접근자.
    /// </summary>
    public Animator Animator => animator;

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
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        sensitivity = DefaultSensitivity;
        groundCheckDistance = DefaultGroundCheckDistance;

        Velocity = Vector3.zero;
        simulatedRotation = 0f;
    }

    private void Awake()
    {
        rb = rb ?? GetComponent<Rigidbody>();
        animator = animator ?? GetComponent<Animator>();
        sensitivity = sensitivity > 0f ? sensitivity : DefaultSensitivity;
        groundCheckDistance = groundCheckDistance > 0f ? groundCheckDistance : DefaultGroundCheckDistance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Velocity = Vector3.zero;
        simulatedRotation = 0f;
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    public void SetRotationSimulation(float rotation)
    {
        simulatedRotation = rotation;
    }

    /// <summary>
    /// 플레이어의 이동을 구현합니다.
    /// </summary>
    private void Move()
    {
        Vector3 movement = transform.TransformDirection(Velocity) * Time.fixedDeltaTime;

        if (rb)
        {
            rb.MovePosition(rb.position + movement);
            return;
        }

        transform.position += movement;
    }

    /// <summary>
    /// 플레이어의 좌우 회전을 구현합니다.
    /// </summary>
    private void Rotate()
    {
        float rotation = simulatedRotation * sensitivity * Time.fixedDeltaTime;

        if (rb)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotation, 0f));
            return;
        }

        transform.Rotate(Vector3.up, rotation, Space.Self);
    }
}
