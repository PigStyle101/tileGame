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
    public List<int> TeamCount;
    [HideInInspector]
    public int CurrentTeamsTurn;
    private Vector2 originalPositionOfUnit;
    private Vector2 MoveToPosition;
    public AudioClip BattleAudio;
    public AudioClip MainAudio;
    public AudioClip MapEditorAudio;
    [HideInInspector]
    public string LogFile = "log.txt";
    public bool EchoToConsole = true;
    public bool AddTimeStamp = true;
    public Dictionary<int, int> TeamGold; //Team,Gold

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
        SceneManager.LoadScene("MapEditorScene");//notes and stuff
    }

    /// <summary>
    /// Checks what scene was loaded then executes required actions
    /// </summary>
    /// <param name="sceneVar">Name of scene that was loaded</param>
    /// <param name="Mode">Not even sure this is needed</param>
    private void OnSceneLoaded(Scene sceneVar, LoadSceneMode Mode) //do i need LoadSceneMode here?
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
                    //creates a map in data to use for drawing later
                    MapDictionary.Add(new Vector2(i, o), DatabaseController.instance.TerrainDictionary[0].Title);
                    //UnityEngine.Debug.Log("Added Key: " + i + o);
                }
            }
            DrawNewMapForMapEditor();
            MapDictionary = new Dictionary<Vector2, string>(); //clear dictionary for good measure
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
                        if (hit.transform.tag == DatabaseController.instance.TerrainDictionary[0].Type) //is it a terrain?
                        {
                            TerrainController TC = hit.transform.GetComponent<TerrainController>();
                            if (hit.transform.name != MEMCC.SelectedButton) //are we changing the tile to something new?
                            {
                                Debug.Log("Changing terrain to " + MEMCC.SelectedButton);
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
                                Debug.Log("Changing unit to " + MEMCC.SelectedButton);
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
                                Debug.Log("Changing building to " + MEMCC.SelectedButton);
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
                                            Debug.Log("Creating " + MEMCC.SelectedButton + " at " + hit.transform.position);
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
                //Debug.Log("Starting play scene ray hits");
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
                                                          //Debug.Log("Selected unit set to null");
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
                            //Debug.Log("Building or terrain hit");
                            if (SelectedUnitPlayScene != null)
                            {
                                if (hit.transform.position != SelectedUnitPlayScene.transform.position) //is the building or terrain not the one the unit is standing on?
                                {
                                    if (!UnitPos.ContainsKey(hit.transform.position)) //there a unit there already?
                                    {
                                        MoveToPosition = hit.transform.position; //get position we want to move to
                                        if (SelectedUnitPlayScene.GetComponent<UnitController>().TilesWeights.ContainsKey(MoveToPosition)) //does the unit have enough move points?
                                        {
                                            Debug.Log("Moving Unit");
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
                                UnitPos.Remove(hit.transform.position);
                                Destroy(hit.transform.gameObject);
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
                    int count = 0;
                    TeamCount = new List<int>();
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
                            save[count].Type = DatabaseController.instance.TerrainDictionary[0].Type;
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
                            save[count].Type = DatabaseController.instance.UnitDictionary[0].Type;
                            save[count].Team = kvp.Value.GetComponent<UnitController>().Team;
                            count = count + 1;
                            if (kvp.Value.GetComponent<UnitController>().Team == 1)
                            {
                                team1 = true;
                            }
                            if (kvp.Value.GetComponent<UnitController>().Team == 2)
                            {
                                team2 = true;
                            }
                            if (kvp.Value.GetComponent<UnitController>().Team == 3)
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
                                save[count].Type = DatabaseController.instance.BuildingDictionary[0].Type;
                                save[count].Team = kvp.Value.GetComponent<BuildingController>().Team;
                                count = count + 1;
                            }
                        }
                    }
                    if (team1)
                    {
                        TeamCount.Add(1);
                    }
                    if (team2)
                    {
                        TeamCount.Add(2);
                    }
                    if (team3)
                    {
                        TeamCount.Add(3);
                    }
                    if (TeamCount.Count >= 2)
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

    /// <summary>
    /// Opens file and deseralizes it and then sends the info to drawLoadMapFromEditor function. File must be in: Assets/StreamingAssets/Core/Maps/
    /// </summary>
    /// <param name="name">Name of map to load</param>
    public void LoadMapMapEditor(string name)
    {
        StreamReader SR = new StreamReader(Application.dataPath + "/StreamingAssets/Maps/" + name + ".json");
        string tempstring = SR.ReadToEnd();
        Map[] Load = JsonHelper.FromJson<Map>(tempstring);


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

        for (int i = 0; i < Load.Length; i++)
        {
            if (Load[i].Type == DatabaseController.instance.TerrainDictionary[0].Type)
            {
                foreach (var kvp in DatabaseController.instance.TerrainDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        GameObject GO = DatabaseController.instance.CreateAdnSpawnTerrain(Load[i].Location, kvp.Value.ID);
                        AddTilesToDictionary(GO);
                    }
                }
            }
            else if (Load[i].Type == DatabaseController.instance.UnitDictionary[0].Type)
            {
                foreach (var kvp in DatabaseController.instance.UnitDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        GameObject GO = DatabaseController.instance.CreateAndSpawnUnit(Load[i].Location, kvp.Value.ID, Load[i].Team);
                        AddUnitsToDictionary(GO);
                    }
                }
            }
            else if (Load[i].Type == DatabaseController.instance.BuildingDictionary[0].Type)
            {
                foreach (var kvp in DatabaseController.instance.BuildingDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        GameObject GO = DatabaseController.instance.CreateAndSpawnBuilding(Load[i].Location, kvp.Value.ID, Load[i].Team);
                        AddBuildingToDictionary(GO);
                    }
                }
            }
        }
        //foreach (var go in GameObject.FindGameObjectsWithTag(DatabaseController.instance.TerrainDictionary[0].Type))
        //{
        //    AddTilesToDictionary(go);
        //}
        //foreach (var go in GameObject.FindGameObjectsWithTag(DatabaseController.instance.UnitDictionary[0].Type))
        //{
        //    AddUnitsToDictionary(go);
        //}
        //foreach (var go in GameObject.FindGameObjectsWithTag(DatabaseController.instance.BuildingDictionary[0].Type))
        //{
        //    AddBuildingToDictionary(go);
        //}
        SR.Close();
        SR.Dispose();
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
        Map[] Load = JsonHelper.FromJson<Map>(tempstring);

        for (int i = 0; i < Load.Length; i++)
        {
            if (Load[i].Type == DatabaseController.instance.TerrainDictionary[0].Type)
            {
                foreach (var kvp in DatabaseController.instance.TerrainDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        DatabaseController.instance.CreateAdnSpawnTerrain(Load[i].Location, kvp.Value.ID);
                        if (Load[i].Location.x > PlayMapSize)
                        {
                            PlayMapSize = (int)Load[i].Location.x;
                        }
                    }
                }
            }
            else if (Load[i].Type == DatabaseController.instance.UnitDictionary[0].Type)
            {
                foreach (var kvp in DatabaseController.instance.UnitDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        DatabaseController.instance.CreateAndSpawnUnit(Load[i].Location, kvp.Value.ID, Load[i].Team);
                    }
                }
            }
            else if (Load[i].Type == DatabaseController.instance.BuildingDictionary[0].Type)
            {
                foreach (var kvp in DatabaseController.instance.BuildingDictionary)
                {
                    if (kvp.Value.Title == Load[i].Name)
                    {
                        DatabaseController.instance.CreateAndSpawnBuilding(Load[i].Location, kvp.Value.ID, Load[i].Team);
                    }
                }
            }
        }
        TeamCount = Load[0].TeamCount;
        Debug.Log("TeamCount = " + TeamCount);
        foreach (var go in GameObject.FindGameObjectsWithTag(DatabaseController.instance.TerrainDictionary[0].Type))
        {
            AddTilesToDictionary(go);
        }
        foreach (var go in GameObject.FindGameObjectsWithTag(DatabaseController.instance.UnitDictionary[0].Type))
        {
            AddUnitsToDictionary(go);
        }
        foreach (var go in GameObject.FindGameObjectsWithTag(DatabaseController.instance.BuildingDictionary[0].Type))
        {
            AddBuildingToDictionary(go);
        }
        SR.Close();
        SR.Dispose();
        SpriteUpdateActivator();
        CameraVar = GameObject.Find("MainCamera");
        CameraVar.transform.position = new Vector3(PlayMapSize / 2 - .5f, PlayMapSize / 2 - .5f, PlayMapSize * -1);

        TeamGold = new Dictionary<int, int>();
        foreach (var t in TeamCount)
        {
            TeamGold.Add(t, 0);
        }
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
        System.Random rnd = new System.Random();
        CurrentTeamsTurn = rnd.Next(1, TeamCount.Count + 1);
        PSCC.CurrentPlayerTurnText.text = CurrentTeamsTurn.ToString();
        PSCC.GoldText.text = "Gold:" + TeamGold[CurrentTeamsTurn].ToString();
        AllRoundUpdater();
        foreach (var kvp in BuildingPos)
        {
            if (kvp.Value.GetComponent<BuildingController>().Team == CurrentTeamsTurn)
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
            if (kvp.Value.GetComponent<BuildingController>().Team == CurrentTeamsTurn)
            {
                TempInt = TempInt + 1;
            }
        }
        TempInt = TempInt * 100;
        TeamGold[CurrentTeamsTurn] = TeamGold[CurrentTeamsTurn] + TempInt;
        PSCC.UpdateGoldThings();
    } //sets up game for new game

    /// <summary>
    /// Used to controll variables for when the turn changes
    /// </summary>
    public void PlaySceneTurnChanger()
    {
        Dictionary<int, int> TempTeamsUnits = new Dictionary<int, int>();
        foreach (var item in TeamCount)
        {
            TempTeamsUnits.Add(item, 0);
        }
        foreach (var kvp in UnitPos) //check through units to make sure there is still enough players to play the game
        {
            if (TempTeamsUnits.ContainsKey(kvp.Value.GetComponent<UnitController>().Team))
            {
                TempTeamsUnits[kvp.Value.GetComponent<UnitController>().Team] = TempTeamsUnits[kvp.Value.GetComponent<UnitController>().Team] + 1;
            }
        }
        foreach (var kvp in BuildingPos)
        {
            if (TempTeamsUnits.ContainsKey(kvp.Value.GetComponent<BuildingController>().Team))
            {
                TempTeamsUnits[kvp.Value.GetComponent<BuildingController>().Team] = TempTeamsUnits[kvp.Value.GetComponent<BuildingController>().Team] + 1;
            }
        }
        foreach (var kvp in TempTeamsUnits)
        {
            if (kvp.Value == 0)
            {
                TeamCount.Remove(kvp.Key);
            }
        }
        if (TeamCount.Count == 1)
        {
            PSCC.GameEndController(TeamCount[0]);
        }
        if (CurrentTeamsTurn != TeamCount[TeamCount.Count - 1])
        {
            CurrentTeamsTurn = TeamCount[TeamCount.IndexOf(CurrentTeamsTurn) + 1];
        }
        else
        {
            CurrentTeamsTurn = TeamCount[0];
        }
        PSCC.CurrentPlayerTurnText.text = CurrentTeamsTurn.ToString();
        AllRoundUpdater();
        foreach (var kvp in BuildingPos)
        {
            if (kvp.Value.GetComponent<BuildingController>().Team == CurrentTeamsTurn)
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
            if (kvp.Value.GetComponent<BuildingController>().Team == CurrentTeamsTurn)
            {
                TempInt = TempInt + 1;
            }
        }
        TempInt = TempInt * 100;
        TeamGold[CurrentTeamsTurn] = TeamGold[CurrentTeamsTurn] + TempInt;
        PSCC.UpdateGoldThings();
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
            UnitPos.Remove(originalPositionOfUnit);
            if (UnitPos.ContainsKey(originalPositionOfUnit))
            {
                Debug.Log("Key was not deleted");
            }
            UnitPos.Add(MoveToPosition, SelectedUnitPlayScene);
            MoveToPosition = new Vector2(-1, -1);
        }
        SelectedUnitPlayScene.GetComponent<UnitController>().UnitMovable = false; //set unit movable to false
        SelectedUnitPlayScene.GetComponent<UnitController>().UnitMoved = true;
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
    }

    public void CaptureActionPlayScene()
    {
        if (MoveToPosition != new Vector2(-1, -1))
        {
            UnitPos.Remove(originalPositionOfUnit);
            if (UnitPos.ContainsKey(originalPositionOfUnit))
            {
                Debug.Log("Key was not deleted");
            }
            UnitPos.Add(MoveToPosition, SelectedUnitPlayScene);
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
        int tempAttack = Attacker.GetComponent<UnitController>().Attack;
        int tempDefence = TilePos[Defender.transform.position].GetComponent<TerrainController>().DefenceBonus + Defender.GetComponent<UnitController>().Defence;
        int AttackReturn = tempAttack - tempDefence;
        return AttackReturn;
    }
}

/// <summary>
/// Array class for saving maps. Variables(Variable type-name): SeralizableVector2-Location,string-Name,int-Team,string-Type,int-TeamCount
/// </summary>
[Serializable]
public class Map
{
    public SeralizableVector2 Location;
    public string Name;
    public int Team;
    public string Type;
    public List<int> TeamCount;
    public List<string> Mods;
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