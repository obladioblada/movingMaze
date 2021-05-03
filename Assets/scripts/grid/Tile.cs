using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace grid
{
    public class Tile
    {
        private readonly int _id;
        public readonly GameObject gameObject;
        public readonly GameObject treasure;

        public Tile(int id, GameObject go, String tilePath) : this(id, go, tilePath, null) { }

        public Tile(int id, GameObject go, String tilePath, GameObject treasure) {
            _id = id;
            gameObject = go;
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tilePath);
            this.treasure = treasure;
        }


        public void Shift(Vector3 direction)
        {
            gameObject.transform.position += direction;
        }

        public bool IsAtRow(int row) {
            return (int) gameObject.transform.position.y == row;
        }
        
        public bool IsAtColumn(int col) {
            return (int) gameObject.transform.position.x == col;
        }
    }
}