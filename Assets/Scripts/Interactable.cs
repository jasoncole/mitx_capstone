using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public delegate void OnInteract(EventInfo eventInfo);
    public event OnInteract Interacted;

    Collider collider_;

    void Awake()
    {
        collider_ = GetComponent<Collider>();
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

    public void Interact(GameObject initiator)
    {
        EventInfo event_ = new EventInfo();
        event_.sender = initiator;
        event_.receiver = this.GameObject();
        
        Interacted?.Invoke(event_);
        Debug.Log("Interact event fired");
    }
}
