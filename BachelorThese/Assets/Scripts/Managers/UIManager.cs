using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Handles UI Functions

    public Image eButtonSprite;
    public static UIManager instance;
    private void Start()
    {
        instance = this;
    }
    /// <summary>
    /// Portray The E-Button Feedack On The Canvas
    /// </summary>
    /// <param name="target"></param>
    public void PortrayEButton(GameObject target)
    {
        if (target == null) //target == null : hide e button
        {
            eButtonSprite.enabled = false; 
            return;
        }
        //place e button correctly
        Vector2 targetInScreenSpace = Camera.main.WorldToScreenPoint(target.transform.position);
        eButtonSprite.transform.position = targetInScreenSpace + Vector2.right*40f;
       
        eButtonSprite.enabled = true; //show e button
    }
}
