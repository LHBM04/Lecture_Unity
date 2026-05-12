using UnityEngine;

/// <summary>
/// 카메라의 이동을 구현합니다.
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// 해당 카메라가 따라갈 대상.
    /// </summary>
    [Header("Status")]
    [Tooltip("해당 카메라가 따라갈 대상.")]
    [SerializeField] 
    private Transform target;

    /// <summary>
    /// 해당 카메라와 대상 사이의 오프셋.
    /// </summary>
    [Space(5.0f)]
    [Tooltip("해당카메라와 대상 사이의 오프셋.")]
    [SerializeField] 
    private Vector3 offset;

    /// <summary>
    /// 해당 카메라의 이동 속도.
    /// </summary>
    [Tooltip("해당 카메라의 이동 속도.")]
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
