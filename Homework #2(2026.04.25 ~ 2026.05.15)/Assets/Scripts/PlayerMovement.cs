using UnityEngine;
using UnityEngine.InputSystem;

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

    private Animator animator;

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
        animator = controller.Animator;

        moveSpeed = 5.0f;
        jumpForce = 5f;
    }

    private void Awake()
    {
        controller = controller ?? GetComponent<PlayerController>();
        animator = animator ?? controller.Animator;
        speedFloatHash = !string.IsNullOrEmpty(speedFloat) ? Animator.StringToHash(speedFloat) : 0;
        groundedBoolHash = !string.IsNullOrEmpty(groundedBool) ? Animator.StringToHash(groundedBool) : 0;
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        controller.Velocity += new Vector3(input.x, 0.0f, input.y) * moveSpeed;
    }

    public void OnJump(InputValue value)
    {
        if (!controller.IsGrounded)
        {
            return;
        }

        if (value.isPressed)
        {
            controller.Velocity += Vector3.up * jumpForce;
        }
    }
}
