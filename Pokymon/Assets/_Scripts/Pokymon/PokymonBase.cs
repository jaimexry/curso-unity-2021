using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokymon", menuName = "Pokymon/Nuevo Pokymon")]
public class PokymonBase : ScriptableObject
{
    [SerializeField] private int id;
    public int ID => id;

    [SerializeField] private string _name;
    public string Name => _name;

    [TextArea] [SerializeField] private string description;
    public string Description => description;

    [SerializeField] private Sprite frontSprite;
    public Sprite FrontSprite => frontSprite;

    [SerializeField] private Sprite backSprite;
    public Sprite BackSprite => backSprite;
    
    [SerializeField] private PokymonType type1;
    public PokymonType Type1 => type1;
    
    [SerializeField] private PokymonType type2;
    public PokymonType Type2 => type2;
    
    [SerializeField] private int maxHp;
    public int MaxHp => maxHp;
    
    [SerializeField] private int attack;
    public int Attack => attack;

    [SerializeField] private int defense;
    public int Defense => defense;
    
    [SerializeField] private int spAttack;
    public int SpAttack => spAttack;
    
    [SerializeField] private int spDefense;
    public int SpDefense => spDefense;
    
    [SerializeField] private int speed;
    public int Speed => speed;

    [SerializeField] private List<LearnableMove> learnableMoves;
    public List<LearnableMove> LearnableMoves => learnableMoves;
}

public enum PokymonType
{
    None,
    Normal,
    Fuego,
    Agua,
    Electrico,
    Planta,
    Lucha,
    Hielo,
    Veneno,
    Tierra,
    Volador,
    Psiquico,
    Roca,
    Bicho,
    Fantasma,
    Dragon,
    Siniestro,
    Hada,
    Acero
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase move;
    public MoveBase Move => move;
    
    [SerializeField] private int level;
    public int Level => level;
}
