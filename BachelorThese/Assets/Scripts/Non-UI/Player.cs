using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    public InputMap controls;
    GameObject target;
    Rigidbody rigid;
    private void Awake()
    {
        controls = new InputMap();
    }
    private void Start()
    {
        controls.Player.WalkUD.performed += context => Movement(-10, context.ReadValue<float>());
        controls.Player.WalkLR.performed += context => Movement(context.ReadValue<float>(), -10);
        controls.Player.Talk.performed += context => Interact();
        rigid = this.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (!DialogueManager.instance.isInDialogue && !DialogueManager.instance.isOnlyInAsk)
            FixedCheckForEButton();
    }

    public void Movement(float directionX, float directionY)
    {
        if (!DialogueManager.instance.isInDialogue && !DialogueManager.instance.isOnlyInAsk)
        {
            if (directionX == -10)
                rigid.velocity = new Vector3(rigid.velocity.normalized.x, 0, directionY);
            else if (directionY == -10)
                rigid.velocity = new Vector3(directionX, 0, rigid.velocity.normalized.z);
            rigid.velocity = rigid.velocity.normalized;
            rigid.velocity *= speed * Time.deltaTime;
        }
    }
    void StopWalking()
    {
        rigid.velocity = Vector3.zero;
    }
    void Interact()
    {
        if (!DialogueManager.instance.isInDialogue && !DialogueManager.instance.isOnlyInAsk)
        {
            if (target != null)
            {
                StopWalking();
                if (target.TryGetComponent<NPC>(out NPC npc))
                    DialogueManager.instance.CheckForNearbyNPC();
                else if (target.TryGetComponent<InteractableObject>(out InteractableObject iO))
                    iO.Interact(true);
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
        Transform[] allNPCs = ReferenceManager.instance.npcParent.GetComponentsInChildren<Transform>();
        for (int i = 1; i < allNPCs.Length; i++)
        {
            if ((allNPCs[i].transform.position - this.transform.position)// is in range?
                    .magnitude <= DialogueManager.instance.interactionRadius)
            {
                target = allNPCs[i].gameObject;
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
            if ((allObjects[i].transform.position - this.transform.position)// is in range?
                    .magnitude <= DialogueManager.instance.interactionRadius * 3)
            {
                target = allObjects[i].gameObject;
                return true;
            }
        }
        return false;
    }
    void FixedCheckForEButton()
    {
        if (DialogueManager.instance.isActiveAndEnabled && !DialogueManager.instance.isInDialogue && !DialogueManager.instance.isOnlyInAsk)
        {
            if (CheckForNPCRange())
            {
                UIManager.instance.PortrayEButton(target);
            }
            else if (CheckForInteractableObjectRange())
            {
                UIManager.instance.PortrayEButton(target);
            }
            else if (ReferenceManager.instance.eButtonSprite.enabled)
                UIManager.instance.PortrayEButton(null);
        }

    }
}

