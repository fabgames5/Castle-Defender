using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class JBR_DayNightSystem : MonoBehaviour {
    //Speed of the cycle (if you set this to 1 then one hour in the cycle will pass in 1 real life second, set to .1 , one game hour passes in 6secs, .016667 one game hour to 60 seconds ,(24mins = one game day)
    //0.004166667 = 96 minutes = 1 game day
    [Tooltip(".016667 one game hour to 60 seconds ,(24mins = one game day)")]
    public float daySpeedMultiplier = 0.1f;
    //control intensity of sun?
    [Tooltip("Check if you want the Sun Light Intesity controlled by the Day & Night System")]
    [HideInInspector]
    public bool controlIntensity = true;
    //how quickly sky objects are updated, .1f = approximately 10 updates per second 
    [Tooltip("Used to save a bit on performance , how quickly sky objects are updated, .1f = approximately 10 updates per second")]
    public float skyUpdated = 0.1f;
    //skybox (Dynamic skybox)
    public Material skyBox;
    //Color of skybox , used to give a more exaggerated sunset & sunrise
    [Tooltip("Color the sky turns during sunrise and sunset")]
    public Color colorStart = Color.red;
    [Tooltip("Color the sky turns during Day")]
    public Color colorEnd = Color.cyan;
    //duration of color change
    [Tooltip("Time it takes for sky colors to change")]
    public float duration = 1.0F;
    [HideInInspector]
    public float step = 0;
    //UI display of  current time
    [Tooltip("Drag & Drop UI display of current time here")]
       public Text timeDisplay;
    //Ui display of Season
    [Tooltip("Drag & Drop UI display of Seasons here")]
    public Text seasonDisplay;

    //main directional light
    [Tooltip("Drag & Drop main directional light here")]
    public Light sunLight;
    //Intensity of Light
    [HideInInspector]
    public float lightIntensity;

    //what time this cycle should start
    [HideInInspector]
    public float startTime = 12.0f;
    [HideInInspector]
    public bool nextDay = false;
	//what's the current time
    [HideInInspector]
	public float currentTime = 0.0f;
    [Tooltip("Set Automatically, the current time in the game")]
    public string timeString = "00:00 AM";
    [HideInInspector]
    public string AMPM = "";
    //the current day
    [HideInInspector]
    public int curDay = 1;
    [HideInInspector]
    public float minutes;
    [Tooltip("How Many days in a year in your game world")]
    public int daysInYear = 365;
    [HideInInspector]
    public int curYear = 1;
    [Tooltip("A list of all the seasonal days")]
    public string[] seasonNames;

    public string curSeason = "";

    //rotation of sun
    [HideInInspector]
    public float xValueOfSun = 90.0f;
    [HideInInspector]
    public Vector3 degsOfSun;
    //Clouds
    [Tooltip("Drag & Drop Cloud spheres here , you can dupicate the spheres to add more clouds to the sky if needed")]
    [SerializeField]	public Transform[] cloudSpheres;
    //Rotation speed of clouds, add multiple clouds at different speeds
    [Tooltip("Set rotation speed of each cloud sphere, having different speeds makes clouds move past other clouds for a more realistic look")]
    public float[] cloudRotationSpeed;


//future Update for weather system
    //transparency of cloud *** used to fade clouds in and out of days
    [HideInInspector]
    public float[] cloudTrans;
    // used to determine if clouds are fading in or out
    [HideInInspector]
    public bool[] cloudTransDir;

    public RenderSettings renderSettings;

    // Star spheres
    [Tooltip("Drag & Drop Star spheres here , you can dupicate the spheres to add more stars to the sky if needed")]
    [SerializeField]	public Transform[] starSpheres;
	//star's rotation speed
	public float starRotationSpeed = 0.15f;
    //camera to follow
    [Tooltip("Drag & Dropthe Gameobject you want the system to follow here, example > Player , or Camera")]
    public GameObject cameraToFollow;
    [HideInInspector]
    public float sec;
    [HideInInspector]
    public float minute;
    [HideInInspector]
    public int hours;
    [HideInInspector]
    public int days;
    [HideInInspector]
    public int years;


    void Awake()
    {
    //    currentTime = (NetworkTime.offset + Time.time)/ daySpeedMultiplier;
    //     sec = (int)(currentTime % 60);
    //     minute = (int)((currentTime / 60) % 60);
    //     hours = (int)((currentTime / 3600) % 24);
    //     days = (int)(currentTime / 86400); // There are 86400 seconds in a day (60*60*24)
    
    }
 	// Use this for initialization
	void Start () {
	//set the start time to something specific, only good for use in a single player game
		currentTime = startTime;
        Invoke("TimeCheck", 2);
       
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void DelayedStart()
    {
        // make sure there is a rotation speed for every cloud sphere
        if (cloudSpheres.Length != cloudRotationSpeed.Length)
        {
            Debug.LogError("Need to add a cloudRotationSpeed for every cloudSphere in the scene or clouds wont move , << DayNightController >>");
            cloudRotationSpeed = new float[cloudSpheres.Length];

        }
        // make sure there is a cloud tranparency for every cloud sphere
        if (cloudSpheres.Length != cloudTrans.Length)
        {
            Debug.LogError("Need to add a cloudTrans for every cloudSphere in the scene or clouds wont move , << DayNightController >>");
            cloudTrans = new float[cloudSpheres.Length];

        }
        CalculateSeason();
        //invoking most aspects so that it uses minimal CPU resources (only noticable if time is set very fast)
        if (starSpheres.Length > 0)
        {
            InvokeRepeating("StarSphere", skyUpdated, skyUpdated);
        }
        if (cloudSpheres.Length > 0)
        {
            InvokeRepeating("ControlClouds", skyUpdated, skyUpdated);
        }
        if (sunLight)
        {
            InvokeRepeating("ControlLight", skyUpdated, skyUpdated);
        }

        InvokeRepeating("CalculateTime", skyUpdated, skyUpdated);
    }
	// Update is called once per frame
	void Update () {
        //only start the time if it has been set
        if (currentTime > 0)
        {
            //increment time
            currentTime += (Time.deltaTime) * daySpeedMultiplier;
            //reset time
            if (currentTime >= 24.0f)
            {
                currentTime %= 24.0f;
            }
        }

        RenderSettings.skybox = null;
        RenderSettings.ambientLight = Color.black;
    }

	void ControlLight() {
        //	if (currentTime >= 17.0f || currentTime <= 6.5f) {

		//Rotate light
		xValueOfSun = -(90.0f+currentTime*15.0f);
        sunLight.transform.position = cameraToFollow.transform.position;
		sunLight.transform.eulerAngles = new Vector3(xValueOfSun,0,0);
	//	sunLight.transform.eulerAngles = sunLight.transform.right  *xValueOfSun;
		degsOfSun = sunLight.transform.eulerAngles;
		//reset angle
		if (xValueOfSun >= 360.0f) {
			xValueOfSun = 0.0f;
		}
		//This basically turn on and off the sun light based on day / night
		if (controlIntensity && sunLight && (currentTime >= 18.0f || currentTime <= 4.5f)) {
			lightIntensity = Mathf.MoveTowards(sunLight.intensity,0.0f,Time.deltaTime*daySpeedMultiplier * 4);
          
            }
            else if (controlIntensity && sunLight) {
            lightIntensity = Mathf.MoveTowards(sunLight.intensity, 1.0f, Time.deltaTime * daySpeedMultiplier* 4);
        }
		sunLight.intensity = lightIntensity;

        
       

// changes skybox color, used during sunrise and sunset
        if (lightIntensity == 0 || lightIntensity == 1)
        {
            step = 0;
        }
        if (lightIntensity != 0)
        {
            step += (Time.deltaTime / duration);
            skyBox.SetColor("_SkyTint", Color.Lerp(colorEnd, colorStart, step));
        }
        if (lightIntensity != 1)
        {
            step += (Time.deltaTime / duration);
            skyBox.SetColor("_SkyTint", Color.Lerp(colorStart, colorEnd, step));
        }
    }

	void ControlClouds (){
        //Rotate clouds
 //       Debug.Log("" + cloudSpheres.Length.ToString());

        for (int i = 0; i < cloudSpheres.Length; i++) { 
            if (cloudSpheres[i])
            {
                //moves stars to follow player
               cloudSpheres[i].transform.position = cameraToFollow.transform.position;
                cloudSpheres[i].transform.Rotate(Vector3.forward * cloudRotationSpeed[i] * daySpeedMultiplier * Time.deltaTime);
            }
                    if (lightIntensity >= .25f)
                    {
                        float cloudEmission = (lightIntensity);
                        Color newColor = new Color(cloudEmission, cloudEmission, cloudEmission, 1);
                        cloudSpheres[i].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", newColor);          //Color(0,0,0,lightIntensity));
                    }
                }	
	}

	void StarSphere() {
		//Get the color of the stars
		Color currentColor;
		//Rotate and eneble and disable stars
		foreach (Transform stars in starSpheres) {
			if (stars){
               
           //     if (cameraToFollow.transform.rotation.x >=25 || cameraToFollow.transform.rotation.x <= 90)
           //     {
           //         stars.transform.position = cameraToFollow.transform.position;
          //      }
                //rotate Starts
                stars.transform.Rotate(Vector3.forward*starRotationSpeed*daySpeedMultiplier*Time.deltaTime);

                if (currentTime > 4.5f && currentTime < 17.0f)
                {
                    currentColor = stars.GetComponent<Renderer>().material.color;
                    stars.GetComponent<Renderer>().material.color = new Color(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currentColor.a, 0.0f, Time.deltaTime * 100.0f * daySpeedMultiplier));
                }
                else
                {
                   currentColor = stars.GetComponent<Renderer>().material.color;
                   stars.GetComponent<Renderer>().material.color = new Color(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(0.0f, currentColor.a, Time.deltaTime * 150.0f * daySpeedMultiplier));
                }
                //fade stars emission in and out as light intensity changes
                    float starEmission = (1 - lightIntensity);
                    Color newColor = new Color(starEmission, starEmission, starEmission, starEmission);
                    stars.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", newColor);  //Color(0,0,0,lightIntensity));
                //chagning Matallic increases the darkness of the night sky
                    stars.GetComponent<MeshRenderer>().material.SetFloat("_Metallic", starEmission);
                stars.transform.Translate(Time.deltaTime, Time.deltaTime, Time.deltaTime, cameraToFollow.transform);
            }
		}
	}

	
	void CalculateTime (){
		//Is it am of pm?
		 AMPM = "";
		 minutes = ((currentTime) - (Mathf.Floor(currentTime)))*60.0f;
		if (currentTime <= 12.0f) {
			AMPM = "AM";
			if (nextDay == true) {
				curDay += 1;
                CalculateSeason();
                if (curDay == daysInYear + 1)
                {
                    curDay = 1;
                    curYear += 1;
                }
				nextDay = false;
			}

		} else {
			AMPM = "PM";
			nextDay = true;
		}
		//Make the final string
		timeString = Mathf.Floor(currentTime).ToString() + " : " + minutes.ToString("F0") + " "+AMPM ;
        timeDisplay.text = ("Time : " + timeString + "  Day :" + curDay.ToString() + "  Year : "+ curYear.ToString() );
        seasonDisplay.text = ("  Season : " + curSeason);
    }

    void CalculateSeason()
    {
        Debug.Log("Calculate season");

        if (curDay == daysInYear)
        {
            Debug.Log("WinterSolstice");
            curSeason = seasonNames[1];//winter Solstice
        }
        if (curDay == 1)
        {
            Debug.Log("New Years");
            curSeason = seasonNames[6];//new years
        }
        if (curDay == 2)
        {
            Debug.Log("Winter");
           curSeason = seasonNames[0];// winter
        }
        if (curDay == Mathf.RoundToInt(daysInYear * .125f)) //spring starts
        {
            Debug.Log("Spring Starts");
            curSeason = seasonNames[2];
        }
        if (curDay == Mathf.RoundToInt(daysInYear * .375f)) // summer starts
        {
            Debug.Log("Summer Starts");
            curSeason = seasonNames[3];
        }
        if (curDay == Mathf.RoundToInt(daysInYear * .5f))  // Summer Solstice
        {
            Debug.Log("Summer Solstice");
            curSeason = seasonNames[4];
        }      
        if (curDay == Mathf.RoundToInt((daysInYear * .5f) + 1)) //Summer
        {
            Debug.Log("Summer");
            curSeason = seasonNames[3];
        }
        if (curDay == Mathf.RoundToInt(daysInYear * .625f)) //fall starts
        {
            Debug.Log("Fall Starts");
            curSeason = seasonNames[5];
        }       
        if (curDay == Mathf.RoundToInt(daysInYear * .875f)) //winter starts
        {
            Debug.Log("Winter Starts");
            curSeason = seasonNames[0];
        }
    }
    void TimeCheck()
    {
        // set the current time off of uMMORPG time..
        float curTime = (  Time.time) *( daySpeedMultiplier *60); //NetworkTime.offset +

        minute = (int)(curTime % 60);
        hours = (int)((curTime / 60) % 24);
        days = (int)(curTime / 1440); // There are 1440 minutes in a day (60*60*24)
        years = (int)(curTime /(1440 * daysInYear));
        if(days >= 1)
        {
            curDay += days;
        }
        if(years >= 1)
        {
            curYear += years;
        }
        float newMinute = minute;
        currentTime = (hours + (float)(newMinute * .01));

        //now that a time is set start the movement process
        DelayedStart();
    }


    /// <summary>
    /// Changes RenderSettings Based off Current Time of Day
    /// </summary>
    void AdjustRenderSettings()
    {


        RenderSettings.skybox = null;
        RenderSettings.ambientLight = Color.black;
    }

}
