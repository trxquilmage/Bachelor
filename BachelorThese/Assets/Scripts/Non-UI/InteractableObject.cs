using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public CallFunction function;
    public float interactionRadius = 3;
    public GameObject targetPlayer;
    public ReferenceManager refM;

    public enum CallFunction
    {
        WorldMap
    }
    void Start()
    {
        SetValues();
    }

    void SetValues()
    {
        refM = ReferenceManager.instance;
        targetPlayer = refM.player;
    }

    public bool IsInRangeToPlayer()
    {
        return (transform.position - targetPlayer.transform.position).magnitude <= interactionRadius;
    }
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
        ReferenceManager.instance.worldMap.SetActive(open);
        EffectUtilities.ReColorAllInteractableWords();
        UIManager.instance.isInteracting = open;
    }
}
