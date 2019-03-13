using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameControllerScript : MonoBehaviour {
    
    private Dictionary <Vector2,string> MapDictionary = new Dictionary<Vector2,string>();
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

    private void Awake()
    {
        DontDestroyOnLoad(this); //need this to always be here
        DBC = gameObject.GetComponent<DatabaseController>(); //referance to database
    }

    void OnEnable ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;//blabla
	}

    public void CreateNewMap (int MapSize)
    {
        mapSize = MapSize;
        SceneManager.LoadScene(2);//notes and stuff
    }

    private void OnSceneLoaded( Scene sceneVar , LoadSceneMode Mode) 
    {
        try
        {
            Debug.Log("OnSceneLoaded: " + sceneVar.name);//debug thing
            Debug.Log(Mode);

            if (sceneVar.name == "MapEditorScene")
            {
                for (int i = 0; i < mapSize; i++)
                {
                    for (int o = 0; o < mapSize; o++)
                    {
                        MapDictionary.Add(new Vector2(i, o), "Grass");
                        //UnityEngine.Debug.Log("Added Key: " + i + o);
                    }
                }
                DrawMap();
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

    private void DrawMap () 
    {
        try
        {
            foreach (var kvp in MapDictionary)
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
    }// create the physical part of the map and reset camera position to center                                 //////EVERY OTHER TILE SPAWNED IS AT 0,0 DONT KNOW WHY.....

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

    public void AddUnitsToDictionary()
    {
        UnitArray = GameObject.FindGameObjectsWithTag("Unit");
    } //needs finished

    public void MouseSelectedController(SpriteRenderer STL, GameObject ST)
    {
        if (SelectedTileOverlay != null) { SelectedTileOverlay.enabled = false; }
        SelectedTileOverlay = STL;
        SelectedTile = ST;
        SelectedTileOverlay.enabled = true;
    }// sets selected tile to whatever tile is clicked on and enables the clickon overlay

    public void UnitSelectedController(GameObject GO)
    {
        if (GO.tag == "Unit") { SelectedUnit = GO; } else { Debug.LogError("GameControllerScript unitSelectedController Failed"); }
    }

}
