using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    ForgetMovement,
    Busy,
    LoseTurn,
    FinishBattle
}

public enum BattleType
{
    WildPokymon,
    Trainer,
    Leader
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialogBox battleDialogBox;
    [SerializeField] private PartyHUD partyHud;
    [SerializeField] private SelectionMovementUI selectMoveUI;
    [SerializeField] private GameObject pokeball;
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
    private bool pokemonFainted;
    public BattleState state;
    public BattleType type;
    private PokymonParty playerParty;
    private Pokymon wildPokymon;
    private bool hasThrownAPokeball;
    private int escapeAttemps;
    private MoveBase moveToLearn;
    
    public void HandleStartBattle(PokymonParty playerParty, Pokymon wildPokymon)
    {
        type = BattleType.WildPokymon;
        this.playerParty = playerParty;
        this.wildPokymon = wildPokymon;
        StartCoroutine(SetupBattle());
    }

    public void HandleStartTrainerBattle(PokymonParty playerParty, PokymonParty trainerParty, bool isLeader)
    {
        type = (isLeader ? BattleType.Leader : BattleType.Trainer);
        //TODO: El resto de batalla contra NPC
    }

    private IEnumerator SetupBattle()
    {
        state = BattleState.StartBattle;

        playerUnit.SetupPokymon(playerParty.GetFirstHealthyPokymon());
        hasThrownAPokeball = false;
        hasChangedPokymon = false;
        escapeAttemps = 0;
        battleDialogBox.SetPokymonMovements(playerUnit.Pokymon.Moves);
        enemyUnit.transform.localScale = new Vector3(1f, 1f, 1f);
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
        battleDialogBox.ToggleActions(false);
        StartCoroutine(ThrowPokeball());
        if (Input.GetAxisRaw("Cancel") != 0)
        {
            timeSinceLastClick = 0;
            StartCoroutine(PlayerActionSelection());
        }
    }
    
    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;

        if (timeSinceLastClick < timeBetweenClicks || battleDialogBox.isWriting || partyHud.isWriting)
        {
            return;
        }
        
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
        }else if (state == BattleState.LoseTurn)
        {
            StartCoroutine(PerformEnemyMovement());
        }else if (state == BattleState.ForgetMovement)
        {
            selectMoveUI.HandleForgetMoveSelection(
                (moveIndex) =>
            {
                if (moveIndex < 0)
                {
                    timeSinceLastClick = 0;
                    return;
                }

                StartCoroutine(ForgetOldMove(moveIndex));
            });
        }
    }

    private void HandlePlayerActionSelection()
    {
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
                StartCoroutine(TryToEscapeFromBattle());
            }
        }
    }

    private void HandlePlayerMovementSelection()
    {
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
            StartCoroutine(PerformPlayerMovement());
        }
        if (Input.GetAxisRaw("Cancel") != 0 && !moveSelected)
        {
            timeSinceLastClick = 0;
            StartCoroutine(PlayerActionSelection());
        }
    }
    
    private void HandlePlayerPartySelection()
    {
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

        if (Input.GetAxisRaw("Submit") != 0)
        {
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
        if (move.PP <= 0 && !playerUnit.Pokymon.CheckIfHasMovesWithPP())
        {
            battleDialogBox.ToggleMovements(false);
            battleDialogBox.ToggleDialogText(true);
            yield return battleDialogBox.WriteDialog("No quedan movimientos con PP.");
            playerUnit.Hud.lastHp = playerUnit.Pokymon.Hp;
            playerUnit.PlayReceiveAttackAnimation();
            pokemonFainted = playerUnit.Pokymon.ReceiveAutoDamage(); 
            playerUnit.Hud.UpdatePokemonData();
            if (pokemonFainted)
            {
                playerUnit.PlayFaintAnimation();
                yield return battleDialogBox.WriteDialog(
                    string.Format("{0} se ha debilitado.", playerUnit.Pokymon.Base.Name));
                yield return new WaitForSeconds(0.5f);
                moveSelected = false;
                CheckForBattleFinish(playerUnit);
                yield break;
            }
            StartCoroutine(PerformEnemyMovement());
            yield break;
        }
        if (move.PP <= 0)
        {
            battleDialogBox.ToggleMovements(false);
            battleDialogBox.ToggleDialogText(true);
            yield return battleDialogBox.WriteDialog("No quedan PPs para ese movimiento.");
            yield return new WaitForSeconds(0.1f);
            PlayerMovementSelection();
            yield break;
        }

        if (state == BattleState.PerformMovement)
        {
            if (enemyUnit.Pokymon.Speed <= playerUnit.Pokymon.Speed)
            {
                yield return RunMovement(playerUnit, enemyUnit, move);
                StartCoroutine(PerformEnemyMovement());
            }
            else
            {
                StartCoroutine(PerformEnemyMovement());
            }
        }
    }
    
    private IEnumerator PerformEnemyMovement()
    {
        state = BattleState.PerformMovement;
        pokemonFainted = false;

        if (enemyUnit.Pokymon.Hp <= 0)
        {
            CheckForBattleFinish(enemyUnit);
            yield break;
        }

        Move move = enemyUnit.Pokymon.RandomMove();
        if (move != null)
        {
            yield return RunMovement(enemyUnit, playerUnit, move);
        }

        if (playerUnit.Pokymon.CheckIfHasMovesWithPP() == false)
        {
            PlayerMovementSelection();
            yield break;
        }

        if (state == BattleState.PerformMovement)
        {
            if (!hasThrownAPokeball)
            {
                if (enemyUnit.Pokymon.Speed > playerUnit.Pokymon.Speed && !hasChangedPokymon)
                {
                    yield return RunMovement(playerUnit, enemyUnit, playerUnit.Pokymon.Moves[currentSelectedMovement]);
                    PlayerMovementSelection();
                }
                else
                {
                    PlayerMovementSelection();
                    hasChangedPokymon = false;
                }
            }
            else
            {
                hasChangedPokymon = false;
                hasThrownAPokeball = false;
                StartCoroutine(PlayerActionSelection());
            }
        }
    }

    private IEnumerator RunMovement(BattleUnit attacker, BattleUnit defender, Move move)
    {
        move.PP--;
        if (attacker.IsPlayer && move.PP <= 0)
        {
            move.PP = 0;
        }
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
                yield return HandlePokymonFainted(defender);
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
        if (move.Base.FixedPower > 0)
        {
            damage = move.Base.FixedPower;
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
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(false);
        battleDialogBox.ToggleDialogText(true);
        if (lastPokymonHp > 0)
        {
            StartCoroutine(PerformEnemyMovement());
        }
        else
        {
            StartCoroutine(PlayerActionSelection());
        }
    }

    private IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if (type != BattleType.WildPokymon)
        {
            yield return battleDialogBox.WriteDialog("No puedes robar los pokemon de otros entrenadores.");
            state = BattleState.LoseTurn;
            yield break;
        }
        
        hasThrownAPokeball = true;
        yield return battleDialogBox.WriteDialog(string.Format("Has lanzado una {0}.", pokeball.name));
        var pokeballInst = Instantiate(pokeball, playerUnit.transform.position - new Vector3(2, 2), Quaternion.identity);

        var pokeballSpt = pokeballInst.GetComponent<SpriteRenderer>();
        yield return pokeballSpt.transform.DOLocalJump(enemyUnit.transform.position + new Vector3(0, 1.5f), 
            2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCapturedAnimation();
        yield return pokeballSpt.transform.DOLocalMoveY(enemyUnit.transform.position.y - 2, 1f).WaitForCompletion();

        var numberOfShakes = TryToCatchPokymon(enemyUnit.Pokymon);
        for (int i = 0; i < Mathf.Min(numberOfShakes, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeballSpt.transform.DOPunchRotation(new Vector3(0, 0, 15), 0.6f).WaitForCompletion();
        }

        if (numberOfShakes == 4)
        {
            yield return battleDialogBox.WriteDialog(string.Format("¡Has capturado un {0}!", enemyUnit.Pokymon.Base.Name));
            yield return pokeballSpt.DOFade(0, 1.5f).WaitForCompletion();

            if (playerParty.AddPokymonToParty(enemyUnit.Pokymon))
            {
                yield return battleDialogBox.WriteDialog(string.Format("{0} se ha añadido a tu equipo", enemyUnit.Pokymon.Base.Name));
            }
            else
            {
                yield return battleDialogBox.WriteDialog("En algun momento lo mandaremos al PC.");
            }
            Destroy(pokeballInst);
            BattleFinish(true);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeballSpt.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (numberOfShakes < 2)
            {
                yield return battleDialogBox.WriteDialog(string.Format("¡El {0} salvaje ha escapado!",
                    enemyUnit.Pokymon.Base.Name));
            }
            else
            {
                yield return battleDialogBox.WriteDialog("¡Casi lo atrapas!");
            }
            Destroy(pokeballInst);
            state = BattleState.LoseTurn;
        }
    }

    private int TryToCatchPokymon(Pokymon pokymon)
    {
        float bonusPokeball = 1;    //TODO: clase pokemon con bonus
        float bonusStat = 1;        //TODO: estados alterados
        float a = ((3 * pokymon.MaxHp - 2 * pokymon.Hp) * pokymon.Base.CatchRate * bonusPokeball * bonusStat) /
                  (3 * pokymon.MaxHp);
        if (a >= 255)
        {
            return 4;
        }
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (Random.Range(0, 65536) >= b)
            {
                break;
            }
            shakeCount++;
        }

        return shakeCount;
    }

    private IEnumerator TryToEscapeFromBattle()
    {
        state = BattleState.Busy;
        battleDialogBox.ToggleActions(false);
        if (type != BattleType.WildPokymon)
        {
            yield return battleDialogBox.WriteDialog("No puedes huir de combates contra entrenadores.");
            state = BattleState.LoseTurn;
            yield break;
        }

        int playerSpeed = playerUnit.Pokymon.Speed;
        int enemySpeed = enemyUnit.Pokymon.Speed;
        escapeAttemps++;
        if (playerSpeed >= enemySpeed)
        {
            yield return battleDialogBox.WriteDialog("Has huido del combate.");
            yield return new WaitForSeconds(1f);
            BattleFinish(false);
        }
        else
        {
            int oddsScape = (Mathf.FloorToInt(playerSpeed * 128 / (float)enemySpeed) + 30 * escapeAttemps) % 256;
            if (Random.Range(0, 256) < oddsScape)
            {
                yield return battleDialogBox.WriteDialog("Has huido del combate.");
                yield return new WaitForSeconds(1f);
                BattleFinish(false);
            }
            else
            {
                yield return battleDialogBox.WriteDialog("¡No puedes escapar!");
                state = BattleState.LoseTurn;
            }
        }
    }

    private IEnumerator HandlePokymonFainted(BattleUnit faintedUnit)
    {
        faintedUnit.PlayFaintAnimation();
        if (!faintedUnit.IsPlayer)
        {
            yield return battleDialogBox.WriteDialog(
                string.Format("El {0} rival se ha debilitado.", faintedUnit.Pokymon.Base.Name));
            int expBase = faintedUnit.Pokymon.Base.ExpBase;
            int level = faintedUnit.Pokymon.Level;
            float multiplier = (type == BattleType.WildPokymon ? 1 : 1.5f);
            int wonExp = Mathf.FloorToInt((multiplier * level * expBase) / 7);
            playerUnit.Pokymon.Experience += wonExp;
            yield return battleDialogBox.WriteDialog(string.Format("{0} ha ganado {1} puntos de experiencia.", playerUnit.Pokymon.Base.Name, wonExp));
            yield return playerUnit.Hud.SetSmoothExp();

            while (playerUnit.Pokymon.NeedsToLevelUp())
            {
                playerUnit.Hud.SetLevelText();
                playerUnit.Hud.lastHp = playerUnit.Pokymon.Hp;
                playerUnit.Hud.UpdatePokemonData();
                yield return battleDialogBox.WriteDialog(string.Format("¡{0} ha subido de nivel!", playerUnit.Pokymon.Base.Name));
                //TODO: intentar aprender un nuevo movimiento
                var newLearnableMove = playerUnit.Pokymon.GetLearnableMoveAtCurrentLevel();
                if (newLearnableMove != null)
                {
                    if (playerUnit.Pokymon.Moves.Count < PokymonBase.NUMBER_OF_LEARNABLE_MOVES)
                    {
                        playerUnit.Pokymon.LearnMove(newLearnableMove);
                        yield return battleDialogBox.WriteDialog(string.Format("¡{0} ha aprendido {1}!", playerUnit.Pokymon.Base.Name, newLearnableMove.Move.Name));
                        battleDialogBox.SetPokymonMovements(playerUnit.Pokymon.Moves);
                    }
                    else
                    {
                        yield return battleDialogBox.WriteDialog(string.Format("¡{0} intenta aprender {1}!", playerUnit.Pokymon.Base.Name, newLearnableMove.Move.Name));
                        yield return battleDialogBox.WriteDialog(string.Format("Pero no puede aprender mas de {0} movimientos...", PokymonBase.NUMBER_OF_LEARNABLE_MOVES));
                        yield return ChooseMovementToForget(playerUnit.Pokymon, newLearnableMove.Move);
                        yield return new WaitUntil(() => state != BattleState.ForgetMovement);
                    }
                }    
                yield return playerUnit.Hud.SetSmoothExp(true);
            }
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return battleDialogBox.WriteDialog(
                string.Format("{0} se ha debilitado.", faintedUnit.Pokymon.Base.Name));
        }
                    
        yield return new WaitForSeconds(0.5f);
        moveSelected = false;
        CheckForBattleFinish(faintedUnit);
    }

    private IEnumerator ChooseMovementToForget(Pokymon learner, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return battleDialogBox.WriteDialog("Selecciona el movimiento que quieres olvidar:");
        selectMoveUI.gameObject.SetActive(true);
        selectMoveUI.SetMovements(learner.Moves.Select(mv => mv.Base).ToList(), newMove);
        moveToLearn = newMove;
        state = BattleState.ForgetMovement;
    }
    
    private IEnumerator ForgetOldMove(int moveIndex)
    {
        selectMoveUI.gameObject.SetActive(false);
        if (moveIndex == PokymonBase.NUMBER_OF_LEARNABLE_MOVES)
        {
            yield return battleDialogBox.WriteDialog(string.Format("{0} no ha aprendido {1}.", playerUnit.Pokymon.Base.Name, moveToLearn.Name));
        }
        else
        {
            var selectedMove = playerUnit.Pokymon.Moves[moveIndex].Base;
            yield return battleDialogBox.WriteDialog(string.Format("¡{0} ha olvidado {1} y ha aprendido {2}!", playerUnit.Pokymon.Base.Name, selectedMove.Name, moveToLearn.Name));
            playerUnit.Pokymon.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;
        state = BattleState.Busy;
    }
}
