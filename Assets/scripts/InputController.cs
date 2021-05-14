using System;
using grid;
using UnityEngine;

public class InputController : MonoBehaviour {
    // Update is called once per frame
    private const KeyCode INPUT_LEFT = KeyCode.LeftArrow;
    private const KeyCode INPUT_RIGHT = KeyCode.RightArrow;
    private const KeyCode INPUT_UP = KeyCode.UpArrow;
    private const KeyCode INPUT_DOWN = KeyCode.DownArrow;
    private const KeyCode INPUT_ROTATE = KeyCode.R;
    private const KeyCode INPUT_INSERT= KeyCode.I;
    private const KeyCode INPUT_START = KeyCode.S;
    private const KeyCode INPUT_SELECT = KeyCode.Space;

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