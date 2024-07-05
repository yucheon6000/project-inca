using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField]
    private GameObject dialogCanvas;
    [SerializeField]
    private GameObject nextButtonTMP;

    [Header("Index")]
    [SerializeField]
    private int targetMessageIndex = 0;

    [Header("Messages")]
    [SerializeField]
    private List<DialogMessage> dialogMessages = new List<DialogMessage>();

    [Header("Global Events")]
    [SerializeField]
    private UnityEvent onStartPrintMessageGlobal = new UnityEvent();
    [SerializeField]
    private UnityEvent onEndPrintMessageGlobal = new UnityEvent();
    [SerializeField]
    private UnityEvent onPressNextButtonGlobal = new UnityEvent();

    private bool isPrintingMessage = false;
    private bool isWaitingForPressNextButton = false;
    private bool skipPrintMessage = false;              // If the player wants to skip the message which is printing now.
    private bool pressNextButton = false;               // If the player wants to see the following message.
    private Coroutine printMessageCoroutine = null;

    private void Awake()
    {
        ViveInput.AddListenerEx(HandRole.RightHand, ControllerButton.FullTrigger, ButtonEventType.Down, () =>
        {
            if (isPrintingMessage)
                SkipPrintMessage();

            else if (isWaitingForPressNextButton)
                PressNextButton();
        });
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (isPrintingMessage)
            SkipPrintMessage();

        else if (isWaitingForPressNextButton)
            PressNextButton();
    }

    public void SetActiveDialogCanvas(bool value)
    {
        dialogCanvas.gameObject.SetActive(value);
    }

    public void SetActiveNextButtonTMP(bool value)
    {
        nextButtonTMP.gameObject.SetActive(value);
    }

    public void SkipPrintMessage()
    {
        // if (printMessageCoroutine == null) return;

        skipPrintMessage = true;
    }

    public void PressNextButton()
    {
        // if (printMessageCoroutine == null) return;

        pressNextButton = true;
    }

    public int GetTargetMessageIndex() => targetMessageIndex;
    public void SetTargetMessageIndex(int index) => targetMessageIndex = index;

    public void PrintNextMessage()
    {
        targetMessageIndex++;
        PrintMessageByTargetIndex();
    }

    [ContextMenu("PrintMessageByTargetIndex")]
    public void PrintMessageByTargetIndex()
    {
        PrintMessage(targetMessageIndex);
    }

    public void PrintMessage(int targetMessageIndex)
    {
        this.targetMessageIndex = targetMessageIndex;
        printMessageCoroutine = StartCoroutine(PrintMessageRoutine(targetMessageIndex));
    }

    private IEnumerator PrintMessageRoutine(int targetMessageIndex)
    {
        DialogMessage message = dialogMessages[targetMessageIndex];

        if (message == null)
        {
            isPrintingMessage = false;
            isWaitingForPressNextButton = false;
            skipPrintMessage = false;
            pressNextButton = false;
            printMessageCoroutine = null;
            yield break;
        }

        isPrintingMessage = true;

        onStartPrintMessageGlobal.Invoke();
        message.onStartPrintMessage.Invoke();

        int length = message.text.Length;
        bool isTag = false;

        dialogTMP.text = "";

        for (int i = 0; i < length; ++i)
        {
            char c = message.text[i];

            // Add character to TMP
            dialogTMP.text += c;

            // When player press a button to skip printing message
            if (!message.blockSkipPrintMessage && skipPrintMessage)
            {
                dialogTMP.text = message.text;
                break;
            }

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

        onEndPrintMessageGlobal.Invoke();
        message.onEndPrintMessage.Invoke();

        skipPrintMessage = false;
        isPrintingMessage = false;

        isWaitingForPressNextButton = true;

        if (message.nextTime > 0)
        {
            yield return new WaitForSeconds(message.nextTime);
        }
        else
        {
            while (true)
            {
                if (pressNextButton)
                    break;

                print(gameObject.name + ": No pressNextButton");
                yield return null;
            }
        }

        printMessageCoroutine = null;

        pressNextButton = false;
        isWaitingForPressNextButton = false;

        onPressNextButtonGlobal.Invoke();
        message.onPressNextButton.Invoke();
    }

    [Serializable]
    private class DialogMessage
    {
        [Header("Message")]
        [TextArea]
        public string text;

        [Header("Properties")]
        public bool blockSkipPrintMessage = true;   // The players can't skip printing a message.
        public float nextTime = 0;                  // If it is greater than 0, it will be skipped based on time.

        [Header("Events")]
        public UnityEvent onStartPrintMessage = new UnityEvent();
        public UnityEvent onEndPrintMessage = new UnityEvent();
        public UnityEvent onPressNextButton = new UnityEvent();
    }
}
