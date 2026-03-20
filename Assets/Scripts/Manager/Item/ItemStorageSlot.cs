[System.Serializable]
public class ItemStorageSlot
{
    public Item Item;
    public int Count;

    public ItemStorageSlot(Item item, int count = 1)
    {
        this.Item = item;
        this.Count = count;
    }

    public void AddCount(int amount)
    {
        Count += amount;
    }

    public bool RemoveCount(int amount = 1)
    {
        if (Count >= amount)
        {
            Count -= amount;
            return true;
        }
        return false;
    }
}
