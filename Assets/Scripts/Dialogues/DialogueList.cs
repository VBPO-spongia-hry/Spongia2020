using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue List", menuName = "Dialogue/DialogueList", order = 1)]
public class DialogueList : ScriptableObject
{
    public Dialogue[] dialogues;
    public Sprite[] characters;
    
}