using UnityEngine;

public class PlayerDeadStateMachine : StateMachineBehaviour
{
    private PlayerController _controller;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _controller = _controller ?? animator.GetComponentInParent<PlayerController>();

        if (_controller != null)
        {
            _controller.IsDead = true;
        }
    }
}
