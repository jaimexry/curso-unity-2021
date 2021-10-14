using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioMixerSnapshot pauseSnp, gameSnp;
    
    private void Awake()
    {
        pauseMenu.SetActive(false);
    }

    private void Start()
    {
        gameSnp.TransitionTo(0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            pauseSnp.TransitionTo(0.1f);
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameSnp.TransitionTo(0.1f);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
