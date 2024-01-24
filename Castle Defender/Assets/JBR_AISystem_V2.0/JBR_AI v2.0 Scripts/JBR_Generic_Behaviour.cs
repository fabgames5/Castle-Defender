using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JBR_Generic_Behaviour : JBR_Base_Behavior_State
{
    
    private IEnumerator coroutine;
    protected Animator animator;
    protected AnimatorOverrideController animatorOverrideController;
    public float timer;
    public bool playRandomSounds = false;
    public float randomTimeSpacing = 5.0f;
    [Tooltip("The animation layer this animation should play in")]
    public int AnimationLayer = 0;
    [Space]
    [Space]
    public bool updateHeight = false;
    public Transform ai_BodyRoot;
    public bool moveUp = false;
    public float flyheight = 3;
    private CapsuleCollider capCol;
    private float heightTimer;
   // private Vector3 capColPos;


    public override void Initialize(JBR_AI_ControllerSystem mainSystem, NavMeshAgent ai_Agent, Animator ai_Animator, AudioSource ai_AudioSource)
    {
        capCol = this.GetComponent<CapsuleCollider>();
    //    capColPos = capCol.center;
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
        if (playRandomSounds)
        {
            bool canActivate = true;
            // if there are behaviour Activators check if they pass 
            if (behaviorActivators.Count > 0)
            {
                canActivate = m_AI_Controller.CheckActivators(behaviorActivators);
            }
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = 0;
                }
            }

            if(timer == 0 && canActivate)
            {
                PlayRandomSound();
            }
        }

        if (updateHeight)
        {
            if(moveUp == false) {
                if (ai_BodyRoot.position.y > 0)
                {
                    heightTimer += Time.deltaTime;
                    ai_BodyRoot.position = Vector3.Lerp(new Vector3(this.transform.position.x, flyheight, this.transform.position.z), Vector3.zero, heightTimer);
                    capCol.center = (ai_BodyRoot.localPosition);
                }
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
            if(behaviorActivators[i].OncePerEvent && behaviorActivators[i].lastTime == m_AI_Controller.time)
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
        base.OnEnterAbility();

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
            randomClipRef = Random.Range(0, animationSet.Length);
            if (!playRandomAudioClip)
            {
                coroutine = PlayAnimation(animationSet[randomClipRef], 0, time, randomClipRef);
            }
            else
            {
                //picks one audio clip randomly to play
                int aClip = Random.Range(0, audioClips.Length);
                coroutine = PlayAnimation(animationSet[randomClipRef], 0, time, aClip);
            }
            StartCoroutine(coroutine);
        }

        for (int i = 0; i < behaviorActivators.Count; i++)
        {
            if (behaviorActivators[i].trigger == Activators.true_Trigger1)
            {
                if (behaviorActivators[i].triggerRange == -1 && behaviorActivators[i].triggerIsTrue)
                {
                    m_AI_Controller.SetTrigger1(false);
                }
            }

            if (behaviorActivators[i].trigger == Activators.true_Trigger2)
            {
                if (behaviorActivators[i].triggerRange == -1 && behaviorActivators[i].triggerIsTrue)
                {
                    m_AI_Controller.SetTrigger2(false);
                }
            }
        }
       
       
    }

    /// <summary>
    /// Exit Ability 
    /// </summary>
    public override void OnExitAbility()
    {

        Debug.Log("*****Exit Generic Ability***** " + this.componentName);
        base.OnExitAbility();
    }

    /// <summary>
    /// Plays Animation Clip, In set Layer, after set delay
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="layer"></param>
    /// <param name="wait"></param>
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

        // add Logic here
    }

    /// <summary>
    /// Plays a random sound
    /// </summary>
    public void PlayRandomSound()
    {
        if (!m_AI_Controller.isDead)
        {
            int rando = Random.Range(0, audioClips.Length);
            m_AI_AudioSource.PlayOneShot(audioClips[rando]);

            float timeSpacing = Random.Range(0, randomTimeSpacing);
            timer = (audioClips[rando].length + timeSpacing);
        }
    }


}
