using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class BattleHUD : MonoBehaviour
{
    public Text pokymonName;
    public Text pokymonLevel;
    public HealthBar healthBar;
    public Text pokymonHealth;
    private Pokymon _pokymon;
    public GameObject expBar;
    public float lastHp;

    public void SetPokymonData(Pokymon pokymon)
    {
        _pokymon = pokymon;
        pokymonName.text = pokymon.Base.Name;
        SetLevelText();
        pokymonHealth.text = string.Format("{0}/{1}", _pokymon.Hp, _pokymon.MaxHp);
        healthBar.SetHp((float)_pokymon.Hp / _pokymon.MaxHp);
        SetExp();
    }

    public void UpdatePokemonData()
    {
        StartCoroutine(healthBar.SetSmoothHp(_pokymon.Hp / (float)_pokymon.MaxHp));
        StartCoroutine(SetSmoothHpPoints());
    }

    public IEnumerator SetSmoothHpPoints()
    {
        float quantity = lastHp - _pokymon.Hp;
        while (lastHp > _pokymon.Hp)
        {
            lastHp -= quantity * Time.deltaTime;
            if (lastHp < _pokymon.Hp)
            {
                lastHp = _pokymon.Hp;
            }
            pokymonHealth.text = string.Format("{0}/{1}", (int)lastHp, _pokymon.MaxHp);
            yield return null;
        }
        pokymonHealth.text = string.Format("{0}/{1}", (int)lastHp, _pokymon.MaxHp);
    }
    
    public void SetExp()
    {
        if (expBar == null)
        {
            return;
        }

        expBar.transform.localScale = new Vector3(NormalizedExp(), 1, 1);
    }

    public IEnumerator SetSmoothExp(bool needsToResetBar = false)
    {
        if (expBar == null)
        {
            yield break;
        }

        if (needsToResetBar)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }
        yield return expBar.transform.DOScaleX(NormalizedExp(), 1.5f).WaitForCompletion();
    }

    private float NormalizedExp()
    {
        int currentLevelExp = _pokymon.Base.GetNecessaryExpForLevel(_pokymon.Level);
        int nextLevelExp = _pokymon.Base.GetNecessaryExpForLevel(_pokymon.Level + 1);
        float normalizedExp = (float)(_pokymon.Experience - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public void SetLevelText()
    {
        pokymonLevel.text = string.Format("Lv. {0}", _pokymon.Level);
    }
}
