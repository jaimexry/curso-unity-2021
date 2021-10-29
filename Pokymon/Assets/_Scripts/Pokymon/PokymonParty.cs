using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokymonParty : MonoBehaviour
{
    [SerializeField] private List<Pokymon> pokymons;
    public List<Pokymon> Pokymons => pokymons;

    private void Start()
    {
        foreach (var pokymon in pokymons)
        {
            pokymon.InitPokymon();
        }
    }

    public Pokymon GetFirstHealthyPokymon()
    {
        return pokymons.Where(x => x.Hp > 0).FirstOrDefault();
    }
}
