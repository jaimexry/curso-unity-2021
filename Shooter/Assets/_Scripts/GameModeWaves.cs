using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameModeWaves : MonoBehaviour
{
    [SerializeField]
    private Life playerLife;
    [SerializeField]
    private Life baseLife;
    
    private void Start()
    {
        playerLife.onDeath.AddListener(CheckLoseCondition);
        baseLife.onDeath.AddListener(CheckLoseCondition);
        
        EnemyManager.SharedInstance.onEnemyChanged.AddListener(CheckWinCondition);
        WaveManager.SharedInstance.onWaveChanged.AddListener(CheckWinCondition);
    }

    void CheckLoseCondition()
    {
        RegisterTime();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("LoseScene");

    }

    void CheckWinCondition()
    {
        //GANAR
        if (EnemyManager.SharedInstance.EnemyCount <= 0 && WaveManager.SharedInstance.WavesCount <= 0)
        {
            RegisterTime();
            RegisterScore();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("WinScene");
        }
    }

    void RegisterScore()
    {
        var actualScore = ScoreManager.SharedInstance.Amount;
        PlayerPrefs.SetInt("Last Score", actualScore);

        var highScore = PlayerPrefs.GetInt("High Score", 0);

        if (actualScore > highScore)
        {
            PlayerPrefs.SetInt("High Score", actualScore);
        }
    }
    
    void RegisterTime()
    {
        var actualTime = Time.time;
        PlayerPrefs.SetFloat("Last Time", actualTime);

        var lowTime = PlayerPrefs.GetFloat("Low Time", 99999999.0f);

        if (actualTime < lowTime)
        {
            PlayerPrefs.SetFloat("Low Time", actualTime);
        }
    }
}
