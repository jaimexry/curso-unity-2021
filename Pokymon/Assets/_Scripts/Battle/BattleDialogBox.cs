using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private Text dialogText;
    [SerializeField] private GameObject actionSelect;
    [SerializeField] private GameObject movementSelect;
    [SerializeField] private GameObject movementDesc;
    [SerializeField] private Color selectedColor = Color.blue;
    public float timeToWaitAfterText = 1.0f;
    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private List<Text> movementTexts;
    public bool isWriting;
    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;
    
    public float charactersPerSecond;

    public IEnumerator WriteDialog(string message)
    {
        dialogText.text = "";
        isWriting = true;
        foreach (var character in message)
        {
            dialogText.text += character;
            yield return new WaitForSeconds(1 / charactersPerSecond);
        }
        yield return new WaitForSeconds(timeToWaitAfterText);
        isWriting = false;
    }

    public void ToggleDialogText(bool activated)
    {
        dialogText.enabled = activated;
    }

    public void ToggleActions(bool activated)
    {
        actionSelect.SetActive(activated);
    }

    public void ToggleMovements(bool activated)
    {
        movementSelect.SetActive(activated);
        movementDesc.SetActive(activated);
    }

    public void SelectAction(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            actionTexts[i].color = (i == selectedAction ? selectedColor : Color.black);
        }
    }

    public void SetPokymonMovements(List<Move> moves)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                movementTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                movementTexts[i].text = "---";
            } 
        }
    }
    
    public void SelectMovement(int selectedMovement, Move move)
    {
        for (int i = 0; i < movementTexts.Count; i++)
        {
            movementTexts[i].color = (i == selectedMovement ? selectedColor : Color.black);
        }
        ppText.text = string.Format("PP {0}/{1}", move.PP, move.Base.PP);
        ChangePPColor(move);
        typeText.text = string.Format("{0}", move.Base.Type.ToString().ToUpper());
    }
    
    private void ChangePPColor(Move move)
    {
        if (move.PP > (move.Base.PP / 2))
        {
            ppText.color = Color.black;
        } else if (move.PP > (move.Base.PP / 10 + 1))
        {
            ppText.color = new Color(0.9411765f, 0.7450981f, 0.09803922f);
        }
        else
        {
            ppText.color = new Color(0.7843137f, 0, 0.01960784f);
        }
    }
}
