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
    // Call this to send snow
    //-----------------------------------------------------------------------
    public void SendSnow(SnowTerrain terrain, Vector2 relativePosition, float xSize, float patternScaleMeters, float? addSaltExpirationTim)
    {
        if (CanUpdateSnow)
        {
            var playerId = NetworkedGameManager.Instance.WorldManager.GetPlayerByTerrain(terrain);
            m_photonView.RPC("AddSnowToDriveway", RpcTarget.AllViaServer, new object[] { playerId, relativePosition, xSize, patternScaleMeters, addSaltExpirationTim });
        }
    }

    // Ignore, as this is the receiver of RPC calls.
    [PunRPC]
    void AddSnowToDriveway(int playerId, Vector2 relativePosition, float xSize, float patternScaleMeters, float? addSaltExpirationTime, PhotonMessageInfo info)
    {
        var playerTerrain = NetworkedGameManager.Instance.WorldManager.GetPlayerSnowTerrain(playerId);
        playerTerrain.Deform(relativePosition, xSize, SnowDeformTexture, patternScaleMeters, addSaltExpirationTime);
    }
    //-----------------------------------------------------------------------
    #endregion

    #region Game Management

    // Start Game
    //-----------------------------------------------------------------------
    public void StartGame()
    {
        m_photonView.RPC("ReceiveGameStart", RpcTarget.AllViaServer);
    }

    // Ignore, as this is the receiver of RPC calls.
    [PunRPC]
    public void ReceiveGameStart(PhotonMessageInfo info)
    {
        Debug.Log("Received GameStart!");
        EventManager.Fire<OnGameStart>(new OnGameStart());
        NetworkedGameManager.Instance.StartGame();
    }
    //-----------------------------------------------------------------------

    // End Game
    //-----------------------------------------------------------------------
    public void EndGame()
    {
        m_photonView.RPC("ReceiveGameStart", RpcTarget.AllViaServer);
    }

    // Ignore, as this is the receiver of RPC calls.
    [PunRPC]
    public void ReceiveGameEnd(PhotonMessageInfo info)
    {
        Debug.Log("Received End Game!");
        EventManager.Fire<OnGameEnd>(new OnGameEnd());
    }
    //-----------------------------------------------------------------------
    #endregion

    #region Player Events

    // Purchase items
    //-----------------------------------------------------------------------
    public void PurchaseItem(int playerId, ShopItemType itemType)
    {
        m_photonView.RPC("ItemPurchased", RpcTarget.AllViaServer, new object[] { playerId, itemType });
    }

    // Ignore, as this is the receiver of RPC calls.
    [PunRPC]
    public void ItemPurchased(int playerId, ShopItemType itemType, PhotonMessageInfo info)
    {
        Debug.Log("Received item purchase update.");
        EventManager.Fire(new ItemPurchased(playerId, itemType));
    }
    //-----------------------------------------------------------------------

    // Snow blower meter
    //-----------------------------------------------------------------------
    public void SendSnowBlowerMeterValue(int playerId, float snowBlowerValue)
    {
        m_photonView.RPC("SnowBlowerMeterValueUpdate", RpcTarget.AllViaServer, new object[] { playerId, snowBlowerValue });
    }

    // Ignore, as this is the receiver of RPC calls.
    [PunRPC]
    public void SnowBlowerMeterValueUpdate(int playerId, float snowBlowerValue, PhotonMessageInfo info)
    {
        Debug.Log("Received snow blower value.");
        EventManager.Fire(new SnowBlowerValueUpdate(playerId, snowBlowerValue));
    }
    //-----------------------------------------------------------------------
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

