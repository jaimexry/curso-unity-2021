using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMovementUI : MonoBehaviour
{
    [SerializeField] private Text[] movementText;
    [SerializeField] private Color selectedColor = Color.blue;
    private int currentSelectedMovement = 0;

    public void SetMovements(List<MoveBase> pokymonMoves, MoveBase newMove)
    {
        currentSelectedMovement = 0;
        for (int i = 0; i < pokymonMoves.Count; i++)
        {
            movementText[i].text = pokymonMoves[i].Name;
        }

        movementText[pokymonMoves.Count].text = newMove.Name;
    }

    public void HandleForgetMoveSelection(Action<int> onSelected)
    {
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            int direction = Mathf.FloorToInt(Input.GetAxisRaw("Vertical"));
            currentSelectedMovement -= direction;
            currentSelectedMovement = Mathf.Clamp(currentSelectedMovement, 0, PokymonBase.NUMBER_OF_LEARNABLE_MOVES);
            UpdateColorForgetMoveSelection();
            onSelected?.Invoke(-1);
        }

        if (Input.GetAxisRaw("Submit") != 0)
        {
            onSelected?.Invoke(currentSelectedMovement); 
        }
    }

    public void UpdateColorForgetMoveSelection()
    {
        for (int i = 0; i <= PokymonBase.NUMBER_OF_LEARNABLE_MOVES; i++)
        {
            movementText[i].color = (i == currentSelectedMovement ? selectedColor : Color.black);
        }
    }
}
