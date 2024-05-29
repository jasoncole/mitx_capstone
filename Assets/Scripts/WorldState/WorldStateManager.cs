using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags] public enum FactionIdentifier
{
    ID_TOWN = 1<<0,
    ID_CULT = 1<<1,
    ID_ROYAL = 1<<2,
}

public class WorldStateManager : MonoBehaviour
{
    // singleton
    private static WorldStateManager _instance;
    public static WorldStateManager Instance 
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
        BehaviorFactionLookup = new Dictionary<BehaviorIdentifier, FactionIdentifier>
        {
            {BehaviorIdentifier.ID_CULTIST, FactionIdentifier.ID_CULT},
            {BehaviorIdentifier.ID_MAYOR, FactionIdentifier.ID_TOWN},
            {BehaviorIdentifier.ID_QUEEN, FactionIdentifier.ID_ROYAL},
        };
    }

    public Dictionary<BehaviorIdentifier, FactionIdentifier> BehaviorFactionLookup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GatherFactionQuery(Dictionary<string, object> query, BehaviorIdentifier behavior_id)
    {
        FactionIdentifier faction_id = 0;
        foreach (KeyValuePair<BehaviorIdentifier, FactionIdentifier> entry in BehaviorFactionLookup)
        {
            if (behavior_id.HasFlag(entry.Key))
            {
                faction_id |= entry.Value;
            }
        }

        if (query.ContainsKey("town") == false & faction_id.HasFlag(FactionIdentifier.ID_TOWN))
        {
            GetComponent<FactionTownState>().FillQuery(null, query);
            query.Add("town", true);
        }
        if (query.ContainsKey("cult") == false & faction_id.HasFlag(FactionIdentifier.ID_CULT))
        {
            GetComponent<FactionTownState>().FillQuery(null, query);
            query.Add("cult", true);
        }
    }

    public string GetFactionName(FactionIdentifier faction_id)
    {
        if (faction_id == FactionIdentifier.ID_TOWN)
        {
            return "town";
        }
        if (faction_id == FactionIdentifier.ID_CULT)
        {
            return "cult";
        }
        return null;
    }

    public void GatherGlobalQuery(Dictionary<string, object> query)
    {
        GetComponent<GlobalState>().FillQuery(null, query);
    }

    public void AlertFactionOfAttacker(GameObject attacker, FactionIdentifier faction)
    {

    }

}
