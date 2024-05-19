using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Assertions;

// info table for
// each NPC
// local world state
// global world state



public interface IGatherable // this component has information that must be gathered to make a query
{
    void FillQuery(Dictionary<string, object> query);
}

public interface IRespondable
{

}

public class EventInfo // every event uses this class. each event will only use some fields
{
    public gameObject sender;
    public gameObject receiver;
}

public class Condition
{
    public string query_name;
    public string query_entry;
    public Func<object, bool> criterion_func;
}

public class Query
{
    public string query_name;
    Dictionary<string, object> query_entries;

    public void AllocateAndGatherQuery(GameObject subject)
    {
        query_entries = new Dictionary<string, object>();
        foreach(var component in subject.GetComponents<IGatherable>())
        {
            component.FillQuery(query_entries);
        }
    }
}

public class Behavior
{
    // response
    //public IRespondable response_component; // the component attached to the BehaviorClass object that stores the response functions
    // unnecessary
    public delegate void ResponseCallback(EventInfo event_info);
    public ResponseCallback response_callback(EventInfo event_info);

    // rules
    public List<Condition> conditions;
    int score_modifier = 0;

    public void AddCondition(string query_name, string query_entry, Func<object, bool> criterion_func)
    {
        Condition condition = new Condition();
        condition.query_name = query_name;
        condition.query_entry = query_entry;
        condition.criterion_func = criterion_func;

        conditions.Add(condition);
    }
    public int CalculateScore()
    {
        return Conditions.Count() + score_modifier;
    }

    void Init(ResponseCallback response_callback, int score_modifier)
    {
        response_callback = response_callback;
        Conditions = new Dictionary<string, Func<object, bool>>();
        score_modifier = score_modifier;
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
    
    // structure:
    // behavior table
    // -- component_name + event_name + any other information
    // -- -- response ids
    // -- -- -- conditions

    Dictionary<string, List<Behavior>> behavior_tables;
    Dictionary<int, Action> response_lookup;

    int CompareConditionCount(Behavior first, Behavior second)
    {
        if (first.Conditions.Count > second.Conditions.Count)
        {
            return 1;
        }
        else if (first.Conditions.Count < second.Conditions.Count)
        {
            return -1;
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

    

    // TODO: use jobs for multithreading?
    Behavior QueryEventResponse(string BehaviorTableKey, List<Query> Queries) // returns an id to the response with the best match
    {
        // exit if requested behavior table is not found
        if (!behavior_tables.ContainsKey(BehaviorTableKey))
        {
            return null;
        }

        List<Behavior> valid_responses = new List<Behavior>(); // randomly choose from valid responses
        int highest_score = 0;

        foreach (Behavior behavior_ in behavior_tables[BehaviorTableKey]) // concat component + event + whatever else to get the specific behavior table
        {
            bool satisfied = true;
            
            foreach (Condition condition_ in behavior_.Conditions)
            {
                bool condition_found = false;

                foreach (Dictionary<string, object> query in Queries)
                {
                    if (query.ContainsKey(condition_.Key))
                    {
                        satisfied = condition_.Value(query[condition_.Key]);
                        condition_found = true;
                        break;
                    }
                }
                Assert.IsTrue(condition_found);
                if (!satisfied)
                {
                    break;
                }
            }
            if (satisfied)
            {
                if (behavior_.CalculateScore() >= highest_score)
                {   
                    highest_score = behavior_.CalculateScore();
                    valid_responses.Add(behavior_);
                }
                else
                {
                    break;
                }
            }
        }

        if (valid_responses.Count == 0)
        {
            return null;
        }
        return valid_responses[UnityEngine.Random.Range(0, valid_responses.Count)];
    }

    public void OnInteract(gameObject sender, gameObject receiver)
    {
        EventInfo event_info = new EventInfo();
        event_info.sender = sender;
        event_info.receiver = receiver;
        OnInteract(event_info);
    }

    public void OnInteract(EventInfo event_info)    
    {
        TableSubscriptions table_subscriptions = interactable.GetComponent<TableSubscriptions>();
        string subscription_list = table_subscriptions.table_identifier.ToString();

        char[] delimiter_chars = {' ', ','};
        string[] table_identifiers = subscription_list.Split(delimiter_chars, StringSplitOptions.RemoveEmptyEntries);

        List<Query> query_params;

        Query sender;
        sender.query_name = "sender";
        sender.AllocateAndGatherQuery(event_info.sender);
        query_params.Add(sender);

        Query receiver;
        receiver.query_name = "receiver";
        receiver.AllocateAndGatherQuery(event_info.receiver);
        query_params.Add(receiver);

        int highest_score = 0;
        Behavior? best_response = null;
        foreach (string table_identifier in table_identifiers)
        {
            // TODO: handle choosing one option from multiple tables
            string BehaviorTableKey = table_identifier + "OnInteract";
            Behavior behavior_ = QueryEventResponse(BehaviorTableKey, queries);

            if (behavior_!= null & behavior_.CalculateScore() >= highest_score)
            {   
                highest_score = behavior_.CalculateScore();
                best_response = behavior_;
            }
        }

        Debug.Assert(best_response != null);

        best_response.response_callback(event_info);
    }

    public void AddBehavior(string BehaviorTableKey, Behavior behavior_)
    {
        behavior_tables[BehaviorTableKey].Add(behavior_);
        behavior_tables[BehaviorTableKey].Sort(CompareConditionCount);
    }
}
