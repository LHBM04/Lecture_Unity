using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{
    private PlayerController controller;

    public float Health
    {
        get;
        set;
    }

    public bool IsDamaged
    {
        get;
        set;
    }

    public bool IsDead
    {
        get;
        set;
    }

    [Header("Tags")]
    [SerializeField]
    private string harmfulTag;

    [SerializeField]
    private string lethalTag;

    private Animator animator;

    [Header("Animations")]
    [SerializeField]
    private string damageTrigger;
    private int damageTriggerHash;

    [SerializeField]
    private string deadTrigger;
    private int deadTriggerHash;

    private void Reset()
    {
        harmfulTag = "Harmful";
        lethalTag = "Lethal";

        damageTrigger = "Damage";
        deadTrigger = "Dead";
    }

    private void Awake()
    {
        controller = controller ?? GetComponentInParent<PlayerController>();
        if (controller == null)
        {
#if UNITY_EDITOR
            Debug.LogError($"{nameof(PlayerController)}를 찾을 수 없습니다.", this);
#endif
            enabled = false;
            return;
        }

        animator = controller.Animator;
        if (!animator)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Animator를 찾을 수 없습니다!");
#endif
            return;
        }

        damageTriggerHash = !string.IsNullOrWhiteSpace(damageTrigger) ? Animator.StringToHash(damageTrigger) : 0;
        deadTriggerHash = !string.IsNullOrWhiteSpace(deadTrigger) ? Animator.StringToHash(deadTrigger) : 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(harmfulTag))
        {
            TakeDamage();
        }
        else if (other.CompareTag(lethalTag))
        {
            Kiil();
        }
    }

    private void TakeDamage()
    {
        if ((Health -= 1.0f) >= 0.0f)
        {
            IsDamaged = true; 
            
            if (animator && damageTriggerHash != 0)
            {
                animator.SetTrigger(damageTriggerHash);
            }
        }
        else
        {
            Health = 0.0f;
            IsDead = true;

            if (animator && deadTriggerHash != 0)
            {
                animator.SetTrigger(deadTriggerHash);
            }
        }
    }

    private void Kiil()
    {
        IsDead = true;

        if (animator && deadTriggerHash != 0)
        {
            animator.SetTrigger(deadTriggerHash);
        }
    }
}