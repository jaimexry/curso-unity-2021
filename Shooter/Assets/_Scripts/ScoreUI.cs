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
        _text.text = "SCORE: " + ScoreManager.SharedInstance.Amount;
    }

    private void ReloadScore()
    {
        _text.text = "SCORE: " + ScoreManager.SharedInstance.Amount;
    }
}
