using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField]
    private PlayerController _controller;

    public Vector2 direction
    {
        get;
        private set;
    }

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        inputActions.Disable();


    }

    private void OnMoveStarted(InputAction.CallbackContext context)
    {

    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        
    }
}
