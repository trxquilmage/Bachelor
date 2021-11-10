using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Yarn.Unity;
using System;
using System.Text.RegularExpressions;

public static class WordUtilities
{
    /// <summary>
    /// Match the word to a color fitting the tag
    /// </summary>
    /// <param name="word"></param>
    public static Color MatchColorToTag(WordInfo.WordTags tag)
    {
        ReferenceManager refM = ReferenceManager.instance;
        Color color = Color.magenta;
        switch (tag)
        {
            case WordInfo.WordTags.Location:
                color = refM.locationColor;
                break;
            case WordInfo.WordTags.Other:
                color = refM.otherColor;
                break;
            case WordInfo.WordTags.Name:
                color = refM.nameColor;
                break;
            case WordInfo.WordTags.Item:
                color = refM.itemColor;
                break;
            case WordInfo.WordTags.Quest:
                color = refM.questColor;
                break;
            case WordInfo.WordTags.AllWords:
                color = refM.allColor;
                break;
            case WordInfo.WordTags.None:
                Debug.Log("this tag doesnt exist");
                break;
        }
        return color;
    }

    /// <summary>
    /// Creates a word-Object at the mouse's position
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tag"></param>
    /// <param name="wordMousePos"></param>
    public static GameObject CreateWord(Word.WordData data, Vector3 wordMousePos, TMP_WordInfo wordInfo, Vector2 firstAndLastWordIndex, WordInfo.Origin origin, bool isLongWord)
    {
        bool inAsk = !DialogueInputManager.instance.continueEnabledAsk;
        ReferenceManager refM = ReferenceManager.instance;
        if (!inAsk || inAsk && origin != WordInfo.Origin.Dialogue)
        {
            GameObject word = GameObject.Instantiate(refM.selectedWordPrefab, wordMousePos, Quaternion.identity);
            if (!inAsk)
                word.transform.SetParent(refM.selectedWordParent.transform, false); // the false makes sure it isnt some random size
            else
                word.transform.SetParent(refM.selectedWordParentAsk.transform, false);
            word.GetComponent<RectTransform>().localPosition = wordMousePos;
            Word wordScript = word.AddComponent<Word>();
            wordScript.Initialize(data.name, data.tagInfo, origin, wordInfo, firstAndLastWordIndex, true, isLongWord);
            return word;
        }
        return null;
    }
    public static GameObject CreateWord(Word.WordData data, Vector3 wordMousePos, Vector2 firstAndLastWordIndex, WordInfo.Origin origin, bool isLongWord)
    {
        ReferenceManager refHandler = ReferenceManager.instance;
        GameObject word = GameObject.Instantiate(refHandler.selectedWordPrefab, wordMousePos, Quaternion.identity);
        if (DialogueInputManager.instance.continueEnabledAsk) // not in an ask
            word.transform.SetParent(refHandler.selectedWordParent.transform, false); // the false makes sure it isnt some random size
        else // in an ask
            word.transform.SetParent(refHandler.selectedWordParentAsk.transform, false);

        word.GetComponent<RectTransform>().localPosition = wordMousePos;
        Word wordScript = word.AddComponent<Word>();
        wordScript.Initialize(data.name, data.tagInfo, origin, new TMP_WordInfo(), firstAndLastWordIndex, false, isLongWord);
        return word;
    }
    /// <summary>
    /// Turn A String into a WordInfo.WordTags enum
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static WordInfo.WordTags StringToTag(string tag)
    {
        switch (tag)
        {
            case "Location":
                return WordInfo.WordTags.Location;
            case "Other":
                return WordInfo.WordTags.Other;
            case "Name":
                return WordInfo.WordTags.Name;
            case "Item":
                return WordInfo.WordTags.Item;
            case "Quest":
                return WordInfo.WordTags.Quest;
            case "AllWords":
                return WordInfo.WordTags.AllWords;
            default:
                Debug.Log("Tag Doesnt exist: " + tag);
                return WordInfo.WordTags.None; //for Debug Reasons
        }
    }
    /// <summary>
    /// turns the words "Yes" and "No" into the correstponding bool
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public static bool StringToBool(string word)
    {
        bool affirmative;
        if (word == "Yes")
            affirmative = true;
        else
            affirmative = false;
        return affirmative;
    }
    /// <summary>
    /// Colors a word in an TMP_Text in a certain color
    /// </summary>
    /// <param name="text"></param>
    /// <param name="firstLetter"></param>
    /// <param name="lastLetter"></param>
    /// <param name="color"></param>

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
        Vector2 lowerLeftCorner;

        // Get the StartPosition (lower left corner) of the button
        TMP_CharacterInfo charInfo = text.textInfo.characterInfo[word.firstCharacterIndex];
        TMP_Vertex vertexBL = charInfo.vertex_BL;
        lowerLeftCorner = vertexBL.position;

        // Get the StartPosition of the bounds (lower left corner)

        Vector2 lowerLeftTextBox = Camera.main.WorldToScreenPoint(text.rectTransform.position);
        Canvas can = ReferenceManager.instance.dialogueCanvas;
        float scaleFactor = can.scaleFactor * 2;
        wordPosition = lowerLeftTextBox / (can.scaleFactor * 2) + (lowerLeftCorner * scaleFactor) - new Vector2(3, 3);//Der Vector Am Ende macht die kleine Verschiebung weg

        //Debug.Log(wordPosition);
        return wordPosition;
    }
    /// <summary>
    /// returns the parameters of a word as (lowerLeftcorner.x,lowerLeftcorner.y),(size.x, size.y). 
    /// Can include ignored characters in this format: /Hello/
    /// </summary>
    /// <param name="text"></param>
    /// <param name="word"></param>
    /// <param name="hasIgnoredletters"></param>
    /// <returns></returns>
    public static Vector2[] GetWordParameters(TMP_Text text, TMP_WordInfo word, bool hasIgnoredChars)
    {
        text.ForceMeshUpdate();

        int firstCharacter = word.firstCharacterIndex;
        int lastCharacter = word.lastCharacterIndex;
        if (hasIgnoredChars)
        {
            firstCharacter--;
            lastCharacter++;
        }

        Vector2[] parameters = new Vector2[2] { new Vector2(0, 0), new Vector2(0, 0) };

        // Get the StartPosition (lower left corner) of the button
        TMP_CharacterInfo charInfo = text.textInfo.characterInfo[firstCharacter];
        TMP_CharacterInfo charInfoLast = text.textInfo.characterInfo[lastCharacter];
        TMP_Vertex vertexBL = charInfo.vertex_BL;
        TMP_Vertex vertexTR = charInfoLast.vertex_TR;
        parameters[0] = vertexBL.position;

        // Get the StartPosition of the bounds (lower left corner)
        Vector2 lowerLeftTextBox = text.rectTransform.position;
        Canvas can = ReferenceManager.instance.dialogueCanvas;
        float scaleFactor = can.scaleFactor * 2;
        parameters[0] = lowerLeftTextBox + (parameters[0] * scaleFactor) - new Vector2(2, 2); //Der Vector Am Ende macht die kleine Verschiebung weg

        // Get the length of the word
        parameters[1] = vertexTR.position - vertexBL.position;
        parameters[1] += new Vector2(4, 4); // Für die Verschiebung 3 Zeilen höher
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
    /// checks the given text for promt inputs in the type of "\Tag\"
    /// </summary>
    public static void CheckForPromptInputs(TMP_Text text, TMP_TextInfo textInfo, Transform bubbleParent, PromptBubble[] saveIn)
    {
        text.ForceMeshUpdate();
        // Variables
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
                    CreatePromptBubble(textInfo.textComponent, wordInfo, bubbleParent, saveIn);
                }
            }
        }
    }
    /// <summary>
    /// Create a Prompt Bubble above a word in a given text
    /// </summary>
    /// <param name="text"></param>
    /// <param name="wordInfo"></param>
    public static void CreatePromptBubble(TMP_Text text, TMP_WordInfo wordInfo, Transform bubbleParent, PromptBubble[] saveIn)
    {
        Vector2[] wordParameters = GetWordParameters(text, wordInfo, true);
        GameObject promptBubble = GameObject.Instantiate(ReferenceManager.instance.promptBoxPrefab, wordParameters[0], Quaternion.identity);
        promptBubble.transform.SetParent(bubbleParent);
        promptBubble.GetComponent<RectTransform>().sizeDelta = wordParameters[1];
        PromptBubble pB = promptBubble.AddComponent<PromptBubble>();
        pB.Initialize(wordInfo.GetWord(), saveIn);
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
            bubble.child.GetComponent<Word>().IsOverNothing(); // put the OG word back where it came from
            bubble.child = null;
        }
        child.transform.SetParent(promptBubble.transform);
        float localX = promptBubble.GetComponent<RectTransform>().sizeDelta.x / 2; //center
        localX -= child.GetComponent<RectTransform>().sizeDelta.x / 2; // - half of word case
        child.transform.localPosition = new Vector3(localX, 0);
        bubble.child = child;
    }
    /// <summary>
    /// returns a given word into the text it was taken from
    /// </summary>
    public static void ReturnWordIntoText(Word word)
    {
        //delete the bubble
        WordClickManager.instance.DestroyCurrentWord();
        //update text colors
        EffectUtilities.ReColorAllInteractableWords();
    }
    /// <summary>
    /// Check if a word is currently in use, meaning, it is in the word case, is highlighted or is currently being dragged
    /// </summary>
    /// <param name="wordInfo"></param>
    /// <param name="wordTagList"></param>
    /// <returns></returns>
    public static bool CheckIfWordIsUsed(string wordName, int wordLength, bool isFillerWord)
    {
        wordName = CapitalizeAllWordsInString(wordName);
        Dictionary<string, string[]> wordTagList;
        if (isFillerWord)
        {
            wordTagList = WordLookupReader.instance.fillerTag;
        }
        else if (wordLength == 1)
            wordTagList = WordLookupReader.instance.wordTag;
        else
            wordTagList = WordLookupReader.instance.longWordTag;
        bool isUsed = false;
        string tagName = "";
        if (wordTagList.ContainsKey(wordName))
            tagName = wordTagList[wordName][0];
        else
            tagName = "Other";
        WordInfo.WordTags tag = StringToTag(tagName);

        if (!ReferenceManager.instance.noGreyOut) //greys out everything used
        {
            if (tag == WordInfo.WordTags.Quest)
            {
                //if it's in the quest case or the quest case is full
                if (!QuestManager.instance.CheckIfCanSaveQuest(wordName, out int index))
                {
                    isUsed = true;
                }
            }
            else
            {
                //if it's in the word case or the word case is full
                if (!WordCaseManager.instance.CheckIfCanSaveWord(wordName, tag, out int index))
                {
                    isUsed = true;
                }
            }

            // if it is the currently highlighted word OR is the current word 
            if (WordClickManager.instance.currentWord != null &&
                wordName == WordClickManager.instance.currentWord.GetComponent<Word>().data.name)
            {
                isUsed = true;
            }
        }
        //otherwise only grey out the word that is being highlighted night now
        if (WordClickManager.instance.wordLastHighlighted != null &&
            wordName == WordClickManager.instance.wordLastHighlighted.GetComponent<Word>().data.name)
        {
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
                runner.StartDialogue(runner.startNode = node);
            else
                runner.StartDialogue(runner.startNode = DialogueManager.instance.currentTarget.characterName + ".Catchall");
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
            s += char.ToUpper(word[0]) + word.Substring(1) + " ";
        }
        s = s.Substring(0, s.Length - 1); //removes last character
        return s;
    }
}
