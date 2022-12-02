using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player data
    public int PlayerID;
    private float m_playerSpeedInSec;
    private Driveway m_driveway;

    public Vector2Int CellPosition;

    public Vector2Int Direction => new Vector2Int(Mathf.CeilToInt(transform.forward.normalized.x), Mathf.CeilToInt(transform.forward.normalized.z));

    // Controls
    private KeyCode m_turnRightKey;
    private KeyCode m_turnLeftKey;

    // Movement
    private Coroutine m_moveRoutine;

    public bool PlayerCanMove(Vector2Int toPosition)
    {
        return toPosition.x >= 0 && toPosition.x <= m_driveway.DimensionX - 1 && toPosition.y >= 0 && toPosition.y <= m_driveway.DimensionZ - 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameData data = GameManager.Instance.GameData;
        m_turnLeftKey = data.ControlData[PlayerID].TurnLeft;
        m_turnRightKey = data.ControlData[PlayerID].TurnRight;

        m_playerSpeedInSec = data.PlayerSpeedInSec;
        m_driveway = GameManager.Instance.WorldManager?.GetPlayerDriveway(PlayerID);

        // Move player to start
        transform.position = m_driveway.GetPositionOfCell(CellPosition);

        ResetMoveRoutine();
    }

    public void ResetMoveRoutine()
    {
        if (m_moveRoutine != null)
            StopCoroutine(m_moveRoutine);
        m_moveRoutine = StartCoroutine(MoveForwardRoutine());
    }

    private IEnumerator MoveForwardRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_playerSpeedInSec);

            // If next cell is movable
            var forwardCellPosition = CellPosition + Direction;
            if (PlayerCanMove(forwardCellPosition))
            {
                // Move 
                transform.position = m_driveway.GetPositionOfCell(forwardCellPosition);
                CellPosition = forwardCellPosition;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(m_turnLeftKey))
        {
            transform.Rotate(new Vector3(0f, -90f, 0f));
            ResetMoveRoutine();
        }
        else if (Input.GetKeyDown(m_turnRightKey))
        {
            transform.Rotate(new Vector3(0f, 90f, 0f));
            ResetMoveRoutine();
        }
    }
}
