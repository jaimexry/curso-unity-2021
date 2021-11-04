using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokymon/Nuevo Movimiento")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private int id;
    public int ID => id;
    
    [SerializeField] private string _name;
    public string Name => _name;
    
    [TextArea] [SerializeField] private string description;
    public string Description => description;
    
    [SerializeField] private PokymonType type;
    public PokymonType Type => type;
    
    [SerializeField] private int power;
    public int Power => power;

    [SerializeField] private int fixedPower;
    public int FixedPower => fixedPower;
    
    [SerializeField] private int accuracy;
    public int Accuracy => accuracy;

    [SerializeField] private int pp;
    public int PP => pp;
    
    [SerializeField] private MoveCategory category;
    public MoveCategory Category => category;
}

public enum MoveCategory
{
    Physical,
    Special,
    Status
}
