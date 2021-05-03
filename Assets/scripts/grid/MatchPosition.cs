using System;
using UnityEngine;

namespace grid {
    public static class MatchPosition {
        public static readonly Func<Tile, int, bool> LastInRow = (t, index) =>
            t.gameObject.transform.position == new Vector3(GridManager.Columns - 1, index);

        public static readonly Func<Tile, int, bool> FirstInRow = (t, index) =>
            t.gameObject.transform.position == new Vector3(0, index);

        public static readonly Func<Tile, int, bool> LastInColumn = (t, index) =>
            t.gameObject.transform.position == new Vector3(index, GridManager.Rows - 1);

        public static readonly Func<Tile, int, bool> FirstInColumn = (t, index) =>
            t.gameObject.transform.position == new Vector3(index, 0);
    }
}