using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
[System.Serializable]

public abstract class JBR_Base_Behavior_State : MonoBehaviour
{
    [Tooltip("When stacking mutiple Behaviors, set a new Name here for better reference to current behaviors in action.")]
    [HideInInspector]
    public string componentName = "";
    [Tooltip("allows this behavior to be used")]
    public bool enabledBehavior = true;
    [Header("_____________________________________________________________________________________________________________________________________")]
    [Header("Check Boxes of any Forced Updating this Behaviour should use, " +
        "Note this behavior will only update with check behaviors" +
        " and never be set as AI_Controller's Current Behaviour" +
        " Also Behaviour Activators will no longer be used")]   

    [Tooltip("Checking this box will Make this behaviour Run the Update Loop every Frame")]
    public bool forceUpdate = false;
    [Tooltip("Checking this box will Make this behaviour Run the FixedUpdate Loop every Physics Frame")]
    public bool forceFixedUpdate = false;
    [Tooltip("Checking this box will Make this behaviour Run the Slow Update Loop every SlowUpdate time setting")]
    public bool forceSlowUpdate = false;
   
    [Header("_____________________________________________________________________________________________________________________________________")]
    [Header("Add Behaviour Activator Parameters Here")]
    
    [SerializeField]
    [Tooltip("if it has True_ Prefix then only ( Trigger Is True ) parameter affects it's outcome...")]
    public List<ActivatorSystem> behaviorActivators = new List<ActivatorSystem>();
  //  public List<UnityEvent> events = new List<UnityEvent>();
    //public UnityEvent bob;

    [Header("_____________________________________________________________________________________________________________________________________")]
    [Header("Special Animations to Use for this behaviour, note adding more than one will cause each one to run after another unless (Play Random Clip) is checked")]

    [Tooltip("Set of Animations to Play for this behaviour ")]
    public AnimationClip[] animationSet;
    [Tooltip("The animation clip in the controller that will be replaced, Note: this should be a generic clip that has no real use in the Animation Controller.")]
    public string animationOverRideClipName = "Throw";
    [Tooltip("if checked this behaviour will interupt the previous behaviour's animation timer to start, good uses would be falling or Dieing")]
    public bool forceBehaviour = false;
    //  [Tooltip("if checked, All Animations must finish before another behaviour can run")]
    //  public bool forceAnimationFinish = true;
    [Tooltip("if checked, Only one of the animation clips will be played, and it will be selected randomly")]
    public bool playRandomAnimationClip = false;
    [Tooltip("if Checked all animations will play one after the other as one animation")]
    public bool playAllAnimationClips = false;
    [HideInInspector]
    public int randomClipRef = -1;
    //  [Tooltip("How quickly this Behavior can be started again")]
    //   public float replayTime = 3.0f;
    [Header("Sounds to play for this behaviour")]
    [Tooltip("Set of Audio Clips to Play for this behaviour, Note audio clips are matched to animations and should have the same count ")]
    public AudioClip[] audioClips;
    [Tooltip("this will randomize the audioclips, that will play when an animation is triggered, otherwise <audioclip 1 will play with animation clip 1>")]
    public bool playRandomAudioClip;
    [Space]
    [Space]
    [Header("_____________________________________________________________________________________________________________________________________")]
    [Header("Unity Events are called when the behavior is started or ended," +
            " use these Events to trigger other actions. " +
            "examples particles or interactions")]

    public UnityEvent OnEnterBehaviorEvent;
    public UnityEvent OnExitBehaviorEvent;
    [Space]
   
    //[HideInInspector]
    public JBR_AI_ControllerSystem m_AI_Controller;
    [HideInInspector]
    public NavMeshAgent m_AI_Agent;
    [HideInInspector]
    public Animator m_AI_Animator;
    [HideInInspector]
    public AudioSource m_AI_AudioSource;
    private float exitTimer;

    //  [Header("_____________________________________________________________________________________________________________________________________")]


    /// <summary>
    /// Intialize Needed Component references
    /// </summary>
    /// <param name="mainSystem"></param>
    /// <param name="ai_Agent"></param>
    /// <param name="ai_Animator"></param>
    public virtual void Initialize(JBR_AI_ControllerSystem mainSystem, NavMeshAgent ai_Agent, Animator ai_Animator, AudioSource ai_AudioSource)
    {
        m_AI_Controller = mainSystem;
        m_AI_Agent = ai_Agent;
        m_AI_Animator = ai_Animator;
        m_AI_AudioSource = ai_AudioSource;

        Debug.Log(componentName + "Initialized *********");
    }

    /// <summary>
    /// Fixed update for each ability. It's updated in the controller
    /// </summary>
    public virtual void FixedUpdateState()
    {

    }

    /// <summary>
    /// Update for each ability. It's updated in the controller
    /// </summary>
    public virtual void UpdateState()
    {

    }

    /// <summary>
    ///   SLow Update for each ability. It's updated in the controller
    /// </summary>
    public virtual void SlowUpdateState()
    {

    }

    // Check if this behaviour can be entered
    public virtual bool TryEnterBehaviour()
    {
       // Debug.Log("OK");
        return true;
        
    }

    /// <summary>
    /// Method called in the moment that ability is entered. Called once.
    /// </summary>
    public virtual void OnEnterAbility()
    {
        OnEnterBehaviorEvent.Invoke();
        // resets the time check
        for (int i = 0; i < behaviorActivators.Count; i++)
        {
            if (behaviorActivators[i].OncePerEvent)
            {
                behaviorActivators[i].lastTime = m_AI_Controller.time;
            }
        }

    }

    /// <summary>
    /// Method called in the moment that ability exit. Called once.
    /// </summary>
    public virtual void OnExitAbility()
    {
        OnExitBehaviorEvent.Invoke();
    }


    /// <summary>
    /// Events
    /// </summary>
    public virtual void OnChangedHealthEvent()
    {
        //health changed
    }

    /// <summary>
    /// Events
    /// </summary>
    public virtual void OnChangedDistanceEvent()
    {
        //Distance changed
    }

    /// <summary>
    /// Events
    /// </summary>
    public virtual void OnChangedDamageEvent()
    {
        //Damage changed
    }
}
