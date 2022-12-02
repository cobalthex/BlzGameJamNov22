using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Driveway[] Driveways;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.WorldManager = this;
    }

    public Driveway GetPlayerDriveway(int playerId)
    {
        return Driveways[playerId];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
