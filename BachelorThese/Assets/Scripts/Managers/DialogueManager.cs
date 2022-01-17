using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    // Handles The Logic during a Dialogue

    [HideInInspector] public static DialogueManager instance;
    [HideInInspector] public bool isInDialogue;
    [HideInInspector] public bool isOnlyInAsk;
    [HideInInspector] public NPC currentTarget;

    List<NPC> allParticipants;
    DialogueRunner runner;
    ReferenceManager refM;


    void Awake()
    {
        instance = this;

    }
    void Start()
    {
        runner = ReferenceManager.instance.standartRunner;
        refM = ReferenceManager.instance;
        allParticipants = new List<NPC>(FindObjectsOfType<NPC>());
    }


    /// Find all DialogueParticipants
    /** Filter them to those that have a Yarn start node and are in range; 
     * then start a conversation with the first one
     */
    public void StartConversationWithNPC(NPC npc)
    {
        if (npc != null && (npc.transform.position - this.transform.position).magnitude <= refM.interactionRadius)
        {
            currentTarget = npc;
            currentTarget.TurnTowardsPlayer((transform.position - npc.transform.position).normalized);
            runner.StartDialogue(npc.talkToNode);
        }
    }
    /// <summary>
    /// called in Dialogue UI -> start Dialogue
    /// </summary>
    public void StartDialogue()
    {
        isInDialogue = true;
        UIManager.instance.PortrayButton(null, refM.eButtonSprite);
    }
    /// <summary>
    /// called in Dialogue UI -> end Dialogue
    /// </summary>
    public void EndDialogue()
    {
        currentTarget.TurnAwayFromPlayer();
        isInDialogue = false;
        currentTarget = null;
    }
}
