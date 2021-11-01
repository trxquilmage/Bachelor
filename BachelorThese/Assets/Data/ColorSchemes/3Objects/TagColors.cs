using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TagColors", order = 1)]
public class TagColors : ScriptableObject
{
    public Color locationColor;
    public Color generalColor;
    public Color nameColor;
    public Color itemColor;
    public Color allColor;
}
