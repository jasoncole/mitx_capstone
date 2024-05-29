using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
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
        if (!agent_control.can_move)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Collider2D collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition), layersToHit);
            if (collider != null)
            {
                agent_control.InteractWithTarget(collider.GameObject());
            }
            else
            {
                agent_control.MoveToPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Collider2D collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition), layersToHit);
            if (collider != null)
            {
                agent_control.MoveToAttackTarget(collider.GameObject());
            }
        }
    }
}
