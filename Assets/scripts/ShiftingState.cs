using System.Collections;
using System.Collections.Generic;
using grid;
using UnityEngine;

public class ShiftingState : AbstractState {
    
    private bool rotating;
    public ShiftingState(State name, StateMachine stateMachine) : base(name, stateMachine) { }

    public override void Enter() {
        base.Enter();
        GridManager.GetSelectedArrow().SetColor(Color.yellow);
        if (!GridManager._isFirstTurn) {
            GridManager.GetOppositeSelectedArrow().SetColor(Color.red);
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();
        if (Input.GetKeyDown(InputController.INPUT_LEFT)) {GameController.gridManager.MoveArrowLeft();}
        if (Input.GetKeyDown(InputController.INPUT_RIGHT)) {GameController.gridManager.MoveArrowRight();}
        if (Input.GetKeyDown(InputController.INPUT_UP)) {GameController.gridManager.MoveArrowUp();}
        if (Input.GetKeyDown(InputController.INPUT_DOWN)) {GameController.gridManager.MoveArrowDown();}
        if (Input.GetKeyDown(InputController.INPUT_ROTATE)) {GameController.gridManager.RotateSpareTile();};
        if (Input.GetKeyDown(InputController.INPUT_INSERT)) {
            if (GameController.gridManager.InsertTile()) {
                StateMachine.ChangeState(GameController._states[State.STATE_MOVE]);
            }
        };
    }

    public override void Exit() {
         var message = GameController.updatePlayerMessage(GameController.activePlayer, true);
         GameController.sendMessageToPlayer(message, GameController.activePlayer);
     }
}
