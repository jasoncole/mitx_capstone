using Unity.Properties;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// BUG: if you interact with something right as you reach the end of a separate path, the game will allow you to interact immediately

public class DamageRecord
{
    public GameObject attacker;
    public GameObject target;
    public float Damage;

    public void ApplyDamage()
    {
        target.GetComponent<HasHealth>().ApplyDamage(this);
    }
}

public class AgentController : MonoBehaviour
{
    Vector3 target_point;
    GameObject target;
    NavMeshAgent agent;

    CharAttributes Attributes;
    int move_state;

    // check if reached end of path
    public float pathEndThreshold = 0.1f;
    private bool hasPath = false;
    public bool IsRanged;
    public bool can_move;
    Animator animator_;
    public string current_animation_state;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        move_state = 0;
        // 1 = attacking
        // 2 = interacting
    }

    // Update is called once per frame
    void Update()
    {
        switch(move_state)
        {
            case 0:
            break;

            case 1:
            if (AtEndOfPath())
            {
                StartAttack();
            }
            break;

            case 2:
            if (AtEndOfPath())
            {
                NPC_BehaviorManager.Instance.OnInteract(this.GameObject(), target);
            }
            break;

            default:
            break;
        }
        
    }
    bool AtEndOfPath()
    {
        hasPath |= agent.hasPath;
        if (hasPath && agent.remainingDistance <= agent.stoppingDistance + pathEndThreshold )
        {
            // Arrived
            hasPath = false;
            Stop();
            return true;
        }
 
        return false;
    }

    public void Stop()
    {
        target_point = transform.position;
        agent.SetDestination(new Vector3(target_point.x, target_point.y, transform.position.z));
    }

    public void MoveToPosition(Vector2 position)
    {
        target_point = position;
        move_state = 0;
        agent.stoppingDistance = 0;
        agent.SetDestination(new Vector3(target_point.x, target_point.y, transform.position.z));
    }

    public void MoveToAttackTarget(GameObject enemy)
    {
        target = enemy;
        move_state = 1;
        agent.stoppingDistance = Attributes.attack_range;
        agent.SetDestination(new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z));
    }

    public void InteractWithTarget(GameObject target_)
    {
        target = target_;
        move_state = 2;
        agent.stoppingDistance = Attributes.interact_range;
        agent.SetDestination(new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z));
    }

    void StartAttack()
    {
        DamageRecord damage_record = Attributes.MakeDamageRecord(gameObject, target);
        if (IsRanged)
        {
            Attributes.MakeProjectile(damage_record);
        }
        else
        {
            damage_record.ApplyDamage();
        }
    }

    public void Die()
    {
        // start death animation
        ChangeAnimationState("Death");
        can_move = false;
    }

    public void AllowMovement(bool allow)
    {
        can_move = allow;
    }

    public void ChangeAnimationState(string new_state)
    {
        if (new_state == current_animation_state)
        {
            return;
        }

        animator_.Play(new_state);
        current_animation_state = new_state;

    }

    public void UpdateMovespeed(float movespeed_)
    {
        agent.speed = movespeed_;
    }
}


