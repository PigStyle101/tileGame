﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System;
using UnityEngine.UI;
using System.Dynamic;
using System.Runtime.InteropServices.Expando;

[System.Serializable]
public class DatabaseController : MonoBehaviour
{

    //this is the second main controller script, everything that needs to be loaded into the game from streaming assets folder should be done through here. 
    //i will need to add a way for looking for more stuff that modders may add. like say a ability or something that is not already in the game. 
    //should potentialy be able to add game changing scripts though this at some point, will need some reasurch i think.

    public static DatabaseController instance = null;

    public Dictionary<int, Terrain> TerrainDictionary = new Dictionary<int, Terrain>(); //stores the classes based on there id
    public Dictionary<int, MouseOverlays> MouseDictionary = new Dictionary<int, MouseOverlays>();
    public Dictionary<int, Unit> UnitDictionary = new Dictionary<int, Unit>();
    public Dictionary<int, Building> BuildingDictionary = new Dictionary<int, Building>();
    public Dictionary<int, FogOfWar> FogOfWarDictionary = new Dictionary<int, FogOfWar>();
    public Dictionary<int, Hero> HeroDictionary = new Dictionary<int, Hero>();
    public Master MasterData = new Master();
    public List<string> ModsLoaded = new List<string>();
    private GameObject NewTile;
    [HideInInspector]
    public bool Initalisation = true;
    public float dragSpeedOffset;
    [HideInInspector]
    public int DragSpeed;
    [HideInInspector]
    public int scrollSpeed;
    private int NextTerrainDicIndex;
    private int NextUnitDicIndex;
    private int NextBuildingIndex;
    private int NextMouseOverlayIndex;
    private int NextFogIndex;
    private int NextHeroIndex;
    public GameObject UnitHealthOverlay;
    public GameObject BuildingHealthOverlay;
    private GameControllerScript GCS;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GCS = GameControllerScript.instance;
        GetMasterJson();
        //MultiplayerController.instance.Connect();
        GCS.LoadingUpdater(.2f);
        ResetNextIndexes();
        GetTerrianJsons("Core");
        GCS.LoadingUpdater(.4f);
        GetUnitJsons("Core");
        GetHeroJsons("Core");
        GCS.LoadingUpdater(.6f);
        GetBuildingJsons("Core");
        ModsLoaded.Add("Core");
        GCS.LoadingUpdater(.8f);
        GetMouseJson();
        GetFogOfWarJson();
        GCS.LoadingUpdater(1f);
    }

    /// <summary>
    /// Gets Master Json file, this is used for any bigger settings
    /// </summary>
    public void GetMasterJson()
    {
        //Debug.log("Fetching json master file");
        var grassstring = File.ReadAllText(Application.dataPath + "/StreamingAssets/MasterData/Master.json"); //temp string to hold the json data
        MasterData = JsonUtility.FromJson<Master>(grassstring); //this converts from json string to unity object
        scrollSpeed = MasterData.ScrollSpeed;
        DragSpeed = MasterData.DragSpeed;
        //Debug.Log("Finished fetching json master file");
    }

    /// <summary>
    /// Gets Terrain Json files. Pulls from data first then searches for sprites.
    /// </summary>
    /// <param name="Mod">Mod folder name to look for</param>
    public void GetTerrianJsons(string Mod)
    {
        if (Directory.Exists(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Terrain"))
        {
            //Debug.log("Fetching json terrain files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Terrain/Data/", "*.json")) //gets only json files form this path
            {
                var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                Terrain tempjson = JsonUtility.FromJson<Terrain>(Tempstring); //this converts from json string to unity object
                tempjson.ID = NextTerrainDicIndex;
                // Debug.Log("Adding: " + tempjson.Slug + " to database");

                TerrainDictionary.Add(tempjson.ID, tempjson); //adds
                tempjson.GetSprites(); //details in fucntion
                NextTerrainDicIndex = NextTerrainDicIndex + 1;
            }
        }
    }//gets the json files from the terrain/data folder and converts them to unity object and stores tehm into dictionary

    /// <summary>
    /// Gets Unit Json files. Pulls from data first then searches for sprites.
    /// </summary>
    /// <param name="Mod">Mod folder name to look for</param>
    public void GetUnitJsons(string Mod)
    {
        if (Directory.Exists(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units"))
        {
            //Debug.log("Fetching json unit files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units/Data/", "*.json")) //gets only json files form this path
            {
                var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                var tempjson = JsonUtility.FromJson<Unit>(Tempstring); //this converts from json string to unity object
                tempjson.ID = NextUnitDicIndex;
                //Debug.Log("Adding: " + tempjson.Slug + " to database");

                UnitDictionary.Add(tempjson.ID, tempjson); //adds
                tempjson.GetSprites(); //details in fucntion
                NextUnitDicIndex = NextUnitDicIndex + 1;
            }
        }
    }//gets the json files from the Units/data folder and converts them to unity object and stores tehm into dictionary

    /// <summary>
    /// Gets Hero Json files. Pulls from data first then searches for sprites.
    /// </summary>
    /// <param name="Mod">Mod folder name to look for</param>
    public void GetHeroJsons(string Mod)
    {
        if (Directory.Exists(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes"))
        {
            //Debug.Log("Fetching json hero files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes/Data/", "*.json")) //gets only json files form this path
            {
                var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                var tempjson = JsonUtility.FromJson<Hero>(Tempstring); //this converts from json string to unity object
                //Debug.Log("Adding: " + tempjson.Slug + " to database");
                tempjson.ID = NextHeroIndex;
                HeroDictionary.Add(tempjson.ID, tempjson); //adds
                tempjson.GetSprites(); //details in fucntion
                NextHeroIndex = NextHeroIndex + 1;
            }
        }
    }//gets the json files from the Units/data folder and converts them to unity object and stores tehm into dictionary

    /// <summary>
    /// Gets Building Json files. Pulls from data first then searches for sprites.
    /// </summary>
    /// <param name="Mod">Mod folder name to look for</param>
    public void GetBuildingJsons(string Mod)
    {
        if (Directory.Exists(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Buildings"))
        {
            //Debug.log("Fetching json building files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Buildings/Data/", "*.json")) //gets only json files form this path
            {
                var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                var tempjson = JsonUtility.FromJson<Building>(Tempstring); //this converts from json string to unity object
                //Debug.Log("Adding: " + tempjson.Slug + " to database");
                tempjson.ID = NextBuildingIndex;
                BuildingDictionary.Add(tempjson.ID, tempjson); //adds
                tempjson.GetSprites(); //details in fucntion
                NextBuildingIndex = NextBuildingIndex + 1;
            }
        }
    }

    /// <summary>
    /// Used to get mouse overlays for hovering over terrain tiles
    /// </summary>
    public void GetMouseJson()
    {
        //Debug.log("Fetching json mouse overlay files");
        foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/MouseOverlays/Data/", "*.json"))
        {
            var TempString = File.ReadAllText(file);
            var tempjson = JsonUtility.FromJson<MouseOverlays>(TempString);
            //Debug.Log("Adding: " + tempjson.Slug + " to database");
            tempjson.ID = NextMouseOverlayIndex;
            MouseDictionary.Add(tempjson.ID, tempjson);
            tempjson.GetSprites();
            NextMouseOverlayIndex = NextMouseOverlayIndex + 1;
        }
    }//same as getTerrainJson

    public void GetFogOfWarJson()
    {
        //Debug.log("Fetching json mouse overlay files");
        foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/FogOfWar/Data/", "*.json"))
        {
            var TempString = File.ReadAllText(file);
            var tempjson = JsonUtility.FromJson<FogOfWar>(TempString);
            // Debug.Log("Adding: " + tempjson.Slug + " to database");
            tempjson.ID = NextFogIndex;
            FogOfWarDictionary.Add(tempjson.ID, tempjson);
            tempjson.GetSprites();
            NextFogIndex = NextFogIndex + 1;
        }
    }

    /// <summary>
    /// Creates a terrain tile and adds all needed components.
    /// </summary>
    /// <param name="location">Location to spawn terrain tile at</param>
    /// <param name="index">Tile index in Terrain Dictionary</param>
    /// <returns></returns>
    public GameObject CreateAdnSpawnTerrain(Vector2 location, int index)
    {
        GameObject TGO = new GameObject();                                                                                  //create gameobject
        TGO.name = TerrainDictionary[index].Title;                                                                          //change the name
        TGO.AddComponent<SpriteRenderer>();                                                                                 //add a sprite controller
        TGO.GetComponent<SpriteRenderer>().sprite = TerrainDictionary[index].ArtworkDirectory[0];               //set the sprite to the texture
        TGO.GetComponent<SpriteRenderer>().sortingLayerName = TerrainDictionary[index].Type;
        TGO.AddComponent<BoxCollider>();
        TGO.GetComponent<BoxCollider>().size = new Vector3(.95f, .95f, .1f);
        TGO.tag = (TerrainDictionary[index].Type);
        //TGO.AddComponent<RectTransform>();

        GameObject MouseOverlayGO = new GameObject();                                                                       //creating the child object for mouse overlay
        MouseOverlayGO.AddComponent<SpriteRenderer>().sprite = MouseDictionary[0].ArtworkDirectory[0];          //adding it to sprite
        MouseOverlayGO.GetComponent<SpriteRenderer>().sortingLayerName = MouseDictionary[0].SortingLayer;
        MouseOverlayGO.GetComponent<SpriteRenderer>().sortingOrder = 4;                                                     //making it so its on top of the default sprite
        MouseOverlayGO.transform.parent = TGO.transform;                                                                    //setting its parent to the main game object
        MouseOverlayGO.name = "MouseOverlay";                                                                               //changing the name

        GameObject FogOverlay = new GameObject();
        FogOverlay.AddComponent<SpriteRenderer>().sprite = FogOfWarDictionary[0].ArtworkDirectory[0];
        FogOverlay.GetComponent<SpriteRenderer>().sortingLayerName = FogOfWarDictionary[0].SortingLayer;
        FogOverlay.GetComponent<SpriteRenderer>().sortingOrder = 3;
        FogOverlay.transform.parent = TGO.transform;
        FogOverlay.name = "FogOfWar";

        GameObject MouseOverlaySelected = new GameObject();
        MouseOverlaySelected.AddComponent<SpriteRenderer>().sprite = MouseDictionary[1].ArtworkDirectory[0];
        MouseOverlaySelected.GetComponent<SpriteRenderer>().sortingLayerName = MouseDictionary[1].SortingLayer;
        MouseOverlaySelected.GetComponent<SpriteRenderer>().sortingOrder = 5;
        MouseOverlaySelected.transform.parent = TGO.transform;
        MouseOverlaySelected.name = "MouseOverlaySelected";

        TGO.AddComponent<TerrainController>();                                                                               //adding the sprite controller script to it
        TGO.GetComponent<TerrainController>().Weight = TerrainDictionary[index].Weight;
        TGO.GetComponent<TerrainController>().Walkable = TerrainDictionary[index].Walkable;
        TGO.GetComponent<TerrainController>().DefenceBonus = TerrainDictionary[index].DefenceBonus;
        TGO.GetComponent<TerrainController>().BlocksSight = TerrainDictionary[index].BlocksSight;
        TGO.GetComponent<TerrainController>().Overlays = TerrainDictionary[index].Overlays;
        TGO.GetComponent<TerrainController>().Connectable = TerrainDictionary[index].Connectable;
        TGO.GetComponent<TerrainController>().IdleAnimations = TerrainDictionary[index].IdleAnimations;
        TGO.GetComponent<TerrainController>().ID = index;
        if (TerrainDictionary[index].IdleAnimations)
        {
            TGO.GetComponent<TerrainController>().IdleAnimationsDirectory = TerrainDictionary[index].IdleAnimationDirectory;
        }
        TGO.transform.position = location;
        return TGO;
    } //used to spawn terrian from database

    /// <summary>
    /// Creates a unit and adds all needed components.
    /// </summary>
    /// <param name="location">Location to spawn unit at</param>
    /// <param name="index">Unit index in unit dictionary</param>
    /// <param name="team">Team to assign to this unit</param>
    /// <returns></returns>
    public GameObject CreateAndSpawnUnit(Vector2 location, int index, int team)
    {
        GameObject TGO = new GameObject();
        TGO.name = UnitDictionary[index].Title;
        TGO.AddComponent<SpriteRenderer>();
        TGO.GetComponent<SpriteRenderer>().sprite = UnitDictionary[index].ArtworkDirectory[0];
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
        TGO.GetComponent<UnitController>().AttackRange = UnitDictionary[index].AttackRange;
        TGO.GetComponent<UnitController>().CanConvert = UnitDictionary[index].CanConvert;
        TGO.GetComponent<UnitController>().SightRange = UnitDictionary[index].SightRange;
        TGO.GetComponent<UnitController>().UnitIdleAnimation = UnitDictionary[index].IdleAnimations;
        TGO.GetComponent<UnitController>().IdleAnimationSpeed = UnitDictionary[index].IdleAnimationSpeed;
        TGO.GetComponent<UnitController>().ID = index;
        if (UnitDictionary[index].IdleAnimations)
        {
            TGO.GetComponent<UnitController>().IdleAnimationsDirectory = UnitDictionary[index].IdleAnimationDirectory;
            TGO.GetComponent<UnitController>().IdleAnimationCount = UnitDictionary[index].IdleAnimationDirectory.Count;
        }
        if (UnitDictionary[index].CanConvert)
        {
            TGO.GetComponent<UnitController>().ConversionSpeed = UnitDictionary[index].ConversionSpeed;
        }
        TGO.tag = UnitDictionary[index].Type;
        TGO.transform.position = location;

        GameObject TempCan = Instantiate(UnitHealthOverlay, TGO.transform);
        TempCan.transform.localPosition = new Vector3(0, 0, 0);
        TempCan.GetComponentInChildren<Text>().text = UnitDictionary[index].Health.ToString();
        TempCan.GetComponent<Canvas>().sortingLayerName = UnitDictionary[index].Type;
        int CaseInt = TGO.GetComponent<UnitController>().Team;
        switch (CaseInt)
        {
            case 1:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.black;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.white;
                break;
            case 2:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.blue;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 3:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.cyan;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 4:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.gray;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 5:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.green;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 6:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.magenta;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 7:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.red;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 8:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.white;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 9:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.yellow;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
        }

        TGO.GetComponent<UnitController>().UnitMovable = false;
        TGO.GetComponent<UnitController>().UnitMoved = true;
        TGO.GetComponent<UnitController>().CanMoveAndAttack = UnitDictionary[index].CanMoveAndAttack;
        //GCS.AddUnitsToDictionary(TGO);
        TGO.GetComponent<UnitController>().TeamSpriteController();
        return TGO;
    } //used to spawn units form database

    public GameObject CreateAndSpawnHero(Vector2 location, string Hname, int team)
    {
        GameObject TGO = new GameObject();
        Hero thero = GCS.HeroDictionary[Hname];
        TGO.name = Hname;
        TGO.AddComponent<SpriteRenderer>();
        TGO.GetComponent<SpriteRenderer>().sprite = HeroDictionary[thero.ID].ArtworkDirectory[0];
        TGO.GetComponent<SpriteRenderer>().sortingLayerName = UnitDictionary[thero.ID].Type;
        TGO.AddComponent<BoxCollider>();
        TGO.GetComponent<BoxCollider>().size = new Vector3(.95f, .95f, .1f);
        TGO.AddComponent<UnitController>();
        TGO.GetComponent<UnitController>().Name = Hname;
        TGO.GetComponent<UnitController>().Hero = true;
        TGO.GetComponent<UnitController>().Team = team;
        TGO.GetComponent<UnitController>().MovePoints = HeroDictionary[thero.ID].MovePoints;
        TGO.GetComponent<UnitController>().Attack = HeroDictionary[thero.ID].Attack;
        TGO.GetComponent<UnitController>().Defence = HeroDictionary[thero.ID].Defence;
        TGO.GetComponent<UnitController>().MaxHealth = HeroDictionary[thero.ID].Health;
        TGO.GetComponent<UnitController>().Health = HeroDictionary[thero.ID].Health;
        TGO.GetComponent<UnitController>().AttackRange = HeroDictionary[thero.ID].AttackRange;
        TGO.GetComponent<UnitController>().CanConvert = HeroDictionary[thero.ID].CanConvert;
        TGO.GetComponent<UnitController>().SightRange = HeroDictionary[thero.ID].SightRange;
        TGO.GetComponent<UnitController>().UnitIdleAnimation = HeroDictionary[thero.ID].IdleAnimations;
        TGO.GetComponent<UnitController>().IdleAnimationSpeed = HeroDictionary[thero.ID].IdleAnimationSpeed;
        TGO.GetComponent<UnitController>().ID = thero.ID;
        if (HeroDictionary[thero.ID].IdleAnimations)
        {
            TGO.GetComponent<UnitController>().IdleAnimationsDirectory = HeroDictionary[thero.ID].IdleAnimationDirectory;
            TGO.GetComponent<UnitController>().IdleAnimationCount = HeroDictionary[thero.ID].IdleAnimationDirectory.Count;
        }
        if (HeroDictionary[thero.ID].CanConvert)
        {
            TGO.GetComponent<UnitController>().ConversionSpeed = HeroDictionary[thero.ID].ConversionSpeed;
        }
        TGO.tag = HeroDictionary[thero.ID].Type;
        TGO.transform.position = location;

        GameObject TempCan = Instantiate(UnitHealthOverlay, TGO.transform);
        TempCan.transform.localPosition = new Vector3(0, 0, 0);
        TempCan.GetComponentInChildren<Text>().text = HeroDictionary[thero.ID].Health.ToString();
        TempCan.GetComponent<Canvas>().sortingLayerName = UnitDictionary[0].Type;
        int CaseInt = TGO.GetComponent<UnitController>().Team;
        switch (CaseInt)
        {
            case 1:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.black;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.white;
                break;
            case 2:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.blue;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 3:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.cyan;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 4:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.gray;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 5:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.green;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 6:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.magenta;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 7:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.red;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 8:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.white;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 9:
                TempCan.transform.Find("Image").GetComponent<Image>().color = Color.yellow;
                TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
        }

        TGO.GetComponent<UnitController>().UnitMovable = false;
        TGO.GetComponent<UnitController>().UnitMoved = true;
        TGO.GetComponent<UnitController>().CanMoveAndAttack = HeroDictionary[thero.ID].CanMoveAndAttack;
        //GCS.AddUnitsToDictionary(TGO);
        TGO.GetComponent<UnitController>().TeamSpriteController();
        return TGO;
    } //used to spawn units form database

    /// <summary>
    /// Creates a building and adds all needed components.
    /// </summary>
    /// <param name="location">Location to spawn unit at</param>
    /// <param name="index">Unit index in unit dictionary</param>
    /// <param name="team">Team to assign to this unit</param>
    /// <returns></returns>
    public GameObject CreateAndSpawnBuilding(Vector2 location, int index, int team)
    {
        GameObject TGO = new GameObject();
        TGO.name = BuildingDictionary[index].Title;
        TGO.AddComponent<SpriteRenderer>();
        TGO.GetComponent<SpriteRenderer>().sprite = BuildingDictionary[index].ArtworkDirectory[0];
        TGO.GetComponent<SpriteRenderer>().sortingLayerName = BuildingDictionary[index].Type;
        TGO.AddComponent<BoxCollider>();
        TGO.GetComponent<BoxCollider>().size = new Vector3(.95f, .95f, .1f);
        TGO.AddComponent<BuildingController>();
        TGO.GetComponent<BuildingController>().Team = team;
        TGO.GetComponent<BuildingController>().Mod = BuildingDictionary[index].Mod;
        TGO.GetComponent<BuildingController>().Health = BuildingDictionary[index].Health;
        TGO.GetComponent<BuildingController>().MaxHealth = BuildingDictionary[index].Health;
        TGO.GetComponent<BuildingController>().DefenceBonus = BuildingDictionary[index].DefenceBonus;
        TGO.GetComponent<BuildingController>().ID = index;
        TGO.GetComponent<BuildingController>().CanBuildUnits = BuildingDictionary[index].CanBuildUnits;
        TGO.GetComponent<BuildingController>().BuildableUnits = BuildingDictionary[index].BuildableUnits;
        TGO.GetComponent<BuildingController>().ID = index;
        TGO.tag = BuildingDictionary[index].Type;
        TGO.transform.position = location;
        GameObject TempCan = Instantiate(BuildingHealthOverlay, TGO.transform);
        TempCan.transform.localPosition = new Vector3(0, 0, 0);
        TempCan.GetComponentInChildren<Text>().text = BuildingDictionary[index].Health.ToString();
        TempCan.GetComponent<Canvas>().sortingLayerName = UnitDictionary[0].Type;
        return TGO;
    }

    /// <summary>
    /// Loads a sprite
    /// </summary>
    /// <param name="FilePath">Location to get file from</param>
    /// <returns></returns>
    public Sprite loadSprite(string FilePath, int PPU)
    {
        Texture2D Tex2D;
        Sprite TempSprite;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(64, 64); // Create new "empty" texture, this requires the size to be added, it get changed in teh next method
            Tex2D.LoadImage(FileData);// Load the imagedata into the texture (size is set automatically)
            TempSprite = Sprite.Create(Tex2D, new Rect(0, 0, Tex2D.width, Tex2D.height), new Vector2(0.5f, 0.5f), PPU);
            return TempSprite;
        }
        return null;
    }// checks for images that are to be used for artwork stuffs

    public void ResetNextIndexes()
    {
        NextBuildingIndex = 0;
        NextFogIndex = 0;
        NextHeroIndex = 0;
        NextMouseOverlayIndex = 0;
        NextTerrainDicIndex = 0;
        NextUnitDicIndex = 0;
    }
}

[System.Serializable]
public class Terrain
{
    public int ID; //used as dictionary referance
    public string Mod;
    public string Title;
    public bool Walkable;
    public string Description;
    public int DefenceBonus;
    public string Slug;
    public string Type;
    public int Weight;
    public int PixelsPerUnit;
    public bool BlocksSight;
    public bool Overlays;
    public bool Connectable;
    public bool IdleAnimations;
    public List<Sprite> ArtworkDirectory;
    public List<Sprite> OverlayArtworkDirectory;
    public List<Sprite> IdleAnimationDirectory;

    /// <summary>
    /// Gets location of sprites and saves them to a list
    /// </summary>
    public void GetSprites()
    {
        //Debug.Log("Getting sprites for: " + Title);
        int count = new int(); //used for debug
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Terrain/Sprites/" + Title +"/", "*.png"))) //could actaully put all png and json in same folder... idea for later
        {
            if (file.Contains(Title) && file.Contains("Overlay")) //checks if any of the files found contains the right words
            {
                OverlayArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                count = count + 1;
            }
            else if (file.Contains(Title) && file.Contains("Idle"))
            {
                IdleAnimationDirectory.Add(DatabaseController.instance.loadSprite(file,PixelsPerUnit));
                count = count + 1;
            }
            else if (file.Contains(Title))
            {
                ArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit)); //adds if they do
                count = count + 1;
            }
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
        //Debug.Log("Sprites found for " + Title + ": " + count + " in mod " + Mod);
        if (Title == "Water")
        {
            //Debug.Log("Overlays Contains:" + OverlayArtworkDirectory.Count);
            //Debug.Log("Idle Contains:" + IdleAnimationDirectory.Count);
            //Debug.Log("Artwork Contains:" + ArtworkDirectory.Count);
        }
        count = 0;
    }  //checks how many sprites are in folder and adds them to the directory
}//the json file cannot have values that are not stated here, this can have more values then the jso

[System.Serializable]
public class Unit
{
    public int ID;
    public string Mod;
    public int Attack;
    public int Defence;
    public int AttackRange;
    public int Health;
    public int MovePoints;
    public int Cost;
    public string Title;
    public string Description;
    public string Slug;
    public string Type;
    public bool IdleAnimations;
    public List<Sprite> ArtworkDirectory;
    public List<Sprite> IdleAnimationDirectory;
    public float IdleAnimationSpeed;
    public int Team;
    public int ConversionSpeed;
    public int PixelsPerUnit;
    public bool CanConvert;
    public bool CanMoveAndAttack;
    public int SightRange;

    /// <summary>
    /// Gets location of sprites and saves them to a list
    /// </summary>
    public void GetSprites()
    {
        //Debug.log("Getting sprites for: " + Title);
        int count = new int();
        if (IdleAnimations)
        {
            foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units/Sprites/" + Title + "/Idle/", "*.png")))
            {
                IdleAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                count = count + 1;
                //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
            } 
        }
        //Debug.Log("Sprites found for " + Title + ": " + count + " in mod " + Mod);
        count = 0;
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units/Sprites/" + Title + "/", "*.png")))
        {
            ArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
            count = count + 1;
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
    }
}//same use as terrian class

[System.Serializable]
public class Building
{
    public int ID;
    public string Mod;
    public List<string> BuildableUnits;
    public string Title;
    public string Description;
    public string Slug;
    public int DefenceBonus;
    public string Type;
    public List<Sprite> ArtworkDirectory;
    public int Team;
    public int Health;
    public int PixelsPerUnit;
    public bool Capturable;
    public bool CanBuildUnits;
    public bool BlocksSight;
    public bool OnlyOneAllowed;
    public bool HeroSpawnPoint;
    public bool MainBase;

    /// <summary>
    /// Gets location of sprites and saves them to a list
    /// </summary>
    public void GetSprites()
    {
        //Debug.log("Getting sprites for: " + Title);
        int count = new int();
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Buildings/Sprites/" + Title + "/", "*.png")))
        {
            if (file.Contains(Title))
            {
                ArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                count = count + 1;
            }
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
        //Debug.Log("Sprites found for " + Title + ": " + count + " in mod " + Mod);
        count = 0;
    }
}//same use as terrian class

public class MouseOverlays
{
    public int ID;
    public string Title;
    public string Slug;
    public string Type;
    public string SortingLayer = "Unit";
    public int PixelsPerUnit;
    public List<Sprite> ArtworkDirectory;

    /// <summary>
    /// Gets location of sprites and saves them to a list
    /// </summary>
    public void GetSprites()
    {
        //Debug.log("Getting sprites for: " + Title);
        int count = new int();
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/MouseOverlays/Sprites", "*.png")))
        {
            if (file.Contains(Title))
            {
                ArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                count = count + 1;
            }
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
        //Debug.Log("Sprites found for MO: " + count);
        count = 0;
    }
}//same use as terrian class

public class FogOfWar
{
    public int ID;
    public string Title;
    public string Slug;
    public string SortingLayer = "Unit";
    public int PixelsPerUnit;
    public List<Sprite> ArtworkDirectory;

    /// <summary>
    /// Gets location of sprites and saves them to a list
    /// </summary>
    public void GetSprites()
    {
        //Debug.log("Getting sprites for: " + Title);
        int count = new int();
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/FogOfWar/Sprites", "*.png")))
        {
            if (file.Contains(Title))
            {
                ArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                count = count + 1;
            }
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
       // Debug.Log("Sprites found for FogOfWar: " + count);
        count = 0;
    }
}//same use as terrian class

[System.Serializable]
public class Master
{
    public int DragSpeed;
    public int ScrollSpeed;
    public string HeroSelectedWhenGameClosed;
}

[System.Serializable]
public class Hero
{
    //Populated by GetHeroesJson();
    public int ID;
    //Populated by CreateNewHero();
    public string Name;
    //Populated by CreateNewHero(), and then updated when the hero lvls.
    public int Intelligance;
    public int Strenght;
    public int Dexterity;
    public int Charisma;
    //Populated from json
    public string Title;
    public string Slug;
    public string Description;
    public string Type;
    public string Mod;
    public int PixelsPerUnit;
    public int Attack;
    public int Defence;
    public int Health;
    public int AttackRange;
    public int MovePoints;
    public int ConversionSpeed;
    public int SightRange;
    public int Mana;
    public int HealthRegen;
    public int ManaRegen;
    public bool CanConvert;
    public bool CanMoveAndAttack;
    public int BaseIntelligance;
    public int BaseStrenght;
    public int BaseDexterity;
    public int BaseCharisma;
    public float IdleAnimationSpeed;
    public bool IdleAnimations;
    //Populated from GetSprites();
    public List<Sprite> ArtworkDirectory;
    public List<Sprite> IdleAnimationDirectory;
    //Blank tell used in game
    public int Team;
    public int Level;
    public int XP;

    /// <summary>
    /// Gets location of sprites and saves them to a list
    /// </summary>
    public void GetSprites()
    {
        //Debug.log("Getting sprites for: " + Title);
        int count = new int();
        if (IdleAnimations)
        {
            foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes/Sprites/" + Title + "/Idle/", "*.png")))
            {
                IdleAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                count = count + 1;
                //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
            }
        }
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/Core/Heroes/Sprites/" + Title + "/", "*.png")))
        {
            if (file.Contains(Title))
            {
                ArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                count = count + 1;
            }
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
        //Debug.Log("Sprites found: " + count);
        count = 0;
    }
}

public class Spell
{
    public string Title;
    public int ID;
    public string Slug;
    public string Description;
    public string Type;
    public string Mod;
    public int PixelsPerUnit;
    public List<string> Effects;
    public int ManaCost;
    public int Range;
    public int Damage;
    public int Healing;
    public bool SpawnUnit;
    public int UnitID;
}

public class Race
{
    public string Title;
    public int ID;
    public string Slug;
    public string Description;
    public string Type;
    public string Mod;
    public int PixelsPerUnit;
    public List<string> UsableUnits;
}

/// Inteligence affects: 
/// Mana Regen
/// Mana Max
/// Xp Gain
/// Spellcasting Chance
/// 
/// Strenght affects:
/// Damage
/// Health Points
/// 
/// Dexterity affects:
/// Move Points
/// Moves per turn
/// Armor
/// Amount of capture per turn
/// 
/// Charisma affects:
/// Cost of stuff
/// Troops Attack
/// Units in return

