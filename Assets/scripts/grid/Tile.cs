using System;
using UnityEngine;

namespace grid
{
    public class Tile : TileBase
    {
        private void Awake()
        {
           Debug.Log(this);
        }
    }
}