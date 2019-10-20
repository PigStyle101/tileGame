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

public class GameControllerScript : MonoBehaviour
{

    //Everything should be able to be referanced and saved by id, or something off of the asset classes(ie:terrain,unit,building) in database manager
    //The reason for that is to make the game modable and easyer to add stuff to, as we will never need to ajdust the script unless
    //we are adding a new game mackanice, or something of that sort. i will figure out a way to add scripts to the game later that can override current script for modders.
    //there is a few things that i will need to change over to using a id, as i was just putting names and stuff in while i was figuring out how to make it all work.

    //this script is ment to be the controller utterly and completely, everything that needs to be stored or can make major adjustments should be ran through here,
    //database controller should be the only other major script. 

    public static GameControllerScript instance = null;

    private Dictionary<Vector2, string> MapDictionary = new Dictionary<Vector2, string>();// this string needs to be converted to a int id, 
    public Dictionary<Vector2, GameObject> TilePos = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> UnitPos = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> BuildingPos = new Dictionary<Vector2, GameObject>();
    private GameObject CameraVar;
    [HideInInspector]
    public string CurrentScene;
    private List<string> LoadButtons;
    private MapEditMenueCamController MEMCC;
    private PlaySceneCamController PSCC;
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
    public List<TeamStuff> TeamList; //contains team, ai bool, gold, unitcount, and building count
    [HideInInspector]
    public TeamStuff CurrentTeamsTurn = new TeamStuff();
    //[HideInInspector]
    //public int CurrentTeamsTurnIndex;
    private Vector2 originalPositionOfUnit;
    private Vector2 MoveToPosition;
    public AudioClip BattleAudio;
    public AudioClip MainAudio;
    public AudioClip MapEditorAudio;
    [HideInInspector]
    public string LogFile = "log.txt";
    public bool EchoToConsole = true;
    public bool AddTimeStamp = true;
    //public List<int> TeamGold; //Team,Gold
    //public List<AiOrNot> AiOrPlayerList; //0 = player, 1 =ai
    private string MapToLoadForMapEditor;
    private bool MapEditorNewMapBool;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;//blabla
    }

    private void Update()
    {
        RayCastForMapEditor();
        RayCastForPlayScene();
    }

    /// <summary>
    /// Saves crash info to DebugLogs file in streaming assets.
    /// </summary>
    /// <param name="Problem">String to be saved to file</param>
    public void LogController(string Problem)
    {
        StreamWriter writer = new StreamWriter(Application.dataPath + "/StreamingAssets/DebugLogs/" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".txt");
        writer.WriteLine(Problem);
        writer.Close();
        writer.Dispose();
    }

    /// <summary>
    /// Saves the map size and loads Map Editor Scene
    /// </summary>
    /// <param name="MapSize">Size of map in x and y</param>
    public void CreateNewMapForMapEditor(int MapSize)
    {
        EditorMapSize = MapSize;
        MapEditorNewMapBool = true;
        SceneManager.LoadScene("MapEditorScene");//notes and stuff
    }

    public void LoadMapForMapEditorFromMainMenu(string map)
    {
        MapToLoadForMapEditor = map;
        MapEditorNewMapBool = false;
        SceneManager.LoadScene("MapEditorScene");
    }

    /// <summary>
    /// Checks what scene was loaded then executes required actions
    /// </summary>
    /// <param name="sceneVar">Name of scene that was loaded</param>
    /// <param name="Mode">Not even sure this is needed</param>
    private void OnSceneLoaded(Scene sceneVar, LoadSceneMode Mode) //do i need LoadSceneMode here?
    {
        //Debug.log("OnSceneLoaded: " + sceneVar.name);//debug thing
        CurrentScene = sceneVar.name;
        if (sceneVar.name == "MapEditorScene")
        {
            if (MapEditorNewMapBool)
            {
                MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
                for (int i = 0; i < EditorMapSize; i++)
                {
                    for (int o = 0; o < EditorMapSize; o++)
                    {
                        //creates a map in data to use for drawing later
                        MapDictionary.Add(new Vector2(i, o), DatabaseController.instance.TerrainDictionary[0].Title);
                        //UnityEngine.//Debug.log("Added Key: " + i + o);
                    }
                }
                DrawNewMapForMapEditor();
                MapDictionary = new Dictionary<Vector2, string>(); //clear dictionary for good measure 
            }
            else
            {
                LoadMapMapEditor(MapToLoadForMapEditor);
            }
        }
        else if (sceneVar.name == "PlayScene")
        {
            PSCC = GameObject.Find("MainCamera").GetComponent<PlaySceneCamController>();
            if (PlaySceneLoadStatus == "NewGame")
            {
                LoadMapPlayScene(MapNameForPlayScene);
                PlaySceneNewGameInitalizer();
            }
            else if (PlaySceneLoadStatus == "SavedGame")
            {
                LoadSavedGamePlayScene(MapNameForPlayScene);
                PlaySceneLoadGameInitalizer();
            }
        }
        else if (sceneVar.name == "MainMenuScene")
        {
            
        }
        //AudioController(sceneVar.name);
    }

    /// <summary>
    /// Creates the graphical representation of the map and sets the camera to center of map
    /// </summary>
    private void DrawNewMapForMapEditor()
    {
        foreach (var kvp in MapDictionary) //runs creation script for each 
        {
            GameObject go = DatabaseController.instance.CreateAdnSpawnTerrain(kvp.Key, 0);
            AddTilesToDictionary(go);
        }
        CameraVar = GameObject.Find("MainCamera");
        CameraVar.transform.position = new Vector3(EditorMapSize / 2 - .5f, EditorMapSize / 2 - .5f, EditorMapSize * -1);
    }

    /// <summary>
    /// Checks if TilePos contains terrains tile and replaces or adds it.
    /// </summary>
    /// <param name="tgo">Object to check dictionary for.</param>
    public void AddTilesToDictionary(GameObject tgo)
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
    }

    /// <summary>
    /// Checks if UnitPos contains unit and replaces or adds it.
    /// </summary>
    /// <param name="tgo">Unit to check dictionary for.</param>
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

    }

    /// <summary>
    /// Removes a unit from UnitPos
    /// </summary>
    /// <param name="newUnit">Unit to add</param>
    /// <param name="oldPos">Old key that needs removed</param>
    public void MoveUnitPositionInDictionary(GameObject newUnit, Vector2 oldPos)
    {
        if (UnitPos.ContainsKey(oldPos))
        {
            UnitPos.Remove(oldPos);
            if (UnitPos.ContainsKey(oldPos))
            {
                throw new Exception("Key was not removed");
            }
            UnitPos.Add(newUnit.transform.position, newUnit);
        }
    }

    public void KillUnitPlayScene(GameObject Dead)
    {
        UnitPos.Remove(Dead.transform.position);
        if (UnitPos.ContainsKey(Dead.transform.position))
        {
            throw new Exception("Key was not removed");
        }
        Destroy(Dead);
    }

    /// <summary>
    /// Checks if BuildingPos contains building and replaces or adds it.
    /// </summary>
    /// <param name="tgo">Building to check dictionary for.</param>
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

    /// <summary>
    /// Saves variables when clicking on something that is not a overlay
    /// </summary>
    /// <param name="STL">Sprite renderer</param>
    /// <param name="ST">GameObject</param>
    public void MouseSelectedController(SpriteRenderer STL, GameObject ST)
    {
        if (SelectedTileOverlay != null) { SelectedTileOverlay.enabled = false; }
        SelectedTileOverlay = STL;
        SelectedTile = ST;
        SelectedTileOverlay.enabled = true;
    }// sets selected tile to whatever tile is clicked on and enables the clickon overlay

    /// <summary>
    /// Checks were mouse is hitting and then acts accordingly.(Used in Map Editor Scene Only)
    /// </summary>
    public void RayCastForMapEditor()
    {
        if (Input.GetMouseButton(0) && SceneManager.GetActiveScene().name == "MapEditorScene") //are we in map editor scene?
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                ////Debug.log("Raycast activated");
                Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray);
                ////Debug.log(hits.Length);
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];
                    if (hit.transform.tag == MEMCC.SelectedTab) //is the hit tag = to what we are trying to place down?
                    {
                        if (hit.transform.tag == DatabaseController.instance.TerrainDictionary[0].Type) //is it a terrain?
                        {
                            TerrainController TC = hit.transform.GetComponent<TerrainController>();
                            if (hit.transform.name != MEMCC.SelectedButton) //are we changing the tile to something new?
                            {
                                //Debug.log("Changing terrain to " + MEMCC.SelectedButton);
                                TC.ChangeTile(); //change tile to new terrain tile
                                AddTilesToDictionary(hit.transform.gameObject);
                                SpriteUpdateActivator();
                            }
                        }
                        else if (hit.transform.tag == DatabaseController.instance.UnitDictionary[0].Type) //is the hit a unit?
                        {
                            UnitController UC = hit.transform.GetComponent<UnitController>();
                            if (hit.transform.name != MEMCC.SelectedButton) //are we changing the unit to something new?
                            {
                                //Debug.log("Changing unit to " + MEMCC.SelectedButton);
                                if (MEMCC.SelectedButton == "Delete Unit")
                                {
                                    UnitPos.Remove(hit.transform.position);
                                }
                                if (MEMCC.SelectedTeam != 0)
                                {
                                    UC.ChangeUnit(); //change to new unit 
                                }
                                else
                                {
                                    MEMCC.CurrentSelectedButtonText.text = "Cannot change unit team to 0";
                                }
                                //if (MEMCC.SelectedButton != "Delete Unit")
                                //{
                                //    AddUnitsToDictionary(hit.transform.gameObject);
                                //}
                                SpriteUpdateActivator();
                            }
                            else if (hit.transform.GetComponent<UnitController>().Team != MEMCC.SelectedTeam)
                            {
                                hit.transform.GetComponent<UnitController>().Team = MEMCC.SelectedTeam;
                                SpriteUpdateActivator();
                            }
                        }
                        else if (hit.transform.tag == DatabaseController.instance.BuildingDictionary[0].Type) //is the hit a building?
                        {
                            BuildingController BC = hit.transform.GetComponent<BuildingController>();
                            if (hit.transform.name != MEMCC.SelectedButton) //are we changing the building to something new?
                            {
                                //Debug.log("Changing building to " + MEMCC.SelectedButton);
                                if (MEMCC.SelectedButton == "Delete Building")
                                {
                                    BuildingPos.Remove(hit.transform.position);
                                }
                                BC.ChangeBuilding(); //change to new building
                                if (MEMCC.SelectedButton != "Delete Building")
                                {
                                    AddBuildingToDictionary(hit.transform.gameObject);
                                }
                                SpriteUpdateActivator();
                            }
                            else if (hit.transform.GetComponent<BuildingController>().Team != MEMCC.SelectedTeam)
                            {
                                hit.transform.GetComponent<BuildingController>().Team = MEMCC.SelectedTeam;
                                SpriteUpdateActivator();
                            }
                        }
                    }
                    else if (hit.transform.tag == DatabaseController.instance.TerrainDictionary[0].Type && MEMCC.SelectedTab == DatabaseController.instance.UnitDictionary[0].Type) //is the current hit not equal to what we are trying to place down. we are trying to place a unit.
                    {
                        if (!UnitPos.ContainsKey(hit.transform.position)) //is there a unit there already?
                        {
                            foreach (KeyValuePair<int, Unit> kvp in DatabaseController.instance.UnitDictionary)
                            {
                                if (kvp.Value.Title == MEMCC.SelectedButton) //need to find id for what unit we are trying to place
                                {
                                    bool tempbool = false;
                                    foreach (var item in DatabaseController.instance.TerrainDictionary)
                                    {
                                        if (hit.transform.name == item.Value.Title)
                                        {
                                            tempbool = item.Value.Walkable;
                                        }
                                    }
                                    if (tempbool)
                                    {
                                        if (MEMCC.SelectedTeam != 0)
                                        {
                                            GameObject tgo = DatabaseController.instance.CreateAndSpawnUnit(hit.transform.position, kvp.Key, MEMCC.SelectedTeam); //creat new unit at position on tile we clicked on.
                                            tgo.GetComponent<UnitController>().Team = MEMCC.SelectedTeam;
                                            AddUnitsToDictionary(tgo);
                                            //Debug.log("Creating " + MEMCC.SelectedButton + " at " + hit.transform.position);
                                            SpriteUpdateActivator();
                                        }
                                        else
                                        {
                                            MEMCC.CurrentSelectedButtonText.text = "Cant place unit for team 0";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (hit.transform.tag == DatabaseController.instance.TerrainDictionary[0].Type && MEMCC.SelectedTab == DatabaseController.instance.BuildingDictionary[0].Type)
                    {
                        if (!BuildingPos.ContainsKey(hit.transform.position))
                        {
                            foreach (KeyValuePair<int, Building> kvp in DatabaseController.instance.BuildingDictionary)
                            {
                                if (kvp.Value.Title == MEMCC.SelectedButton) //need to find id for what unit we are trying to place
                                {
                                    bool tempbool = false;
                                    foreach (var item in DatabaseController.instance.TerrainDictionary)
                                    {
                                        if (hit.transform.name == item.Value.Title)
                                        {
                                            tempbool = item.Value.Walkable;
                                        }
                                    }
                                    if (tempbool)
                                    {
                                        GameObject tgo = DatabaseController.instance.CreateAndSpawnBuilding(hit.transform.position, kvp.Key, MEMCC.SelectedTeam); //creat new building at position on tile we clicked on.
                                        tgo.GetComponent<BuildingController>().Team = MEMCC.SelectedTeam;
                                        AddBuildingToDictionary(tgo);
                                        //Debug.log("Creating " + MEMCC.SelectedButton + " at " + hit.transform.position);
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

    /// <summary>
    /// Checks were mouse is hitting and then acts accordingly.(Used in Play Scene only.)
    /// </summary>
    public void RayCastForPlayScene()
    {
        if (Input.GetMouseButtonDown(0) && CurrentScene == "PlayScene") //are we in play scene?
        {
            if (!EventSystem.current.IsPointerOverGameObject()) //dont want to click through menus
            {
                Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); //GET THEM RAYS
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray);
                ////Debug.log("Starting play scene ray hits");
                for (int i = 0; i < hits.Length; i++) // GO THROUGH THEM RAYS
                {
                    RaycastHit hit = hits[i];
                    if (!PSCC.AttackButtonSelected)
                    {
                        if (hit.transform.tag == DatabaseController.instance.UnitDictionary[0].Type && SelectedUnitPlayScene == null && hit.transform.GetComponent<UnitController>().UnitMovable)
                        {//is the hit a unit? is the unit movable? is the unit not selected?
                            SelectedUnitPlayScene = hit.transform.gameObject; //set unit to selected unit
                            originalPositionOfUnit = SelectedUnitPlayScene.transform.position; //get unit position
                            MoveToPosition = hit.transform.position;
                            foreach (var kvp in SelectedUnitPlayScene.GetComponent<UnitController>().TilesWeights)
                            {
                                foreach (var t in TilePos)
                                {
                                    if (kvp.Key == (Vector2)t.Value.transform.position)
                                    {
                                        t.Value.transform.GetComponent<SpriteRenderer>().color = new Color(.5F, .5F, .5F); //sets dark tint to tiles that the unit can move too
                                    }
                                }
                            }
                            int tempint = SelectedUnitPlayScene.GetComponent<UnitController>().GetEnemyUnitsInRange();
                            PSCC.AttackButtonController(tempint);
                            PSCC.WaitButton.SetActive(true);
                            foreach (var b in BuildingPos)
                            {
                                if (hit.transform.position == b.Value.transform.position && hit.transform.GetComponent<UnitController>().Team != b.Value.GetComponent<BuildingController>().Team && hit.transform.GetComponent<UnitController>().CanConvert)
                                {
                                    PSCC.CaptureButton.SetActive(true);
                                    break;
                                }
                                else
                                {
                                    PSCC.CaptureButton.SetActive(false);
                                }
                            }
                        }
                        else if (hit.transform.tag == DatabaseController.instance.UnitDictionary[0].Type && SelectedUnitPlayScene == hit.transform.gameObject && (Vector2)hit.transform.position == originalPositionOfUnit) //is the hit a unit? is the unit selected already?
                        {
                            SelectedUnitPlayScene = null; //clear selected unit variable
                                                          ////Debug.log("Selected unit set to null");
                            foreach (var kvp in TilePos)
                            {
                                kvp.Value.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                            }
                            originalPositionOfUnit = new Vector2(-1, -1); //set unit position to -1, other methods will ignore anything negative
                            MoveToPosition = new Vector2(-1, -1);
                            PSCC.SetActionButtonsToFalse();
                        }
                        else if (hit.transform.tag == DatabaseController.instance.TerrainDictionary[0].Type || hit.transform.tag == DatabaseController.instance.BuildingDictionary[0].Type) //is the hit a terrain or building?
                        {
                            ////Debug.log("Building or terrain hit");
                            if (SelectedUnitPlayScene != null)
                            {
                                if (hit.transform.position != SelectedUnitPlayScene.transform.position) //is the building or terrain not the one the unit is standing on?
                                {
                                    if (!UnitPos.ContainsKey(hit.transform.position)) //there a unit there already?
                                    {
                                        MoveToPosition = hit.transform.position; //get position we want to move to
                                        if (SelectedUnitPlayScene.GetComponent<UnitController>().TilesWeights.ContainsKey(MoveToPosition)) //does the unit have enough move points?
                                        {
                                            //Debug.log("Moving Unit");
                                            SelectedUnitPlayScene.transform.position = hit.transform.position; //move unit
                                            int tempint = SelectedUnitPlayScene.GetComponent<UnitController>().GetEnemyUnitsInRange();
                                            if (SelectedUnitPlayScene.GetComponent<UnitController>().CanMoveAndAttack)
                                            {
                                                PSCC.AttackButtonController(tempint);
                                            }
                                            else
                                            {
                                                PSCC.AttackButton.SetActive(false);
                                            }
                                            foreach (var b in BuildingPos)
                                            {
                                                if (hit.transform.position == b.Value.transform.position && SelectedUnitPlayScene.GetComponent<UnitController>().Team != b.Value.GetComponent<BuildingController>().Team && SelectedUnitPlayScene.GetComponent<UnitController>().CanConvert)
                                                {
                                                    PSCC.CaptureButton.SetActive(true);
                                                    break;
                                                }
                                                else
                                                {
                                                    PSCC.CaptureButton.SetActive(false);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Debug.log("Not enough move points");
                                        }
                                    }
                                    else
                                    {
                                        //Debug.log("Unit in the way");
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Debug.log("Nothing hit");
                        }
                    }
                    else //attack button is selected actions
                    {
                        if (SelectedUnitPlayScene.GetComponent<UnitController>().EnemyUnitsInRange.ContainsKey(hit.transform.position) && hit.transform.tag == DatabaseController.instance.UnitDictionary[0].Type)
                        {
                            //int attack = SelectedUnitPlayScene.GetComponent<UnitController>().Attack;
                            int attack = CombatCalculator(SelectedUnitPlayScene, UnitPos[hit.transform.position]);
                            UnitPos[hit.transform.position].GetComponent<UnitController>().Health = UnitPos[hit.transform.position].GetComponent<UnitController>().Health - attack;
                            foreach (var kvp in UnitPos)
                            {
                                kvp.Value.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                            }
                            if (UnitPos[hit.transform.position].GetComponent<UnitController>().Health <= 0)
                            {
                                KillUnitPlayScene(hit.transform.gameObject);
                            }
                            else
                            {
                                UnitPos[hit.transform.position].GetComponentInChildren<Text>().text = UnitPos[hit.transform.position].GetComponent<UnitController>().Health.ToString();
                            }
                            foreach (var kvp in TilePos)
                            {
                                kvp.Value.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                            }
                            WaitActionPlayScene();
                            PSCC.AttackButtonSelected = false;
                            PSCC.SetActionButtonsToFalse();
                            PSCC.HideOrShowSaveButton(false);
                            break;
                        }
                    }
                }
            }
        }
    } //used to check were mouse is hitting and then act acordingly

    /// <summary>
    /// Checks if file already exist and if it does not it creates new file and stores map as array
    /// </summary>
    /// <param name="TP">Dictionary(Vector2,Gameobjet) that contains Terrain positions</param>
    /// <param name="UP">Dictionary(Vector2,Gameobjet) that contains Unit positions</param>
    /// <param name="BP">Dictionary(Vector2,Gameobjet) that contains Building positions</param>
    /// <param name="SaveName">String for file name.</param>
    public void SaveMap(Dictionary<Vector2, GameObject> TP, Dictionary<Vector2, GameObject> UP, Dictionary<Vector2, GameObject> BP, string SaveName)
    {
        if (!System.IO.File.Exists(Application.dataPath + "/StreamingAssets/Maps/" + SaveName + ".json"))
        {
            if (SaveName != "")
            {
                if (!Regex.IsMatch(SaveName, @"^[a-z][A-Z]+$"))
                {
                    Map MF = new Map();
                    TeamList = FillTeamList();
                    MF.UnitList = new List<SaveableUnit>();
                    MF.TerrainList = new List<SaveableTile>();
                    MF.BuildingList = new List<SaveableBuilding>();
                    foreach (var kvp in UnitPos)
                    {
                        SaveableUnit SU = new SaveableUnit();
                        SU.Health = kvp.Value.GetComponent<UnitController>().Health;
                        SU.Team = kvp.Value.GetComponent<UnitController>().Team;
                        SU.ID = kvp.Value.GetComponent<UnitController>().ID;
                        SU.Location = kvp.Key;
                        MF.UnitList.Add(SU);
                        foreach (var t in TeamList)
                        {
                            if (t.Team == kvp.Value.GetComponent<UnitController>().Team && t.Active == false && t.Team != 0)
                            {
                                t.Active = true;
                            } 
                        }
                    }
                    foreach (var kvp in TilePos)
                    {
                        SaveableTile ST = new SaveableTile();
                        ST.ID = kvp.Value.GetComponent<TerrainController>().ID;
                        ST.Location = kvp.Key;
                        MF.TerrainList.Add(ST);
                    }
                    foreach (var kvp in BuildingPos)
                    {
                        SaveableBuilding SB = new SaveableBuilding();
                        SB.Health = kvp.Value.GetComponent<BuildingController>().Health;
                        SB.Team = kvp.Value.GetComponent<BuildingController>().Team;
                        SB.ID = kvp.Value.GetComponent<BuildingController>().ID;
                        SB.Location = kvp.Key;
                        MF.BuildingList.Add(SB);
                        foreach (var t in TeamList)
                        {
                            if (t.Team == kvp.Value.GetComponent<BuildingController>().Team && t.Active == false&& t.Team !=0)
                            {
                                t.Active = true;
                            }
                        }
                    }
                    int tempteamcount = new int();
                    foreach(var t in TeamList)
                    {
                        if (t.Active)
                        {
                            tempteamcount = tempteamcount + 1;
                        }
                    }
                    if (tempteamcount >= 2)
                    {
                        MF.Teamlist = TeamList;
                        string tempjson = JsonUtility.ToJson(MF,true);
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

    /// <summary>
    /// Opens file and deseralizes it and then sends the info to drawLoadMapFromEditor function. File must be in: Assets/StreamingAssets/Core/Maps/
    /// </summary>
    /// <param name="name">Name of map to load</param>
    public void LoadMapMapEditor(string name)
    {
        var TilesToDelete = GameObject.FindGameObjectsWithTag(DatabaseController.instance.TerrainDictionary[0].Type);
        var UnitsToDelete = GameObject.FindGameObjectsWithTag(DatabaseController.instance.UnitDictionary[0].Type);
        var BuildingsToDelete = GameObject.FindGameObjectsWithTag(DatabaseController.instance.BuildingDictionary[0].Type);
        BuildingPos = new Dictionary<Vector2, GameObject>();
        TilePos = new Dictionary<Vector2, GameObject>();
        UnitPos = new Dictionary<Vector2, GameObject>();
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

        StreamReader SR = new StreamReader(Application.dataPath + "/StreamingAssets/Maps/" + name + ".json");
        string tempstring = SR.ReadToEnd();
        Map LoadingFile = JsonUtility.FromJson<Map>(tempstring);
        TeamList.Clear();
        TilePos.Clear();
        UnitPos.Clear();
        BuildingPos.Clear();
        TeamList = LoadingFile.Teamlist;
        foreach (var item in LoadingFile.TerrainList)
        {
            GameObject TGO = DatabaseController.instance.CreateAdnSpawnTerrain(item.Location, item.ID);
            TilePos.Add(item.Location, TGO);
            if (item.Location.x > EditorMapSize)
            {
                EditorMapSize = (int)item.Location.x;
            }
        }
        foreach (var item in LoadingFile.UnitList)
        {
            GameObject TGO = DatabaseController.instance.CreateAndSpawnUnit(item.Location, item.ID, item.Team);
            UnitPos.Add(item.Location, TGO);
        }
        foreach (var item in LoadingFile.BuildingList)
        {
            GameObject TGO = DatabaseController.instance.CreateAndSpawnBuilding(item.Location, item.ID, item.Team);
            BuildingPos.Add(item.Location, TGO);
        }
        CameraVar = GameObject.Find("MainCamera");
        CameraVar.transform.position = new Vector3(EditorMapSize / 2 - .5f, EditorMapSize / 2 - .5f, EditorMapSize * -1);
        SpriteUpdateActivator();
    }

    /// <summary>
    /// Loads the map form file and spawns everything in and then adds them to dictionaries, and finally resets camera position.
    /// </summary>
    /// <param name="name">Name of map to load.</param>
    public void LoadMapPlayScene(string name)
    {
        StreamReader SR = new StreamReader(Application.dataPath + "/StreamingAssets/Maps/" + name + ".json");
        string tempstring = SR.ReadToEnd();
        Map LoadingFile = UnityEngine.JsonUtility.FromJson<Map>(tempstring);
        //TeamList.Clear();
        TilePos.Clear();
        UnitPos.Clear();
        BuildingPos.Clear();
        //TeamList = LoadingFile.Teamlist;
        foreach (var item in LoadingFile.TerrainList)
        {
            GameObject TGO = DatabaseController.instance.CreateAdnSpawnTerrain(item.Location, item.ID);
            TilePos.Add(item.Location, TGO);
            if (item.Location.x > PlayMapSize)
            {
                PlayMapSize = (int)item.Location.x;
            }
        }
        foreach (var item in LoadingFile.UnitList)
        {
            GameObject TGO = DatabaseController.instance.CreateAndSpawnUnit(item.Location, item.ID, item.Team);
            UnitPos.Add(item.Location, TGO);
        }
        foreach (var item in LoadingFile.BuildingList)
        {
            GameObject TGO = DatabaseController.instance.CreateAndSpawnBuilding(item.Location, item.ID, item.Team);
            BuildingPos.Add(item.Location, TGO);
        }
        CameraVar = GameObject.Find("MainCamera");
        CameraVar.transform.position = new Vector3(PlayMapSize / 2 - .5f, PlayMapSize / 2 - .5f, PlayMapSize * -1);
        SpriteUpdateActivator();
    }

    /// <summary>
    /// Used to call for a sprite update when it is needed
    /// </summary>
    public void SpriteUpdateActivator()
    {
        foreach (var kvp in TilePos)
        {
            kvp.Value.GetComponent<TerrainController>().WaterSpriteController();
            kvp.Value.GetComponent<TerrainController>().RoadSpriteController();
        }
        foreach (var kvp in UnitPos)
        {
            if (kvp.Value != null)
            {
                kvp.Value.GetComponent<UnitController>().TeamSpriteController();
            }
        }
        foreach (var kvp in BuildingPos)
        {
            if (kvp.Value != null)
            {
                kvp.Value.GetComponent<BuildingController>().TeamSpriteUpdater();
            }
        }
    }

    /// <summary>
    /// Picks random team to go first and then adjust gameobject variables to match
    /// </summary>
    public void PlaySceneNewGameInitalizer()
    {
        List<TeamStuff> tempteamlist = new List<TeamStuff>(); //temp list of teamstuff
        foreach(var t in TeamList)
        {
            tempteamlist.Add(t);
        }
        tempteamlist.RemoveAll(TeamIsActive);
        System.Random rnnd = new System.Random();
        int tempRandom = rnnd.Next(0, tempteamlist.Count); //get random number based on how many are on list
        CurrentTeamsTurn = TeamList[TeamList.IndexOf(tempteamlist[tempRandom])]; //set team turn number to team of random picked number
        PSCC.CurrentPlayerTurnText.text = "Team turn";
        PSCC.GoldText.text = "Gold:" + TeamList[CurrentTeamsTurn.Team].Gold.ToString();
        AllRoundUpdater();
        foreach (var kvp in BuildingPos)
        {
            if (kvp.Value.GetComponent<BuildingController>().Team == CurrentTeamsTurn.Team)
            {
                kvp.Value.GetComponent<BuildingController>().CanBuild = true;
            }
            else
            {
                kvp.Value.GetComponent<BuildingController>().CanBuild = false;
            }
        }
        int TempInt = 0;
        foreach (var kvp in BuildingPos)
        {
            if (kvp.Value.GetComponent<BuildingController>().Team == CurrentTeamsTurn.Team)
            {
                TempInt = TempInt + 1;
            }
        }
        TempInt = TempInt * 100;
        TeamList[CurrentTeamsTurn.Team].Gold = TeamList[CurrentTeamsTurn.Team].Gold + TempInt;
        PSCC.UpdateGoldThings();
        PSCC.UpdateTurnImageColor(CurrentTeamsTurn.Team);
        AiController();
        PSCC.HideOrShowSaveButton(true);
    } //sets up game for new game

    public void PlaySceneLoadGameInitalizer()
    {
        PSCC.CurrentPlayerTurnText.text = "Team turn";
        PSCC.GoldText.text = "Gold:" + TeamList[CurrentTeamsTurn.Team].Gold.ToString();
        AllRoundUpdater();
        foreach (var kvp in BuildingPos)
        {
            if (kvp.Value.GetComponent<BuildingController>().Team == CurrentTeamsTurn.Team)
            {
                kvp.Value.GetComponent<BuildingController>().CanBuild = true;
            }
            else
            {
                kvp.Value.GetComponent<BuildingController>().CanBuild = false;
            }
        }
        PSCC.UpdateGoldThings();
        PSCC.UpdateTurnImageColor(CurrentTeamsTurn.Team);
        AiController();
        PSCC.HideOrShowSaveButton(true);
    }

    /// <summary>
    /// Used to controll variables for when the turn changes
    /// </summary>
    public void PlaySceneTurnChanger()
    {
        foreach(var t in TeamList)
        {
            t.UnitCount = 0;
            t.BuildingCount = 0;
        }
        foreach (var kvp in UnitPos) //check through units to make sure there is still enough players to play the game
        {
            TeamList[kvp.Value.GetComponent<UnitController>().Team].UnitCount++; 
        }
        foreach (var kvp in BuildingPos)
        {
            TeamList[kvp.Value.GetComponent<BuildingController>().Team].BuildingCount++;
        }
        foreach (var t in TeamList)
        {
            if (t.UnitCount + t.BuildingCount == 0)
            {
                t.Defeated = true;
            }
        }
        List<TeamStuff> TempTeamList = new List<TeamStuff>();
        foreach(var t in TeamList)
        {
            TempTeamList.Add(t);
        }
        TempTeamList.RemoveAll(TeamIsActive);
        if (TempTeamList.Count == 1)
        {
            PSCC.GameEndController(TempTeamList[0].Team);
        }
        else
        {
            if (CurrentTeamsTurn.Team < TempTeamList[TempTeamList.Count - 1].Team) //if index is not at max
            {
                CurrentTeamsTurn = TeamList[TeamList.IndexOf(CurrentTeamsTurn)+1];
            }
            else
            {
                CurrentTeamsTurn = TeamList[TeamList.IndexOf(TempTeamList[0])];
            }
            PSCC.CurrentPlayerTurnText.text = "Team turn";
            AllRoundUpdater();
            foreach (var kvp in BuildingPos)
            {
                if (kvp.Value.GetComponent<BuildingController>().Team == CurrentTeamsTurn.Team)
                {
                    kvp.Value.GetComponent<BuildingController>().CanBuild = true;
                    kvp.Value.GetComponent<BuildingController>().HealIfFriendlyUnitOnBuilding();
                }
                else
                {
                    kvp.Value.GetComponent<BuildingController>().CanBuild = false;
                }
            }
            int TempInt = 0;
            foreach (var kvp in BuildingPos)
            {
                if (kvp.Value.GetComponent<BuildingController>().Team == CurrentTeamsTurn.Team)
                {
                    TempInt = TempInt + 1;
                }
            }
            TempInt = TempInt * 100;
            TeamList[CurrentTeamsTurn.Team].Gold = TeamList[CurrentTeamsTurn.Team].Gold + TempInt;
            PSCC.UpdateGoldThings();
            AiController();
            PSCC.HideOrShowSaveButton(true);
        }
    }

    /// <summary>
    /// Controls the loading slider in Initalization Scene
    /// </summary>
    /// <param name="f">Were to move the slider, using a float between 0-1</param>
    public void LoadingUpdater(float f)
    {
        Slider LoadingSlider = GameObject.Find("Canvas").GetComponentInChildren<Slider>();
        LoadingSlider.value = f;
        if (f == 1f)
        {
            SceneManager.LoadScene("MainMenuScene");
            DatabaseController.instance.Initalisation = false;
        }
    }

    /// <summary>
    /// Returns unit to original position and resets all variables set to it
    /// </summary>
    public void CancelActionPlayScene()
    {
        if (originalPositionOfUnit != new Vector2(-1, -1))
        {
            SelectedUnitPlayScene.transform.position = originalPositionOfUnit;
            originalPositionOfUnit = new Vector2(-1, -1);
        }
        foreach (var kvp in TilePos)
        {
            kvp.Value.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }
        SelectedUnitPlayScene.GetComponent<UnitController>().UnitMovable = true;
        foreach (var kvp in UnitPos)
        {
            kvp.Value.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }
        SelectedUnitPlayScene = null; //clear selected unit variable
        PSCC.SetActionButtonsToFalse();
    }

    /// <summary>
    /// Sets picked position as units new position and changes unit Movable variable to false
    /// </summary>
    public void WaitActionPlayScene()
    {
        if (MoveToPosition != new Vector2(-1, -1))
        {
            MoveUnitPositionInDictionary(SelectedUnitPlayScene, originalPositionOfUnit);
            MoveToPosition = new Vector2(-1, -1);
        }
        SelectedUnitPlayScene.GetComponent<UnitController>().UnitMovable = false; //set unit movable to false
        SelectedUnitPlayScene.GetComponent<UnitController>().UnitMoved = true; // why do i have both moveable and moved??????? fix later
        SelectedUnitPlayScene = null;
        foreach (var kvp in TilePos)
        {
            kvp.Value.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            kvp.Value.GetComponent<TerrainController>().TerrainRoundUpdater();
        }
        foreach (var kvp in UnitPos)
        {
            kvp.Value.GetComponent<UnitController>().GetTileValues();
        }
        foreach (var kvp in BuildingPos)
        {
            kvp.Value.GetComponent<BuildingController>().BuildingRoundUpdater();
        }
        PSCC.SetActionButtonsToFalse();
        PSCC.HideOrShowSaveButton(false);
    }

    public void CaptureActionPlayScene()
    {
        if (MoveToPosition != new Vector2(-1, -1))
        {
            MoveUnitPositionInDictionary(SelectedUnitPlayScene, originalPositionOfUnit);
            MoveToPosition = new Vector2(-1, -1);
        }
        SelectedUnitPlayScene.GetComponent<UnitController>().UnitMovable = false; //set unit movable to false
        SelectedUnitPlayScene.GetComponent<UnitController>().UnitMoved = true;
        PSCC.SetActionButtonsToFalse();
        foreach (var b in BuildingPos)
        {
            if (b.Value.transform.position == SelectedUnitPlayScene.transform.position && b.Value.transform.GetComponent<BuildingController>().Team != SelectedUnitPlayScene.GetComponent<UnitController>().Team)
            {
                b.Value.transform.GetComponent<BuildingController>().Health = b.Value.transform.GetComponent<BuildingController>().Health - SelectedUnitPlayScene.GetComponent<UnitController>().ConversionSpeed;
                if (b.Value.transform.GetComponent<BuildingController>().Health <= 0)
                {
                    b.Value.transform.GetComponent<BuildingController>().Team = SelectedUnitPlayScene.GetComponent<UnitController>().Team;
                    b.Value.transform.GetComponent<BuildingController>().Health = 10; // set this to max health variable from building json
                    b.Value.transform.GetComponent<BuildingController>().TeamSpriteUpdater();
                    b.Value.transform.GetComponentInChildren<Text>().text = b.Value.transform.GetComponent<BuildingController>().Health.ToString();
                }
                else
                {
                    b.Value.transform.GetComponentInChildren<Text>().text = b.Value.transform.GetComponent<BuildingController>().Health.ToString();
                }
            }
        }
        foreach (var kvp in TilePos)
        {
            kvp.Value.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            kvp.Value.GetComponent<TerrainController>().TerrainRoundUpdater();
        }
        foreach (var kvp in UnitPos)
        {
            kvp.Value.GetComponent<UnitController>().GetTileValues();
        }
        foreach (var kvp in BuildingPos)
        {
            kvp.Value.GetComponent<BuildingController>().BuildingRoundUpdater();
        }
        SelectedUnitPlayScene = null;
        PSCC.HideOrShowSaveButton(false);
    }

    /// <summary>
    /// Adds a tint to units that this unit can attack
    /// </summary>
    public void AttackActionPlayScene()
    {
        foreach (var kvp in SelectedUnitPlayScene.GetComponent<UnitController>().EnemyUnitsInRange)
        {
            foreach (var u in UnitPos)
            {
                if (kvp.Key == u.Key)
                {
                    u.Value.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
                }
            }
        }
        PSCC.WaitButton.SetActive(false);
        //PSCC.CancelButton.SetActive(false);
    }

    /// <summary>
    /// Simple audio controller to test out audio functionallity
    /// </summary>
    /// <param name="SceneName"></param>
    public void AudioController(string SceneName)
    {
        if (SceneName == "MainMenuScene")
        {
            gameObject.GetComponent<AudioSource>().Stop();
            gameObject.GetComponent<AudioSource>().PlayOneShot(MainAudio);
        }
        else if (SceneName == "MapEditorScene")
        {
            gameObject.GetComponent<AudioSource>().Stop();
            gameObject.GetComponent<AudioSource>().PlayOneShot(MapEditorAudio);
        }
        else if (SceneName == "PlayScene")
        {
            gameObject.GetComponent<AudioSource>().Stop();
            gameObject.GetComponent<AudioSource>().PlayOneShot(BattleAudio);
        }
    }

    /// <summary>
    /// Runs Round updater function for every unit,building and terrain
    /// </summary>
    public void AllRoundUpdater()
    {
        foreach (var kvp in TilePos)
        {
            kvp.Value.GetComponent<TerrainController>().TerrainRoundUpdater();
        }
        foreach (var kvp in UnitPos)
        {
            kvp.Value.GetComponent<UnitController>().UnitRoundUpdater();
        }
        foreach (var kvp in BuildingPos)
        {
            kvp.Value.GetComponent<BuildingController>().BuildingRoundUpdater();
        }
    }

    public int CombatCalculator(GameObject Attacker, GameObject Defender)
    {
        Debug.Log("Health:" + Attacker.GetComponent<UnitController>().Health);
        Debug.Log("MaxHealth:" + Attacker.GetComponent<UnitController>().MaxHealth);
        double HealthModifier = (double)Attacker.GetComponent<UnitController>().Health / Attacker.GetComponent<UnitController>().MaxHealth;
        Debug.Log("HealthMod:" + HealthModifier);
        double AttackDouble = Math.Ceiling(HealthModifier * Attacker.GetComponent<UnitController>().Attack);
        Debug.Log("Rounded:" + AttackDouble);
        int Attack = (int)AttackDouble;
        Debug.Log("Attack:" + Attack);
        int AttackAfterDefence;
        if (BuildingPos.ContainsKey(Defender.transform.position))
        {
            double DefencePercent = (Defender.GetComponent<UnitController>().Defence + TilePos[Defender.transform.position].GetComponent<TerrainController>().DefenceBonus + BuildingPos[Defender.transform.position].GetComponent<BuildingController>().DefenceBonus) / 100d;
            Debug.Log("BuildingDefencePercent:" + DefencePercent);
            double DefenceReduction = DefencePercent * Attack;
            Debug.Log("DefenceReduction:" + DefenceReduction);
            AttackAfterDefence = (int)Math.Round(Attack - DefenceReduction);
            Debug.Log("AttackAfterDefence:" + AttackAfterDefence);
        }
        else
        {
            double DefencePercent = (Defender.GetComponent<UnitController>().Defence + TilePos[Defender.transform.position].GetComponent<TerrainController>().DefenceBonus) / 100d;
            Debug.Log("DefencePercent:" + DefencePercent);
            double DefenceReduction = DefencePercent * Attack;
            Debug.Log("DefenceReduction:" + DefenceReduction);
            AttackAfterDefence = (int)Math.Round(Attack - DefenceReduction);
            Debug.Log("AttackAfterDefence:" + AttackAfterDefence);
        }
        return AttackAfterDefence;
    }

    public void AiController()
    {
        if (TeamList[CurrentTeamsTurn.Team].IsAi) // if team is ai do something
        {
            Dictionary<Vector2, GameObject> TempDict = new Dictionary<Vector2, GameObject>();
            foreach (var kvp in UnitPos)
            {
                TempDict.Add(kvp.Key, kvp.Value);
            }
            foreach (var kvp in TempDict)
            {
                if (kvp.Value.GetComponent<UnitController>().Team == CurrentTeamsTurn.Team)
                {
                    kvp.Value.GetComponent<UnitController>().UnitAi();
                }
            }
            foreach (var kvp in BuildingPos)
            {
                if (kvp.Value.GetComponent<BuildingController>().Team == CurrentTeamsTurn.Team)
                {
                    kvp.Value.GetComponent<BuildingController>().BuildingAiController();
                }
            }
            PSCC.EndTurnButtonClicked();
        }
    }

    public void SaveGame(string SaveName)
    {

        Save SF = new Save();
        SF.TeamList = TeamList;
        SF.CurrentTeamsTurn = CurrentTeamsTurn.Team;
        SF.UnitList = new List<SaveableUnit>();
        SF.TerrainList = new List<SaveableTile>();
        SF.BuildingList = new List<SaveableBuilding>();
        foreach (var kvp in UnitPos)
        {
            SaveableUnit SU = new SaveableUnit();
            SU.Health = kvp.Value.GetComponent<UnitController>().Health;
            SU.Team = kvp.Value.GetComponent<UnitController>().Team;
            SU.ID = kvp.Value.GetComponent<UnitController>().ID;
            SU.Location = kvp.Key;
            SF.UnitList.Add(SU);
        }
        foreach (var kvp in TilePos)
        {
            SaveableTile ST = new SaveableTile();
            ST.ID = kvp.Value.GetComponent<TerrainController>().ID;
            ST.Location = kvp.Key;
            SF.TerrainList.Add(ST);
        }
        foreach (var kvp in BuildingPos)
        {
            SaveableBuilding SB = new SaveableBuilding();
            SB.Health = kvp.Value.GetComponent<BuildingController>().Health;
            SB.Team = kvp.Value.GetComponent<BuildingController>().Team;
            SB.ID = kvp.Value.GetComponent<BuildingController>().ID;
            SB.Location = kvp.Key;
            SF.BuildingList.Add(SB);
        }
        string tempjson = UnityEngine.JsonUtility.ToJson(SF,true);
        FileStream fs = File.Create(Application.dataPath + "/StreamingAssets/Saves/" + SaveName + ".json");
        StreamWriter sr = new StreamWriter(fs);
        sr.Write(tempjson);
        sr.Close();
        sr.Dispose();
        fs.Close();
        fs.Dispose();
    }

    public void LoadSavedGamePlayScene(string name)
    {
        StreamReader SR = new StreamReader(Application.dataPath + "/StreamingAssets/Saves/" + name + ".json");
        string tempstring = SR.ReadToEnd();
        Save LoadingFile = UnityEngine.JsonUtility.FromJson<Save>(tempstring);
        TeamList.Clear();
        TilePos.Clear();
        UnitPos.Clear();
        BuildingPos.Clear();
        TeamList = LoadingFile.TeamList;
        foreach(var item in LoadingFile.TerrainList)
        {
            GameObject TGO = DatabaseController.instance.CreateAdnSpawnTerrain(item.Location, item.ID);
            TilePos.Add(item.Location,TGO);
            if (item.Location.x > PlayMapSize)
            {
                PlayMapSize = (int)item.Location.x;
            }
        }
        foreach(var item in LoadingFile.UnitList)
        {
            GameObject TGO = DatabaseController.instance.CreateAndSpawnUnit(item.Location, item.ID,item.Team);
            TGO.GetComponent<UnitController>().Health = item.Health;
            UnitPos.Add(item.Location, TGO);
        }
        foreach(var item in LoadingFile.BuildingList)
        {
            GameObject TGO = DatabaseController.instance.CreateAndSpawnBuilding(item.Location, item.ID, item.Team);
            TGO.GetComponent<BuildingController>().Health = item.Health;
            BuildingPos.Add(item.Location, TGO);
        }
        CurrentTeamsTurn = TeamList[LoadingFile.CurrentTeamsTurn];
        CameraVar = GameObject.Find("MainCamera");
        CameraVar.transform.position = new Vector3(PlayMapSize / 2 - .5f, PlayMapSize / 2 - .5f, PlayMapSize * -1);
        SpriteUpdateActivator();
    }

    public List<TeamStuff> FillTeamList()
    {
        List<TeamStuff> L = new List<TeamStuff>();
        TeamStuff T0 = new TeamStuff();
        L.Add(T0);
        TeamStuff T1 = new TeamStuff();
        T1.Team = 1;
        L.Add(T1);//1
        TeamStuff T2 = new TeamStuff();
        T2.Team = 2;
        L.Add(T2);//2
        TeamStuff T3 = new TeamStuff();
        T3.Team = 3;
        L.Add(T3);//3
        TeamStuff T4 = new TeamStuff();
        T4.Team = 4;
        L.Add(T4);//4
        TeamStuff T5 = new TeamStuff();
        T5.Team = 5;
        T5.IsAi = false;
        L.Add(T5);//5
        TeamStuff T6 = new TeamStuff();
        T6.Team = 6;
        L.Add(T6);//6
        TeamStuff T7 = new TeamStuff();
        T7.Team = 7;
        L.Add(T7);//7
        TeamStuff T8 = new TeamStuff();
        T8.Team = 8;
        L.Add(T8);//8
        TeamStuff T9 = new TeamStuff();
        T9.Team = 9;
        L.Add(T9);//9
        return L;
    }

    public bool TeamIsActive(TeamStuff t)
    {
        if (t.Active && !t.Defeated)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool FindLastTeam(TeamStuff t)
    {
        if (t.Team == 9)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

/// <summary>
/// Array class for saving maps. Variables(Variable type-name): SeralizableVector2-Location,string-Name,int-Team,string-Type,int-TeamCount
/// </summary>
[Serializable]
public class Map
{
    public List<TeamStuff> Teamlist;
    public List<SaveableUnit> UnitList;
    public List<SaveableBuilding> BuildingList;
    public List<SaveableTile> TerrainList;
    public List<string> Mods;
}

[Serializable]
public class Save
{
    public List<TeamStuff> TeamList;
    public int CurrentTeamsTurn;
    public List<SaveableUnit> UnitList;
    public List<SaveableBuilding> BuildingList;
    public List<SaveableTile> TerrainList;
}

[Serializable]
public class SeralizableVector2
{
    public float x;
    public float y;

    public SeralizableVector2(float rx, float ry)
    {
        x = rx;
        y = ry;
    }

    public static implicit operator Vector2(SeralizableVector2 rValue)
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

[Serializable]
public class SaveableUnit
{
    public int ID;
    public int Team;
    public int Health;
    public SeralizableVector2 Location;
}

[Serializable]
public class SaveableBuilding
{
    public int Team;
    public int Health;
    public int ID;
    public SeralizableVector2 Location;
}

[Serializable]
public class SaveableTile
{
    public int ID;
    public SeralizableVector2 Location;
}

[Serializable]
public class TeamStuff
{
    public int Team;
    public bool Active = false;
    public int UnitCount = 0;
    public int BuildingCount = 0;
    public int Gold = 0;
    public bool IsAi = false;
    public bool Defeated = false;
}