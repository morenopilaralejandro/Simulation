using System.Collections.Generic;
using System.Linq;

public class LruCache<TKey, TValue>
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _map;
    private readonly LinkedList<CacheItem> _list;

    private class CacheItem
    {
        public TKey Key;
        public TValue Value;
    }

    public LruCache(int capacity)
    {
        _capacity = capacity;
        _map = new Dictionary<TKey, LinkedListNode<CacheItem>>(capacity);
        _list = new LinkedList<CacheItem>();
    }

    public bool TryGet(TKey key, out TValue value)
    {
        if (_map.TryGetValue(key, out var node))
        {
            value = node.Value.Value;
            _list.Remove(node);
            _list.AddFirst(node); // recently used
            return true;
        }

        value = default;
        return false;
    }

    public void Add(TKey key, TValue value)
    {
        if (_map.TryGetValue(key, out var node))
        {
            node.Value.Value = value;
            _list.Remove(node);
            _list.AddFirst(node);
            return;
        }

        if (_map.Count >= _capacity)
        {
            var last = _list.Last;
            if (last != null)
            {
                _map.Remove(last.Value.Key);
                _list.RemoveLast();
            }
        }

        var newNode = new LinkedListNode<CacheItem>(new CacheItem { Key = key, Value = value });
        _list.AddFirst(newNode);
        _map[key] = newNode;
    }

    public void Clear()
    {
        _map.Clear();
        _list.Clear();
    }

    public IEnumerable<TValue> Values => _map.Values.Select(n => n.Value.Value);
}
