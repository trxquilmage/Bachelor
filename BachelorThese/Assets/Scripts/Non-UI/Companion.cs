using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : NPC
{
    [HideInInspector] public bool inParty;
    public float minDistanceToPlayer = 2;
    public float speed = 2;

    GameObject targetPlayer;
    ReferenceManager refM;
    DialogueManager diaM;
    private void Start()
    {
        refM = ReferenceManager.instance;
        diaM = DialogueManager.instance;
        targetPlayer = refM.player;
    }
    private void Update()
    {
        if (inParty)
            FollowPlayer();
    }
    void FollowPlayer()
    {
        if (!diaM.isInDialogue && !diaM.isOnlyInAsk)
        {
            Vector3 directionToPlayer = (targetPlayer.transform.position - transform.position).normalized;
            float currentDistance = Vector3.Distance(transform.position, targetPlayer.transform.position);
            if (currentDistance > minDistanceToPlayer)
                transform.position = Vector3.Lerp(transform.position, transform.position + directionToPlayer * speed, 0.4f);
        }
    }

}
