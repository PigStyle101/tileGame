using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class TerrainSpriteController : MonoBehaviour
{

    private GameControllerScript GCS;
    public Vector2 currentPos;
    public Sprite[] Sprits;
    public int counter;
    public SpriteRenderer MouseOverlaySpriteRender;
    private MapEditMenueCamController MEMCC;
    private DatabaseController DBC;


    void Start()
    {
        MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();
        MouseOverlaySpriteRender = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        MouseOverlaySpriteRender.enabled = false;
    }

    private void Update()
    {
        MouseOverlayRayCaster();
    }

    private void OnMouseUp()
    {
        //WaterSpriteController();
    }//WORK IN PROGRESS, actually... need to think about the best way to do this, might be easyer to use overlays?? not sure yet..... currently using a different sprite for things

    public void WaterSpriteController()
    {
        UnityEngine.Debug.Log("Starting Water Sprite Controller");
        try
        {
            if (gameObject.name == "Water")
            {
                counter = 0;
                currentPos = transform.position;
                Vector2 topLeftPos = currentPos + new Vector2(-1, 1);
                Vector2 topPos = currentPos + new Vector2(0, 1);
                Vector2 topRightPos = currentPos + new Vector2(1, 1);
                Vector2 rightPos = currentPos + new Vector2(1, 0);
                Vector2 bottomRightPos = currentPos + new Vector2(1, -1);
                Vector2 bottomPos = currentPos + new Vector2(0, -1);
                Vector2 bottomLeftPos = currentPos + new Vector2(-1, -1);
                Vector2 leftPos = currentPos + new Vector2(-1, 0);
                bool Top = new bool();
                bool Left = new bool();
                bool Right = new bool();
                bool Bottom = new bool();
                UnityEngine.Debug.Log("Starting if Statments");
                // check how many waters around
                //if (surrounding blocks that are land is = 1) {use waterland1side, then rotate to be correct}
                if (GCS.TilePos.ContainsKey(topPos)) { if (GCS.TilePos[topPos].name == "Grass") { counter = counter + 1; Top = true; } }
                if (GCS.TilePos.ContainsKey(rightPos)) { if (GCS.TilePos[rightPos].name == "Grass") { counter = counter + 1; Right = true; } }
                if (GCS.TilePos.ContainsKey(leftPos)) { if (GCS.TilePos[leftPos].name == "Grass") { counter = counter + 1; Left = true; } }
                if (GCS.TilePos.ContainsKey(bottomPos)) { if (GCS.TilePos[bottomPos].name == "Grass") { counter = counter + 1; Bottom = true; } }

                //add the if statment then check exactly what sides are there to determin rotation
                switch (counter)
                {
                    case 0:
                        UnityEngine.Debug.Log("Case 0");
                        gameObject.GetComponent<SpriteRenderer>().sprite = Sprits[0];
                        break;
                    case 1:
                        UnityEngine.Debug.Log("Case 1");
                        gameObject.GetComponent<SpriteRenderer>().sprite = Sprits[1];
                        break;
                    case 2:
                        UnityEngine.Debug.Log("Case 2");
                        if(Top == true && Bottom == true) { /*change to waterland2sideoposite rotation.z = 0*/}
                        if (Left == true && Right == true) { /*change to waterland2sideoposite rotation.z = 180*/}
                        if (Left == true && Top == true) { /*change to WaterLand2Side90Deg rotation.z = 180*/}
                        break;
                    case 3:
                        UnityEngine.Debug.Log("Case 3");
                        break;
                    case 4:
                        UnityEngine.Debug.Log("Case 4");
                        break;
                }

            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
            throw;
        }
    }
    
    //BUG IN MOUSEOVERLAYRAYCASTER// all the terrian mouse overlay will activate when mouse is hovered over certain units
    private void MouseOverlayRayCaster()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); //find all objects under mouse
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray); //add them to array
            string LastHit = "none"; //initate variable for last hit
            foreach (var hit in hits)
            {
                if (hit.transform == this.transform  || LastHit == "Unit") //was last hit a unit? or this?
                {
                    MouseOverlaySpriteRender.enabled = true;
                }
                else if (hit.transform.tag == "Unit") //if unit set last hit accordingly
                {
                    LastHit = hit.transform.tag = "Unit";
                }
                else //else disable overlay
                {
                    MouseOverlaySpriteRender.enabled = false;
                }
            } 
        }
    } //checks if mouse is over tile, activates mouseoverlay if it is.

    public void ChangeTile() //if a terrain is selected and the player clicks a tile it changes the tile to the correct terrain
    {
        Debug.Log("ChangTile activated");
        foreach (KeyValuePair<int, Terrain> kvp in DBC.TerrainDictionary)
        {
            if (MEMCC.SelectedButton == kvp.Value.Title) //checks through dictionary for matching tile to button name
            {
                //Debug.Log("Changing tile to " + kvp.Value.Title);
                gameObject.name = kvp.Value.Title;//change name of tile
                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.TerrainDictionary[kvp.Key].ArtworkDirectory[0]); //change sprite of tile
            }
        }
        
    }
}
