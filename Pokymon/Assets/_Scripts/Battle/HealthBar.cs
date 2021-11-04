using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBar;
    private Image _healthBarImage;
    
    public Color BarColor(float finalScale)
    {
        if (finalScale < 0.15f)
        {
            return  new Color(0.7843137f, 0, 0.01960784f);
        }else if (finalScale < 0.5f)
        {
            return new Color(0.9411765f, 0.7450981f, 0.09803922f);
        }
        else
        {
            return new Color(0, 0.7830189f, 0.06987349f);
        }
    }

    private void Awake()
    {
        _healthBarImage = healthBar.GetComponent<Image>();
    }

    /// <summary>
    /// Actualiza la barra de vida a partir del valor normalizado de la misma
    /// </summary>
    /// <param name="normalizedValue">Valor de la vida normalizado entre 0 y 1</param>
    public void SetHp(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1.0f);
        _healthBarImage.color = BarColor(normalizedValue);
    }

    public IEnumerator SetSmoothHp(float normalizedValue)
    {
        var seq = DOTween.Sequence();
        seq.Append(healthBar.transform.DOScaleX(normalizedValue, 1f));
        seq.Join(_healthBarImage.DOColor(BarColor(normalizedValue), 1f));
        yield return seq.WaitForCompletion();
    }
}
