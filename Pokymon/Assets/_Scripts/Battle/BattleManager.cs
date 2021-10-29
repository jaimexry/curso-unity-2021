using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using Random = UnityEngine.Random;

public enum BattleState
{
    StartBattle,
    ActionSelection,
    MovementSelection,
    PerformMovement,
    ChoosingObject,
    ChoosingPokemon,
    Busy,
    FinishBattle
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialogBox battleDialogBox;
    [SerializeField] private PartyHUD partyHud;
    public event Action<bool> OnBattleFinish;
    private int currentSelectedAction;
    private int currentSelectedMovement;
    private int currentSelectedPokymon;
    private float timeSinceLastClick;
    [SerializeField] private float timeBetweenClicks = 0.2f;
    private int damage;
    private float critical;
    private float effectiveness;
    private bool actionSelected, moveSelected;
    private bool hasChangedPokymon;
    private float timeSinceLastSubmit;
    private bool pokemonFainted;
    public BattleState state;

    private PokymonParty playerParty;
    private Pokymon wildPokymon;
    
    public void HandleStartBattle(PokymonParty playerParty, Pokymon wildPokymon)
    {
        this.playerParty = playerParty;
        this.wildPokymon = wildPokymon;
        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        state = BattleState.StartBattle;

        playerUnit.SetupPokymon(playerParty.GetFirstHealthyPokymon());
        
        battleDialogBox.SetPokymonMovements(playerUnit.Pokymon.Moves);
        
        enemyUnit.SetupPokymon(wildPokymon);
        partyHud.InitPartyHUD();
        partyHud.gameObject.SetActive(false);
        yield return battleDialogBox.WriteDialog(string.Format("Un {0} salvaje apareció.", enemyUnit.Pokymon.Base.Name));
        
        StartCoroutine(PlayerActionSelection());
    }

    private void BattleFinish(bool playerHasWon)
    {
        state = BattleState.FinishBattle;
        OnBattleFinish(playerHasWon);
    }

    private IEnumerator PlayerActionSelection()
    {
        state = BattleState.ActionSelection;
        battleDialogBox.ToggleMovements(false);
        battleDialogBox.ToggleDialogText(true);
        yield return StartCoroutine(battleDialogBox.WriteDialog("Selecciona una acción:"));
        battleDialogBox.ToggleActions(true);
        actionSelected = false;
        currentSelectedAction = 0;
        battleDialogBox.SelectAction(currentSelectedAction);
    }

    private void PlayerMovementSelection()
    {
        state = BattleState.MovementSelection;
        moveSelected = false;
        battleDialogBox.ToggleDialogText(false);
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(true);
        currentSelectedMovement = 0;
        battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokymon.Moves[currentSelectedMovement]);
    }
    
    private void OpenPartySelectionScreen()
    {
        state = BattleState.ChoosingPokemon;
        partyHud.gameObject.SetActive(true);
        partyHud.SetPartyData(playerParty.Pokymons);
        partyHud.UpdateSelectedPokymon(currentSelectedPokymon);
    }
    
    private void OpenInventoryScreen()
    {
        state = BattleState.ChoosingObject;
        print("Abrir la pantalla de inventario");
        if (Input.GetAxisRaw("Cancel") != 0)
        {
            timeSinceLastClick = 0;
            StartCoroutine(PlayerActionSelection());
        }
    }
    
    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;
        timeSinceLastSubmit += Time.deltaTime;
        
        if (state == BattleState.ActionSelection)
        {
            HandlePlayerActionSelection();
        }else if (state == BattleState.MovementSelection)
        {
            HandlePlayerMovementSelection();
        }else if (state == BattleState.ChoosingObject)
        {
            OpenInventoryScreen();
        }else if (state == BattleState.ChoosingPokemon)
        {
            HandlePlayerPartySelection();
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

            currentSelectedAction = (currentSelectedAction + 2) % 4;
            
            battleDialogBox.SelectAction(currentSelectedAction);
        }else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedAction = (currentSelectedAction + 1) % 2 + 2 * (currentSelectedAction / 2);
            battleDialogBox.SelectAction(currentSelectedAction);
        }

        if (Input.GetAxisRaw("Submit") != 0 && !actionSelected)
        {
            actionSelected = true;
            timeSinceLastClick = 0;
            if (currentSelectedAction == 0)
            {
                PlayerMovementSelection();
            }
            else if (currentSelectedAction == 1)
            {
                OpenPartySelectionScreen();
            }else if (currentSelectedAction == 2)
            {
                OpenInventoryScreen();
            }else
            {
                battleDialogBox.ToggleActions(false);
                BattleFinish(false);
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

        if (Input.GetAxisRaw("Submit") != 0 && !moveSelected)
        {
            moveSelected = true;
            timeSinceLastClick = 0;
            battleDialogBox.ToggleMovements(false);
            battleDialogBox.ToggleDialogText(true);
            if (enemyUnit.Pokymon.Speed <= playerUnit.Pokymon.Speed)
            {
                StartCoroutine(PerformPlayerMovement());
            }
            else
            {
                StartCoroutine(PerformEnemyMovement());
            }
        }
        if (Input.GetAxisRaw("Cancel") != 0 && !moveSelected)
        {
            timeSinceLastClick = 0;
            StartCoroutine(PlayerActionSelection());
        }
    }
    
    private void HandlePlayerPartySelection()
    {
        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPokymon -= (int) Input.GetAxisRaw("Vertical") * 2;
        }else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPokymon += (int) Input.GetAxisRaw("Horizontal");
        }

        currentSelectedPokymon = Mathf.Clamp(currentSelectedPokymon, 0, playerParty.Pokymons.Count - 1);
        partyHud.UpdateSelectedPokymon(currentSelectedPokymon);

        if (Input.GetAxisRaw("Submit") != 0 && timeSinceLastSubmit > 1.2f)
        {
            timeSinceLastSubmit = 0;
            timeSinceLastClick = 0;
            var selectedPokymon = playerParty.Pokymons[currentSelectedPokymon];
            if (selectedPokymon.Hp <= 0)
            {
                StartCoroutine(partyHud.SetMessage("No puedes enviar un pokemon debilitado."));
                return;
            }
            else if (selectedPokymon == playerUnit.Pokymon)
            {
                StartCoroutine(partyHud.SetMessage("No puedes seleccionar el pokemon en batalla."));
                return;
            }
            else
            {
                hasChangedPokymon = true;
                partyHud.gameObject.SetActive(false);
                state = BattleState.Busy;
                StartCoroutine(SwitchPokymon(selectedPokymon));
            }
        }
        if (Input.GetAxisRaw("Cancel") != 0 && !hasChangedPokymon)
        {
            timeSinceLastClick = 0;
            partyHud.gameObject.SetActive(false);
            StartCoroutine(PlayerActionSelection());
        }
    }

    private IEnumerator PerformPlayerMovement()
    {
        state = BattleState.PerformMovement;
        
        Move move = playerUnit.Pokymon.Moves[currentSelectedMovement];
        if (move.PP <= 0)
        {
            battleDialogBox.ToggleMovements(false);
            battleDialogBox.ToggleDialogText(true);
            yield return battleDialogBox.WriteDialog("No quedan PPs para ese movimiento");
            PlayerMovementSelection();
        }

        if (state == BattleState.PerformMovement)
        {
            yield return RunMovement(playerUnit, enemyUnit, move);
            if (enemyUnit.Pokymon.Speed <= playerUnit.Pokymon.Speed && !pokemonFainted)
            {
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(PerformEnemyMovement());
            }
            else if (!pokemonFainted)
            {
                PlayerMovementSelection();
            }
        }
    }
    
    private IEnumerator PerformEnemyMovement()
    {
        state = BattleState.PerformMovement;
        Move move;
        pokemonFainted = false;
        do
        {
            move = enemyUnit.Pokymon.RandomMove();
        } while (move.PP <= 0);

        yield return RunMovement(enemyUnit, playerUnit, move);
        if (enemyUnit.Pokymon.Speed > playerUnit.Pokymon.Speed && !pokemonFainted && !hasChangedPokymon)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(PerformPlayerMovement());
        }
        else if (!pokemonFainted)
        {
            PlayerMovementSelection();
            hasChangedPokymon = false;
        }
    }

    private IEnumerator RunMovement(BattleUnit attacker, BattleUnit defender, Move move)
    {
        move.PP--;
        pokemonFainted = false;
        if (attacker.IsPlayer)
        {
            yield return battleDialogBox.WriteDialog(string.Format("{0} ha usado {1}.", attacker.Pokymon.Base.Name,
                move.Base.Name));
        }
        else
        {
            yield return battleDialogBox.WriteDialog(string.Format("El {0} rival ha usado {1}.", attacker.Pokymon.Base.Name,
                move.Base.Name));
        }
        
        if (Random.Range(0, 100) < move.Base.Accuracy) 
        {
            CalculateDamage(attacker.Pokymon, defender.Pokymon, move); 
            defender.Hud.lastHp = defender.Pokymon.Hp;
            attacker.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            if (damage > 0)
            {
                defender.PlayReceiveAttackAnimation();
            }
            pokemonFainted = defender.Pokymon.ReceiveDamage(damage);
            defender.Hud.UpdatePokemonData();
            if (critical > 1.5f)
            {
                yield return battleDialogBox.WriteDialog("¡Golpe crítico!");
            }
            if (effectiveness == 0)
            {
                yield return battleDialogBox.WriteDialog("No hizo efecto...");
            }else if (effectiveness <= 0.5f)
            {
                yield return battleDialogBox.WriteDialog("Es poco eficaz.");
            }else if (effectiveness > 1f)
            {
                if (effectiveness <= 2f)
                {
                    yield return battleDialogBox.WriteDialog("¡Es muy eficaz!"); 
                }
                else
                {
                    yield return battleDialogBox.WriteDialog("¡¡Es súper eficaz!!");
                }
            }
            if (pokemonFainted)
            {
                defender.PlayFaintAnimation();
                if (!defender.IsPlayer)
                {
                    yield return battleDialogBox.WriteDialog(
                        string.Format("El {0} rival se ha debilitado.", defender.Pokymon.Base.Name));
                }
                else
                {
                    yield return battleDialogBox.WriteDialog(
                        string.Format("{0} se ha debilitado.", defender.Pokymon.Base.Name));
                }
                    
                yield return new WaitForSeconds(0.5f);
                moveSelected = false;
                CheckForBattleFinish(defender);
            }
        }
        else
        {
            if (!attacker.IsPlayer)
            {
                yield return battleDialogBox.WriteDialog(string.Format("El {0} rival ha fallado el ataque.", attacker.Pokymon.Base.Name));
            }
            else
            {
                yield return battleDialogBox.WriteDialog(string.Format("{0} ha fallado el ataque.", attacker.Pokymon.Base.Name));
            }
        }
    }

    private void CheckForBattleFinish(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayer)
        {
            var nextPokymon = playerParty.GetFirstHealthyPokymon();
            if (nextPokymon != null)
            {
                OpenPartySelectionScreen();
            }
            else
            {
                BattleFinish(false);
            }
        }
        else
        {
            BattleFinish(true);
        }
    }

    private void CalculateDamage(Pokymon attacker, Pokymon defender, Move move)
    {
        float random = Random.Range(0.85f, 1f);
        critical = (move.Base.Power > 0 && Random.Range(0, 256) < (attacker.Base.Speed / 2) ? 2f : 1f);
        float stab = (move.Base.Type == attacker.Base.Type1 || move.Base.Type == attacker.Base.Type2 ? 1.5f : 1f);
        float attackStat = (move.Base.Category == MoveCategory.Physical ? attacker.Attack : attacker.SpAttack);
        float defenseStat = (move.Base.Category == MoveCategory.Physical ? defender.Defense : defender.SpDefense);
        float baseDamage = (((2 * attacker.Level / 5f + 2.0f) * move.Base.Power * (attackStat / defenseStat)) / 50.0f + 2.0f);
        float effectiveness1 = TypeMatrix.GetMultEffectiveness(move.Base.Type, defender.Base.Type1);
        float effectiveness2 = TypeMatrix.GetMultEffectiveness(move.Base.Type, defender.Base.Type2);
        effectiveness = effectiveness1 * effectiveness2;
        damage = Mathf.FloorToInt(baseDamage * random * critical * stab * effectiveness);
        if (move.Base.Category == MoveCategory.Status)
        {
            damage = 0;
        }
    }

    private IEnumerator SwitchPokymon(Pokymon newPokymon)
    {
        var lastPokymonHp = playerUnit.Pokymon.Hp;
        if (lastPokymonHp > 0)
        {
            battleDialogBox.ToggleActions(false);
            yield return battleDialogBox.WriteDialog(string.Format("¡Vuelve {0}!", playerUnit.Pokymon.Base.Name));
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }
        playerUnit.SetupPokymon(newPokymon);
        battleDialogBox.SetPokymonMovements(newPokymon.Moves);
        yield return battleDialogBox.WriteDialog(string.Format("¡Ve {0}!", newPokymon.Base.Name));
        yield return new WaitForSeconds(0.5f);
        if (lastPokymonHp > 0)
        {
            StartCoroutine(PerformEnemyMovement());
        }
        else
        {
            StartCoroutine(PlayerActionSelection());
        }
    }
}
