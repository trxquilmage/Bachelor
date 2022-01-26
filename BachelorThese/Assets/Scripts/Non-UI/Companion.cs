using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : NPC
{
    [HideInInspector] public bool inParty;
    public float minDistanceToPlayer = 2;
    public float speed = 2;

    Vector3 MovementDir;
    Vector3 movementDir
    {
        get { return MovementDir; }
        set
        {
            rigid.velocity = value;
            MovementDir = value;
            if (value == Vector3.zero)
            {
                rigid.isKinematic = true;
                TriggerWalkingAnimation(false);
            }
            else
            {
                rigid.isKinematic = false;
                TriggerWalkingAnimation(true);
            }
        }
    }

    DialogueManager diaM;
    Rigidbody rigid;
    Animator animator;
    protected override void Start()
    {
        base.Start();
        diaM = DialogueManager.instance;
        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        if (inParty)
        {
            FollowPlayer();
        }
    }
    private void FixedUpdate()
    {
        if (inParty)
        {
            TurnTowardsPlayer((targetPlayer.transform.position - npcMesh.transform.position).normalized);
        }
    }
    public void JoinParty()
    {
        inParty = true;
        interactionRadius /= 1.8f;
    }
    void FollowPlayer()
    {
        if (!diaM.isInDialogue && !diaM.isOnlyInAsk)
        {
            Vector3 directionToPlayer = (targetPlayer.transform.position - transform.position).normalized;

            float currentDistance = Vector3.Distance(transform.position, targetPlayer.transform.position);
            if (currentDistance < minDistanceToPlayer)
                movementDir = Vector3.zero;
            else
                movementDir = directionToPlayer * speed;
        }
        else
            movementDir = Vector3.zero;
    }
    public override void TurnTowardsPlayer(Vector3 directionToPlayer)
    {
        if (!diaM.isInDialogue && !diaM.isOnlyInAsk)
        {
            directionToPlayer = Vector3.Scale(directionToPlayer, new Vector3(1, 0, 1));
            npcMesh.transform.forward = Vector3.Lerp(npcMesh.transform.forward, directionToPlayer, 0.4f);
        }
    }
    public override void TurnAwayFromPlayer()
    {

    }
    void TriggerWalkingAnimation(bool changeTo)
    {
        animator.SetBool("Moving", changeTo);
    }
}
