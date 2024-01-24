using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]

public class JBR_Locamotion : JBR_Base_Behavior_State
{  
    [Tooltip("maximum distance to target, to still be considered at the target location")]
    public float ai_StopDistance = 2.0f;
    [Tooltip("Speed Dampining, how quickly a character can Accelerate")]
    public float speedDampTime = 1.0f;   
    [Tooltip("rotation Dampining, how quickly a character can rotate")]
    public float directionDampTime = .5f;
    [Space]
    
    [Tooltip("Dynamically Set, Speed input to Animation Controller, To adjust Movement speed adjust root animation speed")]
    public float speed;
    [Tooltip("Dynamically Set, Turning input to Animation Controller")]
    public float angle;
    [Space]
    private float targetAngle;
    private float AI_ControllerAngle;
    private int ranNumb;
   // [Tooltip("How long CHaracter will look around before moving on to next task")]
   // public float lookingAroundTimer = 3.0f;
    private float timer;
    private bool canMove = true;
    private Vector3 MoveToLocation;
    private float agentDistance;
    // Start is called before the first frame update
    void Start()
    {
        m_AI_Agent.stoppingDistance = ai_StopDistance; 
    }

    void OnAnimatorMove()
    {  if (canMove)
        {
            m_AI_Agent.velocity = m_AI_Animator.deltaPosition / Time.deltaTime;
                  
                m_AI_Animator.transform.rotation = m_AI_Animator.rootRotation;
                m_AI_Animator.transform.position = m_AI_Animator.rootPosition;
        }
    }

     void Set_AI_Animator(float Speed, float Direction)
    {
            //sets animator speed and direction to move
            m_AI_Animator.SetFloat("Speed", Speed, speedDampTime, Time.deltaTime);
            m_AI_Animator.SetFloat("Direction", Direction, directionDampTime, Time.deltaTime);       
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
    /// Main Update for each ability. It's updated in the controller
    /// </summary>
    public override void UpdateState()
    {
        if (canMove)
        {
            if (m_AI_Agent.destination == Vector3.zero)
            {
            return;
            }
            if(m_AI_Controller.currentTarget != null && speed < .01f)
            {
            Vector3 direction = (m_AI_Controller.currentTarget.transform.position - m_AI_Animator.rootPosition);
            targetAngle = Vector3.Angle(direction,m_AI_Animator.rootPosition);
            Quaternion rootRot = m_AI_Animator.rootRotation;
            AI_ControllerAngle = rootRot.eulerAngles.y;
            //   m_AI_Controller.transform.LookAt(m_AI_Controller.CurrentTarget.transform);
            }
       
            Set_AI_Animator(speed, angle);
        }

        base.UpdateState();
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
            }
            //no target so find a random point, waypoint, or a curious location to move too
            else
            {

            }
            //sets agent destination
            m_AI_Agent.destination = MoveToLocation;
            agentDistance = Vector3.Distance(MoveToLocation, m_AI_Animator.rootPosition);
          
            if (agentDistance > ai_StopDistance)
            {
                speed = m_AI_Agent.desiredVelocity.magnitude;
                Vector3 ai_AgentsNextPathTarget = m_AI_Agent.steeringTarget;

                Vector3 curentDir = m_AI_Animator.rootRotation * Vector3.forward;
                Vector3 wantedDir = (ai_AgentsNextPathTarget - m_AI_Animator.rootPosition).normalized;

                if (Vector3.Dot(curentDir, wantedDir) > 0)
                {
                    angle = Vector3.Cross(curentDir, wantedDir).y;
                }
                else
                {
                    angle = Vector3.Cross(curentDir, wantedDir).y > 0 ? 1 : -1;
                }
            }
            else
            {
                speed = 0;
                angle = 0;
            }
        }
            base.SlowUpdateState();
    }

    public override void OnExitAbility()
    {
        canMove = false;
        angle = 0;
        speed = 0;
        Invoke("SetAnimator", .01f);
        Debug.Log("*****Exit Locamotion Ability*****");

        base.OnExitAbility();
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
        canMove = true;
        base.OnEnterAbility();
    }

    public void SetAnimator()
    {
        m_AI_Animator.SetFloat("Speed", speed);
        m_AI_Animator.SetFloat("Direction", angle);
        Debug.Log("*****Exit Locamotion Ability Set Animator*****");
    }
}
