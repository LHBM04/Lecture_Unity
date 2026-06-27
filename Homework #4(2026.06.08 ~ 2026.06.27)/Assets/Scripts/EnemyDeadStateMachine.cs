using UnityEngine;

public class EnemyDeadStateMachine : StateMachineBehaviour
{
    private EnemyController _controller;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _controller = _controller ?? animator.GetComponentInParent<EnemyController>();

        if (_controller != null)
        {
            _controller.IsDead = true;
        }
    }
}
