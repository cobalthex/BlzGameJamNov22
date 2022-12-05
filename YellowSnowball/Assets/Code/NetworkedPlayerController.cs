using Photon.Pun;
using UnityEngine;

public class NetworkedPlayerController : MonoBehaviour
{
    // Player data
    public int PlayerID;
    public bool CanMove = true;
    public float MoveSpeed;
    private Driveway m_driveway;
    public PhotonView PhotonView;
    public Vector2Int CellPosition;
    public Vector2Int Direction => new Vector2Int(Mathf.CeilToInt(transform.forward.normalized.x), Mathf.CeilToInt(transform.forward.normalized.z));

    public bool PlayerCanMove(Vector2Int toPosition)
    {
        return toPosition.x >= 0 && toPosition.x <= m_driveway.DimensionX - 1 && toPosition.y >= 0 && toPosition.y <= m_driveway.DimensionZ - 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameData data = NetworkedGameManager.Instance.GameData;
        CellPosition = data.PlayerStartPositions[PlayerID];
        m_driveway = NetworkedGameManager.Instance.WorldManager?.GetPlayerDriveway(PlayerID);

        // Move player to start
        transform.position = m_driveway.GetPositionOfCell(CellPosition);
        m_driveway = NetworkedGameManager.Instance.WorldManager?.GetPlayerDriveway(PlayerID);

        // Move player to start
        transform.position = m_driveway.GetPositionOfCell(CellPosition);
        CanMove = PhotonView.IsMine;
    }

    // Update is called once per frame
    void Update()
    {
        HandleControlInput();
    }

    public void SetOwnership(int playerId)
    {
        // TODO: Figure out ownership.
        PhotonView.TransferOwnership(playerId);
    }

    private void HandleControlInput()
    {
        if (!CanMove)
        {
            return;
        }

        // Set player inputs
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(new Vector3(horizontalInput, 0, verticalInput) * MoveSpeed * Time.deltaTime);
    }
}
