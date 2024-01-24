using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class JBR_SceneLoader : MonoBehaviour
{
    [Tooltip("check this if the scene loader should use trigger box  entered to load into scene. Note: Can use both trigger and OnInteracted at same time ")]
    public bool useTriggers;
    [Tooltip("check this if the scene loader should use OnInteracted() code to load into scene. Note: Can use both trigger and OnInteracted at same time ")]
    public bool useInteraction;
    [Space]
    [Tooltip("Check This if an OnInteraction /or OntriggerEnter will load the teleport_Scene, note that a Collider is needed")]
    public bool canTeleport = true;
    [Space]
    [Tooltip("The Name of the scene to teleport too...")]
    public string teleport_scene_name;
    [Tooltip("Time Before the Next scene loads, this should typically be longer than fadeTime")]
    public float loadTime = 3.0f;
    [Space]
    [Space]
    [Tooltip("Add AudioClip that will play, like door opening sound")]
    public AudioClip audioClip;
    [Tooltip("How long after Scene fade starts til audioClip plays, set to -1 to never play clip")]
    public float audioClipDelayTime = -1;
    private bool audioClipPlayed = false;
    [Space]
    [Tooltip("How long the scene takes to fade Out")]
    public float fadeTime = 2.0f;
    [Tooltip("Set Dynamically do not adjust, the color of the UI Fader Panel")]
    public Color fadeColor = Color.clear;
    [Tooltip("Add the UI Fader Panel <Image> here from the UI canvas")]
    public Image panelImage;
    [Space]
    [Tooltip("Set Dynamically, the Fade timer")]
    public float timer = 0;
    [Tooltip("Set Dynamically by entering trigger area, Or OnInteraction starts teleporting")]
    public bool isTeleporting = false;
    [Space]
    [Tooltip("Check this if the door is locked and requires a key to unlock")]
    public bool locked = true;
    [Tooltip("Add the Key prefab that will unlock the door here, Note: only the Name actually matters for a match,not a mataching prefab asset")]
    public GameObject key;
    [Tooltip("Copy paste player name here")]
    public string playerName = "Father_Martin";

    private BoxCollider boxCol;
    private AudioSource aS;

    // Start is called before the first frame update
    void Start()
    {
        if(panelImage != null)
        {
            fadeColor = Color.clear;
            panelImage.color = fadeColor;
        }
        else
        {
            Debug.LogWarning("***Warning*** !! NO Canvas Panel Image Found bye > JBR_SceneLoader > on " + this.gameObject.name + " <> Needs Canvas with a Image added To get a fade effect during scene transitions ..");
        }
        
        isTeleporting = false;
        //check for box collider, if no collider then add one
        boxCol = this.gameObject.GetComponent<BoxCollider>();
        if (boxCol == null)
        {
            Debug.LogWarning("***Warning*** !!  No Box Collider was found on JBR_SceneLoader > " + this.gameObject.name + " A box collider was added but you should add one in editor that fits your needed size requirements..");
            boxCol = this.gameObject.AddComponent<BoxCollider>();
        }
        if (useTriggers)
        {
            //box collider must be a trigger
            boxCol.isTrigger = true;
        }

        // get audioSource
        if (aS == null)
        {
            aS = this.gameObject.GetComponent<AudioSource>();
            // on get add an audioSource if there is a clip to play
            if(aS == null && audioClip != null)
            {
                Debug.LogWarning("***Warning*** !!  No AudioSource was found on JBR_SceneLoader > " + this.gameObject.name + " An AudioSource was added but you should add one in editor ..");
                aS = this.gameObject.AddComponent<AudioSource>();
            }
        }

        if(audioClip == null)
        {
            Debug.LogWarning("***Warning*** !!  No AudioClip was found on JBR_SceneLoader > " + this.gameObject.name + " Please add an audioClip to get audio sound while interacting..");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTeleporting)
        {
            //tick timer
            timer += Time.deltaTime;
            //lerp color from clear to black 
            fadeColor = Color.LerpUnclamped(Color.clear, Color.black, timer / fadeTime);
            //set Image color
            if (panelImage != null)
            {
                panelImage.color = fadeColor;
            }

            //plays audioclip once delay time is reached
            if (audioClipDelayTime != -1)
            {
                if (timer >= audioClipDelayTime && audioClip != null && aS != null && audioClipPlayed == false)
                {
                    audioClipPlayed = true;
                    aS.PlayOneShot(audioClip);
                }
            }

            //timer reached teleport time, start Scene load;
            if (timer >= loadTime) 
            {
                if (canTeleport)
                {
                    timer = 0;
                    isTeleporting = false;
                    SceneManager.LoadSceneAsync(teleport_scene_name, LoadSceneMode.Single);
                }
                else
                {
                    Debug.Log("canTeleport was not checked so scene was not changed !!!");
                    isTeleporting = false;
                    if(panelImage!= null)
                    {
                        fadeColor = Color.clear;
                        panelImage.color = fadeColor;                       
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (locked)
        {
       //     bool found = false;
       //     if (GameObject.Find(playerName).GetComponent<PlayerScript>())
       //     {
       //         var items = GameObject.Find(playerName).GetComponent<PlayerScript>().items;
        //        for (var i = 0; i < items.Count; i++)
        //        {
         //           var item = items[i];
         //           if (item.name == key.name)
          //          {
           //             found = true;
           //             items.RemoveAt(i);
           //             break;
            //        }
           //     }
          //  }
          //  else
         //   {
         //       Debug.LogWarning("No Player Found , please check Naming....On Door Scene Loader Script");
         //   }
         //   if (!found)
         //   {
                //Debug.Log("Door is locked");
            //    Helper.Ccs.Clear();
            //    ((CsSubtitleAction)Helper.Ccs.QueueAction(new CsSubtitleAction("Locked"))).SetWaitForUserInput(true).SetAutoclear(true);
             //   Helper.Ccs.Play();
         //       return;
         //   }
        }


        Debug.Log("Trigger Entered..." + this.name);
        if (other.gameObject.CompareTag("Player") && useTriggers)
        {
            Debug.Log("Teleporting Trigger Entered... to Scene > " + teleport_scene_name + " in " + loadTime + " seconds");
            isTeleporting = true;
            timer = 0;
        }
    }

    public void OnInteracted()
    {
      
        if (locked)
        {
            Debug.LogWarning("On Interacted.... With locked door");
         //   bool found = false;
        //    if (GameObject.Find(playerName).GetComponent<PlayerScript>()) 
         //   { 
         //       var items = GameObject.Find(playerName).GetComponent<PlayerScript>().items;
         //       for (var i = 0; i < items.Count; i++)
         //       {
         //           var item = items[i];
          //          if (item.name == key.name)
          //          {
           //             found = true;
          //              items.RemoveAt(i);
           //             break;
          //          }
          //      }
         //   }
         //   else
         //   {
         //       Debug.LogWarning("No Player Found , please check Naming....On Door Scene Loader Script");
        //    }


         //   if (!found)
         //   {
                //Debug.Log("Door is locked");
         //       Helper.Ccs.Clear();
         //       ((CsSubtitleAction)Helper.Ccs.QueueAction(new CsSubtitleAction("Locked"))).SetWaitForUserInput(true).SetAutoclear(true);
         //       Helper.Ccs.Play();
         //       return;
         //   }
        }



        Debug.Log("OnInteracted... " + this.name);
        if (useInteraction)
        {
            Debug.Log("Teleporting OnInteracted... to Scene > " + teleport_scene_name + " in " + loadTime + " seconds");
            isTeleporting = true;
            timer = 0;
        }
    }

    public void StartSceneLoad()
    {
        isTeleporting = true;
    }
}
