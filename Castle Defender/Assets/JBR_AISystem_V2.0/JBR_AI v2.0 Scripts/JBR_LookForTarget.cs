using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JBR_LookForTarget : JBR_Base_Behavior_State
{
    [Space]
    [Space]
    [Tooltip("The Targets of the AI, This could be a player, another AI, or sound object")]
    public List<TargetList> targets = new List<TargetList>();
    private List<TargetList> removeList = new List<TargetList>();

    [Tooltip("THe Max Distance the AI can see an object")]
    public float maxViewDistance = 100.0f;
    public GameObject lookOrigin;
    public float fieldOfView = 120.0f;
    // public bool inFieldOfView;
    [Tooltip("Set the layers to detect raycast hits ")]
    public LayerMask mask;

    public override void Initialize(JBR_AI_ControllerSystem mainSystem, NavMeshAgent ai_Agent, Animator ai_Animator, AudioSource ai_AudioSource)
    {
        CheckStartUp();
        base.Initialize(mainSystem, ai_Agent, ai_Animator, ai_AudioSource);
    }

    private void CheckStartUp()
    {
        if (lookOrigin == null)
        {
            Debug.LogError("(JBR_LookForTarget) is missing the lookorigin reference... typically the head of the avatar....");
        }


        if (mask == LayerMask.GetMask("Nothing"))
        {
            Debug.LogWarning("(JBR_LookForTarget) has no mask layers setup, please add all layers that will effect vision, like walls, Players, NPCs ");
            mask = LayerMask.GetMask("Default");
        }
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
        base.UpdateState();
    }

    /// <summary>
    /// Slow update for each ability. It's updated in the controller
    /// </summary>
    public override void SlowUpdateState()
    {
        Debug.Log("Looking for a Target ******");
        CheckTargetList();

        base.SlowUpdateState();     
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

    void CheckTargetList()
    {
        if (targets.Count > 0)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].soundObject != null && targets[i].soundObject.activeSelf != true)
                {
                    removeList.Add(targets[i]);
                    continue;
                }
                //reset distance 
                if (targets[i].soundObject != null)
                {
                    targets[i].distance = Vector3.Distance(this.transform.position, targets[i].soundObject.transform.position);
                }
                else
                {
                    if (targets[i].target != null)
                    {
                        targets[i].distance = Vector3.Distance(this.transform.position, targets[i].target.transform.position);
                    }
                }
                //threatLevel decrease 1 point per second
                targets[i].threatLevel -= m_AI_Controller.slowUpdate;

                if (targets[i].threatLevel <= 0)
                {
                    removeList.Add(targets[i]);
                }

                if (targets[i].distance <= maxViewDistance)
                {
                    //does a field of view and raycast check
                    targets[i].canSee = SightCheck(targets[i], i);
                }
                else
                {
                    targets[i].canSee = false;
                }
            }

            //remove all inactive objects
            for (int i = 0; i < removeList.Count; i++)
            {
                targets.Remove(removeList[i]);
            }
            removeList.Clear();
            FindBestTarget();
        }
        else
        {
            if (m_AI_Controller.currentTarget != null)
            {
                m_AI_Controller.currentTarget = null;
                m_AI_Controller.OnTargetChanged.Invoke();
            }
            Collider[] localObjects = Physics.OverlapSphere(this.transform.position, maxViewDistance, mask);
            for (int i = 0; i < localObjects.Length; i++)
            {
                for (int t = 0; t <m_AI_Controller.targetTags.Length; t++)
                {
                    if (localObjects[i].CompareTag(m_AI_Controller.targetTags[t]))
                    //  }
                    //  if(localObjects[i].CompareTag("Player") || localObjects[i].CompareTag("NPC"))
                    {
                        //    for (int t = 0; t < targets.Count; t++)
                        //    {
                        //        if (targets[t].target == localObjects[i].gameObject)
                        //        {
                        //            return;
                        //        }
                        //    }

                        TargetList tl = new TargetList();
                        tl.distance = Vector3.Distance(this.gameObject.transform.position, localObjects[i].transform.position);
                        tl.target = localObjects[i].gameObject;
                        tl.soundObject = null;
                        tl.threatLevel = 100;
                        tl.canSee = false;
                        tl.inFieldOfView = false;
                        targets.Add(tl);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Function Used to find the best target if there are more than one.
    /// </summary>
    public void FindBestTarget()
    {
        if (targets.Count > 0)
        {
            TargetList bestTarget = null;
            float score = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                float newScore = 0;

                //add threat level bias
                newScore += targets[i].threatLevel;

                //Large bias if can see target
                if (targets[i].canSee)
                {
                    newScore += 100;
                }
                //distance bias
                newScore += (100.0f / targets[i].distance);

                if (newScore > score)
                {
                    bestTarget = targets[i];
                    score = newScore;
                    //   Debug.Log("Best Target > " + bestTarget + " Target SCore > " + score);
                }
            }

            //if can see target then set actual target
            if (bestTarget.canSee)
            { 
                if(m_AI_Controller.currentTarget != bestTarget.target)
                {
                    m_AI_Controller.currentTarget = bestTarget.target;
                    m_AI_Controller.OnTargetChanged.Invoke();
                }
                
                m_AI_Controller.distanceToTarget = bestTarget.distance;
                m_AI_Controller.canSeeTarget = true;
                m_AI_Controller.canHearTarget = false;
            }
            else
            {
                //otherwise only set the soundobject as target
                m_AI_Controller.currentTarget = bestTarget.soundObject;
                m_AI_Controller.distanceToTarget = bestTarget.distance;
                m_AI_Controller.canSeeTarget = false;
                m_AI_Controller.canHearTarget = true;
            }
        }
        else
        {
            m_AI_Controller.canSeeTarget = false;
            m_AI_Controller.canHearTarget = false;
            m_AI_Controller.currentTarget = null;
        }
    }


    /// <summary>
    /// Adds a new target with a float threat level of target, ie Sound level
    /// Will also do a Distance calculation for final threat level
    /// </summary>
    /// <param name="newTarget"></param> the Gameobject that made the sound
    /// <param name="soundObject"></param> sound object spawned when Sound was made
    /// <param name="threatLevel"></param> threat level determined by the sound type
    public void AddTarget(GameObject newTarget, GameObject soundObject, float threatLevel)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].soundObject == soundObject)
            {
                return;
            }
        }

        TargetList tl = new TargetList();
        tl.distance = Vector3.Distance(this.gameObject.transform.position, newTarget.transform.position);
        tl.target = newTarget;
        tl.soundObject = soundObject;
        tl.threatLevel = threatLevel;
        tl.canSee = false;
        tl.inFieldOfView = false;
        targets.Add(tl);
    }

    /// <summary>
    /// /// Sight Check For AI returns bool
    /// </summary>
    /// <param name="t"></param> target List
    /// <param name="refID"></param> reference ID number
    /// <returns></returns>
    bool SightCheck(TargetList t, int refID)
    {
        bool canSeeTarget = false;
        Vector3 direction = ((t.target.transform.position + new Vector3(0, 1.0f, 0)) - lookOrigin.transform.position);

        //we only want to do a FOV check once, we assume that if a target is spotted that AI will watch the target. 
        //this stops the target from quickly going behind the AI to trick it into no longer following, but does allow sneaking behind AI if not spotted yet
        if (!t.inFieldOfView)
        {
            float target_Angle = Vector3.Angle(direction, lookOrigin.transform.forward);
            //check and see if targets are with in field of view of ai
            if (target_Angle <= fieldOfView *.5f)
            {
                //if they are in field of view set to true
                t.inFieldOfView = true;
            }
            else
            {
                return false;
            }
        }

        if (t.inFieldOfView)
        {
            RaycastHit hit;
            // ... and if a raycast towards the target hits something...
            if (Physics.Raycast(lookOrigin.transform.position, direction.normalized, out hit, maxViewDistance, mask, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.gameObject.CompareTag("Player") || hit.collider.gameObject.CompareTag("NPC"))
                {
                    canSeeTarget = true;
                    targets[refID].threatLevel += 2;
                }
            }
        }
        return canSeeTarget;
    }

}
[System.Serializable]
//Target List
public class TargetList
{
    public GameObject target;
    public GameObject soundObject;
    public float threatLevel;
    public float distance;
    public bool canSee;
    public bool inFieldOfView;

}

