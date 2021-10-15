using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class WordPointer : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TMP_Text referenceText;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            int wordIndex = TMP_TextUtilities.FindIntersectingWord(referenceText, eventData.position, eventData.enterEventCamera);
            if (wordIndex != -1)
            {
                TMP_WordInfo wordInfo = referenceText.textInfo.wordInfo[wordIndex];
                WordClickManager.instance.SendWord(wordInfo.GetWord(), eventData.position);
            }
        }
    }
}
