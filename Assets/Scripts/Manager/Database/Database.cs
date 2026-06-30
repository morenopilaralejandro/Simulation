using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class Database<T>
{
    private readonly Dictionary<string, T> _data = new();

    private readonly string _label;
    private readonly Func<T, string> _idSelector;

    public IReadOnlyDictionary<string, T> Data => _data;

    public Database(string label, Func<T, string> idSelector)
    {
        _label = label;
        _idSelector = idSelector;
    }

    public async Task LoadAsync()
    {
        var handle = Addressables.LoadAssetsAsync<T>(
            _label,
            item => _data[_idSelector(item)] = item
        );

        await handle.Task;
    }

    public T Get(string id)
    {
        _data.TryGetValue(id, out var value);
        return value;
    }
}
