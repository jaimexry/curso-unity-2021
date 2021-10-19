using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public enum BattleState
{
    StartBattle,
    PlayerSelectAction,
    PlayerMove,
    EnemyMove,
    Busy
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHud;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD enemyHud;
    [SerializeField] private BattleDialogBox battleDialogBox;
    private int currentSelectedAction;
    private int currentSelectedMovement;
    private float timeSinceLastClick;
    public float timeBetweenClicks = 0.2f;

    public BattleState state;
    
    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        state = BattleState.StartBattle;

        playerUnit.SetupPokymon();
        playerHud.SetPokymonData(playerUnit.Pokymon);
        battleDialogBox.SetPokymonMovements(playerUnit.Pokymon.Moves);
        enemyUnit.SetupPokymon();
        enemyHud.SetPokymonData(enemyUnit.Pokymon);
        
        yield return battleDialogBox.WriteDialog(string.Format("Un {0} salvaje apareció.", enemyUnit.Pokymon.Base.Name));

        StartCoroutine(PlayerAction());
    }

    private IEnumerator PlayerAction()
    {
        state = BattleState.PlayerSelectAction;
        battleDialogBox.ToggleMovements(false);
        battleDialogBox.ToggleDialogText(true);
        yield return StartCoroutine(battleDialogBox.WriteDialog("Selecciona una acción:"));
        battleDialogBox.ToggleActions(true);
        currentSelectedAction = 0;
        battleDialogBox.SelectAction(currentSelectedAction);
    }

    private void PlayerMovement()
    {
        state = BattleState.PlayerMove;
        battleDialogBox.ToggleDialogText(false);
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(true);
        currentSelectedMovement = 0;
        battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokymon.Moves[currentSelectedMovement]);
    }

    private IEnumerator EnemyAction()
    {
        state = BattleState.EnemyMove;
        yield return null;
    }

    private void Update()
    {
        timeSinceLastClick += Time.deltaTime;

        if (state == BattleState.PlayerSelectAction)
        {
            HandlePlayerActionSelection();
        }else if (state == BattleState.PlayerMove)
        {
            HandlePlayerMovementSelection();
        }
    }

    private void HandlePlayerActionSelection()
    {
        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;

            currentSelectedAction = (currentSelectedAction + 1) % 2;
            
            battleDialogBox.SelectAction(currentSelectedAction);
        }

        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            if (currentSelectedAction == 0)
            {
                PlayerMovement();
            }
            else
            {
                //TODO: implementar la huida
            }
        }
    }

    private void HandlePlayerMovementSelection()
    {
        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            var oldSelectedMovement = currentSelectedMovement;
            currentSelectedMovement = (currentSelectedMovement + 2) % 4;
            
            if (currentSelectedMovement >= playerUnit.Pokymon.Moves.Count)
            {
                currentSelectedMovement = oldSelectedMovement;
            }
            battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokymon.Moves[currentSelectedMovement]);
        }else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            var oldSelectedMovement = currentSelectedMovement;
            if (currentSelectedMovement <= 1)
            {
                currentSelectedMovement = (currentSelectedMovement + 1) % 2;
            }
            else
            {
                currentSelectedMovement = ((currentSelectedMovement + 1) % 2) + 2;
            }
            
            if (currentSelectedMovement >= playerUnit.Pokymon.Moves.Count)
            {
                currentSelectedMovement = oldSelectedMovement;
            }
            battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokymon.Moves[currentSelectedMovement]);
        }

        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            battleDialogBox.ToggleMovements(false);
            battleDialogBox.ToggleDialogText(true);
            if (enemyUnit.Pokymon.Speed <= playerUnit.Pokymon.Speed)
            {
                StartCoroutine(PerformPlayerMovement());
            }
            else
            {
                StartCoroutine(EnemyAction());
            }
        }
        
        if (Input.GetAxisRaw("Cancel") != 0)
        {
            timeSinceLastClick = 0;
            StartCoroutine(PlayerAction());
        }
    }

    private IEnumerator PerformPlayerMovement()
    {
        Move move = playerUnit.Pokymon.Moves[currentSelectedMovement];
        yield return battleDialogBox.WriteDialog(string.Format("{0} ha usado {1}.", playerUnit.Pokymon.Base.Name,
            move.Base.Name));

        bool pokemonFainted = enemyUnit.Pokymon.ReceiveDamage(playerUnit.Pokymon, move);

        if (pokemonFainted)
        {
            yield return battleDialogBox.WriteDialog(
                string.Format("{0} se ha debilitado.", enemyUnit.Pokymon.Base.Name));
        }
        else if (enemyUnit.Pokymon.Speed <= playerUnit.Pokymon.Speed)
        {
            StartCoroutine(EnemyAction());
        }
        else
        {
            PlayerMovement();
        }
    }
}
