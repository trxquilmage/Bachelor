using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Bubble : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler
{
    [HideInInspector] public TMP_Text relatedText;
    [HideInInspector] public BubbleData data;
    [HideInInspector] public BubbleEffects effects;
    [HideInInspector] public BubbleDoubleClickHandler doubleClickHandler;
    [HideInInspector] public BubblePlaceholderHandler placeholderHandler;
    [HideInInspector] public GameObject prefabReference;
    [HideInInspector] public GameObject wordParent;
    [HideInInspector] public BubbleOffset bubbleOffset;
    public GameObject vfxParent;
    [HideInInspector] public Case relatedCase;

    [HideInInspector] public bool wasDragged; //checks if the object was actually dragged
    [HideInInspector] public bool fadingOut = false;

    [HideInInspector] public TMP_WordInfo originalWordInfo;
    [HideInInspector] public TMP_Text originalText;
    [HideInInspector] public Vector3 wordSize;
    [HideInInspector] public float shakeTime = 0.5f;

    [HideInInspector] public PlayerInputManager piManager;
    [HideInInspector] public ReferenceManager refM;
    /// <summary>
    /// 0 = name, 1 = tags[0] (main tag), 2 = tags[1] (sub tag 1) ...
    /// </summary>
    public class TagObject
    {
        public List<Yarn.Value> allGivenValues;
    }

    public virtual void Awake()
    {
        refM = ReferenceManager.instance;
    }
    public virtual void Start()
    {
        piManager = PlayerInputManager.instance;
        CallEffect(0);
    }
    public virtual void Initialize(BubbleData inputData, WordInfo.Origin origin, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex)
    {
        Initialize(inputData, origin, wordInfo, firstAndLastWordIndex, out BubbleData data);
    }
    public virtual void Initialize(BubbleData inputData, WordInfo.Origin origin, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex, out BubbleData outData)
    {
        data.name = WordUtilities.CapitalizeAllWordsInString(inputData.name);
        data.tagInfo = inputData.tagInfo;
        data.tag = inputData.tagInfo[0];
        data.subtag = inputData.tagInfo[1];
        data.origin = origin;

        if (data.IsNotFromACase())
        {
            originalWordInfo = wordInfo;
            originalText = wordInfo.textComponent;
        }

        SetAdditionalValues();
        NameTextCorrectly();
        CheckIfLongWordAndSaveLineLength();

        outData = data;
    }
    public virtual void Initialize(BubbleData bubbleData, Vector2 firstAndLastWordIndex)
    {
        FillData(bubbleData);
        SetAdditionalValues();
        relatedText = transform.GetComponentInChildren<TMP_Text>();

        NameTextCorrectly();
        ScaleRectHighlighted(relatedText, GetComponentInChildren<Image>().rectTransform);
        wordSize = this.GetComponent<RectTransform>().sizeDelta;

        effects.ColorWordToTag();
    }
    protected void FillData(BubbleData bubbleData)
    {
        if (data == null)
            data = new BubbleData();

        data.name = bubbleData.name;
        data.tagInfo = bubbleData.tagInfo;
        data.tag = bubbleData.tag;
        data.subtag = bubbleData.subtag;
        data.origin = bubbleData.origin;
        data.lineLengths = bubbleData.lineLengths;
        data.isLongWord = bubbleData.isLongWord;
        data.isFavorite = bubbleData.isFavorite;
    }
    protected void SetAdditionalValues()
    {
        doubleClickHandler = new BubbleDoubleClickHandler(this);
        effects = new BubbleEffects(this);
        placeholderHandler = new BubblePlaceholderHandler(this);
    }
    public virtual void DroppedOverWordCase()
    {

    }
    public virtual void DroppedOverPlayerInput()
    {
    }
    public virtual void DroppedOverNothing()
    {
        CheckIfWasDroppedOverNPC();
        WordCaseManager.instance.ReloadContents();
        EffectUtilities.ReColorAllInteractableWords();
        WordClickManager.instance.DestroyCurrentWord();

        if (data.IsNotFromACase())
            WordCaseManager.instance.AutomaticOpenCase(false);
    }
    protected virtual void ParentToNewParent(Transform newParent, bool spawnWordReplacement, bool toCurrentWord)
    {
        transform.SetParent(newParent);

        if (toCurrentWord)
            WordClickManager.instance.currentWord = this.gameObject;
    }
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnBeginDragFunction();
        }
    }
    public void OnBeginDragFunction()
    {
        if (fadingOut)
            return;

        effects.DestroyStar();
        WordClickManager.instance.RemoveFromArray(WordClickManager.instance.activeWords, this.gameObject);

        if (IsCurrentlyInPrompt(out PromptBubble pB))
        {
            OnRemoveFromPromptBubble(pB);
        }

        if (data.IsNotFromACase())
        {
            WordCaseManager.instance.AutomaticOpenCase(true);
            WordCaseManager.instance.openTag = data.tag;
            UpdateImageAndScaleForAllLines();

            if (WordClickManager.instance.wordLastHighlighted != null &&
                this == WordClickManager.instance.wordLastHighlighted.GetComponent<Bubble>())
                WordClickManager.instance.SwitchFromHighlightedToCurrent();
        }
        else if (data.IsFromWordCase())
        {
            ParentToNewParent(ReferenceManager.instance.selectedWordParentAsk.transform, true, true);
        }
        effects.ColorWordToTag();
    }
    protected void ScaleRectHighlighted(TMP_Text text, RectTransform rTransform)
    {
        text.ForceMeshUpdate();
        Bounds bounds = text.textBounds; //we want the actual text size, not the size of the frame
        float width = bounds.size.x + bubbleOffset.offsetHighlighted.x;
        float height = bounds.size.y + bubbleOffset.offsetHighlighted.y;
        rTransform.sizeDelta = new Vector2(width, height);
    }
    protected void ScaleRectSelected(TMP_Text text, RectTransform rTransform)
    {
        text.ForceMeshUpdate();
        Bounds bounds = text.textBounds;
        float width = bounds.size.x + bubbleOffset.offsetSelected.x;
        float height = bounds.size.y + bubbleOffset.offsetSelected.y;

        RectTransform transformHoleImage = rTransform.parent.GetComponentInChildren<Image>().rectTransform;
        if (transformHoleImage != rTransform)
        {
            width = bounds.size.x + 15;
            transformHoleImage.sizeDelta = new Vector2(15, height);
        }

        rTransform.sizeDelta = new Vector2(width, height);
    }
    protected void NameTextCorrectly()
    {
        relatedText.text = data.name;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (fadingOut || eventData.button != PointerEventData.InputButton.Left)
            return;

        RectTransform rT = GetComponent<RectTransform>();
        rT.localPosition = WordUtilities.LocalScreenToCanvasPosition(WordClickManager.instance.GetMousePos());
        rT.localPosition -= wordSize / 2;
        wasDragged = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (fadingOut || eventData.button != PointerEventData.InputButton.Left || !wasDragged)
            return;
        OnDroppedReactToPosition();
        wasDragged = false;
    }
    public void OnDroppedReactToPosition()
    {
        WordClickManager clickM = WordClickManager.instance;
        if (!clickM.isActiveAndEnabled)
            return;

        if (clickM.mouseOverUIObject == "trashCan" && data.origin != WordInfo.Origin.Dialogue &&
            data.origin != WordInfo.Origin.Ask && data.origin != WordInfo.Origin.Environment)
            UIManager.instance.TrashAWord();
        else if (clickM.mouseOverUIObject == "wordCase")
            DroppedOverWordCase();
        else if (clickM.mouseOverUIObject == "playerInput")
            DroppedOverPlayerInput();
        else
            DroppedOverNothing();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // this cant be deleted bc for some reasons the other functions dont work without it
    }
    protected void CheckIfWasDroppedOverNPC()
    {
        Vector2 mousePos = WordClickManager.instance.GetMousePos();
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(mousePos));
        foreach (RaycastHit hit in hits)
        {
            if (!hit.transform.TryGetComponent<NPC>(out NPC npc))
                break;

            if (DialogueManager.instance.currentTarget != null && DialogueManager.instance.currentTarget != npc)
            {
                Debug.Log("cant be in a conversation with a different npc");
                break;
            }

            DialogueManager.instance.currentTarget = npc;
            piManager.OpenAskField();


            if (!refM.askPromptBubbleParent.transform.GetChild(0).TryGetComponent<PromptBubble>(out PromptBubble pB))
            {
                Debug.Log("no prompt was found?");
                break;
            }
            //force the prompt bubble to check, whether the word fits or not
            WordClickManager.instance.CheckPromptBubbleForCurrentWord(pB);
            DroppedOverPlayerInput();
            return;
        }
    }
    public void OnRemoveFromPromptBubble(PromptBubble pB)
    {
        ParentToNewParent(ReferenceManager.instance.selectedWordParentAsk.transform, true, true);
        pB.child = null;
    }
    public void OnEnterPromptBubble()
    {

    }
    protected void ShapeBubbleAccordingToSize(Vector2 firstAndLastWordIndex, bool isHighlighted)
    {
        if (data.isLongWord)
        {
            if (data.IsNotFromACase())
                FitBubbleShapeToSceneText((int)firstAndLastWordIndex.x, (int)firstAndLastWordIndex.y);
            else
                ShapeBubbleIntoCompactForm();
        }
        else
        {
            if (isHighlighted)
                ScaleRectHighlighted(relatedText, GetComponentInChildren<Image>().rectTransform);
            else
                UpdateLineImageAndScale();
            wordSize = this.GetComponent<RectTransform>().sizeDelta;
            StartCoroutine(effects.InstantiateStar());
        }
    }
    protected void FitBubbleShapeToSceneText(int firstWordInfoIndex, int lastWordInfoIndex)
    {
        relatedText.ForceMeshUpdate();
        Vector2[] sourceLineLengths = GetLineLengths(firstWordInfoIndex, lastWordInfoIndex, out TMP_WordInfo[] lineStarts);
        wordParent.GetComponent<VerticalLayoutGroup>().enabled = false;
        GameObject child = null;

        int j = 0;
        foreach (Vector2 startEnd in sourceLineLengths)
        {
            Vector3 position = WordUtilities.GetWordPosition(originalText, lineStarts[j]);
            child = InstantiateNewLineHighlighted(position);
            relatedText = child.GetComponentInChildren<TMP_Text>();

            FillLineWithText(startEnd);
            relatedText.rectTransform.sizeDelta = new Vector2(1000, relatedText.rectTransform.sizeDelta.y);
            ScaleRectHighlighted(relatedText, relatedText.transform.parent.GetComponent<RectTransform>());

            j++;
        }
        DestroyFirstChild();
        EffectUtilities.ColorAllChildrenOfAnObject(wordParent, data.tag, data.subtag);
    }
    protected void ShapeBubbleIntoCompactForm()
    {
        relatedText.ForceMeshUpdate();

        string sourceText = data.name;

        GameObject child = null;
        DestroyEverythingUnderWordParent(false);
        wordParent.GetComponent<VerticalLayoutGroup>().enabled = true;

        int j = 0;
        foreach (Vector2 startEnd in data.lineLengths)
        {
            child = InstantiateNewLineSelected();

            relatedText = child.GetComponentInChildren<TMP_Text>();

            Image relatedImage = GetRelatedImage(child);

            FillLineWithText(sourceText, startEnd);
            ScaleRectSelected(relatedText, relatedImage.rectTransform);
            AddDuctTapeToLowerChildren(relatedImage, j);
            j++;
        }
        StartCoroutine(effects.InstantiateStar());
    }
    protected void UpdateBubbleIntoCompactForm()
    {
        relatedText.text = data.name; //Set the main text to full text again
        relatedText.ForceMeshUpdate();
        string sourceText = data.name;

        DestroyEverythingUnderWordParent(true);

        GameObject child = null;
        wordParent.GetComponent<VerticalLayoutGroup>().enabled = true;

        int j = 0;

        foreach (Vector2 startEnd in data.lineLengths)
        {
            child = InstantiateNewLineSelected();
            relatedText = child.GetComponentInChildren<TMP_Text>();
            Image relatedImage = GetRelatedImage(child);

            FillLineWithText(sourceText, startEnd);
            ScaleRectSelected(relatedText, relatedImage.rectTransform);
            AddDuctTapeToLowerChildren(relatedImage, j);
            j++;
        }
        DestroyFirstChild();
        EffectUtilities.ColorAllChildrenOfAnObject(wordParent, data.tag, data.subtag);
    }
    protected Image GetRelatedImage(GameObject imageParent)
    {
        foreach (Image image in imageParent.GetComponentsInChildren<Image>())
            if (image.tag == "WordCard")
                return image;
        return imageParent.GetComponentInChildren<Image>();
    }
    protected void UpdateImageAndScaleForAllLines()
    {
        if (data.isLongWord)
            UpdateBubbleIntoCompactForm();
        else
            UpdateLineImageAndScale();

        ScaleAllParentsToTheirCorrectSizes();
        MoveLinesAccordingToOffset(false);
    }
    protected void UpdateLineImageAndScale()
    {
        Image wordCard = GetRelatedImage(this.gameObject);
        RectTransform firstChild = wordCard.GetComponent<RectTransform>();

        wordCard.sprite = ReferenceManager.instance.wordSelectedSprite;
        ScaleRectSelected(relatedText, firstChild);
    }
    protected void OnDestroy()
    {
        placeholderHandler.RemoveReplacement();
    }

    #region LINE LENGTHS
    /// <summary>
    /// Gives back an array of Vector2s, each containing the index of the first and last letter of each line in the text
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    protected Vector2[] GetLineLengths()
    {
        relatedText.ForceMeshUpdate();

        List<Vector2> firstAndLastCharIndexPerLine = new List<Vector2>() { Vector2.zero };
        TMP_TextInfo info = relatedText.textInfo;
        TMP_WordInfo[] words = info.wordInfo;
        TMP_CharacterInfo[] characters = info.characterInfo;
        TMP_WordInfo word;

        float lastHeight = characters[0].vertex_BL.position.y;
        float currentHeight;
        int lastCharacterIndex = words[0].lastCharacterIndex;
        int firstCharacterIndex;

        for (int i = 0; i < info.wordCount; i++)
        {
            word = words[i];
            firstCharacterIndex = word.firstCharacterIndex;
            currentHeight = characters[firstCharacterIndex].vertex_BL.position.y;

            if (currentHeight + 10 < lastHeight) //heights can vary a bit, so there is a buffer here
            {
                EndTheCurrentLine(ref firstAndLastCharIndexPerLine, lastCharacterIndex);
                StartANewLine(ref firstAndLastCharIndexPerLine, firstCharacterIndex);
            }

            lastCharacterIndex = word.lastCharacterIndex;
            lastHeight = currentHeight;
        }
        EndTheCurrentLine(ref firstAndLastCharIndexPerLine, lastCharacterIndex);
        return firstAndLastCharIndexPerLine.ToArray();
    }
    protected Vector2[] GetLineLengths(int firstWord, int lastWord, out TMP_WordInfo[] firstWordSelectedPerLine)
    {
        originalText.ForceMeshUpdate();

        TMP_TextInfo info = originalText.textInfo;
        TMP_WordInfo[] words = info.wordInfo;
        TMP_CharacterInfo[] characters = info.characterInfo;
        TMP_WordInfo word;

        List<TMP_WordInfo> firstWordPerLine = new List<TMP_WordInfo>();
        int firstCharacterIndex = words[firstWord].firstCharacterIndex;
        int lastCharacterIndex = words[firstWord].lastCharacterIndex;

        List<Vector2> firstAndLastCharIndexPerLine = new List<Vector2>() { new Vector2(firstCharacterIndex, 0) };

        float lastHeight = characters[firstCharacterIndex].vertex_BL.position.y;
        float currentHeight;
        firstWordPerLine.Add(words[firstWord]);

        for (int i = firstWord; i <= lastWord; i++)
        {
            word = originalText.textInfo.wordInfo[i];
            firstCharacterIndex = word.firstCharacterIndex;
            currentHeight = characters[firstCharacterIndex].vertex_BL.position.y;

            if (currentHeight + 10 < lastHeight) //heights can vary a bit, so there is a buffer here
            {
                EndTheCurrentLine(ref firstAndLastCharIndexPerLine, lastCharacterIndex);
                StartANewLine(ref firstAndLastCharIndexPerLine, firstCharacterIndex);
                firstWordPerLine.Add(word);
            }
            lastCharacterIndex = word.lastCharacterIndex;
            lastHeight = currentHeight;
        }

        EndTheCurrentLine(ref firstAndLastCharIndexPerLine, lastCharacterIndex);
        firstWordSelectedPerLine = firstWordPerLine.ToArray();
        return firstAndLastCharIndexPerLine.ToArray();
    }
    protected void EndTheCurrentLine(ref List<Vector2> firstAndLastCharIndexPerLine, int lastCharacterIndex)
    {
        int numberOfLines = firstAndLastCharIndexPerLine.Count - 1;
        firstAndLastCharIndexPerLine[numberOfLines] += new Vector2(0, lastCharacterIndex);
    }
    protected void StartANewLine(ref List<Vector2> firstAndLastCharIndexPerLine, int firstCharacterIndex)
    {
        firstAndLastCharIndexPerLine.Add(new Vector2(firstCharacterIndex, 0));
    }
    #endregion
    /// <summary>
    /// Call one of the VFX on the bubble: 0 -> spawn, 1 -> delete
    /// </summary>
    /// <param name="i"></param>
    public void CallEffect(int i)
    {
        if (vfxParent != null)
        {
            VisualEffect chosenVFX = vfxParent.GetComponentsInChildren<VisualEffect>()[i];
            if (i != 0)
            {
                chosenVFX.gameObject.transform.SetParent(refM.dialogueCanvas.transform);
                chosenVFX.gameObject.transform.SetAsLastSibling();
            }

            //set effect width to bubble width
            chosenVFX.SetFloat("width", wordParent.GetComponentInChildren<Image>().rectTransform.sizeDelta.x);
            if (i != 1)
            {
                //set effect color to bubble color
                Color color = wordParent.GetComponentInChildren<Image>().color;
                chosenVFX.SetVector3("color", new Vector3(color.r, color.g, color.b));
            }

            UIManager.instance.PlayVFX(chosenVFX);
            if (i != 0)
                StartCoroutine(UIManager.instance.DestroyVFX(chosenVFX));
        }
    }
    protected void CheckIfLongWordAndSaveLineLength()
    {
        data.lineLengths = GetLineLengths();
        data.isLongWord = (data.name.Trim().Split(@" "[0]).Length > 1);
    }
    protected void DestroyEverythingUnderWordParent(bool ignoreFirstChild)
    {
        int limit = ignoreFirstChild ? 1 : 0;
        for (int i = wordParent.transform.childCount - 1; i >= limit; i--)
            Destroy(wordParent.transform.GetChild(i).gameObject);
    }
    protected GameObject InstantiateNewLineHighlighted(Vector3 atPosition)
    {
        GameObject child;
        child = GameObject.Instantiate(ReferenceManager.instance.wordHighlightedPrefab, wordParent.transform, false);
        child.transform.localPosition = atPosition - GetComponent<RectTransform>().localPosition;
        return child;
    }
    protected GameObject InstantiateNewLineSelected()
    {
        return GameObject.Instantiate(ReferenceManager.instance.wordSelectedPrefab, wordParent.transform, false);
    }
    protected void FillLineWithText(Vector2 startEnd)
    {
        string line = "";
        for (int i = (int)startEnd.x; i <= (int)startEnd.y; i++)
        {
            line = line + originalText.textInfo.characterInfo[i].character;
        }
        line = WordUtilities.CapitalizeAllWordsInString(line);
        relatedText.text = line;
    }
    protected void FillLineWithText(string textToCopyFrom, Vector2 startEnd)
    {
        string line = "";
        for (int i = (int)startEnd.x; i <= (int)startEnd.y; i++)
        {
            if (i < textToCopyFrom.Length)
                line = line + textToCopyFrom[i];
        }
        line = WordUtilities.CapitalizeAllWordsInString(line);
        relatedText.text = line;
    }
    protected void ScaleAllParentsToTheirCorrectSizes()
    {
        StartCoroutine(ScaleAllParents());
    }
    protected IEnumerator ScaleAllParents()
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        yield return delay;

        Image currentImage;
        RectTransform currentParent;
        RectTransform mainParent = GetComponent<RectTransform>();
        Vector2 sizeDelta = new Vector2(0, 0);

        foreach (TMP_Text line in GetComponentsInChildren<TMP_Text>())
        {
            currentImage = line.transform.parent.GetComponent<Image>();
            currentParent = currentImage.transform.parent.GetComponent<RectTransform>();
            currentParent.sizeDelta = currentImage.rectTransform.sizeDelta;

            sizeDelta += currentImage.rectTransform.sizeDelta;
            sizeDelta.x = currentImage.rectTransform.sizeDelta.x;
        }
        mainParent.sizeDelta = sizeDelta;

        foreach (VerticalLayoutGroup lG in wordParent.GetComponentsInChildren<VerticalLayoutGroup>())
            lG.SetLayoutVertical();

        if (wordParent.transform.parent.TryGetComponent<VerticalLayoutGroup>(out VerticalLayoutGroup vLG))
            vLG.SetLayoutVertical();

    }
    protected void MoveLinesAccordingToOffset(bool isHighlighted)
    {
        float yOffset = (data.isLongWord) ? 0.5f : 0.5f; //for some reason short words just scale differently
        Vector3 highlighted = Vector3.Scale((Vector3)bubbleOffset.offsetHighlighted, new Vector3(0.5f, yOffset, 0));
        Vector3 selected = Vector3.Scale((Vector3)bubbleOffset.offsetSelected, new Vector3(0.5f, yOffset, 0));
        transform.localPosition -= (isHighlighted) ? highlighted : selected;
    }
    protected void DestroyFirstChild()
    {
        Destroy(wordParent.transform.GetChild(0).gameObject);
    }
    protected void AddDuctTapeToLowerChildren(Image image, int round)
    {
        if (round > 0)
        {
            GameObject tape = Instantiate(refM.tapePrefab, image.transform, false);
            tape.transform.localPosition += new Vector3(15, 20, 0);
            tape = Instantiate(refM.tapePrefab, image.transform, false);
            tape.transform.localPosition += new Vector3(40, 18, 0);
            tape.transform.localScale = Vector3.Scale(tape.transform.localScale, new Vector3(1, -1, 1));
        }
    }
    public virtual Vector2 GetCaseTargetPosition()
    {
        return Vector2.zero;
    }
    public bool IsCurrentlyInPrompt()
    {
        return transform.parent.TryGetComponent<PromptBubble>(out PromptBubble pB);
    }
    public bool IsCurrentlyInPrompt(out PromptBubble pB)
    {
        return transform.parent.TryGetComponent<PromptBubble>(out pB);
    }
}
public class BubbleData
{
    public string name
    {
        get { return Name; }
        set
        {
            Name = value;
            UpdateBubbleData();
        }
    }
    public string[] tagInfo
    {
        get { return TagInfo; }
        set
        {
            TagInfo = value;
            UpdateBubbleData();
        }
    }
    public string tag
    {
        get { return Tag; }
        set
        {
            Tag = value;
            UpdateBubbleData();
        }
    }
    public string subtag
    {
        get { return Subtag; }
        set
        {
            Subtag = value;
            UpdateBubbleData();
        }
    }
    public WordInfo.Origin origin
    {
        get { return Origin; }
        set
        {
            Origin = value;
            UpdateBubbleData();
        }
    }
    public Vector2[] lineLengths
    {
        get { return LineLengths; }
        set
        {
            LineLengths = value;
            UpdateBubbleData();
        }
    }
    public bool isLongWord
    {
        get { return IsLongWord; }
        set
        {
            IsLongWord = value;
            UpdateBubbleData();
        }
    }
    public bool permanentWord
    {
        get { return PermanentWord; }
        set
        {
            PermanentWord = value;
            UpdateBubbleData();
        }
    }
    public bool isFavorite
    {
        get { return IsFavorite; }
        set
        {
            IsFavorite = value;
            UpdateBubbleData();
        }
    }

    string Name;
    string[] TagInfo;
    string Tag;
    string Subtag;
    WordInfo.Origin Origin;
    Vector2[] LineLengths;
    bool IsLongWord;
    bool PermanentWord;
    bool IsFavorite;

    public virtual void UpdateBubbleData()
    {
    }
    public bool IsNotFromACase()
    {
        return origin == WordInfo.Origin.Dialogue || origin == WordInfo.Origin.Environment || origin == WordInfo.Origin.Ask;
    }
    public bool IsFromWordCase()
    {
        return origin == WordInfo.Origin.WordCase;
    }
}
public class BubbleEffects
{
    ReferenceManager refM;
    Bubble relatedBubble;
    BubbleData data;
    float floatTime = 0.4f;
    float shakeTime = 0.5f;
    GameObject star;
    bool currentlyRotatingAsFeedback;

    public BubbleEffects(Bubble relatedBubble)
    {
        refM = ReferenceManager.instance;
        this.relatedBubble = relatedBubble;
        data = relatedBubble.data;
        star = null;
    }
    public void ColorWordGrey()
    {
        if (relatedBubble.placeholderHandler.isPlaceholder)
            return;

        Color grey = Color.Lerp(refM.greyedOutColor, WordUtilities.MatchColorToTag(relatedBubble.data.tag, relatedBubble.data.subtag), 0.2f);
        grey.a = 0.7f;
        EffectUtilities.ColorAllChildrenOfAnObject(relatedBubble.gameObject, grey);
    }
    public void ColorWordToTag()
    {

        if (relatedBubble.placeholderHandler.isPlaceholder)
            return;
        EffectUtilities.ColorAllChildrenOfAnObject(relatedBubble.gameObject, relatedBubble.data.tag, relatedBubble.data.subtag);
    }
    public void SetWordToColorAndTransparency(Color color, float alpha)
    {
        color.a = alpha;
        EffectUtilities.ColorAllChildrenOfAnObject(relatedBubble.gameObject, color);
    }
    public GameObject SpawnDuplicateOnTopOnThisBubble()
    {
        GameObject duplicate = GameObject.Instantiate(relatedBubble.gameObject, relatedBubble.transform.position, relatedBubble.transform.rotation);
        duplicate.transform.SetParent(ReferenceManager.instance.selectedWordParentAsk.transform, false);
        duplicate.transform.position = relatedBubble.transform.position;
        duplicate.transform.rotation = relatedBubble.transform.rotation;
        return duplicate;
    }
    public void MakeBubbleInvisible(bool makeInvisible)
    {
        Color color;
        foreach (Image img in relatedBubble.GetComponentsInChildren<Image>())
        {
            if (makeInvisible)
            {
                color = img.color;
                color.a = 0;
            }
            else
            {
                color = img.color;
                color.a = 1;
            }
            img.color = color;
        }
        foreach (TMP_Text text in relatedBubble.GetComponentsInChildren<TMP_Text>())
        {
            if (makeInvisible)
            {
                color = text.color;
                color.a = 0;
            }
            else
            {
                color = text.color;
                color.a = 1;
            }
            text.color = color;
        }
    }
    public IEnumerator AnimateMovement(RefBool isDone, Vector2 targetPos)
    {
        relatedBubble.fadingOut = false;
        relatedBubble.OnBeginDragFunction();
        relatedBubble.fadingOut = true;
        WaitForEndOfFrame delay = new WaitForEndOfFrame();

        RectTransform rT = relatedBubble.GetComponent<RectTransform>();
        Vector2 startPos = rT.localPosition;

        float timer = 0;
        float t;

        while (timer < floatTime)
        {
            timer += Time.deltaTime;
            t = WordUtilities.Remap(timer, 0, floatTime, 0, 1);
            rT.localPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return delay;
        }
        isDone.refBool = true;
    }
    public void DestroyStar()
    {
        if (star != null)
            Object.Destroy(star.gameObject);
    }
    public IEnumerator InstantiateStar()
    {
        //called after one frame, because otherwise the placement due to acessed Text can be wrong
        yield return new WaitForEndOfFrame();
        if (data.IsNotFromACase() || relatedBubble.placeholderHandler.isPlaceholder)
            yield break;

        refM = ReferenceManager.instance;
        star = Object.Instantiate(refM.starPrefab, relatedBubble.GetComponentInChildren<TMP_Text>().transform, false);
    }
    public IEnumerator ShakeBubbleAsFeedback(bool inACase, bool isDuplicate)
    {
        GameObject duplicate = null;
        if (inACase)
        {
            duplicate = SpawnDuplicateOnTopOnThisBubble();
            relatedBubble.StartCoroutine(duplicate.GetComponent<Bubble>().effects.ShakeBubbleAsFeedback(false, true));
            MakeBubbleInvisible(true);
        }
        relatedBubble.fadingOut = true;

        WaitForEndOfFrame delay = new WaitForEndOfFrame();

        if (!inACase && !isDuplicate)
        {
            WordCaseManager.instance.AutomaticOpenCase(false);
        }

        RectTransform rT = relatedBubble.GetComponent<RectTransform>();
        Vector2 startPos = rT.localPosition;
        Vector2 targetPosRight = startPos + Vector2.right * 4 + Vector2.up * 3;
        Vector2 targetPosLeft = startPos + Vector2.left * 4 + Vector2.down * 3;

        float t;
        float timer = 0;
        while (timer < shakeTime)
        {
            timer += Time.deltaTime;
            if (timer < shakeTime / 4)
                t = WordUtilities.Remap(timer, 0, shakeTime / 4, 0.5f, 1);
            else if (timer < shakeTime * 3 / 4)
                t = WordUtilities.Remap(timer, shakeTime / 4, shakeTime * 3 / 4, 1, 0);
            else
                t = WordUtilities.Remap(timer, shakeTime * 3 / 4, shakeTime, 0, 0.5f);
            rT.localPosition = Vector2.Lerp(targetPosLeft, targetPosRight, t);
            yield return delay;
        }
        relatedBubble.fadingOut = false;

        if (!inACase && !isDuplicate)
            WordClickManager.instance.SwitchFromCurrentToHighlight();

        if (inACase)
        {
            if (duplicate != null)
                Object.Destroy(duplicate);
            relatedBubble.effects.MakeBubbleInvisible(false);
        }
    }
    public void ShakeBubbleAsFeedbackRotated()
    {
        if (!currentlyRotatingAsFeedback)
            relatedBubble.StartCoroutine(ShakeBubbleRotated());
    }
    protected IEnumerator ShakeBubbleRotated()
    {
        currentlyRotatingAsFeedback = true;
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        RectTransform rT = relatedBubble.GetComponent<RectTransform>();
        Vector3 startRotation = rT.localEulerAngles;
        Vector3 targetRotRight = startRotation + (Vector3.forward * 5);
        Vector3 targetRotLeft = startRotation + (Vector3.forward * -5);

        float t;
        float timer = 0;
        while (timer < shakeTime)
        {
            timer += Time.deltaTime;
            if (timer < shakeTime / 6)
                t = WordUtilities.Remap(timer, 0, shakeTime / 6, 0.5f, 1);
            else if (timer < shakeTime * 3 / 6)
                t = WordUtilities.Remap(timer, shakeTime / 6, shakeTime * 3 / 6, 1, 0);
            else if (timer < shakeTime * 5 / 6)
                t = WordUtilities.Remap(timer, shakeTime * 3 / 6, shakeTime * 5 / 6, 0, 1);
            else
                t = WordUtilities.Remap(timer, shakeTime * 5 / 6, shakeTime, 1, 0.5f);

            rT.localEulerAngles = Vector3.Lerp(targetRotLeft, targetRotRight, t);
            yield return delay;
        }
        rT.localEulerAngles = startRotation;
        currentlyRotatingAsFeedback = false;
    }
}
public class BubbleDoubleClickHandler
{
    ReferenceManager refM;
    PlayerInputManager piManager;
    Bubble relatedBubble;
    BubbleData data;
    RefBool movementDone;

    public BubbleDoubleClickHandler(Bubble relatedBubble)
    {
        this.relatedBubble = relatedBubble;
        data = relatedBubble.data;
        refM = ReferenceManager.instance;
        piManager = PlayerInputManager.instance;
        movementDone = new RefBool() { refBool = false };
    }
    public void OnDoubleClicked()
    {
        if (relatedBubble.IsCurrentlyInPrompt())
            DoubleClickedPromptBubble();
        else if (data.IsNotFromACase())
            DoubleClickedOnDialogue();
        else
            DoubleClickedOnCase();
    }
    void DoubleClickedOnDialogue()
    {
        WordCaseManager.instance.openTag = data.tag;
        bool fits = false;

        if (WordCaseManager.instance.CheckIfCanSaveBubble(data.name, out int index, out bool bubbleIsAlreadyInList, out bool caseIsFull))
            fits = true;
        else if (bubbleIsAlreadyInList)
            UIManager.instance.BlendInUI(refM.warningWordAlreadyInList, 3);
        else if (caseIsFull)
            UIManager.instance.BlendInUI(refM.warningCaseFull, 3);

        if (fits)
            AnimateMovementIntoCase();
        else
        {
            relatedBubble.StartCoroutine(relatedBubble.effects.ShakeBubbleAsFeedback(false, false));
            Color color = relatedBubble.GetComponentInChildren<Image>().color;
            relatedBubble.StartCoroutine(EffectUtilities.ColorObjectAndChildrenInGradient(relatedBubble.gameObject, new Color[] { color, Color.red, Color.red, Color.red, color }, 0.6f));
        }
    }
    void DoubleClickedOnCase()
    {
        if (piManager.CheckIfThereArePromptBubblesWithTag(data.tag, out PromptBubble pB))
            AnimateMovementIntoPrompt(pB);
        else
            relatedBubble.StartCoroutine(relatedBubble.effects.ShakeBubbleAsFeedback(true, false));
    }
    void DoubleClickedPromptBubble()
    {

        if (relatedBubble.data.IsNotFromACase())
            RemoveCurrentWord();
        else
            AnimateMovementBackToCase();
    }
    void RemoveCurrentWord()
    {
        relatedBubble.OnBeginDragFunction();
        relatedBubble.DroppedOverNothing();
    }
    void AnimateMovementIntoCase()
    {
        Vector2 targetPos = relatedBubble.GetCaseTargetPosition();
        relatedBubble.relatedCase.AutomaticOpenCase(true);
        relatedBubble.StartCoroutine(relatedBubble.effects.AnimateMovement(movementDone, targetPos));
        relatedBubble.StartCoroutine(AfterMovementToCase(movementDone));
    }
    void AnimateMovementIntoPrompt(PromptBubble pB)
    {
        relatedBubble.transform.SetParent(refM.selectedWordParentAsk.transform);
        Vector2 targetPos = pB.transform.localPosition;
        relatedBubble.StartCoroutine(relatedBubble.effects.AnimateMovement(movementDone, targetPos));
        relatedBubble.StartCoroutine(AfterMovementToPromptBubble(movementDone, pB));
    }
    public void AnimateMovementBackToCase()
    {
        Vector2 targetPos = relatedBubble.GetCaseTargetPosition();
        relatedBubble.StartCoroutine(relatedBubble.effects.AnimateMovement(movementDone, targetPos));
        relatedBubble.StartCoroutine(AfterMovementBackToCase(movementDone));
    }
    IEnumerator AfterMovementToCase(RefBool isDone)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (!isDone.refBool)
            yield return delay;
        movementDone.refBool = false;

        relatedBubble.DroppedOverWordCase();
        relatedBubble.relatedCase.AutomaticOpenCase(false);
    }
    IEnumerator AfterMovementToPromptBubble(RefBool isDone, PromptBubble pB)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (!isDone.refBool)
            yield return delay;
        movementDone.refBool = false;

        WordUtilities.ParentBubbleToPrompt(pB.gameObject, relatedBubble.gameObject);
        relatedBubble.OnEnterPromptBubble();
    }
    IEnumerator AfterMovementBackToCase(RefBool isDone)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (!isDone.refBool)
            yield return delay;
        movementDone.refBool = false;

        Object.Destroy(relatedBubble.gameObject);
        WordCaseManager.instance.ReloadContents();
        EffectUtilities.ReColorAllInteractableWords();
    }
}

public class BubblePlaceholderHandler
{
    Bubble relatedBubble;
    GameObject bubbleReplacement;
    public bool isPlaceholder;
    public BubblePlaceholderHandler(Bubble bubble)
    {
        relatedBubble = bubble;
        isPlaceholder = false;
    }

    public void SpawnReplacement()
    {
        if (bubbleReplacement != null)
            RemoveReplacement();

        bubbleReplacement = WordCaseManager.instance.SpawnBubbleInCase(relatedBubble.data);
        bubbleReplacement.GetComponent<Word>().placeholderHandler.isPlaceholder = true;

        bubbleReplacement.GetComponent<Word>().effects.SetWordToColorAndTransparency(ReferenceManager.instance.greyedOutColor, 0.3f);
    }

    public void RemoveReplacement()
    {
        if (bubbleReplacement == null)
            return;

        Object.Destroy(bubbleReplacement);
        bubbleReplacement = null;
    }
}