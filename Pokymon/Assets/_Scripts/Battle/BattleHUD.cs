using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleHUD : MonoBehaviour
{
    public Text pokymonName;
    public Text pokymonLevel;
    public HealthBar healthBar;
    public Text pokymonHealth;
    private Pokymon _pokymon;
    public float lastHp;
    
    public void SetPokymonData(Pokymon pokymon)
    {
        _pokymon = pokymon;
        pokymonName.text = pokymon.Base.Name;
        pokymonLevel.text = string.Format("Lv. {0}", pokymon.Level);
        pokymonHealth.text = string.Format("{0}/{1}", _pokymon.Hp, _pokymon.MaxHp);
        healthBar.SetHp((float)_pokymon.Hp / _pokymon.MaxHp);        
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
    }
}
