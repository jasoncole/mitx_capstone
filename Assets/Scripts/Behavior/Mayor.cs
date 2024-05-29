using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Mayor : MonoBehaviour
{
    BehaviorIdentifier table_name = BehaviorIdentifier.ID_MAYOR;
    string[] event_names = {
        "OnInteract",
        "OnDialogueEnd"
    };

    public List<string> DialogueOptions;

    void Awake()
    {
        NPC_BehaviorManager.Instance.Register(table_name, event_names);
        Behavior behavior_entry;

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnInteract");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.AddCallback((EventInfo event_info, int response_id) => event_info.receiver.GetComponent<MayorState>().IsFirstMeeting = false);
        behavior_entry.response_id = RegisterDialogue("Hello, traveler!");
        behavior_entry.AddCondition("Town.reputation", (self, value) => (float)value >= 0.0f);
        behavior_entry.AddCondition("self.Mayor.IsFirstMeeting", (self, value) => (bool)value == true);

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Welcome to our humble town!");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "Hello, how are you?");
        behavior_entry.AddCondition("Conversation.speaker", (self, value) => (GameObject)value == self);

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnInteract");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Welcome!");

        // TODO: add to Wcounter when response is triggered

        foreach (string event_name in event_names)
        {
            NPC_BehaviorManager.Instance.ForceSortBehaviors(table_name.ToString() + event_name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int RegisterDialogue(string dialogue)
    {
        DialogueOptions.Add(dialogue);
        return DialogueOptions.Count - 1;
    }

    public void PassToDialogueManager(EventInfo event_info, int response_id)
    {
        Dialogue.Instance.StartDialogue(event_info, DialogueOptions[response_id]);
    } 
}
