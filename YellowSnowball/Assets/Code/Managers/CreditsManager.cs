using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    private void Start()
    {
        MainMenuManager.Instance.CreditsManager = this;
        gameObject.SetActive(false);
    }
}
