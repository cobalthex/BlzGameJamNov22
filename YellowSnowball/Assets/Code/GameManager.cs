using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBehaviour<GameManager>
{
    [SerializeField]
    private GameData m_gameData;

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
        GoToMainMenu();
    }
}
