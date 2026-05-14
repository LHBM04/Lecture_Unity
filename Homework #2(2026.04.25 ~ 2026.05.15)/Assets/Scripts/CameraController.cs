using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] 
    private Transform target;

    [Space, SerializeField] 
    private Vector3 offset;

    [SerializeField]
    private float moveSpeed;

    private void LateUpdate()
    {
        if (!target)
        {
#if UNITY_EDITOR
            Debug.LogWarning("CameraController: 따라갈 타겟이 설정되지 않았습니다.");
#endif
            return;
        }

        Vector3 desiredPosition = target.position + target.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, moveSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}
