using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptBubble : MonoBehaviour
{
    public bool acceptsCurrentWord;
    bool isHover = false;

    Vector2[] parameters;
    PromptBubbleData data;
    Image bubble;
    ReferenceManager refM;
    RectTransform rT;

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

    public struct PromptBubbleData
    {
        public WordInfo.WordTag tag;
        public string subtag;
        public Color imageColor;
    }
    private void Start()
    {
        refM = ReferenceManager.instance;
    }
    public void Initialize(string tag)
    {
        InitializeValues();

        string[] tagAndSubtag = SplitTagIntoTagAndSubtag(tag);
        SaveTagAndSubtag(tagAndSubtag[0], tagAndSubtag[1]);

        ColorPromptBubbleToTagColor();

        PlayerInputManager.instance.SavePrompt(this);
        SaveStartScaleParameters();
    }
    void InitializeValues()
    {
        bubble = GetComponent<Image>();
        data = new PromptBubbleData();
        rT = GetComponent<RectTransform>();
    }
    void SaveTagAndSubtag(string tag, string subtag)
    {
        data.tag = GetTagFromAbbreviation(tag);
        data.subtag = subtag;
    }
    string[] SplitTagIntoTagAndSubtag(string tagAndSubtag)
    {
        return tagAndSubtag.Trim().Split("-"[0]);
    }
    WordInfo.WordTag GetTagFromAbbreviation(string abbreviation)
    {
        switch (abbreviation)
        {
            case "All":
                return WordUtilities.GetTag("AllWords");
            case "Adj":
                return WordUtilities.GetTag("Location");
            case "Item":
                return WordUtilities.GetTag("Item");
            case "Name":
                return WordUtilities.GetTag("Name");
            case "Loc":
                return WordUtilities.GetTag("Location");
            case "Oth":
                return WordUtilities.GetTag("Other");
            default:
                Debug.Log("Tag abbreviation " + abbreviation + " unknown!");
                return new WordInfo.WordTag();
        }
    }
    void ColorPromptBubbleToTagColor()
    {
        data.imageColor = WordUtilities.MatchColorToTag(data.tag.name);
        data.imageColor.a = 1;
        bubble.color = data.imageColor;
    }
    void SaveStartScaleParameters()
    {
        rT.localScale = new Vector3(1, 1, 1);
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
        string allTagName = refM.wordTags[refM.allTagIndex].name;
        if (hasCurrentWord)
            currentBubbleData = WordClickManager.instance.currentWord.GetComponent<Bubble>().data;

        if (isOnHover && !isHover && hasCurrentWord) //mouse starts hover
        {
            //correct input to bubble
            if (
                //prompt bubble: all:all
                data.tag.name == allTagName && data.subtag == "All" ||

                //prompt bubble: all:"TagName" is excluding said tagname
                data.tag.name == allTagName && currentBubbleData.tag != data.subtag ||

                //prompt bubble: "Tag":All (tag != all)
                data.tag.name != allTagName && data.subtag == "All"
                && currentBubbleData.tag == data.tag.name ||

                //prompt bubble: "Tag":"Subtag" (Neither of these is all)
                data.tag.name != allTagName && data.subtag != "All" &&
                data.tag.name == currentBubbleData.tag && data.subtag == currentBubbleData.subtag
                )
            {
                bubble.color = Color.Lerp(data.imageColor, refM.shadowButtonColor, 0.2f);
                acceptsCurrentWord = true;
            }
            //wrong Input to bubble, show message
            
            else if (
                // All:"Tag"
                data.tag.name == allTagName && currentBubbleData.tag == data.subtag ||

                // "Tag":"Subtag"
                data.tag.name != allTagName && data.subtag != "All" &&
                data.tag.name == currentBubbleData.tag && data.subtag != currentBubbleData.subtag
                )
            {
                acceptsCurrentWord = false;
                StartCoroutine(EffectUtilities.ColorObjectInGradient(bubble.gameObject, new Color[] { bubble.color, new Color(), new Color(), new Color(), Color.red }, 0.3f));
                UIManager.instance.BlendInUI(refM.feedbackTextOtherTag, 3);
            }
            //wrong Input to bubble, no message
            else
            {
                acceptsCurrentWord = false;
                StartCoroutine(EffectUtilities.ColorObjectInGradient(bubble.gameObject,
                                    new Color[] { bubble.color, new Color(), new Color(), new Color(), Color.red }, 0.3f));
                StartCoroutine(EffectUtilities.ColorObjectInGradient(WordClickManager.instance.currentWord.gameObject,
                    new Color[] { WordUtilities.MatchColorToTag(currentBubbleData.tag), new Color(), new Color(), new Color(), Color.red }, 0.3f));

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
