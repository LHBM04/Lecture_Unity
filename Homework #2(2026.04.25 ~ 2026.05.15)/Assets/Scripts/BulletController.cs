using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody;

    [Space, SerializeField]
    private float _moveSpeed = 30.0f;

    [SerializeField]
    private float _lifeTime = 3.0f;

    private void Reset()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        _rigidbody = _rigidbody ?? GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rigidbody.useGravity = false;
        _rigidbody.linearVelocity = transform.forward * _moveSpeed;
        Destroy(gameObject, _lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroyImmediate(gameObject);
    }
}
