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
    public static void MatchColorToTag(GameObject word, WordInfo.WordTags tag)
    {
        word.GetComponent<Image>().color = Color.red;
    }

    /// <summary>
    /// Creates a word-Object at the mouse's position
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tag"></param>
    /// <param name="screenMousePos"></param>
    public static GameObject CreateWord(string name, string tag, Vector2 screenMousePos)
    {
        ReferenceManager refHandler = ReferenceManager.instance;
        GameObject word = GameObject.Instantiate(refHandler.selectedWordPrefab, screenMousePos, Quaternion.identity);
        word.transform.SetParent(refHandler.selectedWordParent.transform);
        Word wordScript = word.AddComponent<Word>();
        wordScript.Initialize(name, StringToTag(tag));
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
        for (int i = firstLetter; i <= lastLetter; i++)
        {
            if (text.textInfo.characterInfo[i].isVisible)
            {
                TMP_CharacterInfo charInfo = text.textInfo.characterInfo[i];
                int vertexIndex = charInfo.vertexIndex;
                int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.
                Color32[] vertexColors = text.textInfo.meshInfo[meshIndex].colors32;

                vertexColors[vertexIndex + 0] = color;
                vertexColors[vertexIndex + 1] = color;
                vertexColors[vertexIndex + 2] = color;
                vertexColors[vertexIndex + 3] = color;
            }
            // Update Geometry
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }
    /// <summary>
    /// Should be called at the start or End of a Line 
    /// Finds all words marked as "Interactable", and colors them
    /// </summary>
    /// <param name="text"></param>
    /// <param name="color"></param>
    /// <param name="wordTagList"></param>
    public static void ColorAllInteractableWords(TMP_Text text, Color color, Dictionary<string, string> wordTagList)
    {
        foreach (TMP_WordInfo wordInfo in text.textInfo.wordInfo)
        {
            if (wordTagList.ContainsKey(wordInfo.GetWord()))
            {
                ColorAWord(text, wordInfo.firstCharacterIndex,
                    wordInfo.lastCharacterIndex, color);

            }
        }
    }
}
