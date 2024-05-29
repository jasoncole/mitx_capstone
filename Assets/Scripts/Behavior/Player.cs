using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    BehaviorIdentifier table_name = BehaviorIdentifier.ID_PLAYER;
    string[] event_names = {
        "OnInteract",
        "OnDialogueEnd"
    };
    public List<string> DialogueOptions;

    void Awake()
    {
        NPC_BehaviorManager.Instance.Register(table_name, event_names);
        Behavior behavior_entry;

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("...");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "Would you like to join our cult?");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Sure, why not?");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "Members get a cool robe!");

        foreach (string event_name in event_names)
        {
            NPC_BehaviorManager.Instance.ForceSortBehaviors(table_name.ToString() + event_name);
        }
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
