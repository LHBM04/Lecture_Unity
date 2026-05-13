using UnityEngine;

/// <summary>
/// 플레이어 입력을 이동 시뮬레이션 값으로 변환합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// 해당 플레이어의 컨트롤러.
    /// </summary>
    [HideInInspector]
    public PlayerController controller;

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
    [Tooltip("해당 플레이어의 좌우 이동 키 이름.")]
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
    /// 해당 플레이어의 좌우 회전 입력 축 이름.
    /// </summary>
    [Tooltip("해당 플레이어의 좌우 회전 입력 축 이름.")]
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

    /// <summary>
    /// 해당 플레이어의 속도에 따른 애니메이션 Float 파라미터 이름.
    /// </summary>
    [Tooltip("해당 플레이어의 속도에 따른 애니메이션 Float 파라미터 이름.")]
    [SerializeField]
    private string speedFloat;
    private int speedFloatHash;

    /// <summary>
    /// 해당 플레이어의 접지 여부에 따른 애니메이션 Bool 파라미터 이름.
    /// </summary>
    [Tooltip("해당 플레이어의 접지 여부에 따른 애니메이션 Bool 파라미터 이름.")]
    [SerializeField]
    private string groundedBool;
    private int groundedBoolHash;

    private void Reset()
    {
        controller = GetComponent<PlayerController>();

        verticalAxis = "Vertical";
        horizontalAxis = "Horizontal";
        runAxis = "Run";
        jumpAxis = "Jump";
        mouseXAxis = "Mouse X";

        moveSpeed = 5.0f;
        jumpForce = 5f;
    }

    private void Awake()
    {
        controller = controller ?? GetComponent<PlayerController>();

        if (controller && controller.Animator)
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

    /// <summary>
    /// 플레이어의 이동 입력을 속도로 변환합니다.
    /// </summary>
    private void Move()
    {
        float horizontal = Input.GetAxisRaw(horizontalAxis);
        float vertical = Input.GetAxisRaw(verticalAxis);
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

        float targetSpeed = Input.GetButton(runAxis) ? moveSpeed * 1.5f : moveSpeed;
        Vector3 velocity = Vector3.ClampMagnitude(inputDirection, 1f) * targetSpeed;

        if (controller)
        {
            controller.Velocity = velocity;
        }

        if (controller && controller.Animator && speedFloatHash != 0)
        {
            controller.Animator.SetFloat(speedFloatHash, velocity.magnitude);
        }
    }

    /// <summary>
    /// 플레이어의 점프를 구현합니다.
    /// </summary>
    private void Jump()
    {
        if (Input.GetButtonDown(jumpAxis))
        {
            if (!controller || !controller.Rigidbody)
            {
                Debug.LogWarning("PlayerMovement: Rigidbody를 찾을 수 없습니다!");
                return;
            }

            if (controller.IsGrounded)
            {
                controller.Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        if (controller && controller.Animator && groundedBoolHash != 0)
        {
            controller.Animator.SetBool(groundedBoolHash, controller.IsGrounded);
        }
    }

    /// <summary>
    /// 플레이어의 좌우 회전 입력을 컨트롤러에 전달합니다.
    /// </summary>
    private void Rotate()
    {
        float mouseX = Input.GetAxisRaw(mouseXAxis);

        if (controller)
        {
            controller.SetRotationSimulation(mouseX);
        }
    }
}
