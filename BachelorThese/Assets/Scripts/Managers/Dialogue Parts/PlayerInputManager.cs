using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    public PromptBubble[] currentPromptBubbles;
    public Word.WordData givenAnswer;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        currentPromptBubbles = new PromptBubble[3];
    }
    public void ReactToInput()
    {
        foreach (PromptBubble prompt in currentPromptBubbles)
        {
            if (prompt != null && prompt.child != null)
            {
                givenAnswer = prompt.child.GetComponent<Word>().data; ;
            }
        }
    }
    /// <summary>
    /// Checks, whether all prompts are filled at the moment
    /// </summary>
    public bool CheckForPromptsFilled()
    {
        bool allFilled = true;
        foreach (PromptBubble prompt in currentPromptBubbles)
        {
            if (prompt != null && prompt.child == null) //doesnt have a child
            {
                allFilled = false;
            }
        }
        return allFilled;
    }
    /// <summary>
    /// Save a prompt, after it was created
    /// </summary>
    /// <param name="bubble"></param>
    public void SavePrompt(PromptBubble bubble)
    {
        for (int i = 0; i < currentPromptBubbles.Length; i++)
        {
            if (currentPromptBubbles[i] == null)
            {
                currentPromptBubbles[i] = bubble;
                break;
            }
        }
    }
    /// <summary>
    /// Delete all prompts on the screen
    /// </summary>
    public void DeleteAllPrompts()
    {
        for (int i = 0; i < currentPromptBubbles.Length; i++)
        {
            if (currentPromptBubbles[i] != null)
            {
                Destroy(currentPromptBubbles[i].gameObject);
                currentPromptBubbles[i] = null;
            }
        }
    }
}
