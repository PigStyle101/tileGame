using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using LitJson;

public class GameControllerScript : MonoBehaviour {

    public int mapSize;
    private Dictionary <Vector2,string> MapDictionary = new Dictionary<Vector2,string>();
    public Dictionary<Vector2, GameObject> TilePos = new Dictionary<Vector2, GameObject>();
    private GameObject CameraVar;
    public GameObject[] TileArray;
    public GameObject SelectedTileOverlay;
    public GameObject SelectedTile;
    public DatabaseController DBC;

    private void Awake()
    {
        DontDestroyOnLoad(this); //need this to always be here
        DBC = gameObject.GetComponent<DatabaseController>(); //referance to database
    }

    void OnEnable ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;//blabla
	}

    public void CreateNewMap ()
    {
        SceneManager.LoadScene(2);//notes and stuff
    }

    private void OnSceneLoaded( Scene sceneVar , LoadSceneMode Mode) 
    {
        try
        {
            Debug.Log("OnSceneLoaded: " + sceneVar.name);
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
                DBC.spawnSomeShitTest(kvp.Key, 0);
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

    public void MouseSelectedController(GameObject STL, GameObject ST)
    {
        if (SelectedTileOverlay != null) { SelectedTileOverlay.SetActive(false); }
        SelectedTileOverlay = STL;
        SelectedTile = ST;
        SelectedTileOverlay.SetActive(true);
    }
}
