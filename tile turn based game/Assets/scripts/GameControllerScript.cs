using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameControllerScript : MonoBehaviour {
    
    //Everything should be able to be referanced and saved by id, or something off of the asset classes(ie:terrain,unit,building) in database manager
    //The reason for that is to make the game modable and easyer to add stuff to, as we will never need to ajdust the script unless
    //we are adding a new game mackanice, or something of that sort. i will figure out a way to add scripts to the game later that can override current script for modders.
    //there is a few things that i will need to change over to using a id, as i was just putting names and stuff in while i was figuring out how to make it all work.

    //this script is ment to be the controller uterly and completely, everything that needs to be stored or can make major adjustments should be ran through here,
    //database controller should be the only other major script. 
    //there is probably functions i need to move into here, but for now it was easyer to write them as i go and in the simplest way.

    private Dictionary <Vector2,string> MapDictionary = new Dictionary<Vector2,string>();// this string needs to be converted to a int id, 
    public Dictionary<Vector2, GameObject> TilePos = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> UnitPos = new Dictionary<Vector2, GameObject>();
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
        RayCastTesterMethod();
    }

    public void CreateNewMap (int MapSize)
    {
        mapSize = MapSize;
        SceneManager.LoadScene(2);//notes and stuff
    }//does what it says

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
            var UnitsToDelete = GameObject.FindGameObjectsWithTag("Terrain");
            foreach(var GO in TilesToDelete)
            {
                Destroy(GO);
            }
            foreach(var GOU in UnitsToDelete)
            {
                Destroy(GOU);
            }
            foreach(var kvp in mapToLoad.TerrainPositions)
            {
                DBC.CreateAdnSpawnTerrain(kvp.Key, kvp.Value);
            }
            foreach(var kvp in mapToLoad.UnitPosition)
            {
                DBC.CreateAndSpawnUnit(kvp.Key, kvp.Value);
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

    public void MouseSelectedController(SpriteRenderer STL, GameObject ST)
    {
        if (SelectedTileOverlay != null) { SelectedTileOverlay.enabled = false; }
        SelectedTileOverlay = STL;
        SelectedTile = ST;
        SelectedTileOverlay.enabled = true;
    }// sets selected tile to whatever tile is clicked on and enables the clickon overlay

    public void RayCastTesterMethod()
    {
        try
        {
            if (Input.GetMouseButtonDown(0) && CurrentScene == "MapEditorScene")
            {
                //Debug.Log("Raycast activated");
                Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray);
                //Debug.Log(hits.Length);
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];
                    if (hit.transform.tag == MEMCC.SelectedTab)
                    {
                        if (hit.transform.tag == "Terrain")
                        {
                            Debug.Log("Raycast hit: " + hit.transform.name);
                            TerrainSpriteController TSC = hit.transform.GetComponent<TerrainSpriteController>();
                            if (hit.transform.name != MEMCC.SelectedButton)
                            {
                                TSC.ChangeTile();
                            } 
                        }
                        else if (hit.transform.tag == "Unit")
                        {
                            Debug.Log("Raycast hit: " + hit.transform.name);
                            UnitSpriteController UPC = hit.transform.GetComponent<UnitSpriteController>();
                            if(hit.transform.name != MEMCC.SelectedButton)
                            {
                                UPC.ChangeUnit();
                            }
                        }
                    }
                    else if (hit.transform.tag == "Terrain")
                    {
                        if (!UnitPos.ContainsKey(hit.transform.position))
                        {
                            Debug.Log("Raycast hit: " + hit.transform.name);
                            foreach (KeyValuePair<int, Unit> kvp in DBC.UnitDictionary)
                            {
                                if (kvp.Value.Title == MEMCC.SelectedButton)
                                {
                                    GameObject tgo = DBC.CreateAndSpawnUnit(hit.transform.position, kvp.Key);
                                    AddUnitsToDictionary(tgo);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }// used to check were mouse is hitting and then act acordingly

    public void SaveMap(Dictionary<Vector2, GameObject> TP, Dictionary<Vector2, GameObject> UP,string SaveName)
    {
        if (!System.IO.File.Exists(Application.dataPath + "/StreamingAssets/Saves/" + SaveName + ".dat"))
        {
            Map save = new Map();
            foreach (KeyValuePair<Vector2, GameObject> kvp in TP)
            {
                foreach(var kvvp in DBC.TerrainDictionary)
                {
                    if (kvp.Value.transform.name == kvvp.Value.Title)
                    {
                        save.TerrainPositions.Add(kvp.Key, kvvp.Key);
                    }
                }
            }
            foreach (KeyValuePair<Vector2, GameObject> kvp in UP)
            {
                foreach(var keyvp in DBC.UnitDictionary)
                {
                    if (kvp.Value.transform.name == keyvp.Value.Title)
                    {
                        save.UnitPosition.Add(kvp.Key, keyvp.Key);
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

        if (SceneManager.GetActiveScene().name == "MapEditorScene")
        {
            DrawLoadMapFromMapEditor(load);
        }
    }// working on this, need to add a load function into the map editor.
}

[Serializable]
public class Map
{
    public Dictionary<SeralizableVector2, int> TerrainPositions = new Dictionary<SeralizableVector2, int>();
    public Dictionary<SeralizableVector2, int> UnitPosition = new Dictionary<SeralizableVector2, int>();
}
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
}