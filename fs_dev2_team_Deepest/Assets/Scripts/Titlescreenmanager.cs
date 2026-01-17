using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] string gameplaySceneName = "GameScene";

    public static bool loadFromSave = false;

    public void OnNewGamePressed()
    {
        loadFromSave = false;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnLoadGamePressed()
    {
        loadFromSave = true;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnQuitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the game in a build
        Application.Quit();
#endif
        Debug.Log("Quit requested");
    }
}
