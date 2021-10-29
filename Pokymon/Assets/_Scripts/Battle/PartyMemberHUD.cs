using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{
    public Text nameText, lvlText, typeText, hpText;
    public HealthBar healthBar;
    public Image pokemonImage;
    [SerializeField] private Color selectedColor = Color.blue;
    private Color normalColor;
    private Image borderImage;

    private Pokymon _pokymon;

    private void Awake()
    {
        borderImage = GetComponent<Image>();
        normalColor = borderImage.color;
    }

    public void SetPokymonData(Pokymon pokymon)
    {
        _pokymon = pokymon;

        nameText.text = _pokymon.Base.Name;
        lvlText.text = string.Format("Lv. {0}", _pokymon.Level);
        typeText.text = (_pokymon.Base.Type2 == 0 ? _pokymon.Base.Type1.ToString() : string.Format("{0}/{1}", _pokymon.Base.Type1.ToString(), _pokymon.Base.Type2.ToString()));
        hpText.text = string.Format("{0} / {1}", _pokymon.Hp, _pokymon.MaxHp);
        healthBar.SetHp((float)_pokymon.Hp / _pokymon.MaxHp);
        pokemonImage.sprite = _pokymon.Base.FrontSprite;
    }

    public void SetSelectedPokymon(bool selected)
    {
        if (selected)
        {
            borderImage.color = selectedColor;
        }
        else
        {
            borderImage.color = normalColor;
        }
    }
}
