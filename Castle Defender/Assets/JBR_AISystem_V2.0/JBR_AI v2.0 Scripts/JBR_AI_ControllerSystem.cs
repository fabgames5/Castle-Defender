using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine;
//using System;

[RequireComponent(typeof(Rigidbody))]
public class JBR_AI_ControllerSystem : MonoBehaviour
{
   // public Behavior curBehavior;
   // [SerializeField]
    private JBR_Base_Behavior_State m_ActiveBehavior = null; // Active Behavior
    public string activeBehavior = "";
    [Tooltip("A Slow update for things that only need to check once per second or two ")]
    public float slowUpdate = 1.0f;
    [Space]
    [Tooltip("Select any layers the AI can stand on like : Ground, Roof Tops ")]
    public LayerMask groundLayerMasks;
    [Space]
    [Header("A List of all Behaviours the AI can perform, " +
        "drop Scripts on this Gameobject that inherit from JBR_Base_Behavior_State" +
        " to add different behaviours")]
    [SerializeField]
    [Tooltip("Dynamically Set in RunTime, A List of all Behaviours that have been added. Note: the order of the behaviour on this character determine priority")]
    protected List<string> behaviorNames = new List<string>();
    [SerializeField]
    [Tooltip("the order of the behaviour on this character determine priority")]
    protected List<JBR_Base_Behavior_State> behavior = new List<JBR_Base_Behavior_State>();

    //   [Header("Events")]
    //   [SerializeField] private UnityEvent OnReceiveDamage, OnDie;
    
    // Action Events
    //  public Action OnCharacterDie, OnCharacterDamage, OnHealthChanged;
    private NavMeshAgent ai_Agent;
    private Animator ai_Animator;
    private AudioSource ai_AudioSource;
    [Space]
    
    //bool activators
    [Space]
    [Tooltip("Dynamically Set , true if trigger1 was activated")]
    public bool isTrigger1;
    [Tooltip("Dynamically Set , true if trigger2 was activated")]
    public bool isTrigger2;
    [Space]
    [Tooltip("Dynamically Set , true if grounded")]
    public bool isGrounded;
    [Tooltip("Dynamically Set, true if idle")]
    public bool isIdle;
    [Tooltip("Dynamically Set , true if falling")]
    public bool isFalling;
    [Tooltip("Dynamically Set, true if Dead")]
    public bool isDead;
    [Tooltip("Dynamically Set, true if wandering between waypoints")]
    public bool isWandering;
    [Tooltip("Dynamically Set, true if attacking")]
    public bool isAttacking;
    [Tooltip("Dynamically Set, true if defending from attack")]
    public bool isDefending;
    [Tooltip("Dynamically Set, true if can see Target")]
    public bool canSeeTarget;
    [Tooltip("Dynamically Set, true if can Hear Target")]
    public bool canHearTarget;
    [Space]
    [Tooltip("Dynamically Set, the timer set for each behaviour as it is started, time before a new behaviour can start, () ie finish animation")]
    public float behaviourTimer;
    [Space]
    [Tooltip("Dynamically Set, the current health")]
    public float currentHealth;
    [Tooltip("The  maximum Health the AI can have")]
    public float maxHealth;
    [Space]
    [Tooltip("Select any layers the AI can stand on ")]
    public LayerMask mask;
    [Space]
    [Tooltip("at all tags to detect targets")]
    public string[] targetTags = { "Player", "NPC" };
    [Tooltip("Check this box if AI can target Enemy Tags")]
    public bool canHaveTarget;
    [Tooltip("Dynamically Set , the current Target")]
    public GameObject currentTarget;
    [Tooltip("Dynamically Set, distance to target")]
    public float distanceToTarget;
    [Tooltip("Dynamically Set, the last damage taken ")]
    public float damageTaken;
    [Tooltip("Dynamically Set, the last damage given")]
    public float damageGiven;
    [Tooltip("THe time damage is taken or given")]
    public float time;

    [SerializeField]
    //  private UnityEvent _OnHealthChanged = new UnityEvent();
    //  public UnityEvent OnHealthChanged { get { return _OnHealthChanged; } }
    public UnityEvent OnHealthChanged;
    [HideInInspector] public UnityEvent OnControllerDied;
    [HideInInspector] public UnityEvent OnDamageChanged;
    [HideInInspector] public UnityEvent OnTargetChanged;
    [HideInInspector] public UnityEvent OnDistanceChanged;
    [HideInInspector] public UnityEvent OnSeeTargetChanged;
    [HideInInspector] public UnityEvent OnHearTargetChanged;


    //float activators
    //  public float distanceCloserThan;
    //  public float distanceFurtherThan;
    //  public float takeDamageBelowPercent;
    //  public float takeDamageBelowAmount;
    //  public float takeDamageAbovePercent;
    //  public float takeDamageAboveAmount;
    //  public float healthBelowPercent;
    //  public float healthBelowAmount;
    //  public float healthAbovePercent;
    //  public float healthAboveAmount;
    private float fallTimer;

    void Awake()
    {
        if (this.GetComponent<NavMeshAgent>())
        {
            ai_Agent = this.GetComponent<NavMeshAgent>();
        }

        if (this.GetComponent<Animator>())
        {
            ai_Animator = this.GetComponent<Animator>();
        }
        else 
        { 
            ai_Animator = this.GetComponentInChildren<Animator>(); 
        }

        if (this.GetComponent<AudioSource>())
        {
            ai_AudioSource = this.GetComponent<AudioSource>();
        }
        else
        {
            ai_AudioSource = this.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        }

        if (groundLayerMasks == LayerMask.GetMask("Nothing"))
        {
            Debug.LogWarning(" There are no ground layers currently set.. Adding Default.. (JBR_AI_Controller) Please add all layers the AI can walk on !!!");
            groundLayerMasks = LayerMask.GetMask("Default");
        }

        EvaluateAllBehaviors();
    }

    /// <summary>
    /// if a behavior is added or removed this looks through and resets the behavior lists
    /// </summary>
    public void EvaluateAllBehaviors()
    {
        Debug.Log("Evaluating Behaviors...");
        behavior.Clear();
        behaviorNames.Clear();
        behavior.AddRange(GetComponents<JBR_Base_Behavior_State>());
        foreach (JBR_Base_Behavior_State states in behavior)
        {
            behaviorNames.Add(states.componentName);
            states.Initialize(this, ai_Agent, ai_Animator, ai_AudioSource);


            foreach(ActivatorSystem activators in states.behaviorActivators)
            {
                if( activators.trigger == Activators.healthAboveAmount || activators.trigger == Activators.healthAbovePercent || activators.trigger == Activators.healthBelowAmount || activators.trigger == Activators.healthBelowPercent)
                {
                    OnHealthChanged.AddListener(states.OnChangedHealthEvent);
                }

                if(activators.trigger == Activators.distanceCloserThan || activators.trigger == Activators.distanceFurtherThan)
                {
                    OnDistanceChanged.AddListener(states.OnChangedDistanceEvent);
                }

                if(activators.trigger == Activators.takeDamageAboveAmount || activators.trigger == Activators.takeDamageAbovePercent || activators.trigger == Activators.takeDamageBelowAmount || activators.trigger == Activators.takeDamageBelowPercent || activators.trigger == Activators.takeDamageZero)
                {
                    OnDamageChanged.AddListener(states.OnChangedDamageEvent);
                }


            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
    
    }

    private void OnEnable()
    {
        Debug.Log("AI Enabled Starting InvokeRepeating...");
        InvokeRepeating("SlowUpdate", slowUpdate, slowUpdate);
    }

    private void OnDisable()
    {
        Debug.Log("Cancel Invoke... ");
        CancelInvoke();
    }

    // Update is called once per frame
    void Update()
    {
        // Timer for Active Behavior
        if(behaviourTimer > 0)
        {
            behaviourTimer -= Time.deltaTime;
        }

        //Update active behaviour
        if (m_ActiveBehavior != null)
        {
            m_ActiveBehavior.UpdateState();
        }
        //Update any behaviour that has forced update checked
        for (int i = 0; i < behavior.Count; i++)
        {
            if (behavior[i].forceUpdate && behavior[i] != m_ActiveBehavior)
            {
                behavior[i].UpdateState();
            }
        }
        TryEnterBehaviour();
    }
    // Update is called once per Physics frame
    void FixedUpdate()
    {
        //Grounded Checks
       Collider[] cols = Physics.OverlapSphere(this.transform.position, .1f, mask, QueryTriggerInteraction.Ignore);
        if(cols.Length > 0)
        {
            isGrounded = true;
            isFalling = false;
        }
        else
        {
            isGrounded = false;
            if(fallTimer <= 0)
            {
                fallTimer = .1f;
            }
        }
        //Falling Checks
        if(fallTimer > 0)
        {
            fallTimer -= Time.deltaTime;
            if(fallTimer <= 0)
            {
                isFalling = true;
            }
        }

        //dead check
        if(currentHealth <= 0)
        {
            isDead = true;
        }


        if (m_ActiveBehavior != null)
        {
            m_ActiveBehavior.FixedUpdateState();
        }
        //Update any behaviour that has forced Fixed Update checked
        for (int i = 0; i < behavior.Count; i++)
        {
            if (behavior[i].forceFixedUpdate && behavior[i] != m_ActiveBehavior)
            {
                behavior[i].FixedUpdateState();
            }
        }

    }

    /// <summary>
    /// A slow update used for code that needs to be checked on a few times per second
    /// </summary>
    void SlowUpdate()
    {
        //check current animation
        AnimatorClipInfo[] curClip = ai_Animator.GetCurrentAnimatorClipInfo(0);

        //check if current animation is Idle
        if (curClip.Length > 0 && curClip[0].clip.name == "Idle")
        {
            isIdle = true;
         //   Debug.Log(curClip[0].clip.name);
        }
        else
        {
            isIdle = false;
        }
       

        //check targets
        if (m_ActiveBehavior != null)
        {
            m_ActiveBehavior.SlowUpdateState();
        }
        //Update any behaviour that has forced Slow Update checked
        for (int i = 0; i < behavior.Count; i++)
        {
            if (behavior[i].forceSlowUpdate && behavior[i] != m_ActiveBehavior)
            {
                behavior[i].SlowUpdateState();
            }
        }
    }

    /// <summary>
    /// loops through each list of behaviors and returns a true or false if all behaviors passed testing
    /// </summary>
    /// <param name="behaviorActivators"></param>
    /// <returns></returns>
    public bool CheckActivators(List<ActivatorSystem>  behaviorActivators)
    {
        bool tryEnterBehaviorCheck = true;
        //loop through behaviours
        for (int e = 0; e < behaviorActivators.Count; e++)
        {
            // Check Trigger1
            if (behaviorActivators[e].trigger == Activators.true_Trigger1)
            {
                if (behaviorActivators[e].triggerIsTrue == isTrigger1)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.true_Grounded.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.true_Grounded.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Trigger2
            if (behaviorActivators[e].trigger == Activators.true_Trigger2)
            {
                if (behaviorActivators[e].triggerIsTrue == isTrigger2)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if(behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.true_Grounded.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.true_Grounded.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Grounded
            if (behaviorActivators[e].trigger == Activators.true_Grounded)
            {
                if (behaviorActivators[e].triggerIsTrue == isGrounded)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.true_Grounded.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.true_Grounded.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check idle
            if (behaviorActivators[e].trigger == Activators.true_Idle)
            {
                if (behaviorActivators[e].triggerIsTrue == isIdle)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.true_Idle.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.true_Idle.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Falling
            if (behaviorActivators[e].trigger == Activators.true_Falling)
            {
                if (behaviorActivators[e].triggerIsTrue == isFalling)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.true_Falling.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.true_Falling.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Dead
            if (behaviorActivators[e].trigger == Activators.true_Dead)
            {
                if (behaviorActivators[e].triggerIsTrue == isDead)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.true_Dead.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //   Debug.Log(behavior[i].ToString() + " didnt match " + Activators.true_Dead.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Can Have Target
            if (behaviorActivators[e].trigger == Activators.true_CanHaveTarget)
            {
                if (behaviorActivators[e].triggerIsTrue == canHaveTarget)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.true_CanHaveTarget.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //   Debug.Log(behavior[i].ToString() + " didnt match " + Activators.true_CanHaveTarget.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Has Target
            if (behaviorActivators[e].trigger == Activators.true_hasTarget)
            {
                if (behaviorActivators[e].triggerIsTrue == currentTarget)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //   Debug.Log(behavior[i].ToString() + " " + Activators.true_hasTarget.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.true_hasTarget.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Can Hear Target
            if (behaviorActivators[e].trigger == Activators.true_canHearTarget)
            {
                if (behaviorActivators[e].triggerIsTrue == canHearTarget)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //  Debug.Log(behavior[i].ToString() + " " + Activators.true_canHearTarget.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //   Debug.Log(behavior[i].ToString() + " didnt match " + Activators.true_canHearTarget.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Can See Target
            if (behaviorActivators[e].trigger == Activators.true_canSeeTarget)
            {
                if (behaviorActivators[e].triggerIsTrue == canSeeTarget)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.true_canSeeTarget.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.true_canSeeTarget.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Wandering
            if (behaviorActivators[e].trigger == Activators.true_Wandering)
            {
                if (behaviorActivators[e].triggerIsTrue == isWandering)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //   Debug.Log(behavior[i].ToString() + " " + Activators.true_Wandering.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //   Debug.Log(behavior[i].ToString() + " didnt match " + Activators.true_Wandering.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Distance Closer Than
            if (behaviorActivators[e].trigger == Activators.distanceCloserThan)
            {
                if (behaviorActivators[e].triggerRange > distanceToTarget && distanceToTarget != 0)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //   Debug.Log(behavior[i].ToString() + " " + Activators.distanceCloserThan.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //   Debug.Log(behavior[i].ToString() + " didnt match " + Activators.distanceCloserThan.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Distance Further Than
            if (behaviorActivators[e].trigger == Activators.distanceFurtherThan)
            {
                if (behaviorActivators[e].triggerRange < distanceToTarget && distanceToTarget != 0)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //  Debug.Log(behavior[i].ToString() + " " + Activators.distanceFurtherThan.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //   Debug.Log(behavior[i].ToString() + " didnt match " + Activators.distanceFurtherThan.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Damage Taken above amount
            if (behaviorActivators[e].trigger == Activators.takeDamageAboveAmount)
            {
                if (behaviorActivators[e].triggerRange < damageTaken && damageTaken > 0)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //   Debug.Log(behavior[i].ToString() + " " + Activators.takeDamageAboveAmount.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //   Debug.Log(behavior[i].ToString() + " didnt match " + Activators.takeDamageAboveAmount.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Damage Taken below amount
            if (behaviorActivators[e].trigger == Activators.takeDamageBelowAmount)
            {
                if (behaviorActivators[e].triggerRange > damageTaken && damageTaken > 0)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.takeDamageBelowAmount.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.takeDamageBelowAmount.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Damage Taken Above % amount
            if (behaviorActivators[e].trigger == Activators.takeDamageAbovePercent)
            {
                if (behaviorActivators[e].triggerRange < (damageTaken / maxHealth) && damageTaken > 0)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //     Debug.Log(behavior[i].ToString() + " " + Activators.takeDamageAbovePercent.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.takeDamageAbovePercent.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Damage Taken Bel0w % amount
            if (behaviorActivators[e].trigger == Activators.takeDamageBelowPercent)
            {
                if (behaviorActivators[e].triggerRange > (damageTaken / maxHealth) && damageTaken > 0)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.takeDamageBelowPercent.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.takeDamageBelowPercent.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }

            // Check Damage Taken Zero
            if (behaviorActivators[e].trigger == Activators.takeDamageZero)
            {
                if (damageTaken == 0)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.takeDamageZero.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.takeDamageZero.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }
            // Check health Below
            if (behaviorActivators[e].trigger == Activators.healthBelowAmount)
            {
                if (behaviorActivators[e].triggerRange > currentHealth)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.takeDamageBelowAmount.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.takeDamageBelowAmount.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }
            // Check health Below %
            if (behaviorActivators[e].trigger == Activators.healthBelowPercent)
            {
                if (behaviorActivators[e].triggerRange > currentHealth / maxHealth)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.takeDamageBelowAmount.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.takeDamageBelowAmount.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }
            // Check health Above
            if (behaviorActivators[e].trigger == Activators.healthAboveAmount)
            {
                if (behaviorActivators[e].triggerRange < currentHealth)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.takeDamageBelowAmount.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.takeDamageBelowAmount.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }
            // Check health Above %
            if (behaviorActivators[e].trigger == Activators.healthAbovePercent)
            {
                if (behaviorActivators[e].triggerRange < currentHealth / maxHealth)
                {
                    if (behaviorActivators[e].OncePerEvent)
                    {
                        if (behaviorActivators[e].lastTime != time)
                        {
                            continue;
                        }
                        else
                        {
                            tryEnterBehaviorCheck = false;
                            return tryEnterBehaviorCheck;
                        }
                    }
                    //    Debug.Log(behavior[i].ToString() + " " + Activators.takeDamageBelowAmount.ToString() + "Behavior check is good.. Moving on..");
                    continue;
                }
                else
                {
                    //    Debug.Log(behavior[i].ToString() + " didnt match " + Activators.takeDamageBelowAmount.ToString());
                    tryEnterBehaviorCheck = false;
                    return tryEnterBehaviorCheck;
                }
            }
        }

        return tryEnterBehaviorCheck;
    }

    /// <summary>
    /// Tests each behavior and enters it if all parameters pass
    /// </summary>
    private void TryEnterBehaviour()
    {
        if (isDead)
        {
            return;
        }

        for (int i = 0; i < behavior.Count; i++)
        {
            //only check behaviours that dont have a forced Update already in use...
            if (!behavior[i].forceFixedUpdate && !behavior[i].forceUpdate && !behavior[i].forceSlowUpdate )
            {
                //if behaviour is a forced override behavior or last behaviour timer has ended then we can check 
                if (behaviourTimer <= 0 || behavior[i].forceBehaviour )
                {
                    //if behaviour is a forced override behavior and not the current behavior 
                    if (behavior[i].forceBehaviour && behavior[i] == m_ActiveBehavior)
                    {
                        continue;
                    }
                    bool tryEnterBehaviorCheck = false;
                    //only check enabled behaviors
                    if (behavior[i].enabledBehavior)
                    {     
                        // if all checks pass this this behavior will return true 
                        tryEnterBehaviorCheck = CheckActivators(behavior[i].behaviorActivators);
                    }
                
                    //if try enter stayed true then this behavior passed all test and should start.
                    if (tryEnterBehaviorCheck == true)
                    {
                        Debug.Log("All Parameters passed " + behavior[i].ToString() + " Try Start Behavior !!!");
                        // see if behavior has other specific enterence needs
                        bool canEnter = behavior[i].TryEnterBehaviour();
                        Debug.Log(behavior[i].componentName + " can Enter Ability > " + canEnter.ToString());
                        if (canEnter)
                        {
                            if(m_ActiveBehavior == behavior[i])
                            {
                                //// stays in active behavior
                            }
                            else
                            {
                                if (m_ActiveBehavior != null)
                                {
                                    m_ActiveBehavior.OnExitAbility();                                  
                                }

                                m_ActiveBehavior = behavior[i];
                                activeBehavior = (m_ActiveBehavior.componentName + " "+m_ActiveBehavior.ToString());
                            }
                            //Set lastTime
                            for (int b = 0; b < behavior[i].behaviorActivators.Count; b++)
                            {
                              //  if (behavior[i].behaviorActivators[b].OncePerEvent)
                              //  {
                                    behavior[i].behaviorActivators[b].lastTime = time;
                              //  }
                            }
                                //reset behaviorTimer
                                behaviourTimer = 0;
                                if (behavior[i].animationSet.Length > 0)
                                {
                                    //add all behavior animation times together (if not play random clip) to set new BehaviorTimer..
                                    if (behavior[i].playAllAnimationClips)
                                    {
                                        for (int a = 0; a < behavior[i].animationSet.Length; a++)
                                        {
                                            behaviourTimer += behavior[i].animationSet[a].length;
                                        }
                                    }
                                    else
                                    {
                                        if (behavior[i].playRandomAnimationClip)
                                        {
                                            behavior[i].randomClipRef = Random.Range(0, behavior[i].animationSet.Length);
                                            behaviourTimer = behavior[i].animationSet[behavior[i].randomClipRef].length;
                                        }
                                        else
                                        {
                                            if (behavior[i].randomClipRef >= 0 && behavior[i].randomClipRef < behavior[i].animationSet.Length)
                                            {
                                                behaviourTimer = behavior[i].animationSet[behavior[i].randomClipRef].length;
                                            }
                                            else
                                            {
                                                behavior[i].randomClipRef = 0;
                                                behaviourTimer = behavior[i].animationSet[behavior[i].randomClipRef].length;
                                            }
                                        }
                                    }
                                    behavior[i].OnEnterAbility();
                                    //dont check any other behaviors (We Pick the first passed behavior, note: behavior order in Inspector is important.)
                                    return;
                                }
                                else
                                {
                                // there is no animation clip for this behaviour
                                behaviourTimer = 1;
                                behavior[i].OnEnterAbility();
                            }
                        }
                        else
                        {
                            //behavior failed with special permissions so move to next...
                            continue;
                        }
                    }
                }
            }
            else
            {
                continue;
            }
        }
    }

    /// <summary>
    /// Called when AI Recieves Damage
    /// </summary>
    /// <param name="Damage"></param>
    /// <param name="attacker"></param>
    /// <param name="hitPosition"></param>
    public void RecieveDamage(float Damage, Transform attacker, Vector3 hitPosition)
    {
        if (isDead)
        {
            return;
        }
     //   Debug.Log("Damage Taken _" + Damage + " at_" + hitPosition + " by_" + attacker.name);
        damageTaken = Damage;
        OnHealthChanged.Invoke();
        time = Time.timeSinceLevelLoad;
        currentHealth -= damageTaken;
        //if attacked make attacker the new target
        if(currentTarget == null || currentTarget != attacker.gameObject)
        {
            currentTarget = attacker.gameObject;
        }

        if(currentHealth <= 0)
        {
            currentHealth = 0;
           // isDead = true;
        }
    }

    /// <summary>
    /// Kills the ai 
    /// </summary>
    public void Die()
    {
        Debug.Log("Dead >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        isDead = true;
        ai_Animator.SetTrigger("Dead");
        GameObject.FindGameObjectWithTag("Player").SendMessageUpwards("removeDeadTarget", this.transform, SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Called when AI Gives Damage
    /// </summary>
    /// <param name="Damage"></param>
    /// <param name="attacker"></param>
    /// <param name="hitPosition"></param>
    public void GiveDamage(float Damage, Transform attacked, Vector3 hitPosition)
    {
    //    Debug.Log("Damage Given _" + Damage + " at_" + hitPosition + " To_" + attacked.name);
        damageGiven = Damage;
        time = Time.timeSinceLevelLoad;
    }

    /// <summary>
    /// Sets a trigger1 on AI
    /// </summary>
    /// <param name="trigger"></param>
    public void SetTrigger1( bool trigger)
    {      
            isTrigger1 = trigger;      
    }

    /// <summary>
    /// Sets a trigger1 on AI
    /// </summary>
    /// <param name="trigger"></param>
    public void SetTrigger2(bool trigger)
    {
        isTrigger2 = trigger;
    }

}

   // public enum Behavior { Locamotion, Attack, Wander };

                            //bool Activators
    /// <summary>
    /// Behavior Activator options 
    /// </summary>
    public enum Activators {true_Grounded,true_Idle,true_Falling,true_Dead,true_Wandering, //,true_Attacking,true_Defending
                            true_CanHaveTarget,true_hasTarget,true_canSeeTarget,true_canHearTarget,true_Trigger1,true_Trigger2,
                            //float Acticators
                            distanceCloserThan,distanceFurtherThan,takeDamageBelowPercent,takeDamageBelowAmount,takeDamageAbovePercent,takeDamageAboveAmount,takeDamageZero,
                            healthBelowPercent,healthBelowAmount,healthAbovePercent,healthAboveAmount };

[System.Serializable]
public class ActivatorSystem
{
    [Tooltip("Select the Activator Trigger")]
    public Activators trigger;
    [Tooltip("Set the Activator Number if it takes a number range. examples would be : distance away or damage taken.  Note: setting to -1 will reset triggers after Behavior is triggered")]
    public float triggerRange;
    [Tooltip("Select if the Activator Parameter should be true or false to activate this behaviour")]
    public bool triggerIsTrue = false;
    [Tooltip("if true a time stamp match is made when this triggers and it can not be triggered again until <time> is changed in AI Controller")]
    public bool OncePerEvent = false;
    [Tooltip("How long until this behavior can be activated again after was it was last activated ")]
    public float activationCooldown = 0;
    [HideInInspector]
    public float lastTime = -1;
    [HideInInspector]
    public bool hasChanged = true;
}
