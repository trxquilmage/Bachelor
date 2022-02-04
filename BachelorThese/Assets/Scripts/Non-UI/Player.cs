using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    public InputMap controls;
    [SerializeField] GameObject characterMesh;

    public Companion currentCompanion;

    Vector3 MovementDir;
    Vector3 movementDir
    {
        get { return MovementDir; }
        set
        {
            if (value == Vector3.zero && MovementDir != value)
                OnStopWalking();
            else if (value != Vector3.zero && MovementDir == Vector3.zero)
                OnStartWalking();

            MovementDir = value;
        }
    }

    NPC npcTarget;
    Companion companionTarget;
    GameObject objectTarget;
    Rigidbody rigid;
    ReferenceManager refM;
    Animator animator;

    private void Awake()
    {
        controls = new InputMap();
        animator = GetComponentInChildren<Animator>();
        rigid = this.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        refM = ReferenceManager.instance;

        controls.Player.Walk.performed += context => Movement(context.ReadValue<Vector2>());
        controls.Player.Talk.performed += context => InteractE();
        controls.Player.TalkCompanion.performed += context => InteractF();
        controls.Player.Escape.performed += context => EnterMenu();
    }
    private void Update()
    {
        if (!DialogueManager.instance.isInDialogue && !DialogueManager.instance.isOnlyInAsk)
            CheckForButton();
    }
    private void FixedUpdate()
    {
        TurnCharacterTowardMovementDirection();
    }
    public void Movement(Vector2 direction)
    {
        if (!DialogueManager.instance.isInDialogue && !DialogueManager.instance.isOnlyInAsk)
        {
            movementDir = new Vector3(direction.x, 0, direction.y);

            rigid.velocity = movementDir * speed;
        }
    }
    void OnStopWalking()
    {
        rigid.isKinematic = true;
        TriggerWalkingAnimation(false);
        CallMovementInCompanion(false);
    }
    void OnStartWalking()
    {
        rigid.isKinematic = false;
        TriggerWalkingAnimation(true);
        CallMovementInCompanion(true);
    }
    void CallMovementInCompanion(bool start)
    {
        if (currentCompanion == null)
            return;
        currentCompanion.StartFollowPlayer(start);
    }
    void TriggerWalkingAnimation(bool changeTo)
    {
        animator.SetBool("Moving", changeTo);
    }
    void TurnCharacterTowardMovementDirection()
    {
        if (movementDir != Vector3.zero)
        {
            characterMesh.transform.forward = Vector3.Lerp(characterMesh.transform.forward, movementDir, 0.4f);
        }
    }
    public void TurnTowardsNPC(GameObject npc)
    {
        StartCoroutine(Turn(npc.transform.position - transform.position));
    }
    IEnumerator Turn(Vector3 targetDirection)
    {
        targetDirection = Vector3.Scale(targetDirection, new Vector3(1, 0, 1)).normalized;
        WaitForEndOfFrame delay = new WaitForEndOfFrame();

        while (Vector3.Dot(targetDirection, animator.transform.forward) < 0.99f)
        {
            animator.transform.forward = Vector3.Lerp(animator.transform.forward, targetDirection, 0.4f);
            yield return delay;
        }
        animator.transform.forward = targetDirection;
    }
    void EnterMenu()
    {
        MenuManager.instance.PressedEsc();
    }
    void StopWalking()
    {
        rigid.velocity = Vector3.zero;
        TriggerWalkingAnimation(false);
    }
    void InteractE()
    {
        if (!DialogueManager.instance.isInDialogue && !DialogueManager.instance.isOnlyInAsk)
        {
            if (npcTarget != null && npcTarget.TryGetComponent<NPC>(out NPC npc) && npc.IsInRangeToPlayer())
            {
                StopWalking();
                TurnTowardsNPC(npc.gameObject);
                DialogueManager.instance.StartConversationWithNPC(npc);
            }
            else if (objectTarget != null && objectTarget.TryGetComponent<InteractableObject>(out InteractableObject iO))
            {
                StopWalking();
                iO.Interact(true);
            }
        }
    }
    void InteractF()
    {
        if (!DialogueManager.instance.isInDialogue && !DialogueManager.instance.isOnlyInAsk)
        {
            if (companionTarget != null)
            {
                StopWalking();
                if (companionTarget.TryGetComponent<Companion>(out Companion npc))
                {
                    TurnTowardsNPC(npc.gameObject);
                    DialogueManager.instance.StartConversationWithNPC(npc);
                }
            }
        }
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    bool CheckForNPCRange()
    {
        NPC[] allNPCs = ReferenceManager.instance.npcParent.GetComponentsInChildren<NPC>();
        for (int i = 0; i < allNPCs.Length; i++)
        {
            if (!(allNPCs[i] is Companion) || !((Companion)allNPCs[i]).inParty)
            {
                if (allNPCs[i].IsInRangeToPlayer())
                {
                    npcTarget = allNPCs[i];
                    return true;
                }
            }
        }
        return false;
    }
    bool CheckForCompanionRange()
    {
        Companion[] allNPCs = ReferenceManager.instance.npcParent.GetComponentsInChildren<Companion>();
        for (int i = 0; i < allNPCs.Length; i++)
        {
            if (allNPCs[i].inParty && allNPCs[i].IsInRangeToPlayer())
            {
                companionTarget = allNPCs[i];
                return true;
            }
        }
        return false;
    }
    bool CheckForInteractableObjectRange()
    {
        Transform[] allObjects = ReferenceManager.instance.allInteractableObjects;
        for (int i = 0; i < allObjects.Length; i++)
        {
            if (allObjects[i].GetComponentInChildren<InteractableObject>().IsInRangeToPlayer())
            {
                objectTarget = allObjects[i].gameObject;
                return true;
            }
        }
        return false;
    }
    void CheckForButton()
    {

        if (DialogueManager.instance.isActiveAndEnabled && !DialogueManager.instance.isInDialogue && !DialogueManager.instance.isOnlyInAsk)
        {
            if (CheckForCompanionRange())
            {
                UIManager.instance.PortrayOrHideInputButtonFeedback(companionTarget.pivotForE, refM.fButtonSprite);
            }
            else if (ReferenceManager.instance.fButtonSprite.enabled)
                UIManager.instance.PortrayOrHideInputButtonFeedback(null, refM.fButtonSprite);

            if (CheckForNPCRange())
            {
                UIManager.instance.PortrayOrHideInputButtonFeedback(npcTarget.pivotForE, refM.eButtonSprite);
            }
            else if (CheckForInteractableObjectRange())
            {
                UIManager.instance.PortrayOrHideInputButtonFeedback(objectTarget, refM.eButtonSprite);
            }
            else if (ReferenceManager.instance.eButtonSprite.enabled)
                UIManager.instance.PortrayOrHideInputButtonFeedback(null, refM.eButtonSprite);
        }
    }
}

