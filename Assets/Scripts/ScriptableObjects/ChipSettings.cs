using UnityEngine;

[CreateAssetMenu(fileName = "ChipSettings", menuName = "Add ChipSettings")]
public class ChipSettings : ScriptableObject
{
    public Chip[] chips;
    public float spaceBetweenChips;
}

[System.Serializable]
public struct Chip
{
    public int value;
    public Material material;
    public Texture texture;
    public int initialAmount;
}
