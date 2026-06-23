using UnityEngine;

public class PlayerIdleStateMachine : StateMachineBehaviour
{
    private int _nextIdleIndex;

    [SerializeField]
    private int _minIdleIndex;

    [SerializeField]
    private int _maxIdleIndex;

    [Space, SerializeField]
    private string _idleInt;
    private int _idleIntHash;

    private void Reset()
    {
        _minIdleIndex = 1;
        _maxIdleIndex = 3;
    }

    private void Awake()
    {
        _idleIntHash = !string.IsNullOrEmpty(_idleInt) ? Animator.StringToHash(_idleInt) : 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // TODO: 이거 다른 방법 알아보기.
        if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
        {
            animator.SetInteger(_idleIntHash, -1);
        }

        if (stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(layerIndex))
        {
            _nextIdleIndex = Random.Range(_minIdleIndex, _maxIdleIndex);
            animator.SetInteger(_idleIntHash, Random.Range(_minIdleIndex, _maxIdleIndex + 1));

            // Debug.Log($"다음 Idle 애니메이션: {_nextIdleIndex}");
        }
    }
}
