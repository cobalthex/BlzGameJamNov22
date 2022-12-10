using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

public class NetworkedPlayerController : MonoBehaviour
{
    // Player data
    public int PlayerID;
    public bool CanMove = true;
    public float MoveSpeed;
    private Driveway m_driveway;
    private bool m_hasBeenClaimed = false;
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
        CanMove = PhotonView.IsMine;
    }

    // Update is called once per frame
    void Update()
    {
        HandleControlInput();
        HandleLaunchSnowInput();
    }

    public void SetOwnership(Photon.Realtime.Player player)
    {
        PhotonView.TransferOwnership(player);
        m_hasBeenClaimed = true;
    }

    public void SendSnowToEnemy(int snowInMeters)
    {
        RPCManager.Instance.SendSnowToEnemy(snowInMeters);
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
        transform.position += (direction * MoveSpeed * Time.deltaTime);
        transform.forward = direction.normalized;
    }

    private void HandleLaunchSnowInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // TODO: Placeholder
            SendSnowToEnemy(100);
        }
    }
}
