using System;
using grid;
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
        if (Input.GetKeyDown(INPUT_START)) GameController.StartGame();
        if (Input.GetKeyDown(INPUT_LEFT)) GridManager.MoveArrowLeft();
        if (Input.GetKeyDown(INPUT_RIGHT)) GridManager.MoveArrowRight();
        if (Input.GetKeyDown(INPUT_UP)) GridManager.MoveArrowUp();
        if (Input.GetKeyDown(INPUT_DOWN)) GridManager.MoveArrowDown();
        if (Input.GetKeyDown(INPUT_ROTATE)) GridManager.RotateSpareTile();
        if (Input.GetKeyDown(INPUT_INSERT)) GridManager.InsertTile();
        if (Input.GetKeyDown(INPUT_SELECT)) { }
    }
}