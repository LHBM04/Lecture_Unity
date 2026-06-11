using UnityEngine;

public class KickEvents : MonoBehaviour
{
    [SerializeField]
    private PlayerController_Dribble controller;

    private void Reset()
    {
        controller = GetComponentInParent<PlayerController_Dribble>();
    }

    private void Awake()
    {
        controller = controller ?? GetComponentInParent<PlayerController_Dribble>();
    }

    public void KickAsFreeKick()
    {
        if (controller == null)
        {
#if UNITY_EDITOR
            Debug.LogError("부모 객체에서 PlayerController_Dribble을 찾을 수 없습니다.");
#endif
        }

        controller.ExecuteKick();
    }

    public void KickAsPenaltyKick()
    {
        if (controller == null)
        {
#if UNITY_EDITOR
            Debug.LogError("부모 객체에서 PlayerController_Dribble을 찾을 수 없습니다.");
#endif
        }

        controller.ExecuteKick();
    }
}
