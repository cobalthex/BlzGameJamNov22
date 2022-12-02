using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    private void Start()
    {
        MainMenuManager.Instance.TutorialManager = this;
        gameObject.SetActive(false);
    }
}
