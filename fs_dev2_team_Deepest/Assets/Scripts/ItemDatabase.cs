using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> allItems = new List<ItemData>();

    public ItemData GetItemByName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        for (int i = 0; i < allItems.Count; i++)
        {
            if (allItems[i] != null && allItems[i].itemName == name)
                return allItems[i];
        }

        Debug.LogWarning("ItemDatabase: Could not find item with name " + name);
        return null;
    }
}