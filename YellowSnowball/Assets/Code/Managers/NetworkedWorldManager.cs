using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;

public class NetworkedWorldManager : MonoBehaviour
{
    public Driveway[] Driveways;
    public SnowTerrain[] SnowTerrains;
    public NetworkedPlayerController[] Players;
    public SnowTerrain[] SnowTerrain;
    public ClientNetworking ClientNetworking;
    public int PlayerIndex;

    private bool m_initialized = false;

    public SnowTerrain LocalPlayerTerrain => SnowTerrain[PlayerIndex];

    // Start is called before the first frame update
    void Start()
    {
        NetworkedGameManager.Instance.WorldManager = this;
        StartCoroutine(EnablePlayerWhenReady());
    }

    public NetworkedPlayerController GetLocalPlayer()
    {
        return Players[PlayerIndex];
    }

    public Driveway GetPlayerDriveway(int playerId)
    {
        return Driveways[playerId];
    }

    public SnowTerrain GetPlayerSnowTerrain(int playerId)
    {
        return SnowTerrains[playerId];
    }

    public int GetPlayerByTerrain(SnowTerrain terrain)
    {
        if (SnowTerrains[0].GetInstanceID() == terrain.GetInstanceID())
        {
            return 0;
        }

        return 1;
    }

    public void EnablePlayer()
    {
        Debug.Log("Enabling Player");
        // Base ownership of player by order of their ids in the room
        var playerIds = PhotonNetwork.CurrentRoom.Players.Keys.ToList();
        playerIds.Sort();
        PlayerIndex = playerIds.IndexOf(PhotonNetwork.LocalPlayer.ActorNumber);
        Players[PlayerIndex].gameObject.SetActive(true);
        Players[PlayerIndex].SetOwnership(PhotonNetwork.LocalPlayer);
    }

    private IEnumerator EnablePlayerWhenReady()
    {
        while (!m_initialized)
        {
            if (ClientNetworking.IsConnectedToRoom())
            {
                EnablePlayer();
                m_initialized = true;
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
