using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerController controller;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private string availableTrigger;
    private int availableTriggerHash;

    [SerializeField]
    private string activeBool;
    private int activeBoolHash;

    [SerializeField]
    private InputActionReference actionReference;

    protected virtual void Reset()
    {
        // 항상 부모 객체보다 하위에 있을 거니까 부모 객체를 향해서 검색해야 함.
        controller = GetComponentInParent<PlayerController>();
    }

    protected virtual void Awake()
    {
        controller = controller ?? GetComponentInParent<PlayerController>();

        animator = animator ?? controller.animator;
        availableTriggerHash = !string.IsNullOrEmpty(availableTrigger) ? Animator.StringToHash(availableTrigger) : 0;
        activeBoolHash = !string.IsNullOrEmpty(activeBool) ? Animator.StringToHash(activeBool) : 0;
    }

    protected virtual void OnEnable()
    {
        if (actionReference == null)
        {
#if UNITY_EDITOR
            Debug.LogError($"할당된 InputActionReference가 없음!! --> {nameof(PlayerAction)}");
#endif
            return;
        }

        actionReference.action.started += OnActionStarted;
        actionReference.action.performed += OnActionPerformed;
        actionReference.action.canceled += OnActionCanceled;
    }

    protected virtual void OnDisable()
    {
        if (actionReference == null)
        {
#if UNITY_EDITOR
            Debug.LogError($"할당된 InputActionReference가 없음!! --> {nameof(PlayerAction)}");
#endif
            return;
        }

        actionReference.action.started -= OnActionStarted;
        actionReference.action.performed -= OnActionPerformed;
        actionReference.action.canceled -= OnActionCanceled;
    }

    protected virtual void OnActionStarted(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnActionPerformed(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnActionCanceled(InputAction.CallbackContext context) 
    {
    }
}
