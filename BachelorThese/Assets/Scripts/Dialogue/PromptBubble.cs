using UnityEngine;
using UnityEngine.UI;

public class PromptBubble : MonoBehaviour
{
    [SerializeField] Vector2 additionalSizeDelta;
    [HideInInspector] public bool acceptsCurrentWord;
    bool isHover = false;

    Vector2[] defaultSizeDelta;
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
                ScaleToChildsSize();
            else
                ScaleBackToOriginalSize();
        }
    }
    GameObject Child;

    public struct PromptBubbleData
    {
        public WordInfo.WordTag tag;
        public string[] subtags;
        public Color imageColor;
    }
    private void Awake()
    {
        refM = ReferenceManager.instance;
    }
    public void Initialize(string tag, string givenSubtags, Vector3[] wordParameters)
    {
        InitializeValues();
        ScaleObjectOnSpawn(wordParameters);

        string[] subtags = SplitSubtagsIntoArray(givenSubtags);
        SaveTagAndSubtag(tag, subtags);
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
    void SaveTagAndSubtag(string tag, string[] subtags)
    {
        data.tag = WordUtilities.GetTag(tag);
        data.subtags = subtags;
    }
    string[] SplitSubtagsIntoArray(string subtags)
    {
        if (subtags.Contains("-"))
            return subtags.Trim().Split("-"[0]);
        else if (subtags != "")
            return new string[1] { subtags };
        return new string[0];
    }
    void ColorPromptBubbleToTagColor()
    {
        data.imageColor = WordUtilities.MatchColorToTag(data.tag.name);
        bubble.color = data.imageColor;
    }
    void SaveStartScaleParameters()
    {
        rT.localScale = new Vector3(1, 1, 1);
        defaultSizeDelta = new Vector2[2] { rT.localPosition, rT.sizeDelta };
    }
    bool DoesPromptTagMatchASubtag(string compareToTag)
    {
        foreach (string subtag in data.subtags)
            if (subtag == compareToTag)
                return true;
        return false;
    }
    /// <summary>
    /// Called, when the mouse is over the bubble. colors it darker for words of the correct tah
    /// </summary>
    /// <param name="isOnHover"></param>
    public void OnBubbleHover(bool isOnHover)
    {
        bool hasCurrentWord = WordClickManager.instance.currentWord != null;
        bool noSubtags = data.subtags.Length == 0;
        bool promptSubtagMatchesAWordSubtag = false;
        bool promptSubtagMatchesAWordTag = false;

        BubbleData currentBubbleData = null;
        string allTagName = refM.wordTags[refM.allTagIndex].name;
        if (hasCurrentWord)
        {
            currentBubbleData = WordClickManager.instance.currentWord.GetComponent<Bubble>().data;
            promptSubtagMatchesAWordSubtag = DoesPromptTagMatchASubtag(currentBubbleData.subtag);
            promptSubtagMatchesAWordTag = DoesPromptTagMatchASubtag(currentBubbleData.tag);
        }

        if (isOnHover && !isHover && hasCurrentWord) //mouse starts hover
        {
            //correct input to bubble
            if (
                //prompt bubble: all:all
                data.tag.name == allTagName && noSubtags ||

                //prompt bubble: all:"TagName" is excluding said tagname
                data.tag.name == allTagName && !noSubtags && 
                !promptSubtagMatchesAWordTag && !promptSubtagMatchesAWordSubtag ||

                //prompt bubble: "Tag":All (tag != all)
                data.tag.name != allTagName && noSubtags
                && currentBubbleData.tag == data.tag.name ||

                //prompt bubble: "Tag":"Subtag" (Neither of these is all)
                data.tag.name != allTagName && !noSubtags &&
                data.tag.name == currentBubbleData.tag && promptSubtagMatchesAWordSubtag
                )
            {
                bubble.color = Color.Lerp(data.imageColor, refM.shadowButtonColor, 0.2f);
                acceptsCurrentWord = true;
            }
            //wrong Input to bubble, show message

            else if (
                // All:"Tag"
                data.tag.name == allTagName && promptSubtagMatchesAWordTag ||
                data.tag.name == allTagName && promptSubtagMatchesAWordSubtag ||

                // "Tag":"Subtag"
                data.tag.name != allTagName && noSubtags &&
                data.tag.name == currentBubbleData.tag && !promptSubtagMatchesAWordSubtag
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
    void ScaleObjectOnSpawn(Vector3[] wordParameters)
    {
        rT.localPosition = wordParameters[0];
        rT.sizeDelta = wordParameters[1];
        rT.localEulerAngles = Vector3.zero;

        rT.sizeDelta += additionalSizeDelta;
        rT.localPosition -= (Vector3)(additionalSizeDelta / 2) + Vector3.down * 2;
    }
    /// <summary>
    /// Scale the bubble so it fits the child
    /// </summary>
    public void ScaleToChildsSize()
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
    public void ScaleBackToOriginalSize()
    {
        RectTransform rT = GetComponent<RectTransform>();
        rT.localPosition = defaultSizeDelta[0];
        rT.sizeDelta = defaultSizeDelta[1];
    }
}
