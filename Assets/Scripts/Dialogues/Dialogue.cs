using System;
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

        private void OnValidate()
        {
            foreach (var message in messages)
            {
                if (message.emotionIndex < 0)
                {
                    message.emotionIndex = 0;
                    continue;
                }
                if (message.emotionIndex >= message.character.emotions.Length)
                {
                    message.emotionIndex = message.character.emotions.Length - 1;
                }
            }
        }
    }

    [System.Serializable]
    public class Message
    {
        [TextArea(5,10)]
        public string msg;
        public int emotionIndex;
        public DialogueCharacter character;
    }
}