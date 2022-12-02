using UnityEngine;

public class MainMenuManager : SingletonBehaviour<MainMenuManager>
{
    [HideInInspector]
    public StartGameManager StartGameManager;
    [HideInInspector]
    public LeaderBoardManager LeaderBoardManager;
    [HideInInspector]
    public TutorialManager TutorialManager;
    [HideInInspector]
    public CreditsManager CreditsManager;
   
    public void GoToMainMenu()
    {
        gameObject.SetActive(true);
    }

    public void StartGame()
    {
        StartGameManager.Show();
    }
    
    public void GoToLeaderBoard()
    {
        LeaderBoardManager.Show();
    }

    public void GoToTutorial()
    {
        TutorialManager.Show();
    }

    public void GoToCredits()
    {
        CreditsManager.Show();
    }

    private void OnEnable()
    {
        GoToMainMenu();
    }
    
}
