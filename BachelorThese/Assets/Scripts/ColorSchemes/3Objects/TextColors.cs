using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TextColors", order = 1)]
public class TextColors : ScriptableObject
{
    public Color normalColor;
    public Color interactableColor;
    public Color interactedColor;
}
