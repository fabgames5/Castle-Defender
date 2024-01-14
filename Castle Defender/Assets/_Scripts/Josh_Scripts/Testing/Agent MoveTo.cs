using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class AgentMoveTo : MonoBehaviour
{

    NavMeshAgent agent;

    public Transform target;
    public float updateSpeed = .25f;
    public bool usingInvoke =false;

    public NavMeshSurface surface;
    // Start is called before the first frame update
    void Start()
    {
        surface.BuildNavMesh();

       
        if (updateSpeed > 0)
        {
            usingInvoke = true;
            InvokeRepeating("SetAgentDestination", updateSpeed *8, updateSpeed);
        }
    }
    private void OnDisable()
    {
        CancelInvoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateSpeed <= 0)
        {
            if (usingInvoke)
            {
                CancelInvoke();
            }
            agent.SetDestination(target.position);
        }
    }

    public void SetAgentDestination()
    {
        if (agent == null)
        {
            agent = this.gameObject.GetComponent<NavMeshAgent>();
            if (agent.enabled == false)
            {
                agent.enabled = true;
            }
        }
        agent.SetDestination(target.position);
    }
}
