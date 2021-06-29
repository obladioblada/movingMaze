using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Random = UnityEngine.Random;

namespace grid {
    public class GridManager : MonoBehaviour {
        public const int N = 7;
        [SerializeField] private GameObject tile;
        [SerializeField] private GameObject allowMoving;
        [SerializeField] private GameObject treasure;
        public List<Tile> _tiles;
        public List<Tile> _allowedTilePath;
        public Arrow[] _arrows;
        public int _selectedArrowIndex;
        private int _oppositeSelectedArrowIndex;
        private Tile _spare;
        public bool _isFirstTurn;

        // returns the specified tile or a random one otherwise
        private static string GetTilePathWithNumber(int id) {
            return  "Tiles/tile_" + id;
        }
        
        /// <summary>
        /// This static class will generate the wall for each tile and handle accessig on tiles from players
        /// A wall is an array of 4 bool
        /// The order of the bool represents the direction the wall is facing and it is clockwise
        /// NESW. It means that for [false, true, false, true]  a player on a tile can only move horizontally
        /// </summary>
        public bool[] generateWall(int type) {
            Debug.Log("wall type: " + type);
            var wall = type switch {
                1 => new[] {false, true, false, true},
                2 => new[] {true, true, false, true}, 
                3 => new[] {true, true, false, false},
                _ => null};
            return wall;
        }

        public Arrow GetSelectedArrow() {
            return _arrows[_selectedArrowIndex];
        }
        public  Arrow GetOppositeSelectedArrow() {
            return _arrows[_oppositeSelectedArrowIndex];
        }

        public enum ShiftAxis {
            Horizontally,
            Vertically
        }

        private void Awake() {
            _tiles = new List<Tile>();
            _allowedTilePath = new List<Tile>();
            _arrows = new Arrow[N / 2 * 4]; 
             tile.transform.parent = transform;
            _spare = new Tile(-1, tile, GetTilePathWithNumber(1), generateWall(1));
            _tiles.Add(_spare);
            GenerateGrid();
            _selectedArrowIndex = 0;
            _oppositeSelectedArrowIndex = 1;
            _arrows[_selectedArrowIndex].SetColor(Color.yellow);
            _isFirstTurn = true;
        }
        public void InsertTile() {
            if (_oppositeSelectedArrowIndex == _selectedArrowIndex && !_isFirstTurn) {
                Debug.Log("Can't do this because it would revert last change");
                _arrows[_selectedArrowIndex].SetColor(Color.red);
            }
            _isFirstTurn = false;
            _arrows[_oppositeSelectedArrowIndex].SetColor(Color.grey);
            // get the opposite arrow index before assigning new one to prevent reverting last action
            _oppositeSelectedArrowIndex = _selectedArrowIndex % 2 == 0 ? _selectedArrowIndex + 1 : _selectedArrowIndex - 1;
            ShiftTiles(_arrows[_selectedArrowIndex].index, _arrows[_selectedArrowIndex].axes, _arrows[_selectedArrowIndex].direction);
        }

        public void MoveArrowRight() {
            if ( _arrows[_selectedArrowIndex].direction.Equals(Vector3.left)) return;
            if (_selectedArrowIndex ==  MatchPosition.ARROW_NE)   GetArrowAtIndex(MatchPosition.ARROW_EN);
            else if (_selectedArrowIndex == MatchPosition.ARROW_SE) GetArrowAtIndex(MatchPosition.ARROW_ES);
            else if ( _arrows[_selectedArrowIndex].axes.Equals(ShiftAxis.Vertically)) SelectShiftArrowAtIndex(MatchPosition.ShiftUpOrRight);
            else if ( _arrows[_selectedArrowIndex].axes.Equals(ShiftAxis.Horizontally)) SelectShiftArrowAtIndex(MatchPosition.shiftOppositeRightOrUp);
        }

        public void MoveArrowLeft() {
            if ( _arrows[_selectedArrowIndex].direction.Equals(Vector3.right)) return;
            if (_selectedArrowIndex == MatchPosition.ARROW_NW) GetArrowAtIndex(MatchPosition.ARROW_WN);
            else if (_selectedArrowIndex == MatchPosition.ARROW_SW) GetArrowAtIndex(MatchPosition.ARROW_WS);
            else if ( _arrows[_selectedArrowIndex].axes.Equals(ShiftAxis.Vertically)) SelectShiftArrowAtIndex(MatchPosition.ShiftDownOrLeft);
            else if ( _arrows[_selectedArrowIndex].axes.Equals(ShiftAxis.Horizontally)) SelectShiftArrowAtIndex(MatchPosition.shiftOppositeLeftOrDown);
        }
        
        public void MoveArrowUp() {
            if ( _arrows[_selectedArrowIndex].direction.Equals(Vector3.down)) return;
            if (_selectedArrowIndex == MatchPosition.ARROW_WN) GetArrowAtIndex(MatchPosition.ARROW_NW);
            else if (_selectedArrowIndex == MatchPosition.ARROW_EN) GetArrowAtIndex(MatchPosition.ARROW_NE);
            else if ( _arrows[_selectedArrowIndex].direction.Equals(Vector3.up)) SelectShiftArrowAtIndex(MatchPosition.shiftOppositeRightOrUp);
            else SelectShiftArrowAtIndex(MatchPosition.ShiftUpOrRight);
        }
        
        public void MoveArrowDown() {
            if ( _arrows[_selectedArrowIndex].direction == Vector3.up) return;
            if (_selectedArrowIndex == MatchPosition.ARROW_WS) GetArrowAtIndex(MatchPosition.ARROW_SW);
            else if (_selectedArrowIndex == MatchPosition.ARROW_ES) GetArrowAtIndex(MatchPosition.ARROW_SE);
            else if ( _arrows[_selectedArrowIndex].direction == Vector3.down) SelectShiftArrowAtIndex(MatchPosition.shiftOppositeLeftOrDown);
            else SelectShiftArrowAtIndex(MatchPosition.ShiftDownOrLeft);
        }
        
        
        private void SelectShiftArrowAtIndex(int offset) {
            GetArrowAtIndex(_selectedArrowIndex + offset);
        }

        public void RotateSpareTile() {
            Debug.Log("rotating");
            // always prevent concurrent animations
            if (DOTween.IsTweening(_spare.gameObject.transform.transform)) return;
            _spare.gameObject.transform.DORotate(_spare.gameObject.transform.eulerAngles + new Vector3(0, 0, -90), 0.5f);
            updateWall(_spare.wall, 1);
        }

        private void updateWall(bool[] wall, int times) {
            for (var i = 0; i < times; i++) {
                var lastSide = wall[_spare.wall.Length - 1];
                Array.Copy(wall, 0, wall, 1, _spare.wall.Length - 1);
                wall[0] = lastSide;
            }
        }

        // swap color and   
        private void GetArrowAtIndex(int arrowIndex) {
            _arrows[_selectedArrowIndex].SetColor(Color.gray);
            if (_arrows[_selectedArrowIndex] == _arrows[_oppositeSelectedArrowIndex] && !_isFirstTurn) {
                _arrows[_selectedArrowIndex].SetColor(Color.red);
            }
            _selectedArrowIndex = arrowIndex;
            _arrows[_selectedArrowIndex].SetColor(Color.yellow);
            
        }
        private void GenerateGrid() {
            for (var x = 0; x < N; x++) {
                // creating arrows bottom/top side
                if (x % 2 == 1) {
                    _arrows[N + x - 2] = new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Vertically,
                        new Vector3(x, -1, 1), Vector3.up, x);
                    _arrows[N + x - 1] = new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Vertically,
                        new Vector3(x, N, 1), Vector3.down, x);
                }
                for (var y = 0; y < N; y++) {
                    // create tiles player base
                    if ((x == 0 || x == N - 1) && (y == 0 || y == N - 1)) {
                        var go = SpawnTile(x, y);
                        var wall = generateWall(3);
                        go.transform.rotation = new Quaternion(0, 0, 0, 0);
                        switch (x) {
                            case 0 when y == N - 1:
                                go.transform.Rotate(0, 0, 270);
                                updateWall(wall, 1);
                                break;
                            case N - 1 when y == 0:
                                go.transform.Rotate(0, 0, 90);
                                updateWall(wall, 3);
                                break;
                            case N - 1 when y == N - 1:
                                go.transform.Rotate(0, 0, 180);
                                updateWall(wall, 2);
                                break;
                        }
                        Debug.Log("tile at the angle: X-Y" + x + " - " + y);
                        Debug.Log(go.transform.rotation);
                        Debug.Log(wall[0] + "" + wall[1] + "" + wall[2] + "" + wall[3]);
                        _tiles.Add(new Tile(x + y * 10, go, GetTilePathWithNumber(3), wall));
                    } else {
                        // create rest of the grid
                        var tileId = Random.Range(1, 4);
                        var tilePath = GetTilePathWithNumber(tileId);
                        var go = SpawnTile(x, y);
                        var rotationRandomId = Random.Range(0, 4);
                        go.transform.Rotate(0, 0, rotationRandomId * -90); 
                        var wall = generateWall(tileId);
                        updateWall(wall, rotationRandomId);
                        GameObject t = null;
                        if (Random.Range(1, 10) < 3) {
                            t = Instantiate(treasure, go.transform.position, go.transform.rotation, go.transform);
                            t.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                        }
                        _tiles.Add( new Tile(x + y * 10, go, tilePath, wall,t));
                    }
                    // creating arrows right/left side
                    if (y % 2 == 1 && x == 0) {
                        _arrows[y - 1] = (new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Horizontally,
                            new Vector3(-1, y, 1), Vector3.right, y));
                        _arrows[y] = (new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Horizontally,
                            new Vector3(N, y, 1), Vector3.left, y));
                    }
                }
            }
            
            Destroy(allowMoving);
            Destroy(treasure);
        }
        
        private void ShiftTiles(int index, ShiftAxis axis, Vector3 direction) {
            if (index % 2 != 1) return;
            var shiftSequence = DOTween.Sequence();
            foreach (var t in _tiles.Where(x => axis == ShiftAxis.Horizontally ? !x.IsAtRow(index) : !x.IsAtColumn(index))) {
               t.SetColor(Color.gray);
            }
            foreach (var t in _tiles.Where(x => axis == ShiftAxis.Horizontally ? x.IsAtRow(index) : x.IsAtColumn(index))) {
                var endPosition =  t.gameObject.transform.position + direction;
                checkIfPlayerObjectShifts(t, endPosition);
                shiftSequence.Append(t.gameObject.transform.DOMove(endPosition, 0.05f));
            }
            shiftSequence.OnComplete(() => {
                if (axis == ShiftAxis.Horizontally) {
                    var leftOver = _tiles.First(t =>
                        direction == Vector3.right ? MatchPosition.LastInRow(t, index) : MatchPosition.FirstInRow(t, index));
                    Debug.Log(leftOver);
                    SwapTiles(leftOver, direction == Vector3.right ? MatchPosition.BeforeFirstInRow(index) : MatchPosition.AfterLastInRow(index));
                }
                else {
                    var leftOver = _tiles.First(t =>
                        direction == Vector3.up ? MatchPosition.LastInColumn(t, index) : MatchPosition.FirstInColumn(t, index));
                    Debug.Log(leftOver);
                    SwapTiles(leftOver, direction == Vector3.up ? MatchPosition.BeforeFirstInColumn(index) : MatchPosition.AfterLastInColumn(index));
                }
            });
        }

        private void checkIfPlayerObjectShifts(Tile t, Vector3 endPosition) {
            foreach (var player in GameController._players) {
                var playerPos = player.playerGameObject.transform.position;
                if ((Vector2) playerPos != (Vector2) t.gameObject.transform.position) continue;
                Debug.Log("OK I SHOULD MOVE PLAYER NOW");
                var newPlayerPos = new Vector3(endPosition.x, endPosition.y, playerPos.z);
                if (newPlayerPos.x >= N) {
                    newPlayerPos.x = 0;
                } else if (newPlayerPos.x < 0){
                    newPlayerPos.x = N -1 ;
                }
                if (newPlayerPos.y >= N) {
                    newPlayerPos.y = 0;
                } else if (newPlayerPos.y < 0){
                    newPlayerPos.y = N -1 ;
                }
                DOTween.Sequence()
                    .Append(player.playerGameObject.transform.DOMove(newPlayerPos, 0.05f));
            }
        }

        private void SwapTiles(Tile newSpare, Vector2 oldSparePosition) {
            var swapSequence = DOTween.Sequence();
            swapSequence.Append(newSpare.gameObject.transform.DOMove(_spare.gameObject.transform.position, 0.5f));
            swapSequence.Append(_spare.gameObject.transform.DOMove(oldSparePosition, 0.5f));
            swapSequence.OnComplete(() => {
                foreach (var t in _tiles) {
                    t.SetColor(Color.white);
                }
                _spare = newSpare;
                CalculatePath();
            });
        }

        private GameObject SpawnTile(int x, int y) {
            var go = Instantiate(
                tile, transform);
            go.transform.position = new Vector3(x, y, 0);
            go.name = x + "," + y;
            return go;
        }

        void CalculatePath() {
            var player = GameController.getActivePlayer();
            // todo take player position and go through the tiles to match paths
            Debug.Log("calculating path for player" + player.name);
            Debug.Log("POS: " + player.playerGameObject.transform.position);
            var startingTile = _tiles.Find(t =>
                (Vector2) t.gameObject.transform.position == (Vector2) player.playerGameObject.transform.position);
            Debug.Log(startingTile);
            Debug.Log("on TILE " + startingTile.gameObject.transform.position);
            Debug.Log("with wall:");
            startingTile.weight = 0;
            _allowedTilePath.Add(startingTile);
            recursiveExplorePlayerGrid(startingTile, startingTile.weight + 1);
        }
        private void recursiveExplorePlayerGrid(Tile startingTile, int weight) {
            if (startingTile.explored) return;
            startingTile.explored = true;
            startingTile.SetColor(Color.yellow);
            Vector2[] dir = {Vector2.up, Vector2.right, Vector2.down, Vector2.left};
            int[] TileToCheckDir = {2, 3, 0, 1}; // SWNE
            Vector2 startingTilePos = startingTile.gameObject.transform.position;
            // i = direction NESW
            Debug.Log("starting checking adjacent tiles"); 
            for (var i = 0; i < 4; i++) {
                // check the walls and if true it means I could theoretically move to new tile
                var nextTileToCheck = _tiles.FirstOrDefault(t => (Vector2) t.gameObject.transform.position == startingTilePos + dir[i]);
                //check if path on i direction 
                if (startingTile.wall[i] && nextTileToCheck != null && nextTileToCheck.wall[TileToCheckDir[i]]) {
                    // there is connection between starting Tile and nextTileToCheck
                    if (nextTileToCheck.weight > weight) {
                        Debug.Log("setting W:" + weight); 
                        Debug.Log("for:" + nextTileToCheck.gameObject.transform.position); 
                        nextTileToCheck.weight = weight;
                        if (nextTileToCheck.connectedTile == null || nextTileToCheck.connectedTile.weight > startingTile.weight) {
                            nextTileToCheck.connectedTile = startingTile;
                        }
                    }
                    _allowedTilePath.Add(nextTileToCheck);
                    recursiveExplorePlayerGrid(nextTileToCheck, nextTileToCheck.weight + 1);
                }
            }
        }

        public void MovePlayer(Player player, Vector2 pos) {
            player.playerGameObject.transform.position = new Vector3(pos.x, pos.y, player.playerGameObject.transform.position.z);
        }
    }
}