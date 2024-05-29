using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public LayerMask layersToHit;
    AgentController agent_control;

    public SpriteRenderer sprite_renderer;

    public Sprite HeroicGarb;

    public Sprite CultRobe;

    public Sprite InvisibilityCloak;

    public Sprite default_clothes;

    public Inventory my_inventory;

    void Awake()
    {
        agent_control = GetComponent<AgentController>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        my_inventory = GetComponent<Inventory>();
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
        Vector3 position = gameObject.transform.position;
        position.z = -10;
        Camera.main.transform.position = position;
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
        else if (Input.GetMouseButtonDown(0))
        {
            Collider2D collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition), layersToHit);
            if (collider != null)
            {
                agent_control.MoveToAttackTarget(collider.GameObject());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q)) // heroic garb
        {   
            if (my_inventory.HeroGarb == 1)
            {
                sprite_renderer.sprite = HeroicGarb;
                my_inventory.UnEquipAll();
                my_inventory.HeroGarb = 2;
            }
            else
            {
                my_inventory.UnEquipAll();
                sprite_renderer.sprite = default_clothes;
            }
        }
        else if (Input.GetKeyDown(KeyCode.W)) // cult robe
        {
            if (my_inventory.CultRobe == 1)
            {
                sprite_renderer.sprite = CultRobe;
                my_inventory.UnEquipAll();
                my_inventory.CultRobe = 2;
            }
            else
            {
                my_inventory.UnEquipAll();
                sprite_renderer.sprite = default_clothes;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E)) // invisibility
        {
            if (my_inventory.InvisibilityRobe == 1)
            {
                sprite_renderer.sprite = InvisibilityCloak;
                my_inventory.UnEquipAll();
                my_inventory.InvisibilityRobe = 2;
            }
            else
            {
                my_inventory.UnEquipAll();
                sprite_renderer.sprite = default_clothes;
            }
        }
    }
}
