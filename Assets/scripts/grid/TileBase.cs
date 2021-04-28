
using UnityEngine;

namespace grid
{
    public abstract class TileBase : MonoBehaviour
    {
        private void Start()
        {
           Debug.Log("attached to gameObject " + this);
           Debug.Log(gameObject);
        }
        
        public void Rotate()
        {
            transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(90, 0, 0), Time.deltaTime);
        }
    }
}