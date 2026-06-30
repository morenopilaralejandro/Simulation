using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

public class DatabaseLoader
{
    private readonly DatabaseRegistry _db;
    private readonly DatabaseDependencies _deps;

    public DatabaseLoader(DatabaseRegistry db, DatabaseDependencies deps)
    {
        _db = db;
        _deps = deps;
    }

    public async Task LoadAllAsync()
    {
        var loaded = new HashSet<object>();

        foreach (var db in GetAllDatabases())
        {
            await LoadRecursive(db, loaded);
        }
    }

    private IEnumerable<object> GetAllDatabases()
    {
        return _db.GetType()
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Select(f => f.GetValue(_db));
    }

    private async Task LoadRecursive(object db, HashSet<object> loaded)
    {
        if (loaded.Contains(db))
            return;

        foreach (var dep in _deps.Get(db))
        {
            await LoadRecursive(dep, loaded);
        }

        var method = db.GetType().GetMethod("LoadAsync");
        await (Task)method.Invoke(db, null);

        loaded.Add(db);
    }
}
