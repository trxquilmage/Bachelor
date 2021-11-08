using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class Word : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerClickHandler, IPointerDownHandler
{
    TMP_Text nameText;
    bool wasDragged; //after the mouse goes up, after it was dragged it checks, where the object is now at
    bool fadingOut;
    public WordData data;

    public TMP_WordInfo relatedWordInfo;
    public TMP_Text relatedText;
    Vector3 wordSize;
    PlayerInputManager piManager;
    float floatTime = 0.75f;
    private void Start()
    {
        piManager = PlayerInputManager.instance;
    }
    public struct WordData
    {
        public string name;
        public string[] tagInfo;
        public WordInfo.WordTags tag;
        public WordInfo.Origin origin;
        public TagObject tagObj;
        public Vector2[] lineLengths;
    }
    /// <summary>
    /// 0 = name, 1 = tags[0] (main tag), 2 = tags[1] (sub tag 1) ...
    /// </summary>
    public struct TagObject
    {
        public List<Yarn.Value> allGivenValues;
    }
    /// <summary>
    /// Call this, when creating a Word. If you dont have a Word Info, create one and set "hasWordInfo" to false
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tags"></param>
    public void Initialize(string name, string[] tags, WordInfo.Origin origin, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex, bool hasWordInfo, bool longWord)
    {
        data = new WordData();

        //capitalize name
        data.name = WordUtilities.CapitalizeAllWordsInString(name);

        data.tagInfo = tags;
        data.tag = WordUtilities.StringToTag(tags[0]);
        data.origin = origin;
        if (hasWordInfo)
        {
            relatedWordInfo = wordInfo;
            relatedText = wordInfo.textComponent;
        }
        nameText = transform.GetComponentInChildren<TMP_Text>();
        nameText.text = data.name;
        ScaleRect(nameText, GetComponent<RectTransform>());
        wordSize = this.GetComponent<RectTransform>().sizeDelta;
        WordUtilities.ColorTag(this.gameObject, data.tag);

        //initialize the tag Object
        data.tagObj = new TagObject();
        data.tagObj.allGivenValues = new List<Yarn.Value>();
        data.tagObj.allGivenValues.Add(new Yarn.Value(data.name));
        int i = 0;
        foreach (string tag in tags)
        {
            if (i != 0)
            {
                Yarn.Value val = WordUtilities.TransformIntoYarnVal(tag);
                data.tagObj.allGivenValues.Add(val);
            }
            i++; //we dont want the location to be in this
        }
        data.lineLengths = GetLineLengths();

        if (longWord) //See how the word should be shaped
        {
            if (origin == WordInfo.Origin.Dialogue || origin == WordInfo.Origin.Ask || origin == WordInfo.Origin.Environment)
                FitToText(nameText, relatedText, (int)firstAndLastWordIndex.x, (int)firstAndLastWordIndex.y);
            else if (origin == WordInfo.Origin.QuestLog)
                FitToBubbleShape(nameText);
        }
    }
    /// <summary>
    /// Scale the picked up word, so that the rect of the background fits the word in the center
    /// </summary>
    void ScaleRect(TMP_Text text, RectTransform rTransform)
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
            transform.position = WordClickManager.instance.GetMousePos();
            transform.position -= wordSize / 2;
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
                        WordCaseManager.instance.TrashAWord();
                    }
                    else if (clickM.mouseOverUIObject == "wordCase")
                    {
                        IsOverWordCase();
                    }
                    else if (clickM.mouseOverUIObject == "questLog")
                    {
                        IsOverQuestLog();
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
            //else
            //    IsOverNothing();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // this cant be deleted bc for some reasons the other functions dont work without it
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!fadingOut)
        {
            // This is basically OnDragStart()

            if (transform.parent.TryGetComponent<PromptBubble>(out PromptBubble pB)) //if currently attached to a prompt bubble
            {
                pB.child = null;
            }

            // if the word is being dragged out of the dialogue
            if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
            {
                if (data.tag != WordInfo.WordTags.Quest)
                {
                    WordCaseManager.instance.AutomaticOpenCase(true);
                    WordCaseManager.instance.openTag = data.tag;
                }
                else if (data.tag == WordInfo.WordTags.Quest)
                {
                    QuestManager.instance.AutomaticOpenCase(true);
                    UpdateToBubbleShape();
                }
                WordClickManager.instance.currentWord = this.gameObject;
                WordClickManager.instance.wordLastHighlighted = null;

            }
            else if (data.origin == WordInfo.Origin.WordCase)
            {
                transform.SetParent(ReferenceManager.instance.selectedWordParentAsk.transform);
                //Delete Word from case list
                WordClickManager.instance.currentWord = this.gameObject;
                WordCaseManager.instance.SpawnWordReplacement(this);
            }
            else if (data.origin == WordInfo.Origin.QuestLog)
            {
                transform.SetParent(ReferenceManager.instance.selectedWordParentAsk.transform);
                //Spawn Replacement
                WordClickManager.instance.currentWord = this.gameObject;
                QuestManager.instance.SpawnQuestReplacement(this);
            }
        }
    }
    /// <summary>
    /// The bubble was dragged onto the word case and dropped
    /// </summary>
    void IsOverWordCase()
    {
        if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
        {
            //if its NOT a quest, put into word case
            if (data.tag != WordInfo.WordTags.Quest)
            {

                //save it
                WordCaseManager.instance.SaveWord(WordClickManager.instance.currentWord.GetComponent<Word>());

                //close the case & Delete the UI word
                WordCaseManager.instance.AutomaticOpenCase(false);
                WordClickManager.instance.DestroyCurrentWord();
            }
            //if its a quest, pretend as if is over nothing
            else if (data.tag == WordInfo.WordTags.Quest)
            {
                IsOverNothing();
            }
        }
        else if (data.origin == WordInfo.Origin.WordCase)
        {
            // put it back
            WordCaseManager.instance.PutWordBack(this, ReferenceManager.instance.listingParent.transform);
        }
        else if (data.origin == WordInfo.Origin.QuestLog)
        {
            // put it back
            WordCaseManager.instance.PutWordBack(this, ReferenceManager.instance.questListingParent.transform);
        }
    }
    /// <summary>
    /// The bubble was dragged onto the questLog and dropped
    /// </summary>
    void IsOverQuestLog()
    {
        if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
        {
            //if its a quest, put into quest Log
            if (data.tag == WordInfo.WordTags.Quest)
            {
                //save it
                QuestManager.instance.SaveQuest(WordClickManager.instance.currentWord.GetComponent<Word>());

                //close the case & Delete the UI word
                QuestManager.instance.AutomaticOpenCase(false);
                WordClickManager.instance.DestroyCurrentWord();
            }
            //if its NOT a quest, pretend as if is over nothing
            else if (data.tag != WordInfo.WordTags.Quest)
            {
                IsOverNothing();
            }
        }
        else if (data.origin == WordInfo.Origin.WordCase)
        {
            // put it back
            WordCaseManager.instance.PutWordBack(this, ReferenceManager.instance.listingParent.transform);
        }
        else if (data.origin == WordInfo.Origin.QuestLog)
        {
            // put it back
            WordCaseManager.instance.PutWordBack(this, ReferenceManager.instance.questListingParent.transform);
        }
    }
    /// <summary>
    /// The bubble was dragged onto a prompt case and dropped
    /// </summary>
    void IsOverPlayerInput()
    {
        if (WordClickManager.instance.promptBubble.acceptsCurrentWord)
        {
            if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
            {
                //parent to word
                WordUtilities.ParentBubbleToPrompt(this.gameObject);
                //close wordCase
                WordCaseManager.instance.AutomaticOpenCase(false);
            }
            else if (data.origin == WordInfo.Origin.WordCase)
            {
                // parent to word
                WordUtilities.ParentBubbleToPrompt(this.gameObject);
            }
        }
        else
            IsOverNothing();
    }
    /// <summary>
    /// The bubble was dragged onto nothing and dropped
    /// </summary>
    public void IsOverNothing()
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
                    if (ReferenceManager.instance.askPromptBubbleParent.transform.GetChild(0).TryGetComponent<PromptBubble>(out PromptBubble pB))
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

        //this happens, regardless wheter a character was hit or not
        if (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Ask || data.origin == WordInfo.Origin.Environment)
        {
            //close the case & Delete the UI word
            WordCaseManager.instance.AutomaticOpenCase(false);
            WordClickManager.instance.DestroyCurrentWord();
        }
        else if (data.origin == WordInfo.Origin.WordCase)
        {
            // put it back
            WordClickManager.instance.DestroyCurrentWord();
            WordCaseManager.instance.PutWordBack(this, ReferenceManager.instance.listingParent.transform);
        }
        else if (data.origin == WordInfo.Origin.QuestLog)
        {
            // put it back
            WordClickManager.instance.DestroyCurrentWord();
            WordCaseManager.instance.PutWordBack(this, ReferenceManager.instance.questListingParent.transform);
        }
    }
    /// <summary>
    /// Takes a long word and fits it above the text it is portraying
    /// </summary>
    void FitToText(TMP_Text editableText, TMP_Text sourceText, int firstWordInfoIndex, int lastWordInfoIndex)
    {
        Color tagColor = WordUtilities.MatchColorToTag(data.tag);
        editableText.ForceMeshUpdate();
        Vector2[] sourceLineLengths = GetLineLengths(sourceText, firstWordInfoIndex, lastWordInfoIndex, out TMP_WordInfo[] lineStarts);
        // set variables
        TMP_CharacterInfo[] fullText = editableText.textInfo.characterInfo;

        //Create a child of the Word, that is also a bubble and fill the text with the correlating text
        GameObject child = this.gameObject;
        GetComponent<VerticalLayoutGroup>().enabled = false;

        int j = 0;
        foreach (Vector2 startEnd in sourceLineLengths)
        {
            Vector2 position = WordUtilities.GetWordPosition(sourceText, lineStarts[j]);
            child = GameObject.Instantiate(ReferenceManager.instance.selectedWordPrefab, position, Quaternion.identity);
            child.transform.SetParent(transform, false); // false fixes a scaling issue
            child.transform.position = position;
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
        //remove the iamge and text from the original bubble
        Destroy(GetComponent<Image>());
        Destroy(transform.GetChild(0).gameObject);
        //scale the parent so that the layout group gets the distances right
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, data.lineLengths.Length * 20);
    }
    /// <summary>
    /// Takes a long word and fits it into a shape that is compact
    /// </summary>
    void FitToBubbleShape(TMP_Text text)
    {
        Color tagColor = WordUtilities.MatchColorToTag(data.tag);
        text.ForceMeshUpdate();
        // set variables
        TMP_CharacterInfo[] fullText = text.textInfo.characterInfo;

        //Create a child of the Word, that is also a bubble and fill the text with the correlating text
        GameObject child;
        GetComponent<VerticalLayoutGroup>().enabled = true;
        int j = 0;
        foreach (Vector2 startEnd in data.lineLengths)
        {
            child = GameObject.Instantiate(ReferenceManager.instance.selectedWordPrefab, Vector2.zero, Quaternion.identity);
            child.transform.SetParent(transform, false); // false fixes a scaling issue
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
        //remove the iamge and text from the original bubble
        Destroy(GetComponent<Image>());
        Destroy(transform.GetChild(0).gameObject);
        //scale the parent so that the layout group gets the distances right
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, data.lineLengths.Length * 20);
    }
    /// <summary>
    /// Takes a long word from a dialogue and changes it into the compact shape
    /// </summary>
    public void UpdateToBubbleShape()
    {
        Color tagColor = WordUtilities.MatchColorToTag(data.tag);
        GameObject toUpdate = WordClickManager.instance.wordLastHighlighted;
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
        GetComponent<VerticalLayoutGroup>().enabled = true;
        int j = 0;
        foreach (Vector2 startEnd in data.lineLengths)
        {
            child = GameObject.Instantiate(ReferenceManager.instance.selectedWordPrefab, Vector2.zero, Quaternion.identity);
            child.transform.SetParent(transform, false); // false fixes a scaling issue
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
        //scale the parent so that the layout group gets the distances right
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, data.lineLengths.Length * 20);
    }
    /// <summary>
    /// Move the word to the case it belongs to
    /// </summary>
    public void MoveToCase()
    {
        bool isQuest = data.tag == WordInfo.WordTags.Quest;
        bool fits = false;
        //check, if the word fits in the case right now
        if (isQuest)
        {
            fits = true;
        }
        else
        {
            fits = true;
        }
        //if yes slowly animate this to the correct case & save it in there
        if (fits)
            StartCoroutine(AnimateMoveToCase(isQuest));
        //if not, shaking animation
        else
            Debug.Log("shake anim");
    }
    /// <summary>
    /// Animate the word's movement to it's case, 
    /// </summary>
    /// <returns></returns>
    IEnumerator AnimateMoveToCase(bool isQuest)
    {
        fadingOut = true;
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        RectTransform rT = GetComponent<RectTransform>();
        Vector2 startPos = rT.position;
        Vector2 targetPos;
        if (isQuest)
            targetPos = ReferenceManager.instance.questCase.GetComponent<RectTransform>().rect.center + 
                (Vector2)ReferenceManager.instance.questCase.GetComponent<RectTransform>().position;
        else
            targetPos = ReferenceManager.instance.wordCase.GetComponent<RectTransform>().rect.center +
                (Vector2)ReferenceManager.instance.wordCase.GetComponent<RectTransform>().position;
        float timer = 0;
        float t = 0;
        while (timer < floatTime)
        {
            timer += Time.deltaTime;
            t = WordUtilities.Remap(timer, 0, floatTime, 0, 1);
            rT.position = Vector2.Lerp(startPos, targetPos, t);
            yield return delay;
        }
        if (isQuest)
            IsOverQuestLog();
        else
            IsOverWordCase();
    }
    /// <summary>
    /// Gives back an array of Vector2s, each containing the index of the first and last letter of each line in the text
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    Vector2[] GetLineLengths()
    {
        nameText.ForceMeshUpdate();
        //Set these for the first round just in case they create problems otherwise
        List<Vector2> firstAndLast = new List<Vector2>() { Vector2.zero }; //first and last letter of a line
        float lastHeight = nameText.textInfo.characterInfo[0].vertex_BL.position.y;
        float currentHeight = lastHeight;
        int wordLastChar = nameText.textInfo.wordInfo[0].lastCharacterIndex;

        //go through all words and check the height of their first letter (== how many lines do we need)
        for (int i = 0; i < nameText.textInfo.wordCount; i++)
        {
            TMP_WordInfo wordInfo = nameText.textInfo.wordInfo[i];
            int wordFirstChar = wordInfo.firstCharacterIndex;
            currentHeight = nameText.textInfo.characterInfo[wordFirstChar].vertex_BL.position.y;

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
        firstAndLast[firstAndLast.Count - 1] = new Vector2(firstAndLast[firstAndLast.Count - 1].x, nameText.textInfo.characterCount - 1);
        return firstAndLast.ToArray();
    }
    Vector2[] GetLineLengths(TMP_Text text, int firstWord, int lastWord, out TMP_WordInfo[] lineStarts)
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
}

