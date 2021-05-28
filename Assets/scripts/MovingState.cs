using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : AbstractState {
    
    public MovingState(State name, StateMachine stateMachine) : base(name, stateMachine) {}
    
    
    public override void HandleInput()
    {
        base.HandleInput();
        // todo handle movement around the grid of a player
        if (Input.GetKeyDown(InputController.INPUT_INSERT)) {
           StateMachine.ChangeState(GameController._states[State.STATE_SHIFT]);
        }
    }
    
    public override void Exit() {
        Debug.Log("calling exit from moving!");
        Debug.Log("current activePlayer " + GameController.activePlayer);
        GameController.UpdateActivePlayer();
    }
}
