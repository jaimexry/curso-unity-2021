using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LifeBar : MonoBehaviour
{
    [Tooltip("Vida objetivo que se reflejará en la barra")]
    public Life targetLife;

    private Image _lifeBar;

    private void Awake()
    {
        _lifeBar = GetComponent<Image>();
    }

    private void Update()
    {
        _lifeBar.fillAmount = targetLife.Amount / targetLife.MaxAmount;
    }
}
