using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateMachine {
    
    public AbstractState currentState;
    public int activePlayer = - 1;
    private Text _text;
    
    public void Initialize(AbstractState startingState, Text text) {
        _text = text;
        currentState = startingState;
        Debug.Log(startingState);
        startingState.Enter();
        _text.text = currentState.Name.ToString();
    }

    public void ChangeState(AbstractState newState) {
        Debug.Log("Changing State to " + newState.Name);
        GameController.activeState = newState.Name;
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
        _text.text = currentState.Name.ToString();
    }
}
