using UnityEngine;

// Information about character who is speaking
[CreateAssetMenu(fileName = "New Character", menuName= "Character")]
public class Character : ScriptableObject
{
    public Sprite portrait;
    public string fullName;
}
