using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Zeldinha.Story
{
    /// <summary>
    /// Sistema simples de diálogos estilo Zelda.
    /// </summary>
    public class DialogueSystem : MonoBehaviour
    {
        public static DialogueSystem Instance { get; private set; }

        [Header("UI")]
        public GameObject dialoguePanel;
        public TextMeshProUGUI speakerText;
        public TextMeshProUGUI dialogueText;
        public float typeSpeed = 0.03f;

        Queue<DialogueLine> lines = new Queue<DialogueLine>();
        bool isTyping;
        bool lineFinished;
        Coroutine typeRoutine;

        public bool IsDialogueActive { get; private set; }

        [System.Serializable]
        public class DialogueLine
        {
            public string speaker;
            [TextArea] public string text;
        }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }

        void Update()
        {
            if (!IsDialogueActive) return;

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                if (!lineFinished)
                {
                    // Completa a linha imediatamente
                    if (typeRoutine != null) StopCoroutine(typeRoutine);
                    dialogueText.text = currentFullText;
                    lineFinished = true;
                }
                else
                {
                    ShowNextLine();
                }
            }
        }

        string currentFullText;

        public void StartDialogue(List<DialogueLine> dialogue)
        {
            lines.Clear();
            foreach (var line in dialogue) lines.Enqueue(line);

            IsDialogueActive = true;
            Systems.GameManager.Instance?.SetState(Systems.GameManager.GameState.Dialogue);
            if (dialoguePanel != null) dialoguePanel.SetActive(true);

            ShowNextLine();
        }

        void ShowNextLine()
        {
            if (lines.Count == 0)
            {
                EndDialogue();
                return;
            }

            var line = lines.Dequeue();
            if (speakerText != null) speakerText.text = line.speaker;
            currentFullText = line.text;
            lineFinished = false;

            if (typeRoutine != null) StopCoroutine(typeRoutine);
            typeRoutine = StartCoroutine(TypeText(line.text));
        }

        IEnumerator TypeText(string text)
        {
            isTyping = true;
            dialogueText.text = "";
            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSecondsRealtime(typeSpeed);
            }
            isTyping = false;
            lineFinished = true;
        }

        void EndDialogue()
        {
            IsDialogueActive = false;
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            Systems.GameManager.Instance?.SetState(Systems.GameManager.GameState.Playing);
        }
    }
}
