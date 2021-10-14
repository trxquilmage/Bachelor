using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NPC : MonoBehaviour
{

    public string characterName = "";
    public string talkToNode = "";

    [SerializeField] float fixedTime = 0.25f;

    GameObject player;
    float timer = 0;
    [Header("Optional")]
    public YarnProgram scriptToLoad;

    void Start()
    {
        if (scriptToLoad != null)
        {
            DialogueRunner dialogueRunner = FindObjectOfType<DialogueRunner>();
            dialogueRunner.Add(scriptToLoad);
        }
        player = FindObjectOfType<Player>().gameObject;
    }
    private void Update()
    {
        
        if (DialogueManager.instance.isActiveAndEnabled && !DialogueManager.instance.isInDialogue)
        {
            timer += Time.deltaTime;//every 0.25 sec, check if the Player is in Range
            if (fixedTime <= timer) //not really 100% fixed time bc i ignore the leftover time
            {
                timer = 0;
                if (CheckForPlayerRange())
                {
                    UIManager.instance.PortrayEButton(this.gameObject);
                }
                else if (UIManager.instance.eButtonSprite.enabled && !CheckForPlayerRange())
                    UIManager.instance.PortrayEButton(null);
            }
        }

    }
    bool CheckForPlayerRange()
    {

        if ((player.transform.position - this.transform.position)// is in range?
        .magnitude <= DialogueManager.instance.interactionRadius)
        {
            return true;
        }
        return false;
    }
}

