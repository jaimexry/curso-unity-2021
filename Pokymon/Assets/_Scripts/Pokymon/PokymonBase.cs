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

    [SerializeField] private int expBase;
    public int ExpBase => expBase;
    
    [SerializeField] private int catchRate = 255;
    public int CatchRate => catchRate;

    [SerializeField] private GrowthRate growthRate;
    public GrowthRate GrowthRate => growthRate;
    
    [SerializeField] private List<LearnableMove> learnableMoves;
    public List<LearnableMove> LearnableMoves => learnableMoves;

    public static int NUMBER_OF_LEARNABLE_MOVES { get; } = 4;

    public int GetNecessaryExpForLevel(int level)
    {
        switch (GrowthRate)
        {
            case GrowthRate.Fast:
                return Mathf.FloorToInt(4 * Mathf.Pow(level, 3) / 5);
                break;
            case GrowthRate.MediumFast:
                return Mathf.FloorToInt(Mathf.Pow(level, 3));
                break;
            case GrowthRate.MediumSlow:
                return Mathf.FloorToInt((6 * Mathf.Pow(level, 3) / 5) - (15 * Mathf.Pow(level, 2)) + (100 * level) - 140);
                break;
            case GrowthRate.Slow:
                return Mathf.FloorToInt(5 * Mathf.Pow(level, 3) / 4);
                break;
            case GrowthRate.Erratic:
                if (level < 50)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (100 - level) / 50);
                }else if (level < 68)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (150 - level) / 100);
                }else if (level < 98)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * Mathf.FloorToInt((1911 - 10 * level) / (float)3) / 500);
                }
                else
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (160 - level) / 100);
                }
                break;
            case GrowthRate.Fluctuating:
                if (level < 15)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.FloorToInt((level + 1) / (float)3) + 24) / (float)50));
                }else if (level < 36)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((level + 14) / (float)50));
                }
                else
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.FloorToInt(level / (float)2) + 32) / (float)50));
                } 
                break;
        }

        return -1;
    }
}

public enum GrowthRate
{
    Erratic,
    Fast,
    MediumFast,
    MediumSlow,
    Slow,
    Fluctuating
}

public enum PokymonType
{
    None,
    Normal,
    Fuego,
    Agua,
    Electrico,
    Planta,
    Hielo,
    Lucha,
    Veneno,
    Tierra,
    Volador,
    Psiquico,
    Bicho,
    Roca,
    Fantasma,
    Dragon,
    Siniestro,
    Acero,
    Hada
}

public class TypeMatrix
{
    private static float[][] matrix =
    {
        new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0f, 1f, 1f, 0.5f, 1f},
        new float[] {1f, 0.5f, 0.5f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 1f, 2f, 1f},
        new float[] {1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f, 1f},
        new float[] {1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f},
        new float[] {1f, 0.5f, 2f, 1f, 0.5f, 1f, 1f, 0.5f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 0.5f, 1f, 0.5f, 1f},
        new float[] {1f, 0.5f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f},
        new float[] {2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 0.5f, 0.5f, 2f, 0f, 1f, 2f, 2f, 0.5f},
        new float[] {1f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 0f, 2f},
        new float[] {1f, 2f, 1f, 2f, 0.5f, 1f, 1f, 2f, 1f, 0f, 1f, 0.5f, 2f, 1f, 1f, 1f, 2f, 1f},
        new float[] {1f, 1f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 0.5f, 1f},
        new float[] {1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 0.5f, 1f, 1f, 1f, 1f, 0f, 0.5f, 1f},
        new float[] {1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 0.5f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 1f, 2f, 0.5f, 0.5f},
        new float[] {1f, 2f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f},
        new float[] {0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f},
        new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 0f},
        new float[] {1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f},
        new float[] {1f, 0.5f, 0.5f, 0.5f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 0.5f, 2f},
        new float[] {1f, 0.5f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 0.5f, 1f}
    };

    public static float GetMultEffectiveness(PokymonType attackType, PokymonType defender)
    {
        if (attackType == PokymonType.None || defender == PokymonType.None)
        {
            return 1f;
        }

        int row = (int) attackType;
        int col = (int) defender;

        return matrix[row - 1][col - 1];
    }
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase move;
    public MoveBase Move => move;
    
    [SerializeField] private int level;
    public int Level => level;
}
