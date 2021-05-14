using UnityEngine;

namespace grid {
    public class Arrow {
        public readonly GameObject gameObject;
        public readonly GridManager.ShiftAxis axes;
        public Vector3 direction;
        public readonly int index;

        public Arrow(GameObject gameObject,
            GridManager.ShiftAxis axes,
            Vector3 pos,
            Vector3 direction,
            int index) {
            this.gameObject = gameObject;
            this.axes = axes;
            this.gameObject.transform.position = pos;
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
            this.direction = direction;
            this.index = index;
            this.gameObject.name = direction + "," + axes + "," + index + "," + pos;
        }

        public void SetColor(Color color) {
            this.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
}