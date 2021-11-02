using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptBubble : MonoBehaviour
{
    public bool acceptsCurrentWord;
    bool isHover = false;
    Image bubble;
    PromptBubbleData data;
    public GameObject child;
    public struct PromptBubbleData
    {
        public WordInfo.WordTags tag;
        public Color imageColor;
    }
    public void Initialize(string tag, PromptBubble[] saveIn)
    {
        bubble = GetComponent<Image>();
        data = new PromptBubbleData()
        {
            tag = WordUtilities.StringToTag(tag)
        };
        data.imageColor = WordUtilities.MatchColorToTag(data.tag);
        data.imageColor.a = 1;
        bubble.color = data.imageColor;
        GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        PlayerInputManager.instance.SavePrompt(this, saveIn);
    }
    /// <summary>
    /// Called, when the mouse is over the bubble. colors it darker for words of the correct tah
    /// </summary>
    /// <param name="isOnHover"></param>
    public void OnBubbleHover(bool isOnHover)
    {
        if (isOnHover && !isHover) //mouse starts hover
        {
            //if there is a currentWord AND it has the same tag as this specific word
            if (WordClickManager.instance.currentWord != null &&
                WordClickManager.instance.currentWord.GetComponent<Word>().data.tag == data.tag ||
                WordClickManager.instance.currentWord != null &&
                data.tag == WordInfo.WordTags.AllWords)
            {
                bubble.color = Color.Lerp(data.imageColor, ReferenceManager.instance.shadowButtonColor, 0.2f);
                acceptsCurrentWord = true;
            }
            else
                acceptsCurrentWord = false;
        }
        else if (!isOnHover && isHover) //mouse stops hover
        {
            acceptsCurrentWord = false;
            bubble.color = data.imageColor;
        }
        isHover = isOnHover;
    }
}