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

    [SerializeField]
    private float lookHeight;

    private void Reset()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

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

        Vector3 desiredPosition = target.position + offset;
        Vector3 lookTarget = target.position + Vector3.up * lookHeight;

        float positionT = 1.0f - Mathf.Exp(-moveSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            positionT);

        Quaternion desiredRotation = Quaternion.LookRotation(
            lookTarget - transform.position,
            Vector3.up);

        transform.rotation = desiredRotation;
    }
}
