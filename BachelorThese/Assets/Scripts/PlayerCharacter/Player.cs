using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    public InputMap controls;
    Vector3 movementAddition;
    float timer = 0;
    float fixedTime = 0.25f;
    private void Awake()
    {
        controls = new InputMap();
    }
    private void Start()
    {
        controls.Player.WalkUD.performed += context => Movement(-10, context.ReadValue<float>());
        controls.Player.WalkLR.performed += context => Movement(context.ReadValue<float>(), -10);
        controls.Player.Talk.performed += context => Talk();

    }
    private void Update()
    {
        transform.position += movementAddition * speed;
        FixedCheckForEButton();
    }

    public void Movement(float directionX, float directionY)
    {
        if (!DialogueManager.instance.isInDialogue)
        {
            if (directionX == -10)
                movementAddition = new Vector3(movementAddition.x, 0, directionY);
            else if (directionY == -10)
                movementAddition = new Vector3(directionX, 0, movementAddition.z);
        }
    }

    void Talk()
    {
        if (!DialogueManager.instance.isInDialogue)
        {
            DialogueManager.instance.CheckForNearbyNPC();
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
                return true;
            }
        }
        return false;
    }
    void FixedCheckForEButton()
    {
        if (DialogueManager.instance.isActiveAndEnabled && !DialogueManager.instance.isInDialogue)
        {
            timer += Time.deltaTime;//every 0.25 sec, check if the Player is in Range
            if (fixedTime <= timer) //not really 100% fixed time bc i ignore the leftover time
            {
                timer = 0;
                if (CheckForNPCRange())
                {
                    UIManager.instance.PortrayEButton(this.gameObject);
                }
                else if (UIManager.instance.eButtonSprite.enabled && !CheckForNPCRange())
                    UIManager.instance.PortrayEButton(null);
            }
        }

    }
}

