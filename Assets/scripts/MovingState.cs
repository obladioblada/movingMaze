using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : AbstractState {
    
    public MovingState(State name, StateMachine stateMachine) : base(name, stateMachine) {}
    
    
    public override void HandleInput()
    {
        base.HandleInput();
        // todo handle movement around the grid of a player
    }
}
