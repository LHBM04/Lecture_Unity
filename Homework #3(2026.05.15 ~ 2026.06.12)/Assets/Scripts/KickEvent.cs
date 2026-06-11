using UnityEngine;

public class KickEvent : MonoBehaviour
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
            Debug.LogError("부모 객체에서 PlayerController_Dribble을 찾을 수 없습니다.");
#endif
        }

        controller.Kick();
    }
}
