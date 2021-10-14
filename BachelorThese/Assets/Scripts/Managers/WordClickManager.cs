using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class WordClickManager : MonoBehaviour//, IPointerClickHandler
{
    public static WordClickManager instance;
    [SerializeField] private TextMeshProUGUI targetLabel;

    void Awake()
    {
        instance = this;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked");
        int linkIndex = TMP_TextUtilities.FindIntersectingWord(targetLabel, eventData.position, eventData.enterEventCamera);
        if (linkIndex != -1)
        {
            Debug.Log($"click on link: {eventData.position}, {eventData.enterEventCamera}", eventData.enterEventCamera);

            TMP_LinkInfo linkInfo = targetLabel.textInfo.linkInfo[linkIndex];
        }
    }

    /*public void OnPointerEnter(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(targetLabel, eventData.position, eventData.enterEventCamera);
        if (linkIndex != -1)
        {
            Debug.Log($"start hovering over {targetLabel.textInfo.linkInfo[linkIndex].GetLinkID()}");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(targetLabel, eventData.position, eventData.enterEventCamera);
        if (linkIndex != -1)
        {
            Debug.Log($"stop hovering over {targetLabel.textInfo.linkInfo[linkIndex].GetLinkID()}");
            SetLinkToColor(linkIndex, Constants.UI.Colors.SelectedBlack);
        }
    }*/

    /*void Update()
    {
        if (DialogueInputManager.instance.isActiveAndEnabled)
        {
            TextMeshProUGUI text = targetLabel;

            Vector3 worldPoint;
            if (TMP_TextUtilities.ScreenPointToWorldPointInRectangle(targetLabel.transform,
                DialogueInputManager.instance.GetCurrentMousePosition(), Camera.main, out worldPoint))
            {
                int i = TMP_TextUtilities.FindIntersectingWord(text, worldPoint, Camera.main);
                if (i != -1)
                {
                    string s = targetLabel.textInfo.wordInfo[i].GetWord();
                    Debug.Log(s);
                }
            }

        }
    }*/



    void OnHover()
    {

    }
    void OnClick()
    {

    }
}
