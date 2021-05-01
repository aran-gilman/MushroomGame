using UnityEngine;

[CreateAssetMenu]
public class Mushroom : ScriptableObject
{
    public Sprite sprite;
    public string displayName;
    public int cost;
    public int value;
    public bool plantable = true;
    public int growTimeInSeconds = 300;
}
