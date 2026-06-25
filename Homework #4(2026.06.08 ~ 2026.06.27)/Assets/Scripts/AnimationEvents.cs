using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField]
    private PlayerController _controller;

    private void Reset()
    {
        _controller = GetComponentInParent<PlayerController>();
    }

    private void Awake()
    {
        _controller = _controller ?? GetComponentInParent<PlayerController>();
    }

    public void Ready()
    {
        GetComponentInParent<PlayerController>().Ready();
    }

    public void PlaySound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
