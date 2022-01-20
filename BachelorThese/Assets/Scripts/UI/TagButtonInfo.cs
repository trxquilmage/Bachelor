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
    protected TMP_Text relatedText;
    protected Image relatedImage;
    protected RectTransform relatedTransform;
    protected ReferenceManager refM;

    public virtual void Awake()
    {

    }
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
    protected void SetAllClassVariablesVariables(int i, WordInfo.WordTag tag)
    {
        refM = ReferenceManager.instance;
        tagIndex = i;
        wordTag = tag;
        relatedImage = GetComponent<Image>();
        relatedText = GetComponentInChildren<TMP_Text>();
        relatedTransform = GetComponent<RectTransform>();
    }
    protected void ColorButtonToTagColor()
    {
        relatedImage.color = wordTag.tagColor;
    }
    protected void ScaleButtonToAlignWithTextLength()
    {
        Vector3[] parameters = WordUtilities.GetWordParameters(relatedText, relatedText.textInfo.wordInfo[0], false);
        relatedTransform.sizeDelta = new Vector2(parameters[1].x + 25, relatedTransform.sizeDelta.y);
    }
    protected void ReplaceWordsForBrevity()
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
    protected void SaveButtonAsReferenceInTagList()
    {
        ReferenceManager.instance.wordTags[tagIndex].tagButton = GetComponent<Button>();
    }
    public virtual void SetInactive()
    {
        if (tagIndex != refM.allTagIndex) // if is "AllWords"
        {
            this.gameObject.SetActive(false);
            refM.wordTags[tagIndex].buttonIsActive = false;
        }
        else
            refM.wordTags[tagIndex].buttonIsActive = true;
    }
    public virtual void SetActive()
    {
        this.gameObject.SetActive(true);
        refM.wordTags[tagIndex].buttonIsActive = true;
    }
    protected void StartWithProminentAllButton()
    {
        if (wordTag.name == ReferenceManager.instance.wordTags[ReferenceManager.instance.allTagIndex].name)
        {
            WordCaseManager.instance.ChangeToTag(this);
            MakeTagButtonProminent();
        }
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        WordCaseManager.instance.ChangeToTag(this);
    }
    public void MakeTagButtonSmaller()
    {
        if (relatedImage != null)
        {
            relatedImage.sprite = ReferenceManager.instance.buttonNotSelected;
            if (relatedText != null)
                relatedText.fontStyle = FontStyles.Normal;
        }
    }
    public void MakeTagButtonProminent()
    {
        if (relatedImage != null)
        {
            relatedImage.sprite = ReferenceManager.instance.buttonSelected;
            if (relatedText != null)
                relatedText.fontStyle = FontStyles.Bold;
        }
    }
}
