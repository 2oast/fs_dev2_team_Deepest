using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [SerializeField] AudioClip clickSound;
    [SerializeField] float volume = 1f;

    static AudioSource audioSource;

    void Awake()
    {
        if (audioSource == null)
        {
            GameObject obj = new GameObject("UI_ButtonAudio");
            audioSource = obj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            DontDestroyOnLoad(obj);
        }

        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound, volume);
    }
}
