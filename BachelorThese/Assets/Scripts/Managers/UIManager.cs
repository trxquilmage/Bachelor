using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image eButtonSprite;
    public static UIManager instance;
    private void Start()
    {
        instance = this;
    }
    public void PortrayEButton(GameObject target)
    {
        if (target == null)
        {
            eButtonSprite.enabled = false; //hide e button
            return;
        }
        //place e button correctly
        Vector2 targetInScreenSpace = Camera.main.WorldToScreenPoint(target.transform.position);
        eButtonSprite.transform.position = targetInScreenSpace + Vector2.right*40f;
       
        eButtonSprite.enabled = true; //show e button
    }
}
