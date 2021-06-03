using System.Collections;
using System.Collections.Generic;
using grid;
using UnityEngine;

public class MovingState : AbstractState {
    
    private Vector2 selectedTilePos;
    public MovingState(State name, StateMachine stateMachine) : base(name, stateMachine) {}


    public override void Enter() {
        base.Enter();
        // todo Calculate paths from player position
        var playerPosition = GameController.getActivePlayer().playerGameObject.transform.position;
        var baseTile = GridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == (Vector2) playerPosition);
        baseTile.SetColor(Color.blue);
        selectedTilePos = baseTile.gameObject.transform.position;
        GridManager.CalculatePath(GameController.getActivePlayer());
    }
    
    

    public override void HandleInput()
    {
        base.HandleInput();
        if (Input.GetKeyDown(InputController.INPUT_INSERT)) {
           StateMachine.ChangeState(GameController._states[State.STATE_SHIFT]);
        }

        // todo handle movement around the grid of a player
        if (Input.GetKeyDown(InputController.INPUT_LEFT)) {
            GridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position ==  selectedTilePos).SetColor(Color.white);
            if (selectedTilePos.x == 0) selectedTilePos = new Vector2(GridManager.N - 1, selectedTilePos.y);
            else selectedTilePos += Vector2.left;
            GridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position ==  selectedTilePos).SetColor(Color.blue);

        }

        if (Input.GetKeyDown(InputController.INPUT_RIGHT)) {
            GridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.white);
            if ((int)selectedTilePos.x == GridManager.N - 1) selectedTilePos = new Vector2(0, selectedTilePos.y);
            else selectedTilePos += Vector2.right;
            GridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.blue);
        }

        if (Input.GetKeyDown(InputController.INPUT_UP)) {
            GridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.white);
            if ((int)selectedTilePos.y == GridManager.N - 1) selectedTilePos = new Vector2( selectedTilePos.x, 0);
            else selectedTilePos += Vector2.up;
            GridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.blue);
        }

        if (Input.GetKeyDown(InputController.INPUT_DOWN)) {
            GridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.white);
            if ((int)selectedTilePos.y == 0) selectedTilePos = new Vector2( selectedTilePos.x, GridManager.N - 1);
            selectedTilePos += Vector2.down;
            GridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position ==  selectedTilePos).SetColor(Color.blue);
        }
    }
    
    public override void Exit() {
        Debug.Log("calling exit from moving!");
        Debug.Log("current activePlayer " + GameController.activePlayer);
        GameController.UpdateActivePlayer();
    }
}
