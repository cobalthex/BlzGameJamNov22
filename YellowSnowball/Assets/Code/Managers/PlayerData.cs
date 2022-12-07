using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    // Player 
    public int Money;
    public Dictionary<ShopItemType, InventoryItem> Inventory = new Dictionary<ShopItemType, InventoryItem>();

    public bool CanAfford(int cost) => cost <= Money;

    [Serializable]
    public class InventoryItem
    {
        public ShopItemData Data;
        public int Count;
    }

    public void Init()
    {
        var gameData = GameManager.Instance.GameData;

        // Populate inventory slots
        foreach (var itemData in gameData.ShopItems)
        {
            var item = new InventoryItem();
            item.Data = itemData;
            Inventory.Add(itemData.ItemType, item);
        }

        Money = gameData.PlayerStartMoney;
    }

    public bool AddItem(ShopItemData shopItem)
    {
        if (shopItem.Cost > Money)
        {
            Debug.Log($"Can't afford item cost {shopItem.Cost} and Money {Money}");
            return false;
        }


        if (Inventory[shopItem.ItemType].Count >= shopItem.MaxCount)
        {
            Debug.Log($"Can't add item because at max {Inventory[shopItem.ItemType].Count}");
            return false;
        }

        Inventory[shopItem.ItemType].Count++;
        Money -= shopItem.Cost;
        Debug.Log($"Money left {Money}");
        return true;
    }
    
    public void RemoveItem(ShopItemType itemType)
    {
        if (Inventory[itemType].Count <= 0)
            return;

        Inventory[itemType].Count--;
        Debug.Log($"Item type {itemType} count {Inventory[itemType].Count}");
    }

}
