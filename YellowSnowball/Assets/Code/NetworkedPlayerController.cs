using Photon.Pun;
using UnityEngine;

public class NetworkedPlayerController : MonoBehaviour
{
    // Player data
    public int PlayerID;
    public bool CanMove = true;
    private Driveway m_driveway;
    private bool m_hasBeenClaimed = false;
    public PhotonView PhotonView;
    public Vector2Int CellPosition;

    private GameData m_gameData;
    private float MoveSpeed;
    private float m_lastRemainingSnow;
    private SnowTerrain m_snowTerrain;

    public Vector2Int Direction => new Vector2Int(Mathf.CeilToInt(transform.forward.normalized.x), Mathf.CeilToInt(transform.forward.normalized.z));

    public bool PlayerCanMove(Vector2Int toPosition)
    {
        return toPosition.x >= 0 && toPosition.x <= m_driveway.DimensionX - 1 && toPosition.y >= 0 && toPosition.y <= m_driveway.DimensionZ - 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        var worldManager = NetworkedGameManager.Instance.WorldManager;
        m_snowTerrain = worldManager.LocalPlayerTerrain;
        m_gameData = NetworkedGameManager.Instance.GameData;
        CellPosition = m_gameData.PlayerStartPositions[PlayerID];
        m_driveway = NetworkedGameManager.Instance.WorldManager?.GetPlayerDriveway(PlayerID);
        MoveSpeed = m_gameData.PlayerSpeedNormal;
        m_lastRemainingSnow = m_snowTerrain.RemainingSnow;

        EventManager.AddListener<OnGameStart>((e) =>
        {
            CanMove = PhotonView.IsMine;
        });
        EventManager.AddListener<OnRemotePlayerJoined>((e) =>
        {
            if (PlayerID == 0)
            {
                RPCManager.Instance.StartGame();
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        HandleControlInput();
    }

    public void SetOwnership(Photon.Realtime.Player player)
    {
        PhotonView.TransferOwnership(player);
        m_hasBeenClaimed = true;
    }

    private void HandleControlInput()
    {
        if (!CanMove || !PhotonView.IsMine || !m_hasBeenClaimed)
        {
            return;
        }

        // Set player inputs
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        var direction = new Vector3(horizontalInput, 0, verticalInput);

        // Determine speed
        Debug.Log(m_lastRemainingSnow - m_snowTerrain.RemainingSnow);
        if (m_lastRemainingSnow - m_snowTerrain.RemainingSnow > 50f)
            MoveSpeed = m_gameData.PlayerSpeedReduced;
        else
            MoveSpeed = m_gameData.PlayerSpeedNormal;
        m_lastRemainingSnow = m_snowTerrain.RemainingSnow;

        // Position
        transform.position += (direction * MoveSpeed * Time.deltaTime);

        // Rotation 
        if (direction.magnitude > 0.2f)
            transform.forward = direction.normalized;
    }
}
