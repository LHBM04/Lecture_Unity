using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public sealed class PlayerMovement : MonoBehaviour
{
    private PlayerController controller;

    [Header("Inputs")]
    [SerializeField] 
    private InputActionReference moveInput;

    [SerializeField] 
    private InputActionReference runInput;

    [SerializeField] 
    private InputActionReference jumpInput;

    [SerializeField] 
    private InputActionReference lookInput;

    [SerializeField] 
    private InputActionReference dodgeInput;

    [Header("Movement")]
    [SerializeField] 
    private float moveSpeed = 5.0f;

    [SerializeField] 
    private float runMultiplier = 1.5f;

    [SerializeField]
    private float dodgeMultiplier = 2.0f;

    private Vector2 moveValue;
    private bool shouldRun;
    private bool shouldDodge;

    [Space, SerializeField] private float jumpForce = 5.0f;
    private bool shouldJump;

    [Space, SerializeField] private float lookSensitivity = 100.0f;
    private Vector2 lookValue;

    private Animator animator;

    [Header("Animation")]
    [SerializeField] 
    private string walkBool = "Walk";
    private int walkBoolHash;

    [SerializeField] 
    private string runBool = "Run";
    private int runBoolHash;

    [SerializeField] 
    private string dodgeBool = "Dodge";
    private int dodgeBoolHash;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        if (!controller)
        {
#if UNITY_EDITOR
            Debug.LogError($"{nameof(PlayerController)}를 찾을 수 없습니다.", this);
#endif
            enabled = false;
            return;
        }

        animator = controller.Animator;

        if (animator)
        {
            walkBoolHash = string.IsNullOrWhiteSpace(walkBool) ? 0 : Animator.StringToHash(walkBool);
            runBoolHash = string.IsNullOrWhiteSpace(runBool) ? 0 : Animator.StringToHash(runBool);
            dodgeBoolHash = string.IsNullOrWhiteSpace(dodgeBool) ? 0 : Animator.StringToHash(dodgeBool);
        }
    }

    private void OnEnable()
    {
        if (moveInput)
        {
            moveInput.action.performed += OnMovePerformed;
            moveInput.action.canceled += OnMoveCanceled;
            moveInput.action.Enable();
        }

        if (runInput)
        {
            runInput.action.performed += OnRunPerformed;
            runInput.action.canceled += OnRunCanceled;
            runInput.action.Enable();
        }

        if (jumpInput)
        {
            jumpInput.action.performed += OnJumpPerformed;
            jumpInput.action.Enable();
        }

        if (lookInput)
        {
            lookInput.action.performed += OnLookPerformed;
            lookInput.action.canceled += OnLookCanceled;
            lookInput.action.Enable();
        }

        if (dodgeInput)
        {
            dodgeInput.action.performed += OnDodgePerformed;
            dodgeInput.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveInput)
        {
            moveInput.action.performed -= OnMovePerformed;
            moveInput.action.canceled -= OnMoveCanceled;
            moveInput.action.Disable();
        }

        if (runInput)
        {
            runInput.action.performed -= OnRunPerformed;
            runInput.action.canceled -= OnRunCanceled;
            runInput.action.Disable();
        }

        if (jumpInput)
        {
            jumpInput.action.performed -= OnJumpPerformed;
            jumpInput.action.Disable();
        }

        if (lookInput)
        {
            lookInput.action.performed -= OnLookPerformed;
            lookInput.action.canceled -= OnLookCanceled;
            lookInput.action.Disable();
        }

        if (dodgeInput)
        {
            dodgeInput.action.performed -= OnDodgePerformed;
            dodgeInput.action.Disable();
        }
    }

    private void Update()
    {
        HandleRotation();
        HandleAnimation();
    }

    private void FixedUpdate()
    {
        HandleVelocity();
        shouldJump = false;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveValue = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveValue = Vector2.zero;
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        shouldRun = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        shouldRun = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (controller.IsGrounded)
        {
            shouldJump = true;
        }
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        lookValue = context.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookValue = Vector2.zero;
    }

    private void OnDodgePerformed(InputAction.CallbackContext context)
    {
    }

    private void HandleVelocity()
    {
        if (shouldDodge)
        {
        }
        else
        {

        }
        float totalSpeed = shouldRun
            ? moveSpeed * runMultiplier
            : moveSpeed;

        Vector3 localMove = new Vector3(moveValue.x, 0.0f, moveValue.y);
        localMove = Vector3.ClampMagnitude(localMove, 1.0f);

        Vector3 velocity = transform.TransformDirection(localMove) * totalSpeed;

        velocity.y = controller.Velocity.y;

        if (controller.IsGrounded && shouldJump)
        {
            velocity.y = jumpForce;
        }

        controller.Velocity = velocity;
    }

    private void HandleRotation()
    {
        Vector3 rotation = transform.eulerAngles;
        rotation.y += lookValue.x * lookSensitivity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void HandleAnimation()
    {
        if (!animator)
        {
            return;
        }

        if (walkBoolHash != 0)
        {
            animator.SetBool(walkBoolHash, moveValue.sqrMagnitude > 0.0f);
        }

        if (runBoolHash != 0)
        {
            animator.SetBool(runBoolHash, shouldRun && moveValue.sqrMagnitude > 0.0f);
        }
    }
}