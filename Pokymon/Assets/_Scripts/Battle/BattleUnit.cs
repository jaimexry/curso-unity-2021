using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    public PokymonBase _base;
    public int _level;

    public bool isPlayer;
    public Pokymon Pokymon { get; set; }
    
    public void SetupPokymon()
    {
        Pokymon = new Pokymon(_base, _level);

        GetComponent<Image>().sprite = (isPlayer ? Pokymon.Base.BackSprite : Pokymon.Base.FrontSprite);
    }
}
