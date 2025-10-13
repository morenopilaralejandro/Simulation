using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RefCountedAsset
{
    public AsyncOperationHandle Handle;
    public int RefCount;
}
