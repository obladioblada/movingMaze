using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;


namespace grid {
    public class GridManager : MonoBehaviour {
        public const int N = 7;
        [SerializeField] private GameObject tile;
        [SerializeField] private GameObject allowMoving;
        [SerializeField] private GameObject treasure;
        private static List<Tile> _tiles;
        private static Arrow[] _arrows;
        private static int _selectedArrowIndex;
        private static int _oppositeSelectedArrowIndex;
        private static Tile _spare;
        private static bool _isFirstTurn;
        
        // returns the specified tile or a random one otherwise
        private static string GetTilePathWithNumber([CanBeNull] string tileNumber) {
            if (tileNumber != null ) return  "Tiles/tile_" + tileNumber;
            return "Tiles/tile_" + Random.Range(1, 4);
        }

        public enum ShiftAxis {
            Horizontally,
            Vertically
        }

        private void Awake() {
            _tiles = new List<Tile>();
            _arrows = new Arrow[N / 2 * 4];
            tile.transform.parent = transform;
            _spare = new Tile(-1, tile, GetTilePathWithNumber(null));
            _tiles.Add(_spare);
            GenerateGrid();
            _selectedArrowIndex = 0;
            _oppositeSelectedArrowIndex = 1;
            _arrows[_selectedArrowIndex].SetColor(Color.yellow);
            _isFirstTurn = true;
        }
        public static void InsertTile() {
            if (_oppositeSelectedArrowIndex == _selectedArrowIndex && !_isFirstTurn) {
                Debug.Log("Can't do this because it would revert last change");
                _arrows[_selectedArrowIndex].SetColor(Color.red);
            } else {
                _isFirstTurn = false;
                // get the opposite arrow index before assigning new one to prevent reverting last action
                _oppositeSelectedArrowIndex = _selectedArrowIndex % 2 == 0 ? _selectedArrowIndex + 1 : _selectedArrowIndex - 1;
                ShiftTiles( _arrows[_selectedArrowIndex].index,  _arrows[_selectedArrowIndex].axes,  _arrows[_selectedArrowIndex].direction);
            }
        }

        public static void MoveArrowRight() {
            if ( _arrows[_selectedArrowIndex].direction.Equals(Vector3.left)) return;
            if (_selectedArrowIndex ==  MatchPosition.ARROW_NE)   GetArrowAtIndex(MatchPosition.ARROW_EN);
            else if (_selectedArrowIndex == MatchPosition.ARROW_SE) GetArrowAtIndex(MatchPosition.ARROW_ES);
            else if ( _arrows[_selectedArrowIndex].axes.Equals(ShiftAxis.Vertically)) ShiftArrow(MatchPosition.ShiftUpOrRight);
            else if ( _arrows[_selectedArrowIndex].axes.Equals(ShiftAxis.Horizontally)) ShiftArrow(MatchPosition.shiftOppositeRightOrUp);
        }

        public static void MoveArrowLeft() {
            if ( _arrows[_selectedArrowIndex].direction.Equals(Vector3.right)) return;
            if (_selectedArrowIndex == MatchPosition.ARROW_NW) GetArrowAtIndex(MatchPosition.ARROW_WN);
            else if (_selectedArrowIndex == MatchPosition.ARROW_SW) GetArrowAtIndex(MatchPosition.ARROW_WS);
            else if ( _arrows[_selectedArrowIndex].axes.Equals(ShiftAxis.Vertically)) ShiftArrow(MatchPosition.ShiftDownOrLeft);
            else if ( _arrows[_selectedArrowIndex].axes.Equals(ShiftAxis.Horizontally)) ShiftArrow(MatchPosition.shiftOppositeLeftOrDown);
        }
        
        public static void MoveArrowUp() {
            if ( _arrows[_selectedArrowIndex].direction.Equals(Vector3.down)) return;
            if (_selectedArrowIndex == MatchPosition.ARROW_WN) GetArrowAtIndex(MatchPosition.ARROW_NW);
            else if (_selectedArrowIndex == MatchPosition.ARROW_EN) GetArrowAtIndex(MatchPosition.ARROW_NE);
            else if ( _arrows[_selectedArrowIndex].direction.Equals(Vector3.up)) ShiftArrow(MatchPosition.shiftOppositeRightOrUp);
            else ShiftArrow(MatchPosition.ShiftUpOrRight);
        }
        
        public static void MoveArrowDown() {
            if ( _arrows[_selectedArrowIndex].direction == Vector3.up) return;
            if (_selectedArrowIndex == MatchPosition.ARROW_WS) GetArrowAtIndex(MatchPosition.ARROW_SW);
            else if (_selectedArrowIndex == MatchPosition.ARROW_ES) GetArrowAtIndex(MatchPosition.ARROW_SE);
            else if ( _arrows[_selectedArrowIndex].direction == Vector3.down) ShiftArrow(MatchPosition.shiftOppositeLeftOrDown);
            else ShiftArrow(MatchPosition.ShiftDownOrLeft);
        }
        
        
        private static void ShiftArrow(int offset) {
            GetArrowAtIndex(_selectedArrowIndex + offset);
        }

        public static void RotateSpareTile() {
            Debug.Log("rotating");
            _spare.gameObject.transform.Rotate(0, 0, 90);
        }
        
        // swap color and   
        private static void GetArrowAtIndex(int arrowIndex) {
            _arrows[_selectedArrowIndex].SetColor(Color.gray);
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
                    var go = SpawnTile(x, y);
                    string tilePath;
                    if ((x == 0 || x == N - 1) && (y == 0 || y == N - 1)) {
                        tilePath = GetTilePathWithNumber("3");
                        go.transform.rotation = new Quaternion(0, 0, 0, 0);
                        switch (x) {
                            case 0 when y == N - 1:
                                go.transform.Rotate(0, 0, -90);
                                break;
                            case N - 1 when y == 0:
                                go.transform.Rotate(0, 0, 90);
                                break;
                            case N - 1 when y == N - 1:
                                go.transform.Rotate(0, 0, 180);
                                break;
                        }
                        _tiles.Add(new Tile(x + y * 10, go, tilePath));
                    } else {
                        tilePath =  GetTilePathWithNumber(null);
                        GameObject t = null;
                        if (Random.Range(1, 10) < 3) {
                            t = Instantiate(treasure, go.transform.position, go.transform.rotation, go.transform);
                            t.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                        }
                        _tiles.Add(new Tile(x + y * 10, go, tilePath, t));
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
        
        private static void ShiftTiles(int index, ShiftAxis axis, Vector3 direction) {
            if (index % 2 != 1) return;
            if (axis == ShiftAxis.Horizontally) {
                var leftOver = _tiles.First(t =>
                    direction == Vector3.right ? MatchPosition.LastInRow(t, index) : MatchPosition.FirstInRow(t, index));
                SwapTiles(leftOver, direction == Vector3.right ? MatchPosition.BeforeFirstInRow(index) : MatchPosition.AfterLastInRow(index));
            }
            else {
                var leftOver = _tiles.First(t =>
                    direction == Vector3.up ? MatchPosition.LastInColumn(t, index) : MatchPosition.FirstInColumn(t, index));
                SwapTiles(leftOver, direction == Vector3.up ? MatchPosition.BeforeFirstInColumn(index) : MatchPosition.AfterLastInColumn(index));
            }

            foreach (var t in _tiles.Where(x => axis == ShiftAxis.Horizontally ? x.IsAtRow(index) : x.IsAtColumn(index))) {
                t.Shift(direction);
            }
        }

        private static void SwapTiles(Tile newSpare, Vector2 oldSparePosition) {
            newSpare.gameObject.transform.position = _spare.gameObject.transform.position;
            _spare.gameObject.transform.position = oldSparePosition;
            _spare = newSpare;
        }

        private GameObject SpawnTile(int x, int y) {
            var go = Instantiate(
                tile,
                gameObject.transform.position,
                Quaternion.Euler(0, 0, Random.Range(0, 4) * 90),
                transform);
            go.transform.position = new Vector3(x, y, 0);
            go.name = y + "," + x;
            return go;
        }
    }
}