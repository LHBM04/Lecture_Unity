using System.Net.NetworkInformation;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Animator animator;
    public Animator Animator
    {
        get { return animator; }
    }

    [SerializeField]
    private string groundedBool;
    private int groundedBoolHash;

    [SerializeField]
    private float groundCheckDistance;

    public Vector3 Velocity 
    { 
        get; 
        set; 
    }

    public bool IsGrounded
    {
        get
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            return Physics.Raycast(origin, Vector3.down, groundCheckDistance);
        }
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Velocity = Vector3.zero;
    }

    private void Awake()
    {
        rb = rb ?? GetComponent<Rigidbody>();
        animator = animator ?? GetComponent<Animator>();

        groundedBoolHash = !string.IsNullOrWhiteSpace(groundedBool) ? Animator.StringToHash(groundedBool) : 0;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Velocity = Vector3.zero;
    }

    private void Update()
    {
        if (groundedBoolHash != 0)
        {
            animator.SetBool(groundedBoolHash, IsGrounded);
        }
    }

    private void FixedUpdate()
    {
        HandleVelocity();
    }

    private void HandleVelocity()
    {
        if (rb == null)
        {
            return;
        }

        Vector3 worldHorizontal = transform.TransformDirection(new Vector3(Velocity.x, 0f, Velocity.z));
        Vector3 current = rb.linearVelocity;
        rb.linearVelocity = new Vector3(worldHorizontal.x, current.y, worldHorizontal.z);

        if (Velocity.y > 0f)
        {
            rb.AddForce(Vector3.up * Velocity.y, ForceMode.VelocityChange);
        }
    }
}
