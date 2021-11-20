using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pinwheel.UIEffects;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class Bubble : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler
{
    public TMP_Text relatedText;
    public BubbleData data;
    public GameObject prefabReference;
    public GameObject wordParent;

    public bool wasDragged; //checks if the object was actually dragged
    public bool fadingOut;

    public TMP_WordInfo originalWordInfo;
    public TMP_Text originalText;
    public Vector3 wordSize;
    public RefBool movementDone;
    public float floatTime = 0.4f;
    public float shakeTime = 0.5f;

    public PlayerInputManager piManager;
    public ReferenceManager refM;
    /// <summary>
    /// 0 = name, 1 = tags[0] (main tag), 2 = tags[1] (sub tag 1) ...
    /// </summary>
    public struct TagObject
    {
        public List<Yarn.Value> allGivenValues;
    }

    #region VIRTUAL
    public virtual void Start()
    {
        movementDone = new RefBool() { refBool = false };
        piManager = PlayerInputManager.instance;
        refM = ReferenceManager.instance;
    }
    /// <summary>
    /// Call this, when creating a Word. If you dont have a Word Info, create one and set "hasWordInfo" to false
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tags"></param>
    public virtual void Initialize(string name, string[] tags, WordInfo.Origin origin, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex)
    {
        Initialize(name, tags, origin, wordInfo, firstAndLastWordIndex, out BubbleData data);
    }
    public virtual void Initialize(string name, string[] tags, WordInfo.Origin origin, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex, out BubbleData data)
    {
        data = new BubbleData();
        //capitalize name
        data.name = WordUtilities.CapitalizeAllWordsInString(name);
        data.tagInfo = tags;
        data.tag = tags[0];
        data.origin = origin;

        //save the location in the text this word came from (if this word came from the Text)
        if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
        {
            originalWordInfo = wordInfo;
            originalText = wordInfo.textComponent;
        }

        // Set the bubble to the correct text
        relatedText = transform.GetComponentInChildren<TMP_Text>();
        relatedText.text = data.name;

        //Scale the bubble correctly
        ScaleRect(relatedText, GetComponent<RectTransform>());
        wordSize = this.GetComponent<RectTransform>().sizeDelta;

        //Color the bubble correctly
        EffectUtilities.ColorTag(wordParent, data.tag);

        //Get how many lines the word would use
        data.lineLengths = GetLineLengths();

        // is there more than one word?
        data.isLongWord = (data.name.Trim().Split(@" "[0]).Length > 1);
    }
    /// <summary>
    /// The bubble was dragged onto the word case and dropped
    /// </summary>
    public virtual void IsOverWordCase()
    {

    }
    /// <summary>
    /// The bubble was dragged onto the questLog and dropped
    /// </summary>
    public virtual void IsOverQuestLog()
    {
    }
    /// <summary>
    /// The bubble was dragged onto a prompt case and dropped
    /// </summary>
    public virtual void IsOverPlayerInput()
    {
    }
    /// <summary>
    /// The bubble was dragged onto nothing and dropped
    /// </summary>
    public virtual void IsOverNothing()
    {
        CheckIfOverCharacter();
        if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
        {
            //close the case & Delete the UI word
            WordCaseManager.instance.AutomaticOpenCase(false);
            WordClickManager.instance.DestroyCurrentWord();
        }
    }
    /// <summary>
    /// The bubble was dragged onto a quest in the quest case and dropped
    /// </summary>
    public virtual void IsOverQuestCase()
    {
    }
    /// <summary>
    /// Takes a word and unparents it from its current parent, to the new parent & Spawns a word replacement
    /// </summary>
    /// <param name="newParent"></param>
    /// <param name="isWordCase"></param>
    public virtual void Unparent(Transform newParent, bool spawnWordReplacement, bool toCurrentWord)
    {
        transform.SetParent(newParent);

        if (toCurrentWord)
            WordClickManager.instance.currentWord = this.gameObject;
    }
    #endregion

    #region NOT VIRTUAL

    /// <summary>
    /// Scale the picked up word, so that the rect of the background fits the word in the center
    /// </summary>
    public void ScaleRect(TMP_Text text, RectTransform rTransform)
    {
        text.ForceMeshUpdate();
        Bounds bounds = text.textBounds;
        float width = bounds.size.x;
        width = width + 4;
        rTransform.sizeDelta = new Vector2(width, rTransform.sizeDelta.y);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!fadingOut)
        {
            //drag the object through the scene
            RectTransform rT = GetComponent<RectTransform>();
            rT.localPosition = WordUtilities.LocalScreenToCanvasPosition(WordClickManager.instance.GetMousePos());
            rT.localPosition -= wordSize / 2;
            wasDragged = true;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!fadingOut)
        {
            // if mouse was dragging the object and now releases it
            if (eventData.button == PointerEventData.InputButton.Left && wasDragged)
            {
                WordClickManager clickM = WordClickManager.instance;
                // check where it is released
                //if it was dragged into the case, save it
                if (clickM.isActiveAndEnabled)
                {
                    if (clickM.mouseOverUIObject == "trashCan")
                    {
                        UIManager.instance.TrashAWord();
                    }
                    else if (clickM.mouseOverUIObject == "wordCase")
                    {
                        IsOverWordCase();
                    }
                    else if (clickM.mouseOverUIObject == "questLog")
                    {
                        IsOverQuestLog();
                    }
                    else if (clickM.mouseOverUIObject == "questCase")
                    {
                        IsOverQuestCase();
                    }
                    // if it was dragged onto a prompt, react
                    else if (clickM.mouseOverUIObject == "playerInput")
                    {
                        IsOverPlayerInput();
                    }
                    else
                    {
                        IsOverNothing();
                    }
                }

            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // this cant be deleted bc for some reasons the other functions dont work without it
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!fadingOut)
        {
            WordClickManager.instance.RemoveFromArray(WordClickManager.instance.activeWords, this.gameObject);

            if (transform.parent.TryGetComponent<PromptBubble>(out PromptBubble pB)) //if currently attached to a prompt bubble
            {
                pB.child = null;
            }

            // if the word is being dragged out of the dialogue
            if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
            {
                if (data.tag != ReferenceManager.instance.wordTags[ReferenceManager.instance.questTagIndex].name)
                {
                    WordCaseManager.instance.AutomaticOpenCase(true);
                    WordCaseManager.instance.openTag = data.tag;
                }
                else if (data.tag == ReferenceManager.instance.wordTags[ReferenceManager.instance.questTagIndex].name)
                {
                    QuestManager.instance.AutomaticOpenCase(true);
                    UpdateToBubbleShape();
                }
                WordClickManager.instance.SwitchFromHighlightedToCurrent();

            }
            else if (data.origin == WordInfo.Origin.WordCase)
            {
                Unparent(ReferenceManager.instance.selectedWordParentAsk.transform, true, true);
            }
            else if (data.origin == WordInfo.Origin.QuestLog)
            {
                Unparent(ReferenceManager.instance.selectedWordParentAsk.transform, true, true);
            }
        }
    }
    /// <summary>
    /// If the word was over nothing, check, if it was above a character and open the ask menu to the according page
    /// </summary>
    public void CheckIfOverCharacter()
    {
        //Check, if the word is above a character
        Vector2 mousePos = WordClickManager.instance.GetMousePos();
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(mousePos));
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.TryGetComponent<NPC>(out NPC npc))
            {
                if (DialogueManager.instance.currentTarget == null || DialogueManager.instance.currentTarget == npc)
                {
                    DialogueManager.instance.currentTarget = npc;
                    piManager.AskButton();
                    //force the prompt bubble to check, whether the word fits or not
                    if (refM.askPromptBubbleParent.transform.GetChild(0).TryGetComponent<PromptBubble>(out PromptBubble pB))
                    {
                        WordClickManager.instance.CheckPromptBubbleForCurrentWord(pB);
                        IsOverPlayerInput();
                        return;
                    }
                    else
                        Debug.Log("no prompt was found?");
                }
                else
                    Debug.Log("cant be in a conversation with a different npc");
            }
        }
    }
    /// <summary>
    /// Takes a long word and fits it above the text it is portraying
    /// </summary>
    public void FitToText(TMP_Text editableText, TMP_Text sourceText, int firstWordInfoIndex, int lastWordInfoIndex)
    {
        Color tagColor = WordUtilities.MatchColorToTag(data.tag);
        editableText.ForceMeshUpdate();
        Vector2[] sourceLineLengths = GetLineLengths(sourceText, firstWordInfoIndex, lastWordInfoIndex, out TMP_WordInfo[] lineStarts);

        //Create a child of the Word, that is also a bubble and fill the text with the correlating text
        GameObject child;
        wordParent.GetComponent<VerticalLayoutGroup>().enabled = false;

        int j = 0;
        foreach (Vector2 startEnd in sourceLineLengths)
        {
            Vector3 position = WordUtilities.GetWordPosition(sourceText, lineStarts[j]);
            child = GameObject.Instantiate(ReferenceManager.instance.selectedWordPrefab, wordParent.transform, false);// false fixes a scaling issue
            child.transform.localPosition = position - GetComponent<RectTransform>().localPosition; //- parent transform, because of the new canvas scaling
            editableText = child.GetComponentInChildren<TMP_Text>();

            string line = "";
            for (int i = (int)startEnd.x; i <= (int)startEnd.y; i++)
            {
                line = line + sourceText.textInfo.characterInfo[i].character;
            }
            line = WordUtilities.CapitalizeAllWordsInString(line);
            editableText.text = line;

            // Scale the text boxes
            child.GetComponentsInChildren<RectTransform>()[1].sizeDelta = new Vector2(1000, child.GetComponentsInChildren<RectTransform>()[1].sizeDelta.y);
            ScaleRect(editableText, child.GetComponent<RectTransform>());
            // color the boxes
            child.GetComponent<Image>().color = tagColor;
            j++;
        }
        if (this is Quest)
            GetComponent<QuestCase>().ChangeAddedParentScale(true);
        //remove the iamge and text from the original bubble
        Destroy(wordParent.GetComponent<UIEffectStack>());
        Destroy(wordParent.GetComponent<Image>());
        Destroy(wordParent.transform.GetChild(0).gameObject);
        //scale the parent so that the layout group gets the distances right
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, data.lineLengths.Length * 20);
    }
    /// <summary>
    /// Takes a long word and fits it into a shape that is compact
    /// </summary>
    public void FitToBubbleShape(TMP_Text text)
    {
        Color tagColor = WordUtilities.MatchColorToTag(data.tag);
        text.ForceMeshUpdate();
        // set variables
        TMP_CharacterInfo[] fullText = text.textInfo.characterInfo;

        //Create a child of the Word, that is also a bubble and fill the text with the correlating text
        GameObject child;
        wordParent.GetComponent<VerticalLayoutGroup>().enabled = true;
        int j = 0;
        foreach (Vector2 startEnd in data.lineLengths)
        {
            child = GameObject.Instantiate(ReferenceManager.instance.selectedWordPrefab, wordParent.transform, false);// false fixes a scaling issue
            text = child.GetComponentInChildren<TMP_Text>();
            string line = "";
            for (int i = (int)startEnd.x; i <= (int)startEnd.y; i++)
            {
                line = line + fullText[i].character;
            }
            line = WordUtilities.CapitalizeAllWordsInString(line);
            text.text = line;

            // Scale the text boxes
            ScaleRect(text, child.GetComponent<RectTransform>());
            // color the boxes
            child.GetComponent<Image>().color = tagColor;
            j++;
        }
        if (this is Quest)
            GetComponent<QuestCase>().ChangeAddedParentScale(true);
        //remove the iamge and text from the original bubble
        Destroy(wordParent.GetComponent<UIEffectStack>());
        Destroy(wordParent.GetComponent<Image>());
        Destroy(wordParent.transform.GetChild(0).gameObject);
        //scale the parent so that the layout group gets the distances right
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, data.lineLengths.Length * 20);
    }
    /// <summary>
    /// Takes a long word from a dialogue and changes it into the compact shape
    /// </summary>
    public void UpdateToBubbleShape()
    {
        Color tagColor = WordUtilities.MatchColorToTag(data.tag);

        GameObject toUpdate = wordParent;

        TMP_Text mainText = toUpdate.GetComponentInChildren<TMP_Text>();
        //Set the main text to full text again
        mainText.text = data.name;
        mainText.ForceMeshUpdate();
        TMP_CharacterInfo[] fullText = mainText.textInfo.characterInfo;

        //Delete the other Bubbles
        Image[] images = toUpdate.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
            Destroy(images[i].gameObject);

        //Create a child of the Word, that is also a bubble and fill the text with the correlating text
        GameObject child;
        wordParent.GetComponent<VerticalLayoutGroup>().enabled = true;
        int j = 0;
        foreach (Vector2 startEnd in data.lineLengths)
        {
            child = GameObject.Instantiate(ReferenceManager.instance.selectedWordPrefab, wordParent.transform, false); // false fixes a scaling issue
            mainText = child.GetComponentInChildren<TMP_Text>();
            string line = "";
            for (int i = (int)startEnd.x; i <= (int)startEnd.y; i++)
            {
                line = line + fullText[i].character;
            }
            line = WordUtilities.CapitalizeAllWordsInString(line);
            mainText.text = line;

            // Scale the text boxes
            ScaleRect(mainText, child.GetComponent<RectTransform>());
            // color the boxes
            child.GetComponent<Image>().color = tagColor;
            j++;
        }
        if (this is Quest)
            GetComponent<QuestCase>().ChangeAddedParentScale(true);
        //scale the parent so that the layout group gets the distances right
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, data.lineLengths.Length * 20);
    }
    /// <summary>
    /// if the word doesnt fit the case, shake it
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShakeNoWord(bool isQuest, bool inACase, bool isDuplicate)
    {
        GameObject duplicate = null;
        if (inACase)
        {
            duplicate = SpawnDuplicate();
            StartCoroutine(duplicate.GetComponent<Bubble>().ShakeNoWord(isQuest, false, true));
            MakeInvisible(true);
        }
        fadingOut = true;

        WaitForEndOfFrame delay = new WaitForEndOfFrame();

        if (!inACase && !isDuplicate)
        {
            if (isQuest)
                QuestManager.instance.AutomaticOpenCase(false);
            else
                WordCaseManager.instance.AutomaticOpenCase(false);
        }

        RectTransform rT = GetComponent<RectTransform>();
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
        fadingOut = false;

        if (!inACase && !isDuplicate)
            WordClickManager.instance.SwitchFromCurrentToHighlight();

        if (inACase)
        {
            if (duplicate != null)
                Destroy(duplicate);
            MakeInvisible(false);
        }
    }
    /// <summary>
    /// Gives back an array of Vector2s, each containing the index of the first and last letter of each line in the text
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Vector2[] GetLineLengths()
    {
        relatedText.ForceMeshUpdate();
        //Set these for the first round just in case they create problems otherwise
        List<Vector2> firstAndLast = new List<Vector2>() { Vector2.zero }; //first and last letter of a line
        float lastHeight = relatedText.textInfo.characterInfo[0].vertex_BL.position.y;
        float currentHeight = lastHeight;
        int wordLastChar = relatedText.textInfo.wordInfo[0].lastCharacterIndex;

        //go through all words and check the height of their first letter (== how many lines do we need)
        for (int i = 0; i < relatedText.textInfo.wordCount; i++)
        {
            TMP_WordInfo wordInfo = relatedText.textInfo.wordInfo[i];
            int wordFirstChar = wordInfo.firstCharacterIndex;
            currentHeight = relatedText.textInfo.characterInfo[wordFirstChar].vertex_BL.position.y;

            if (currentHeight + 10 < lastHeight) //the values are EXTREMELY ungenau
            {
                int listCount = firstAndLast.Count - 1;
                firstAndLast[listCount] = new Vector2(firstAndLast[listCount].x, wordLastChar);
                firstAndLast.Add(new Vector2(wordFirstChar, 0));
            }

            // this goes last, because we want to check if we still need it first
            wordLastChar = wordInfo.lastCharacterIndex;
            lastHeight = currentHeight;
        }
        //as there is no line to jump to after the last word, we need to add the last character manually
        firstAndLast[firstAndLast.Count - 1] = new Vector2(firstAndLast[firstAndLast.Count - 1].x, relatedText.textInfo.characterCount - 1);
        return firstAndLast.ToArray();
    }
    public Vector2[] GetLineLengths(TMP_Text text, int firstWord, int lastWord, out TMP_WordInfo[] lineStarts)
    {
        text.ForceMeshUpdate();
        List<TMP_WordInfo> lineStartsList = new List<TMP_WordInfo>();
        int wordFirstChar = text.textInfo.wordInfo[firstWord].firstCharacterIndex;
        int wordLastChar = text.textInfo.wordInfo[firstWord].lastCharacterIndex;
        //Set these for the first round just in case they create problems otherwise
        List<Vector2> firstAndLast = new List<Vector2>() { new Vector2(wordFirstChar, 0) }; //first and last letter of a line
        float lastHeight = text.textInfo.characterInfo[wordFirstChar].vertex_BL.position.y;
        float currentHeight = lastHeight;
        lineStartsList.Add(text.textInfo.wordInfo[firstWord]);
        //go through all words and check the height of their first letter (== how many lines do we need)
        for (int i = firstWord; i <= lastWord; i++)
        {
            TMP_WordInfo wordInfo = text.textInfo.wordInfo[i];
            wordFirstChar = wordInfo.firstCharacterIndex;
            currentHeight = text.textInfo.characterInfo[wordFirstChar].vertex_BL.position.y;

            if (currentHeight + 10 < lastHeight) //the values are EXTREMELY ungenau
            {
                int listCount = firstAndLast.Count - 1;
                firstAndLast[listCount] = new Vector2(firstAndLast[listCount].x, wordLastChar);
                firstAndLast.Add(new Vector2(wordFirstChar, 0));
                lineStartsList.Add(wordInfo);
            }

            // this goes last, because we want to check if we still need it first
            wordLastChar = wordInfo.lastCharacterIndex;
            lastHeight = currentHeight;
        }
        //as there is no line to jump to after the last word, we need to add the last character manually
        firstAndLast[firstAndLast.Count - 1] = new Vector2(firstAndLast[firstAndLast.Count - 1].x, text.textInfo.wordInfo[lastWord].lastCharacterIndex);
        lineStarts = lineStartsList.ToArray();
        return firstAndLast.ToArray();
    }
    /// <summary>
    /// Makes a word visible or invisible
    /// </summary>
    public void MakeInvisible(bool makeInvisible)
    {
        Color color;

        foreach (Image img in GetComponentsInChildren<Image>())
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
        foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>())
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
    /// <summary>
    /// takes this word and spawns an exact duplicate, parented to selectedWordAskParent 
    /// </summary>
    /// <param name="spawn"></param>
    public GameObject SpawnDuplicate()
    {
        GameObject duplicate = GameObject.Instantiate(this.gameObject, this.transform.position, this.transform.rotation);
        duplicate.transform.SetParent(ReferenceManager.instance.selectedWordParentAsk.transform, false);
        duplicate.transform.position = this.transform.position;
        duplicate.transform.rotation = this.transform.rotation;
        return duplicate;
    }
    /// <summary>
    /// as the function needs the data object to be initilized, this can only happen after Inizialize()
    /// </summary>
    public void InitializeBubbleShaping(Vector2 firstAndLastWordIndex)
    {
        //Shape the bubble correctly
        if (data.isLongWord)
        {
            if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
                FitToText(relatedText, originalText, (int)firstAndLastWordIndex.x, (int)firstAndLastWordIndex.y);
            else if (data.origin == WordInfo.Origin.QuestLog)
                FitToBubbleShape(relatedText);
        }
    }
    /// <summary>
    /// Called when the word is double clicked
    /// </summary>
    public void OnDoubleClicked(bool isQuest)
    {
        if (data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Environment)
        {
            DoubleClickedOnDialogue(isQuest);
        }
        else //is in word case or quest log
        {
            DoubleClickedOnCase();
        }
    }
    public void DoubleClickedOnDialogue(bool isQuest)
    {
        WordCaseManager.instance.openTag = data.tag;
        bool fits = false;
        //check, if the word fits in the case right now
        if (isQuest)
        {
            if (QuestManager.instance.CheckIfCanSaveBubble(data.name, out int index))
                fits = true;
        }
        else
        {
            if (WordCaseManager.instance.CheckIfCanSaveBubble(data.name, out int index))
                fits = true;
        }
        //if yes slowly animate this to the correct case & save it in there
        if (fits)
        {
            AnimateMovementIntoCase(isQuest);
        }
        //if not, shaking animation
        else
        {
            StartCoroutine(ShakeNoWord(isQuest, false, false));
            Color color = GetComponentInChildren<Image>().color;
            StartCoroutine(EffectUtilities.ColorTagGradient(this.gameObject, new Color[] { color, Color.red, Color.red, Color.red, color }, 0.6f));
        }
    }
    public void DoubleClickedOnCase()
    {
        //figure out, if there is a promptbubble nearby()
        if (piManager.CheckForPromptsWithTag(data.tag, out PromptBubble pB))
        {
            AnimateMovementIntoPrompt(pB);
        }
        else
        {
            StartCoroutine(ShakeNoWord(false, true, false)); //isQuest is irrelevant
        }
    }
    /// <summary>
    /// Animates a word from its current position into a fitting case and saves it
    /// </summary>
    /// <param name="isQuest"></param>
    public void AnimateMovementIntoCase(bool isQuest)
    {
        //get target position
        Vector2 targetPos = GetCaseTargetPosition(isQuest);
        StartCoroutine(AnimateMovement(movementDone, targetPos));
        StartCoroutine(AfterMovement_ToCase(movementDone, isQuest));
    }
    /// <summary>
    /// Animates a word from its current position into a fitting prompt
    /// </summary>
    public void AnimateMovementIntoPrompt(PromptBubble pB)
    {
        //get target position
        transform.SetParent(refM.selectedWordParentAsk.transform);
        Vector2 targetPos = pB.transform.localPosition;
        StartCoroutine(AnimateMovement(movementDone, targetPos));
        StartCoroutine(AfterMovement_ToPromptBubble(movementDone, pB));
    }
    /// <summary>
    /// Animates a word from its current position back into the case where it came from
    /// </summary>
    /// <param name="isQuest"></param>
    public void AnimateMovementBackToCase(bool isQuest)
    {
        //get target position
        Vector2 targetPos = GetCaseTargetPosition(isQuest);
        StartCoroutine(AnimateMovement(movementDone, targetPos));
        StartCoroutine(AfterMovement_BackToCase(movementDone, isQuest));
    }
    /// <summary>
    /// Get the target position of the case we want to animate to 
    /// </summary>
    /// <param name="isQuest"></param>
    /// <returns></returns>
    public Vector2 GetCaseTargetPosition(bool isQuest)
    {
        if (isQuest)
            return ReferenceManager.instance.questCase.GetComponent<RectTransform>().rect.center +
                 (Vector2)WordUtilities.GlobalScreenToCanvasPosition(
                     ReferenceManager.instance.questCase.GetComponent<RectTransform>().position);
        else
            return ReferenceManager.instance.wordCase.GetComponent<RectTransform>().rect.center +
                            (Vector2)WordUtilities.GlobalScreenToCanvasPosition(
                                ReferenceManager.instance.wordCase.GetComponent<RectTransform>().position);
    }
    /// <summary>
    /// Animate this word from its position to a targetPosition
    /// </summary>
    /// <returns></returns>
    public IEnumerator AnimateMovement(RefBool isDone, Vector2 targetPos)
    {
        if (data.isLongWord)
            UpdateToBubbleShape();

        fadingOut = true;
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        RectTransform rT = GetComponent<RectTransform>();
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
    /// <summary>
    /// wait until movement is done and then save the word in the fitting case
    /// </summary>
    /// <param name="isQuest"></param>
    /// <returns></returns>
    public IEnumerator AfterMovement_ToCase(RefBool isDone, bool isQuest)
    {
        //wait until movement is done
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (!isDone.refBool)
            yield return delay;

        //treat the word as if it was dragged onto the Questlog/Wordcase
        movementDone.refBool = false;
        if (isQuest)
        {
            IsOverQuestLog();
            QuestManager.instance.AutomaticOpenCase(false);
        }
        else
        {
            IsOverWordCase();
            WordCaseManager.instance.AutomaticOpenCase(false);
        }
        EffectUtilities.ReColorAllInteractableWords();
    }
    /// <summary>
    /// wait until movement is done and then parent the word to a promptbubble
    /// </summary>
    /// <param name="isDone"></param>
    /// <param name="pB"></param>
    /// <returns></returns>
    public IEnumerator AfterMovement_ToPromptBubble(RefBool isDone, PromptBubble pB)
    {
        //wait until movement is done
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (!isDone.refBool)
            yield return delay;
        movementDone.refBool = false;

        WordClickManager.instance.promptBubble = pB;
        WordUtilities.ParentBubbleToPrompt(this.gameObject);
        WordClickManager.instance.promptBubble = null;
    }
    public IEnumerator AfterMovement_BackToCase(RefBool isDone, bool isQuest)
    {
        //wait until movement is done
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (!isDone.refBool)
            yield return delay;
        movementDone.refBool = false;

        //put the word back
        WordClickManager.instance.DestroyCurrentWord(this);
        if (!isQuest)
            WordCaseManager.instance.ReloadContents(false);
        else
            QuestManager.instance.ReloadContents(false);
        EffectUtilities.ReColorAllInteractableWords();
    }
    #endregion
}
public class BubbleData
{
    public string name;
    public string[] tagInfo;
    public string tag;
    public WordInfo.Origin origin;
    public Vector2[] lineLengths;
    public bool isLongWord;
}
