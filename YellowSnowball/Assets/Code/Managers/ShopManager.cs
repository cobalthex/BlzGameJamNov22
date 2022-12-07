using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private int m_currentItem;
    private int m_shopItemCount;

    public void NextShopItem()
    {
    }

    public void PreviousShopItem()
    {
    }

    private void Start()
    {
        GameManager.Instance.ShopManager = this;

        var gameData = GameManager.Instance.GameData;
        m_shopItemCount = gameData.ShopItems.Length;
    }
}
