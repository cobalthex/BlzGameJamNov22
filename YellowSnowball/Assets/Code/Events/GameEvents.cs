public class OnGameStart : TEventBase
{
}

public class OnGameEnd : TEventBase
{
}

public class OnWinnerDeclared : TEventBase
{
}

public class ItemPurchased : TEventBase
{
    public ItemPurchased(int player, ShopItemType shopItemType)
    {
        Player = player;
        ShopItemType = shopItemType;
    }

    public int Player { get; }
    public ShopItemType ShopItemType { get; }
}

public class SnowBlowerValueUpdate : TEventBase
{
    public SnowBlowerValueUpdate(int playerId, float amount)
    {
        PlayerId = playerId;
        Amount = amount;
    }

    public int PlayerId { get; }
    public float Amount { get; }
}