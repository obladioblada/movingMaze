using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace grid
{
    public class Tile
    {
        public int _id;
        public GameObject gameObject;

        public Tile(int id, GameObject go)
        {
            _id = id;
            gameObject = go;
            //Set the Texture you assign in the Inspector as the main texture (Or Albedo)
            gameObject.GetComponent<SpriteRenderer>().sprite =
                Resources.Load<Sprite>("Tiles/tile_" + Random.Range(1, 4));
        }


        public void Shift(Vector3 direction)
        {
            gameObject.transform.position += direction;
        }
    }
}