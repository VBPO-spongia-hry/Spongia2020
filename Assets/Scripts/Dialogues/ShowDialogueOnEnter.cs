using UnityEngine;

namespace Dialogues
{
    public class ShowDialogueOnEnter : MonoBehaviour
    {
        public string enterTag;
        public Dialogue dialogue;
        private bool _triggered = false;
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Enter");
            if (_triggered) return;
            if (other.CompareTag(enterTag))
            {
                DialogueManager.Singleton.BeginDialogue(dialogue);
                _triggered = true;
            }
        }
    }
}