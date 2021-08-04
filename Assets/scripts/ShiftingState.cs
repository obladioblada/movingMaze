using System.Collections;
using System.Collections.Generic;
using grid;
using TMPro;
using UnityEngine;

public class ShiftingState : AbstractState {
    
    private bool rotating;
    private GameController _gameController;

    public ShiftingState(State name, StateMachine stateMachine, GameController gameController) : base(name, stateMachine) {
        _gameController = gameController;
    }

    public override void Enter() {
        base.Enter();
        GameController.gridManager.GetSelectedArrow().SetColor(Color.yellow);
        if (!GameController.gridManager._isFirstTurn) {
            GameController.gridManager.GetOppositeSelectedArrow().SetColor(Color.red);
        }
        _gameController.rotate_function.GetComponent<TextMeshProUGUI>().text = UIManager.BUTTON_ROTATE_FUNCTION_ROTATE;
        _gameController.select_function.GetComponent<TextMeshProUGUI>().text = UIManager.BUTTON_SELECT_FUNCTION_SHIFT;
    }

    public override void HandleInput()
    {
        base.HandleInput();
        if (Input.GetKeyDown(InputController.INPUT_LEFT)) {
            GameController.gridManager.MoveArrowLeft();
        }

        if (Input.GetKeyDown(InputController.INPUT_RIGHT)) {
            GameController.gridManager.MoveArrowRight();
        }

        if (Input.GetKeyDown(InputController.INPUT_UP)) {
            GameController.gridManager.MoveArrowUp();
        }

        if (Input.GetKeyDown(InputController.INPUT_DOWN)) {
            GameController.gridManager.MoveArrowDown();
        }

        if (Input.GetKeyDown(InputController.INPUT_ROTATE)) {
            GameController.gridManager.RotateSpareTile();
        }

        if (Input.GetKeyDown(InputController.INPUT_INSERT)) {
            GameController.gridManager.InsertTile();
            StateMachine.ChangeState(GameController._states[State.STATE_MOVE]);
        }
        
    }

    public override void Exit() {
         var message = GameController.updatePlayerMessage(GameController.activePlayer, true);
         GameController.sendMessageToPlayer(message, GameController.activePlayer);
     }
}
