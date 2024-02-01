using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class JBR_MoonPhase : MonoBehaviour {

    [Header("Moon Phases ,  the length of the lunar cycle is relative to how many sprites you use , in this case it would be 24 days")]

    [Tooltip("Sprites of all the moon phases")]
    public Sprite[] moonSprites;
    [HideInInspector]
    public int curMoonSprite;
    [HideInInspector]
    public int arraySize;
    [HideInInspector]
    public bool changedSprite;
    [Tooltip("Drag & Drop the DayNightSystem GameObject here")]
    public GameObject day_Night_GO;
    [HideInInspector]
    public JBR_DayNightSystem dayNightSystem;
    [HideInInspector]
    public SpriteRenderer moonRenderer;
    [Tooltip("a string of all the important moon phases")]
    public string[] moonPhases;
    [Tooltip("Drag & Drop UI display of the current moon phase here")]
    public Text MoonPhaseText;



    // Use this for initialization
    void Start () {
        dayNightSystem = day_Night_GO.GetComponent<JBR_DayNightSystem>();
        moonRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        InvokeRepeating("ChangeSprite", 1.0f, .25f);
        arraySize = moonSprites.Length;//make a note of the size of the moon sprites
        if (curMoonSprite == 0)
        {
            MoonPhaseText.text = ("Moon : " + moonPhases[1]);//waxing cresant
        }
    }
	
    //change moon sprite every game day
    void ChangeSprite()
    {
        //check if its the middle of the day so the moon change isnt noticable
        if(dayNightSystem.currentTime > 12 && dayNightSystem.currentTime < 15 )
        {
            if (changedSprite == false) 
            {
                curMoonSprite += 1;
                //we reached the last moon sprite so now we start over
                if (curMoonSprite >= arraySize)
                {
                    curMoonSprite = 0;
                }
                moonRenderer.sprite = moonSprites[curMoonSprite];// sets the correct sprite


// this is used so the player can see the name of a specific stage of the moon
                if(curMoonSprite == 23)
                {
                    MoonPhaseText.text =("Moon : " + moonPhases[0]);//new moon
                }
                if (curMoonSprite == 0)
                {
                    MoonPhaseText.text = ("Moon : " + moonPhases[1]);//waxing cresant
                }
                if (curMoonSprite == 6)
                {
                    MoonPhaseText.text = ("Moon : " + moonPhases[2]);//first quarter
                }
                if (curMoonSprite == 7)
                {
                    MoonPhaseText.text = ("Moon : " + moonPhases[3]);//waxing gibbous
                }
                if (curMoonSprite == 12)
                {
                    MoonPhaseText.text = ("Moon : " + moonPhases[4]);//full moon
                }
                if (curMoonSprite == 13)
                {
                    MoonPhaseText.text = ("Moon : " + moonPhases[5]);//wanning gibbous
                }
                if (curMoonSprite == 18)
                {
                    MoonPhaseText.text = ("Moon : " + moonPhases[6]);//third quarter
                }
                if (curMoonSprite == 19)
                {
                    MoonPhaseText.text = ("Moon : " + moonPhases[7]);// wanning cresant
                }
                changedSprite = true;
            }
        }else
        {
            if (changedSprite)
            {
                changedSprite = false;
            }
        }

        // always look at camera
        this.transform.LookAt(dayNightSystem.cameraToFollow.transform);

        // Debug.Log(this.transform.eulerAngles + "  Moon Angles");
        if (this.transform.eulerAngles.y > 5 || this.transform.eulerAngles.y < -5)
        {
            moonRenderer.flipX = true;
        }
        else
        {
            moonRenderer.flipX = false;
        }     
    }


}
