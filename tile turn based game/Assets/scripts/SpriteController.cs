using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteController : MonoBehaviour
{
    private GameControllerScript GCS;
    private MapEditMenueCamController MEMCC;
    private DatabaseController DBC;
    public SpriteRenderer MouseOverlaySpriteRender;
    private Dictionary<string, GameObject> WaterOverlays = new Dictionary<string, GameObject>();
    private Vector2 TopPosOffset = new Vector2(1, 0);
    private Vector3 TopRotOffest = new Vector3(0, 0, 90);
    private Vector2 LeftPosOffset = new Vector2(1, 1);
    private Vector3 LeftRotOffset = new Vector3(0, 0, 180);
    private Vector2 BottomPosOffset = new Vector2(0, 1);
    private Vector3 BottomRotOffset = new Vector3(0, 0, 270);
    private Vector2 BottomRightPosOffest = new Vector2(0, 1);
    private Vector3 BottomRightRotOffset = new Vector3(0, 0, 270);
    private Vector2 BottomLeftPosOffset = new Vector2(1, 1);
    private Vector3 BottomLeftRotOffset = new Vector3(0, 0, 180);
    private Vector2 TopLeftPosOffset = new Vector2(1, 0);
    private Vector3 TopLeftRotOffset = new Vector3(0, 0, 90);

    // Start is called before the first frame update
    void Start()
    {
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
        DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();

        if (gameObject.transform.tag == "Terrain")
        {
            MouseOverlaySpriteRender = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            MouseOverlaySpriteRender.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MouseOverlayRayCaster();
    }

    public void ChangeBuilding()
    {
        if (MEMCC.SelectedButton == "Delete Building")
        {
            Debug.Log("Deleting Building");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("ChangBuilding activated");
            foreach (KeyValuePair<int, Building> kvp in DBC.BuildingDictionary)
            {
                if (MEMCC.SelectedButton == kvp.Value.Title) //checks through dictionary for matching tile to button name
                {
                    //Debug.Log("Changing tile to " + kvp.Value.Title);
                    gameObject.name = kvp.Value.Title;//change name of tile
                    gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.BuildingDictionary[kvp.Key].ArtworkDirectory[0]); //change sprite of tile
                }
            }
        }
    }

    public void ChangeUnit()
    {
        if (MEMCC.SelectedButton == "Delete Unit")
        {
            Debug.Log("Deleting Unit");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("ChangUnit activated");
            foreach (KeyValuePair<int, Unit> kvp in DBC.UnitDictionary)
            {
                if (MEMCC.SelectedButton == kvp.Value.Title) //checks through dictionary for matching tile to button name
                {
                    //Debug.Log("Changing tile to " + kvp.Value.Title);
                    gameObject.name = kvp.Value.Title;//change name of tile
                    gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.UnitDictionary[kvp.Key].ArtworkDirectory[0]); //change sprite of tile
                }
            }
        }
    }

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

    private void MouseOverlayRayCaster()
    {
        if (gameObject.transform.tag == "Terrain")
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); //find all objects under mouse
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray); //add them to array
                foreach (var hit in hits)
                {
                    if (hit.transform == this.transform) //was last hit a unit? or this?
                    {
                        MouseOverlaySpriteRender.enabled = true;
                    }
                    else if (hit.transform.tag == "Unit" || hit.transform.tag == "Building") //if tag is a unit or building do nothing.
                    {

                    }
                    else //else disable overlay
                    {
                        MouseOverlaySpriteRender.enabled = false;
                    }
                }
            } 
        }
    } //checks if mouse is over tile, activates mouseoverlay if it is.

    public void TerrainSpriteAdjuster()
    {
        if (gameObject.name == "Water")
        {
            //UnityEngine.Debug.Log("Starting Water Sprite Controller");
            var currentPos = (Vector2)transform.position;
            int TileId = 0;
            foreach (var kvp in DBC.TerrainDictionary)
            {
                if (kvp.Value.Title == gameObject.name)
                {
                    TileId = kvp.Value.ID;
                    Debug.Log("Set ID to: " + kvp.Value.ID);
                }
            }
            Vector2 topLeftPos = currentPos + new Vector2(-1, 1);
            Vector2 topPos = currentPos + new Vector2(0, 1);
            Vector2 topRightPos = currentPos + new Vector2(1, 1);
            Vector2 rightPos = currentPos + new Vector2(1, 0);
            Vector2 bottomRightPos = currentPos + new Vector2(1, -1);
            Vector2 bottomPos = currentPos + new Vector2(0, -1);
            Vector2 bottomLeftPos = currentPos + new Vector2(-1, -1);
            Vector2 leftPos = currentPos + new Vector2(-1, 0);
            //UnityEngine.Debug.Log("Starting if Statments");
            if (GCS.TilePos.ContainsKey(topPos))
            {
                //Debug.Log("tile pos contains key");
                if (GCS.TilePos[topPos].name != "Water")//is the tle above this one not water?
                {
                    if (WaterOverlays.ContainsKey("TopLandOverlay"))//does the dictionary contain a key for this already?
                    {
                        //Debug.Log("wo contains key");
                    }
                    else
                    {
                        //Debug.Log("wo does not");
                        GameObject RLO = new GameObject();
                        RLO.name = "TopLandOverlay";
                        RLO.transform.position = gameObject.transform.position;
                        RLO.AddComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        RLO.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
                        RLO.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        RLO.transform.parent = gameObject.transform;
                        RLO.transform.localPosition = TopPosOffset;
                        RLO.transform.eulerAngles = TopRotOffest;
                        WaterOverlays.Add(RLO.name, RLO);
                    }
                }
                else//remove overlay if water is above
                {
                    if (WaterOverlays.ContainsKey("TopLandOverlay"))
                    {
                        Destroy(WaterOverlays["TopLandOverlay"]);
                        WaterOverlays.Remove("TopLandOverlay"); 
                    }
                }
            }
            if (GCS.TilePos.ContainsKey(topRightPos))
            {
                //Debug.Log("tile pos contains key");
                if (GCS.TilePos[topRightPos].name != "Water")//is the tle above this one not water?
                {
                    if (WaterOverlays.ContainsKey("TopRightLandOverlay"))//does the dictionary contain a key for this already?
                    {
                        //Debug.Log("wo contains key");
                    }
                    else
                    {
                        //Debug.Log("wo does not");
                        GameObject RLO = new GameObject();
                        RLO.name = "TopRightLandOverlay";
                        RLO.transform.position = gameObject.transform.position;
                        RLO.AddComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[1]);
                        RLO.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
                        RLO.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        RLO.transform.parent = gameObject.transform;
                        WaterOverlays.Add(RLO.name, RLO);
                    }
                }
                else//remove overlay if water is above
                {
                    if (WaterOverlays.ContainsKey("TopRightLandOverlay"))
                    {
                        Destroy(WaterOverlays["TopRightLandOverlay"]);
                        WaterOverlays.Remove("TopRightLandOverlay");
                    }
                }
            }
            if (GCS.TilePos.ContainsKey(rightPos))
            {
                //Debug.Log("tile pos contains key");
                if (GCS.TilePos[rightPos].name != "Water")//is the tle above this one not water?
                {
                    if (WaterOverlays.ContainsKey("RightLandOverlay"))//does the dictionary contain a key for this already?
                    {
                        //Debug.Log("wo contains key");
                    }
                    else
                    {
                        //Debug.Log("wo does not");
                        GameObject RLO = new GameObject();
                        RLO.name = "RightLandOverlay";
                        RLO.transform.position = gameObject.transform.position;
                        RLO.AddComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        RLO.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
                        RLO.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        RLO.transform.parent = gameObject.transform;
                        WaterOverlays.Add(RLO.name, RLO);
                    }
                }
                else//remove overlay if water is above
                {
                    if (WaterOverlays.ContainsKey("RightLandOverlay"))
                    {
                        Destroy(WaterOverlays["RightLandOverlay"]);
                        WaterOverlays.Remove("RightLandOverlay");
                    }
                }
            }
            if (GCS.TilePos.ContainsKey(bottomRightPos))
            {
                //Debug.Log("tile pos contains key");
                if (GCS.TilePos[bottomRightPos].name != "Water")//is the tle above this one not water?
                {
                    if (WaterOverlays.ContainsKey("BottomRightLandOverlay"))//does the dictionary contain a key for this already?
                    {
                        //Debug.Log("wo contains key");
                    }
                    else
                    {
                        //Debug.Log("wo does not");
                        GameObject RLO = new GameObject();
                        RLO.name = "BottomRightLandOverlay";
                        RLO.transform.position = gameObject.transform.position;
                        RLO.AddComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[1]);
                        RLO.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
                        RLO.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        RLO.transform.parent = gameObject.transform;
                        RLO.transform.localPosition = BottomRightPosOffest;
                        RLO.transform.eulerAngles = BottomRightRotOffset;
                        WaterOverlays.Add(RLO.name, RLO);
                    }
                }
                else//remove overlay if water is above
                {
                    if (WaterOverlays.ContainsKey("BottomRightLandOverlay"))
                    {
                        Destroy(WaterOverlays["BottomRightLandOverlay"]);
                        WaterOverlays.Remove("BottomRightLandOverlay");
                    }
                }
            }
            if (GCS.TilePos.ContainsKey(bottomPos))
            {
                //Debug.Log("tile pos contains key");
                if (GCS.TilePos[bottomPos].name != "Water")//is the tle above this one not water?
                {
                    if (WaterOverlays.ContainsKey("BottomLandOverlay"))//does the dictionary contain a key for this already?
                    {
                        //Debug.Log("wo contains key");
                    }
                    else
                    {
                        //Debug.Log("wo does not");
                        GameObject RLO = new GameObject();
                        RLO.name = "BottomLandOverlay";
                        RLO.transform.position = gameObject.transform.position;
                        RLO.AddComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        RLO.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
                        RLO.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        RLO.transform.parent = gameObject.transform;
                        RLO.transform.localPosition = BottomPosOffset;
                        RLO.transform.eulerAngles = BottomRotOffset;
                        WaterOverlays.Add(RLO.name, RLO);
                    }
                }
                else//remove overlay if water is above
                {
                    if (WaterOverlays.ContainsKey("BottomLandOverlay"))
                    {
                        Destroy(WaterOverlays["BottomLandOverlay"]);
                        WaterOverlays.Remove("BottomLandOverlay");
                    }
                }
            }
            if (GCS.TilePos.ContainsKey(bottomLeftPos))
            {
                //Debug.Log("tile pos contains key");
                if (GCS.TilePos[bottomLeftPos].name != "Water")//is the tle above this one not water?
                {
                    if (WaterOverlays.ContainsKey("BottomLeftLandOverlay"))//does the dictionary contain a key for this already?
                    {
                        //Debug.Log("wo contains key");
                    }
                    else
                    {
                        //Debug.Log("wo does not");
                        GameObject RLO = new GameObject();
                        RLO.name = "BottomLeftLandOverlay";
                        RLO.transform.position = gameObject.transform.position;
                        RLO.AddComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[1]);
                        RLO.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
                        RLO.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        RLO.transform.parent = gameObject.transform;
                        RLO.transform.localPosition = BottomLeftPosOffset;
                        RLO.transform.eulerAngles = BottomLeftRotOffset;
                        WaterOverlays.Add(RLO.name, RLO);
                    }
                }
                else//remove overlay if water is above
                {
                    if (WaterOverlays.ContainsKey("BottomLeftLandOverlay"))
                    {
                        Destroy(WaterOverlays["BottomLeftLandOverlay"]);
                        WaterOverlays.Remove("BottomLeftLandOverlay");
                    }
                }
            }
            if (GCS.TilePos.ContainsKey(leftPos))
            {
                //Debug.Log("tile pos contains key");
                if (GCS.TilePos[leftPos].name != "Water")//is the tle above this one not water?
                {
                    if (WaterOverlays.ContainsKey("LeftLandOverlay"))//does the dictionary contain a key for this already?
                    {
                        //Debug.Log("wo contains key");
                    }
                    else
                    {
                        //Debug.Log("wo does not");
                        GameObject RLO = new GameObject();
                        RLO.name = "LeftLandOverlay";
                        RLO.transform.position = gameObject.transform.position;
                        RLO.AddComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        RLO.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
                        RLO.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        RLO.transform.parent = gameObject.transform;
                        RLO.transform.localPosition = LeftPosOffset;
                        RLO.transform.eulerAngles = LeftRotOffset;
                        WaterOverlays.Add(RLO.name, RLO);
                    }
                }
                else//remove overlay if water is above
                {
                    if (WaterOverlays.ContainsKey("LeftLandOverlay"))
                    {
                        Destroy(WaterOverlays["LeftLandOverlay"]);
                        WaterOverlays.Remove("LeftLandOverlay");
                    }
                }
            }
            if (GCS.TilePos.ContainsKey(topLeftPos))
            {
                //Debug.Log("tile pos contains key");
                if (GCS.TilePos[topLeftPos].name != "Water")//is the tle above this one not water?
                {
                    if (WaterOverlays.ContainsKey("TopLeftLandOverlay"))//does the dictionary contain a key for this already?
                    {
                        //Debug.Log("wo contains key");
                    }
                    else
                    {
                        //Debug.Log("wo does not");
                        GameObject RLO = new GameObject();
                        RLO.name = "TopLeftLandOverlay";
                        RLO.transform.position = gameObject.transform.position;
                        RLO.AddComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[1]);
                        RLO.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
                        RLO.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        RLO.transform.parent = gameObject.transform;
                        RLO.transform.localPosition = TopLeftPosOffset;
                        RLO.transform.eulerAngles = TopLeftRotOffset;
                        WaterOverlays.Add(RLO.name, RLO);
                    }
                }
                else//remove overlay if water is above
                {
                    if (WaterOverlays.ContainsKey("TopLeftLandOverlay"))
                    {
                        Destroy(WaterOverlays["TopLeftLandOverlay"]);
                        WaterOverlays.Remove("TopLeftLandOverlay");
                    }
                }
            }

        }
        else
        {
            if (WaterOverlays.Count > 0)
            {
                var list = new List<GameObject>();
                foreach (var kvvp in WaterOverlays)
                {
                    list.Add(kvvp.Value);
                }
                foreach (var item in list)
                {
                    Destroy(item);
                }
                list.Clear();
                WaterOverlays.Clear();
            }
        }

    }
}
