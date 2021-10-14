using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour
{
    private float amount;
    
    [SerializeField]    
    private float maxAmount;
    public UnityEvent onDeath;

    public float Amount
    {
        get => amount;
        set
        {
            amount = value;
            if (amount <= 0)
            {
                onDeath.Invoke();
            }
        } 
    }

    public float MaxAmount
    {
        get => maxAmount;
        set
        {
            maxAmount = value;
            if (maxAmount < amount)
            {
                maxAmount = amount;
            }
        } 
    }

    private void Awake()
    {
        amount = maxAmount;
    }
}
