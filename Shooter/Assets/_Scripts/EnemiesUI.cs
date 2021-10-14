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
        ReloadEnemies();
    }

    private void ReloadEnemies()
    {
        _text.text = string.Format("ENEMIES REMAINING: {0}", EnemyManager.SharedInstance.EnemyCount);
    }
}
