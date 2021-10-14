using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordClickManager : MonoBehaviour
{
    public static WordClickManager instance;
    public List<TextMeshProUGUI> availableTexts;
    string s;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        /*if (DialogueInputManager.instance.isActiveAndEnabled)
        {
            TextMeshProUGUI text = availableTexts[0];

            Vector3 worldPoint;
            if (TMP_TextUtilities.ScreenPointToWorldPointInRectangle(availableTexts[0].transform,
                DialogueInputManager.instance.GetCurrentMousePosition(), Camera.main, out worldPoint))
            {
                int i = TMP_TextUtilities.FindIntersectingWord(text, worldPoint, Camera.main);
                if (i != -1)
                {
                    string s = availableTexts[0].textInfo.wordInfo[i].GetWord();
                    Debug.Log(s);
                }
            }

        }*/
    }



    void OnHover()
    {

    }
    void OnClick()
    {

    }
}
