using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private void Awake()
    {
        target = target ?? GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("타겟이 없다!!");
#endif
            return;
        }


    }
}
