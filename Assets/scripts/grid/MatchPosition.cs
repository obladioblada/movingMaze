using System;
using UnityEngine;

namespace grid {
    public static class MatchPosition {
        
        public static readonly Func<Tile, int, bool> LastInRow = (t, index) =>
            t.gameObject.transform.position == new Vector3(GridManager.X - 1, index);

        public static readonly Func<Tile, int, bool> FirstInRow = (t, index) =>
            t.gameObject.transform.position == new Vector3(0, index);

        public static readonly Func<Tile, int, bool> LastInColumn = (t, index) =>
            t.gameObject.transform.position == new Vector3(index, GridManager.Y - 1);

        public static readonly Func<Tile, int, bool> FirstInColumn = (t, index) =>
            t.gameObject.transform.position == new Vector3(index, 0);

        public static readonly Func<int, Vector2> BeforeFirstInRow = (index) => new Vector2(-1, index);
        
        public static readonly Func<int, Vector2> AfterLastInRow = (index) => new Vector2(GridManager.X, index);
        
        public static readonly Func<int, Vector2> BeforeFirstInColumn = (index) => new Vector2(index, -1);
        
        public static readonly Func<int, Vector2> AfterLastInColumn = (index) =>  new Vector2(index, GridManager.Y);


    }
}