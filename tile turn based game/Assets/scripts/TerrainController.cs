using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class TerrainController : MonoBehaviour
{
    private MapEditMenueCamController MEMCC;
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
    public SpriteRenderer MouseOverlaySelectedSpriteRender;
    public SpriteRenderer FogOfWar;
    public bool Occupied;
    [HideInInspector]
    public int Weight;
    //[HideInInspector]
    public bool Walkable;
    [HideInInspector]
    public bool BlocksSight;
    [HideInInspector]
    public int DefenceBonus;
    [HideInInspector]
    public int ID;
    [HideInInspector]
    public bool FogOfWarBool;
    [HideInInspector]
    public bool Overlays;
    [HideInInspector]
    public bool Connectable;
    private bool LastHitThis = false;
    //[HideInInspector]
    public bool IdleAnimations;
    private float Timer = 0;
    public List<string> IdleAnimationsDirectory;
    private int IdleState = 1;
    [HideInInspector]
    public int DictionaryReferance;

    private void Awake()
    {
        if (GameControllerScript.instance.CurrentScene == "MapEditorScene")
        {
            MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
        }
        MouseOverlaySpriteRender = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        MouseOverlaySelectedSpriteRender = gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>();
        MouseOverlaySpriteRender.enabled = false;
        FogOfWar = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        MouseOverlayRayCaster();
        if (LastHitThis)
        {
            MouseOverlaySelectedSpriteRender.enabled = true;
        }
        else
        {
            MouseOverlaySelectedSpriteRender.enabled = false;
        }
    }

    public void TerrainRoundUpdater()
    {
        if (GameControllerScript.instance.UnitPos.ContainsKey(gameObject.transform.position))
        {
            Occupied = true;
            //Debug.Log("set tile " + gameObject.transform.position + "Occupied to " + Occupied);
        }
        else
        {
            Occupied = false;
        }
    }

    public void ChangeTile()
    {
        //Debug.Log("Changing tile to " + kvp.Value.Title);
        gameObject.name = DatabaseController.instance.TerrainDictionary[MEMCC.SelectedButtonDR].Title;//change name of tile
        gameObject.GetComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[MEMCC.SelectedButtonDR].ArtworkDirectory[0]); //change sprite of tile
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        ID = DatabaseController.instance.TerrainDictionary[MEMCC.SelectedButtonDR].ID;
        Overlays = DatabaseController.instance.TerrainDictionary[MEMCC.SelectedButtonDR].Overlays;
        Connectable = DatabaseController.instance.TerrainDictionary[MEMCC.SelectedButtonDR].Connectable;
        IdleAnimations = DatabaseController.instance.TerrainDictionary[MEMCC.SelectedButtonDR].IdleAnimations;
        Walkable = DatabaseController.instance.TerrainDictionary[MEMCC.SelectedButtonDR].Walkable;
        DictionaryReferance = MEMCC.SelectedButtonDR;
        if (IdleAnimations)
        {
            IdleAnimationsDirectory = DatabaseController.instance.TerrainDictionary[MEMCC.SelectedButtonDR].IdleAnimationDirectory;
        }
        if (GameControllerScript.instance.UnitPos.ContainsKey(gameObject.transform.position) && DatabaseController.instance.TerrainDictionary[MEMCC.SelectedButtonDR].Walkable == false)
        {
            Destroy(GameControllerScript.instance.UnitPos[gameObject.transform.position]);
            GameControllerScript.instance.UnitPos.Remove(gameObject.transform.position);
        }
        if (GameControllerScript.instance.BuildingPos.ContainsKey(gameObject.transform.position) && DatabaseController.instance.TerrainDictionary[MEMCC.SelectedButtonDR].Walkable == false)
        {
            Destroy(GameControllerScript.instance.BuildingPos[gameObject.transform.position]);
            GameControllerScript.instance.BuildingPos.Remove(gameObject.transform.position);
        }
    } //if a terrain is selected and the player clicks a tile it changes the tile to the correct terrain

    private void MouseOverlayRayCaster()
    {
        if (gameObject.transform.tag == DatabaseController.instance.TerrainDictionary[0].Type)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); //find all objects under mouse
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray); //add them to array
                foreach (var hit in hits)
                {
                    if (hit.transform == transform) //was last hit this?
                    {
                        MouseOverlaySpriteRender.enabled = true;
                        if (Input.GetMouseButtonDown(0))
                        {
                            LastHitThis = true;
                        }
                    }
                    else if (hit.transform.tag == DatabaseController.instance.UnitDictionary[0].Type || hit.transform.tag == DatabaseController.instance.BuildingDictionary[0].Type)
                    {
                        //if tag is a unit or building do nothing.
                    }
                    else //else disable overlay
                    {
                        MouseOverlaySpriteRender.enabled = false;
                        if (Input.GetMouseButtonDown(0))
                        {
                            LastHitThis = false;
                        }
                    }
                }
            }
        }
    } //checks if mouse is over tile, activates mouseoverlay if it is.
    
    public void WaterSpriteController() 
    {
        if (Overlays) 
        {
            //UnityEngine.//Debug.Log("Starting Water Sprite Controller");
            var currentPos = (Vector2)transform.position;
            int TileId = 0;
            foreach (var kvp in DatabaseController.instance.TerrainDictionary)
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
            //UnityEngine.//Debug.Log("Starting if Statments");
            if (GameControllerScript.instance.TilePos.ContainsKey(topPos))
            {
                //Debug.Log("tile pos contains key");
                if (GameControllerScript.instance.TilePos[topPos].name != gameObject.name)//is the tle above this one not water?
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
                        RLO.AddComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].OverlayArtworkDirectory[1]);
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
            if (GameControllerScript.instance.TilePos.ContainsKey(topRightPos))
            {
                //Debug.Log("tile pos contains key");
                if (GameControllerScript.instance.TilePos[topRightPos].name != gameObject.name)//is the tle above this one not water?
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
                        RLO.AddComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].OverlayArtworkDirectory[0]);
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
            if (GameControllerScript.instance.TilePos.ContainsKey(rightPos))
            {
                //Debug.Log("tile pos contains key");
                if (GameControllerScript.instance.TilePos[rightPos].name != gameObject.name)//is the tle above this one not water?
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
                        RLO.AddComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].OverlayArtworkDirectory[1]);
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
            if (GameControllerScript.instance.TilePos.ContainsKey(bottomRightPos))
            {
                //Debug.Log("tile pos contains key");
                if (GameControllerScript.instance.TilePos[bottomRightPos].name != gameObject.name)//is the tle above this one not water?
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
                        RLO.AddComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].OverlayArtworkDirectory[0]);
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
            if (GameControllerScript.instance.TilePos.ContainsKey(bottomPos))
            {
                //Debug.Log("tile pos contains key");
                if (GameControllerScript.instance.TilePos[bottomPos].name != gameObject.name)//is the tle above this one not water?
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
                        RLO.AddComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].OverlayArtworkDirectory[1]);
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
            if (GameControllerScript.instance.TilePos.ContainsKey(bottomLeftPos))
            {
                //Debug.Log("tile pos contains key");
                if (GameControllerScript.instance.TilePos[bottomLeftPos].name != gameObject.name)//is the tle above this one not water?
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
                        RLO.AddComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].OverlayArtworkDirectory[0]);
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
            if (GameControllerScript.instance.TilePos.ContainsKey(leftPos))
            {
                //Debug.Log("tile pos contains key");
                if (GameControllerScript.instance.TilePos[leftPos].name != gameObject.name)//is the tle above this one not water?
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
                        RLO.AddComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].OverlayArtworkDirectory[1]);
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
            if (GameControllerScript.instance.TilePos.ContainsKey(topLeftPos))
            {
                //Debug.Log("tile pos contains key");
                if (GameControllerScript.instance.TilePos[topLeftPos].name != "Water")//is the tle above this one not water?
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
                        RLO.AddComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].OverlayArtworkDirectory[0]);
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
                WaterOverlays = new Dictionary<string, GameObject>(); ;
            }
        }
    } //Controlls the overlay for adding banks to the water sprites
    
    public void RoadSpriteController()
    { //gameObject.name == DatabaseController.instance.TerrainDictionary[3].Title
        if (Connectable)
        {
            var currentPos = (Vector2)transform.position;
            if (OriginalRot == null)
            {
                OriginalRot = gameObject.transform.eulerAngles;
            }
            var SR = gameObject.GetComponent<SpriteRenderer>();
            int TileId = 0;
            int counter = 0;
            foreach (var kvp in DatabaseController.instance.TerrainDictionary)
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

            if (GameControllerScript.instance.TilePos.ContainsKey(topPos))
            {
                if (GameControllerScript.instance.TilePos[topPos].name == gameObject.name)
                {
                    counter = counter + 1;
                    topBool = true;
                }
            }
            if (GameControllerScript.instance.TilePos.ContainsKey(rightPos))
            {
                if (GameControllerScript.instance.TilePos[rightPos].name == gameObject.name)
                {
                    counter = counter + 1;
                    rightbool = true;
                }
            }
            if (GameControllerScript.instance.TilePos.ContainsKey(bottomPos))
            {
                if (GameControllerScript.instance.TilePos[bottomPos].name == gameObject.name)
                {
                    counter = counter + 1;
                    bottombool = true;
                }
            }
            if (GameControllerScript.instance.TilePos.ContainsKey(leftPos))
            {
                if (GameControllerScript.instance.TilePos[leftPos].name == gameObject.name)
                {
                    counter = counter + 1;
                    leftbool = true;
                }
            }

            switch (counter)
            {
                case 0:
                    SR.sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].ArtworkDirectory[0]);
                    break;
                case 1:
                    SR.sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].ArtworkDirectory[1]);
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
                        SR.sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        gameObject.transform.eulerAngles = Road2way90TopRotOffset + OriginalRot;
                    }
                    else if (rightbool && bottombool)
                    {
                        SR.sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        gameObject.transform.eulerAngles = Road2way90RightRotOffset + OriginalRot;
                    }
                    else if (bottombool && leftbool)
                    {
                        SR.sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        gameObject.transform.eulerAngles = OriginalRot;
                    }
                    else if (leftbool && topBool)
                    {
                        SR.sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].ArtworkDirectory[2]);
                        gameObject.transform.eulerAngles = Road2way90LeftRotOffset + OriginalRot;
                    }
                    else if (topBool && bottombool)
                    {
                        SR.sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].ArtworkDirectory[3]);
                        gameObject.transform.eulerAngles = Road2wayStraightTopBottomRotOffset + OriginalRot;
                    }
                    else if (leftbool && rightbool)
                    {
                        SR.sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].ArtworkDirectory[3]);
                        gameObject.transform.eulerAngles = OriginalRot;
                    }
                    break;
                case 3:
                    SR.sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].ArtworkDirectory[4]);
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
                    SR.sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[TileId].ArtworkDirectory[5]);
                    gameObject.transform.eulerAngles = OriginalRot;
                    break;
            }
            counter = 0;
        }
    } //makes the roads attatch to each other

    public void FogOfWarController()
    {
        if (GameControllerScript.instance.CurrentScene == "MapEditorScene")
        {
            FogOfWar.enabled = false;
            FogOfWarBool = false;
        }
        else if (GameControllerScript.instance.CurrentScene == "PlayScene")
        {
            bool UnitOnMe = false;
            foreach(var kvp in GameControllerScript.instance.UnitPos)
            {
                if (kvp.Value.GetComponent<UnitController>().SightTiles.ContainsKey((Vector2)gameObject.transform.position) && kvp.Value.GetComponent<UnitController>().Team == GameControllerScript.instance.CurrentTeamsTurn.Team)
                {
                    FogOfWar.enabled = false;
                    FogOfWarBool = false;
                    break;
                }
                else if (!UnitOnMe)
                {
                    FogOfWar.enabled = true;
                    FogOfWarBool = true;
                }
                if (kvp.Key == (Vector2)gameObject.transform.position && kvp.Value.GetComponent<UnitController>().Team == GameControllerScript.instance.CurrentTeamsTurn.Team)
                {
                    FogOfWar.enabled = false;
                    FogOfWarBool = false;
                    UnitOnMe = true;
                    break;
                }
            }
            if (GameControllerScript.instance.BuildingPos.ContainsKey((Vector2)transform.position))
            {
                if (GameControllerScript.instance.BuildingPos[(Vector2)transform.position].GetComponent<BuildingController>().Team == GameControllerScript.instance.CurrentTeamsTurn.Team)
                {
                    FogOfWar.enabled = false;
                    FogOfWarBool = false;
                }
            }
        }
    }

    public void IdleAnimationController()
    {
        if (IdleAnimations)
        {
            if (GameControllerScript.instance.IdleState == 1)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(IdleAnimationsDirectory[1]);
                //Debug.Log("1");
            }
            else if (GameControllerScript.instance.IdleState == 2)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(IdleAnimationsDirectory[0]);
                //Debug.Log("2");
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(IdleAnimationsDirectory[1]);
                //Debug.Log("3");
            }
            
        }
    } //currently used to control idle animations for water, will need polished later when other idle animations are added. can use for referance for making unit and building animations
}
