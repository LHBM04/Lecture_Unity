using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Cartridge : MonoBehaviour
{
    [SerializeField]
    private float _lifeTime = 5.0f;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }
}
