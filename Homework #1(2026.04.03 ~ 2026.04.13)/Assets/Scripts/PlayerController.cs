using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 해당 플레이어의 Rigidbody.
    /// </summary>
    [Tooltip("해당 플레이어의 Rigidbody.")]
    [SerializeField]
    private Rigidbody rb;

    /// <summary>
    /// 해당 플레이어의 이동 속도.
    /// </summary>
    [Tooltip("해당 플레이어의 이동 속도.")]
    [SerializeField]
    private float moveSpeed;

    /// <summary>
    /// 회전 민감도.
    /// </summary>
    [Tooltip("회전 민감도.")]
    [SerializeField]
    private float sensitivity;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        moveSpeed = 15.0f;
        sensitivity = 2f;
    }

    private void Awake()
    {
        rb = rb ?? GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
        Jump();
        Rotate();
    }

    /// <summary>
    /// 플레이어의 이동을 구현합니다.
    /// </summary>
    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float totalSpeed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * 1.5f : moveSpeed;

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * Time.deltaTime * totalSpeed;
        transform.Translate(movement, Space.Self);
    }

    /// <summary>
    /// 플레이어의 점프를 구현합니다.
    /// </summary>
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!rb)
            {
                Debug.LogWarning("PlayerController: Rigidbody를 찾을 수 없습니다!");
                return;
            }

            if (Physics.Raycast(transform.position, Vector3.down, 1.1f))
            {
                rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }
        }
    }

    /// <summary>
    /// 플레이어의 회전을 구현합니다.
    /// </summary>
    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up, mouseX * sensitivity);
    }
}
