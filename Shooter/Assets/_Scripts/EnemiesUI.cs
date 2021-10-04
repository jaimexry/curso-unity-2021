using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemiesUI : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        EnemyManager.SharedInstance.onEnemyChanged.AddListener(ReloadEnemies);
    }

    private void ReloadEnemies()
    {
        _text.text = "ENEMIES REMAINING: " + EnemyManager.SharedInstance.EnemyCount;
    }
}
