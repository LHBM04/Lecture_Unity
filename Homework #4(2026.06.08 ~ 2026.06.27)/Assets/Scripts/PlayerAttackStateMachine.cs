using UnityEngine;

public class PlayerAttackStateMachine : StateMachineBehaviour
{
    private PlayerController _controller;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _controller = _controller ?? animator.GetComponentInParent<PlayerController>();

        if (_controller != null)
        {
            _controller.IsAttacking = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _controller = _controller ?? animator.GetComponentInParent<PlayerController>();

        if (_controller != null)
        {
            _controller.IsAttacking = false;
        }
    }
}
