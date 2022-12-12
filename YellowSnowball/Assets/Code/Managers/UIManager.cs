using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_gameOverPanel;

    [SerializeField]
    private GameObject m_hudPanel;

    public TMP_Text TimerText;
    public TMP_Text PlayerWinText;

    public void ShowGameOver(string winnerName)
    {
        PlayerWinText.SetText($"{winnerName} wins!!!");
        m_hudPanel.SetActive(false);
        m_gameOverPanel.SetActive(true);
    }

    public void GoToMainMenu()
    {
        NetworkedGameManager.Instance.GoToMainMenu();
        NetworkedGameManager.Instance.GameOverTeardown();
    }

    private void Start()
    {
        NetworkedGameManager.Instance.UIManager = this;

        m_hudPanel.SetActive(true);
        m_gameOverPanel.SetActive(false);
    }
}
