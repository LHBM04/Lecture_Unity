using UnityEngine;

public class PlayerRollStateMachine : StateMachineBehaviour
{
    private PlayerController _controller;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        _controller = _controller ?? animator.GetComponentInParent<PlayerController>();

        if (_controller != null)
        {
            _controller.IsRolling = true;
        }
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        _controller = _controller ?? animator.GetComponentInParent<PlayerController>();

        if (_controller != null)
        {
            _controller.IsRolling = false;
        }
    }
}
