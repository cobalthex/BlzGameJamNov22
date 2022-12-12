using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkedGameManager : SingletonBehaviour<NetworkedGameManager>
{
    public bool IsProxyManager;

    [HideInInspector]
    public NetworkedWorldManager WorldManager;

    [HideInInspector]
    public RPCManager RPCManager;

    [HideInInspector]
    public ShopManager ShopManager;

    [HideInInspector]
    public UIManager UIManager;

    public PlayerData PlayerData = new PlayerData();

    [SerializeField]
    private GameData m_gameData;
    public GameData GameData => m_gameData;

    public int StartGameTimerInSec = 100;
    public int GameTimer = 0;

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

    // When player exits game over screen
    public void GameOverTeardown()
    {
    }

    private string GetWinner()
    {
        // If only one player, return the local player
        if (WorldManager.Players.Length == 1)
            return $"Player {(WorldManager.PlayerIndex + 1).ToString()}";

        // If two players determine who won
        if (WorldManager.SnowTerrain[0].RemainingSnow > WorldManager.SnowTerrain[1].RemainingSnow)
            return $"Player 1";

        return $"Player 1";
    }

    private IEnumerator GameTimerRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            GameTimer--;
            if (UIManager != null)
            {
                UIManager.TimerText.SetText(GameTimer.ToString());
            }

            if (GameTimer <= 0)
            {
                // Determine winner
                string winnerName = GetWinner();
                UIManager.ShowGameOver(winnerName);
                yield break;
            }
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (!IsProxyManager)
            GoToMainMenu();

        PlayerData.Init();

        GameTimer = StartGameTimerInSec;
        StartCoroutine(GameTimerRoutine());
    }
}
