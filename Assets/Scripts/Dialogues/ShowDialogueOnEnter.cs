using System;
using UnityEngine;

public class ShowDialogueOnEnter : MonoBehaviour
{
    public string enterTag;
    public Dialogue dialogue;
    private bool _triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if(_triggered) return;
        if (other.CompareTag(enterTag))
        {
            DialogueManager.Singleton.BeginDialogue(dialogue);
            _triggered = true;
        }
    }
}