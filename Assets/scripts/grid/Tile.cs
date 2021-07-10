using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace grid
{
    public class Tile
    {
        private readonly int _id;
        public readonly GameObject gameObject;
        public readonly Card card;
        public readonly bool[] wall;
        public bool explored;
        public int weight;
        public Tile connectedTile;

        public Tile(int id, GameObject go, string tilePath, bool[] wall, Card card = null) {
            _id = id;
            gameObject = go;
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tilePath);
            this.card = card;
            this.wall = wall;
            explored = false;
            weight = int.MaxValue;
            connectedTile = null;
        }

        public void SetColor(Color color) {
            gameObject.GetComponent<SpriteRenderer>().DOColor(color, 0.4f) ;
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