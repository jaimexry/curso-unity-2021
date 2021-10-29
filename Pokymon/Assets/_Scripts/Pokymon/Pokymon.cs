using System;
using System.Collections;
using System.Collections.Generic;
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
        set => _hp = value;
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

    public void InitPokymon()
    {
        
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
        int randId = Random.Range(0, Moves.Count);
        return Moves[randId];
    }
}
