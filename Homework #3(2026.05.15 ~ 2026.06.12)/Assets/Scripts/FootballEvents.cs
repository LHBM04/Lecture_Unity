using UnityEngine;

public class FootballEvents : MonoBehaviour
{
    [SerializeField]
    private PlayerController controller;

    private void Reset()
    {
        controller = GetComponentInParent<PlayerController>();
    }

    private void Awake()
    {
        controller = controller ?? GetComponentInParent<PlayerController>();
    }
    
    public void Kick()
    {
        if (controller == null)
        {
#if UNITY_EDITOR
            Debug.LogError("PlayerController 없다!!!");
#endif
        }

        controller.Kick();
    }

    public void Dribble()
    {
        if (controller == null)
        {
#if UNITY_EDITOR
            Debug.LogError("PlayerController 없다!!!");
#endif
        }

        controller.Dribble();
    }
}
