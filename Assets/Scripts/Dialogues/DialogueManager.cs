using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogues
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Singleton;
        public Image character1;
        public Text bubble1;
        public Image character2;
        public Text bubble2;
        public DialogueList dialogues;
        public Animator dialogueAnimator;
        private int _character1Index = -1;
        private int _character2Index = -1;
        private static readonly int Skip = Animator.StringToHash("Skip");
        private static readonly int Next = Animator.StringToHash("Next");
        private static readonly int Isfirst = Animator.StringToHash("Isfirst");
        private static readonly int Viewing = Animator.StringToHash("Viewing");
        private static readonly int DialogueStart = Animator.StringToHash("DialogueStart");

        private void OnEnable()
        {
            if(Singleton != null) Destroy(Singleton.gameObject);
            Singleton = this;
        }

        private void Start()
        {
       //     BeginDialogue(dialogues.dialogues[0]);
        }

        public void BeginDialogue(Dialogue dialogue)
        {
            InputHandler.DisableInput = true;
            _character1Index = -1;
            _character2Index = -1;
            foreach (var msg in dialogue.messages)
            {
                if (_character1Index == -1) _character1Index = msg.characterIndex;
                else if (msg.characterIndex != _character1Index && _character2Index == -1) _character2Index = msg.characterIndex;
                else if (msg.characterIndex != _character2Index && msg.characterIndex != _character1Index)
                {
                    Debug.LogError("More than 2 characters in dialogue. Ignoring ... ");
                    Debug.Log(_character1Index);
                    Debug.Log(_character2Index);
                    return;
                }
            }

            character1.sprite = dialogues.characters[_character1Index];
            character2.sprite = dialogues.characters[_character2Index];
            StartCoroutine(Begin(dialogue));
        }

        private IEnumerator Begin(Dialogue dialogue)
        {
            dialogueAnimator.SetTrigger(DialogueStart);
            dialogueAnimator.SetBool(Viewing,true);
            dialogue.Reset();
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < dialogue.messages.Length; i++)
            {
                yield return ShowMessage(dialogue.ShowNext());
            }
            dialogueAnimator.SetBool(Viewing, false);
            InputHandler.DisableInput = false;
            Debug.Log("End");
        }

        private IEnumerator ShowMessage(Message msg)
        {
            Text updating = msg.characterIndex == _character1Index ? bubble1 : bubble2;
            dialogueAnimator.SetBool(Isfirst, _character1Index == msg.characterIndex);
            dialogueAnimator.SetTrigger(Next);
            string displayed = "";
            foreach (var t in msg.msg)
            {
                displayed += t;
                updating.text = displayed;
                yield return new WaitForSeconds(0.05f);
                if (!Input.anyKeyDown) continue;
                updating.text = msg.msg;
                dialogueAnimator.SetTrigger(Skip);
                yield break;
            }
            yield return new WaitUntil(() => Input.anyKeyDown);
            dialogueAnimator.SetTrigger(Skip);
        }
    }
}
