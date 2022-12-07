using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private int m_currentItem;
    private int m_shopItemCount;
    private GameData m_gameData;

    public void NextShopItem()
    {
        m_currentItem = m_currentItem++ % m_shopItemCount;
        Debug.Log($"Item is {m_gameData.ShopItems[m_currentItem].ItemType}");
        Debug.Log($"Item is {m_gameData.ShopItems[m_currentItem].Cost}");
    }

    public void PreviousShopItem()
    {
        m_currentItem = m_currentItem-- % m_shopItemCount;
        Debug.Log($"Item is {m_gameData.ShopItems[m_currentItem].ItemType}");
        Debug.Log($"Item is {m_gameData.ShopItems[m_currentItem].Cost}");
    }

    private void Start()
    {
        GameManager.Instance.ShopManager = this;

        m_gameData = GameManager.Instance.GameData;
        m_shopItemCount = m_gameData.ShopItems.Length;
    }
}
