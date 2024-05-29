using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;

// info table for
// each NPC
// local world state
// global world state

public interface IGatherable // this component has information that must be gathered to make a query
{
    string gather_name
    {
        get;
    }
    void FillQuery(string parent_name, Dictionary<string, object> query);
}

public enum EventType
{
    OnInteract,
    OnDialogueEnd,
    OnTakeDamage,
    OnDeath
}

public class EventInfo : IGatherable
{
    public EventType event_type;
    public GameObject sender;
    public GameObject receiver;
    public Conversation conversation;
    public float damage;

    public string gather_name => null;

    public void FillQuery(string parent_name, Dictionary<string, object> query)
    {
        switch(event_type)
        {
            case EventType.OnInteract:
            {
                query.Add("sender", sender);
                query.Add("self", receiver);
                foreach(IGatherable component in sender.GetComponents<IGatherable>())
                {
                    component.FillQuery("sender", query);
                }
                foreach(IGatherable component in receiver.GetComponents<IGatherable>())
                {
                    component.FillQuery("self", query);
                }
                break;
            }
            case EventType.OnDialogueEnd:
            {
                conversation.FillQuery(gather_name, query);
                break;
            }
            case EventType.OnTakeDamage:
            {
                query.Add("damage", damage);
                query.Add("attacker", sender);
                query.Add("self", receiver);
                foreach(IGatherable component in sender.GetComponents<IGatherable>())
                {
                    component.FillQuery("attacker", query);
                }
                foreach(IGatherable component in receiver.GetComponents<IGatherable>())
                {
                    component.FillQuery("self", query);
                }
                break;
            }
            case EventType.OnDeath:
            {
                query.Add("damage", damage);
                query.Add("killer", sender);
                query.Add("victim", receiver);
                foreach(IGatherable component in sender.GetComponents<IGatherable>())
                {
                    component.FillQuery("killer", query);
                }
                foreach(IGatherable component in receiver.GetComponents<IGatherable>())
                {
                    component.FillQuery("victim", query);
                }
                break;
            }
        }
    }
}

public class Condition
{
    public string query_name;
    public delegate bool CriterionFunc(GameObject game_object, object query_entry);
    public CriterionFunc criterion_func;
}

public class Behavior
{
    // response
    public int response_id;

    public delegate void ResponseCallback(EventInfo event_info, int response_id);
    public event ResponseCallback response_callback;

    // rules
    public List<Condition> conditions;
    public int score_modifier = 0;

    public Behavior()
    {
        conditions = new List<Condition>();
    }

    public void AddCondition(string query_name, Condition.CriterionFunc criterion_func)
    {
        Condition condition = new Condition();
        condition.query_name = query_name;
        condition.criterion_func = criterion_func;

        conditions.Add(condition);
    }
    public int CalculateScore()
    {
        return conditions.Count + score_modifier;
    }

    public void AddCallback(ResponseCallback callback)
    {
        response_callback += callback;
    }

    public void InitiateResponse(EventInfo event_info, int response_id)
    {
        response_callback?.Invoke(event_info, response_id);
    }
}

// behavior table
// contains list of conditions and corresponding responses
// behavior tables are linked to corresponding components
// one unit may have multiple components with their own behavior table
// for example there is a table that describes orc behavior and a table that describes human behavior. One NPC can have both.

// TODO: how do I ensure that only one table exists across multiple NPCs even though each one has the component
// TODO: allow me to manually adjust importance of specific conditions
public class NPC_BehaviorManager : MonoBehaviour
{
    // singleton
    private static NPC_BehaviorManager _instance;
    public static NPC_BehaviorManager Instance 
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

        behavior_tables = new Dictionary<string, List<Behavior>>();
    }

    Dictionary<string, List<Behavior>> behavior_tables;
    Dictionary<int, Action> response_lookup;

    // events
    // TODO: is there any point in the code where other parts need to know what events are happening?
    // currently not using C# events, just calling the global behavior manager directly
    // maybe behavior manager should trigger the event so others can see?

    public event Action<EventInfo> Interacted;
    public event Action<EventInfo> DialogueEnded;
    public event Action<EventInfo> DamageTaken;

    int CompareConditionCount(Behavior first, Behavior second)
    {
        if (first.conditions.Count > second.conditions.Count)
        {
            return -1;
        }
        else if (first.conditions.Count < second.conditions.Count)
        {
            return 1;
        }
        return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Behavior QueryEventResponse(GameObject subject, string BehaviorTableKey, List<Dictionary<string, object>> Queries) // returns an id to the response with the best match
    {
        // exit if requested behavior table is not found
        if (!behavior_tables.ContainsKey(BehaviorTableKey))
        {
            return null;
        }

        List<Behavior> valid_responses = new List<Behavior>(); // randomly choose from valid responses
        int highest_score = 0;

        // iterate through behaviors to find the best one
        foreach (Behavior behavior_ in behavior_tables[BehaviorTableKey]) // concat component + event + whatever else to get the specific behavior table
        {
            if (behavior_.CalculateScore() < highest_score)
            {
                break;
            }

            bool satisfied = true;
            // iterate through each condition in the behavior to check that all are satisfied
            foreach (Condition condition_ in behavior_.conditions)
            {
                satisfied = false;
                foreach (Dictionary<string, object> query in Queries)
                {
                    if (query.ContainsKey(condition_.query_name))
                    {
                        satisfied = condition_.criterion_func(subject, query[condition_.query_name]);
                        break;
                    }
                }
                if (!satisfied)
                {
                    break;
                }
            }
            if (satisfied)
            {
                highest_score = behavior_.CalculateScore();
                valid_responses.Add(behavior_);
            }
        }

        if (valid_responses.Count == 0)
        {
            return null;
        }
        return valid_responses[UnityEngine.Random.Range(0, valid_responses.Count)];
    }


    public void OnInteract(GameObject sender_, GameObject receiver_)
    {
        // create event
        EventInfo event_info = new EventInfo();
        event_info.event_type = EventType.OnInteract;
        event_info.sender = sender_;
        event_info.receiver = receiver_;

        // gather event queries
        var query = new Dictionary<string, object>();
        event_info.FillQuery(null, query);
        WorldStateManager.Instance.GatherGlobalQuery(query);

        List<Dictionary<string, object>> query_list = new List<Dictionary<string, object>>();
        query_list.Add(query);

        List<Behavior> best_responses = QueryAllBehaviorsForObject(event_info.receiver, "OnInteract", query_list);

        if (best_responses.Count > 0)
        {
            Behavior best_response = best_responses[UnityEngine.Random.Range(0, best_responses.Count)];
            best_response.InitiateResponse(event_info, best_response.response_id);
        }

        // fire event
        Interacted?.Invoke(event_info);
        Debug.Log("Interact event fired");
    }

    public void OnDialogueEnd(Conversation conversation) // NOTE: we're assuming that there is only one responder for the entire conversation
    {
        // create event
        EventInfo event_info = new EventInfo();
        event_info.event_type = EventType.OnDialogueEnd;
        event_info.conversation = conversation;

        // gather event queries
        var query = new Dictionary<string, object>();

        event_info.FillQuery(null, query);
        WorldStateManager.Instance.GatherGlobalQuery(query);

        int highest_score = 0;
        Dictionary<Behavior, GameObject> best_responses = new Dictionary<Behavior, GameObject>();
        Dictionary<string, object> participant_query = new Dictionary<string, object>();
        List<Dictionary<string, object>> query_list = new List<Dictionary<string, object>>();
        query_list.Add(query);
        query_list.Add(participant_query);
        // ask each participant if they have something to say in response
        foreach (GameObject participant in conversation.participants)
        {
            // gather participant queries
            // each participant makes a decision based on their own state and the state in the event
            // they are not aware of the internal states of other participants
            foreach(IGatherable component in participant.GetComponents<IGatherable>())
            {
                component.FillQuery("self", participant_query);
            }

            List<Behavior> participant_best_responses = QueryAllBehaviorsForObject(participant, "OnDialogueEnd", query_list);

            // if highest score, save for later
            if (participant_best_responses.Count > 0)
            {
                int participant_score = participant_best_responses.First().CalculateScore();
                if (participant_score >= highest_score)
                {
                    if (participant_score > highest_score)
                    {
                        best_responses.Clear();
                    }
                    foreach (Behavior participant_response in participant_best_responses)
                    {
                        best_responses.Add(participant_response, participant);
                    }
                }
            }
            // reset query for next pass
            participant_query.Clear();
        }
        
        if (best_responses.Count > 0)
        {
            KeyValuePair<Behavior, GameObject> bestkv = best_responses.ElementAt(UnityEngine.Random.Range(0, best_responses.Count));
            Behavior best_response = bestkv.Key;
            event_info.receiver = bestkv.Value;
            best_response.InitiateResponse(event_info, best_response.response_id);
        }
        else
        {
            conversation.CloseConversation();
        }

        // fire event
        DialogueEnded?.Invoke(event_info);
        Debug.Log("End Dialogue event fired");
    }

    public void OnTakeDamage(DamageRecord damage_record)
    {
        // create event
        EventInfo event_info = new EventInfo();
        event_info.event_type = EventType.OnTakeDamage;
        event_info.sender = damage_record.attacker;
        event_info.receiver = damage_record.target;

        // gather event queries
        var query = new Dictionary<string, object>();
        event_info.FillQuery(null, query);
        WorldStateManager.Instance.GatherGlobalQuery(query);
        List<Dictionary<string, object>> query_list = new List<Dictionary<string, object>>();
        query_list.Add(query);

        List<Behavior> best_responses = QueryAllBehaviorsForObject(event_info.receiver, "OnTakeDamage", query_list);

        if (best_responses.Count > 0)
        {
            Behavior best_response = best_responses[UnityEngine.Random.Range(0, best_responses.Count)];
            best_response.InitiateResponse(event_info, best_response.response_id);
        }

        // fire event
        DamageTaken?.Invoke(event_info);
        Debug.Log("Damage Taken event fired");
    }

    public void OnDeath(DamageRecord damage_record)
    {
        // create event
        EventInfo event_info = new EventInfo();
        event_info.event_type = EventType.OnTakeDamage;
        event_info.sender = damage_record.attacker;
        event_info.receiver = damage_record.target;

        // gather event queries
        var query = new Dictionary<string, object>();
        event_info.FillQuery(null, query);
        WorldStateManager.Instance.GatherGlobalQuery(query);
        List<Dictionary<string, object>> query_list = new List<Dictionary<string, object>>();
        query_list.Add(query);

        List<Behavior> best_responses = QueryAllBehaviorsForObject(event_info.receiver, "OnTakeDamage", query_list);

        if (best_responses.Count > 0)
        {
            Behavior best_response = best_responses[UnityEngine.Random.Range(0, best_responses.Count)];
            best_response.InitiateResponse(event_info, best_response.response_id);
        }

        // fire event
        DamageTaken?.Invoke(event_info);
        Debug.Log("Death event fired");
    }

    public List<Behavior> QueryAllBehaviorsForObject(
        GameObject subject, string event_name, 
        List<Dictionary<string, object>> queries
        )
    {
        BehaviorSubscriptions behavior_subscriptions = subject.GetComponent<BehaviorSubscriptions>();
        Debug.Assert(behavior_subscriptions != null);
        string subscription_list = behavior_subscriptions.behavior_identifier.ToString();

        char[] delimiter_chars = {' ', ','};
        string[] table_identifiers = subscription_list.Split(delimiter_chars, StringSplitOptions.RemoveEmptyEntries);

        int highest_score = 0;
        List<Behavior> best_responses = new List<Behavior>();
        foreach (string table_identifier in table_identifiers)
        {
            string BehaviorTableKey = table_identifier + event_name;
            Behavior behavior_ = QueryEventResponse(subject, BehaviorTableKey, queries);

            if (behavior_!= null)
            {
                if (behavior_.CalculateScore() >= highest_score)
                {
                    if (behavior_.CalculateScore() > highest_score)
                    {
                        best_responses.Clear();
                    }
                    best_responses.Add(behavior_);
                }
            }
        }
        return best_responses;
    }

    public void Register(BehaviorIdentifier table_name, string[] event_names)
    {
        foreach (string event_name in event_names)
        {
            List<Behavior> behavior_table = new List<Behavior>();
            string name = table_name.ToString() + event_name;
            behavior_tables.Add(name, behavior_table);
        }
    }

    public Behavior AddBehavior(string BehaviorTableKey)
    {
        Behavior ret = new Behavior();
        behavior_tables[BehaviorTableKey].Add(ret);

        return ret;
    }

    public void ForceSortBehaviors(string BehaviorTableKey)
    {
        behavior_tables[BehaviorTableKey].Sort(CompareConditionCount);
    }
}
