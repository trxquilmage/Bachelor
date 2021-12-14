using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Colors", order = 1)]
public class Colors : ScriptableObject
{
    // text colors
    [Header("Text Colors")]
    public Color normalColor;
    public Color interactableColor;
    public Color interactedColor;
    public Color inListColor;

    //Tag colors
    [Header("Tag Colors")]
    public Color[] generalTagColors;

    // Other colors
    [Header("Other Colors")]
    public Color shadowButtonColor; //Color that mixes into Button Shadows
    public Color askColor; //Color for ask & barter button (not set on start rn)
    public Color greyedOutColor; //greyed out color for ask & barter button
    public Color textFieldColor;
    public Color inputFieldColor;
    public Color nameFieldColor;
    public Color interactableButtonsColor;
    public Color limitColor;
    public Color headerColor;
    public Color trashColor;
    public Color highlightColor;
}
