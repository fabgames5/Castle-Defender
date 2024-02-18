using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JBR_Torches : MonoBehaviour
{
    [Tooltip("The lightSource of this object")]
    [SerializeField]
    private Light lightSource;
    [Tooltip("All Particle systems this object has")]
    [SerializeField]
    private ParticleSystem[] particles;
    [Tooltip("Reference to the DayNightSystem")]
    [SerializeField]
    private JBR_DayNightSystem dayNightSystem;
    [Space(5)]
    [Tooltip("Dynamically Set, display if the light currentlt enabled ")]
    [SerializeField]
    private bool isLightEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        lightSource = GetComponentInChildren<Light>();
        particles = GetComponentsInChildren<ParticleSystem>();
        EnableLights(true);
        dayNightSystem = GameObject.Find("DayNightSystem").GetComponent<JBR_DayNightSystem>();
        RegisterLight();
    }

    //Call this function to enable the sceneLights ant Night
    public void EnableLights(bool isLightOn)
    {
        Debug.Log("Turn Light On (" + isLightOn+")");
        lightSource.enabled = isLightOn;
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].gameObject.SetActive(isLightOn);
        }

        isLightEnabled = lightSource.enabled;
    }

    private void RegisterLight()
    {
      dayNightSystem.RegisterScenePointLights(this);
    }
}
