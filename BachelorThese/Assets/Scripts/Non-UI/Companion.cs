using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Companion : NPC
{
    [HideInInspector] public bool inParty;
    public float minDistanceToPlayer = 2;
    public float speed = 2;

    Vector3 currentTargetPosition
    {
        get { return CurrentTargetPosition; }
        set
        {
            MoveAgentToCurrentTargetPosition();
            CurrentTargetPosition = value;
        }
    }
    Vector3 CurrentTargetPosition;

    DialogueManager diaM;
    Rigidbody rigid;
    Animator animator;
    NavMeshAgent agent;

    bool playerIsMoving;

    protected override void Start()
    {
        base.Start();
        SetCompanionValues();
    }
    void SetCompanionValues()
    {
        diaM = DialogueManager.instance;
        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }
    private void FixedUpdate()
    {
        if (!inParty)
            return;
        Vector3 normalizedDirectionToPlayer = (targetPlayer.transform.position - npcMesh.transform.position).normalized;
        TurnTowardsPlayer(normalizedDirectionToPlayer);
    }
    void MoveAgentToCurrentTargetPosition()
    {
        agent.destination = currentTargetPosition;
        if (agent.hasPath)
            OnMovementStart();
    }
    public void JoinParty()
    {
        inParty = true;
        interactionRadius /= 1.8f;
        targetPlayer.GetComponent<Player>().currentCompanion = this;
    }
    public void StartFollowPlayer(bool start)
    {
        if (diaM.isInDialogue || diaM.isOnlyInAsk)

            return;

        playerIsMoving = start;
        if (playerIsMoving)
            StartCoroutine(FollowPlayer());
    }
    IEnumerator FollowPlayer()
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (playerIsMoving)
        {
            SetCurrentTargetPosition();
            yield return delay;
        }
    }
    void SetCurrentTargetPosition()
    {
        Vector3 directionToPlayer = Vector3.Scale((targetPlayer.transform.position - transform.position), new Vector3(1, 0, 1));
        directionToPlayer = directionToPlayer.normalized;

        float currentDistanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
        float distanceToTargetPosition = currentDistanceToPlayer - minDistanceToPlayer;

        if (currentDistanceToPlayer < 0)
            return;

        currentTargetPosition = transform.position + (directionToPlayer * distanceToTargetPosition);
    }
    IEnumerator CheckIfGoalIsReachedEachFrame()
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (agent.hasPath)
        {
            yield return delay;
        }
        OnMovementEnd();
    }
    void OnMovementStart()
    {
        rigid.isKinematic = false;
        TriggerWalkingAnimation(true);
        StartCoroutine(CheckIfGoalIsReachedEachFrame());
    }
    void OnMovementEnd()
    {
        rigid.isKinematic = true;
        TriggerWalkingAnimation(false);
    }
    public override void TurnTowardsPlayer(Vector3 directionToPlayer)
    {
        if (diaM.isInDialogue || diaM.isOnlyInAsk)
            return;

        directionToPlayer = Vector3.Scale(directionToPlayer, new Vector3(1, 0, 1));
        npcMesh.transform.forward = Vector3.Lerp(npcMesh.transform.forward, directionToPlayer, 0.4f);
    }
    public override void TurnAwayFromPlayer()
    {

    }
    void TriggerWalkingAnimation(bool changeTo)
    {
        animator.SetBool("Moving", changeTo);
    }
}
