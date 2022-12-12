using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private bool m_isShopOpen;
    private int m_currentItem;
    private int m_shopItemCount;
    private GameData m_gameData;

    public void NextShopItem()
    {
    }

    public void PreviousShopItem()
    {
    }

    private void Start()
    {
        NetworkedGameManager.Instance.ShopManager = this;

        m_gameData = NetworkedGameManager.Instance.GameData;
        m_shopItemCount = m_gameData.ShopItems.Length;
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
        PlayerData playerData = NetworkedGameManager.Instance.PlayerData[evt.Player];

        playerData.Inventory[evt.ShopItemType].Count++;
        foreach(ShopItemData itemData in m_gameData.ShopItems)
        {
            if (itemData.ItemType == evt.ShopItemType)
            playerData.Money -= itemData.Cost;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_gameData.Keys.AccessShop))
        {
            // AccessShop
            m_isShopOpen = !m_isShopOpen;
            Debug.Log($"Shop is Open");
        }
        else if (Input.GetKeyDown(m_gameData.Keys.PreviousShopItem))
        {
            if (m_currentItem > 0)
                m_currentItem--;
            Debug.Log($"Previous item is {m_gameData.ShopItems[m_currentItem].ItemType}");
        }
        else if (Input.GetKeyDown(m_gameData.Keys.NextShopItem))
        {
            if (m_currentItem < m_shopItemCount - 1)
                m_currentItem++;
            Debug.Log($"Next item is {m_gameData.ShopItems[m_currentItem].ItemType}");
        }
        else if (Input.GetKeyDown(m_gameData.Keys.BuyShopItem))
        {
            var player = NetworkedGameManager.Instance.GetLocalPlayerData();
            var shopItem = m_gameData.ShopItems[m_currentItem];
            // If player can afford and is not at max count
            if (player.CanPurchaseItem(shopItem))
            {
                Debug.Log($"Bought Item is {m_gameData.ShopItems[m_currentItem].ItemType}");
                RPCManager.Instance.PurchaseItem(NetworkedGameManager.Instance.WorldManager.PlayerIndex, shopItem.ItemType);
            }
        }
    }
}


