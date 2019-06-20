﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System;
using UnityEngine.UI;

[System.Serializable]
public class DatabaseController : MonoBehaviour {

    //this is the second main controller script, everything that needs to be loaded into the game from streaming assets folder should be done through here. 
    //i will need to add a way for looking for more stuff that modders may add. like say a ability or something that is not already in the game. 
    //should potentialy be able to add game changing scripts though this at some point, will need some reasurch i think.

    public Dictionary<int, Terrain> TerrainDictionary = new Dictionary<int, Terrain>(); //stores the classes based on there id
    public Dictionary<int, MouseOverlays> MouseDictionary = new Dictionary<int, MouseOverlays>();
    public Dictionary<int, Unit> UnitDictionary = new Dictionary<int, Unit>();
    public Dictionary<int, Building> BuildingDictionary = new Dictionary<int, Building>();
    private GameObject NewTile;
    [HideInInspector]
    public bool Initalisation = true;
    public float dragSpeedOffset;
    [HideInInspector]
    public int DragSpeed;
    [HideInInspector]
    public int scrollSpeed;
    public GameObject CanvasPrefab;

    private void Start()
    {
        try
        {
            //GameControllerScript.instance = gameObject.GetComponent<GameControllerScript>();
            GetMasterJson();
            GameControllerScript.instance.LoadingUpdater(.2f);
            GetTerrianJsons();
            GameControllerScript.instance.LoadingUpdater(.4f);
            GetUnitJsons();
            GameControllerScript.instance.LoadingUpdater(.6f);
            GetBuildingJsons();
            GameControllerScript.instance.LoadingUpdater(.8f);
            GetMouseJson();
            GameControllerScript.instance.LoadingUpdater(1f);
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }

    public void GetMasterJson()
    {
        try
        {

            Debug.Log("Fetching json master file");
            var grassstring = File.ReadAllText(Application.dataPath + "/StreamingAssets/MasterData/Master.json"); //temp string to hold the json data
            Master tempjson = JsonUtility.FromJson<Master>(grassstring); //this converts from json string to unity object
            scrollSpeed = tempjson.ScrollSpeed;
            DragSpeed = tempjson.DragSpeed;
            Debug.Log("Finished fetching json master file");
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }

    public void GetTerrianJsons()
    {
        try
        {
            
            Debug.Log("Fetching json terrain files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Terrain/Data/", "*.json")) //gets only json files form this path
            {
                var grassstring = File.ReadAllText(file); //temp string to hold the json data
                Terrain tempjson = JsonUtility.FromJson<Terrain>(grassstring); //this converts from json string to unity object
                Debug.Log("Adding: " + tempjson.Slug + " to database");
                if (!TerrainDictionary.ContainsKey(tempjson.ID))
                {
                    TerrainDictionary.Add(tempjson.ID, tempjson); //adds
                    tempjson.GetSprites(); //details in fucntion
                    Debug.Log("Finished adding: " + tempjson.Slug + " to database"); 
                }else
                {
                    Debug.LogError("Item id number: " + tempjson.ID + " is already claimed, change " + tempjson.Title + " ID please");
                }
            }
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }//gets the json files from the terrain/data folder and converts them to unity object and stores tehm into dictionary

    public void GetUnitJsons()
    {
        try
        {
            Debug.Log("Fetching json unit files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Units/Data/", "*.json")) //gets only json files form this path
            {
                var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                var tempjson = JsonUtility.FromJson<Unit>(Tempstring); //this converts from json string to unity object
                Debug.Log("Adding: " + tempjson.Slug + " to database");
                if (!UnitDictionary.ContainsKey(tempjson.ID))
                {
                    UnitDictionary.Add(tempjson.ID, tempjson); //adds
                    tempjson.GetSprites(); //details in fucntion
                    Debug.Log("Finished adding: " + tempjson.Slug + " to database");
                }
                else
                {
                    Debug.LogError("Item id number: " + tempjson.ID + " is already claimed, change " + tempjson.Title + " ID please");
                }
            }
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }//gets the json files from the Units/data folder and converts them to unity object and stores tehm into dictionary

    public void GetBuildingJsons()
    {
        try
        {
            Debug.Log("Fetching json building files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Buildings/Data/", "*.json")) //gets only json files form this path
            {
                var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                var tempjson = JsonUtility.FromJson<Building>(Tempstring); //this converts from json string to unity object
                Debug.Log("Adding: " + tempjson.Slug + " to database");
                if (!BuildingDictionary.ContainsKey(tempjson.ID))
                {
                    BuildingDictionary.Add(tempjson.ID, tempjson); //adds
                    tempjson.GetSprites(); //details in fucntion
                    Debug.Log("Finished adding: " + tempjson.Slug + " to database");
                }
                else
                {
                    Debug.LogError("Item id number: " + tempjson.ID + " is already claimed, change " + tempjson.Title + " ID please");
                }
            }
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }

    public void GetMouseJson()
    {
        try
        {
            Debug.Log("Fetching json mouse overlay files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/MouseOverlays/Data/", "*.json"))
            {
                var TempString = File.ReadAllText(file);
                var tempjson = JsonUtility.FromJson<MouseOverlays>(TempString);
                Debug.Log("Adding: " + tempjson.Slug + " to database");
                if (!MouseDictionary.ContainsKey(tempjson.ID))
                {
                    MouseDictionary.Add(tempjson.ID, tempjson);
                    tempjson.GetSprites();
                    Debug.Log("Finished adding: " + tempjson.Slug + " to database");
                }
                else
                {
                    Debug.LogError("Item id number: " + tempjson.ID + " is already claimed" + tempjson.Title + " ID please");
                }
            }
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }//same as getTerrainJson

    public GameObject CreateAdnSpawnTerrain(Vector2 location, int index)
    {
        try
        {
            GameObject TGO = new GameObject();                                                                                  //create gameobject
            TGO.name = TerrainDictionary[index].Title;                                                                          //change the name
            TGO.AddComponent<SpriteRenderer>();                                                                                 //add a sprite controller
            TGO.GetComponent<SpriteRenderer>().sprite = loadSprite(TerrainDictionary[index].ArtworkDirectory[0]);               //set the sprite to the texture
            TGO.GetComponent<SpriteRenderer>().sortingLayerName = TerrainDictionary[index].Type;
            TGO.AddComponent<BoxCollider>();
            TGO.GetComponent<BoxCollider>().size = new Vector3(.95f, .95f, .1f);
            TGO.tag = (TerrainDictionary[index].Type);
            //TGO.AddComponent<RectTransform>();

            GameObject MouseOverlayGO = new GameObject();                                                                       //creating the child object for mouse overlay
            MouseOverlayGO.AddComponent<SpriteRenderer>().sprite = loadSprite(MouseDictionary[0].ArtworkDirectory[0]);          //adding it to sprite
            MouseOverlayGO.GetComponent<SpriteRenderer>().sortingLayerName = TerrainDictionary[index].Type;
            MouseOverlayGO.GetComponent<SpriteRenderer>().sortingOrder = 3;                                                     //making it so its on top of the default sprite
            MouseOverlayGO.transform.parent = TGO.transform;                                                                    //setting its parent to the main game object
            MouseOverlayGO.name = "MouseOverlay";                                                                               //changing the name
            TGO.AddComponent<TerrainController>();                                                                               //adding the sprite controller script to it
            TGO.GetComponent<TerrainController>().Weight = TerrainDictionary[index].Weight;
            TGO.GetComponent<TerrainController>().Walkable = TerrainDictionary[index].Walkable;
            TGO.GetComponent<TerrainController>().DefenceBonus = TerrainDictionary[index].DefenceBonus;
            TGO.transform.position = location;
            return TGO;
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //used to spawn terrian from database

    public GameObject CreateAndSpawnUnit(Vector2 location, int index, int team)
    {
        try
        {
            GameObject TGO = new GameObject();
            TGO.name = UnitDictionary[index].Title;
            TGO.AddComponent<SpriteRenderer>();
            TGO.GetComponent<SpriteRenderer>().sprite = loadSprite(UnitDictionary[index].ArtworkDirectory[0]);
            TGO.GetComponent<SpriteRenderer>().sortingLayerName = UnitDictionary[index].Type;
            TGO.AddComponent<BoxCollider>();
            TGO.GetComponent<BoxCollider>().size = new Vector3(.95f, .95f, .1f);
            TGO.AddComponent<UnitController>();
            TGO.GetComponent<UnitController>().Team = team;
            TGO.GetComponent<UnitController>().MovePoints = UnitDictionary[index].MovePoints;
            TGO.GetComponent<UnitController>().Attack = UnitDictionary[index].Attack;
            TGO.GetComponent<UnitController>().Defence = UnitDictionary[index].Defence;
            TGO.GetComponent<UnitController>().MaxHealth = UnitDictionary[index].Health;
            TGO.GetComponent<UnitController>().Health = UnitDictionary[index].Health;
            TGO.GetComponent<UnitController>().Range = UnitDictionary[index].Range;
            TGO.tag = UnitDictionary[index].Type;
            TGO.transform.position = location;

            GameObject TempCan = Instantiate(CanvasPrefab, TGO.transform);
            TempCan.transform.localPosition = new Vector3(0, 0, 0);
            TempCan.GetComponentInChildren<Text>().text = UnitDictionary[index].Health.ToString();
            TGO.GetComponent<UnitController>().UnitMovable = true;
            TGO.GetComponent<UnitController>().UnitMoved = false;
            GameControllerScript.instance.AddUnitsToDictionary(TGO);
            TGO.GetComponent<UnitController>().TeamSpriteController();
            return TGO;
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //used to spawn units form database

    public GameObject CreateAndSpawnBuilding(Vector2 location, int index, int team)
    {
        try
        {
            GameObject TGO = new GameObject();
            TGO.name = BuildingDictionary[index].Title;
            TGO.AddComponent<SpriteRenderer>();
            TGO.GetComponent<SpriteRenderer>().sprite = loadSprite(BuildingDictionary[index].ArtworkDirectory[0]);
            TGO.GetComponent<SpriteRenderer>().sortingLayerName = BuildingDictionary[index].Type;
            TGO.AddComponent<BoxCollider>();
            TGO.GetComponent<BoxCollider>().size = new Vector3(.95f, .95f, .1f);
            TGO.AddComponent<BuildingController>();
            TGO.GetComponent<BuildingController>().Team = team;
            TGO.tag = BuildingDictionary[index].Type;
            TGO.transform.position = location;
            return TGO;
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }

    public Sprite loadSprite(string FilePath)
    {
        try
        {
            Texture2D Tex2D;
            Sprite TempSprite;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(2, 2); // Create new "empty" texture
                Tex2D.LoadImage(FileData);         // Load the imagedata into the texture (size is set automatically)
                TempSprite = Sprite.Create(Tex2D, new Rect(0, 0, Tex2D.width, Tex2D.height), new Vector2(0.5f, 0.5f), 64);
                return TempSprite;
            }
            return null;
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }// checks for images that are to be used for artwork stuffs

}

[System.Serializable]
public class Terrain
{
    public int ID;
    public string Title;
    public bool Walkable;
    public string Description;
    public int DefenceBonus;
    public string Slug;
    public string Type;
    public int Weight;
    public List<string> ArtworkDirectory;


    public void GetSprites()
    {
        Debug.Log("Getting sprites for: " + Title);
        int count = new int(); //used for debug
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Terrain/Sprites/", "*.png"))) //could actaully put all png and json in same folder... idea for later
        {
            if (file.Contains(Title)) //checks if any of the files found contain the name of the object
            {
                ArtworkDirectory.Add(file); //adds if they do
                count = count + 1;
            }
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
        Debug.Log("Sprites found: " + count);
        count = 0;
    }  //checks how many sprites are in folder and adds them to the directory
}//the json file cannot have values that are not stated here, this can have more values then the json

public class MouseOverlays
{
    public int ID;
    public string Title;
    public string Slug;
    public string Type;
    public List<string> ArtworkDirectory;

    public void GetSprites()
    {
        Debug.Log("Getting sprites for: " + Title);
        int count = new int();
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/MouseOverlays/Sprites", "*.png")))
        {
            if (file.Contains(Title))
            {
                ArtworkDirectory.Add(file);
                count = count + 1;
            }
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
        Debug.Log("Sprites found: " + count);
        count = 0;
    }
}//same use as terrian class

[System.Serializable]
public class Unit
{
    public int ID;
    public int Attack;
    public int Defence;
    public int Range;
    public int Health;
    public int MovePoints;
    public int Cost;
    public string Title;
    public string Description;
    public string Slug;
    public string Type;
    public List<string> ArtworkDirectory;
    public int Team;

    public void GetSprites()
    {
        Debug.Log("Getting sprites for: " + Title);
        int count = new int();
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Units/Sprites", "*.png")))
        {
            if (file.Contains(Title))
            {
                ArtworkDirectory.Add(file);
                count = count + 1;
            }
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
        Debug.Log("Sprites found: " + count);
        count = 0;
    }
}//same use as terrian class

[System.Serializable]
public class Building
{
    public int ID;
    public string BuyableUnits;
    public string Title;
    public string Description;
    public string Slug;
    public int DefenceBonus;
    public bool Walkable;
    public string Type;
    public List<string> ArtworkDirectory;
    public int Team;

    public void GetSprites()
    {
        Debug.Log("Getting sprites for: " + Title);
        int count = new int();
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Buildings/Sprites", "*.png")))
        {
            if (file.Contains(Title))
            {
                ArtworkDirectory.Add(file);
                count = count + 1;
            }
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
        Debug.Log("Sprites found: " + count);
        count = 0;
    }
}//same use as terrian class

[System.Serializable]
public class Master
{
    public int DragSpeed;
    public int ScrollSpeed;
}



