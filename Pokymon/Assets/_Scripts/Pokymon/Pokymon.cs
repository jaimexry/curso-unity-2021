using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokymon
{
    private PokymonBase _base;
    public PokymonBase Base => _base;

    private int _level;
    public int Level
    {
        get => _level;
        set => _level = value;
    }

    private int _hp;
    public int Hp
    {
        get => _hp;
        set => _hp = value;
    }
    
    private List<Move> _moves;
    public List<Move> Moves
    {
        get => _moves;
        set => _moves = value;
    }
    public int MaxHp => Mathf.FloorToInt((_base.MaxHp * _level) / 20.0f) + 10;
    public int Attack => Mathf.FloorToInt((_base.Attack * _level) / 100.0f) + 1;
    public int Defense => Mathf.FloorToInt((_base.Defense * _level) / 100.0f) + 1;
    public int SpAttack => Mathf.FloorToInt((_base.SpAttack * _level) / 100.0f) + 1;
    public int SpDefense => Mathf.FloorToInt((_base.SpDefense * _level) / 100.0f) + 1;
    public int Speed => Mathf.FloorToInt((_base.Speed * _level) / 100.0f) + 1;

    public Pokymon(PokymonBase pokymonBase, int pokymonLevel)
    {
        _base = pokymonBase;
        _level = pokymonLevel;
        _hp = MaxHp;
        
        _moves = new List<Move>();

        foreach (var learnableMove in _base.LearnableMoves)
        {
            if (learnableMove.Level <= _level)
            {
                _moves.Add(new Move(learnableMove.Move));
            }

            if (_moves.Count >= 4)
            {
                break;
            }
        }
    }

    public bool ReceiveDamage(Pokymon attacker, Move move)
    {
        float modifiers = Random.Range(0.85f, 1f);
        float attackStat = (move.Base.Category == MoveCategory.Physical ? attacker.Attack : attacker.SpAttack);
        float defenseStat = (move.Base.Category == MoveCategory.Physical ? this.Defense : this.SpDefense);
        float baseDamage = (((2 * attacker.Level / 5f + 2.0f) * move.Base.Power * (attackStat / defenseStat)) / 50.0f + 2.0f);
        int damage = Mathf.FloorToInt(baseDamage * modifiers);

        if (Random.Range(0, 100) < move.Base.Accuracy)
        {
            Hp -= damage;
        }
        if (Hp <= 0)
        {
            Hp = 0;
            return true;
        }

        return false;
    }
}
