using UnityEngine;

namespace Dialogues
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        public Message[] messages;
        private int _messageIndex = 0;

        public Message ShowNext()
        {
            var next = messages[_messageIndex];
            _messageIndex++;
            return next;
        }

        public void Reset()
        {
            _messageIndex = 0;
        }
    
    }

    [System.Serializable]
    public class Message
    {
        [TextArea(5,10)]
        public string msg;
        public int characterIndex;
        public DialogueCharacter character;
    }
}