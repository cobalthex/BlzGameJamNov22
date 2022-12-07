using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBehaviour<GameManager>
{
    public bool IsProxyManager;

    [HideInInspector]
    public WorldManager WorldManager;
    
    [HideInInspector]
    public ShopManager ShopManager;

    public PlayerData PlayerData = new PlayerData();

    [SerializeField]
    private GameData m_gameData;
    public GameData GameData => m_gameData;

    public void GoToMainMenu()
    {
        SceneManager.LoadScene((int)SceneNameEnum.MainMenu);
        SceneManager.LoadScene((int)SceneNameEnum.StartGame, LoadSceneMode.Additive);
        SceneManager.LoadScene((int)SceneNameEnum.LeaderBoard, LoadSceneMode.Additive);
        SceneManager.LoadScene((int)SceneNameEnum.Tutorial, LoadSceneMode.Additive);
        SceneManager.LoadScene((int)SceneNameEnum.Credits, LoadSceneMode.Additive);
    }

    public void GoToGame()
    {
        SceneManager.LoadScene((int)SceneNameEnum.World_1);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (!IsProxyManager)
            GoToMainMenu();

        PlayerData.Init();
    }
}
