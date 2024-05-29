using System.Collections.Generic;
using UnityEngine;

public class Cultist : MonoBehaviour
{
    BehaviorIdentifier table_name = BehaviorIdentifier.ID_CULTIST;
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
        behavior_entry.response_id = RegisterDialogue("Would you like to join our cult?");
        behavior_entry.AddCondition("Cult.PlayerHostile", (self, value) => (bool)value == false);

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnInteract");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("I have no business with you, hero.");
        behavior_entry.AddCondition("Cult.PlayerHostile", (self, value) => (bool)value == false);
        behavior_entry.AddCondition("sender.Inventory.HeroGarb", (self, value) => (int)value == 2);

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnInteract");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Greetings, friend. You look rather mysterious.");
        behavior_entry.AddCondition("sender.Inventory.CultRobe", (self, value) => (int)value == 2);
        behavior_entry.score_modifier += 2;

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Members get a cool robe!");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "I'm not sure..");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnTakeDamage");
        behavior_entry.AddCallback(AlertFaction);
        behavior_entry.response_id = 0;

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

    public void AlertFaction(EventInfo event_info, int response_id)
    {
        WorldStateManager.Instance.AlertFactionOfAttacker(event_info.sender, FactionIdentifier.ID_CULT);
    }
}
