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
    TMP_Text relatedText;
    Image relatedImage;
    RectTransform relatedTransform;
    ReferenceManager refM;

    public void Initialize(int index, WordInfo.WordTag tag)
    {
        SetAllClassVariablesVariables(index, tag);
        ColorButtonToTagColor();
        ReplaceWordsForBrevity();
        ScaleButtonToAlignWithTextLength();
        SaveButtonAsReferenceInTagList();
        SetInactive();
        StartWithProminentAllButton();
    }
    void SetAllClassVariablesVariables(int i, WordInfo.WordTag tag)
    {
        refM = ReferenceManager.instance;
        tagIndex = i;
        wordTag = tag;
        relatedImage = GetComponent<Image>();
        relatedText = GetComponentInChildren<TMP_Text>();
        relatedTransform = GetComponent<RectTransform>();
    }
    void ColorButtonToTagColor()
    {
        relatedImage.color = wordTag.tagColor;
    }
    void ScaleButtonToAlignWithTextLength()
    {
        Vector3[] parameters = WordUtilities.GetWordParameters(relatedText, relatedText.textInfo.wordInfo[0], false);
        relatedTransform.sizeDelta = new Vector2(parameters[1].x + 25, relatedTransform.sizeDelta.y);
    }
    void ReplaceWordsForBrevity()
    {
        //apply text into the buttons
        if (wordTag.name == ReferenceManager.instance.wordTags[ReferenceManager.instance.allTagIndex].name) // if is "AllWords"
            relatedText.text = "All";
        else if (wordTag.name == "Adjective") // if is "AllWords"
            relatedText.text = "Adj.";
        else if (wordTag.name == "Location") // if is "AllWords"
            relatedText.text = "Places";
        else
            relatedText.text = wordTag.name;

        relatedText.ForceMeshUpdate();
    }
    void SaveButtonAsReferenceInTagList()
    {
        ReferenceManager.instance.wordTags[tagIndex].tagButton = GetComponent<Button>();
    }
    public void SetInactive()
    {
        if (tagIndex != refM.allTagIndex) // if is "AllWords"
        {
            this.gameObject.SetActive(false);
            refM.wordTags[tagIndex].buttonIsActive = false;
        }
        else
            refM.wordTags[tagIndex].buttonIsActive = true;
    }
    public void SetActive()
    {
        this.gameObject.SetActive(true);
        refM.wordTags[tagIndex].buttonIsActive = true;
    }
    void StartWithProminentAllButton()
    {
        if (wordTag.name == ReferenceManager.instance.wordTags[ReferenceManager.instance.allTagIndex].name)
        {
            WordCaseManager.instance.ChangeToTag(this);
            MakeTagButtonProminent();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        WordCaseManager.instance.ChangeToTag(this);
    }
    public void MakeTagButtonSmaller()
    {
        if (relatedImage != null)
        {
            relatedImage.sprite = ReferenceManager.instance.buttonNotSelected;
            relatedText.fontStyle = FontStyles.Normal;
        }
    }
    public void MakeTagButtonProminent()
    {
        if (relatedImage != null)
        {
            relatedImage.sprite = ReferenceManager.instance.buttonSelected;
            relatedText.fontStyle = FontStyles.Bold;
        }
    }
}
