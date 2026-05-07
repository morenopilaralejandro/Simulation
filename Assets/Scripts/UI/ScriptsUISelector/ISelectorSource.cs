using UnityEngine;
using System.Collections.Generic;

public interface ISelectorSource<T>
{
    IEnumerable<T> Enumerate();
}
