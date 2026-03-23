using UnityEngine;
using Simulation.Enums.Item;

[CreateAssetMenu(fileName = "ItemDataRecovery", menuName = "ScriptableObject/Item/ItemDataRecovery")]
public class ItemDataRecovery : ItemData
{
    public int RecoveryAmountHp;
    public int RecoveryAmountSp;
}
