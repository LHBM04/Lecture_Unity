using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _gravity;

    [SerializeField]
    private Vector3 _gravityDirection;

    private void Reset()
    {
        _gravity = 9.81f;
        _gravityDirection = Vector3.down;
    }

    private void FixedUpdate()
    {
        Vector3 gravityForce = _gravityDirection.normalized * (_gravity * Time.fixedDeltaTime);
    }
}
