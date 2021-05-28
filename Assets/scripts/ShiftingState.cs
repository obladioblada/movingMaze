using System.Collections;
using System.Collections.Generic;
using grid;
using UnityEngine;

public class ShiftingState : AbstractState
{
    public ShiftingState(State name, StateMachine stateMachine) : base(name, stateMachine) { }
    
    public override void HandleInput()
    {
        base.HandleInput();
        if (Input.GetKeyDown(InputController.INPUT_LEFT)) {GridManager.MoveArrowLeft();}
        if (Input.GetKeyDown(InputController.INPUT_RIGHT)) {GridManager.MoveArrowRight();}
        if (Input.GetKeyDown(InputController.INPUT_UP)) {GridManager.MoveArrowUp();}
        if (Input.GetKeyDown(InputController.INPUT_DOWN)) {GridManager.MoveArrowDown();}
        if (Input.GetKeyDown(InputController.INPUT_ROTATE)) {GridManager.RotateSpareTile();
        };
    }
}
