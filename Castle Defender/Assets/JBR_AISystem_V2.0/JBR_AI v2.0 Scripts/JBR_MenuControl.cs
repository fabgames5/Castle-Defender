using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class JBR_MenuControl : MonoBehaviour
{
    public GameObject quitPanel;
    public Text TitleText;
    public bool gamePaused = false;

    public string[] Titles = {"Gameover", "Paused" };
    // Start is called before the first frame update
    void Start()
    {
        quitPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = CursorLockMode.Confined;
            TitleText.text = Titles[1];
            ResumeGame();
        }
    }

    /// <summary>
    /// Quits Application
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }


    /// <summary>
    /// Restarts the Current Scene
    /// </summary>
    public void RestartScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }


    public void ResumeGame()
    {
        if (gamePaused)
        {
            Debug.Log("UnPause game");         
            gamePaused = false;
            quitPanel.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            quitPanel.SetActive(true);
            gamePaused = true;
            Time.timeScale = 0.001f;
            Cursor.lockState = CursorLockMode.Confined;

        }      
    }

    public void ShowPanel()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Confined;
        quitPanel.SetActive(true);
    }
}
