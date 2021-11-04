using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Travel, Battle }

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Camera worldMainCamera;
    private GameState _gameState;

    private void Awake()
    {
        _gameState = GameState.Travel;
    }

    private void Start()
    {
        playerController.OnPokemonEncountered += StartPokemonBattle;
        battleManager.OnBattleFinish += FinishPokymonBattle;
    }

    private void StartPokemonBattle()
    {
        _gameState = GameState.Battle;
        worldMainCamera.gameObject.SetActive(false);
        battleManager.gameObject.SetActive(true);
        var playerParty = playerController.GetComponent<PokymonParty>();
        var wildPokymon = FindObjectOfType<PokymonMapArea>().GetComponent<PokymonMapArea>().GetRandomWildPokymon();

        var wildPokymonCopy = new Pokymon(wildPokymon.Base, wildPokymon.Level);
        
        battleManager.HandleStartBattle(playerParty, wildPokymonCopy);
    }

    private void FinishPokymonBattle(bool playerHasWon)
    {
        _gameState = GameState.Travel;
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_gameState == GameState.Travel)
        {
            playerController.HandleUpdate();
        }else if (_gameState == GameState.Battle)
        {
            battleManager.HandleUpdate();
        }
    }
}
