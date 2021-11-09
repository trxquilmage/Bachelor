using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public CallFunction function;
    public enum CallFunction
    {
        WorldMap
    }
    /// <summary>
    /// calls the function, that has been selected in the inspector
    /// </summary>
    public void Interact(bool open)
    {
        switch (function)
        {
            case CallFunction.WorldMap:
                WorldMap(open);
                break;
        }
    }
    void WorldMap(bool open)
    {
        //enable world map
        ReferenceManager.instance.worldMap.SetActive(open);
        // Update Color all text
        EffectUtilities.ReColorAllInteractableWords();
        // pause movement
        UIManager.instance.isInteracting = open;
    }
}
