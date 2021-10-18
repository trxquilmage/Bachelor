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
    public struct WordData
    {
        public string name;
        public WordInfo.WordTags tag;
    }
    /// <summary>
    /// Call this, when creating a Word
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tag"></param>
    public void Initialize(string name, WordInfo.WordTags tag)
    {
        WordData data = new WordData();
        data.name = name;
        data.tag = tag;
        nameText = transform.GetComponentInChildren<TMP_Text>();
        nameText.text = name;
        ScaleRect();
        WordUtilities.MatchColorToTag(this.gameObject, tag);
    }
    /// <summary>
    /// Scale the picked up word, so that the rect of the background fits the word in the center
    /// </summary>
    void ScaleRect()
    {
        //this doesnt work, bc the bounds of the transform stay the same

        /*
        nameText.UpdateMeshPadding();
        nameText.ForceMeshUpdate();
        Vector2 size = new Vector2(nameText.rectTransform.rect.width + 10,
            nameText.rectTransform.rect.height + 5);
        RectTransform rT = this.GetComponent(typeof(RectTransform)) as RectTransform;
        rT.sizeDelta = size;
        */
    }
    public void OnDrag(PointerEventData eventData)
    {
        //drag the object through the scene
        transform.position = DialogueInputManager.instance.GetMousePos();
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
                    //save it
                    Debug.Log("Save Current Word");

                    //close the case & Delete the UI word
                    WordCaseManager.instance.OpenCase(false);
                    WordClickManager.instance.DestroyCurrentWord();
                }
                // if it was dragged onto a prompt, react
                else if (input.mouseOverUIObject == "playerInput")
                {
                    //check if there is a promt to be filled right now
                    Debug.Log("Player Input");
                    //close wordCase
                    WordCaseManager.instance.OpenCase(false);
                }
                else
                {
                    //if it was dragged nowhere, destroy it
                    WordClickManager.instance.DestroyCurrentWord();
                    WordCaseManager.instance.OpenCase(false);
                }
            }

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // this cant be deleted bc for some reasons the other functions dont work without it
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // This is basically OnDragStart()
        WordCaseManager.instance.OpenCase(true);
    }
}

