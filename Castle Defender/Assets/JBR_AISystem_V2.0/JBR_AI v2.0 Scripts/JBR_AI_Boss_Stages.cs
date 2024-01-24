using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;


/// <summary>
/// This Behaviour should never be called to Try Enter it only recieves Health change events to trigger new stages
/// </summary>
public class JBR_AI_Boss_Stages : JBR_Base_Behavior_State
{
   // [Tooltip("When checked the editor will continusly update the behavior names to properly match the drop components , this should only be check while editing the stages and behaviors")]
  //  [HideInInspector]
  //  public bool allowEditorAutoFill = true;
    [Tooltip("The list of behaviors that should be enabled or disabled based off current stage needs, Only add components that need to change enable/disabled")]
    public List<Stages> stages = new List<Stages>();
    [Tooltip("The Health amount per stage, 1st should = max health while last should be zero")]
    public float[] healthStages = { 500, 400, 200, 0 };
    [Tooltip("Dynamically Set the current Stage")]
    public int currentStage = -1;


    //internal
   
    private int refStage = -1;

    // Start is called before the first frame update
    public override void Initialize(JBR_AI_ControllerSystem mainSystem, NavMeshAgent ai_Agent, Animator ai_Animator, AudioSource ai_AudioSource)
    {      
        base.Initialize(mainSystem, ai_Agent, ai_Animator, ai_AudioSource);

        mainSystem.OnHealthChanged.AddListener(OnChangedHealthEvent);
        OnChangedHealthEvent();
    }

    private void OnDisable()
    {
        m_AI_Controller.OnHealthChanged.RemoveListener(OnChangedHealthEvent);
    }

    /// <summary>
    /// Try Enter behavior, this should return a bool of true or false if their are extra checks
    /// </summary>
    /// <returns></returns>
    public override bool TryEnterBehaviour()
    {
       return base.TryEnterBehaviour();
       // return false;
    }

    /// <summary>
    /// On Enter Behavior
    /// </summary>
    public override void OnEnterAbility()
    {      
        base.OnEnterAbility();
    }


    public void UpdateStage(int currentStageSet)
    {
        Debug.Log("Updating Stage info");
        currentStage = currentStageSet;
        refStage = currentStageSet;

        //enable or disable each behaviour
        for (int i = 0; i < stages[currentStage].behaviourSetup.Count; i++)
        {
            stages[currentStage].behaviourSetup[i].behavior.enabledBehavior = stages[currentStage].behaviourSetup[i].enableBehavior;
        }

        //Invoke unity event for the stage
        stages[currentStage].OnEnterStageEvent.Invoke();
        if(currentStage > 0)
        {
            stages[currentStage - 1].OnExitStageEvent.Invoke();
        }
    }

    public override void OnChangedHealthEvent()
    {
        Debug.Log("OnChangedHealthEvent********");

        float health = m_AI_Controller.currentHealth;
        if (health <= healthStages[0] && health > healthStages[1])
        {
            currentStage = 0;
        }
        if (health <= healthStages[1] && health > healthStages[2])
        {
            currentStage = 1;
        }
        if (health <= healthStages[3] && health > healthStages[4])
        {
            currentStage = 3;
        }
        if (health <= healthStages[4] && health > healthStages[5])
        {
            currentStage = 4;
        }


        if (currentStage != refStage)
        {
            UpdateStage(currentStage);
        }
        base.OnChangedHealthEvent();
    }
}

[System.Serializable]

public class Stages
{
    [Tooltip("Stage Name for reference, not needed")]
    public string stageName;
    [Tooltip("add behaviors that need to be enabled or disabled here")]
    public List<BehaviorSetup> behaviourSetup = new List<BehaviorSetup>();

    [Space]
    [Header("_____________________________________________________________________________________________________________________________________")]
    [Header("Unity Events are called when the behavior is started or ended," +
           " use these Events to trigger other actions. " +
           "examples particles or interactions")]


    public UnityEvent OnEnterStageEvent;
    public UnityEvent OnExitStageEvent;
   // [Space]

    // [Tooltip("the minimum health to start this")]
    //  public float healthMin = 0;
    //  public float healthMax = 1;
}

[System.Serializable]
public class BehaviorSetup
{
    [Tooltip("Reference Behavior Name")]
    public string behaviorName = "";
    [Tooltip("Reference Behavior to etither enable or disable ")]
    public JBR_Base_Behavior_State behavior;
    [Tooltip("Check this box to Enable Component, otherwise the component will be disabled")]
    public bool enableBehavior;
}
