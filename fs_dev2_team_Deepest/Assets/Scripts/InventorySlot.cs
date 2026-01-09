using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, ISubmitHandler, IPointerClickHandler, ISelectHandler
{
    public ItemData itemInSlot;
    public Image itemImageComp;
    public Sprite itemSprite;

    public bool isFilled;

    private void Awake()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        itemInSlot.Use(GameManager.instance.playerControllerScript);
    }

    public void OnSelect(BaseEventData eventData)
    {

    }

    public void OnSubmit(BaseEventData eventData)
    {
        UseItem();
    }
}