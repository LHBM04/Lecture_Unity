using UnityEngine;

public class CameraController : MonoBehaviour
{
    /// <summary>
    /// 따라갈 대상.
    /// </summary>
    [Tooltip("따라갈 대상.")]
    [SerializeField] 
    private Transform target;

    /// <summary>
    /// 카메라와 대상 사이의 오프셋.
    /// </summary>
    [Tooltip("카메라와 대상 사이의 오프셋.")]
    [SerializeField] 
    private Vector3 offset;

    /// <summary>
    /// 카메라 이동 속도.
    /// </summary>
    [Tooltip("카메라 이동 속도.")]
    [SerializeField] 
    private float moveSpeed;

    private void LateUpdate()
    {
        if (!target)
        {
            Debug.LogWarning("CameraController: 따라갈 타겟이 설정되지 않았습니다.");
            return;
        }

        Vector3 desiredPosition = target.position + target.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, moveSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}
