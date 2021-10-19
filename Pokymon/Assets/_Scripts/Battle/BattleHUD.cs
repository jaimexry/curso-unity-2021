using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleHUD : MonoBehaviour
{
    public Text pokymonName;
    public Text pokymonLevel;
    public HealthBar healthBar;
    public Text pokymonHealt;
    
    public void SetPokymonData(Pokymon pokymon)
    {
        pokymonName.text = pokymon.Base.Name;
        pokymonLevel.text = string.Format("Lv. {0}", pokymon.Level);
        healthBar.SetHp(pokymon.Hp / pokymon.MaxHp);
        pokymonHealt.text = string.Format("{0}/{1}", pokymon.Hp, pokymon.MaxHp);
    }
}
