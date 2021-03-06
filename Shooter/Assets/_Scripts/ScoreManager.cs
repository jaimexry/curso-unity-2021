using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager SharedInstance;
    
    [SerializeField]
    [Tooltip("Cantidad de Puntos de la Partida Actual")]
    private int amount;

    public UnityEvent onScoreChanged;
    
    public int Amount
    {
        get => amount;
        set
        {
            amount = value;
            onScoreChanged.Invoke();
        }
    }

    private void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            Debug.LogWarning("ScoreManager duplicado; debe ser destruido");
            Destroy(gameObject);
        }
    }
}
