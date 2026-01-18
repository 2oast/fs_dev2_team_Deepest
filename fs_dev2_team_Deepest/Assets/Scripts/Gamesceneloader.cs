using System.Collections;
using UnityEngine;

public class GameSceneLoader : MonoBehaviour
{
    IEnumerator Start()
    {
        if (!TitleScreenManager.loadFromSave)
            yield break;

        TitleScreenManager.loadFromSave = false;

        while (SaveManager.instance == null ||
               GameManager.instance == null ||
               GameManager.instance.playerControllerScript == null)
        {
            yield return null;
        }

        SaveManager.instance.LoadGame();

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetAfterLoad();
        }
    }
}

