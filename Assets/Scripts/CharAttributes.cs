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
    public GameObject spear_prefab;
    

    public void UpdateDamage(float damage)
    {
        attack_damage = damage;
    }

    public void MakeProjectile(DamageRecord damage_record)
    {
        Instantiate(spear_prefab, gameObject.transform.position, Quaternion.identity);
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