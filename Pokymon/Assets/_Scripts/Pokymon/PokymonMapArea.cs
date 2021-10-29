using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokymonMapArea : MonoBehaviour
{
    [SerializeField] private List<Pokymon> wildPokymons;

    public Pokymon GetRandomWildPokymon()
    {
        var pokymon = wildPokymons[Random.Range(0, wildPokymons.Count)];
        pokymon.InitPokymon();
        return pokymon;
    }
}
