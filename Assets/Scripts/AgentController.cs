using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// BUG: if you interact with something right as you reach the end of a separate path, the game will allow you to interact immediately

public class AgentController : MonoBehaviour
{
    Vector3 target_point;
    Interactable target;
    NavMeshAgent agent;
    [SerializeField] float attack_range;
    [SerializeField] float interact_range;
    int move_state;
    
    public delegate void OnAttackStart();
    public event OnAttackStart AttackStarted;

    // check if reached end of path
    public float pathEndThreshold = 0.1f;
    private bool hasPath = false;

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
                AttackStarted?.Invoke();
            }
            break;

            case 2:
            if (AtEndOfPath())
            {
                target.Interact(this.GameObject());
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

    public void AttackTarget(Interactable enemy)
    {
        target = enemy;
        move_state = 1;
        agent.stoppingDistance = attack_range;
        agent.SetDestination(new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z));
    }

    public void Interact(Interactable interactable_)
    {
        target = interactable_;
        move_state = 2;
        agent.stoppingDistance = interact_range;
        agent.SetDestination(new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z));
    }

    /*
    void AttackMove()
    */
}
