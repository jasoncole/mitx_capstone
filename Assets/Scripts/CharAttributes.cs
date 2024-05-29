using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.AI;

public class CharAttributes : MonoBehaviour
{
    public float attack_range;
    public float interact_range;
    public float attack_damage;
    

    public void UpdateDamage(float damage)
    {
        attack_damage = damage;
    }

    public AttackProjectile MakeProjectile(DamageRecord damage_record)
    {
        GameObject projectile_object = new GameObject();
        AttackProjectile projectile = projectile_object.AddComponent<AttackProjectile>();
        return projectile;
    }

    public DamageRecord MakeDamageRecord(GameObject attacker, GameObject target)
    {
        DamageRecord damage_record = new DamageRecord
        {
            attacker = attacker,
            target = target,
            Damage = attack_damage
        };
        return damage_record;
    }
}