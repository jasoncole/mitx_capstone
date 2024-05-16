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
    void FillQuery(ref Dictionary<string, object> query);
}

public class Response
{
    public int response_id;
    public Dictionary<string, // query entry
    Func<object, bool> // boolean condition function
    > Conditions;
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

        behavior_tables = new Dictionary<string, List<Response>>();
    }
    
    // structure:
    // behavior table
    // -- component_name + event_name + any other information
    // -- -- response ids
    // -- -- -- conditions

    Dictionary<string, List<Response>> behavior_tables;
    Dictionary<int, Action> response_lookup;

    int CompareConditionCount(Response first, Response second)
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

    Dictionary<string, object>
    GatherQuery(GameObject subject)
    {
        Dictionary<string, object> ret = new Dictionary<string, object>();

        foreach(var component in subject.GetComponents<IGatherable>())
        {
            component.FillQuery(ref ret);
        }
        return ret;
    }

    // TODO: use jobs for multithreading?
    int QueryEventResponse(string BehaviorTableKey, List<Dictionary<string, object>> Queries) // returns an id to the response with the best match
    {
        // exit if requested behavior table is not found
        if (!behavior_tables.ContainsKey(BehaviorTableKey))
        {
            return 0;
        }

        List<int> valid_responses = new List<int>(); // randomly choose from valid responses
        int best_match = 0;

        foreach (Response response_ in behavior_tables[BehaviorTableKey]) // concat component + event + whatever else to get the specific behavior table
        {
            bool satisfied = true;
            
            foreach (KeyValuePair<string, Func<object, bool>> condition_ in response_.Conditions)
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
                if (response_.Conditions.Count >= best_match)
                {   
                    best_match = response_.Conditions.Count;
                    valid_responses.Add(response_.response_id);
                }
                else
                {
                    break;
                }
            }
        }

        if (valid_responses.Count == 0)
        {
            return 0;
        }
        return valid_responses[UnityEngine.Random.Range(0, valid_responses.Count)];
    }

    public void OnInteract(AgentController agent, Interactable interactable)    
    {
        TableSubscriptions table_subscriptions = interactable.GetComponent<TableSubscriptions>();
        string subscription_list = table_subscriptions.table_identifier.ToString();

        char[] delimiter_chars = {' ', ','};
        string[] table_identifiers = subscription_list.Split(delimiter_chars, StringSplitOptions.RemoveEmptyEntries);

        List<Dictionary<string, object>> queries = new List<Dictionary<string, object>>();
        queries.Add(GatherQuery(agent.gameObject));
        queries.Add(GatherQuery(interactable.gameObject));

        foreach (string table_identifier in table_identifiers)
        {
            string BehaviorTableKey = table_identifier + "OnInteract";
            int response = QueryEventResponse(BehaviorTableKey, queries);
        }
    }

    public void AddResponse(string BehaviorTableKey, Response response_)
    {
        behavior_tables[BehaviorTableKey].Add(response_);
        behavior_tables[BehaviorTableKey].Sort(CompareConditionCount);
    }
}
