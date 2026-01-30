using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class LavaMover : MonoBehaviour
{
    [Header("Lava Movement")]
    [SerializeField] Vector2 scrollSpeed = new Vector2(0.05f, 0.02f);

    [Header("Audio")]
    [SerializeField] AudioClip lavaLoop;
    [SerializeField] float volume = 0.7f;

    Renderer rend;
    Vector2 offset;
    AudioSource audioSource;
    bool wasPaused;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = lavaLoop;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    void Start()
    {
        if (lavaLoop != null)
            audioSource.Play();
    }

    void Update()
    {
        offset += scrollSpeed * Time.deltaTime;
        rend.material.mainTextureOffset = offset;

        bool paused = GameManager.instance != null && GameManager.instance.isPaused;

        if (paused && !wasPaused)
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
        else if (!paused && wasPaused)
        {
            if (lavaLoop != null && !audioSource.isPlaying)
                audioSource.UnPause();
        }

        wasPaused = paused;
    }
}

