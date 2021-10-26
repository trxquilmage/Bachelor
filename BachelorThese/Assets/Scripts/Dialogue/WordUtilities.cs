using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

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
            case WordInfo.WordTags.Item:
                color = refM.itemColor;
                break;
            case WordInfo.WordTags.Name:
                color = refM.nameColor;
                break;
            case WordInfo.WordTags.All:
                color = refM.allColor;
                break;
            case WordInfo.WordTags.None:
                Debug.Log("this tag doesnt exist");
                break;
        }
        return color;
    }
    public static void ColorTag(GameObject word, WordInfo.WordTags tag)
    {
        word.GetComponent<Image>().color = MatchColorToTag(tag);
    }
    /// <summary>
    /// Creates a word-Object at the mouse's position
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tag"></param>
    /// <param name="wordMousePos"></param>
    public static GameObject CreateWord(Word.WordData data, Vector2 wordMousePos, TMP_WordInfo wordInfo, WordInfo.Origin origin)
    {
        ReferenceManager refHandler = ReferenceManager.instance;
        GameObject word = GameObject.Instantiate(refHandler.selectedWordPrefab, wordMousePos, Quaternion.identity);
        word.transform.SetParent(refHandler.selectedWordParent.transform, false); // the false makes sure it isnt some random size
        word.transform.position = wordMousePos;
        Word wordScript = word.AddComponent<Word>();
        wordScript.Initialize(data.name, data.tagInfo, origin, wordInfo, true);
        return word;
    }
    public static GameObject CreateWord(Word.WordData data, Vector2 wordMousePos, WordInfo.Origin origin)
    {
        ReferenceManager refHandler = ReferenceManager.instance;
        GameObject word = GameObject.Instantiate(refHandler.selectedWordPrefab, wordMousePos, Quaternion.identity);
        word.transform.SetParent(refHandler.selectedWordParent.transform, false); // the false makes sure it isnt some random size
        word.transform.position = wordMousePos;
        Word wordScript = word.AddComponent<Word>();
        wordScript.Initialize(data.name, data.tagInfo, origin, new TMP_WordInfo(), false);
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
            case "Item":
                return WordInfo.WordTags.Item;
            case "Name":
                return WordInfo.WordTags.Name;
            default:
                Debug.Log("Tag Doesnt exist: " + tag);
                return WordInfo.WordTags.None; //for Debug Reasons
        }
    }
    /// <summary>
    /// Colors a word in an TMP_Text in a certain color
    /// </summary>
    /// <param name="text"></param>
    /// <param name="firstLetter"></param>
    /// <param name="lastLetter"></param>
    /// <param name="color"></param>
    public static void ColorAWord(TMP_Text text, int firstLetter, int lastLetter, Color32 color)
    {
        Color32[] newColors = text.mesh.colors32;
        for (int i = firstLetter; i <= lastLetter; i++)
        {
            TMP_CharacterInfo charInfo = text.textInfo.characterInfo[i];
            int vertexIndex = charInfo.vertexIndex;
            int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.
            Color32[] vertexColors = text.textInfo.meshInfo[meshIndex].colors32;

            vertexColors[vertexIndex + 0] = color;
            vertexColors[vertexIndex + 1] = color;
            vertexColors[vertexIndex + 2] = color;
            vertexColors[vertexIndex + 3] = color;

            // Update Geometry
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }
    public static void ColorAWord(Word word, Color color)
    {
        if (word.relatedText != null)
        {
            ColorAWord(word.relatedText, word.relatedWordInfo.firstCharacterIndex, word.relatedWordInfo.lastCharacterIndex, color);
        }
    }
    /// <summary>
    /// Should be called at the start or End of a Line 
    /// Finds all words marked as "Interactable", and colors them
    /// </summary>
    /// <param name="text"></param>
    /// <param name="color"></param>
    /// <param name="wordTagList"></param>
    public static void ColorAllInteractableWords(TMP_Text text, Dictionary<string, string[]> wordTagList)
    {
        for (int i = 0; i < text.textInfo.wordCount; i++)
        {
            TMP_WordInfo wordInfo = text.textInfo.wordInfo[i];
            if (wordTagList.ContainsKey(wordInfo.GetWord()))
            {
                if (CheckIfWordIsUsed(wordInfo, wordTagList))
                {
                    ColorAWord(text, wordInfo.firstCharacterIndex, wordInfo.lastCharacterIndex, ReferenceManager.instance.interactedColor);
                }
                else
                    ColorAWord(text, wordInfo.firstCharacterIndex, wordInfo.lastCharacterIndex, ReferenceManager.instance.interactableColor);
            }
        }
    }
    /// <summary>
    /// Take all colorable texts, check if they are active and color them
    /// </summary>
    public static void ReColorAllInteractableWords()
    {
        foreach (TMP_Text text in DialogueInputManager.instance.interactableTextList)
        {
            if (text.isActiveAndEnabled)
            {
                for (int i = 0; i < text.textInfo.wordCount; i++)
                {
                    TMP_WordInfo wordInfo = text.textInfo.wordInfo[i];
                    if (WordLookupReader.instance.wordTag.ContainsKey(wordInfo.GetWord()))
                    {
                        if (CheckIfWordIsUsed(wordInfo, WordLookupReader.instance.wordTag))
                        {
                            ColorAWord(text, wordInfo.firstCharacterIndex, wordInfo.lastCharacterIndex, ReferenceManager.instance.interactedColor);
                        }
                        else
                            ColorAWord(text, wordInfo.firstCharacterIndex, wordInfo.lastCharacterIndex, ReferenceManager.instance.interactableColor);
                    }
                }
            }
        }
    }
    /// <summary>
    /// Get The Position of a wordInfo that is added to this
    /// </summary>
    /// <param name="text"></param>
    /// <param name="word"></param>
    /// <returns></returns>
    public static Vector2 GetWordPosition(TMP_Text text, TMP_WordInfo word)
    {
        text.ForceMeshUpdate();
        Vector2 wordPosition = new Vector2(0, 0);
        Vector2 lowerLeftCorner = new Vector2(0, 0);

        // Get the StartPosition (lower left corner) of the button
        TMP_CharacterInfo charInfo = text.textInfo.characterInfo[word.firstCharacterIndex];
        int meshIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;
        TMP_Vertex vertexBL = charInfo.vertex_BL;
        lowerLeftCorner = vertexBL.position;

        // Get the StartPosition of the bounds (lower left corner)
        Vector2 lowerLeftTextBox = text.rectTransform.position;
        Canvas can = ReferenceManager.instance.dialogueCanvas;
        float scaleFactor = can.scaleFactor * 2;
        wordPosition = lowerLeftTextBox + (lowerLeftCorner * scaleFactor) - new Vector2(2, 2); //Der Vector Am Ende macht die kleine Verschiebung weg

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
    public static void CreateABubble(TMP_Text text, TMP_WordInfo wordInfo)
    {
        text.ForceMeshUpdate();
        Vector2 wordPosition = GetWordPosition(text, wordInfo);
        WordClickManager.instance.CheckWord(wordInfo.GetWord(), wordPosition, wordInfo);
        // Set the hovered word to interacted
        ColorAllInteractableWords(text, WordLookupReader.instance.wordTag);
    }
    /// <summary>
    /// checks the given text for promt inputs in the type of "\Tag\"
    /// </summary>
    public static void CheckForPromptInputs(TMP_Text text)
    {
        text.ForceMeshUpdate();
        // Variables
        TMP_CharacterInfo[] charInfo = text.textInfo.characterInfo;

        //If the word starts with "\" -> give out the word
        foreach (TMP_WordInfo wordInfo in text.textInfo.wordInfo)
        {
            // wordInfo igores Sonderzeichen in front of words, so iam checking the letter in front of the word
            if (wordInfo.firstCharacterIndex - 1 > -1)
            {
                if (charInfo[wordInfo.firstCharacterIndex - 1].character == @"\"[0])
                {
                    CreatePromptBubble(text, wordInfo);
                }
            }
        }
    }
    /// <summary>
    /// Create a Prompt Bubble above a word in a given text
    /// </summary>
    /// <param name="text"></param>
    /// <param name="wordInfo"></param>
    public static void CreatePromptBubble(TMP_Text text, TMP_WordInfo wordInfo)
    {
        Vector2[] wordParameters = GetWordParameters(text, wordInfo, true);
        GameObject promptBubble = GameObject.Instantiate(ReferenceManager.instance.promptBoxPrefab, wordParameters[0], Quaternion.identity);
        promptBubble.transform.SetParent(ReferenceManager.instance.promptBubbleParent.transform);
        promptBubble.GetComponent<RectTransform>().sizeDelta = wordParameters[1];
        PromptBubble pB = promptBubble.AddComponent<PromptBubble>();
        pB.Initialize(wordInfo.GetWord());
    }
    /// <summary>
    /// Parents the child to the prompt the mouse is currently hovering over
    /// </summary>
    /// <param name="child"></param>
    public static void ParentBubbleToPrompt(GameObject child)
    {
        GameObject promptBubble = DialogueInputManager.instance.promptBubble.gameObject;

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
        TMP_Text relatedText = word.relatedText;

        //delete the bubble
        WordClickManager.instance.DestroyCurrentWord();
        //update text colors
        ColorAllInteractableWords(relatedText, WordLookupReader.instance.wordTag);
    }
    /// <summary>
    /// Check if a word is currently in use, meaning, it is in the word case, is highlighted or is surrently being dragged
    /// </summary>
    /// <param name="wordInfo"></param>
    /// <param name="wordTagList"></param>
    /// <returns></returns>
    static bool CheckIfWordIsUsed(TMP_WordInfo wordInfo, Dictionary<string, string[]> wordTagList)
    {
        bool isUsed = false;
        string tag = wordTagList[wordInfo.GetWord()][0];
        foreach (Word.WordData data in WordCaseManager.instance.tagRelatedWords[StringToTag(tag)]) //for all words in the word's tag in the word case
        {
            string word = wordInfo.GetWord();
            // if it is in the word case, is the currently highlighted word OR is the current word 
            if (data.name != null &&
                data.name == word ||
                WordClickManager.instance.currentWord != null &&
                word == WordClickManager.instance.currentWord.GetComponent<Word>().relatedWordInfo.GetWord() ||
                WordClickManager.instance.wordLastHighlighted != null &&
                word == WordClickManager.instance.wordLastHighlighted.GetComponent<Word>().relatedWordInfo.GetWord())
            {
                isUsed = true;
            }
        }
        return isUsed;
    }
}
