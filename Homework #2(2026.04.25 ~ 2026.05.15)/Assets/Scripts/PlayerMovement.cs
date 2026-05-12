using UnityEngine;

/// <summary>
/// 플레이어의 이동을 구현합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// 해당 플레이어의 Rigidbody.
    /// </summary>
    [Tooltip("해당 플레이어의 Rigidbody.")]
    [SerializeField]
    private Rigidbody rb;

    /// <summary>
    /// 해당 플레이어의 상하 이동 키 이름.
    /// </summary>
    [Space(5.0f)]
    [Tooltip("해당 플레이어의 상하 이동 키 이름.")]
    [SerializeField]
    private string verticalAxis;

    /// <summary>
    /// 해당 플레이어의 좌우 이동 키 이름.
    /// </summary>
    [Tooltip("해당 카메라의 좌우 이동 키 이름.")]
    [SerializeField]
    private string horizontalAxis;

    /// <summary>
    /// 해당 플레이어의 달리기 키 이름.
    /// </summary>
    [Tooltip("해당 플레이어의 달리기 키 이름.")]
    [SerializeField]
    private string runAxis;

    /// <summary>
    /// 해당 플레이어의 점프 키 이름.
    /// </summary>
    [Tooltip("해당 플레이어의 점프 키 이름.")]
    [SerializeField]
    private string jumpAxis;

    /// <summary>
    /// 해당 플레이어의 좌우 이동 키 이름.
    /// </summary>
    [Tooltip("해당 카메라의 좌우 이동 키 이름.")]
    [SerializeField]
    private string mouseXAxis;

    /// <summary>
    /// 해당 플레이어의 이동 속도.
    /// </summary>
    [Header("Status")]
    [Tooltip("해당 플레이어의 이동 속도.")]
    [SerializeField]
    private float moveSpeed;

    /// <summary>
    /// 해당 플레이어의 점프 힘.
    /// </summary>
    [Tooltip("해당 플레이어의 점프 힘.")]
    [SerializeField]
    private float jumpForce;

    private float velocity;

    /// <summary>
    /// 해당 플레이어의 회전 민감도.
    /// </summary>
    [Header("Other Settings")]
    [Tooltip("해당 플레이어의 회전 민감도.")]
    [SerializeField]
    private float sensitivity;

    /// <summary>
    /// 해당 플레이어의 바닥 체크 거리.
    /// </summary>
    [Tooltip("해당 플레이어의 바닥 체크 거리.")]
    [SerializeField]
    private float groundCheckDistance;

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

    /// <summary>
    /// 해당 플레이어의 애니메이터.
    /// </summary>
    [Header("Animations")]
    [Tooltip("해당 플레이어의 애니메이터.")]
    [SerializeField]
    private Animator animator;

    /// <summary>
    /// 해당 플레이어의 속도에 따른 애니메이션 Float 파라미터 이름.
    /// </summary>
    [Tooltip("해당 플레이어의 속도에 따른 애니메이션 Float 파라미터 이름.")]
    [SerializeField]
    private string speedFloat;
    private int speedFloatHash;

    /// <summary>
    /// 해당 플레이어의 속도에 따른 애니메이션 Float 파라미터 이름.
    /// </summary>
    [Tooltip("해당 플레이어의 속도에 따른 애니메이션 Float 파라미터 이름.")]
    [SerializeField]
    private string groundedBool;
    private int groundedBoolHash;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();

        verticalAxis = "Vertical";
        horizontalAxis = "Horizontal";
        runAxis = "Run";
        jumpAxis = "Jump";
        mouseXAxis = "Mouse X";

        moveSpeed = 5.0f;
        jumpForce = 5f;
        
        sensitivity = 2f;
        groundCheckDistance = 1.2f;

        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        rb = rb ?? GetComponent<Rigidbody>();

        animator = animator ?? GetComponent<Animator>();
        if (animator)
        {
            speedFloatHash = !string.IsNullOrEmpty(speedFloat) ? Animator.StringToHash(speedFloat) : 0;
            groundedBoolHash = !string.IsNullOrEmpty(groundedBool) ? Animator.StringToHash(groundedBool) : 0;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Move();
        Jump();
        Rotate();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + Vector3.down * groundCheckDistance);
    }
#endif

    /// <summary>
    /// 플레이어의 이동을 구현합니다.
    /// </summary>
    private void Move()
    {
        float horizontal = Input.GetAxisRaw(horizontalAxis);
        float vertical = Input.GetAxisRaw(verticalAxis);

        if (horizontal <= 0f && vertical <= 0f)
        {
            if (animator && speedFloatHash != 0)
            {
                animator.Update(0.0f);
                animator.SetFloat(speedFloatHash, 0f);
            }
        }
        else
        {
            float moveSpeed = Input.GetButton(runAxis) ? this.moveSpeed * 1.5f : this.moveSpeed;

            Vector3 movement = new Vector3(horizontal, 0f, vertical) * Time.deltaTime * moveSpeed;
            transform.Translate(movement, Space.Self);

            if (animator && speedFloatHash != 0)
            {
                animator.Update(0.0f);
                animator.SetFloat(speedFloatHash, moveSpeed);
            }
        }
    }

    /// <summary>
    /// 플레이어의 점프를 구현합니다.
    /// </summary>
    private void Jump()
    {
        if (Input.GetButtonDown(jumpAxis))
        {
            if (!rb)
            {
                Debug.LogWarning("PlayerController: Rigidbody를 찾을 수 없습니다!");
                return;
            }

            if (IsGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        if (animator && groundedBoolHash != 0)
        {
            animator.Update(0.0f);
            animator.SetBool(groundedBoolHash, IsGrounded);
        }
    }

    /// <summary>
    /// 플레이어의 좌우 회전을 구현합니다.
    /// </summary>
    private void Rotate()
    {
        float mouseX = Input.GetAxisRaw(mouseXAxis);
        transform.Rotate(Vector3.up, mouseX * sensitivity * Time.deltaTime, Space.Self);
    }
}
