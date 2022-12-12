using Photon.Pun;
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

    #region Snow Sending
    public void SendSnow(SnowTerrain terrain, Vector2 relativePosition, float xSize, float patternScaleMeters)
    {
        if (CanUpdateSnow)
        {
            var playerId = NetworkedGameManager.Instance.WorldManager.GetPlayerByTerrain(terrain);
            m_photonView.RPC("AddSnowToDriveway", RpcTarget.AllViaServer, new object[] { playerId, relativePosition, xSize, patternScaleMeters });
        }
    }

    [PunRPC]
    void AddSnowToDriveway(int playerId, Vector2 relativePosition, float xSize, float patternScaleMeters, PhotonMessageInfo info)
    {
        var playerTerrain = NetworkedGameManager.Instance.WorldManager.GetPlayerSnowTerrain(playerId);
        playerTerrain.Deform(relativePosition, xSize, SnowDeformTexture, patternScaleMeters);
    }
    #endregion

    #region Game Management
    public void StartGame()
    {
        m_photonView.RPC("ReceiveGameStart", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void ReceiveGameStart(PhotonMessageInfo info)
    {
        EventManager.Fire<OnGameStart>(new OnGameStart());
    }

    public void EndGame()
    {
        m_photonView.RPC("ReceiveGameStart", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void ReceiveGameEnd(PhotonMessageInfo info)
    {
        EventManager.Fire<OnGameEnd>(new OnGameEnd());
    }
    #endregion

    #region Example RPC
    ///// <summary>
    ///// Method that triggers the RPC (called from a client)
    ///// </summary>
    //public void SomeMethodName(string parameterOne, int parameterTwo)
    //{
    //    m_photonView.RPC("RpcMethodName", RpcTarget.AllViaServer, new object[] { parameterOne, parameterTwo });
    //}

    ///// <summary>
    ///// RPC Receiver. The name here much match the string in the above call.
    ///// </summary>
    //[PunRPC]
    //public void RpcMethodName(PhotonMessageInfo info)
    //{
    //    // do work
    //}
    #endregion
}

