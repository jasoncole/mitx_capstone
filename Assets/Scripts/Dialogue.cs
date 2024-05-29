using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.Assertions;

public class Dialogue : MonoBehaviour
{
    // singleton
    private static Dialogue _instance;
    public static Dialogue Instance 
    {
        get 
        {
            return _instance;
        } 
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    public TextMeshProUGUI textComponent;
    public GameObject dialogueBox;
    string line;
    public float textSpeed; // chars per second
    public AgentController player;
    public int state;
    // 0 = idle
    // 1 = typing
    // 2 = finished
    public event Action DialogueEnded;

    // Start is called before the first frame update
    void Start()
    {
        CloseDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == 0)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            // if dialogue still playing, type line fast
            // if dialogue is not playing,
            // end dialogue and invoke event to prompt for next dialogue
            switch(state)
            {
                case 1:
                TypeLineFast();
                break;

                case 2:
                DialogueEnded?.Invoke();
                break;
            }
        }
    }

    public void StartDialogue(EventInfo event_info, string line_)
    {
        Conversation conversation_ = event_info.conversation;
        if (conversation_ == null)
        {
            Debug.Assert(event_info.event_type == EventType.OnInteract);
            conversation_ = gameObject.AddComponent<Conversation>();
            conversation_.Init();
            conversation_.speaker = event_info.receiver;
            conversation_.participants.Add(event_info.sender);
            conversation_.participants.Add(event_info.receiver);
        }
        conversation_.last_line = line_;
        DialogueEnded += conversation_.DialogueEndCallback;

        line = line_;
        dialogueBox.SetActive(true);
        player.AllowMovement(false);
        StartCoroutine(TypeLineSlow());
    }

    IEnumerator TypeLineSlow()
    {
        state = 1;
        textComponent.text = string.Empty;
        foreach (char c in line.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(1/textSpeed);
        }
        state = 2;
    }

    void TypeLineFast()
    {
        StopAllCoroutines();
        textComponent.text = string.Empty;
        foreach (char c in line.ToCharArray())
        {
            textComponent.text += c;
        }
        state = 2;
    }

    public void CloseDialogue()
    {
        state = 0;
        player.AllowMovement(true);
        dialogueBox.SetActive(false);
    }
}
