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
        public readonly bool[] wall;
        public bool explored;
        public int weight;
        public Tile connectedTile;

        public Tile(int id, GameObject go, string tilePath, bool[] wall, GameObject treasure = null) {
            _id = id;
            gameObject = go;
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tilePath);
            this.treasure = treasure;
            this.wall = wall;
            explored = false;
            weight = int.MaxValue;
            connectedTile = null;
        }

        public void SetColor(Color color) {
            gameObject.GetComponent<SpriteRenderer>().color = color;
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