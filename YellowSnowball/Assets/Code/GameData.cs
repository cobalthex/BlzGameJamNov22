using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameData", order = 1)]
public class GameData : ScriptableObject
{
    public float PlayerSpeedInSec = 1f;

    public Vector2Int[] PlayerStartPositions;

    public PlayerControlData[] ControlData;

    public int PlayerStartMoney;
    public ItemData[] ShopItems;

    [Serializable]
    public class PlayerControlData
    {
        public KeyCode TurnLeft = KeyCode.A;
        public KeyCode TurnRight = KeyCode.D;
    }

    [Serializable]
    public class ItemData
    {
        public ShopItemType ItemType;
        public int Cost;
        public Sprite ItemIcon;
    }
}

