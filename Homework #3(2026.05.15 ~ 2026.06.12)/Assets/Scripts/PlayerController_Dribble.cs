using NUnit.Framework.Constraints;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 컨트롤러
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController_Dribble : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Rigidbody rigidbody;

    [SerializeField]
    private Animator animator;

    [Header("Movement")]
    [SerializeField]
    private float jogSpeed;

    [SerializeField]
    private float sprintSpeed;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField] 
    private Transform ballTransform;

    [SerializeField] 
    private float dribbleRange;

    public Transform dribblePoint;

    [Header("Animations")]
    [SerializeField]
    private string jogBool;
    private int jogBoolHash;

    [SerializeField]
    private string sprintBool;
    private int sprintBoolHash;

    [SerializeField]
    private string dribbleBool;
    private int dribbleBoolHash;

    [SerializeField]
    private string freeKickTrigger;
    private int freeKickTriggerHash;

    private Vector2 direction;
    private bool isSprinting;
    private bool isDribbling;

    private PlayerInputActions playerActions;
    private InputAction jogAction;
    private InputAction sprintAction;
    private InputAction kickAction;

    private void Reset()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        jogSpeed = 3.0f;
        sprintSpeed = 5.5f;
        rotateSpeed = 10.0f;

        jogBool = "Jog";
        sprintBool = "Sprint";
        dribbleBool = "Dribble";
        freeKickTrigger = "Freekick";
    }

    private void Awake()
    {
        rigidbody = rigidbody ?? GetComponent<Rigidbody>();

        playerActions = new PlayerInputActions();
        jogAction = playerActions.Player.Jog;
        sprintAction = playerActions.Player.Sprint;
        kickAction = playerActions.Player.Kick;

        animator = animator ?? GetComponentInChildren<Animator>();
        jogBoolHash = !string.IsNullOrEmpty(jogBool) ? Animator.StringToHash(jogBool) : 0;
        sprintBoolHash = !string.IsNullOrEmpty(sprintBool) ? Animator.StringToHash(sprintBool) : 0;
        dribbleBoolHash = !string.IsNullOrEmpty(dribbleBool) ? Animator.StringToHash(dribbleBool) : 0;
        freeKickTriggerHash = !string.IsNullOrEmpty(freeKickTrigger) ? Animator.StringToHash(freeKickTrigger) : 0;

        isSprinting = false;
        isDribbling = false;
    }

    private void OnEnable()
    {
        if (jogAction == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Move Action이 없다!!");
#endif
            return;
        }

        jogAction.Enable();
        jogAction.started += OnJogStarted;
        jogAction.performed += OnJogPerformed;
        jogAction.canceled += OnJogCanceled;

        sprintAction.Enable();
        sprintAction.started += OnSprintStarted;
        sprintAction.canceled += OnSprintCanceled;

        kickAction.Enable();
        kickAction.started += OnFreeKickStarted;
    }

    private void OnDisable()
    {
        if (jogAction == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Move Action이 없다!!");
#endif
            return;
        }

        jogAction.Disable();
        jogAction.started -= OnJogStarted;
        jogAction.performed -= OnJogPerformed;
        jogAction.canceled -= OnJogCanceled;

        sprintAction.Disable();
        sprintAction.started -= OnSprintStarted;
        sprintAction.canceled -= OnSprintCanceled;

        kickAction.Disable();
        kickAction.started -= OnFreeKickStarted;
    }

    private void FixedUpdate()
    {
        CheckDribbleState();
        GroundControl();
    }

    private void GroundControl()
    {
        if (direction == Vector2.zero)
        {
            rigidbody.linearVelocity = new Vector3(0, rigidbody.linearVelocity.y, 0);
            return;
        }

        Vector3 moveDirection = new Vector3(direction.x, 0f, direction.y).normalized;
        float currentSpeed = isSprinting ? sprintSpeed : jogSpeed;

        Vector3 targetVelocity = moveDirection * currentSpeed;
        targetVelocity.y = rigidbody.linearVelocity.y;

        rigidbody.linearVelocity = targetVelocity;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
    }

    private void CheckDribbleState()
    {
        if (ballTransform == null || animator == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, ballTransform.position);
        bool nearBall = (distance <= dribbleRange) && (direction != Vector2.zero);

        if (nearBall != isDribbling)
        {
            isDribbling = nearBall;

            if (dribbleBoolHash != 0)
            {
                animator.SetBool(dribbleBoolHash, isDribbling);
            }
        }
    }

    [Header("Kick Settings")]
    [SerializeField] 
    private float kickForce = 20.0f;
    
    [SerializeField] 
    private float upwardAngle = 0.2f;

    [SerializeField] 
    private float kickCooldown = 0.5f;

    private float nextDribbleTime = 0f;

    public void ExecuteKick()
    {
        if (ballTransform == null && ballTransform.TryGetComponent(out Rigidbody ballRigidbody))
        {
            Vector3 kickDirection = (transform.forward + (Vector3.up * upwardAngle)).normalized;

            ballRigidbody.linearVelocity = Vector3.zero;
            ballRigidbody.AddForce(kickDirection * kickForce, ForceMode.Impulse);

            nextDribbleTime = Time.time + kickCooldown;
            isDribbling = false;

            if (animator != null && dribbleBoolHash != 0)
            {
                animator.SetBool(dribbleBoolHash, false);
            }
        }
    }

    private void OnJogStarted(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();

        if (animator != null && jogBoolHash != 0)
        {
            animator.SetBool(jogBoolHash, true);
        }
    }

    private void OnJogPerformed(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    private void OnJogCanceled(InputAction.CallbackContext context)
    {
        direction = Vector2.zero;

        if (animator != null && jogBoolHash != 0)
        {
            animator.SetBool(jogBoolHash, false);
        }
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        isSprinting = true;

        if (animator != null && sprintBoolHash != 0)
        {
            animator.SetBool(sprintBoolHash, true);
        }
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;

        if (animator != null && sprintBoolHash != 0)
        {
            animator.SetBool(sprintBoolHash, false);
        }
    }

    private void OnFreeKickStarted(InputAction.CallbackContext context)
    {
        if (!isDribbling)
        {
            return;
        }

        if (animator != null && freeKickTriggerHash != 0)
        {
            animator.SetTrigger(freeKickTriggerHash);

            // tartCoroutine(WaitForFreeKickAnimationCoroutine());
        }
    }

    private void OnPenaltyKickStarted(InputAction.CallbackContext context)
    {
        if (!isDribbling)
        {
            return;
        }

        if (animator != null && freeKickTriggerHash != 0)
        {
            animator.SetTrigger(freeKickTriggerHash);
        }
    }
}
