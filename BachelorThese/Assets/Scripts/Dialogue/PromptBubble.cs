using UnityEngine;
using UnityEngine.UI;

public class PromptBubble : MonoBehaviour
{
    [SerializeField] Vector2 additionalSizeDelta;
    [HideInInspector] public bool acceptsCurrentWord;
    bool wasHoveringLastFrame = false;

    public PromptBubbleData data;

    Vector2[] defaultSizeDelta;
    Image bubble;
    ReferenceManager refM;
    RectTransform rT;
    PromptBubblePadding childPadding;

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

        PlayerInputManager.instance.SavePromptAfterCreation(this);
        SaveStartScaleParameters();
        childPadding.UpdateBounds();
    }
    void InitializeValues()
    {
        bubble = GetComponent<Image>();
        data = new PromptBubbleData();
        rT = GetComponent<RectTransform>();
        childPadding = GetComponentInChildren<PromptBubblePadding>();
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
        data.imageColor = WordUtilities.MatchColorToTag(data.tag.name, "");
        bubble.color = data.imageColor;
    }

    void SaveStartScaleParameters()
    {
        rT.localScale = new Vector3(1, 1, 1);
        defaultSizeDelta = new Vector2[2] { rT.localPosition, rT.sizeDelta };
    }
    public bool DoesPromptTagMatchASubtag(string compareToTag)
    {
        foreach (string subtag in data.subtags)
            if (subtag == compareToTag)
                return true;
        return false;
    }
    /// <summary>
    /// Called, when the mouse is over the bubble. colors it darker for words of the correct tah
    /// </summary>
    /// <param name="isCurrentlyHovering"></param>
    public void OnBubbleHover(bool isCurrentlyHovering)
    {
        OnHoverHandler onHoverHandler = new OnHoverHandler(this);
        Bubble currentWord = onHoverHandler.HasCurrentWord() ? WordClickManager.instance.currentWord.GetComponent<Bubble>() : null;

        if (onHoverHandler.MouseStartsHover(isCurrentlyHovering, wasHoveringLastFrame))
        {
            if (onHoverHandler.InputIsCorrect())
            {
                bubble.color = Color.Lerp(data.imageColor, refM.shadowButtonColor, 0.2f);
                acceptsCurrentWord = true;
            }
            else
            {
                acceptsCurrentWord = false;

                StartCoroutine(EffectUtilities.ColorSingularObjectInGradient(bubble.gameObject,
                                    new Color[] { bubble.color, new Color(), new Color(), new Color(), Color.red }, 0.3f));
                StartCoroutine(EffectUtilities.ColorObjectAndChildrenInGradient(WordClickManager.instance.currentWord.gameObject,
                    new Color[] { WordUtilities.MatchColorToTag(currentWord.data.tag, currentWord.data.subtag), new Color(), new Color(), new Color(), Color.red }, 0.3f));

                currentWord.ShakeBubbleAsFeedbackRotated();

                if (onHoverHandler.InputIsIncorrect_RequiresFeedback())
                    UIManager.instance.BlendInUI(refM.warningWrongTag, 3);
            }
        }
        else if (onHoverHandler.MouseEndsHover(isCurrentlyHovering, wasHoveringLastFrame))
        {
            acceptsCurrentWord = false;
            if (currentWord != null)
            {
                Color currentWordColor = currentWord.wordParent.GetComponentInChildren<Image>().color;
                Color targetWordColor = WordUtilities.MatchColorToTag(currentWord.data.tag, currentWord.data.subtag);

                if (!EffectUtilities.CompareColor(currentWordColor, targetWordColor))
                {
                    StartCoroutine(EffectUtilities.ColorObjectAndChildrenInGradient(currentWord.gameObject,
                        new Color[] { currentWordColor, new Color(), new Color(), new Color(), targetWordColor }, 0.3f));
                }
            }

            StartCoroutine(EffectUtilities.ColorSingularObjectInGradient(bubble.gameObject, new Color[]
                    { bubble.color, new Color(), new Color(), new Color(), data.imageColor }, 0.4f));

        }
        wasHoveringLastFrame = isCurrentlyHovering;
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
        childPadding.UpdateBounds();
    }
    /// <summary>
    /// Scale the bubble back to It's original size
    /// </summary>
    public void ScaleBackToOriginalSize()
    {
        RectTransform rT = GetComponent<RectTransform>();
        rT.localPosition = defaultSizeDelta[0];
        rT.sizeDelta = defaultSizeDelta[1];
        childPadding.UpdateBounds();
    }
}

public class OnHoverHandler
{
    protected bool hasCurrentWord;
    protected bool noSubtags;
    protected bool promptSubtagMatchesAWordSubtag;
    protected bool promptSubtagMatchesAWordTag;
    protected BubbleData wordData;
    protected PromptBubble.PromptBubbleData data;
    protected PromptBubble relatedPromptBubble;
    protected string allTagName;

    protected ReferenceManager refM;

    public OnHoverHandler(PromptBubble relatedPromptBubble)
    {
        AssignInitialValues(relatedPromptBubble);

        if (hasCurrentWord)
        {
            wordData = WordClickManager.instance.currentWord.GetComponent<Bubble>().data;
            CheckSubtagMatches();
        }
    }

    protected void CheckSubtagMatches()
    {
        promptSubtagMatchesAWordSubtag = relatedPromptBubble.DoesPromptTagMatchASubtag(wordData.subtag);
        promptSubtagMatchesAWordTag = relatedPromptBubble.DoesPromptTagMatchASubtag(wordData.tag);
    }

    protected void AssignInitialValues(PromptBubble relatedPromptBubble)
    {
        refM = ReferenceManager.instance;

        hasCurrentWord = WordClickManager.instance.currentWord != null;
        data = relatedPromptBubble.data;
        noSubtags = data.subtags.Length == 0;
        promptSubtagMatchesAWordSubtag = false;
        promptSubtagMatchesAWordTag = false;
        wordData = null;
        allTagName = refM.wordTags[refM.allTagIndex].name;
        this.relatedPromptBubble = relatedPromptBubble;
    }

    public virtual bool InputIsCorrect()
    {
        return
        (//prompt bubble: all:all
        data.tag.name == allTagName && noSubtags ||

        //prompt bubble: all:"TagName" is excluding said tagname
        data.tag.name == allTagName && !noSubtags &&
        !promptSubtagMatchesAWordTag && !promptSubtagMatchesAWordSubtag ||

        //prompt bubble: "Tag":All (tag != all)
        data.tag.name != allTagName && noSubtags
        && wordData.tag == data.tag.name ||

        //prompt bubble: "Tag":"Subtag" (Neither of these is all)
        data.tag.name != allTagName && !noSubtags &&
        data.tag.name == wordData.tag && promptSubtagMatchesAWordSubtag
        );
    }

    public bool HasCurrentWord()
    {
        return hasCurrentWord;
    }

    public bool InputIsIncorrect_RequiresFeedback()
    {
        return (
        // All:"Tag"
        data.tag.name == allTagName && promptSubtagMatchesAWordTag ||
        data.tag.name == allTagName && promptSubtagMatchesAWordSubtag ||

        // "Tag":"Subtag"
        data.tag.name != allTagName && !noSubtags &&
        data.tag.name == wordData.tag && !promptSubtagMatchesAWordSubtag
        );
    }

    public bool MouseStartsHover(bool isCurrentlyHovering, bool wasHoveringLastFrame)
    {
        return (isCurrentlyHovering && !wasHoveringLastFrame && HasCurrentWord());
    }

    public bool MouseEndsHover(bool isCurrentlyHovering, bool wasHoveringLastFrame)
    {
        return (!isCurrentlyHovering && wasHoveringLastFrame);
    }
}

public class OnPromptGreyoutHandler : OnHoverHandler
{
    public OnPromptGreyoutHandler(PromptBubble relatedPromptBubble) : base(relatedPromptBubble)
    {
        AssignInitialValues(relatedPromptBubble);
    }
    void GetWordDataAndCheckSubtagMatches(BubbleData wordData)
    {
        this.wordData = wordData;
        CheckSubtagMatches();
    }
    public bool InputIsCorrect(BubbleData wordData)
    {
        GetWordDataAndCheckSubtagMatches(wordData);
        return base.InputIsCorrect();
    }
}