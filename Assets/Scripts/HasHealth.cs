using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HasHealth : MonoBehaviour, IGatherable
{
    public float RemainingHealth;
    public float MaxHealth;

    public delegate void OnTakeDamage(DamageRecord damage);
    public event OnTakeDamage DamageTaken;
    [SerializeField] HealthBar health_bar;
    public string gather_name => "Health";

    void Awake()
    {
        health_bar = GetComponentInChildren<HealthBar>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ApplyDamage(DamageRecord damage_record)
    {
        RemainingHealth -= damage_record.Damage;
        health_bar.UpdateHealthBar(RemainingHealth, MaxHealth);
        NPC_BehaviorManager.Instance.OnTakeDamage(damage_record);
        if (RemainingHealth < 0)
        {
            NPC_BehaviorManager.Instance.OnDeath(damage_record);
        }
        GetComponent<AgentController>().Die();
    }

    public void FillQuery(string parent_name, Dictionary<string, object> query)
    {
        if (parent_name != null)
        {
            parent_name = parent_name + ".";
        }
        query.Add(parent_name + gather_name + "." + nameof(RemainingHealth), RemainingHealth);
        query.Add(parent_name + gather_name + "." + nameof(MaxHealth), MaxHealth);
    }
}
