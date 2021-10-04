using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static WaveManager SharedInstance;

    private List<WaveSpawner> waves;
    public UnityEvent onWaveChanged;

    public int WavesCount
    {
        get => waves.Count;
    }

    private int currentWave;
    public int CurrentWave
    {
        get => currentWave;
    }

    private void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
            waves = new List<WaveSpawner>();
        }
        else
        {
            Destroy(gameObject);
        }

        currentWave = 1;
    }

    private void Start()
    {
        EnemyManager.SharedInstance.onEnemyChanged.AddListener(IncreaseCurrentWave);
    }

    private void IncreaseCurrentWave()
    {
        if (EnemyManager.SharedInstance.EnemyCount <= 0)
        {
            currentWave++;
        }
    }
    
    public void AddWave(WaveSpawner wave)
    {
        waves.Add(wave);
        onWaveChanged.Invoke();
    }

    public void RemoveWave(WaveSpawner wave)
    {
        waves.Remove(wave);
        onWaveChanged.Invoke();
    }
}
