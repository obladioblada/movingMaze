using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbstractState {
    protected readonly StateMachine stateMachine;

    protected AbstractState(State name, StateMachine stateMachine) {
        this.Name = name;
        this.stateMachine = stateMachine;
    }

    public State Name { get; }

    public StateMachine StateMachine => stateMachine;

    public virtual void Enter() {
        
    }

    public virtual void HandleInput() {}
    
}

public enum State
{
    STATE_MENU,
    STATE_SHIFT,
    STATE_MOVE,
};