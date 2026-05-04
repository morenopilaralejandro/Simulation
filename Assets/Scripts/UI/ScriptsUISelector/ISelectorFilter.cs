using UnityEngine;

public interface ISelectorFilter<T>
{
    bool Matches(T item);
}
