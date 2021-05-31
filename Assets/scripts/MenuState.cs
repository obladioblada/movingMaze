using System.Collections;
using System.Collections.Generic;
using grid;
using UnityEngine;

public class MenuState : AbstractState
{
    public MenuState(State name, StateMachine stateMachine) : base(name, stateMachine) { }


    public override void Enter() {
        base.Enter();
        
    }


    public override void HandleInput()
    {
        base.HandleInput();
        
    }

    public override void Exit() {
    }
}
