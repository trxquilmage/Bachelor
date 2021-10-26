using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Word : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerClickHandler, IPointerDownHandler
{
    TMP_Text nameText;
    bool wasDragged; //after the mouse goes up, after it was dragged it checks, where the object is now at
    public WordData data;

    public TMP_WordInfo relatedWordInfo;
    public TMP_Text relatedText;
    Vector3 wordSize;
    public struct WordData
    {
        public string name;
        public string[] tagInfo;
        public WordInfo.WordTags tag;
        public WordInfo.Origin origin;
        public TagObject tagObj;
    }
    public struct TagObject
    {
        public Location loc;
    }
    public struct Location
    {
        public string position;
    }
    /// <summary>
    /// Call this, when creating a Word. If you dont have a Word Info, create one and set "hasWordInfo" to false
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tags"></param>
    public void Initialize(string name, string[] tags, WordInfo.Origin origin, TMP_WordInfo wordInfo, bool hasWordInfo)
    {
        data = new WordData();
        data.name = name;
        data.tagInfo = tags;
        data.tag = WordUtilities.StringToTag(tags[0]);
        data.origin = origin;
        if (hasWordInfo)
        {
            relatedWordInfo = wordInfo;
            relatedText = wordInfo.textComponent;
        }

        nameText = transform.GetComponentInChildren<TMP_Text>();
        nameText.text = name;
        ScaleRect();
        wordSize = this.GetComponent<RectTransform>().sizeDelta;
        WordUtilities.ColorTag(this.gameObject, data.tag);

        data.tagObj = new TagObject() { };
        FindCorrectTag(data.tagInfo);
    }
    /// <summary>
    /// Scale the picked up word, so that the rect of the background fits the word in the center
    /// </summary>
    void ScaleRect()
    {
        nameText.ForceMeshUpdate();
        Bounds bounds = nameText.textBounds;
        float width = bounds.size.x;
        width = width + 4;
        RectTransform rT = this.GetComponent(typeof(RectTransform)) as RectTransform;
        rT.sizeDelta = new Vector2(width, rT.sizeDelta.y);
    }
    public void OnDrag(PointerEventData eventData)
    {
        //drag the object through the scene

        transform.position = DialogueInputManager.instance.GetMousePos();
        transform.position -= wordSize / 2;
        wasDragged = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        // if mouse was dragging the object and now releases it
        if (eventData.button == PointerEventData.InputButton.Left && wasDragged)
        {
            DialogueInputManager input = DialogueInputManager.instance;
            // check where it is released
            //if it was dragged into the case, save it
            if (input.isActiveAndEnabled)
            {
                if (input.mouseOverUIObject == "wordCase")
                {
                    IsOverWordCase();
                }
                else if (input.mouseOverUIObject == "trashCan")
                {
                    WordCaseManager.instance.TrashAWord();
                }
                // if it was dragged onto a prompt, react
                else if (input.mouseOverUIObject == "playerInput")
                {
                    IsOverPlayerInput();
                }
                else
                {
                    IsOverNothing();
                }
            }

        }
        if (data.origin == WordInfo.Origin.WordCase)
        {
            WordCaseManager.instance.DestroyWordReplacement();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // this cant be deleted bc for some reasons the other functions dont work without it
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // This is basically OnDragStart()

        if (transform.parent.TryGetComponent<PromptBubble>(out PromptBubble pB)) //if currently attached to a prompt bubble
        {
            pB.child = null;
        }

        // if the word is being dragged out of the dialogue
        if (data.origin == WordInfo.Origin.Dialogue)
        {
            WordCaseManager.instance.AutomaticOpenCase(true);
            WordClickManager.instance.currentWord = this.gameObject;
            WordClickManager.instance.wordLastHighlighted = null;
            WordCaseManager.instance.openTag = data.tag;
        }
        else if (data.origin == WordInfo.Origin.WordCase)
        {
            transform.SetParent(ReferenceManager.instance.selectedWordParent.transform);
            //Delete Word from case list
            WordClickManager.instance.currentWord = this.gameObject;
            WordCaseManager.instance.SpawnWordReplacement(this);
        }

    }
    /// <summary>
    /// The bubble was dragged onto the word case and dropped
    /// </summary>
    void IsOverWordCase()
    {
        if (data.origin == WordInfo.Origin.Dialogue)
        {
            //save it
            WordCaseManager.instance.SaveWord(WordClickManager.instance.currentWord.GetComponent<Word>());

            //close the case & Delete the UI word
            WordCaseManager.instance.AutomaticOpenCase(false);
            WordClickManager.instance.DestroyCurrentWord();
        }
        else if (data.origin == WordInfo.Origin.WordCase)
        {
            // put it back
            WordCaseManager.instance.PutWordBack(this);
        }
    }
    /// <summary>
    /// The bubble was dragged onto a prompt case and dropped
    /// </summary>
    void IsOverPlayerInput()
    {
        if (data.origin == WordInfo.Origin.Dialogue)
        {
            //parent to word
            WordUtilities.ParentBubbleToPrompt(this.gameObject);
            //close wordCase
            WordCaseManager.instance.AutomaticOpenCase(false);
        }
        else if (data.origin == WordInfo.Origin.WordCase)
        {
            // parent to word
            WordUtilities.ParentBubbleToPrompt(this.gameObject);
        }
    }
    /// <summary>
    /// The bubble was dragged onto nothing and dropped
    /// </summary>
    public void IsOverNothing()
    {
        if (data.origin == WordInfo.Origin.Dialogue)
        {
            //save it
            WordCaseManager.instance.SaveWord(WordClickManager.instance.currentWord.GetComponent<Word>());

            //close the case & Delete the UI word
            WordCaseManager.instance.AutomaticOpenCase(false);
            WordClickManager.instance.DestroyCurrentWord();
        }
        else if (data.origin == WordInfo.Origin.WordCase)
        {
            // put it back
            WordCaseManager.instance.PutWordBack(this);
        }
    }
    /// <summary>
    /// Find the correct tag and add all subtags
    /// </summary>
    /// <param name="tagObj"></param>
    /// <param name="tags"></param>
    void FindCorrectTag(string[] tags)
    {
        switch (data.tag)
        {
            case WordInfo.WordTags.Location:
                data.tagObj.loc = new Location()
                {
                    position = tags[1]
                };
                break;
            default:
                Debug.Log("you didnt add something here");
                break;
        }
    }
}

