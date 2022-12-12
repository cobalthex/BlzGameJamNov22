using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_gameOverPanel;

    [SerializeField]
    private GameObject m_hudPanel;

    [SerializeField]
    private TMP_Text[] m_playerMoneyText;

    public TMP_Text TimerText;
    public TMP_Text PlayerWinText;

    private GameData m_gameData;

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

        m_gameData = NetworkedGameManager.Instance.GameData;

        m_playerMoneyText[0].SetText(NetworkedGameManager.Instance.PlayerData[0].Money.ToString());
        m_playerMoneyText[1].SetText(NetworkedGameManager.Instance.PlayerData[1].Money.ToString());
    }

    private void OnEnable()
    {
        EventManager.AddListener<ItemPurchased>(OnItemPurchased);
    }

    private void OnDisabled()
    {
        EventManager.RemoveListener<ItemPurchased>(OnItemPurchased);
    }

    private void OnItemPurchased(ItemPurchased evt)
    {
        m_playerMoneyText[evt.Player].SetText(NetworkedGameManager.Instance.PlayerData[evt.Player].Money.ToString());
    }
}
