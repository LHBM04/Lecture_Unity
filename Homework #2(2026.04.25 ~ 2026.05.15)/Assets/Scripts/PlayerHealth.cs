using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    /// <summary>
    /// «ÿ¥Á Player¿« Controller.
    /// </summary>
    [HideInInspector]
    public PlayerController controller;

    private void Reset()
    {
        controller = GetComponentInParent<PlayerController>();
    }

    private void Awake()
    {
        controller = controller ?? GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (other.CompareTag("Enemy"))
        // {
        //     controller.animator.SetTrigger("Hit");
        // }
    }
}