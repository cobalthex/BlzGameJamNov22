using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItemWidget : MonoBehaviour
{
    public TMP_Text m_itemCountText;
    public Image m_itemIconImage;
    public ShopItemType m_itemType;
    public int m_itemCost;

    public void ShopMode()
    {

    }

    public void Init(ShopItemData itemData)
    {
        m_itemType = itemData.ItemType;
        m_itemIconImage.sprite = itemData.ItemIcon;
        m_itemCost = itemData.Cost;
        m_itemCountText.SetText("0");
        gameObject.SetActive(true);
    }
}
