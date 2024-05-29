using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FactionCultState : MonoBehaviour, IGatherable
{
    public bool PlayerHostile = false;
    public int awareness;
    // 0 = idle
    // 1 = alert
    // 2 = hostile
    public string gather_name => "Cult";

    void Awake()
    {
        // test

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FillQuery(string parent_name, Dictionary<string, object> query)
    {
        query.Add(gather_name + "." + nameof(PlayerHostile), PlayerHostile);
        query.Add(gather_name + "." + nameof(awareness), awareness);
    }
}
