using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 컨트롤러
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

    private PlayerInputActions playerActions;
    private InputAction moveAction;

    /// <summary>
    /// 플레이어 현재 위치.
    /// </summary>
    [HideInInspector] 
    public Vector3 direction;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private string dribbleBool;
    private int dribbleBoolHash;

    [SerializeField]
    private string shootBool;
    private int shootBoolHash;

    [SerializeField]
    private string penaltyKickBool;
    private int penaltyKickBoolHash;

    private void Reset()
    {
        controller = GetComponentInParent<CharacterController>();
    }

    private void Awake()
    {
        controller = controller ?? GetComponentInParent<CharacterController>();

        playerActions = new PlayerInputActions();
        moveAction = playerActions.Player.Move;

        dribbleBoolHash = !string.IsNullOrEmpty(dribbleBool) ? Animator.StringToHash(dribbleBool) : 0;
        shootBoolHash = !string.IsNullOrEmpty(shootBool) ? Animator.StringToHash(shootBool) : 0;
        penaltyKickBoolHash = !string.IsNullOrEmpty(penaltyKickBool) ? Animator.StringToHash(penaltyKickBool) : 0;
    }

    private void OnEnable()
    {
        moveAction.Enable();

        // 드리블 동작
        {
            moveAction.started += OnMoveStarted;
            moveAction.performed += OnMovePerformed;
            moveAction.canceled += OnMoveCanceled;
        }
    }

    private void FixedUpdate()
    {
        if (controller.isGrounded)
        {
            // 이동 로직
        }

        direction.y += Physics.gravity.y * Time.fixedDeltaTime;
        controller.Move(direction * Time.fixedDeltaTime);
    }

    private void OnDisable()
    {
        moveAction.Disable();

        // 드리블 동작
        {
            moveAction.started -= OnMoveStarted;
            moveAction.performed -= OnMovePerformed;
            moveAction.canceled -= OnMoveCanceled;
        }
    }

    private void OnMoveStarted(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        Debug.Log("드리블 시작");
#endif

        if (animator != null && dribbleBoolHash != 0)
        {
            animator.SetBool(dribbleBoolHash, true);
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        Debug.Log("드리블 진행");
#endif
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        Debug.Log("드리블 종료");
#endif

        if (animator != null && dribbleBoolHash != 0)
        {
            animator.SetBool(dribbleBoolHash, false);
        }
    }
}
