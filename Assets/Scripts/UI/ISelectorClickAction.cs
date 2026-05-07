using UnityEngine;

public interface ISelectorClickAction<T>
{
    void Execute(T data, IClosableMenu menu);
}
