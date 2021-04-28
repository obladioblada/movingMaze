using UnityEngine;


namespace grid
{
    public class GridManager : MonoBehaviour
    {
        private static int rows = 4;
        private static int columns = 4;
        [SerializeField] private GameObject tile;
        GameObject[,] grid = new GameObject[rows, columns];
        
        enum Axis
        {
            X,
            Y
        }


        // Start is called before the first frame update
        void Awake()
        {
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

        void GenerateGrid()
        {
            for (var x = 0; x < columns; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    GameObject newTile = Instantiate(tile, transform);
                    newTile.AddComponent<Tile>();
                    newTile.transform.position = new Vector2(x, y);
                    newTile.name = y + "," + x;
                    grid[y,x] = newTile;
                }
            }
        }

        void Shift(GameObject t, int index, Axis axis, Vector3 direction)
        {
            if (Axis.X.Equals(axis)) ShiftOnX(index, direction);
            if (Axis.Y.Equals(axis)) ShiftOnY(index, direction);
        }
        
        private void ShiftOnX(int index, Vector3 direction)
        {
            for (var x = 0; x < columns; x++)
            {
                grid[index, x].transform.position += direction;
            }
        }
        
        private void ShiftOnY(int index, Vector3 direction)
        {
            for (var y = 0; y < rows; y++)
            {
                grid[y,index].transform.position += direction;
            }
        }
    }
}