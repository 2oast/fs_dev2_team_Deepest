using UnityEngine;

public class GameSceneLoader : MonoBehaviour
{
    void Start()
    {
        if (TitleScreenManager.loadFromSave)
        {
            TitleScreenManager.loadFromSave = false;

            if (SaveManager.instance != null)
            {
                SaveManager.instance.LoadGame();
            }
            else
            {
                Debug.LogWarning("SaveManager.instance is null, cannot load game.");
            }
        }
        else
        {

        }
    }
}
