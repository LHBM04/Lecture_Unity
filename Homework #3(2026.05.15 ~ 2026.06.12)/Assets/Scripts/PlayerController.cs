using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 컨트롤러
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Rigidbody rigidbody;

    [SerializeField]
    private Animator animator;

    [Header("Status")]
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

    [SerializeField]
    private float kickForce;

    [SerializeField]
    private float upwardAngle;

    [SerializeField]
    private float maxChargeTime;
    private float chargeStartTime;

    [SerializeField]
    private float minPowerMultiplier;
    private float currentPowerMultiplier;
    
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

    [SerializeField]
    private string penaltyKickTrigger;
    private int penaltyKickTriggerHash;

    private Vector2 direction;
    private bool isSprinting;
    private bool isDribbling;
    private bool isKicking;

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

        kickForce = 25.0f;
        upwardAngle = 0.3f;

        maxChargeTime = 1.0f;
        minPowerMultiplier = 0.3f;

        jogBool = "Jog";
        sprintBool = "Sprint";
        dribbleBool = "Dribble";
        freeKickTrigger = "Free Kick";
        penaltyKickTrigger = "Penalty Kick";
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
        penaltyKickTriggerHash = !string.IsNullOrEmpty(penaltyKickTrigger) ? Animator.StringToHash(penaltyKickTrigger) : 0;

        isSprinting = false;
        isDribbling = false;
        isKicking = false;
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
        kickAction.started += OnKickStarted;
        kickAction.canceled += OnKickCanceled;
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
        kickAction.started -= OnKickStarted;
        kickAction.canceled -= OnKickCanceled;
    }

    private void FixedUpdate()
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

    public void ChangeMode(GameManager.GameMode mode)
    {
        switch(mode)
        {
            case GameManager.GameMode.Dribble:
                jogAction.Enable();
                sprintAction.Enable();
                kickAction.Disable();
                break;
            case GameManager.GameMode.FreeKick:
            case GameManager.GameMode.PenaltyKick:
                jogAction.Disable();
                sprintAction.Disable();
                kickAction.Enable();
                break;
        }
    }

    public void Kick()
    {
        if (ballTransform != null && ballTransform.TryGetComponent(out Rigidbody ballRigidbody))
        {
            float finalForce = kickForce * currentPowerMultiplier;
            Vector3 kickDirection = (transform.forward + (Vector3.up * upwardAngle)).normalized;

            ballRigidbody.linearVelocity = Vector3.zero;
            ballRigidbody.AddForce(kickDirection * finalForce, ForceMode.Impulse);
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

    private void OnKickStarted(InputAction.CallbackContext context)
    {
        chargeStartTime = Time.time;
        isKicking = true;
    }

    private void OnKickCanceled(InputAction.CallbackContext context)
    {
        float holdTime = Time.time - chargeStartTime;

        currentPowerMultiplier = Mathf.Clamp(holdTime / maxChargeTime, minPowerMultiplier, 1.0f);
        isKicking = false;

        if (animator != null)
        {
            var currentMode = GameManager.Instance.CurrentGameMode;

            switch (currentMode)
            {
                case GameManager.GameMode.FreeKick:
                    if (freeKickTriggerHash != 0)
                    {
                        animator.SetTrigger(freeKickTriggerHash);
                    }
                    break;
                case GameManager.GameMode.PenaltyKick:
                    if (penaltyKickTriggerHash != 0)
                    {
                        animator.SetTrigger(penaltyKickTriggerHash);
                    }
                    break;
            }
        }
    }
}
