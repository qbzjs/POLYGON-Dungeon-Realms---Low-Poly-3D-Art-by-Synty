using UnityEngine;
using UnityEngine.UI;

// Information about the line to display
[System.Serializable]
public struct Line 
{
    public Character speaker;

    // Max text size: 200 char
    [TextArea(2, 5)]
    public string text;
}

// Information about conversation : characters who speaking and the lines
[CreateAssetMenu(fileName = "new conversation", menuName = "Conversation")]
public class Conversation : ScriptableObject
{
    public Character speakerLeft;
    public Character speakerRight;
    public Line[] lines;
}
