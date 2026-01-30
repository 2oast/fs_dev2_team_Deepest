using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] string gameplaySceneName = "GameScene";

    [Header("Music")]
    [SerializeField] AudioSource musicSource;

    [Header("Load Button")]
    [SerializeField] Button loadGameButton;
    [SerializeField] Image loadGameButtonImage;
    [SerializeField] float disabledAlpha = 0.4f;

    public static bool loadFromSave = false;

    string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    void Start()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.loop = true;
            musicSource.Play();
        }

        UpdateLoadButtonState();
    }

    void UpdateLoadButtonState()
    {
        bool hasSave = File.Exists(SavePath);

        if (loadGameButton != null)
            loadGameButton.interactable = hasSave;

        if (loadGameButtonImage != null)
        {
            Color c = loadGameButtonImage.color;
            c.a = hasSave ? 1f : disabledAlpha;
            loadGameButtonImage.color = c;
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
        StopMusic();
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnLoadGamePressed()
    {
        if (!File.Exists(SavePath))
            return;

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
