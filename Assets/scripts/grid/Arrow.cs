using UnityEngine;

namespace grid {
    

public class Arrow  {
    public readonly GameObject gameObject;
    public GridManager.ShiftAxis axes;
    public Vector3 direction;
    public int index;
    public Arrow(GameObject gameObject, GridManager.ShiftAxis axes, Vector3 pos, Vector3 direction, int index) {
        this.gameObject = gameObject;
        this.gameObject.name = direction + "," + axes;
        this.axes = axes;
        this.gameObject.transform.position = pos;
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        this.direction = direction;
        this.index = index;
    }
}


}