using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Word : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerClickHandler
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
            // check where it is released
            /*if()
            {
                //if it was dragged into the case, save it
            }
            else if ()
            {
                // if it was dragged onto a prompt, react
            }
            else
            {*/
            //if it was dragged nowhere, destroy it
            WordClickManager.instance.DestroyCurrentWord();
            //}
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       // nothing, but if you delete this, other funtions dont work for some reason?
    }
}

