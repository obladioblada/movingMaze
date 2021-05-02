using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace grid {
    public class GridManager : MonoBehaviour {
        private const int ROWS = 7;
        private const int COLUMNS = 7;
        [SerializeField] private GameObject tile;
        [SerializeField] private GameObject allowMoving;
        private List<Tile> _tiles;
        private Tile spare;

        private enum Axis {
            X,
            Y
        }

        void Awake() {
            _tiles = new List<Tile>();
            tile.transform.parent = transform;
            spare = new Tile(-1, tile, GetRandomTilePath());
            _tiles.Add(spare);
            GenerateGrid();
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown("left")) {
                Shift(1, Axis.X, Vector3.left);
            }

            if (Input.GetKeyDown("right")) {
                Shift(1, Axis.X, Vector3.right);
            }

            if (Input.GetKeyDown("up")) {
                Shift(1, Axis.Y, Vector3.up);
            }
            if (Input.GetKeyDown("down")) {
                Shift(1, Axis.Y, Vector3.down);
            }
            if (Input.GetKeyDown("r")) {
               spare.gameObject.transform.Rotate(0,0, 90);
            }
        }

         void GenerateGrid() {
            for (var x = 0; x < COLUMNS; x++) {
                if (x % 2 == 1) {
                    Instantiate(allowMoving, transform).transform
                        .position = new Vector3(x, -1, 1);
                    Instantiate(allowMoving, transform).transform
                        .position = new Vector3(x, ROWS, 1);
                }

                for (var y = 0; y < ROWS; y++) {
                    GameObject go = SpawnTile(x, y);
                    var tilePath = GetRandomTilePath();
                    if ((x == 0 || x == COLUMNS - 1) && (y == 0 || y == ROWS - 1)) {
                        tilePath = "Tiles/tile_3_blue";
                    }

                    if (y % 2 == 1 && (x == 0 || x == COLUMNS - 1)) {
                        Instantiate(allowMoving, transform).transform
                            .position = new Vector3(-1, y, 1);
                        Instantiate(allowMoving, transform).transform
                            .position = new Vector3(COLUMNS, y, 1);
                    }

                    _tiles.Add(new Tile(x + y * 10, go, tilePath));
                }
            }
        }

        private static string GetRandomTilePath() {
            return "Tiles/tile_" + Random.Range(1, 4);
        }

        void Shift(int index, Axis axis, Vector3 direction) {
            if (index % 2 != 1) return;
            if (axis == Axis.X) {
                if (direction == Vector3.right) {
                    var leftOver = _tiles.First(t => t.gameObject.transform.position == new Vector3(COLUMNS - 1, index ));
                    leftOver.gameObject.transform.position = spare.gameObject.transform.position;
                    spare.gameObject.transform.position = new Vector2(-1, index);
                    spare = leftOver;
                }
                else if (direction  == Vector3.left){
                    var leftOver = _tiles.Find(t => t.gameObject.transform.position == new Vector3(0, index));
                    leftOver.gameObject.transform.position = spare.gameObject.transform.position;
                    spare.gameObject.transform.position = new Vector2(COLUMNS, index);
                    spare = leftOver;
                }

                foreach (var t in _tiles.Where(x => (int) x.gameObject.transform.position.y == index)) {
                    t.Shift(direction);
                }
            } else {
                if (direction == Vector3.up) {
                    var leftOver = _tiles.Find(t => t.gameObject.transform.position == new Vector3(index, ROWS - 1));
                    leftOver.gameObject.transform.position = spare.gameObject.transform.position;
                    spare.gameObject.transform.position = new Vector2(index, -1);
                    spare = leftOver;
                }
                else if (direction == Vector3.down) {
                    var leftOver = _tiles.Find(t => t.gameObject.transform.position == new Vector3(index, 0));
                    leftOver.gameObject.transform.position = spare.gameObject.transform.position;
                    spare.gameObject.transform.position = new Vector2(index, ROWS);
                }

                foreach (var t in _tiles.Where(x => (int) x.gameObject.transform.position.x == index)) {
                    t.Shift(direction);
                }
            }
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