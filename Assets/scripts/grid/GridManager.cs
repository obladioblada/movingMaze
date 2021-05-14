using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace grid {
    public class GridManager : MonoBehaviour {
        public const int N = 9;
        [SerializeField] private GameObject tile;
        [SerializeField] private GameObject allowMoving;
        [SerializeField] private GameObject treasure;
        private List<Tile> _tiles;
        private List<Arrow> _arrows;
        private Dictionary<int, Arrow[]> arrowsMap; 
        private Arrow _currentSelectedArrow;
        private int _selectedArrowIndex;
        private int _oppositeSelectedArrowIndex;
        private Tile _spare;
        private bool _isFirstTurn;
        private static string GetRandomTilePath() {
            return "Tiles/tile_" + Random.Range(1, 4);
        }

        public enum ShiftAxis {
            Horizontally,
            Vertically
        }

        void Awake() {
            _tiles = new List<Tile>();
            _arrows = new List<Arrow>();
            tile.transform.parent = transform;
            _spare = new Tile(-1, tile, GetRandomTilePath());
            _tiles.Add(_spare);
            GenerateGrid();
            _selectedArrowIndex = 0;
            _oppositeSelectedArrowIndex = 1;
            _currentSelectedArrow = _arrows[_selectedArrowIndex];
            _currentSelectedArrow.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            _isFirstTurn = true;
        }

        void Update() {
            if (Input.GetKeyDown("left")) {
                if (_currentSelectedArrow.direction.Equals(Vector3.right)) return;
                if (_selectedArrowIndex == N) {
                    SelectRowOrColumn(N - 3);
                } else if (_selectedArrowIndex == N - 1) {
                    SelectRowOrColumn(0);
                } else if (_currentSelectedArrow.axes.Equals(ShiftAxis.Vertically)) SelectRowOrColumn(_selectedArrowIndex - 2);
                else if (_currentSelectedArrow.axes.Equals(ShiftAxis.Horizontally)) SelectRowOrColumn(_selectedArrowIndex - 1);
            }

            if (Input.GetKeyDown("right")) {
                if (_currentSelectedArrow.direction.Equals(Vector3.left)) return;
                // 4 -> 7
                if (_selectedArrowIndex ==  2 * N - 3)   SelectRowOrColumn(N - 2);
                else if (_selectedArrowIndex ==  2 * N - 4) SelectRowOrColumn(1);
                else if (_currentSelectedArrow.axes.Equals(ShiftAxis.Vertically)) SelectRowOrColumn(_selectedArrowIndex + 2);
                else if (_currentSelectedArrow.axes.Equals(ShiftAxis.Horizontally)) SelectRowOrColumn(_selectedArrowIndex + 1);
            }

            if (Input.GetKeyDown("up")) {
                //do nothing if already at the top
                if (_currentSelectedArrow.direction.Equals(Vector3.down)) return;
                // 4 -> 7
                if (_selectedArrowIndex == N - 3) {
                    SelectRowOrColumn(N);
                } else if (_selectedArrowIndex == N - 2) {
                    SelectRowOrColumn(2 * N - 3);
                } else if (_currentSelectedArrow.direction.Equals(Vector3.up)) {
                    //move vertically up =
                    SelectRowOrColumn(_selectedArrowIndex + 1);
                } //move vertically up ||
                else SelectRowOrColumn(_selectedArrowIndex + 2);
            }
            if (Input.GetKeyDown("down")) {
                //do nothing if already at the bottom
                if (_currentSelectedArrow.direction == Vector3.up) return;
                // 0-> 6
                if (_selectedArrowIndex == 0) {
                    SelectRowOrColumn(N - 1);
                }
                // 1 -> 10
                else if (_selectedArrowIndex == 1) {
                    SelectRowOrColumn(2 * N - 4);
                }
                else if (_currentSelectedArrow.direction == Vector3.down) {
                    //move vertically down =
                    SelectRowOrColumn(_selectedArrowIndex - 1);
                } //move vertically down ||
                else SelectRowOrColumn(_selectedArrowIndex - 2);
            }

            if (Input.GetKeyDown("r")) {
                _spare.gameObject.transform.Rotate(0, 0, 90);
            }

            // start game
            if (Input.GetKeyDown("s")) {
                Debug.Log("should start game..");
                GameController.StartGame();
            }

            // insert tile and shift row/column
            if (Input.GetKeyDown("i")) {
                if (_oppositeSelectedArrowIndex == _selectedArrowIndex && !_isFirstTurn) {
                    Debug.Log("Can't do this because it would revert last change");
                    _arrows[_selectedArrowIndex].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                }
                else {
                    _isFirstTurn = false;
                    // get the opposite arrow index before assigning new one to prevent reverting last action
                    _oppositeSelectedArrowIndex = _selectedArrowIndex % 2 == 0 ? _selectedArrowIndex + 1 : _selectedArrowIndex - 1;
                    Shift(_currentSelectedArrow.index, _currentSelectedArrow.axes, _currentSelectedArrow.direction);
                }
            }
        }
        
        // move   
        private void SelectRowOrColumn(int arrowIndex) {
            _arrows[_selectedArrowIndex].gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
            _selectedArrowIndex = arrowIndex;
            _currentSelectedArrow = _arrows[_selectedArrowIndex];
            _currentSelectedArrow.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        void GenerateGrid() {
            for (var x = 0; x < N; x++) {
                if (x % 2 == 1) {
                    _arrows.Add( new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Vertically,
                        new Vector3(x, -1, 1), Vector3.up, x));
                    _arrows.Add(new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Vertically,
                        new Vector3(x, N, 1), Vector3.down, x));
                }

                for (var y = 0; y < N; y++) {
                    var go = SpawnTile(x, y);
                    var tilePath = GetRandomTilePath();
                    if ((x == 0 || x == N - 1) && (y == 0 || y == N - 1)) {
                        tilePath = "Tiles/tile_3_blue";
                        _tiles.Add(new Tile(x + y * 10, go, tilePath));
                    }
                    else if (Random.Range(1, 10) < 3) {
                        var t = Instantiate(treasure, go.transform.position, go.transform.rotation, go.transform);
                        t.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                        _tiles.Add(new Tile(x + y * 10, go, tilePath, t));
                    }
                    else {
                        _tiles.Add(new Tile(x + y * 10, go, tilePath));
                    }

                    if (y % 2 == 1 && x == 0) {
                        _arrows.Add(new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Horizontally,
                            new Vector3(-1, y, 1), Vector3.right, y));
                        _arrows.Add(new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Horizontally,
                            new Vector3(N, y, 1), Vector3.left, y));
                    }
                }
            }
        }

        private void Shift(int index, ShiftAxis axis, Vector3 direction) {
            if (index % 2 != 1) return;
            if (axis == ShiftAxis.Horizontally) {
                var leftOver = _tiles.First(t =>
                    direction == Vector3.right ? MatchPosition.LastInRow(t, index) : MatchPosition.FirstInRow(t, index));
                Swap(leftOver, direction == Vector3.right ? MatchPosition.BeforeFirstInRow(index) : MatchPosition.AfterLastInRow(index));
            }
            else {
                var leftOver = _tiles.First(t =>
                    direction == Vector3.up ? MatchPosition.LastInColumn(t, index) : MatchPosition.FirstInColumn(t, index));
                Swap(leftOver, direction == Vector3.up ? MatchPosition.BeforeFirstInColumn(index) : MatchPosition.AfterLastInColumn(index));
            }

            foreach (var t in _tiles.Where(x => axis == ShiftAxis.Horizontally ? x.IsAtRow(index) : x.IsAtColumn(index))) {
                t.Shift(direction);
            }
        }

        private void Swap(Tile newSpare, Vector2 oldSparePosition) {
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