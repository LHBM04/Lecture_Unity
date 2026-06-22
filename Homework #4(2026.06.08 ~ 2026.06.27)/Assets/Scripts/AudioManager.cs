using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Object already = FindFirstObjectByType(typeof(AudioManager));
            DestroyImmediate(already);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip music)
    {

    }

    public void PlaySound(AudioClip music, Vector3 position)
    {

    }
}
