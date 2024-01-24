using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]

public abstract class JBR_Weapon_Base : MonoBehaviour
{
  //  [Tooltip("Check this if you want this script to handle damage. Note; this is a very generic damage system ")]
  //  public bool basicDamage = false;
    [Tooltip("the type of wepon this is...")]
    public WeaponTypes type = WeaponTypes.handsTrigger;
    [Space]
    public string DamageMessage = "DamageHealth";
    public string AttackerID = "AttackerID";
    [Space]
    public float damage = 35.0f;
    [Tooltip("the delay from start of animation to when damage is actually taken, a small delay for hit reaction or death")]
    public float damageDelay = 1.0f;

    [Space]
    [Header("_____________________________________________________________________________________________________________________________________")]
    [Header("Unity Events are called when the behavior is started or ended," +
            " use these Events to trigger other actions. " +
            "examples particles or interactions")]

    public UnityEvent OnEnterBehaviorEvent;
    public UnityEvent OnExitBehaviorEvent;
    [Space]
    [HideInInspector]
    public JBR_AI_ControllerSystem m_AI_Controller;
    [HideInInspector]
    public Animator m_AI_Animator;
    [HideInInspector]
    public AudioSource m_AI_AudioSource;
    //[HideInInspector]
    public JBR_Attack_Behavior m_AI_Attack;


    /// <summary>
    /// Intialize Needed Component references
    /// </summary>
    /// <param name="mainSystem"></param>
    /// <param name="ai_Agent"></param>
    /// <param name="ai_Animator"></param>
    public virtual void Initialize(JBR_AI_ControllerSystem mainSystem,JBR_Attack_Behavior ai_Attack ,Animator ai_Animator, AudioSource ai_AudioSource)
    {
        m_AI_Controller = mainSystem;
        m_AI_Attack = ai_Attack;
        m_AI_Animator = ai_Animator;
        m_AI_AudioSource = ai_AudioSource;

    }

    /// <summary>
    /// Method called in the moment that ability is entered. Called once.
    /// </summary>
    public virtual void OnEnterAbility()
    {
        OnEnterBehaviorEvent.Invoke();
    }

    /// <summary>
    /// Update for each ability. It's updated in the controller
    /// </summary>
    public virtual void UpdateState()
    {

    }

    /// <summary>
    /// Method called in the moment that ability exit. Called once.
    /// </summary>
    public virtual void OnExitAbility()
    {
        OnExitBehaviorEvent.Invoke();
    }   

    public enum WeaponTypes {handsTrigger, handCollider,knifeTrigger,knifeCollider,gun,throwing,spell};
}
