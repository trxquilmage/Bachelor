using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TagButtonInfo : MonoBehaviour, IPointerClickHandler
{
    public int tagIndex;
    public WordInfo.WordTag wordTag;
    public void Initizalize(int i, WordInfo.WordTag tag)
    {
        tagIndex = i;
        wordTag = tag;

        // Color correctly
        Image img = GetComponent<Image>();
        img.color = tag.tagColor;

        // Scale Correctly
        TMP_Text text = GetComponentInChildren<TMP_Text>();
        RectTransform rT = GetComponent<RectTransform>();

        //apply text into the buttons
        if (tag.name == ReferenceManager.instance.wordTags[ReferenceManager.instance.allTagIndex].name) // if is "AllWords"
            text.text = "All";
        else if (tag.name == "Adjective") // if is "AllWords"
            text.text = "Adj.";
        else if (tag.name == "Location") // if is "AllWords"
            text.text = "Places";
        else
            text.text = tag.name;

        text.ForceMeshUpdate();
        Vector3[] parameters = WordUtilities.GetWordParameters(text, text.textInfo.wordInfo[0], false);
        rT.sizeDelta = new Vector2(parameters[1].x + 10, rT.sizeDelta.y);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        WordCaseManager.instance.ChangeToTag(this);
    }
}
