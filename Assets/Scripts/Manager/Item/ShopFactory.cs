using Aremoreno.Enums.Item;

public static class ShopFactory
{
    public static Shop Create(ShopData data)
    {
        return new Shop(data);
    }

    public static Shop CreateById(string id)
    {
        return new Shop(DatabaseManager.Instance.GetShopData(id));
    }
}
