using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalState : MonoBehaviour, IGatherable
{
    public string gather_name => "Global";
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
    }
}
