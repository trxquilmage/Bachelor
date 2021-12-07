using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarFavorite : MonoBehaviour
{
    [SerializeField] Color normalColor, clickedColor;
    Image image;
    TMP_Text text;
    Word word;
    void Start()
    {
        word = transform.parent.parent.GetComponent<Word>();
        text = transform.parent.GetComponent<TMP_Text>();
        float positionX = text.textInfo.characterInfo[text.textInfo.characterCount - 1].bottomRight.x;
        transform.localPosition = new Vector3(positionX + 16, transform.localPosition.y, transform.localPosition.z);
        image = GetComponent<Image>();
        UpdateStar();
    }
    public void ClickedFavorite()
    {
        word.data.isFavorite = !word.data.isFavorite;
        UpdateStar();
    }
    public void UpdateStar()
    {
        image.color = word.data.isFavorite ? clickedColor : normalColor;
    }
}
