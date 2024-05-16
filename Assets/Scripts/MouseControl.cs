using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public Camera camera;
    public LayerMask layersToHit;
    AgentController agent_control;

    void Awake()
    {
        agent_control = GetComponent<AgentController>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Collider2D collider = Physics2D.OverlapPoint(camera.ScreenToWorldPoint(Input.mousePosition), layersToHit);
            if (collider != null)
            {
                agent_control.Interact(collider.GetComponent<Interactable>());
            }
            else
            {
                agent_control.MoveToPosition(camera.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }
}
