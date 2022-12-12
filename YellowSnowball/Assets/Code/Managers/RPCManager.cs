using Photon.Pun;
using System.Linq;
using UnityEngine;

public class RPCManager : SingletonBehaviour<RPCManager>
{
    public bool CanUpdateSnow;
    public Texture2D SnowDeformTexture;

    private PhotonView m_photonView;
    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
        NetworkedGameManager.Instance.RPCManager = this;
    }

    public void SendSnow(SnowTerrain terrain, Vector2 relativePosition, float xSize, float patternScale, float pressureMpa)
    {
        if (CanUpdateSnow)
        {
            var playerId = NetworkedGameManager.Instance.WorldManager.GetPlayerByTerrain(terrain);
            m_photonView.RPC("AddSnowToDriveway", RpcTarget.AllViaServer, new object[] { playerId, relativePosition, xSize, patternScale, pressureMpa });
        }
    }

    [PunRPC]
    void AddSnowToDriveway(int playerId, Vector2 relativePosition, float xSize, float patternScale, float pressureMpa, PhotonMessageInfo info)
    {
        var playerTerrain = NetworkedGameManager.Instance.WorldManager.GetPlayerSnowTerrain(playerId);
        playerTerrain.Deform(relativePosition, xSize, SnowDeformTexture, patternScale, pressureMpa);
    }
}