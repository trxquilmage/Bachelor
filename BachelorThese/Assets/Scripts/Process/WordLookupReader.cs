using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WordLookupReader : MonoBehaviour
{
    public static WordLookupReader instance;
    string pathWord, pathQuestion, allWords, allQuestions;
    [HideInInspector]
    public Dictionary<string, string> wordTag = new Dictionary<string, string>(), questionTag = new Dictionary<string, string>();
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        pathWord = Application.dataPath + "/Data/Word-LookupTable.csv";
        allWords = File.ReadAllText(pathWord);
        string[] lines = allWords.Split("\n"[0]);
        foreach (string s in lines)
        {
            if (s != "")
            {
                string[] lineData = s.Trim().Split(";"[0]);
                wordTag.Add(lineData[0], lineData[1]);
            }
        }

        pathQuestion = Application.dataPath + "/Data/Question-LookupTable.csv";
        allQuestions = File.ReadAllText(pathQuestion);
        string[] linesQ = allQuestions.Split("\n"[0]);
        foreach (string s in linesQ)
        {
            if (s != "")
            {
                string[] lineData = s.Trim().Split(";"[0]);
                questionTag.Add(lineData[0], lineData[1]);
            }
        }
    }
}
