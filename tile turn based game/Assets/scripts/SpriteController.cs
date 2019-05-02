using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SpriteController : MonoBehaviour
{
    private GameControllerScript GCS;
    private MapEditMenueCamController MEMCC;
    private DatabaseController DBC;
    private Dictionary<string, GameObject> WaterOverlays = new Dictionary<string, GameObject>();
    private Vector3 TopRotOffest = new Vector3(0, 0, 90);
    private Vector3 LeftRotOffset = new Vector3(0, 0, 180);
    private Vector3 BottomRotOffset = new Vector3(0, 0, 270);
    private Vector3 BottomRightRotOffset = new Vector3(0, 0, 270);
    private Vector3 BottomLeftRotOffset = new Vector3(0, 0, 180);
    private Vector3 TopLeftRotOffset = new Vector3(0, 0, 90);
    private Vector3 Road1wayRightRotOffset = new Vector3(0, 0, 270);
    private Vector3 Road1wayBottomRotOffset = new Vector3(0, 0, 180);
    private Vector3 Road1wayLeftRotOffset = new Vector3(0, 0, 90);
    private Vector3 Road2way90TopRotOffset = new Vector3(0, 0, 180);
    private Vector3 Road2way90RightRotOffset = new Vector3(0, 0, 90);
    private Vector3 Road2way90LeftRotOffset = new Vector3(0, 0, 270);
    private Vector3 Road2wayStraightTopBottomRotOffset = new Vector3(0, 0, 90);
    private Vector3 Road3wayTopRotOffset = new Vector3(0, 0, 180);
    private Vector3 Road3wayRightRotOffset = new Vector3(0, 0, 90);
    private Vector3 Road3wayLeftRotOffset = new Vector3(0, 0, 270);
    private Vector3 OriginalRot;
    [HideInInspector]
    public SpriteRenderer MouseOverlaySpriteRender;
    [HideInInspector]
    public int Team;
    [HideInInspector]
    public bool UnitMovable;
    public bool Occupied;
    [HideInInspector]
    public int Weight;
    [HideInInspector]
    public List<Vector2> FloodFillList;
    [HideInInspector]
    public int MovePoints;
    public Dictionary<Vector2, int[]> TilesWeights = new Dictionary<Vector2, int[]>();

    // Start is called before the first frame update
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MapEditorScene")
        {
            MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
            Team = MEMCC.SelectedTeam;
        }
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();
    }
    void Start()
    {

        if (gameObject.transform.tag == DBC.TerrainDictionary[0].Type)
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
                gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
                if (GCS.UnitPos.ContainsKey(gameObject.transform.position) && DBC.TerrainDictionary[kvp.Key].Walkable == false)
                {
                    Destroy(GCS.UnitPos[gameObject.transform.position]);
                    GCS.UnitPos.Remove(gameObject.transform.position);
                }
                if (GCS.BuildingPos.ContainsKey(gameObject.transform.position) && DBC.TerrainDictionary[kvp.Key].Walkable == false)
                {
                    Destroy(GCS.BuildingPos[gameObject.transform.position]);
                    GCS.BuildingPos.Remove(gameObject.transform.position);
                }
            }
        }

    }

    private void MouseOverlayRayCaster()
    {
        if (gameObject.transform.tag == DBC.TerrainDictionary[0].Type)
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
                    else if (hit.transform.tag == DBC.UnitDictionary[0].Type || hit.transform.tag == DBC.BuildingDictionary[0].Type) //if tag is a unit or building do nothing.
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

    public void WaterSpriteController()
    {
        if (gameObject.name == DBC.TerrainDictionary[1].Title)
        {
            //UnityEngine.Debug.Log("Starting Water Sprite Controller");
            var currentPos = (Vector2)transform.position;
            int TileId = 0;
            foreach (var kvp in DBC.TerrainDictionary)
            {
                if (kvp.Value.Title == gameObject.name)
                {
                    TileId = kvp.Value.ID;
                    //Debug.Log("Set ID to: " + kvp.Value.ID);
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

    } //Controlls the overlay for adding banks to the water sprites

    public void RoadSpriteController()
    {
        if (gameObject.name == DBC.TerrainDictionary[3].Title)
        {
            var currentPos = (Vector2)transform.position;
            if (OriginalRot == null)
            {
                OriginalRot = gameObject.transform.eulerAngles;
            }
            var SR = gameObject.GetComponent<SpriteRenderer>();
            int TileId = 0;
            int counter = 0;
            foreach (var kvp in DBC.TerrainDictionary)
            {
                if (kvp.Value.Title == gameObject.name)
                {
                    TileId = kvp.Value.ID;
                    //Debug.Log("Set ID to: " + kvp.Value.ID);
                }
            }
            Vector2 topPos = currentPos + new Vector2(0, 1);
            Vector2 rightPos = currentPos + new Vector2(1, 0);
            Vector2 bottomPos = currentPos + new Vector2(0, -1);
            Vector2 leftPos = currentPos + new Vector2(-1, 0);
            bool topBool = false;
            bool rightbool = false;
            bool bottombool = false;
            bool leftbool = false;

            if (GCS.TilePos.ContainsKey(topPos))
            {
                if (GCS.TilePos[topPos].name == "Road")
                {
                    counter = counter + 1;
                    topBool = true;
                }
            }
            if (GCS.TilePos.ContainsKey(rightPos))
            {
                if (GCS.TilePos[rightPos].name == "Road")
                {
                    counter = counter + 1;
                    rightbool = true;
                }
            }
            if (GCS.TilePos.ContainsKey(bottomPos))
            {
                if (GCS.TilePos[bottomPos].name == "Road")
                {
                    counter = counter + 1;
                    bottombool = true;
                }
            }
            if (GCS.TilePos.ContainsKey(leftPos))
            {
                if (GCS.TilePos[leftPos].name == "Road")
                {
                    counter = counter + 1;
                    leftbool = true;
                }
            }

            switch (counter)
            {
                case 0:
                    SR.sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[0]);
                    break;
                case 1:
                    SR.sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[1]);
                    if (topBool)
                    {
                        gameObject.transform.eulerAngles = OriginalRot;
                    }
                    else if (rightbool)
                    {
                        gameObject.transform.eulerAngles = Road1wayRightRotOffset + OriginalRot;
                    }
                    else if (bottombool)
                    {
                        gameObject.transform.eulerAngles = Road1wayBottomRotOffset + OriginalRot;
                    }
                    else if (leftbool)
                    {
                        gameObject.transform.eulerAngles = Road1wayLeftRotOffset + OriginalRot;
                    }
                    break;
                case 2:
                    if (topBool && rightbool)
                    {
                        SR.sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        gameObject.transform.eulerAngles = Road2way90TopRotOffset + OriginalRot;
                    }
                    else if (rightbool && bottombool)
                    {
                        SR.sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        gameObject.transform.eulerAngles = Road2way90RightRotOffset + OriginalRot;
                    }
                    else if (bottombool && leftbool)
                    {
                        SR.sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        gameObject.transform.eulerAngles = OriginalRot;
                    }
                    else if (leftbool && topBool)
                    {
                        SR.sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        gameObject.transform.eulerAngles = Road2way90LeftRotOffset + OriginalRot;
                    }
                    else if (topBool && bottombool)
                    {
                        SR.sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[3]);
                        gameObject.transform.eulerAngles = Road2wayStraightTopBottomRotOffset + OriginalRot;
                    }
                    else if (leftbool && rightbool)
                    {
                        SR.sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[3]);
                        gameObject.transform.eulerAngles = OriginalRot;
                    }
                    break;
                case 3:
                    SR.sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[4]);
                    if (!bottombool)
                    {
                        gameObject.transform.eulerAngles = Road3wayTopRotOffset + OriginalRot;
                    }
                    else if (!leftbool)
                    {
                        gameObject.transform.eulerAngles = Road3wayRightRotOffset + OriginalRot;
                    }
                    else if (!topBool)
                    {
                        gameObject.transform.eulerAngles = OriginalRot;
                    }
                    else if (!rightbool)
                    {
                        gameObject.transform.eulerAngles = Road3wayLeftRotOffset + OriginalRot;
                    }
                    break;
                case 4:
                    SR.sprite = DBC.loadSprite(DBC.TerrainDictionary[TileId].ArtworkDirectory[5]);
                    gameObject.transform.eulerAngles = OriginalRot;
                    break;
            }
            counter = 0;
        }
    } //makes teh roads attatch to each other

    public void TeamSpriteController()
    {
        if (gameObject.tag == "Unit")
        {
            foreach (var kvp in DBC.UnitDictionary)
            {
                if (gameObject.name == kvp.Value.Title)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.UnitDictionary[kvp.Value.ID].ArtworkDirectory[Team]);
                }
            }
        }
        else if (gameObject.tag == "Building")
        {
            foreach (var kvp in DBC.BuildingDictionary)
            {
                if (gameObject.name == kvp.Value.Title)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.BuildingDictionary[kvp.Value.ID].ArtworkDirectory[Team]);
                }
            }
        }
    }   //controlls the sprite for unit or building based on the team

    public void RoundUpdater()
    {
        if (Team == GCS.CurrentTeamsTurn && gameObject.tag == DBC.UnitDictionary[0].Type) //object is on the current team and it is a unit
        {
            UnitMovable = true;
            foreach (var kvp in DBC.UnitDictionary)
            {
                if (gameObject.transform.name == kvp.Value.Title)
                {
                    GetTileValues(gameObject.transform.position);
                }
            }
        }
        else if (gameObject.tag == DBC.TerrainDictionary[0].Type) // else if terrain 
        {
            UnitMovable = false;
            if (GCS.UnitPos.ContainsKey(gameObject.transform.position))
            {
                Occupied = true;
                //Debug.Log("set tile " + gameObject.transform.position + "Occupied to " + Occupied);
            }
            else
            {
                Occupied = false;
            }
        }
        else
        {
            UnitMovable = false;
        }
    }  //used to set the movable variable. called from game controller

    public void GetTileValues(Vector2 Position)
    {
        Debug.Log ("MP = " + MovePoints);
        List<Vector2> Directions = new List<Vector2>();
        Directions.Add( new Vector2(0, 1));
        Directions.Add( new Vector2(1, 0));
        Directions.Add( new Vector2(0, -1));
        Directions.Add( new Vector2(-1, 0));
        int count = 1;
        TilesWeights.Clear();
        
        foreach (var dir in Directions)
        {
            if (GCS.TilePos.ContainsKey(Position + dir) && !TilesWeights.ContainsKey(Position + dir))
            {
                if (GCS.TilePos[Position + dir].GetComponent<SpriteController>().Weight <= MovePoints && !GCS.TilePos[Position + dir].GetComponent<SpriteController>().Occupied)
                {
                    int[] temparray = new int[2];
                    temparray[0] = count;
                    temparray[1] = GCS.TilePos[Position].GetComponent<SpriteController>().Weight;
                    TilesWeights.Add(Position + dir, temparray); 
                }
            }
        }
        //count = count + 1;
        while (count <= MovePoints)
        {
            int[] tempIntArray = new int[2];
            Dictionary<Vector2, int[]> Temp = new Dictionary<Vector2, int[]>();
            foreach (var kvp in TilesWeights)
            {
                foreach(var dir in Directions)
                {
                    if (GCS.TilePos.ContainsKey(kvp.Key + dir) && !TilesWeights.ContainsKey(kvp.Key + dir) && !Temp.ContainsKey(kvp.Key + dir))
                    {
                        if (kvp.Value[1] + GCS.TilePos[kvp.Key + dir].GetComponent<SpriteController>().Weight <= MovePoints && !GCS.TilePos[kvp.Key + dir].GetComponent<SpriteController>().Occupied)
                        {
                            tempIntArray[0] = count;
                            tempIntArray[1] = GCS.TilePos[kvp.Key + dir].transform.GetComponent<SpriteController>().Weight + kvp.Value[1];
                            Temp.Add(kvp.Key + dir, tempIntArray); 
                        }
                    }
                }
            }
            foreach(var kvp in Temp)
            {
                TilesWeights.Add(kvp.Key, kvp.Value);
            }
            if (count == 15)
            {
                Debug.Log("Broke, count at:" + count);
                break;
            }
            count = count + 1;
        }
        
    }
}

