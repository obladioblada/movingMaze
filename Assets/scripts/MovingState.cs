using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using grid;
using UnityEngine;

public class MovingState : AbstractState {
    
    public Vector2 selectedTilePos;
    public MovingState(State name, StateMachine stateMachine) : base(name, stateMachine) {}


    public override void Enter() {
        base.Enter();
        var playerPosition = GameController.getActivePlayer().playerGameObject.transform.position;
        var baseTile = GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == (Vector2) playerPosition);
        baseTile.SetColor(Color.gray);
        selectedTilePos = baseTile.gameObject.transform.position;
    }

    private void clearTilePath() {
        foreach (var tile in GameController.gridManager._allowedTilePath) {
            tile.explored = false;
            tile.SetColor(Color.white);
            tile.weight = 0;
        }
        GameController.gridManager._allowedTilePath.Clear();
    }
    

    public override void HandleInput()
    {
        base.HandleInput();
        if (Input.GetKeyDown(InputController.INPUT_INSERT)) {
            var targetTile = GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos);
            if (GameController.gridManager._allowedTilePath.Contains(targetTile) &&
                !DOTween.IsTweening(GameController.getActivePlayer().playerGameObject.transform)) {
                //todo pathfinding to find shortest route to selected tile
                var playerTile = GameController.gridManager._tiles
                    .Find(t => (Vector2) t.gameObject.transform.position == 
                               (Vector2)GameController.getActivePlayer().playerGameObject.transform.position);
                if (targetTile != playerTile ) {
                    //have to move --> get shortest path to get to target
                   
                }
                StateMachine.ChangeState(GameController._states[State.STATE_SHIFT]);
            }
        }

        // todo handle movement around the grid of a player
        if (Input.GetKeyDown(InputController.INPUT_LEFT)) {
            resetTileColor();
            if (selectedTilePos.x == 0) selectedTilePos = new Vector2(GridManager.N - 1, selectedTilePos.y);
            else selectedTilePos += Vector2.left;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position ==  selectedTilePos).SetColor(Color.gray);
        }

        if (Input.GetKeyDown(InputController.INPUT_RIGHT)) {
            resetTileColor();           
            if ((int)selectedTilePos.x == GridManager.N - 1) selectedTilePos = new Vector2(0, selectedTilePos.y);
            else selectedTilePos += Vector2.right;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.grey);
        }

        if (Input.GetKeyDown(InputController.INPUT_UP)) {
            resetTileColor();    
            if ((int)selectedTilePos.y == GridManager.N - 1) selectedTilePos = new Vector2( selectedTilePos.x, 0);
            else selectedTilePos += Vector2.up;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.grey);
        }

        if (Input.GetKeyDown(InputController.INPUT_DOWN)) {
            resetTileColor();
            if ((int)selectedTilePos.y == 0) selectedTilePos = new Vector2( selectedTilePos.x, GridManager.N - 1);
            else selectedTilePos += Vector2.down;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position ==  selectedTilePos).SetColor(Color.gray);
        }
    }

    public void resetTileColor() {
        var currentTile = GameController.gridManager._tiles.FirstOrDefault(t => (Vector2) t.gameObject.transform.position == selectedTilePos);
        if (currentTile == null) return;
        currentTile.SetColor(GameController.gridManager._allowedTilePath.Contains(currentTile) ? Color.yellow : Color.white);
    }

    public override void Exit() {
        Debug.Log("calling exit from moving!");
        Debug.Log("current activePlayer " + GameController.activePlayer);
        clearTilePath();
        GameController.UpdateActivePlayer();
    }
}
