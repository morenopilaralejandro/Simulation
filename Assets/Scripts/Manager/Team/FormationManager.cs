using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class FormationManager : MonoBehaviour
{
    public static FormationManager Instance { get; private set; }

    private readonly Dictionary<string, Formation> formations = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadAllFormations()
    {
        Addressables.LoadAssetsAsync<FormationData>("Formations", RegisterFormation);
    }

    public void RegisterFormation(FormationData data)
    {
        if (!formations.ContainsKey(data.FormationId))
        {
            var f = new Formation();
            f.Initialize(data);
            formations.Add(f.FormationId, f);
        }
    }

    public Formation GetFormation(string id)
    {
        formations.TryGetValue(id, out var f);
        return f;
    }
}
