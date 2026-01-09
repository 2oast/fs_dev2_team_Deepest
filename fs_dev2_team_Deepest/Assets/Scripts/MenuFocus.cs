using UnityEngine;
using UnityEngine.EventSystems;

public class MenuFocus : MonoBehaviour
{
    [SerializeField] GameObject firstButton;

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
    }
}
