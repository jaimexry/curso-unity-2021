using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WavesUI : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        WaveManager.SharedInstance.onWaveChanged.AddListener(ReloadWaves);
    }

    private void ReloadWaves()
    {
        _text.text = "WAVE: " + WaveManager.SharedInstance.CurrentWave;
    }
}
