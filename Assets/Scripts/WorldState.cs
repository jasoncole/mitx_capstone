using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    // singleton
    private static WorldState _instance;
    public static WorldState Instance 
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
    }
    Dictionary<string, object> world_state_table;
}
