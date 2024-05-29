using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class AttackProjectile : MonoBehaviour
{
    public float speed;
    public DamageRecord damage_record;

    void Awake()
    {
    }

    void Update()
    {
        // point towards target
        Vector2 target_position = damage_record.target.transform.position;
        transform.LookAt(target_position);

        // move towards target
        Vector2 old_position = transform.position;
        transform.position += speed*Time.deltaTime*transform.forward;

        // if overshot target, destroy and apply damage
        float dot_product = Vector2.Dot(target_position - old_position, target_position - (Vector2)transform.position);
        if (dot_product < 0.0f)
        {
            damage_record.ApplyDamage();
            Destroy(gameObject);
        }
    }
}
