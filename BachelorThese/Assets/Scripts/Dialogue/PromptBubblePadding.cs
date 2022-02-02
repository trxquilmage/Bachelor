using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptBubblePadding : MonoBehaviour
{
    RectTransform rectTransform;
    RectTransform parentRectTransform;
    [SerializeField] Vector2 bounds;

    private void Awake()
    {
        GetVariables();
    }
    void GetVariables()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
    }
    public void UpdateBounds()
    {
        rectTransform.sizeDelta = parentRectTransform.sizeDelta + bounds;
    }
}
