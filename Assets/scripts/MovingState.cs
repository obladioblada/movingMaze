using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using grid;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Tile = grid.Tile;

public class MovingState : AbstractState {
    public Vector2 selectedTilePos;
    private GameController _gameController;

    public MovingState(State name, StateMachine stateMachine, GameController gameController) : base(name, stateMachine) {
        _gameController = gameController;
    }


    public override void Enter() {
        base.Enter();
        var playerPosition = GameController.getActivePlayer().playerGameObject.transform.position;
        var baseTile = GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == (Vector2) playerPosition);
        baseTile.SetColor(Color.gray);
        selectedTilePos = baseTile.gameObject.transform.position;
        _gameController.rotate_function.SetActive(false);
        _gameController.select_function.GetComponent<TextMeshProUGUI>().text = UIManager.BUTTON_SELECT_FUNCTION_MOVE;
           
    }

    private void clearTilePath() {
        foreach (var tile in GameController.gridManager._allowedTilePath) {
            tile.explored = false;
            tile.SetColor(Color.white);
            tile.weight = int.MaxValue;
            tile.connectedTile = null;
        }

        GameController.gridManager._allowedTilePath.Clear();
    }

    public void getPath(ICollection<Tile> path, Tile tile) {
        path.Add(tile);
        Debug.Log("path" + tile.gameObject.transform.position);
        if (tile.weight == 0 || tile.connectedTile == null) {
            return;
        }

        getPath(path, tile.connectedTile);
    }


    public override void HandleInput() {
        base.HandleInput();
        if (GameController.gridManager._allowedTilePath.Count == 1) {
            StateMachine.ChangeState(GameController._states[State.STATE_SHIFT]);
        }
        if (Input.GetKeyDown(InputController.INPUT_INSERT)) {
            var targetTile = GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos);
            if (GameController.gridManager._allowedTilePath.Contains(targetTile) &&
                !DOTween.IsTweening(GameController.getActivePlayer().playerGameObject.transform)) {
                var playerTile = GameController.gridManager._tiles
                    .Find(t => (Vector2) t.gameObject.transform.position ==
                               (Vector2) GameController.getActivePlayer().playerGameObject.transform.position);
                if (targetTile != playerTile) {
                    Debug.Log("player tile with weight = " + playerTile.weight);
                    Debug.Log("target tile with weight = " + targetTile.weight);
                    Debug.Log("connected tile with weight = " + targetTile.connectedTile.weight);
                    var path = new List<Tile>();
                    getPath(path, targetTile);
                    if (path.Count > 0) {
                        var vectorPath = path.OrderBy(tile => tile.weight).Select(tile => {
                            var tilePos = tile.gameObject.transform.position;
                            Debug.Log(new Vector3(tilePos.x, tilePos.y, GameController.getActivePlayer().playerGameObject.transform.position.z));
                            return new Vector3(tilePos.x, tilePos.y, GameController.getActivePlayer().playerGameObject.transform.position.z);
                        });
                        var enumerable = vectorPath as Vector3[] ?? vectorPath.ToArray();
                        Debug.Log("vectorPath.count " + enumerable.Count());
                        GameController.getActivePlayer().playerGameObject.transform.DOPath(enumerable.ToArray(),
                            1f, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() => {
                            if (GameController.getActivePlayer().cards.Count > 0) {
                                Debug.Log("TARGET TILE");
                                if (targetTile.card != null) {
                                    Debug.Log(targetTile.card.id);
                                    Debug.Log(targetTile.card.cardGO.name);
                                    Debug.Log(targetTile.card.id +":"+GameController.getActivePlayer().cards.Peek().id);
                                }
                                if (targetTile.card == null || GameController.getActivePlayer().cards.Peek().id != targetTile.card.id) {
                                    Debug.Log("CHANGING STATE!");
                                    StateMachine.ChangeState(GameController._states[State.STATE_SHIFT]);
                                    return;
                                }
                                GameController.getActivePlayer().cards.Pop();
                                GameController.getActivePlayer().playerScoreLabel.GetComponent<TextMeshProUGUI>().text = "" + GameController.getActivePlayer().cards.Count;
                                    
                                Debug.Log(GameController.getActivePlayer().ToString());
                                var bytes = GameController.getActivePlayer().cards.Peek().cardGO.GetComponent<SpriteRenderer>().sprite.texture.EncodeToPNG();
                                GameController.UpdateActivePlayerCard(GameController.getActivePlayer().number,  Convert.ToBase64String(bytes));
                                Debug.Log("GAME FINISHED");
                                SceneManager.LoadScene(0);
                            }
                            else if (GameController.getActivePlayer().initialPosition == (Vector2) targetTile.gameObject.transform.position) {
                                Debug.Log("GAME FINISHED");
                                StateMachine.ChangeState(GameController._states[State.STATE_MENU]);
                            }
                        });
                    }
                }
                else {
                    Debug.Log("CHANGING STATE!");
                    StateMachine.ChangeState(GameController._states[State.STATE_SHIFT]);
                    return;
                }
            }
        }
        
        if (Input.GetKeyDown(InputController.INPUT_LEFT)) {
            if (!checkIfSelectedTileInPath(Vector2.left)) return;
            resetTileColor();
            if (selectedTilePos.x == 0) selectedTilePos = new Vector2(GridManager.N - 1, selectedTilePos.y);
            else selectedTilePos += Vector2.left;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.gray);
        }

        if (Input.GetKeyDown(InputController.INPUT_RIGHT)) {
            if (!checkIfSelectedTileInPath(Vector2.right)) return;
            resetTileColor();
            if ((int) selectedTilePos.x == GridManager.N - 1) selectedTilePos = new Vector2(0, selectedTilePos.y);
            else selectedTilePos += Vector2.right;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.grey);
        }

        if (Input.GetKeyDown(InputController.INPUT_UP)) {
            if (!checkIfSelectedTileInPath(Vector2.up)) return;
            resetTileColor();
            if ((int) selectedTilePos.y == GridManager.N - 1) selectedTilePos = new Vector2(selectedTilePos.x, 0);
            else selectedTilePos += Vector2.up;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.grey);
        }

        if (Input.GetKeyDown(InputController.INPUT_DOWN)) {
            if (!checkIfSelectedTileInPath(Vector2.down)) return;
            resetTileColor();
            if ((int) selectedTilePos.y == 0) selectedTilePos = new Vector2(selectedTilePos.x, GridManager.N - 1);
            else selectedTilePos += Vector2.down;
            GameController.gridManager._tiles.Find(t => (Vector2) t.gameObject.transform.position == selectedTilePos).SetColor(Color.gray);
        }
    }

    public void resetTileColor() {
        var currentTile =
            GameController.gridManager._tiles.FirstOrDefault(t => (Vector2) t.gameObject.transform.position == selectedTilePos);
        if (currentTile == null) return;
        currentTile.SetColor(GameController.gridManager._allowedTilePath.Contains(currentTile) ? Color.yellow : Color.white);
    }

    
    public bool checkIfSelectedTileInPath(Vector2 dir) {
        var currentTile = GameController.gridManager._allowedTilePath.FirstOrDefault(t => (Vector2) t.gameObject.transform.position == selectedTilePos + dir);
        return  currentTile != null;
    }

    public override void Exit() {
        Debug.Log("calling exit from moving!");
        Debug.Log("current activePlayer " + GameController.activePlayer);
        clearTilePath();
        _gameController.UpdateActivePlayer();
        _gameController.rotate_function.SetActive(false);
    }
}