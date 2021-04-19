using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {
    private int rows = 10;
    private int columns = 10;

    [SerializeField]
     Tilemap tileMap;
    
    
   

    // Start is called before the first frame update
    void Start()
    {
       Debug.Log(tileMap);
       GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetKeyDown("left"))
        {
        
        
        }

         if (Input.GetKeyDown("right"))
        {
           
        
        }
         if (Input.GetKeyDown("space"))
        {
        
           
        }
        
    }


    private void GenerateGrid() {  
        Debug.Log("---");
        Sprite tileSprite = Resources.Load<Sprite>("Tiles/tile");
        Sprite grass = Resources.Load<Sprite>("Tiles/Grass");
        TileMaze tile = ScriptableObject.CreateInstance<TileMaze>();
        tile.sprite = tileSprite;
        TileMaze grassTile = ScriptableObject.CreateInstance<TileMaze>();
        grassTile.sprite = grass;
        Debug.Log("Sprite");
        Debug.Log(tileSprite);
        Debug.Log("Tile");
        Debug.Log(tile);
        for(int row = 0; row < rows; row++) {
            for(int col = 0; col < columns; col++) {
                if (col % 2 == 0 && row % 2 == 0) {
                    Debug.Log("changing tile to Grass");
                       tileMap.SetTile(new Vector3Int(row, col, 0), tile);
                } else {
                    Debug.Log("changing tile to tile");
                       tileMap.SetTile(new Vector3Int(row, col, 0), grassTile);
                }
                Debug.Log("---");
                tileMap.RefreshAllTiles();
            } 
            
        }
    }
}
