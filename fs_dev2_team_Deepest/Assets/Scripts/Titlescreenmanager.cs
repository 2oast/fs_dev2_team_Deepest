using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] string gameplaySceneName = "GameScene";

    [Header("Music")]
    [SerializeField] AudioSource musicSource;

    public static bool loadFromSave = false;

    void Start()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void OnNewGamePressed()
    {
        loadFromSave = false;

        SaveManager.DeleteSaveFile();

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnLoadGamePressed()
    {
        loadFromSave = true;
        StopMusic();
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnQuitPressed()
    {
        StopMusic();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Quit requested");
    }
}