using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;


namespace grid {
    public class GridManager : MonoBehaviour {
        public const int Rows = 7;
        public const int Columns = 7;
        [SerializeField] private GameObject tile;
        [SerializeField] private GameObject allowMoving;
        [SerializeField] private GameObject treasure;
        private List<Tile> _tiles;
        private Tile _spare;

        private static string GetRandomTilePath() {
            return "Tiles/tile_" + Random.Range(1, 4);
        }

        private enum ShiftAxis {
            Horizontally,
            Vertically
        }

        void Awake() {
            _tiles = new List<Tile>();
            tile.transform.parent = transform;
            _spare = new Tile(-1, tile, GetRandomTilePath());
            _tiles.Add(_spare);
            GenerateGrid();
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown("left")) {
                Shift(1, ShiftAxis.Horizontally, Vector3.left);
            }

            if (Input.GetKeyDown("right")) {
                Shift(1, ShiftAxis.Horizontally, Vector3.right);
            }

            if (Input.GetKeyDown("up")) {
                Shift(1, ShiftAxis.Vertically, Vector3.up);
            }

            if (Input.GetKeyDown("down")) {
                Shift(1, ShiftAxis.Vertically, Vector3.down);
            }

            if (Input.GetKeyDown("r")) {
                _spare.gameObject.transform.Rotate(0, 0, 90);
            }
        }

        void GenerateGrid() {
            for (var x = 0; x < Columns; x++) {
                if (x % 2 == 1) {
                    Instantiate(allowMoving, transform).transform
                        .position = new Vector3(x, -1, 1);
                    Instantiate(allowMoving, transform).transform
                        .position = new Vector3(x, Rows, 1);
                }

                for (var y = 0; y < Rows; y++) {
                    var go = SpawnTile(x, y);
                    var tilePath = GetRandomTilePath();
                    if ((x == 0 || x == Columns - 1) && (y == 0 || y == Rows - 1)) {
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

                    if (y % 2 == 1 && (x == 0 || x == Columns - 1)) {
                        Instantiate(allowMoving, transform).transform
                            .position = new Vector3(-1, y, 1);
                        Instantiate(allowMoving, transform).transform
                            .position = new Vector3(Columns, y, 1);
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