using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace grid {
    public class GridManager : MonoBehaviour {
        public const int Y = 7;
        public const int X = 7;
        [SerializeField] private GameObject tile;
        [SerializeField] private GameObject allowMoving;
        [SerializeField] private GameObject treasure;
        private List<Tile> _tiles;
        private List<Arrow> _arrows;
        private int _selectedArrow;
        private Tile _spare;
        private Vector3 _lastDirection;
        private int _lastIndex;

        private static string GetRandomTilePath() {
            return "Tiles/tile_" + Random.Range(1, 4);
        }

        public enum ShiftAxis {
            Horizontally,
            Vertically
        }

        void Awake() {
            _tiles = new List<Tile>();
            //create Y arrays
            _arrows = new List<Arrow>();
            tile.transform.parent = transform;
            _spare = new Tile(-1, tile, GetRandomTilePath());
            _tiles.Add(_spare);
            GenerateGrid();
            _selectedArrow = 0;
            _lastIndex = 0;
            _lastDirection = _arrows[_selectedArrow].direction; 
            _arrows[_selectedArrow].gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown("left")) {
                SelectRowOrColumn(_selectedArrow - 1);
            }

            if (Input.GetKeyDown("right")) {
                SelectRowOrColumn(_selectedArrow + 1);
                //Shift(1, ShiftAxis.Horizontally, Vector3.right);
                Debug.Log(_selectedArrow);
            }

            if (Input.GetKeyDown("up")) {
            }

            if (Input.GetKeyDown("down")) {
            }

            if (Input.GetKeyDown("r")) {
                _spare.gameObject.transform.Rotate(0, 0, 90);
            }
            if (Input.GetKeyDown("s")) {
                // start gamr an
                Debug.Log("should start game..");
                GameController.StartGame();
            }
            if (Input.GetKeyDown("i")) {
                // insert tile and shift row/column
                var newindex = (int) (_arrows[_selectedArrow].axes == ShiftAxis.Horizontally
                    ? _arrows[_selectedArrow].gameObject.transform.position.y
                    : _arrows[_selectedArrow].gameObject.transform.position.x);

                Debug.Log("_lastIndex" + _lastIndex);
                Debug.Log("Last direction " + _lastDirection);
                Debug.Log("_newIndex" + newindex);
                Debug.Log("new direction " + -_arrows[_selectedArrow].direction);
                if (newindex == _lastIndex && _lastDirection == -_arrows[_selectedArrow].direction) {
                    Debug.Log("Can't this because it would revert last change");
                } else {
                    Shift(newindex, _arrows[_selectedArrow].axes, _arrows[_selectedArrow].direction);
                }
                
            }
        }

        private void SelectRowOrColumn(int index) {
            Debug.Log(index);
            _arrows[_selectedArrow].gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
            _selectedArrow = index;
            if (_selectedArrow < 0) _selectedArrow = _arrows.Count - 1;
            if (_selectedArrow > _arrows.Count - 1) _selectedArrow = 0;
            _arrows[_selectedArrow].gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        void GenerateGrid() {
            for (var x = 0; x < X; x++) {
                if (x % 2 == 1) {
                    _arrows.Add(new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Vertically, new Vector3(x, -1, 1), Vector3.up));
                    _arrows.Add(new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Vertically, new Vector3(x, Y, 1),  Vector3.down));
                }

                for (var y = 0; y < Y; y++) {
                    var go = SpawnTile(x, y);
                    var tilePath = GetRandomTilePath();
                    if ((x == 0 || x == X - 1) && (y == 0 || y == Y - 1)) {
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
                        Debug.Log("Arrow:" + x + ":" + y);
                        _arrows.Add(new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Horizontally, new Vector3(-1, y, 1),Vector3.right ));
                        _arrows.Add(new Arrow(Instantiate(allowMoving, transform), ShiftAxis.Horizontally,  new Vector3(X, y, 1),Vector3.left ));
                    }
                }
            }
            Debug.Log(_arrows.Count);
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
            foreach (var t in _tiles.Where(x => axis == ShiftAxis.Horizontally ? x.IsAtRow(index):x.IsAtColumn(index) )) {
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