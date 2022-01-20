using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class FavoriteButtonInfo : TagButtonInfo
{
    public override void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        SetAllClassVariablesVariables();
        SetInactive();
    }
    protected void SetAllClassVariablesVariables()
    {
        refM = ReferenceManager.instance;
        relatedImage = GetComponent<Image>();
        relatedText = null;
        relatedTransform = GetComponent<RectTransform>();
    }
    public override void SetInactive()
    {
        this.gameObject.SetActive(false);
    }
    public override void SetActive()
    {
        this.gameObject.SetActive(true);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
