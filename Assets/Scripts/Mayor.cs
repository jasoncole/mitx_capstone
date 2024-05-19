using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mayor : MonoBehaviour, IRespondable
{
    

    void Awake()
    {
        // associate response funcs with conditions here

        Dictionary<string, Func<object, bool>> conditions;
        

        Behavior behavior_entry = new Behavior();
        behavior_entry.Init(test_callback, 0);

        behavior_entry.AddCondition("receiver", "sadness", value => value < 10);
        

        NPC_BehaviorManager.Instance.AddBehavior("Mayor" + "OnInteract", behavior_entry);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void test_callback(EventInfo event_info)
    {

    }
}
