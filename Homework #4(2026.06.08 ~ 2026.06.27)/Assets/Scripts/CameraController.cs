using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Vector3 _offset;

    private void Reset()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _offset = new Vector3(0f, 5f, -10f);
    }

    private void OnValidate()
    {
         if (_target == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("카메라가 따라갈 대상이 없음!!!");
#endif
            return;
        }

        transform.position = _target.position + _offset;
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("카메라가 따라갈 대상이 없음!!!");
#endif
            return;
        }

        transform.position = _target.position + _offset;
    }
}
