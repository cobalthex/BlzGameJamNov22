using Photon.Pun;
using System.Linq;
using UnityEngine;

public class RPCManager : SingletonBehaviour<RPCManager>
{
    private PhotonView m_photonView;
    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
    }
    public void SendSnowToEnemy(int snowInMeters)
    {
        var enemyPlayer = PhotonNetwork.CurrentRoom.Players.First(x => !x.Value.IsLocal).Value;
        m_photonView.RPC("AddSnowToDriveway", enemyPlayer, new object[] { snowInMeters });
    }

    [PunRPC]
    void AddSnowToDriveway(int snowInMeters, PhotonMessageInfo info)
    {
        Debug.Log(string.Format("{0} meters of snow sent from player {1}", snowInMeters, info.Sender.ActorNumber));
    }
}