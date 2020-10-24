using System;
using UnityEngine;

public class ShowDialogueOnEnter : MonoBehaviour
{
    public string enterTag;
    public Dialogue dialogue;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enterTag))
        {
            DialogueManager.Singleton.BeginDialogue(dialogue);
        }
    }
}