using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    // Handles The Logic during a Dialogue

    public static DialogueManager instance;
    public bool isInDialogue;
    public bool isOnlyInAsk;
    public float interactionRadius = 2.0f;
    public NPC currentTarget;

    List<NPC> allParticipants;
    DialogueRunner runner;


    void Awake()
    {
        instance = this;

    }
    void Start()
    {
        runner = ReferenceManager.instance.standartRunner;
        allParticipants = new List<NPC>(FindObjectsOfType<NPC>());
    }


    /// Find all DialogueParticipants
    /** Filter them to those that have a Yarn start node and are in range; 
     * then start a conversation with the first one
     */
    public void CheckForNearbyNPC()
    {
        var target = allParticipants.Find(delegate (NPC p)
        {
            return string.IsNullOrEmpty(p.talkToNode) == false && // has a conversation node?
            (p.transform.position - this.transform.position)// is in range?
            .magnitude <= interactionRadius;
        });
        if (target != null)
        {
            // Kick off the dialogue at this node.
            //Debug.Log("Found A Dialogue Partner: " + target.characterName);
            currentTarget = target;
            runner.StartDialogue(target.talkToNode);
        }
        //else
        //Debug.Log("Found No One");
    }
    /// <summary>
    /// called in Dialogue UI -> start Dialogue
    /// </summary>
    public void StartDialogue()
    {
        isInDialogue = true;
        UIManager.instance.PortrayEButton(null);
    }
    /// <summary>
    /// called in Dialogue UI -> end Dialogue
    /// </summary>
    public void EndDialogue()
    {
        isInDialogue = false;
        currentTarget = null;
    }
}
