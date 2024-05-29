using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingState : MonoBehaviour, IGatherable
{
    public bool IsFirstMeeting = true;
    public string gather_name => "King";
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
        if (parent_name != null)
        {
            parent_name = parent_name + ".";
        }
        query.Add(parent_name + gather_name + ".IsFirstMeeting", IsFirstMeeting);
    }
}
