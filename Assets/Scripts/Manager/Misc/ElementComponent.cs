using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;

public class ElementComponent
{
    public Element[] Elements { get; private set; }

    public ElementComponent(Element[] elements)
    {
        Elements = elements;
    }

    public bool ContainsElement(Element element) 
    {
        for (int i = 0; i < Elements.Length; i++)
        {
            if (Elements[i] == element)
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsElement(Element[] elements) 
    {
        for (int i = 0; i < elements.Length; i++)
        {
            Element target = elements[i];
            bool foundCurrent = false;

            for (int j = 0; j < Elements.Length; j++)
            {
                if (Elements[j] == target)
                {
                    foundCurrent = true;
                    break; // Move to the next target element
                }
            }

            // If even one target element wasn't found, the whole check fails (ALL condition)
            if (!foundCurrent)
            {
                return false;
            }
        }
        return true;
    }
}
