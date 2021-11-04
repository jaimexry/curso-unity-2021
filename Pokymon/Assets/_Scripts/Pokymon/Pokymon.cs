using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Pokymon
{
    [SerializeField] private PokymonBase _base;
    public PokymonBase Base => _base;

    [SerializeField] private int _level;
    public int Level
    {
        get => _level;
        set => _level = value;
    }

    private int _hp;
    public int Hp
    {
        get => _hp;
        set
        {
            _hp = value;
            _hp = Mathf.FloorToInt(Mathf.Clamp(_hp, 0, MaxHp));
        }
    }

    private int _experience;

    public int Experience
    {
        get => _experience;
        set => _experience = value;
    }

    private List<Move> _moves;
    public List<Move> Moves
    {
        get => _moves;
        set => _moves = value;
    }
    public int MaxHp => Mathf.FloorToInt((_base.MaxHp * _level) / 20.0f) + 10;
    public int Attack => Mathf.FloorToInt((_base.Attack * _level) / 20.0f) + 1;
    public int Defense => Mathf.FloorToInt((_base.Defense * _level) / 20.0f) + 1;
    public int SpAttack => Mathf.FloorToInt((_base.SpAttack * _level) / 20.0f) + 1;
    public int SpDefense => Mathf.FloorToInt((_base.SpDefense * _level) / 20.0f) + 1;
    public int Speed => Mathf.FloorToInt((_base.Speed * _level) / 20.0f) + 1;

    public Pokymon(PokymonBase pBase, int pLevel)
    {
        _base = pBase;
        _level = pLevel;
        InitPokymon();
    }
    
    public void InitPokymon()
    {
        
        _hp = MaxHp;
        _experience = Base.GetNecessaryExpForLevel(_level);
        _moves = new List<Move>();

        foreach (var learnableMove in _base.LearnableMoves)
        {
            if (learnableMove.Level <= _level)
            {
                _moves.Add(new Move(learnableMove.Move));
            }

            if (_moves.Count >= PokymonBase.NUMBER_OF_LEARNABLE_MOVES)
            {
                break;
            }
        }
    }

    public bool ReceiveDamage(int damage)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            return true;
        }

        return false;
    }

    public Move RandomMove()
    {
        var movesWithPP = Moves.Where(m => m.PP > 0).ToList();
        if (movesWithPP.Count > 0)
        {
            int randId = Random.Range(0, movesWithPP.Count);
            return movesWithPP[randId];
        }
        return null;
    }

    public bool ReceiveAutoDamage()
    {
        Hp -= (MaxHp / 15) + 1;
        if (Hp <= 0)
        {
            Hp = 0;
            return true;
        }

        return false;
    }

    public bool CheckIfHasMovesWithPP()
    {
        var movesWithPP = Moves.Where(m => m.PP > 0).ToList();
        if (movesWithPP.Count > 0)
        {
            return true;
        }

        return false;
    }

    public bool NeedsToLevelUp()
    {
        if (Experience > Base.GetNecessaryExpForLevel(_level + 1))
        {
            int currentMaxHp = MaxHp;
            _level++;
            Hp += (MaxHp - currentMaxHp);
            return true;
        }

        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return Base.LearnableMoves.Where(lm => lm.Level == _level).FirstOrDefault();
    }

    public void LearnMove(LearnableMove learnableMove)
    {
        if (Moves.Count >= PokymonBase.NUMBER_OF_LEARNABLE_MOVES)
        {
            return;
        }
        Moves.Add(new Move(learnableMove.Move));
    }
}
