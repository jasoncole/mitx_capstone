using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public delegate void OnInteract(AgentController agent, Interactable interactable);
    public event OnInteract Interacted;

    Collider2D collider;

    void Awake()
    {
        collider = GetComponent<Collider2D>();
        Interacted += NPC_BehaviorManager.Instance.OnInteract;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(AgentController agent)
    {
        Interacted?.Invoke(agent, this);
        Debug.Log("Interact event fired");
    }
}
