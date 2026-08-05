using UnityEngine;
using System;
using System.Collections.Generic;

public class IconManager : MonoBehaviour
{
    public static IconManager Instance { get; private set; }

    public CategoryIconLibrary Category;
    public TraitIconLibrary Trait;
    public ElementIconLibrary Element;
    public GenderIconLibrary Gender;
    public IconLibraryMatchRank MatchRank;
    //public IconLibraryStat Stat;

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

/*
Usage: IconManager.Instance.Category.GetIcon(Category.Catch);
*/

}
