using System.ComponentModel;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    public ItemData currentItemEquipped;

    private void Awake()
    {
        instance = this;
    }

   
}
