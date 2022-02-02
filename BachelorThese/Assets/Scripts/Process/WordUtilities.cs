using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using Yarn.Unity;

public static class WordUtilities
{
    /// <summary>
    /// Match the word to a color fitting the tag
    /// </summary>
    /// <param name="word"></param>
    public static Color MatchColorToTag(string tagName, string subtagName)
    {
        WordInfo.WordTag tag = GetTag(tagName);
        Color color = tag.tagColor;
        color = (subtagName != "") ? DiluteColorToSubtag(color, subtagName) : color;
        return color;
    }

    public static Color DiluteColorToSubtag(Color color, string subtagName)
    {
        int seed = subtagName.GetHashCode();
        System.Random randomSystem = new System.Random(seed);

        for (int i = 0; i < 2; i++)
            randomSystem.Next();

        Color dilutionColor = new Color32(
            (byte)randomSystem.Next(0, 255),
            (byte)randomSystem.Next(0, 255),
            (byte)randomSystem.Next(0, 255), 255);

        color = Color.Lerp(color, dilutionColor, 0.15f);
        return color;
    }

    /// <summary>
    /// Creates a word-Object at the mouse's position
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tag"></param>
    /// <param name="wordMousePos"></param>
    public static GameObject CreateWord(BubbleData data, Vector3 wordMousePos, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex, WordInfo.Origin origin, bool createBubbleFromBubble)
    {
        ReferenceManager refM = ReferenceManager.instance;
        Transform parent = (PlayerInputManager.instance.inAsk) ? refM.selectedWordParentAsk.transform : refM.selectedWordParent.transform;

        GameObject word = GameObject.Instantiate(refM.wordParentPrefab, wordMousePos, Quaternion.identity);
        word.transform.SetParent(parent, false); // the false makes sure it isnt some random size
        word.GetComponent<RectTransform>().localPosition = wordMousePos;
        Word wordScript = word.AddComponent<Word>();

        if (createBubbleFromBubble)
            wordScript.Initialize(data, firstAndLastWordIndex);
        else
            wordScript.Initialize(data, origin, wordInfo, firstAndLastWordIndex);

        return word;
    }
    /// <summary>
    /// Find the correct struct WordInfo.WordTag for the given string
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static WordInfo.WordTag GetTag(string tagName)
    {
        WordInfo.WordTag tagInfo = new WordInfo.WordTag();
        ReferenceManager refM = ReferenceManager.instance;
        foreach (WordInfo.WordTag tag in refM.wordTags)
        {
            if (tag.name == tagName)
                tagInfo = tag;
        }
        return tagInfo;
    }

    /// <summary>
    /// Get The Position of a wordInfo that is added to this
    /// </summary>
    /// <param name="text"></param>
    /// <param name="word"></param>
    /// <returns></returns>
    public static Vector3 GetWordPosition(TMP_Text text, TMP_WordInfo word)
    {
        text.ForceMeshUpdate();
        Vector3 wordPosition;
        Vector3 lowerLeftCorner;
        bool textOnOverlay = text.gameObject.tag == "TextOnOverlayCanvas";

        //Text is on a canvas that has ScreenSpace - Camera
        if (!textOnOverlay)
        {
            // Get the StartPosition (lower left corner) of the button
            TMP_CharacterInfo charInfo = text.textInfo.characterInfo[word.firstCharacterIndex];
            TMP_Vertex vertexBL = charInfo.vertex_BL;
            lowerLeftCorner = vertexBL.position;

            // Get the StartPosition of the bounds (lower left corner)
            Vector3 lowerLeftTextBox = Camera.main.WorldToScreenPoint(text.rectTransform.position);
            lowerLeftTextBox = LocalScreenToCanvasPosition(lowerLeftTextBox);
            wordPosition = lowerLeftTextBox + lowerLeftCorner;
            wordPosition = Vector3.Scale(wordPosition, new Vector3(1, 1, 0));
            return wordPosition;
        }
        //Text is on a canvas that has ScreenSpace - Overlay
        else
        {
            // Get the StartPosition (lower left corner) of the button
            TMP_CharacterInfo charInfo = text.textInfo.characterInfo[word.firstCharacterIndex];
            TMP_Vertex vertexBL = charInfo.vertex_BL;
            lowerLeftCorner = vertexBL.position;

            // Get the StartPosition of the bounds (lower left corner)
            Vector3 lowerLeftTextBox = text.rectTransform.position;
            float canvasScaler = (ReferenceManager.instance.canvas.scaleFactor * 2);
            wordPosition = lowerLeftTextBox + lowerLeftCorner * canvasScaler;
            wordPosition = Vector3.Scale(wordPosition, new Vector3(1, 1, 0));
            return wordPosition;
        }
    }
    /// <summary>
    /// returns the parameters of a word as (lowerLeftcorner.x,lowerLeftcorner.y),(size.x, size.y). 
    /// Can include ignored characters in this format: /Hello/
    /// </summary>
    /// <param name="text"></param>
    /// <param name="word"></param>
    /// <param name="hasIgnoredletters"></param>
    /// <returns></returns>
    public static Vector3[] GetWordParameters(TMP_Text text, TMP_WordInfo word, bool hasIgnoredChars)
    {
        text.ForceMeshUpdate();

        int firstCharacter = word.firstCharacterIndex;
        int lastCharacter = word.lastCharacterIndex;

        if (hasIgnoredChars)
        {
            firstCharacter--;
            lastCharacter++;
        }
        Vector3[] parameters = new Vector3[2] { Vector3.zero, Vector3.zero };

        // Get the StartPosition (lower left corner) of the button
        TMP_CharacterInfo charInfo = text.textInfo.characterInfo[firstCharacter];
        TMP_CharacterInfo charInfoLast = text.textInfo.characterInfo[lastCharacter];
        TMP_Vertex vertexBL = charInfo.vertex_BL;
        TMP_Vertex vertexTR = charInfoLast.vertex_TR;
        parameters[0] = vertexBL.position;

        // Get the StartPosition of the bounds (lower left corner)
        Vector3 lowerLeftTextBox = Camera.main.WorldToScreenPoint(text.rectTransform.position);
        lowerLeftTextBox = LocalScreenToCanvasPosition(lowerLeftTextBox);
        parameters[0] = lowerLeftTextBox + parameters[0];

        // Get the length of the word
        parameters[1] = vertexTR.position - vertexBL.position;
        return parameters;
    }
    /// <summary>
    /// Creates a bubble over the wordInfo word.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="wordInfo"></param>
    public static void CreateABubble(TMP_Text text, TMP_WordInfo[] wordInfos)
    {
        text.ForceMeshUpdate();
        Vector3 wordPosition = GetWordPosition(text, wordInfos[0]);
        Vector2 firstAndLastWordIndex = GetFirstAndLastWordIndex(text, wordInfos);

        WordInfo.Origin origin;
        if (text == ReferenceManager.instance.interactableTextList[0])
            origin = WordInfo.Origin.Dialogue;
        else if (text == ReferenceManager.instance.interactableTextList[2])
            origin = WordInfo.Origin.Ask;
        else
            origin = WordInfo.Origin.Environment;

        string wordToSend = CapitalizeAllWordsInString(WordInfoToString(wordInfos));
        WordClickManager.instance.CheckWord(wordToSend, wordPosition, wordInfos[0], firstAndLastWordIndex, origin);
        // Set the hovered word to interacted
        EffectUtilities.ReColorAllInteractableWords();
    }

    /// <summary>
    /// checks the given text for promt inputs in the type of "|Tag|"
    /// </summary>
    public static GameObject CheckForPromptInputsAndCreatePrompt(TMP_Text text, TMP_TextInfo textInfo, Transform bubbleParent, string subtags)
    {
        text.ForceMeshUpdate();
        TMP_CharacterInfo[] charInfo = textInfo.characterInfo;

        //If the word starts with "|" -> give out the word
        TMP_WordInfo wordInfo;
        for (int i = 0; i < textInfo.wordCount; i++)
        {
            wordInfo = textInfo.wordInfo[i];

            // wordInfo igores Sonderzeichen in front of words, so iam checking the letter in front of the word
            if (wordInfo.firstCharacterIndex - 1 > -1)
            {
                if (charInfo[wordInfo.firstCharacterIndex - 1].character == @"|"[0])
                {
                    return CreatePromptBubble(textInfo.textComponent, wordInfo, bubbleParent, subtags);
                }
            }
        }
        return null;
    }

    public static GameObject GetChildWithTag(GameObject parent, string tag)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
            if (parent.transform.GetChild(i).tag == tag)
                return parent.transform.GetChild(i).gameObject;

        Debug.Log("No child with that tag found. Note that this " +
            "function only finds first generation children");
        return null;
    }

    /// <summary>
    /// Create a Prompt Bubble above a word in a given text
    /// </summary>
    /// <param name="text"></param>
    /// <param name="wordInfo"></param>
    public static GameObject CreatePromptBubble(TMP_Text text, TMP_WordInfo wordInfo, Transform bubbleParent, string subtags)
    {
        Vector3[] wordParameters = GetWordParameters(text, wordInfo, true);
        GameObject promptBubble = GameObject.Instantiate(ReferenceManager.instance.promptBoxPrefab, wordParameters[0], Quaternion.identity);
        promptBubble.transform.SetParent(bubbleParent, false);

        PromptBubble pB = promptBubble.GetComponent<PromptBubble>();
        pB.Initialize(wordInfo.GetWord(), subtags, wordParameters);
        return promptBubble;
    }
    /// <summary>
    /// Parents the child to the prompt the mouse is currently hovering over
    /// </summary>
    /// <param name="child"></param>
    public static void ParentBubbleToPrompt(GameObject child)
    {
        GameObject promptBubble = WordClickManager.instance.promptBubble.gameObject;

        PromptBubble bubble = promptBubble.GetComponent<PromptBubble>();
        if (bubble.child != null) // if there is already a prompt put in
        {
            // remove the word
            bubble.child.transform.SetParent(ReferenceManager.instance.selectedWordParentAsk.transform);
            bubble.child.GetComponent<Bubble>().DroppedOverNothing(); // put the OG word back where it came from
            bubble.child = null;
        }
        child.transform.SetParent(promptBubble.transform);
        float localX = promptBubble.GetComponent<RectTransform>().sizeDelta.x / 2; //center
        localX -= child.GetComponent<RectTransform>().sizeDelta.x / 2; // - half of word case
        child.transform.localPosition = new Vector3(localX, 0);
        bubble.child = child;
        WordClickManager.instance.currentWord = null;
    }
    /// <summary>
    /// returns a given word into the text it was taken from
    /// </summary>
    public static void ReturnWordIntoText(Bubble word)
    {
        //delete the bubble
        WordClickManager.instance.DestroyCurrentWord();
        //update text colors
        EffectUtilities.ReColorAllInteractableWords();
    }


    public static bool IsWordInADataBank(string wordName, bool isFillerWord, out bool wordAlreadyInWordCase, out bool wordCaseIsFull)
    {
        wordAlreadyInWordCase = true;
        wordCaseIsFull = true;
        int numberOfWords = wordName.Trim().Split(" "[0]).Length;
        if (wordName == "")
            return false;

        wordName = CapitalizeAllWordsInString(wordName);
        bool isUsed = false;

        WordLookupReader wlReader = WordLookupReader.instance;
        ReferenceManager refM = ReferenceManager.instance;
        Dictionary<string, string[]> wordTagList = (isFillerWord) ? wlReader.fillerTag :
            (numberOfWords == 1) ? wlReader.wordTag : wlReader.longWordTag;

        string tagName = wordTagList.ContainsKey(wordName) ? wordTagList[wordName][0] : refM.wordTags[refM.otherTagIndex].name;

        if (!refM.noGreyOut)
        {
            if (!WordCaseManager.instance.CheckIfCanSaveBubble(wordName, out int index, out bool bubbleIsAlreadyInList, out bool caseIsFull, tagName))
            {
                wordAlreadyInWordCase = bubbleIsAlreadyInList;
                wordCaseIsFull = caseIsFull;
                isUsed = true;
            }

            if (WordClickManager.instance.currentWord != null &&
                wordName == WordClickManager.instance.currentWord.GetComponent<Bubble>().data.name)
            {
                wordCaseIsFull = false;
                wordAlreadyInWordCase = false;
                isUsed = true;
            }
        }

        if (WordClickManager.instance.wordLastHighlighted != null &&
            wordName == WordClickManager.instance.wordLastHighlighted.GetComponent<Bubble>().data.name)
        {
            wordCaseIsFull = false;
            wordAlreadyInWordCase = false;
            isUsed = true;
        }

        return isUsed;
    }
    /// <summary>
    /// Jump to the given node of the given Dialogmanager
    /// </summary>
    public static void JumpToNode(DialogueRunner runner, string nodeName)
    {
        nodeName = Regex.Replace(nodeName, @"\s+", "");
        string node = DialogueManager.instance.currentTarget.characterName + "." + nodeName;
        if (runner.isActiveAndEnabled)
        {
            if (runner.NodeExists(node))
            {
                runner.startNode = node;
                runner.StartDialogue();
            }
            else
            {
                runner.startNode = DialogueManager.instance.currentTarget.characterName + ".Catchall";
                runner.StartDialogue();
            }

        }
    }
    /// <summary>
    /// Enter a word, to check, if it might be a number or a bool, if not return it as a sting-Yarn.Value
    /// </summary>
    /// <param name="tag"></param>
    public static Yarn.Value TransformIntoYarnVal(string tag)
    {
        if (tag.Length > 0)
        {
            if (char.IsDigit(tag[0]))
                return new Yarn.Value((float)Convert.ToInt32(tag));
            else if (tag == "true")
                return new Yarn.Value(true);
            else if (tag == "false")
                return new Yarn.Value(false);
            else
                return new Yarn.Value(tag);
        }
        return null;
    }
    /// <summary>
    /// Remap.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="from1"></param>
    /// <param name="to1"></param>
    /// <param name="from2"></param>
    /// <param name="to2"></param>
    /// <returns></returns>
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    /// <summary>
    /// Reads all words in the wordInfo[] and returns them as one string
    /// </summary>
    /// <param name="wordInfo"></param>
    /// <returns></returns>
    public static string WordInfoToString(TMP_WordInfo[] wordInfos)
    {
        string words = "";
        words += wordInfos[0].GetWord();
        for (int i = 1; i < wordInfos.Length; i++)
        {
            words += " " + wordInfos[i].GetWord();
        }
        return words;
    }
    /// <summary>
    /// Takes a text and a TMP_WordInfo and returns the first and last words as x and y in a vector 2
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetFirstAndLastWordIndex(TMP_Text text, TMP_WordInfo[] wordInfos)
    {
        text.ForceMeshUpdate();
        int i = 0;
        int j = 0;
        Vector2 firstAndLast = Vector2.zero;
        bool hasFirst = false;
        for (int k = 0; k < text.textInfo.wordCount; k++)
        {
            if (j < wordInfos.Length && text.textInfo.wordInfo[k].GetWord() == wordInfos[j].GetWord())
            {
                if (!hasFirst)
                {
                    hasFirst = true;
                    firstAndLast = new Vector2(i, 0);
                }
                j++;
            }
            else if (hasFirst) //words are NOT the same, but the list had been already going
            {
                //check if the list might be done
                if (j == wordInfos.Length)
                {
                    //finish off
                    firstAndLast = new Vector2(firstAndLast.x, i - 1);
                    return firstAndLast;
                }
                //else cancel list
                else
                {
                    hasFirst = false;
                    firstAndLast = Vector2.zero;
                    j = 0;
                }
            }
            i++;
        }
        //finish off
        firstAndLast = new Vector2(firstAndLast.x, i - 1);
        return firstAndLast;
    }
    /// <summary>
    /// takes a string and returns it with every word capitalized
    /// </summary>
    /// <param name="givenSentence"></param>
    /// <returns></returns>
    public static string CapitalizeAllWordsInString(string givenSentence)
    {
        string s = "";
        foreach (string word in givenSentence.Trim().Split(@" "[0]))
        {
            if (word.Length > 0)
                s += char.ToUpper(word[0]) + word.Substring(1) + " ";
        }
        if (s.Length > 0)
            s = s.Substring(0, s.Length - 1); //removes last character
        return s;
    }
    /// <summary>
    /// Takes a Screen Position and changes it to a canvas rednder mode: Camera coordinates
    /// </summary>
    /// <param name="screenPosition"></param>
    /// <returns></returns>
    public static Vector3 LocalScreenToCanvasPosition(Vector3 screenPosition)
    {
        Vector3 canvasPosition = screenPosition / (ReferenceManager.instance.canvas.scaleFactor * 2);
        return canvasPosition;
    }
    public static Vector3 GlobalScreenToCanvasPosition(Vector3 screenPosition)
    {
        Vector3 canvasPosition = Camera.main.WorldToScreenPoint(screenPosition);
        canvasPosition = canvasPosition / (ReferenceManager.instance.canvas.scaleFactor * 2);
        return canvasPosition;
    }
    /// <summary>
    /// Goes through the array and places the Object in the first free Spot
    /// </summary>
    public static void AddToArray(GameObject[] array, GameObject toAdd)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                array[i] = toAdd;
            }
        }
    }
    public static void AddToArray(RefBool[] array, RefBool toAdd)
    {
        bool isFull = true;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].refObject == null)
            {
                array[i] = toAdd;
                isFull = false;
            }
        }
        if (isFull)
            Debug.Log("THIS ARRAY IS FULL");
    }
    public static void RemoveFromArray(RefBool[] array, RefBool toRemove)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == toRemove)
            {
                array[i] = new RefBool();
            }
        }
    }
    public static bool ArrayContains(RefBool[] array, GameObject contains)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].refObject != null && array[i].refObject == contains)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsNotFromACase(BubbleData data)
    {
        return (data.origin == WordInfo.Origin.Dialogue || data.origin == WordInfo.Origin.Environment || data.origin == WordInfo.Origin.Ask);
    }
}
