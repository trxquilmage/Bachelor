using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : NPC
{
    [HideInInspector] public bool inParty;
    public float distanceToPlayer = 2;

    GameObject targetPlayer;
    ReferenceManager refM;
    private void Start()
    {
        refM = ReferenceManager.instance;
        targetPlayer = refM.player;
    }
    private void Update()
    {
        if (inParty)
            FollowPlayer();
    }
    void FollowPlayer()
    {
        if (!DialogueManager.instance.isInDialogue && !DialogueManager.instance.isOnlyInAsk)
        {
            transform.position = (transform.position - targetPlayer.transform.position).normalized * distanceToPlayer + targetPlayer.transform.position;
        }
    }

}
