using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokymonParty : MonoBehaviour
{
    [SerializeField] private List<Pokymon> pokymons;
    public const int NUM_MAX_POKYMON_IN_PARTY = 6;
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

    public bool AddPokymonToParty(Pokymon pokymon)
    {
        if (pokymons.Count < NUM_MAX_POKYMON_IN_PARTY)
        {
            pokymons.Add(pokymon);
            return true;
        }
        else
        {
            //TODO: Añadir la funcionalidad de enviar al PC
            return false;
        }
    }
}
