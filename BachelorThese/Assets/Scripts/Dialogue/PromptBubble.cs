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
    public GameObject child
    {
        get { return Child; }
        set
        {
            Child = value;
            if (value != null)
                ScaleToChild();
            else
                ScaleBack();
        }
    }
    GameObject Child;
    Vector2[] parameters;
    public struct PromptBubbleData
    {
        public WordInfo.WordTag tag;
        public Color imageColor;
    }
    public void Initialize(string tag)
    {
        bubble = GetComponent<Image>();
        WordInfo.WordTag tagInfo = WordUtilities.GetTag(tag);
        data = new PromptBubbleData()
        {
            tag = tagInfo
        };
        data.imageColor = WordUtilities.MatchColorToTag(data.tag.name);
        data.imageColor.a = 1;
        bubble.color = data.imageColor;
        RectTransform rT = GetComponent<RectTransform>();
        rT.localScale = new Vector3(1, 1, 1);
        PlayerInputManager.instance.SavePrompt(this);
        parameters = new Vector2[2] {rT.localPosition, rT.sizeDelta};
    }
    /// <summary>
    /// Called, when the mouse is over the bubble. colors it darker for words of the correct tah
    /// </summary>
    /// <param name="isOnHover"></param>
    public void OnBubbleHover(bool isOnHover)
    {
        if (isOnHover && !isHover) //mouse starts hover
        {
            //if there is a currentWord AND it has the same tag as this specific prompt
            if (WordClickManager.instance.currentWord != null &&
                WordClickManager.instance.currentWord.GetComponent<Bubble>().data.tag == data.tag.name ||
                WordClickManager.instance.currentWord != null &&
                data.tag.name == ReferenceManager.instance.wordTags[0].name 
                && WordClickManager.instance.currentWord.GetComponent<Bubble>().data.tag != ReferenceManager.instance.wordTags[ReferenceManager.instance.questTagIndex].name)
            {
                bubble.color = Color.Lerp(data.imageColor, ReferenceManager.instance.shadowButtonColor, 0.2f);
                acceptsCurrentWord = true;
            }
            // in the specific situation, where a word tagged "Other" is dragged onto a prompt titled "All"
            else if (data.tag.name == ReferenceManager.instance.wordTags[ReferenceManager.instance.allTagIndex].name && 
                WordClickManager.instance.currentWord != null &&
                WordClickManager.instance.currentWord.GetComponent<Bubble>().data.tag == ReferenceManager.instance.wordTags[ReferenceManager.instance.otherTagIndex].name)
            {
                acceptsCurrentWord = false;
                StartCoroutine(EffectUtilities.ColorObjectInGradient(bubble.gameObject, new Color[] { bubble.color, new Color(), new Color(), new Color(), Color.red }, 0.3f));
                UIManager.instance.BlendInUI(ReferenceManager.instance.feedbackTextOtherTag, 3);
            }
            else
            {
                acceptsCurrentWord = false;
                if (WordClickManager.instance.currentWord != null)
                {
                    StartCoroutine(EffectUtilities.ColorObjectInGradient(bubble.gameObject,
                                        new Color[] { bubble.color, new Color(), new Color(), new Color(), Color.red }, 0.3f));
                    StartCoroutine(EffectUtilities.ColorObjectInGradient(WordClickManager.instance.currentWord.gameObject,
                        new Color[] { WordClickManager.instance.currentWord.GetComponent<Image>().color, new Color(), new Color(), new Color(), Color.red }, 0.3f));
                }
            }
        }
        else if (!isOnHover && isHover) //mouse stops hover
        {
            acceptsCurrentWord = false;
            if (WordClickManager.instance.currentWord != null && bubble.color == Color.red)
            {
                StartCoroutine(EffectUtilities.ColorObjectInGradient(bubble.gameObject, new Color[] { bubble.color, new Color(), new Color(), new Color(), data.imageColor }, 0.4f));
                StartCoroutine(EffectUtilities.ColorObjectInGradient(WordClickManager.instance.currentWord.gameObject,
                    new Color[] { WordClickManager.instance.currentWord.GetComponent<Image>().color, new Color(), new Color(), new Color(),
                    WordUtilities.MatchColorToTag(WordClickManager.instance.currentWord.GetComponent<Bubble>().data.tag) }, 0.3f));
            }
        }
        isHover = isOnHover;
    }
    /// <summary>
    /// takes a tag and checks, wheter it fits the bubble or not
    /// </summary>
    /// <param name="tag"></param>
    public bool CheckIfTagFits(string givenTag)
    {
        if (givenTag == data.tag.name)
            return true;
        else if (data.tag.name == ReferenceManager.instance.wordTags[ReferenceManager.instance.allTagIndex].name &&
            givenTag != ReferenceManager.instance.wordTags[ReferenceManager.instance.otherTagIndex].name) // the bubble is a "All"-bubble and the tag isnt "other"
            return true;
        else
            return false;
    }
    /// <summary>
    /// Scale the bubble so it fits the child
    /// </summary>
    public void ScaleToChild()
    {
        RectTransform childRT = child.GetComponent<RectTransform>();
        RectTransform rT = GetComponent<RectTransform>();
        Vector2 childPosition = (Vector2)(childRT.localPosition + rT.localPosition) - new Vector2(5, 3);
        Vector2 childSize = childRT.sizeDelta + new Vector2(10, 6); 
        rT.localPosition = childPosition;
        rT.sizeDelta = childSize;
        childRT.localPosition = new Vector2(5, 3);
    }
    /// <summary>
    /// Scale the bubble back to It's original size
    /// </summary>
    public void ScaleBack()
    {
        RectTransform rT = GetComponent<RectTransform>();
        rT.localPosition = parameters[0];
        rT.sizeDelta = parameters[1];
    }
}
