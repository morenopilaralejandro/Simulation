using System.Collections.Generic;

public class DatabaseDependencies
{
    private readonly Dictionary<object, object[]> _deps = new();

    public void Register(object db, params object[] dependsOn)
    {
        _deps[db] = dependsOn;
    }

    public object[] Get(object db)
    {
        return _deps.TryGetValue(db, out var d)
            ? d
            : System.Array.Empty<object>();
    }
}
