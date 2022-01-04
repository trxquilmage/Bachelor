using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptBubble : MonoBehaviour
{
    public bool acceptsCurrentWord;
    public bool acceptsEveryWord; //means accepts every word, even "Other" words
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
    ReferenceManager refM;
    public struct PromptBubbleData
    {
        public WordInfo.WordTag tag;
        public Color imageColor;
    }
    private void Start()
    {
        refM = ReferenceManager.instance;
    }
    public void Initialize(string tag)
    {
        bubble = GetComponent<Image>();
        //in the specific case of the Tag being "AllWordsA"
        if (tag == "AllWordsA")
        {
            acceptsEveryWord = true;
            tag = "AllWords";
        }

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
        parameters = new Vector2[2] { rT.localPosition, rT.sizeDelta };
    }
    /// <summary>
    /// Called, when the mouse is over the bubble. colors it darker for words of the correct tah
    /// </summary>
    /// <param name="isOnHover"></param>
    public void OnBubbleHover(bool isOnHover)
    {
        bool hasCurrentWord = WordClickManager.instance.currentWord != null;
        BubbleData currentBubbleData = null;
        if (hasCurrentWord)
            currentBubbleData = WordClickManager.instance.currentWord.GetComponent<Bubble>().data;
        if (isOnHover && !isHover) //mouse starts hover
        {
            //prompt bubble: allwords, acceptsEveryWord = true
            if (acceptsEveryWord && hasCurrentWord && data.tag.name == refM.wordTags[refM.allTagIndex].name ||

                //prompt bubble: allwords, word: !other 
                hasCurrentWord && data.tag.name == refM.wordTags[refM.allTagIndex].name
                && currentBubbleData.tag != refM.wordTags[refM.otherTagIndex].name ||

                //prompt bubble tag == word tag
                hasCurrentWord && currentBubbleData.tag == data.tag.name)
            {
                bubble.color = Color.Lerp(data.imageColor, refM.shadowButtonColor, 0.2f);
                acceptsCurrentWord = true;
            }
            // in the specific situation, where this is a bubble tagged "AllWords" and it is filled with unfitting contents
            else if (!acceptsEveryWord && data.tag.name == refM.wordTags[refM.allTagIndex].name &&
                hasCurrentWord && currentBubbleData.tag == refM.wordTags[refM.otherTagIndex].name)
            {
                acceptsCurrentWord = false;
                StartCoroutine(EffectUtilities.ColorObjectInGradient(bubble.gameObject, new Color[] { bubble.color, new Color(), new Color(), new Color(), Color.red }, 0.3f));
                UIManager.instance.BlendInUI(refM.feedbackTextOtherTag, 3);
            }
            else
            {
                acceptsCurrentWord = false;
                if (hasCurrentWord)
                {
                    StartCoroutine(EffectUtilities.ColorObjectInGradient(bubble.gameObject,
                                        new Color[] { bubble.color, new Color(), new Color(), new Color(), Color.red }, 0.3f));
                    StartCoroutine(EffectUtilities.ColorObjectInGradient(WordClickManager.instance.currentWord.gameObject,
                        new Color[] { WordUtilities.MatchColorToTag(currentBubbleData.tag), new Color(), new Color(), new Color(), Color.red }, 0.3f));
                }
            }
        }
        else if (!isOnHover && isHover) //mouse stops hover
        {
            acceptsCurrentWord = false;
            if (WordClickManager.instance.currentWord != null && bubble.color == Color.red)
            {
                StartCoroutine(EffectUtilities.ColorObjectInGradient(bubble.gameObject, new Color[] 
                    { bubble.color, new Color(), new Color(), new Color(), data.imageColor }, 0.4f));

                Color currentBubbleColor = WordClickManager.instance.currentWord.GetComponent<Bubble>().
                    wordParent.GetComponentInChildren<Image>().color;

                StartCoroutine(EffectUtilities.ColorObjectInGradient(WordClickManager.instance.currentWord.gameObject,
                    new Color[] { currentBubbleColor, new Color(), new Color(), new Color(),
                    WordUtilities.MatchColorToTag(currentBubbleData.tag) }, 0.3f));
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
