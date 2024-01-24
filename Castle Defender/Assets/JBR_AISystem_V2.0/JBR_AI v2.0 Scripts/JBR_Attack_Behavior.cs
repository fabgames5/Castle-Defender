using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JBR_Attack_Behavior: JBR_Base_Behavior_State
{

    private IEnumerator coroutine;
    protected Animator animator;
    protected AnimatorOverrideController animatorOverrideController;
    [Tooltip("Add all weapons the AI character can possibly use here.")]
    public JBR_Weapon_Base.WeaponTypes weaponType = JBR_Weapon_Base.WeaponTypes.handsTrigger;
    [Tooltip("Set a delay for shooting a projectile/ or melee here")]
    public float weaponActiveDelay = 0;
    [Tooltip("Add weapons here")]
    public JBR_Weapon_Base[] weapons;
    
  
    [Tooltip("The animation layer this animation should play in")]
    public int AnimationLayer = 0;

    public override void Initialize(JBR_AI_ControllerSystem mainSystem, NavMeshAgent ai_Agent, Animator ai_Animator, AudioSource ai_AudioSource)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].Initialize(mainSystem, this, ai_Animator, ai_AudioSource);
            }
            else
            {
                Debug.LogError(".... Weapons ( " + i + " ) is missing on JBR_Attack Behavior, please add a JBR_Weapon ....");
            }
        }

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
    /// Main Update for each ability. It's updated in the controller
    /// </summary>
    public override void UpdateState()
    { 
        // runUpdate code on all active weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].gameObject.activeSelf)
            {
                weapons[i].UpdateState();
            }
        }
            base.UpdateState();
    }

    /// <summary>
    /// Slow update for each ability. It's updated in the controller
    /// </summary>
    public override void SlowUpdateState()
    {
        base.SlowUpdateState();
    }

    public override bool TryEnterBehaviour()
    {
        bool canStart = false;
        // add logic to check if this ability can be enter
        // return true if so;
        for (int i = 0; i < behaviorActivators.Count; i++)
        {
            if (behaviorActivators[i].OncePerEvent && behaviorActivators[i].lastTime == m_AI_Controller.time)
            {
                canStart = false;
                base.TryEnterBehaviour();
                return canStart;
            }
        }
        canStart = true;
        base.TryEnterBehaviour();
        return canStart;

    }

    /// <summary>
    /// On Enter Behavior
    /// </summary>
    public override void OnEnterAbility()
    {
        if (m_AI_Controller.currentTarget != null)
        {
            m_AI_Animator.transform.LookAt(m_AI_Controller.currentTarget.transform);          
        }

        Debug.Log("On Enter Behavior Animation " + this.componentName);
        float time = .01f;
        animator = m_AI_Animator;
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        //Plays all Animation clips in order
        if (!playRandomAnimationClip)
        {
            if (playAllAnimationClips)
            {
                for (int i = 0; i < animationSet.Length; i++)
                {
                    if (!playRandomAudioClip)
                    {
                        coroutine = PlayAnimation(animationSet[i], AnimationLayer, time, i);
                    }
                    else
                    {
                        //picks one audio clip randomly to play
                        int aClip = Random.Range(0, audioClips.Length);
                        coroutine = PlayAnimation(animationSet[i], AnimationLayer, time, aClip);
                    }
                    StartCoroutine(coroutine);
                    time += animationSet[i].length;                  
                }
            }
            else
            {
                if (randomClipRef < 0 || randomClipRef >= animationSet.Length)
                {
                    randomClipRef = 0;
                }


                if (!playRandomAudioClip)
                {
                    coroutine = PlayAnimation(animationSet[randomClipRef], AnimationLayer, time, randomClipRef);
                }
                else
                {
                    //picks one audio clip randomly to play
                    int aClip = Random.Range(0, audioClips.Length);
                    coroutine = PlayAnimation(animationSet[randomClipRef], AnimationLayer, time, aClip);
                }

                randomClipRef += 1;
                StartCoroutine(coroutine);
            }
        }
        else
        {
            //picks one animation clip randomly to play
           // randomClipRef = Random.Range(0, animationSet.Length);
            if (!playRandomAudioClip)
            {
                coroutine = PlayAnimation(animationSet[randomClipRef], AnimationLayer, time, randomClipRef);
            }
            else
            {
                //picks one audio clip randomly to play
                int aClip = Random.Range(0, audioClips.Length);
                coroutine = PlayAnimation(animationSet[randomClipRef], AnimationLayer, time, aClip);
            }
            StartCoroutine(coroutine);
        }
        base.OnEnterAbility();
    }

    public override void OnExitAbility()
    {
      
        Debug.Log("*****Exit Attack Ability*****");
        for (int i = 0; i < weapons.Length; i++)
        {

        }
        base.OnExitAbility();
    }

    /// <summary>
    /// Plays Animation Clip, In set Layer, after set delay
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="layer"></param>
    /// <param name="wait"></param>
    /// <param name="refNumb"></param>
    /// <returns></returns>
    IEnumerator PlayAnimation(AnimationClip clip, int layer, float wait, int refNumb)
    {
        yield return new WaitForSeconds(wait);

        animatorOverrideController[animationOverRideClipName] = clip;
        m_AI_Animator.SetTrigger("GenericAnimation");

        if (audioClips[refNumb] != null && m_AI_AudioSource != null)
        {
            m_AI_AudioSource.PlayOneShot(audioClips[refNumb]);
        }
        else
        {
            Debug.LogError("AudioClip or AudioSource is missing for " + this.componentName + " > " + refNumb);
        }

        Debug.Log("PLay Animation Clip > Time >" + wait);

        Invoke("Attack", weaponActiveDelay);
        
    }

    public void Attack()
    {
        //Starts Attack Behaviour on all active weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].gameObject.activeSelf)
            {
                weapons[i].type = weaponType;
                weapons[i].OnEnterAbility();
            }
        }
    }

}
