using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Flags] public enum BehaviorIdentifier
{
    ID_MAYOR = 1<<0,
    ID_CULTIST = 1<<1,
    ID_QUEEN = 1<<2,
    ID_PLAYER = 1<<3
}

public class BehaviorSubscriptions : MonoBehaviour, IGatherable
{
    public BehaviorIdentifier behavior_identifier;
    public string gather_name => "Behavior";
    
    public void FillQuery(string parent_name, Dictionary<string, object> query)
    {
        query.Add(gather_name + ".behavior_identifier", behavior_identifier);
        // NOTE: note using parent name since factions are global
        WorldStateManager.Instance.GatherFactionQuery(query, behavior_identifier);
    }
}
