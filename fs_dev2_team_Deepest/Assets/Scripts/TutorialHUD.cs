using UnityEngine;
using TMPro;

public class TutorialHUD : MonoBehaviour
{
    public static TutorialHUD instance;

    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] bool hideOnStart = true;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (hideOnStart)
            Hide();
    }

    public void Show(string message)
    {
        if (tutorialText == null) return;

        tutorialText.text = message;
        tutorialText.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (tutorialText == null) return;

        tutorialText.text = "";
        tutorialText.gameObject.SetActive(false);
    }

    public bool HasTextReference()
    {
        return tutorialText != null;
    }
}