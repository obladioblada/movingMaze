using System.Collections;
using System.Collections.Generic;
using grid;
using UnityEngine;

public class MovingState : AbstractState {
    
    private Vector2 selectedTilePos;
    public MovingState(State name, StateMachine stateMachine) : base(name, stateMachine) {}


    public override void Enter() {
        base.Enter();
        var playerPosition = GameController.getActivePlayer().playerGameObject.transform.position;
        var baseTile = GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == (Vector2) playerPosition);
        baseTile.SetColor(Color.gray);
        selectedTilePos = baseTile.gameObject.transform.position;
        GameController.gridManager.CalculatePath(GameController.getActivePlayer());
        baseTile.SetColor(Color.white);
    }
    

    public override void HandleInput()
    {
        base.HandleInput();
        if (Input.GetKeyDown(InputController.INPUT_INSERT)) {
            var onTile = GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos);
            GameController.gridManager.MovePlayer(GameController.getActivePlayer(), onTile.gameObject.transform.position);
            StateMachine.ChangeState(GameController._states[State.STATE_SHIFT]);
        }

        // todo handle movement around the grid of a player
        if (Input.GetKeyDown(InputController.INPUT_LEFT)) {
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position ==  selectedTilePos).SetColor(Color.white);
            if (selectedTilePos.x == 0) selectedTilePos = new Vector2(GridManager.N - 1, selectedTilePos.y);
            else selectedTilePos += Vector2.left;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position ==  selectedTilePos).SetColor(Color.gray);

        }

        if (Input.GetKeyDown(InputController.INPUT_RIGHT)) {
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.white);
            if ((int)selectedTilePos.x == GridManager.N - 1) selectedTilePos = new Vector2(0, selectedTilePos.y);
            else selectedTilePos += Vector2.right;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.grey);
        }

        if (Input.GetKeyDown(InputController.INPUT_UP)) {
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.white);
            if ((int)selectedTilePos.y == GridManager.N - 1) selectedTilePos = new Vector2( selectedTilePos.x, 0);
            else selectedTilePos += Vector2.up;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.grey);
        }

        if (Input.GetKeyDown(InputController.INPUT_DOWN)) {
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.white);
            if ((int)selectedTilePos.y == 0) selectedTilePos = new Vector2( selectedTilePos.x, GridManager.N - 1);
            else selectedTilePos += Vector2.down;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position ==  selectedTilePos).SetColor(Color.gray);
        }
    }

    public override void Exit() {
        Debug.Log("calling exit from moving!");
        Debug.Log("current activePlayer " + GameController.activePlayer);
        GameController.UpdateActivePlayer();
    }
}
