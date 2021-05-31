using System;
using UnityEngine;

namespace grid {
    public static class MatchPosition {
        
        public const int ARROW_WS = 0;
        public const int ARROW_ES = 1;
        public const int ARROW_NW = GridManager.N;
        public const int ARROW_SW = GridManager.N - 1;
        public const int ARROW_EN = GridManager.N - 2;
        public const int ARROW_WN = GridManager.N - 3;
        public const int ARROW_NE = 2 * GridManager.N - 3;
        public const int ARROW_SE = 2 * GridManager.N - 4;
        public const int ShiftUpOrRight = 2;
        public const int ShiftDownOrLeft = -2;
        public const int shiftOppositeRightOrUp = 1;
        public const int shiftOppositeLeftOrDown = -1;

        public static readonly Func<Tile, int, bool> LastInRow = (t, index) =>
            t.gameObject.transform.position == new Vector3(GridManager.N - 1, index);

        public static readonly Func<Tile, int, bool> FirstInRow = (t, index) =>
            t.gameObject.transform.position == new Vector3(0, index);

        public static readonly Func<Tile, int, bool> LastInColumn = (t, index) =>
            t.gameObject.transform.position == new Vector3(index, GridManager.N - 1);

        public static readonly Func<Tile, int, bool> FirstInColumn = (t, index) =>
            t.gameObject.transform.position == new Vector3(index, 0);

        public static readonly Func<Tile, Vector3, Tile> MatchTile = (t, targetPosition) => t.gameObject.transform.position == targetPosition ? t : null;


        public static readonly Func<int, Vector2> BeforeFirstInRow = (index) => new Vector2(-1, index);

        public static readonly Func<int, Vector2> AfterLastInRow = (index) => new Vector2(GridManager.N, index);

        public static readonly Func<int, Vector2> BeforeFirstInColumn = (index) => new Vector2(index, -1);

        public static readonly Func<int, Vector2> AfterLastInColumn = (index) =>  new Vector2(index, GridManager.N);
    }
}