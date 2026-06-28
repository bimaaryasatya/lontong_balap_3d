using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}