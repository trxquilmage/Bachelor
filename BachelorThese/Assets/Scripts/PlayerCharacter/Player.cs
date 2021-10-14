using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    public InputMap controls;
    Vector3 movementAddition;
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
}
