using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace grid
{
    public class Tile
    {
        public int _id;
        public GameObject gameObject;

        public Tile(int id, GameObject gameObject) {
            _id = id;
            this.gameObject = gameObject;
        }

        public Tile(int id, GameObject go, String tilePath)
        {
            _id = id;
            gameObject = go;
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tilePath);
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

        public Tile Clone() {
            return new Tile(_id, gameObject);
        }
    }
}