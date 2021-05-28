using System;
using grid;
using NDream.AirConsole;
using UnityEngine;

public class InputController : MonoBehaviour {
    // Update is called once per frame
    public const KeyCode INPUT_LEFT = KeyCode.LeftArrow;
    public const KeyCode INPUT_RIGHT = KeyCode.RightArrow;
    public const KeyCode INPUT_UP = KeyCode.UpArrow;
    public const KeyCode INPUT_DOWN = KeyCode.DownArrow;
    public const KeyCode INPUT_ROTATE = KeyCode.R;
    public const KeyCode INPUT_INSERT= KeyCode.I;
    public const KeyCode INPUT_START = KeyCode.S;
    public const KeyCode INPUT_SELECT = KeyCode.Space;

    private void Update() {
        if (Input.GetKeyDown(INPUT_INSERT)) {
            switch (GameController.activeState) {
                case State.STATE_MENU: break;
                case State.STATE_MOVE:
                    GameController.activeState = State.STATE_SHIFT;
                    Debug.Log("state is SHIFT");
                    GameController.sendMessageToPlayer( 
                        new {  
                            action = "UPDATE_STATE",
                            active = true,
                            state = State.STATE_SHIFT,
                        }, GameController.activePlayer + 1);
                    break;
                case State.STATE_SHIFT:
                    GridManager.InsertTile();
                    GameController.activeState =State.STATE_MOVE;
                    Debug.Log("state is MOVE");
                    GameController.sendMessageToPlayer( 
                        new {  
                            action = "UPDATE_STATE",
                            active = true,
                            state = "move"
                        }, GameController.activePlayer);
                    GridManager._tiles[0].gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                    break;
            }
        };
        if (Input.GetKeyDown(INPUT_SELECT)) { }
    }
}