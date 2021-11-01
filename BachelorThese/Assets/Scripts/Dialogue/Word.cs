using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Yarn.Unity;

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
    /// <summary>
    /// 0 = name, 1 = tags[0] (main tag), 2 = tags[1] (sub tag 1) ...
    /// </summary>
    public struct TagObject
    {
        public List<Yarn.Value> allGivenValues;
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

        //initialize the tag Object
        data.tagObj = new TagObject();
        data.tagObj.allGivenValues = new List<Yarn.Value>();
        data.tagObj.allGivenValues.Add(new Yarn.Value(data.name));
        int i = 0;
        foreach (string tag in tags)
        {
            if (i != 0)
            {
                Yarn.Value val = WordUtilities.TransformIntoYarnVal(tag);
                data.tagObj.allGivenValues.Add(val);
            }
            i++; //we dont want the location to be in this
        }
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

        transform.position = WordClickManager.instance.GetMousePos();
        transform.position -= wordSize / 2;
        wasDragged = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        // if mouse was dragging the object and now releases it
        if (eventData.button == PointerEventData.InputButton.Left && wasDragged)
        {
            WordClickManager clickM = WordClickManager.instance;
            // check where it is released
            //if it was dragged into the case, save it
            if (clickM.isActiveAndEnabled)
            {
                if (clickM.mouseOverUIObject == "wordCase")
                {
                    IsOverWordCase();
                }
                else if (clickM.mouseOverUIObject == "trashCan")
                {
                    WordCaseManager.instance.TrashAWord();
                }
                // if it was dragged onto a prompt, react
                else if (clickM.mouseOverUIObject == "playerInput")
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
            transform.SetParent(ReferenceManager.instance.selectedWordParentAsk.transform);
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
        if (WordClickManager.instance.promptBubble.acceptsCurrentWord)
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
        else
            IsOverNothing();
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
}

