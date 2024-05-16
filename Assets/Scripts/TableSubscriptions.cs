using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Flags] public enum TableIdentifier
{
    Mayor = 1<<0,
    Grunt = 1<<1
}

public class TableSubscriptions : MonoBehaviour
{
    public TableIdentifier table_identifier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
