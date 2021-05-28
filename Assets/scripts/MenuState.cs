using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuState : AbstractState
{
    public MenuState(State name, StateMachine stateMachine) : base(name, stateMachine) { }
    
    
    public override void HandleInput()
    {
        base.HandleInput();
        if (Input.GetKeyDown(InputController.INPUT_START)) {
            stateMachine.ChangeState(GameController._states[State.STATE_SHIFT]);
        }
    }

    public override void Exit() {
        GameController.StartGame();
    }
}
