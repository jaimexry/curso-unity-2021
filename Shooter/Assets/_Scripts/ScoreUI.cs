using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        ScoreManager.SharedInstance.onScoreChanged.AddListener(ReloadScore);
        ReloadScore();
    }

    private void ReloadScore()
    {
        _text.text = string.Format("SCORE: {0}", ScoreManager.SharedInstance.Amount);
    }
}
