﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{

    /*
     * Tutorial used: https://youtu.be/JivuXdrIHK0
     */
    public static bool GameIsCurrentlyPaused = false;
    public Button pauseButton;
    [SerializeField] GameObject pauseMenuUI;

    private String sceneToLoad = "MenuScene"; //I just put in a fake Menu Scene to test
    // Scene to load can be changed later 

    void Start () {
        Button btn = pauseButton.GetComponent<Button>();
        btn.onClick.AddListener(Pause);
    }
    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameIsCurrentlyPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }*/
    }
    public void Resume() //public to be able to call it from the button
    {
        pauseMenuUI.SetActive(false); //disable Pause Menu (Child of the Canvas this script is linked to 
        Time.timeScale = 1f; // normal time
        GameIsCurrentlyPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true); //enable Pause Menu (Child of the Canvas this script is linked to 
        Time.timeScale = 0f; // Stop time
        GameIsCurrentlyPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; // normal time
        SceneManager.LoadScene(sceneToLoad); //load any scene (in this case menu scene)
    }
    
    public void QuitGame()
    {
        Debug.Log("Quit"); //Application.Quit does nothing visible, so I left the Debug.Log statement
        Application.Quit();
    }
}