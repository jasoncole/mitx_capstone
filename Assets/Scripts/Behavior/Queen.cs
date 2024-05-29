using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : MonoBehaviour
{
    BehaviorIdentifier table_name = BehaviorIdentifier.ID_QUEEN;
    string[] event_names = {
        "OnInteract",
        "OnDialogueEnd",
        "OnTakeDamage"
    };
    public List<string> DialogueOptions;

    void Awake()
    {
        NPC_BehaviorManager.Instance.Register(table_name, event_names);
        Behavior behavior_entry;

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnInteract");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("I have no business with you.");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnInteract");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("What are you doing here?!!");
        behavior_entry.AddCondition("sender.Inventory.CultRobe", (self, value) => (int)value == 2);

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Nobody can know I'm a part of the cult!");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "What are you doing here?!!");
        behavior_entry.AddCondition("Conversation.speaker", (self, value) => (GameObject)value == self);

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Pretend like you don't know me.");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "Nobody can know I'm a part of the cult!");
        behavior_entry.AddCondition("Conversation.speaker", (self, value) => (GameObject)value == self);

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
