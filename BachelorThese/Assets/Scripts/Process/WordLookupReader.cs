using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WordLookupReader : MonoBehaviour
{
    public static WordLookupReader instance;
    string pathWord, pathQuestion, pathTagLookup, allWords, allQuestions, allTags;
    [HideInInspector]
    public Dictionary<string, string[]> wordTag = new Dictionary<string, string[]>();
    public Dictionary<string, string[]> questionTag = new Dictionary<string, string[]>();
    public Dictionary<WordInfo.WordTags, string[]> tagSubtag = new Dictionary<WordInfo.WordTags, string[]>();
    private void Awake()
    {
        instance = this;
        questionTag = new Dictionary<string, string[]>();
    }
    private void Start()
    {
        LookUpWord();
        LookUpQuestion();
        LookUpTag();
    }
    /// <summary>
    /// Go through the excel sheet & save the info
    /// </summary>
    void LookUpQuestion()
    {
        pathQuestion = Application.dataPath + "/Data/Question-LookupTable.csv";
        allQuestions = File.ReadAllText(pathQuestion);
        string[] linesQ = allQuestions.Split("\n"[0]);
        foreach (string s in linesQ)
        {
            if (s != "")
            {
                string[] lineData = s.Trim().Split(";"[0]);
                string[] textPrompts = new string[2]{
                    lineData[1], lineData[2]
                };
                questionTag.Add(lineData[0], textPrompts);
            }
        }
    }
    void LookUpWord()
    {
        pathWord = Application.dataPath + "/Data/Word-LookupTable.csv";
        allWords = File.ReadAllText(pathWord);
        string[] lines = allWords.Split("\n"[0]);
        foreach (string s in lines)
        {
            if (s != "")
            {
                string[] lineData = s.Trim().Split(";"[0]);
                string[] lineInfo = new string[lineData.Length - 1];
                for (int i = 0; i < lineInfo.Length; i++)
                {
                    lineInfo[i] = lineData[i + 1];
                }
                wordTag.Add(lineData[0], lineInfo);
            }
        }

    }
    void LookUpTag()
    {
        pathTagLookup = Application.dataPath + "/Data/Tag-Subtag-Table.csv";
        allTags = File.ReadAllText(pathTagLookup);
        string[] lines = allTags.Split("\n"[0]);
        foreach (string s in lines)
        {
            if (s != "")
            {
                string[] lineData = s.Trim().Split(";"[0]);
                string[] lineInfo = new string[lineData.Length - 1];
                for (int i = 0; i < lineInfo.Length; i++)
                {
                    lineInfo[i] = lineData[i + 1];
                }
                tagSubtag.Add(WordUtilities.StringToTag(lineData[0]), lineInfo);
            }
        }
    }
    /// <summary>
    /// Returns the index of the word that is being looked for
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int CheckAgainstList(Word.WordData data, string lookingFor)
    {
        //check the dictionary for the tag
        int i = 0;
        foreach (string subtag in tagSubtag[data.tag])
        {
            if (subtag == lookingFor)
                return i;
            i++;
        }
        //get the index list from it
        Debug.Log("Found nothing");
        return 0;
    }
}
