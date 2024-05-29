using System.Collections.Generic;
using UnityEngine;

public class DemonLord : MonoBehaviour
{
    BehaviorIdentifier table_name = BehaviorIdentifier.ID_DEMON;
    string[] event_names = {
        "OnInteract",
        "OnDialogueEnd",
    };

    public List<string> DialogueOptions;

    void Awake()
    {
        NPC_BehaviorManager.Instance.Register(table_name, event_names);
        Behavior behavior_entry;

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnInteract");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("So you're the hero.");
        behavior_entry.AddCondition("sender.Inventory.HeroGarb", (self, value) => (int)value == 2);


        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("I suppose my time has finally come.");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "So you're the hero.");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("I've lived a good life with no regrets.");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "I suppose my time has finally come.");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Go on then, kill me!");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "I've lived a good life with no regrets.");

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
