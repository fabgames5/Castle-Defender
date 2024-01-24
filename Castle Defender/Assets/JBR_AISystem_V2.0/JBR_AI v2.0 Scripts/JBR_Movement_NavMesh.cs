using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JBR_Movement_NavMesh : JBR_Base_Behavior_State
{
    [Tooltip("maximum distance to target, to still be considered at the target location")]
    public float ai_StopDistance = 1.0f;
    [Tooltip("Speed of navMesh agent")]
    public float speed = 5;

    public Transform ai_BodyRoot;
    public float flyheight = 4.0f;
    public CapsuleCollider capCol;
    private Vector3 capColPos;


    private float timer;
    private bool canMove = true;
    private Vector3 MoveToLocation;
    private float agentDistance;

    public override void Initialize(JBR_AI_ControllerSystem mainSystem, NavMeshAgent ai_Agent, Animator ai_Animator, AudioSource ai_AudioSource)
    {
        capCol = this.GetComponent<CapsuleCollider>();
        capColPos = capCol.center;
        base.Initialize(mainSystem, ai_Agent, ai_Animator, ai_AudioSource);
    }


    /// <summary>
    /// Fixed update for each ability. It's updated in the controller
    /// </summary>
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        //  Debug.Log("Fixed Update Ability...");
    }

    /// <summary>
    /// Slow update for each ability. It's updated in the controller
    /// </summary>
    public override void SlowUpdateState()
    { 
        if (canMove)
        {
            //if there is a target move towards it
            if (m_AI_Controller.currentTarget != null)
            {
                MoveToLocation = m_AI_Controller.currentTarget.transform.position;
                //sets agent destination
                m_AI_Agent.destination = MoveToLocation;
                agentDistance = Vector3.Distance(MoveToLocation, m_AI_Animator.transform.root.position);

            }
            speed = m_AI_Agent.velocity.magnitude;
            m_AI_Animator.SetFloat("Speed", speed);
        }
        base.SlowUpdateState();
    }

    /// <summary>
    /// Main Update for each ability. It's updated in the controller
    /// </summary>
    public override void UpdateState()
    {
        if (ai_BodyRoot.position.y < flyheight) 
        {
            timer += Time.deltaTime;
            ai_BodyRoot.position = Vector3.Lerp(Vector3.zero, new Vector3(this.transform.position.x, flyheight, this.transform.position.z), timer);
            capCol.center = (ai_BodyRoot.localPosition + capColPos);
        }

        base.UpdateState();
    }

    public override bool TryEnterBehaviour()
    {
        bool canStart = false;
        // add logic to check if this ability can be enter
        // return true if so;
        canStart = true;
        base.TryEnterBehaviour();
        return canStart;
    }

    public override void OnEnterAbility()
    {

        m_AI_Animator.applyRootMotion = false;
        m_AI_Agent.stoppingDistance = ai_StopDistance;
        m_AI_Animator.SetBool("Flying", true);
        canMove = true;

        base.OnEnterAbility();
    }

    public override void OnExitAbility()
    {
        m_AI_Animator.applyRootMotion = false;
        m_AI_Animator.SetBool("Flying", false);
        base.OnExitAbility();
    }

}
