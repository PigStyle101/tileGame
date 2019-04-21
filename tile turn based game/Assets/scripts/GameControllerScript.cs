using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class GameControllerScript : MonoBehaviour {
    
    //Everything should be able to be referanced and saved by id, or something off of the asset classes(ie:terrain,unit,building) in database manager
    //The reason for that is to make the game modable and easyer to add stuff to, as we will never need to ajdust the script unless
    //we are adding a new game mackanice, or something of that sort. i will figure out a way to add scripts to the game later that can override current script for modders.
    //there is a few things that i will need to change over to using a id, as i was just putting names and stuff in while i was figuring out how to make it all work.

    //this script is ment to be the controller uterly and completely, everything that needs to be stored or can make major adjustments should be ran through here,
    //database controller should be the only other major script. 

    private Dictionary <Vector2,string> MapDictionary = new Dictionary<Vector2,string>();// this string needs to be converted to a int id, 
    public Dictionary<Vector2, GameObject> TilePos = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> UnitPos = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> BuildingPos = new Dictionary<Vector2, GameObject>();
    private GameObject CameraVar;
    public GameObject[] TileArray;
    public GameObject[] UnitArray;
    public SpriteRenderer SelectedTileOverlay;
    public GameObject SelectedTile;
    public DatabaseController DBC;
    public int mapSize;
    public GameObject SelectedUnit;
    private MapEditMenueCamController MEMCC;
    private string CurrentScene;
    private List<string> LoadButtons;

    private void Awake()
    {
        DontDestroyOnLoad(this); //need this to always be here
        DBC = gameObject.GetComponent<DatabaseController>(); //referance to database
    }

    void OnEnable ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;//blabla
	}

    private void Update()
    {
        RayCastForMapEditor();
    }

    public void CreateNewMap (int MapSize)
    {
        mapSize = MapSize;
        SceneManager.LoadScene(2);//notes and stuff
    }//used to pull map size from menue controller script and set a varaible in here

    private void OnSceneLoaded( Scene sceneVar , LoadSceneMode Mode) 
    {
        try
        {
            Debug.Log("OnSceneLoaded: " + sceneVar.name);//debug thing
            CurrentScene = sceneVar.name;
            if (sceneVar.name == "MapEditorScene")
            {
                MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
                for (int i = 0; i < mapSize; i++)
                {
                    for (int o = 0; o < mapSize; o++)
                    {
                        MapDictionary.Add(new Vector2(i, o), "Grass");
                        //UnityEngine.Debug.Log("Added Key: " + i + o);
                    }
                }
                DrawNewMapForMapEditor();
                MapDictionary.Clear(); //clear dictionary for good measure
                AddTilesToDictionary();
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
            throw;
        }
    }//when map editor scene loads, create map of map size loaded with all grass

    private void DrawNewMapForMapEditor () 
    {
        try
        {
            foreach (var kvp in MapDictionary) //runs creation script for each 
            {
                DBC.CreateAdnSpawnTerrain(kvp.Key, 0); 
            }
            CameraVar = GameObject.Find("MainCamera");
            CameraVar.transform.position = new Vector3(mapSize / 2 - .5f, mapSize / 2 - .5f, mapSize * -1);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
            throw;
        }
    }// create the physical part of the map and reset camera position to center

    private void DrawLoadMapFromMapEditor(Map mapToLoad)
    {
        try
        {
            var TilesToDelete = GameObject.FindGameObjectsWithTag("Terrain");
            var UnitsToDelete = GameObject.FindGameObjectsWithTag("Unit");
            var BuildingsToDelete = GameObject.FindGameObjectsWithTag("Building");
            TilePos.Clear();
            UnitPos.Clear();
            BuildingPos.Clear();
            foreach(var GO in TilesToDelete)
            {
                Destroy(GO);
            }
            foreach(var GOU in UnitsToDelete)
            {
                Destroy(GOU);
            }
            foreach(var BGO in BuildingsToDelete)
            {
                Destroy(BGO);
            }
            foreach(var kvp in mapToLoad.TerrainPositions)
            {
                DBC.CreateAdnSpawnTerrain(kvp.Key, kvp.Value);
            }
            foreach(var kvp in mapToLoad.UnitPosition)
            {
                DBC.CreateAndSpawnUnit(kvp.Key, kvp.Value);
            }
            foreach(var kvp in mapToLoad.BuildingPositions)
            {
                DBC.CreateAndSpawnBuilding(kvp.Key, kvp.Value);
            }
            AddTilesToDictionary();
            foreach(GameObject UGO in GameObject.FindGameObjectsWithTag("Unit"))
            {
                AddUnitsToDictionary(UGO);
            }
            foreach(GameObject BGO in GameObject.FindGameObjectsWithTag("Building"))
            {
                AddBuildingToDictionary(BGO);
            }
            foreach(var kvp in TilePos)
            {
                kvp.Value.GetComponent<SpriteController>().RoadSpriteController();
                kvp.Value.GetComponent<SpriteController>().WaterSpriteController();
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }//working on this, need to adjust the position dictionarys to clear and repopulate when a new map is loaded

    public void AddTilesToDictionary ()
    {
        try
        {
            TileArray = GameObject.FindGameObjectsWithTag("Terrain"); //add all terrain to the array

            foreach (GameObject tile in TileArray)
            {
                Vector2 Position = tile.transform.position; //get current position of tile
                if (TilePos.ContainsKey(Position)) //does the dictionary contain the key for the current position?
                {
                    if (TilePos[Position] != tile) //is the tile in this position the correct one?
                    {
                        TilePos.Remove(Position);
                        TilePos.Add(Position, tile);
                    }
                }
                else
                {
                    TilePos.Add(Position, tile); //if not add the key to teh dictionary
                }
            }
            Array.Clear(TileArray, 0, TileArray.Length); // clear the array just for good measure 
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
            throw;
        }
    }//adds tiles to dicitonary as position for key and game object for value.

    public void AddUnitsToDictionary(GameObject tgo)
    {
        if (UnitPos.ContainsKey(tgo.transform.position))
        {
            UnitPos.Remove(tgo.transform.position);
            UnitPos.Add(tgo.transform.position, tgo);
        }
        else
        {
            UnitPos.Add(tgo.transform.position, tgo);
        }
        
    } //adds unit to dictionary and deletes unit if key is already taken

    public void AddBuildingToDictionary(GameObject bgo)
    {
        if (BuildingPos.ContainsKey(bgo.transform.position))
        {
            BuildingPos.Remove(bgo.transform.position);
            BuildingPos.Add(bgo.transform.position, bgo);
        }
        else
        {
            BuildingPos.Add(bgo.transform.position, bgo);
        }
    }

    public void MouseSelectedController(SpriteRenderer STL, GameObject ST)
    {
        if (SelectedTileOverlay != null) { SelectedTileOverlay.enabled = false; }
        SelectedTileOverlay = STL;
        SelectedTile = ST;
        SelectedTileOverlay.enabled = true;
    }// sets selected tile to whatever tile is clicked on and enables the clickon overlay

    public void RayCastForMapEditor()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0) && CurrentScene == "MapEditorScene") //are we in map editor scene?
            {
                //Debug.Log("Raycast activated");
                Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray);
                //Debug.Log(hits.Length);
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];
                    if (hit.transform.tag == MEMCC.SelectedTab) //is the hit tag = to what we are trying to place down?
                    {
                        if (hit.transform.tag == "Terrain") //is it a terrain?
                        {
                            SpriteController SC = hit.transform.GetComponent<SpriteController>();
                            if (hit.transform.name != MEMCC.SelectedButton) //are we changing the tile to something new?
                            {
                                Debug.Log("Changing unit to " + MEMCC.SelectedButton);
                                SC.ChangeTile(); //change tile to new terrain tile
                                AddTilesToDictionary();
                                SpriteUpdateActivator();
                            }
                        }
                        else if (hit.transform.tag == "Unit") //is the hit a unit?
                        {
                            SpriteController SC = hit.transform.GetComponent<SpriteController>();
                            if (hit.transform.name != MEMCC.SelectedButton) //are we changing the unit to something new?
                            {
                                Debug.Log("Changing unit to " + MEMCC.SelectedButton);
                                if (MEMCC.SelectedButton == "Delete Unit")
                                {
                                    UnitPos.Remove(hit.transform.position);
                                }
                                SC.ChangeUnit(); //change to new unit
                                AddUnitsToDictionary(hit.transform.gameObject);
                            }
                        }
                        else if (hit.transform.tag == "Building")
                        {
                            SpriteController SC = hit.transform.GetComponent<SpriteController>();
                            if (hit.transform.name != MEMCC.SelectedButton) //are we changing the unit to something new?
                            {
                                Debug.Log("Changing building to " + MEMCC.SelectedButton);
                                if (MEMCC.SelectedButton == "Delete Building")
                                {
                                    BuildingPos.Remove(hit.transform.position);
                                }
                                SC.ChangeBuilding(); //change to new unit
                                AddBuildingToDictionary(hit.transform.gameObject);
                            }
                        }
                    }
                    else if (hit.transform.tag == "Terrain" && MEMCC.SelectedTab == "Unit") //is the current hit not equal to what we are trying to place down. we are trying to place a unit.
                    {
                        if (!UnitPos.ContainsKey(hit.transform.position)) //is there a unit there already?
                        {
                            foreach (KeyValuePair<int, Unit> kvp in DBC.UnitDictionary)
                            {
                                if (kvp.Value.Title == MEMCC.SelectedButton) //need to find id for what unit we are trying to place
                                {
                                    GameObject tgo = DBC.CreateAndSpawnUnit(hit.transform.position, kvp.Key); //creat new unit at position on tile we clicked on.
                                    AddUnitsToDictionary(tgo);
                                    Debug.Log("Creating " + MEMCC.SelectedButton + " at " + hit.transform.position);
                                }
                            }
                        }
                    }
                    else if (hit.transform.tag == "Terrain" && MEMCC.SelectedTab == "Building")
                    {
                        if (!BuildingPos.ContainsKey(hit.transform.position))
                        {
                            foreach (KeyValuePair<int, Building> kvp in DBC.BuildingDictionary)
                            {
                                if (kvp.Value.Title == MEMCC.SelectedButton) //need to find id for what unit we are trying to place
                                {
                                    GameObject tgo = DBC.CreateAndSpawnBuilding(hit.transform.position, kvp.Key); //creat new building at position on tile we clicked on.
                                    AddBuildingToDictionary(tgo);
                                    Debug.Log("Creating " + MEMCC.SelectedButton + " at " + hit.transform.position);
                                }
                            }
                        }
                    }
                }
            }
        }

    }// used to check were mouse is hitting and then act acordingly

    public void SaveMap(Dictionary<Vector2, GameObject> TP, Dictionary<Vector2, GameObject> UP,Dictionary<Vector2,GameObject> BP, string SaveName)
    {
        if (!System.IO.File.Exists(Application.dataPath + "/StreamingAssets/Saves/" + SaveName + ".dat"))
        {
            if (SaveName != "")
            {
                if (!Regex.IsMatch(SaveName, @"^[a-z][A-Z]+$"))
                {
                    Map save = new Map();
                    foreach (KeyValuePair<Vector2, GameObject> kvp in TP)
                    {
                        foreach (var kvvp in DBC.TerrainDictionary)
                        {
                            if (kvp.Value.transform.name == kvvp.Value.Title)
                            {
                                save.TerrainPositions.Add(kvp.Key, kvvp.Key);
                            }
                        }
                    }
                    foreach (KeyValuePair<Vector2, GameObject> ukvp in UP)
                    {
                        if (ukvp.Value != null)
                        {
                            foreach (var keyvp in DBC.UnitDictionary)
                            {
                                if (ukvp.Value.transform.name == keyvp.Value.Title)
                                {
                                    save.UnitPosition.Add(ukvp.Key, keyvp.Key);
                                }
                            }
                        }
                    }
                    foreach (KeyValuePair<Vector2, GameObject> bkvp in BP)
                    {
                        if (bkvp.Value != null)
                        {
                            foreach (var keyvvp in DBC.BuildingDictionary)
                            {
                                if (bkvp.Value.transform.name == keyvvp.Value.Title)
                                {
                                    save.BuildingPositions.Add(bkvp.Key, keyvvp.Key);
                                }
                            } 
                        }
                    }
                    string destination = Application.dataPath + "/StreamingAssets/Saves/" + SaveName + ".dat";
                    var fs = File.Create(destination);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, save);
                    fs.Close();
                    MEMCC.SaveFeedback.text = "File saved as: " + SaveName;  
                }
                else
                {
                    MEMCC.SaveFeedback.text = "Only use letters";
                }
            }
            else
            {
                MEMCC.SaveFeedback.text = "Add a name.";
            }
        }
        else
        {
            MEMCC.SaveFeedback.text = "Current file name is already in use, please rename your save or delete old file.";
        }
    }//checks if file already exsist and if it does not it creates new file and stores current map items into dictionarys as (location,id number)

    public void LoadMap(string name)
    {
        Map load = new Map();
        string destination = Application.dataPath + "/StreamingAssets/Saves/" + name + ".dat";
        FileStream fs = File.OpenRead(destination);
        BinaryFormatter bf = new BinaryFormatter();
        load = (Map)bf.Deserialize(fs);
        fs.Close();

        if (SceneManager.GetActiveScene().name == "MapEditorScene")
        {
            DrawLoadMapFromMapEditor(load);
        }
    }// opens file and deseralizes it and then sends the info to drawLoadMapFromEditor function

    public void SpriteUpdateActivator()
    {
        foreach(var kvp in TilePos)
        {
            kvp.Value.GetComponent<SpriteController>().WaterSpriteController();
            kvp.Value.GetComponent<SpriteController>().RoadSpriteController();
        }
    }
}

[Serializable]
public class Map
{
    public Dictionary<SeralizableVector2, int> TerrainPositions = new Dictionary<SeralizableVector2, int>();
    public Dictionary<SeralizableVector2, int> UnitPosition = new Dictionary<SeralizableVector2, int>();
    public Dictionary<SeralizableVector2, int> BuildingPositions = new Dictionary<SeralizableVector2, int>();
} //this is needed to save the map, might be better way to do this.

[Serializable]
public class SeralizableVector2
{
    public float x;
    public float y;

    public SeralizableVector2(float rx,float ry)
    {
        x = rx;
        y = ry;
    }

    public static implicit operator Vector2 (SeralizableVector2 rValue)
    {
        Vector2 newVec = new Vector2(rValue.x, rValue.y);
        return newVec;
    }

    public static implicit operator SeralizableVector2(Vector2 Value)
    {
        return new SeralizableVector2(Value.x, Value.y);
    }
} //this is needed for seralization, as vectors from unity are not seralizable.