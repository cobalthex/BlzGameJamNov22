using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

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

    public ItemWidgets[] m_playerItems = new ItemWidgets[2];

    private GameData m_gameData;

    [Serializable]
    public class ItemWidgets
    {
        public InventoryItemWidget[] m_itemWidgets;
    }

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

    public void UpdateOnItemEvent(int playerIndex)
    {
        m_playerMoneyText[playerIndex].SetText(NetworkedGameManager.Instance.PlayerData[playerIndex].Money.ToString());

        for(int j = 0; j < m_gameData.ShopItems.Length; j++)
        {
            m_playerItems[playerIndex].m_itemWidgets[j].m_itemCountText.SetText(NetworkedGameManager.Instance.PlayerData[playerIndex].Inventory[m_gameData.ShopItems[j].ItemType].Count.ToString());
        }
    }

    private void Start()
    {
        NetworkedGameManager.Instance.UIManager = this;

        m_hudPanel.SetActive(true);
        m_gameOverPanel.SetActive(false);

        m_gameData = NetworkedGameManager.Instance.GameData;

        m_playerMoneyText[0].SetText(NetworkedGameManager.Instance.PlayerData[0].Money.ToString());
        m_playerMoneyText[1].SetText(NetworkedGameManager.Instance.PlayerData[1].Money.ToString());

        // Setup player items
        for(int i = 0; i <= 1; i++)
        {
            for(int j = 0; j < m_gameData.ShopItems.Length; j++)
            {
                m_playerItems[i].m_itemWidgets[j].Init(m_gameData.ShopItems[j]);
            }
        }
    }

    private void OnEnable()
    {
    }

    private void OnDisabled()
    {
    }
}
