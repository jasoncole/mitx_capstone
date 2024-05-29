using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class King : MonoBehaviour
{
    BehaviorIdentifier table_name = BehaviorIdentifier.ID_KING;
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
        behavior_entry.AddCallback((EventInfo event_info, int response_id) => event_info.receiver.GetComponent<KingState>().IsFirstMeeting = false);
        behavior_entry.response_id = RegisterDialogue("Ah yes, the hero!");
        behavior_entry.AddCondition("self.King.IsFirstMeeting", (self, value) => (bool)value == true);
        behavior_entry.AddCondition("sender.Inventory.HeroGarb", (self, value) => (int)value == 2);

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("You have been resurrected after 1000 years to slay the Demon Lord.");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "Ah yes, the hero!");
        behavior_entry.AddCondition("Conversation.speaker", (self, value) => (GameObject)value == self);

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Now begone.");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "You have been resurrected after 1000 years to slay the Demon Lord.");
        behavior_entry.AddCondition("Conversation.speaker", (self, value) => (GameObject)value == self);

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnInteract");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("You've no doubt dealt with the Demon Lord by now, haven't you?");
        behavior_entry.AddCondition("sender.Inventory.HeroGarb", (self, value) => (int)value == 2);

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Thank you so much! The kingdom is forever in your debt..");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "You've no doubt dealt with the Demon Lord by now, haven't you?");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Figuratively speaking of course.");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "Thank you so much! The kingdom is forever in your debt..");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Since you seem to have a lot of free time on your hands, I don't suppose you could do something about the cultists outside?");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "Figuratively speaking of course.");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("My wife is terrified.");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "I don't suppose you could do something about the cultists outside?");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnInteract");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("I'm terribly sorry but I'm busy at the moment.");

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnInteract");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Begone, heathen!");
        behavior_entry.AddCondition("sender.Inventory.CultRobe", (self, value) => (int)value == 2);

        behavior_entry = NPC_BehaviorManager.Instance.AddBehavior(table_name.ToString() + "OnDialogueEnd");
        behavior_entry.AddCallback(PassToDialogueManager);
        behavior_entry.response_id = RegisterDialogue("Do not fear, darling! I will protect you!");
        behavior_entry.AddCondition("Conversation.last_line", (self, value) => (string)value == "Begone, heathen!");

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
