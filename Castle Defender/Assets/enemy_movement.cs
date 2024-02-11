using UnityEngine;
using UnityEngine.AI;

public class enemy_movement : MonoBehaviour
{
    // Public variables
    public float moveSpeed;
    public GameObject tower;

    // Private variables
    private NavMeshAgent agent;
    public bool allowMoving = true;

    void Start()
    {
        // Get NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        tower = GameObject.FindGameObjectWithTag("TownHall");

       

       //only move if allowed
        if (allowMoving)
        {
            // Set agent speed
            agent.speed = moveSpeed;

            // Set target to the tower
            agent.SetDestination(tower.transform.position);
        }
    }

    void Update()
    {
        // Check if agent has reached the destination (tower)
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // Perform tower attack logic here (e.g., deal damage, destroy)

            // Optionally, destroy enemy upon reaching the tower
            //Destroy(gameObject);
        }
    }
}
