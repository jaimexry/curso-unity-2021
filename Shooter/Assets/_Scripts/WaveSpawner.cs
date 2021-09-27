using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Tooltip("Prefab de Enemigo a generar")]
    public GameObject prefab;
    [Tooltip("Tiempo en el que se inicia y finaliza la oleada")]
    public float startTime, endTime;
    [Tooltip("Tiempo entre la generación de Enemigos")]
    public float spawnRate;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", startTime, spawnRate);
        Invoke("CancelInvoke", endTime);
    }

    void SpawnEnemy()
    {
        Instantiate(prefab, transform.position, transform.rotation);
    }
}
