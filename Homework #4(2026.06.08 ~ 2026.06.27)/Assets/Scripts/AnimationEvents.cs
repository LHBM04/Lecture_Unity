using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void PlayMusic(AudioClip music)
    {
        AudioManager.Instance.PlayMusic(music);
    }

    public void PlaySound(AudioClip sound)
    {
        AudioManager.Instance.PlaySound(sound, transform.position);
    }
}
