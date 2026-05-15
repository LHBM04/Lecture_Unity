using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public sealed class EnemyController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string _targetTag = "Player";
    [SerializeField] private float _detectRadius = 5.0f;
    [SerializeField] private LayerMask _targetLayer;

    private readonly Collider[] _detectResults = new Collider[16];
    private Transform _target;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 3.0f;
    [SerializeField] private float _rotateSpeed = 720.0f;
    [SerializeField] private float _targetReachDistance = 0.2f;

    [Header("Wander")]
    [SerializeField] private float _moveDuration = 2.0f;
    [SerializeField] private float _waitDuration = 0.5f;

    private Vector3 _moveDirection;
    private float _stateTimer;
    private bool _isWaiting;

    private bool _isDead;

    private bool _shouldAttack;

    private Coroutine _attackCoroutine;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    [SerializeField] 
    private string _walkBool;
    private int _walkBoolHash;

    [SerializeField]
    private string _attackTrigger;
    private int _attackTriggerHash;

    [SerializeField] 
    private string _deadTrigger;
    private int _deadTriggerHash;

    private void Awake()
    {
        _animator = _animator ? _animator : GetComponentInChildren<Animator>();

        if (_animator)
        {
            _walkBoolHash = string.IsNullOrWhiteSpace(_walkBool)
                ? 0
                : Animator.StringToHash(_walkBool);
            _attackTriggerHash = string.IsNullOrWhiteSpace(_attackTrigger)
                ? 0
                : Animator.StringToHash(_attackTrigger);
            _deadTriggerHash = string.IsNullOrWhiteSpace(_deadTrigger)
                ? 0
                : Animator.StringToHash(_deadTrigger);
        }

        _stateTimer = _moveDuration;
        _isWaiting = false;
        _shouldAttack = false;
    }

    private void Start()
    {
        ChooseRandomDirection();
    }

    private void Update()
    {
        if (_isDead)
        {
            return;
        }

        UpdateTarget();

        if (_target)
        {
            ChaseTarget();
        }
        else
        {
            Wander();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isDead)
        {
            return;
        }

        if (collision.collider.CompareTag("Bullet"))
        {
            _isDead = true;

            if (_animator && _deadTriggerHash != 0)
            {
                _animator.SetTrigger(_deadTriggerHash);
            }

            Destroy(gameObject, 5.0f);
        }
    }

    private void UpdateTarget()
    {
        if (_target)
        {
            float sqrDistance = (_target.position - transform.position).sqrMagnitude;

            if (sqrDistance > _detectRadius * _detectRadius)
            {
                _target = null;
            }

            return;
        }

        FindTarget();
    }

    private void FindTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            _detectRadius,
            _detectResults,
            _targetLayer
        );

        Transform nearestTarget = null;
        float nearestSqrDistance = float.MaxValue;

        for (int i = 0; i < count; ++i)
        {
            Collider candidate = _detectResults[i];

            if (!candidate || !candidate.CompareTag(_targetTag))
            {
                continue;
            }

            Vector3 offset = candidate.transform.position - transform.position;
            float sqrDistance = offset.sqrMagnitude;

            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearestTarget = candidate.transform;
            }
        }

        _target = nearestTarget;
    }

    private void ChaseTarget()
    {
        Vector3 direction = _target.position - transform.position;
        direction.y = 0.0f;

        if (direction.sqrMagnitude <= _targetReachDistance * _targetReachDistance)
        {
            _shouldAttack = true;
            // 가까워지면 공격
            if (_animator != null && _attackTriggerHash != 0)
            {
                _animator.SetBool(_attackTriggerHash, false);
            }

            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }

            _attackCoroutine = StartCoroutine(AttackCoroutine());
        }
        else
        {
            if (_animator != null && _walkBoolHash != 0)
            {
                _animator.SetBool(_walkBoolHash, true);
            }

            Move(direction.normalized);
        }
    }

    private void Wander()
    {
        _stateTimer -= Time.deltaTime;

        if (_isWaiting)
        {
            if (_animator && _walkBoolHash != 0)
            {
                _animator.SetBool(_walkBoolHash, false);
            }

            if (_stateTimer <= 0.0f)
            {
                ChooseRandomDirection();
                _stateTimer = _moveDuration;
                _isWaiting = false;
            }
        }
        else
        {
            if (_animator != null && _walkBoolHash != 0)
            {
                _animator.SetBool(_walkBoolHash, true);
            }

            Move(_moveDirection);

            if (_stateTimer <= 0.0f)
            {
                _stateTimer = _waitDuration;
                _isWaiting = true;
            }
        }
    }

    private void ChooseRandomDirection()
    {
        Vector2 random = Random.insideUnitCircle.normalized;
        _moveDirection = new Vector3(random.x, 0.0f, random.y);
    }

    private void Move(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        transform.position += direction * (_moveSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            _rotateSpeed * Time.deltaTime
        );
    }

    private IEnumerator AttackCoroutine()
    {
        if (_animator != null && !string.IsNullOrWhiteSpace(_attackTrigger))
        {
            yield return new WaitWhile(() =>
            {
                return _animator.GetCurrentAnimatorStateInfo(0).IsName("") &&
                       _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
            });

            if (_attackTriggerHash != 0)
            {
                _animator.ResetTrigger(_attackTrigger);
            }
        }

        _shouldAttack = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }
#endif
}