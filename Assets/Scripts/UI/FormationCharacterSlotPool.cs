using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;

public class FormationCharacterSlotPool : MonoBehaviour
{
    #region Field

    [SerializeField] private GameObject prefab;
    [SerializeField] private RectTransform fieldArea;
    [SerializeField] private RectTransform benchArea;
    private int initialPoolSizeField = 11;
    private int initialPoolSizeBench = 5;

    private Queue<FormationCharacterSlotUI> poolField = new Queue<FormationCharacterSlotUI>();
    private Queue<FormationCharacterSlotUI> poolBench = new Queue<FormationCharacterSlotUI>();

    #endregion

    #region Lifecycle

    private void Awake() 
    {
        Prewarm();
    }

    #endregion

    #region Pool
    
    private void Prewarm()
    {
        for (int i = 0; i < initialPoolSizeField; i++)
        {
            var go = Instantiate(prefab, fieldArea);
            var o = go.GetComponent<FormationCharacterSlotUI>();
            o.SetVisible(false);
            poolField.Enqueue(o);
        }

        for (int i = 0; i < initialPoolSizeBench; i++)
        {
            var go = Instantiate(prefab, benchArea);
            var o = go.GetComponent<FormationCharacterSlotUI>();
            o.SetVisible(false);
            poolBench.Enqueue(o);
        }
    }

    public FormationCharacterSlotUI Get(bool isBench)
    {
        var o = isBench ? poolBench.Dequeue() : poolField.Dequeue();
        o.SetVisible(true);
        return o;
    }

    public void Return(FormationCharacterSlotUI o, bool isBench)
    {
        if (o == null) return;

        o.Reset();
        o.SetVisible(false);

        if (isBench)
            poolBench.Enqueue(o);
        else 
            poolField.Enqueue(o);
    }

    public void ReturnAll(List<FormationCharacterSlotUI> slots, bool isBench)
    {
        foreach (var o in slots) 
            Return(o, isBench);
    }

    public void Clear()
    {
        foreach (var o in poolField) 
        {
            if (o != null) Destroy(o.gameObject);
        }
        poolField.Clear();

        foreach (var o in poolBench) 
        {
            if (o != null) Destroy(o.gameObject);
        }
        poolBench.Clear();
    }

    #endregion
}
