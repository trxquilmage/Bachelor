using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarFavorite : MonoBehaviour
{
    [SerializeField] Color normalColor, clickedColor;
    [SerializeField] int offsetToText = 16;
    Image image;
    TMP_Text text;
    Word word;

    void Awake()
    {
        text = transform.parent.GetComponent<TMP_Text>();
        image = text.transform.parent.GetComponent<Image>();
        word = image.transform.parent.parent.GetComponent<Word>();
        text.ForceMeshUpdate();
        TMP_TextInfo textInfo = text.textInfo;

        if (textInfo.characterCount > 0)
        {
            float endOfRightmostCharacter = textInfo.characterInfo[textInfo.characterCount - 1].bottomRight.x;
            transform.localPosition = new Vector3(endOfRightmostCharacter + offsetToText, transform.localPosition.y, transform.localPosition.z);
            image = GetComponent<Image>();
            UpdateStar();
        }
    }
    public void ClickedFavorite()
    {
        word.data.isFavorite = !word.data.isFavorite;
        UpdateStar();
        FavoriteButtonInfo favoriteButton = ReferenceManager.instance.favoritesButton.GetComponent<FavoriteButtonInfo>();
        Debug.Log(WordCaseManager.instance.GetFavoritesCount());
        if (WordCaseManager.instance.GetFavoritesCount() > 0)
            favoriteButton.SetActive();
        else
            favoriteButton.SetInactive();
    }
    void UpdateStar()
    {
        image.color = word.data.isFavorite ? clickedColor : normalColor;
    }
}

