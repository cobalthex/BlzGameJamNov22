using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkedGameManager : SingletonBehaviour<NetworkedGameManager>
{
    public bool IsProxyManager;

    [HideInInspector]
    public NetworkedWorldManager WorldManager;

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

    private IEnumerator GameTimerRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            GameTimer--;
            UIManager.TimerText.SetText(GameTimer.ToString());
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
