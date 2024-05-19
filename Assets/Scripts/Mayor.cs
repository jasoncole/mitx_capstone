using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mayor : MonoBehaviour, IRespondable
{
    

    void Awake()
    {
        // associate response funcs with conditions here
    
        Behavior behavior_entry = new Behavior();
        behavior_entry.Init(test_callback);

        behavior_entry.AddCondition("receiver", "sadness", value => (int)value < 10);
        

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
