using UnityEngine;

namespace Dialogues
{
    [CreateAssetMenu(fileName = "Dialogue Character", menuName = "Dialogue/DialogueCharacter", order = 1)]
    public class DialogueCharacter : ScriptableObject
    {
        public Sprite baseImage;
        public string characterName;
        public string job;
    }
}