using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FactionTownState : MonoBehaviour, IGatherable
{
    public float reputation; // -1.0f to 1.0f
    public int player_awareness;
    // 0 = idle
    // 1 = searching for player
    // 2 = attacking player
    public string gather_name => "Town";

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

    public void FillQuery(string parent_name, Dictionary<string, object> query) // parent_name should be null so just ignore
    {
        query.Add(gather_name + "." + nameof(reputation), reputation);
        query.Add(gather_name + "." + nameof(player_awareness), player_awareness);
    }
}
