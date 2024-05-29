using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conversation : MonoBehaviour, IGatherable
{
    public GameObject speaker;
    public List<GameObject> participants;
    public string last_line; // last line spoken in conversation
    public string gather_name => "Conversation";
    void Awake()
    {
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        participants = new List<GameObject>();
    }

    public void DialogueEndCallback()
    {
        Dialogue.Instance.DialogueEnded -= DialogueEndCallback;
        NPC_BehaviorManager.Instance.OnDialogueEnd(this);
    }

    public void CloseConversation()
    {
        Dialogue dialogue = GetComponent<Dialogue>();
        dialogue.CloseDialogue();
        Destroy(this);
    }

    public void FillQuery(string parent_name, Dictionary<string, object> query)
    {
        if (parent_name != null)
        {
            parent_name = parent_name + ".";
        }
        query.Add(parent_name + gather_name + ".speaker", speaker);
        query.Add(parent_name + gather_name + ".last_line", last_line);
    }
}
