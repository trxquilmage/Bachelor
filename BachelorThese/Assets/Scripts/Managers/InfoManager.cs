using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class InfoManager : MonoBehaviour
{
    // Saves all relevant Information that was given in previous Dialogues;

    public static InfoManager instance;

    public List<Rumor> allRumors = new List<Rumor>();

    //Dictionary containing all names of visited nodes
    [HideInInspector] public Dictionary<string, List<string>> visitedNodes = new Dictionary<string, List<string>>();
    private void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// Save the given ifomation as a Rumor and put it into the list
    /// </summary>
    /// <param name="variableName"></param>
    /// <param name="valueToSave"></param>
    /// <param name="nameOfNPC"></param>
    public void SaveInfo(string variableName, Yarn.Value valueToSave, string nameOfNPC, Bubble.TagObject tagObj)
    {

        Rumor rumor = FindCorrectRumor(variableName, out int foundIndex);
        rumor.rumorName = variableName;
        rumor.value = valueToSave;
        rumor.toldTo = new Yarn.Value(nameOfNPC);
        rumor.tagObject = tagObj;
        if (foundIndex == -1)
            allRumors.Add(rumor);
        else
            allRumors[foundIndex] = rumor;
    }
    /// <summary>
    /// returns info, int chosenValue => toldTo = 1; value = 0
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    public Yarn.Value GetInfo(string variableName, int chosenValue)
    {
        if (chosenValue == 0)
            return FindCorrectRumor(variableName, out int foundIndex).value;
        else
            return FindCorrectRumor(variableName, out int foundIndex).toldTo;
    }
    /// <summary>
    /// returns a value Yarn.Value saved in data. Looking for the index of the vairable in their tag object
    /// </summary>
    /// <param name="data"></param>
    /// <param name="lookingFor"></param>
    /// <returns></returns>
    public Yarn.Value FindValue(BubbleData data, string lookingFor)
    {
        if (data is WordData)
        {
            Bubble.TagObject tagObject = ((WordData)data).tagObj;
            int i = WordLookupReader.instance.CheckForSubtags((WordData)data, lookingFor);
            return tagObject.allGivenValues[i];
        }
        else
            Debug.Log("This shouldnt happen");
        return new Yarn.Value();
    }
    /// <summary>
    /// for get info, find the rumor we are looking for
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    Rumor FindCorrectRumor(string variableName, out int foundIndex)
    {
        int i = 0;
        foreach (Rumor rumor in allRumors)
        {
            if (rumor.rumorName == variableName)
            {
                foundIndex = i;
                return rumor;
            }
            i++;
        }
        foundIndex = -1;
        return new Rumor() { rumorName = variableName, toldTo = new Yarn.Value(), value = new Yarn.Value(), tagObject = new Bubble.TagObject() };
    }
}
//Saves ANY given information in a Rumor, containing the value's name,
//it's value as a Yarn.Value and the name of the character the Info was told
public class Rumor
{
    public string rumorName;
    public Yarn.Value toldTo;
    public Yarn.Value value;
    public Bubble.TagObject tagObject;
}
