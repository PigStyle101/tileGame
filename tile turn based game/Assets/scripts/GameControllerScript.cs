using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using UnityEngine.UI;

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
    private string CurrentScene;
    private List<string> LoadButtons;
    private DatabaseController DBC;
    private MapEditMenueCamController MEMCC;
    private PlaySceneCamController PSCC;
    [HideInInspector]
    public GameObject[] TileArray;
    [HideInInspector]
    public GameObject[] UnitArray;
    [HideInInspector]
    public SpriteRenderer SelectedTileOverlay;
    [HideInInspector]
    public GameObject SelectedTile;
    [HideInInspector]
    public int EditorMapSize;
    [HideInInspector]
    public int PlayMapSize;
    public GameObject SelectedUnitPlayScene = null;
    [HideInInspector]
    public string MapNameForPlayScene;
    [HideInInspector]
    public string PlaySceneLoadStatus;
    [HideInInspector]
    public int TeamCount;
    [HideInInspector]
    public int CurrentTeamsTurn;

    private void Awake()
    {
        DBC = gameObject.GetComponent<DatabaseController>(); //referance to database
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;//blabla
	}

    private void Update()
    {
        RayCastForMapEditor();
        RayCastForPlayScene();
    }

    public void CreateNewMapForMapEditor (int MapSize)
    {
        EditorMapSize = MapSize;
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
                for (int i = 0; i < EditorMapSize; i++)
                {
                    for (int o = 0; o < EditorMapSize; o++)
                    {
                        MapDictionary.Add(new Vector2(i, o), DBC.TerrainDictionary[0].Title);
                        //UnityEngine.Debug.Log("Added Key: " + i + o);
                    }
                }
                DrawNewMapForMapEditor();
                MapDictionary.Clear(); //clear dictionary for good measure
            }
            else if (sceneVar.name == "PlayScene")
            {
                PSCC = GameObject.Find("MainCamera").GetComponent<PlaySceneCamController>();
                if (PlaySceneLoadStatus == "NewGame")
                {
                    LoadMapPlayScene(MapNameForPlayScene);
                    PlaySceneNewGameInitalizer();
                }
            }
            else if (sceneVar.name  == "MainMenuScene")
            {

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
                GameObject go = DBC.CreateAdnSpawnTerrain(kvp.Key, 0);
                AddTilesToDictionary(go);
            }
            CameraVar = GameObject.Find("MainCamera");
            CameraVar.transform.position = new Vector3(EditorMapSize / 2 - .5f, EditorMapSize / 2 - .5f, EditorMapSize * -1);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
            throw;
        }
    }// create the physical part of the map and reset camera position to center

    public void AddTilesToDictionary (GameObject tgo)
    {
        if (TilePos.ContainsKey(tgo.transform.position))
        {
            TilePos.Remove(tgo.transform.position);
            TilePos.Add(tgo.transform.position, tgo);
        }
        else
        {
            TilePos.Add(tgo.transform.position, tgo);
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

    public void AddBuildingToDictionary(GameObject tgo)
    {
        if (BuildingPos.ContainsKey(tgo.transform.position))
        {
            BuildingPos.Remove(tgo.transform.position);
            BuildingPos.Add(tgo.transform.position, tgo);
        }
        else
        {
            BuildingPos.Add(tgo.transform.position, tgo);
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
        if (Input.GetMouseButtonDown(0) && CurrentScene == "MapEditorScene") //are we in map editor scene?
        {
            if (!EventSystem.current.IsPointerOverGameObject()) 
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
                        if (hit.transform.tag == DBC.TerrainDictionary[0].Type) //is it a terrain?
                        {
                            SpriteController SC = hit.transform.GetComponent<SpriteController>();
                            if (hit.transform.name != MEMCC.SelectedButton) //are we changing the tile to something new?
                            {
                                Debug.Log("Changing terrain to " + MEMCC.SelectedButton);
                                SC.ChangeTile(); //change tile to new terrain tile
                                AddTilesToDictionary(hit.transform.gameObject);
                                SpriteUpdateActivator();
                            }
                        }
                        else if (hit.transform.tag == DBC.UnitDictionary[0].Type) //is the hit a unit?
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
                                SpriteUpdateActivator();
                            }
                            else if (hit.transform.GetComponent<SpriteController>().Team != MEMCC.SelectedTeam)
                            {
                                hit.transform.GetComponent<SpriteController>().Team = MEMCC.SelectedTeam;
                                SpriteUpdateActivator();
                            }
                        }
                        else if (hit.transform.tag == DBC.BuildingDictionary[0].Type)
                        {
                            SpriteController SC = hit.transform.GetComponent<SpriteController>();
                            if (hit.transform.name != MEMCC.SelectedButton) //are we changing the building to something new?
                            {
                                Debug.Log("Changing building to " + MEMCC.SelectedButton);
                                if (MEMCC.SelectedButton == "Delete Building")
                                {
                                    BuildingPos.Remove(hit.transform.position);
                                }
                                SC.ChangeBuilding(); //change to new building
                                AddBuildingToDictionary(hit.transform.gameObject);
                                SpriteUpdateActivator();
                            }
                            else if (hit.transform.GetComponent<SpriteController>().Team != MEMCC.SelectedTeam)
                            {
                                hit.transform.GetComponent<SpriteController>().Team = MEMCC.SelectedTeam;
                                SpriteUpdateActivator();
                            }
                        }
                    }
                    else if (hit.transform.tag == DBC.TerrainDictionary[0].Type && MEMCC.SelectedTab == DBC.UnitDictionary[0].Type) //is the current hit not equal to what we are trying to place down. we are trying to place a unit.
                    {
                        if (!UnitPos.ContainsKey(hit.transform.position)) //is there a unit there already?
                        {
                            foreach (KeyValuePair<int, Unit> kvp in DBC.UnitDictionary)
                            {
                                if (kvp.Value.Title == MEMCC.SelectedButton) //need to find id for what unit we are trying to place
                                {
                                    bool tempbool = false;
                                    foreach (var item in DBC.TerrainDictionary)
                                    {
                                        if (hit.transform.name ==item.Value.Title)
                                        {;
                                            tempbool = item.Value.Walkable;
                                        }
                                    }
                                    if (tempbool)
                                    {
                                        GameObject tgo = DBC.CreateAndSpawnUnit(hit.transform.position, kvp.Key,MEMCC.SelectedTeam); //creat new unit at position on tile we clicked on.
                                        tgo.GetComponent<SpriteController>().Team = MEMCC.SelectedTeam;
                                        AddUnitsToDictionary(tgo);
                                        Debug.Log("Creating " + MEMCC.SelectedButton + " at " + hit.transform.position);
                                        SpriteUpdateActivator();
                                    }
                                }
                            }
                        }
                    }
                    else if (hit.transform.tag == DBC.TerrainDictionary[0].Type && MEMCC.SelectedTab == DBC.BuildingDictionary[0].Type)
                    {
                        if (!BuildingPos.ContainsKey(hit.transform.position))
                        {
                            foreach (KeyValuePair<int, Building> kvp in DBC.BuildingDictionary)
                            {
                                if (kvp.Value.Title == MEMCC.SelectedButton) //need to find id for what unit we are trying to place
                                {
                                    bool tempbool = false;
                                    foreach (var item in DBC.TerrainDictionary)
                                    {
                                        if (hit.transform.name == item.Value.Title)
                                        {
                                            tempbool = item.Value.Walkable;
                                        }
                                    }
                                    if (tempbool)
                                    {
                                        GameObject tgo = DBC.CreateAndSpawnBuilding(hit.transform.position, kvp.Key,MEMCC.SelectedTeam); //creat new building at position on tile we clicked on.
                                        tgo.GetComponent<SpriteController>().Team = MEMCC.SelectedTeam;
                                        AddBuildingToDictionary(tgo);
                                        Debug.Log("Creating " + MEMCC.SelectedButton + " at " + hit.transform.position);
                                        SpriteUpdateActivator();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }// used to check were mouse is hitting and then act acordingly

    public void RayCastForPlayScene()
    {
        if (Input.GetMouseButtonDown(0) && CurrentScene == "PlayScene") //are we in play scene?
        {
            if (!EventSystem.current.IsPointerOverGameObject()) //dont want to click through menus
            {
                Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); //GET THEM RAYS
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray);
                Debug.Log("Starting play scene ray hits");
                for (int i = 0; i < hits.Length; i++) // GO THROUGH THEM RAYS
                {
                    RaycastHit hit = hits[i];
                    if (hit.transform.tag == DBC.UnitDictionary[0].Type && SelectedUnitPlayScene == null && hit.transform.GetComponent<SpriteController>().Movable)
                    {//is the hit a unit? is the unit movable? is the unit not selected?
                        SelectedUnitPlayScene = hit.transform.gameObject; //set unit to selected unit
                        Debug.Log("SelectedUnit = " + SelectedUnitPlayScene.transform.name);
                    }
                    else if (hit.transform.tag == DBC.UnitDictionary[0].Type && SelectedUnitPlayScene == hit.transform.gameObject) //is the hit a unit? is the unit selected already?
                    {
                        SelectedUnitPlayScene = null; //clear selected unit variable
                        Debug.Log("Selected unit set to null");
                    }
                    else if (hit.transform.tag == DBC.TerrainDictionary[0].Type || hit.transform.tag == DBC.BuildingDictionary[0].Type) //is the hit a terrain or building?
                    {
                        Debug.Log("Building or terrain hit");
                        if (SelectedUnitPlayScene != null)
                        {
                            if (hit.transform.position != SelectedUnitPlayScene.transform.position) //is the building or terrain not the one the unit is standing on?
                            {
                                if (!UnitPos.ContainsKey(hit.transform.position))
                                {
                                    int originalPositionOfUnit = ((int)SelectedUnitPlayScene.transform.position.x) + ((int)SelectedUnitPlayScene.transform.position.y); //get unit position
                                    int MoveToPosition = ((int)hit.transform.position.x) + ((int)hit.transform.position.y); //get position we want to move to
                                    int MovePointsNeeded = System.Math.Abs(originalPositionOfUnit - MoveToPosition); //do teh math to get how many move points are needed
                                    Debug.Log("Move points needed:" + MovePointsNeeded);
                                    int MovePoints = 0; //delair a variable
                                    foreach (var kvp in DBC.UnitDictionary) //need to get the unit move points
                                    {
                                        if (SelectedUnitPlayScene.transform.name == kvp.Value.Title)
                                        {
                                            MovePoints = kvp.Value.MovePoints;
                                            Debug.Log("Move points avalible: " + MovePoints);
                                        }
                                    }
                                    if (MovePointsNeeded <= MovePoints) //does the unit have enough move points?
                                    {
                                        Debug.Log("Moving Unit");
                                        SelectedUnitPlayScene.transform.position = hit.transform.position; //move unit
                                        SelectedUnitPlayScene.GetComponent<SpriteController>().Movable = false; //set unit movable to false
                                        SelectedUnitPlayScene = null;
                                    }
                                    else
                                    {
                                        Debug.Log("Not enough move points");
                                    }
                                }
                                else
                                {
                                    Debug.Log("Unit in the way");
                                }
                            } 
                        }
                    }
                    else
                    {
                        Debug.Log("Nothing hit");
                    }
                }
            }
        }
    } //used to check were mouse is hitting and then act acordingly  //STILL WORKING ON THIS, NOT ENTIRLY SURE HOW I AM GOING TO DO THIS YET

    public void SaveMap(Dictionary<Vector2, GameObject> TP, Dictionary<Vector2, GameObject> UP,Dictionary<Vector2,GameObject> BP, string SaveName)
    {
        if (!System.IO.File.Exists(Application.dataPath + "/StreamingAssets/Maps/" + SaveName + ".json"))
        {
            if (SaveName != "")
            {
                if (!Regex.IsMatch(SaveName, @"^[a-z][A-Z]+$"))
                {
                    int count = 0;
                    TeamCount = 0;
                    Map[] save = new Map[TP.Count + UP.Count + BP.Count];
                    bool team1 = false;
                    bool team2 = false;
                    bool team3 = false;
                    foreach (KeyValuePair<Vector2, GameObject> kvp in TP)
                    {
                        if (kvp.Value != null)
                        {
                            save[count] = new Map();
                            save[count].Location = kvp.Key;
                            save[count].Name = kvp.Value.name;
                            save[count].Type = DBC.TerrainDictionary[0].Type;
                            count = count + 1; 
                        }
                    }
                    foreach (KeyValuePair<Vector2, GameObject> kvp in UP)
                    {
                        if (kvp.Value != null)
                        {
                            save[count] = new Map();
                            save[count].Location = kvp.Key;
                            save[count].Name = kvp.Value.name;
                            save[count].Type = DBC.UnitDictionary[0].Type;
                            save[count].Team = kvp.Value.GetComponent<SpriteController>().Team;
                            count = count + 1;
                            if (kvp.Value.GetComponent<SpriteController>().Team == 1)
                            {
                                team1 = true;
                            }
                            if (kvp.Value.GetComponent<SpriteController>().Team == 2)
                            {
                                team2 = true;
                            }
                            if (kvp.Value.GetComponent<SpriteController>().Team == 3)
                            {
                                team3 = true;
                            }
                        }
                    }
                    foreach (KeyValuePair<Vector2, GameObject> kvp in BP)
                    {
                        if (kvp.Value != null)
                        {
                            if (kvp.Value != null)
                            {
                                save[count] = new Map();
                                save[count].Location = kvp.Key;
                                save[count].Name = kvp.Value.name;
                                save[count].Type = DBC.BuildingDictionary[0].Type;
                                save[count].Team = kvp.Value.GetComponent<SpriteController>().Team;
                                count = count + 1;
                            }
                        }
                    }
                    if (team1)
                    {
                        TeamCount = TeamCount + 1;
                    }
                    if (team2)
                    {
                        TeamCount = TeamCount + 1;
                    }
                    if (team3)
                    {
                        TeamCount = TeamCount + 1;
                    }
                    if (TeamCount >= 2)
                    {
                        save[0].TeamCount = TeamCount;
                        string tempjson = JsonHelper.ToJson(save, true);
                        FileStream fs = File.Create(Application.dataPath + "/StreamingAssets/Maps/" + SaveName + ".json");
                        StreamWriter sr = new StreamWriter(fs);
                        sr.Write(tempjson);
                        sr.Close();
                        sr.Dispose();
                        fs.Close();
                        fs.Dispose();
                        MEMCC.SaveFeedback.text = "File saved as: " + SaveName;   
                    }
                    else
                    {
                        MEMCC.SaveFeedback.text = "Need to have more then 1 team to save a map";
                    }
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

    public void LoadMapMapEditor(string name)
    {
        StreamReader SR = new StreamReader( Application.dataPath + "/StreamingAssets/Maps/" + name + ".json");
        string tempstring = SR.ReadToEnd();
        Map[] Load = JsonHelper.FromJson<Map>(tempstring);


        var TilesToDelete = GameObject.FindGameObjectsWithTag(DBC.TerrainDictionary[0].Type);
        var UnitsToDelete = GameObject.FindGameObjectsWithTag(DBC.UnitDictionary[0].Type);
        var BuildingsToDelete = GameObject.FindGameObjectsWithTag(DBC.BuildingDictionary[0].Type);
        TilePos.Clear();
        UnitPos.Clear();
        BuildingPos.Clear();
        foreach (var GO in TilesToDelete)
        {
            Destroy(GO);
        }
        foreach (var GOU in UnitsToDelete)
        {
            Destroy(GOU);
        }
        foreach (var BGO in BuildingsToDelete)
        {
            Destroy(BGO);
        }

        for (int i = 0; i < Load.Length; i++)
        {
            if (Load[i].Type == DBC.TerrainDictionary[0].Type)
            {
                foreach(var kvp in DBC.TerrainDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        DBC.CreateAdnSpawnTerrain(Load[i].Location, kvp.Value.ID);
                    }
                }
            }
            else if (Load[i].Type == DBC.UnitDictionary[0].Type)
            {
                foreach (var kvp in DBC.UnitDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        DBC.CreateAndSpawnUnit(Load[i].Location, kvp.Value.ID,Load[i].Team);
                    }
                }
            }
            else if (Load[i].Type == DBC.BuildingDictionary[0].Type)
            {
                foreach (var kvp in DBC.BuildingDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        DBC.CreateAndSpawnBuilding(Load[i].Location, kvp.Value.ID, Load[i].Team);
                    }
                }
            }
        }

        foreach(var go in GameObject.FindGameObjectsWithTag(DBC.TerrainDictionary[0].Type))
        {
            AddTilesToDictionary(go);
        }
        foreach (var go in GameObject.FindGameObjectsWithTag(DBC.UnitDictionary[0].Type))
        {
            AddUnitsToDictionary(go);
        }
        foreach (var go in GameObject.FindGameObjectsWithTag(DBC.BuildingDictionary[0].Type))
        {
            AddBuildingToDictionary(go);
        }
        SR.Close();
        SR.Dispose();
        SpriteUpdateActivator();
    }// opens file and deseralizes it and then sends the info to drawLoadMapFromEditor function

    public void LoadMapPlayScene(string name)
    {
        StreamReader SR = new StreamReader(Application.dataPath + "/StreamingAssets/Maps/" + name + ".json");
        string tempstring = SR.ReadToEnd();
        Map[] Load = JsonHelper.FromJson<Map>(tempstring);

        for (int i = 0; i < Load.Length; i++)
        {
            if (Load[i].Type == DBC.TerrainDictionary[0].Type)
            {
                foreach (var kvp in DBC.TerrainDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        DBC.CreateAdnSpawnTerrain(Load[i].Location, kvp.Value.ID);
                        if (Load[i].Location.x > PlayMapSize)
                        {
                            PlayMapSize = (int)Load[i].Location.x;
                        }
                    }
                }
            }
            else if (Load[i].Type == DBC.UnitDictionary[0].Type)
            {
                foreach (var kvp in DBC.UnitDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        DBC.CreateAndSpawnUnit(Load[i].Location, kvp.Value.ID, Load[i].Team);
                    }
                }
            }
            else if (Load[i].Type == DBC.BuildingDictionary[0].Type)
            {
                foreach (var kvp in DBC.BuildingDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        DBC.CreateAndSpawnBuilding(Load[i].Location, kvp.Value.ID, Load[i].Team);
                    }
                }
            }
        }

        TeamCount = Load[0].TeamCount;
        Debug.Log("TeamCount = " + TeamCount);

        foreach (var go in GameObject.FindGameObjectsWithTag(DBC.TerrainDictionary[0].Type))
        {
            AddTilesToDictionary(go);
        }
        foreach (var go in GameObject.FindGameObjectsWithTag(DBC.UnitDictionary[0].Type))
        {
            AddUnitsToDictionary(go);
        }
        foreach (var go in GameObject.FindGameObjectsWithTag(DBC.BuildingDictionary[0].Type))
        {
            AddBuildingToDictionary(go);
        }
        SR.Close();
        SR.Dispose();
        SpriteUpdateActivator();
        CameraVar = GameObject.Find("MainCamera");
        CameraVar.transform.position = new Vector3(PlayMapSize / 2 - .5f, PlayMapSize / 2 - .5f, PlayMapSize * -1);
    }

    public void SpriteUpdateActivator()
    {
        foreach(var kvp in TilePos)
        {
            kvp.Value.GetComponent<SpriteController>().WaterSpriteController();
            kvp.Value.GetComponent<SpriteController>().RoadSpriteController();
        }
        foreach(var kvp in UnitPos)
        {
            if (kvp.Value != null)
            {
                kvp.Value.GetComponent<SpriteController>().TeamSpriteController(); 
            }
        }
        foreach (var kvp in BuildingPos)
        {
            if (kvp.Value != null)
            {
                kvp.Value.GetComponent<SpriteController>().TeamSpriteController(); 
            }
        }
    }

    public void PlaySceneNewGameInitalizer()
    {
        System.Random rnd = new System.Random();
        CurrentTeamsTurn = rnd.Next(1, TeamCount + 1);
        PSCC.CurrentPlayerTurnText.text = CurrentTeamsTurn.ToString();
        foreach (var kvp in UnitPos)
        {
            kvp.Value.GetComponent<SpriteController>().RoundUpdater();
        }
    }

    public void PlaySceneTurnChanger()
    {
        if (CurrentTeamsTurn != TeamCount)
        {
            CurrentTeamsTurn = CurrentTeamsTurn + 1;
        }
        else
        {
            CurrentTeamsTurn = 1;
        }
        PSCC.CurrentPlayerTurnText.text = CurrentTeamsTurn.ToString();
        foreach (var kvp in UnitPos)
        {
            kvp.Value.GetComponent<SpriteController>().RoundUpdater();
        }
    }

    public void LoadingUpdater(float f)
    {
        Slider LoadingSlider = GameObject.Find("Canvas").GetComponentInChildren<Slider>();
        LoadingSlider.value = f;
        if (f == 1f)
        {
            SceneManager.LoadScene("MainMenuScene");
            DBC.Initalisation = false;
        }
    }
}

[Serializable]
public class Map
{
    public SeralizableVector2 Location;
    public string Name;
    public int Team;
    public string Type;
    public int TeamCount;
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

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}