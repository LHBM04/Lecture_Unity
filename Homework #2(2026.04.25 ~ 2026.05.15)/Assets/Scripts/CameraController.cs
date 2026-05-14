using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] 
    private Transform target;

    private PlayerController _playerController;

    [Space, SerializeField] 
    private Vector3 offset;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float lookHeight;

    private void Awake()
    {
        if (target)
        {
            _playerController = target.GetComponent<PlayerController>();
        }
    }

    private void LateUpdate()
    {
        if (!target)
        {
#if UNITY_EDITOR
            Debug.LogWarning("CameraController: 따라갈 타겟이 설정되지 않았습니다!");
#endif
            return;
        }

        if (_playerController == null)
        {
            _playerController = target.GetComponent<PlayerController>();
        }

        if (moveSpeed <= 0.0f)
        {
            moveSpeed = 14.0f;
        }

        if (lookHeight <= 0.0f)
        {
            lookHeight = 1.5f;
        }

        Quaternion cameraYawRotation = _playerController != null
            ? _playerController.CameraYawRotation
            : target.rotation;

        Vector3 desiredPosition = target.position + cameraYawRotation * offset;
        Vector3 lookTarget = target.position + Vector3.up * lookHeight;
        float positionT = 1.0f - Mathf.Exp(-moveSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionT);
        Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
        transform.rotation = desiredRotation;
    }
}
