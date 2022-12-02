using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameData", order = 1)]
public class GameData : ScriptableObject
{
    public float PlayerSpeedInSec = 1f;

    public Vector2Int[] PlayerStartPositions;

    public PlayerControlData[] ControlData;

    [Serializable]
    public class PlayerControlData
    {
        public KeyCode TurnLeft = KeyCode.A;
        public KeyCode TurnRight = KeyCode.D;
    }
}

