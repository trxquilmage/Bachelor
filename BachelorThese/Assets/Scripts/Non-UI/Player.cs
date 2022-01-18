using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    public InputMap controls;
    [SerializeField] GameObject characterMesh;

    Vector3 MovementDir;
    Vector3 movementDir
    {
        get { return MovementDir; }
        set
        {
            MovementDir = value;
        }
    }

    GameObject target;
    GameObject companionTarget;
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

            if (rigid.velocity == Vector3.zero)
                TriggerWalkingAnimation(false);
            else
                TriggerWalkingAnimation(true);
        }
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
        StartCoroutine(Turn((npc.transform.position - transform.position).normalized));
    }
    IEnumerator Turn(Vector3 targetDirection)
    {
        targetDirection = Vector3.Scale(targetDirection, new Vector3(1,0,1));
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (Vector3.Dot(targetDirection, animator.transform.forward) < 0.99f)
        {
            animator.transform.forward = Vector3.Lerp(animator.transform.forward, targetDirection, 0.4f);
            yield return delay;
        }
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
            if (target != null)
            {
                StopWalking();
                if (target.TryGetComponent<NPC>(out NPC npc))
                {
                    TurnTowardsNPC(npc.gameObject);
                    DialogueManager.instance.StartConversationWithNPC(npc);
                }
                else if (target.TryGetComponent<InteractableObject>(out InteractableObject iO))
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
                if ((allNPCs[i].transform.position - this.transform.position)// is in range?
                        .magnitude <= ReferenceManager.instance.interactionRadius)
                {
                    target = allNPCs[i].gameObject;
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
            if (allNPCs[i].inParty)
            {
                if ((allNPCs[i].transform.position - this.transform.position)// is in range?
                        .magnitude <= ReferenceManager.instance.interactionRadius)
                {
                    companionTarget = allNPCs[i].gameObject;
                    return true;
                }
            }
        }
        return false;
    }
    bool CheckForInteractableObjectRange()
    {
        Transform[] allObjects = ReferenceManager.instance.allInteractableObjects;
        for (int i = 0; i < allObjects.Length; i++)
        {
            if ((allObjects[i].transform.position - this.transform.position)// is in range?
                    .magnitude <= ReferenceManager.instance.interactionRadius)
            {
                target = allObjects[i].gameObject;
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
                UIManager.instance.PortrayButton(companionTarget, refM.fButtonSprite);
            }
            if (CheckForNPCRange())
            {
                UIManager.instance.PortrayButton(target, refM.eButtonSprite);
            }
            else if (CheckForInteractableObjectRange())
            {
                UIManager.instance.PortrayButton(target, refM.eButtonSprite);
            }
            else if (ReferenceManager.instance.eButtonSprite.enabled)
                UIManager.instance.PortrayButton(null, refM.eButtonSprite);
        }
    }
}

