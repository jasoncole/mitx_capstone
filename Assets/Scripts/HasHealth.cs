using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageRecord
{
    public int attacker;
    public int target;
    public float Damage;
    public string DamageType;
}

public class HasHealth : MonoBehaviour, IGatherable
{
    public float RemainingHealth;
    public float MaxHealth;

    public delegate void OnTakeDamage(DamageRecord damage);
    public event OnTakeDamage DamageTaken;

    [SerializeField] HealthBar healthBar_;

    void Awake()
    {
        healthBar_ = GetComponentInChildren<HealthBar>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ApplyDamage(DamageRecord damage)
    {
        healthBar_.UpdateHealthBar(RemainingHealth, MaxHealth);
        DamageTaken?.Invoke(damage);
    }

    public void FillQuery(ref Dictionary<string, object> query)
    {
        query.Add("RemainingHealth", RemainingHealth);
        query.Add("MaxHealth", MaxHealth);
    }
}
