using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiLinkOpener : MonoBehaviour//, IPointerClickHandler
{
    /*[SerializeField] private TextMeshProUGUI targetLabel;
    [SerializeField] private LinkOpener linkOpener;

    private int currentLinkIndex = -1;


    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(targetLabel, eventData.position, eventData.enterEventCamera);
        if (linkIndex != -1)
        {
            Debug.Log($"click on link: {eventData.position}, {Input.mousePosition}, {eventData.enterEventCamera}", eventData.enterEventCamera);
            
            TMP_LinkInfo linkInfo = targetLabel.textInfo.linkInfo[linkIndex];
            linkOpener.OpenLinkById(linkInfo.GetLinkID());
        }
    }
    
    void LateUpdate() {
        
        if (!IngameUi.Instance)
            return;
        
        // is the cursor in the correct region (above the text area) and furthermore, in the link region?
        var isHoveringOver = TMP_TextUtilities.IsIntersectingRectTransform(
            targetLabel.rectTransform, Input.mousePosition, null);
        int linkIndex = isHoveringOver ? 
            TMP_TextUtilities.FindIntersectingLink(targetLabel, Input.mousePosition, null)
            : -1;

        // Clear previous link selection if one existed.
        if( currentLinkIndex != -1 && linkIndex != currentLinkIndex ) {
            // Debug.Log("Clear old selection");
            SetLinkToColor(currentLinkIndex, Constants.UI.Colors.SelectedBlack);
            currentLinkIndex = -1;
        }

        // Handle new link selection.
        if( linkIndex != -1 && linkIndex != currentLinkIndex ) {
            // Debug.Log("New selection");
            currentLinkIndex = linkIndex;
            SetLinkToColor(linkIndex, Color.white);
        }

        // Debug.Log(string.Format("isHovering: {0}, link: {1}", isHoveringOver, linkIndex));
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(targetLabel, eventData.position, eventData.enterEventCamera);
        if (linkIndex != -1)
        {
            Debug.Log($"start hovering over {targetLabel.textInfo.linkInfo[linkIndex].GetLinkID()}");
            SetLinkToColor(linkIndex, Constants.UI.Colors.HoverWhite);
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
    }
    
    
    void SetLinkToColor(int linkIndex, Color32 color) 
    {
        
        TMP_LinkInfo linkInfo = targetLabel.textInfo.linkInfo[linkIndex];

        var oldVertColors = new List<Color32[]>(); // store the old character colors

        for( int i = 0; i < linkInfo.linkTextLength; i++ ) { // for each character in the link string
            int characterIndex = linkInfo.linkTextfirstCharacterIndex + i; // the character index into the entire text
            var charInfo = targetLabel.textInfo.characterInfo[characterIndex];
            int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.
            int vertexIndex = charInfo.vertexIndex; // Get the index of the first vertex of this character.

            Color32[] vertexColors = targetLabel.textInfo.meshInfo[meshIndex].colors32; // the colors for this character
            oldVertColors.Add(vertexColors.ToArray());

            if( charInfo.isVisible ) {
                vertexColors[vertexIndex + 0] = color;
                vertexColors[vertexIndex + 1] = color;
                vertexColors[vertexIndex + 2] = color;
                vertexColors[vertexIndex + 3] = color;
            }
        }

        // Update Geometry
        targetLabel.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
   
    */
}
