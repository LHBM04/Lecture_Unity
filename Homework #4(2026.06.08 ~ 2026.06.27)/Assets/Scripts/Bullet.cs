using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody;

    [Space]
    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _lifeTime;

    private void Reset()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _moveSpeed = 30.0f;
        _lifeTime = 3.0f;
    }

    private void Awake()
    {
        _rigidbody = _rigidbody ?? GetComponent<Rigidbody>();
        //_rigidbody.linearVelocity = transform.forward * _moveSpeed;
    }

    private void Start()
    {
        _rigidbody.AddForce(transform.forward * _moveSpeed, ForceMode.Impulse);
        Destroy(gameObject, _lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
