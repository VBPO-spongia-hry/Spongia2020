using System;
using System.Collections;
using TMPro;
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
        public Animator dialogueAnimator;
        public TextMeshProUGUI nameText1;
        public TextMeshProUGUI nameText2;
        public GameObject vitalsUI;
        public GameObject controlsUI;
        
        private DialogueCharacter _character1;
        private DialogueCharacter _character2;
        
        private static readonly int Skip = Animator.StringToHash("Skip");
        private static readonly int Next = Animator.StringToHash("Next");
        private static readonly int Isfirst = Animator.StringToHash("Isfirst");
        private static readonly int Viewing = Animator.StringToHash("Viewing");
        private static readonly int DialogueStart = Animator.StringToHash("DialogueStart");
        private bool _showingDialogue;
        
        private void OnEnable()
        {
            if(Singleton != null) Destroy(Singleton.gameObject);
            Singleton = this;
        }

        private void Start()
        {
       //     BeginDialogue(dialogues.dialogues[0]);
        }

        private void Update()
        {
            if (_showingDialogue)
            {
                if(Input.anyKeyDown) dialogueAnimator.SetTrigger(Skip);
            }
        }

        public void BeginDialogue(Dialogue dialogue)
        {
            dialogueAnimator.ResetTrigger(Skip);
            InputHandler.DisableInput = true;
            _character1 = null;
            _character2 = null;
            vitalsUI.SetActive(false);
            controlsUI.SetActive(false);
            foreach (var msg in dialogue.messages)
            {
                if (_character1 == null) _character1 = msg.character;
                else if (msg.character != _character1 && _character2 == null) _character2 = msg.character;
                else if (msg.character != _character2 && msg.character != _character1)
                {
                    Debug.LogError("More than 2 characters in dialogue. Ignoring ... ");
                    return;
                }
            }
            nameText2.gameObject.SetActive(_character2 != null);
            if (_character2 != null)
            {
                nameText2.SetText($"{_character2.characterName}, {_character2.job}");
                character2.sprite = _character2.baseImage;
            }

            if (_character1 != null)
            {
                character1.sprite = _character1.baseImage;
                nameText1.SetText($"{_character1.characterName}, {_character1.job}");
            }
            
            StartCoroutine(Begin(dialogue));
        }

        private IEnumerator Begin(Dialogue dialogue)
        {
            _showingDialogue = true;
            dialogueAnimator.SetTrigger(DialogueStart);
            dialogueAnimator.SetBool(Viewing,true);
            character1.transform.GetChild(1).GetComponent<Image>().sprite = _character1.emotions[0];
            if(_character2 != null) character2.transform.GetChild(1).GetComponent<Image>().sprite = _character2.emotions[0];
            dialogue.Reset();
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < dialogue.messages.Length; i++)
            {
                yield return ShowMessage(dialogue.ShowNext());
            }
            dialogueAnimator.SetBool(Viewing, false);
            InputHandler.DisableInput = false;
            _showingDialogue = false;
            vitalsUI.SetActive(true);
            controlsUI.SetActive(true);
        }

        private IEnumerator ShowMessage(Message msg)
        {
            Text updating = msg.character == _character1 ? bubble1 : bubble2;
            dialogueAnimator.SetBool(Isfirst, _character1 == msg.character);
            dialogueAnimator.SetTrigger(Next);
            dialogueAnimator.ResetTrigger(Skip);
            updating.text = "";
            if (_character1 == msg.character)
            {
                character1.transform.GetChild(1).GetComponent<Image>().sprite = _character1.emotions[msg.emotionIndex];
            }
            else
            {
                character2.transform.GetChild(1).GetComponent<Image>().sprite = _character2.emotions[msg.emotionIndex];
            }
            yield return new WaitUntil(() => dialogueAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Shown"));
            string displayed = "";
            foreach (var t in msg.msg)
            {
                displayed += t;
                updating.text = displayed;
                yield return new WaitForSeconds(0.02f);
                if (!Input.anyKeyDown) continue;
                updating.text = msg.msg;
                break;
            }
            yield return new WaitUntil(() => dialogueAnimator.GetCurrentAnimatorStateInfo(0).IsName("UIShown"));
        }
    }
}
