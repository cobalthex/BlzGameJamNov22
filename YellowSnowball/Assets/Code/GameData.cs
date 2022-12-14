using System;
using UnityEngine;

public enum ShopItemType
{
    Nitro,
    Salt
}

[Serializable]
public class ShopItemData
{
    public ShopItemType ItemType;
    public int Cost;
    public int MaxCount;
    public Sprite ItemIcon;
}

[CreateAssetMenu(fileName = "GameData", menuName = "GameData", order = 1)]
public class GameData : ScriptableObject
{
    public float PlayerSpeedNormal = 5f;
    public float PlayerSpeedReduced = 2f;

    public Vector2Int[] PlayerStartPositions;

    public ShopItemData[] ShopItems;

    public int PlayerStartMoney = 1000;

    public PlayerKeyboardControls Keys;

    [Serializable]
    public class PlayerKeyboardControls
    {
        // Player Keyboard Controls
        public KeyCode PreviousShopItem = KeyCode.Q;
        public KeyCode NextShopItem = KeyCode.E;
        public KeyCode BuyShopItem = KeyCode.LeftControl;
        public KeyCode AccessShop = KeyCode.LeftShift;
        public KeyCode ToggleBlowerDoor = KeyCode.Space;
        public KeyCode Item1Key = KeyCode.Alpha1;
        public KeyCode Item2Key = KeyCode.Alpha2;
        public KeyCode Item3Key = KeyCode.Alpha3;
    }
}

