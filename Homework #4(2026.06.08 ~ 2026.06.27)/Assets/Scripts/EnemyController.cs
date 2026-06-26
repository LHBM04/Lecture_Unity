using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    private string _targetTag;

    [SerializeField]
    private LayerMask _targetLayer;

    [SerializeField]
    private float _detectRadius;

    private Collider[] _detectResults;
    private Transform _target;
    private Rigidbody _rigidbody;

    [Header("Movement")]
    [SerializeField]
    private float _walkSpeed;

    [SerializeField]
    private float _runSpeed;

    [SerializeField]
    private float _rotateSpeed;

    [SerializeField]
    private float _targetReachDistance;

    [SerializeField]
    private float _attackCooldown;

    [SerializeField]
    private float _attackDuration;

    [SerializeField]
    private float _maxMoveDeltaTime;

    [SerializeField]
    private float _maxMoveDistancePerFrame;

    private float _moveSpeedParameter;

    [Header("Wander")]
    [SerializeField]
    private float _moveDuration;

    [SerializeField]
    private float _waitDuration;

    private Vector3 _moveDirection;
    private Vector3 _desiredMoveDirection;
    private float _desiredMoveSpeed;
    private bool _shouldMove;
    private float _stateTimer;
    private bool _isWaiting;
    private bool _isDead;
    private bool _shouldAttack;
    private bool _isAttacking;
    private float _lastAttackTime;
    private float _attackEndTime;

    [Header("Animation")]
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private string _movingBool;
    private int _movingBoolHash;

    [SerializeField]
    private string _moveSpeedFloat;
    private int _moveSpeedFloatHash;

    [SerializeField]
    private string _attackTrigger;
    private int _attackTriggerHash;

    [SerializeField]
    private string _deadTrigger;
    private int _deadTriggerHash;

    private void Reset()
    {
        _targetTag = "Player";
        _detectRadius = 5.0f;

        _walkSpeed = 3.0f;
        _runSpeed = 6.0f;
        _rotateSpeed = 720.0f;
        _targetReachDistance = 0.2f;
        _attackCooldown = 1.0f;
        _attackDuration = 0.9f;
        _maxMoveDeltaTime = 0.05f;
        _maxMoveDistancePerFrame = 0.5f;

        _moveDuration = 2.0f;
        _waitDuration = 0.5f;

        _animator = GetComponentInChildren<Animator>();
        _movingBool = "Is Moving";
        _moveSpeedFloat = "Move Speed";
    }

    private void Awake()
    {
        _detectResults = new Collider[16];
        _rigidbody = GetComponent<Rigidbody>();
        _animator = _animator ?? GetComponentInChildren<Animator>();

        ApplyDefaultValues();
        ConfigureRigidbody();
        ConfigureAnimator();

        _movingBoolHash = GetAnimatorParameterHash(_movingBool);
        _moveSpeedFloatHash = GetAnimatorParameterHash(_moveSpeedFloat);
        _attackTriggerHash = GetAnimatorParameterHash(_attackTrigger);
        _deadTriggerHash = GetAnimatorParameterHash(_deadTrigger);

        _stateTimer = _moveDuration;
        _desiredMoveDirection = Vector3.zero;
        _desiredMoveSpeed = 0.0f;
        _shouldMove = false;
        _isWaiting = false;
        _isDead = false;
        _shouldAttack = false;
        _isAttacking = false;
        _lastAttackTime = -_attackCooldown;
        _attackEndTime = 0.0f;
        _moveSpeedParameter = 0.0f;
    }

    private void ApplyDefaultValues()
    {
        _targetTag = !string.IsNullOrEmpty(_targetTag) ? _targetTag : "Player";
        _detectRadius = _detectRadius > 0.0f ? _detectRadius : 5.0f;

        _walkSpeed = _walkSpeed > 0.0f ? _walkSpeed : 3.0f;
        _runSpeed = _runSpeed > 0.0f ? _runSpeed : 6.0f;
        _rotateSpeed = _rotateSpeed > 0.0f ? _rotateSpeed : 720.0f;
        _targetReachDistance = _targetReachDistance > 0.0f ? _targetReachDistance : 0.2f;
        _attackCooldown = _attackCooldown > 0.0f ? _attackCooldown : 1.0f;
        _attackDuration = _attackDuration > 0.0f ? _attackDuration : 0.9f;
        _maxMoveDeltaTime = _maxMoveDeltaTime > 0.0f ? _maxMoveDeltaTime : 0.05f;
        _maxMoveDistancePerFrame = _maxMoveDistancePerFrame > 0.0f ? _maxMoveDistancePerFrame : 0.5f;

        _moveDuration = _moveDuration > 0.0f ? _moveDuration : 2.0f;
        _waitDuration = _waitDuration > 0.0f ? _waitDuration : 0.5f;

        _movingBool = !string.IsNullOrEmpty(_movingBool) ? _movingBool : "Is Moving";
        _moveSpeedFloat = !string.IsNullOrEmpty(_moveSpeedFloat) ? _moveSpeedFloat : "Move Speed";
    }

    private void ConfigureRigidbody()
    {
        if (_rigidbody != null)
        {
            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    private void ConfigureAnimator()
    {
        if (_animator != null)
        {
            _animator.applyRootMotion = false;
        }
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

        if (_isAttacking && Time.time >= _attackEndTime)
        {
            AttackEnd();
        }

        UpdateTarget();

        if (_target != null)
        {
            if (_isAttacking)
            {
                FaceTarget();
                SetMovementAnimation(false, 0.0f);
                StopMoving();
                return;
            }

            ChaseTarget();
        }
        else
        {
            Wander();
        }
    }

    private void FixedUpdate()
    {
        if (_isDead || !_shouldMove)
        {
            return;
        }

        Move(_desiredMoveDirection, _desiredMoveSpeed, Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isDead)
        {
            return;
        }

        if (collision.collider.CompareTag("Weapon"))
        {
            _isDead = true;
            SetMovementAnimation(false, 0.0f);

            if (_animator != null && _deadTriggerHash != 0)
            {
                _animator.SetTrigger(_deadTriggerHash);
            }

            Destroy(gameObject, 5.0f);
        }
    }

    private void UpdateTarget()
    {
        if (_target != null)
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

            if (candidate == null || !candidate.CompareTag(_targetTag))
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
            SetMovementAnimation(false, 0.0f);
            FaceDirection(direction);
            StopMoving();
            TryAttack();
            return;
        }

        _shouldAttack = false;
        SetMovementAnimation(true, 1.0f);
        StartMoving(direction.normalized, _runSpeed);
    }

    private void Wander()
    {
        _stateTimer -= Time.deltaTime;

        if (_isWaiting)
        {
            SetMovementAnimation(false, 0.0f);
            StopMoving();

            if (_stateTimer <= 0.0f)
            {
                ChooseRandomDirection();
                _stateTimer = _moveDuration;
                _isWaiting = false;
            }

            return;
        }

        SetMovementAnimation(true, 0.5f);
        StartMoving(_moveDirection, _walkSpeed);

        if (_stateTimer <= 0.0f)
        {
            _stateTimer = _waitDuration;
            _isWaiting = true;
        }
    }

    private void ChooseRandomDirection()
    {
        Vector2 random = Random.insideUnitCircle;

        if (random.sqrMagnitude <= 0.0001f)
        {
            random = Vector2.up;
        }

        random.Normalize();
        _moveDirection = new Vector3(random.x, 0.0f, random.y);
    }

    private void StartMoving(Vector3 direction, float moveSpeed)
    {
        if (direction.sqrMagnitude <= 0.0001f)
        {
            StopMoving();
            return;
        }

        _desiredMoveDirection = direction.normalized;
        _desiredMoveSpeed = moveSpeed;
        _shouldMove = true;
    }

    private void StopMoving()
    {
        _desiredMoveDirection = Vector3.zero;
        _desiredMoveSpeed = 0.0f;
        _shouldMove = false;
    }

    private void Move(Vector3 direction, float moveSpeed, float deltaTime)
    {
        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        deltaTime = Mathf.Min(deltaTime, _maxMoveDeltaTime);
        Vector3 moveDelta = direction * (moveSpeed * deltaTime);

        if (moveDelta.magnitude > _maxMoveDistancePerFrame)
        {
            moveDelta = moveDelta.normalized * _maxMoveDistancePerFrame;
        }

        Vector3 nextPosition = transform.position + moveDelta;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion nextRotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            _rotateSpeed * deltaTime
        );

        if (_rigidbody != null)
        {
            _rigidbody.MovePosition(nextPosition);
            _rigidbody.MoveRotation(nextRotation);
            return;
        }

        transform.position = nextPosition;
        transform.rotation = nextRotation;
    }

    private void FaceTarget()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 direction = _target.position - transform.position;
        direction.y = 0.0f;
        FaceDirection(direction);
    }

    private void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        float deltaTime = Mathf.Min(Time.deltaTime, _maxMoveDeltaTime);
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            _rotateSpeed * deltaTime
        );
    }

    private void SetMovementAnimation(bool isMoving, float targetMoveSpeed)
    {
        if (_animator == null)
        {
            return;
        }

        if (_movingBoolHash != 0)
        {
            _animator.SetBool(_movingBoolHash, isMoving);
        }

        _moveSpeedParameter = Mathf.MoveTowards(_moveSpeedParameter, targetMoveSpeed, 5.0f * Time.deltaTime);

        if (_moveSpeedFloatHash != 0)
        {
            _animator.SetFloat(_moveSpeedFloatHash, _moveSpeedParameter);
        }
    }

    private void TryAttack()
    {
        if (!_shouldAttack || _isAttacking || Time.time < _lastAttackTime + _attackCooldown)
        {
            return;
        }

        _isAttacking = true;
        _lastAttackTime = Time.time;
        _attackEndTime = Time.time + _attackDuration;

        if (_animator == null || _attackTriggerHash == 0)
        {
            _shouldAttack = false;
            return;
        }

        _animator.SetTrigger(_attackTriggerHash);
        _shouldAttack = false;
    }

    public void AttackBegin()
    {
        _isAttacking = true;
        SetMovementAnimation(false, 0.0f);
    }

    public void AttackEnd()
    {
        _isAttacking = false;
        _shouldAttack = false;
    }

    private int GetAnimatorParameterHash(string parameterName)
    {
        if (_animator == null || string.IsNullOrEmpty(parameterName))
        {
            return 0;
        }

        foreach (AnimatorControllerParameter parameter in _animator.parameters)
        {
            if (parameter.name == parameterName)
            {
                return Animator.StringToHash(parameterName);
            }
        }

        return 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }
#endif
}
