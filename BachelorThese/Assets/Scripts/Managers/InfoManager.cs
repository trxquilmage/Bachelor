using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoManager : MonoBehaviour
{
    // Saves all relevant Information that was given in previous Dialogues;

    public static InfoManager instance;

    [Header("None of these are meant to be touched.")]
    //strings
    public string placeOfOrigin; //born in

    //bools
    public bool local; // is a local
    public bool isSingle; // not in a relationship
    public bool katherineMother; // katherine is their mom
    public bool likesMayfair; // said, they like Mayfair

    //floats

    //Dictionary containing all names of visited nodes
    [HideInInspector] public Dictionary<string, List<string>> visitedNodes = new Dictionary<string, List<string>>();
    private void Awake()
    {
        instance = this;
    }
    public void SaveInfo(bool info, string saveIn)
    {
        switch (saveIn)
        {
            case "local":
                local = info;
                break;
            case "katherineMother":
                katherineMother = info;
                break;
            case "likesMayfair":
                likesMayfair = info;
                break;
            case "isSingle":
                isSingle = info;
                break;
        }
    }
    public void SaveInfo(string info, string saveIn)
    {
        switch (saveIn)
        {
            case "placeOfOrigin":
                placeOfOrigin = info;
                if (info == "Mayfair")
                    local = true;
                else
                    local = false;
                break;
        }
    }
    public void SaveInfo(float info, string saveIn)
    {
        switch (saveIn)
        {
            /*case "local":
                local = info;
                break;
            case "katherineMother":
                katherineMother = info;
                break;*/
        }
    }
}
