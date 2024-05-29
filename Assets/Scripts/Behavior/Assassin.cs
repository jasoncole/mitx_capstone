using System.Collections.Generic;
using UnityEngine;

public class Assassin : MonoBehaviour
{
    BehaviorIdentifier table_name = BehaviorIdentifier.ID_ASSASSIN;
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
        behavior_entry.response_id = RegisterDialogue("I heard you're off to kill the demon lord.");
        behavior_entry.AddCondition("sender.Inventory.HeroGarb", (self, value) => (int)value == 2);


        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("I used to be quite the assassin myself.");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "I heard you're off to kill the demon lord.");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Here, take this.");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "I used to be quite the assassin myself.");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(GiveCloak);
        behavior_entry.response_id = 0;
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "Here, take this.");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnInteract");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Sorry kid, that's all I got!");
        behavior_entry.AddCondition("sender.Inventory.InvisibilityRobe", (self, value) => (int)value != 0);
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

    public void GiveCloak(EventInfo event_info, int response_id)
    {
        WorldStateManager.Instance.player_object.GetComponent<Inventory>().InvisibilityRobe = 1;
        Dialogue.Instance.CloseDialogue();
    }
}
