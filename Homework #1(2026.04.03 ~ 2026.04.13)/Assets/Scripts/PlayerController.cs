using UnityEngine;

public class PlayerController : MonoBehaviour
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

    [Tooltip("점프 힘.")]
    [SerializeField]
    private float jumpForce;

    /// <summary>
    /// 회전 민감도.
    /// </summary>
    [Header("Other Settings")]
    [Tooltip("회전 민감도.")]
    [SerializeField]
    private float sensitivity;

    /// <summary>
    /// 
    /// </summary>
    [Tooltip("바닥 체크 거리.")]
    [SerializeField]
    private float groundCheckDistance;

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
    }

    private void Awake()
    {
        rb = rb ?? GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Move();
        Jump();
        Rotate();
    }

    /// <summary>
    /// 플레이어의 이동을 구현합니다.
    /// </summary>
    private void Move()
    {
        float horizontal = Input.GetAxisRaw(horizontalAxis);
        float vertical = Input.GetAxisRaw(verticalAxis);
        float totalSpeed = Input.GetButton(runAxis) ? moveSpeed * 1.5f : moveSpeed;

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * Time.deltaTime * totalSpeed;
        transform.Translate(movement, Space.Self);
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
    }

    /// <summary>
    /// 플레이어의 좌우 회전을 구현합니다.
    /// </summary>
    private void Rotate()
    {
        float mouseX = Input.GetAxisRaw(mouseXAxis);
        transform.Rotate(Vector3.up, mouseX * sensitivity, Space.Self);
    }
}
