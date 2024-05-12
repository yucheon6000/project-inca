using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Dialog : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float charDelay = 0.05f;
    [SerializeField]
    private float spaceDelay = 0.1f;
    [SerializeField]
    private float lineBreakDelay = 0.15f;

    [Header("Components")]
    [SerializeField]
    private TextMeshProUGUI dialogTMP;

    [Header("Debug")]
    [SerializeField]
    private int targetMessageIndex = 0;

    [Header("Messages")]
    [SerializeField]
    private List<DialogMessage> dialogMessages = new List<DialogMessage>();

    [ContextMenu("Debug_PrintMessageByTargetIndex")]
    private void PrintMessageByTargetIndex()
    {
        PrintMessage(targetMessageIndex);
    }

    public void PrintMessage(int targetMessageIndex) => StartCoroutine(PrintMessageRoutine(targetMessageIndex));

    private IEnumerator PrintMessageRoutine(int targetMessageIndex)
    {
        DialogMessage message = dialogMessages[targetMessageIndex];
        if (message == null) yield break;

        message.OnStartPrintMessage.Invoke();

        int length = message.text.Length;
        bool isTag = false;

        dialogTMP.text = "";

        for (int i = 0; i < length; ++i)
        {
            char c = message.text[i];

            // Add character to TMP
            dialogTMP.text += c;

            // Check if this is a tag
            if (c == '<')
            {
                isTag = true;
                continue;
            }
            else if (isTag && c == '>')
            {
                isTag = false;
                continue;
            }
            else if (isTag)
                continue;

            // Waiting time depending on character type
            if (c == ' ')
                yield return new WaitForSeconds(spaceDelay);
            else if (c == '\n')
                yield return new WaitForSeconds(lineBreakDelay);
            else
                yield return new WaitForSeconds(charDelay);
        }

        message.OnEndPrintMessage.Invoke();
    }

    [Serializable]
    private class DialogMessage
    {
        [Header("Message")]
        [TextArea]
        public string text;

        [Header("Events")]
        public UnityEvent OnStartPrintMessage = new UnityEvent();
        public UnityEvent OnEndPrintMessage = new UnityEvent();
    }
}
