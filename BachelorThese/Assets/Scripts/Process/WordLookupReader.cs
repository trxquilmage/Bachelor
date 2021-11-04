using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class WordLookupReader : MonoBehaviour
{
    public static WordLookupReader instance;
    string pathWord, pathLongWord, pathQuestion, pathTagLookup, allWords, allLongWords, allQuestions, allTags;
    [HideInInspector]
    public Dictionary<string, string[]> wordTag = new Dictionary<string, string[]>();
    public Dictionary<string[], string[]> longWordTagSingular = new Dictionary<string[], string[]>();
    public Dictionary<string, string[]> longWordTag = new Dictionary<string, string[]>();
    public Dictionary<string, string[]> questionTag = new Dictionary<string, string[]>();
    public Dictionary<WordInfo.WordTags, string[]> tagSubtag = new Dictionary<WordInfo.WordTags, string[]>();
    List<TMP_WordInfo> currentWordList;
    string[] currentLongWord;
    int currentLongWordIndex;
    private void Awake()
    {
        instance = this;
        questionTag = new Dictionary<string, string[]>();
    }
    private void Start()
    {
        LookUpWord();
        LookUpLongWord();
        LookUpQuestion();
        LookUpTag();
        currentWordList = new List<TMP_WordInfo>();
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
                string[] textPrompts = new string[1]{
                    lineData[1]
                };
                questionTag.Add(lineData[0], textPrompts);
            }
        }
    }
    void LookUpLongWord()
    {
        pathLongWord = Application.dataPath + "/Data/LongWord-LookupTable.csv";
        allLongWords = File.ReadAllText(pathLongWord);
        string[] lines = allLongWords.Split("\n"[0]);
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
                string[] allWords = lineData[0].Trim().Split(" "[0]);

                longWordTagSingular.Add(allWords, lineInfo);
                longWordTag.Add(lineData[0], lineInfo);
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
    public int CheckForSubtags(Word.WordData data, string lookingFor)
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
    public bool CheckForWord(TMP_WordInfo wordInfo, out TMP_WordInfo[] wordCollection)
    {
        wordCollection = null;
        bool isWord = false;
        if (currentWordList.Count != 0) // doing this first block out the chance for a word list to be broken up
        {
            currentLongWordIndex++;
            if (currentLongWordIndex < currentLongWord.Length)
            {
                if (currentLongWord[currentLongWordIndex] == wordInfo.GetWord())
                {
                    currentWordList.Add(wordInfo);
                }
                else
                {
                    // this wasnt actually a word
                    currentLongWord = null;
                    currentWordList = new List<TMP_WordInfo>();
                    currentLongWordIndex = 0;
                }
            }
            if (currentLongWord != null && currentLongWordIndex + 1 == currentLongWord.Length) //finish off word #1
            {
                isWord = true;
                wordCollection = currentWordList.ToArray();
                currentLongWord = null;
                currentWordList = new List<TMP_WordInfo>();
                currentLongWordIndex = 0;
            }
        }
        if (wordTag.ContainsKey(wordInfo.GetWord()))
        {
            isWord = true;
            wordCollection = new TMP_WordInfo[] { wordInfo };
            //Might be temporary, but delete long word progress, when finding a short word
            currentLongWord = null;
            currentWordList = new List<TMP_WordInfo>();
            currentLongWordIndex = 0;
        }
        else if (currentWordList.Count == 0) //needs a first word
        {
            foreach (string[] words in longWordTagSingular.Keys)
            {
                if (words[0] == wordInfo.GetWord())
                {
                    currentLongWord = words;
                    currentWordList.Add(wordInfo);
                    currentLongWordIndex = 0;
                    break;
                }
            }
        }

        return isWord;
    }
    public bool CheckForUnimportantWord(TMP_WordInfo wordInfo, out TMP_WordInfo[] wordInfos)
    {
        wordInfos = new TMP_WordInfo[0];
        return false;
    }
}
