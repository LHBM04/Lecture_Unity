using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void PlaySound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
