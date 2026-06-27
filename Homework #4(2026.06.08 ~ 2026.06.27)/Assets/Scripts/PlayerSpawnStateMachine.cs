using UnityEngine;

public class PlayerSpawnStateMachine : StateMachineBehaviour
{
    private PlayerController _controller;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _controller = _controller ?? animator.GetComponentInParent<PlayerController>();
        _controller?.Ready();
    }
}
