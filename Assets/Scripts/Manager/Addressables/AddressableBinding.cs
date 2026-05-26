using System.Threading.Tasks;
using UnityEngine;

public class AddressableBinding<T> where T : Object
{
    private string _currentAddress;
    private int _version;

    public async Task<T> LoadAsync(string address)
    {
        int version = ++_version;

        Release();

        _currentAddress = address;

        T asset = await AddressableLoader.LoadAsync<T>(address);

        if (version != _version)
        {
            if (asset != null)
                AddressableLoader.Release(address);

            return null;
        }

        return asset;
    }

    public async Task<T> LoadOptionalAsync(string address)
    {
        int version = ++_version;

        Release();

        _currentAddress = address;

        T asset = await AddressableLoader.LoadOptionalAsync<T>(address);

        if (version != _version)
        {
            if (asset != null)
                AddressableLoader.Release(address);

            return null;
        }

        return asset;
    }

    public void Release()
    {
        if (!string.IsNullOrEmpty(_currentAddress))
        {
            AddressableLoader.Release(_currentAddress);
            _currentAddress = null;
        }
    }

    public void Cancel()
    {
        _version++;
    }
}
