using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    private void Start()
    {
        MainMenuManager.Instance.LeaderBoardManager = this;
        gameObject.SetActive(false);
    }
}
