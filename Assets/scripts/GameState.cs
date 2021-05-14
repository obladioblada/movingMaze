using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    Menu, // when waiting for player before starting the game
    GameStarted, // when the game has started and 
    ArrowSelection, // when a user is selecting an arrow to do the shift
    TileSelection, // when a user is selection a tile to move the piece
}