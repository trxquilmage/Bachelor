using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/OtherColors", order = 1)]
public class OtherColors : ScriptableObject
{
    public Color shadowButtonColor; //Color that mixes into Button Shadows
    public Color askColor; //Color for ask & barter button (not set on start rn)
    public Color greyedOutColor; //greyed out color for ask & barter button
}
