using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace grid
{
    public class GridManager : MonoBehaviour
    {
        public const int ROWS = 7;
        public const int COLUMNS = 7;
        [SerializeField] private GameObject tile;
        [SerializeField] private GameObject allowMoving;
        private List<Tile> _tiles;

        enum Axis
        {
            X,
            Y
        }

        void Awake()
        {
            _tiles = new List<Tile>();
            GenerateGrid();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("left"))
            {
                Shift(tile, 0, Axis.X, Vector3.left);
            }

            if (Input.GetKeyDown("right"))
            {
                Shift(tile, 0, Axis.X, Vector3.right);
            }

            if (Input.GetKeyDown("up"))
            {
                Shift(tile, 0, Axis.Y, Vector3.up);
            }

            if (Input.GetKeyDown("down"))
            {
                Shift(tile, 0, Axis.Y, Vector3.down);
            }
        }

        private void GenerateGrid()
        {
            for (var x = 0; x < COLUMNS; x++)
            {
                if (x % 2 == 1)
                {
                    Instantiate(allowMoving, transform).transform
                        .position = new Vector3(x, -1, 0);
                    Instantiate(allowMoving, transform).transform
                        .position = new Vector3(x, ROWS, 0);
                }

                for (var y = 0; y < ROWS; y++)
                {
                    if (y % 2 == 1 && (x == 0 || x == COLUMNS - 1))
                    {
                        Instantiate(allowMoving, transform).transform
                            .position = new Vector3(-1, y, 0);
                        Instantiate(allowMoving, transform).transform
                            .position = new Vector3(COLUMNS, y, 0);
                    }

                    _tiles.Add(new Tile(x + y * 10, SpawnAndStore(x, y)));
                }
            }
        }

        void Shift(GameObject ta, int index, Axis axis, Vector3 direction)
        {
            if (index % 2 != 1) return;
            if (axis == Axis.X)
            {
                foreach (var t in _tiles.Where(x => (int) x.gameObject.transform.position.y == index))
                {
                    t.Shift(direction);
                }
            } else
            {
                foreach (var t in _tiles.Where(x => (int) x.gameObject.transform.position.x == index))
                {
                    t.Shift(direction);
                }
            }
        }

        private GameObject SpawnAndStore(int x, int y)
        {
            var go = Instantiate(
                tile,
                gameObject.transform.position,
                Quaternion.Euler(0, 0, Random.Range(0, 4) * 90),
                transform);
            go.transform.position = new Vector3(x, y, 5);
            go.name = y + "," + x;
            return go;
        }
    }
}