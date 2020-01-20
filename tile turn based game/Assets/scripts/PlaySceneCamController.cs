using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class PlaySceneCamController : MonoBehaviour
{
    Vector3 dragOrigin;
    [HideInInspector]
    public Text CurrentPlayerTurnText;
    public bool AttackButtonSelected = false;
    //[HideInInspector]
    public GameObject AttackButton;
    //[HideInInspector]
    public GameObject CancelButton;
    //[HideInInspector]
    public GameObject WaitButton;
    //[HideInInspector]
    public GameObject CaptureButton;
    //[HideInInspector]
    public GameObject MoveButton;
    private GameObject TerrainImage;
    private GameObject TerrainText;
    private GameObject TerrainDescription;
    private GameObject TerrainToolTipSet;
    private GameObject TerrainToolTipData;
    private GameObject UnitImage;
    private GameObject UnitText;
    private GameObject UnitDescription;
    private GameObject UnitToolTipSet;
    private GameObject UnitToolTipData;
    private GameObject BuildingImage;
    private GameObject BuildingText;
    private GameObject BuildingDescription;
    private GameObject BuildingToolTipSet;
    private GameObject BuildingToolTipData;
    public GameObject BuildingButtonPrefab;
    private GameObject ContentWindowBuilding;
    private Text MovableUnitsCountText;
    [HideInInspector]
    public GameObject CurrentPlayerTurnImage;
    private GameObject CurrentPlayerGoldImage;
    public GameObject EndTurnButton;
    private GameObject TooltipPanel;
    private GameObject ActionPanel;
    private GameObject BuildingPanel;
    private GameObject EndGamePanel;
    private GameObject InfoPanel;
    private Text FeedbackSaveMenu;
    private InputField InputFieldSaveMenu;
    private GameObject SavePanel;
    private GameObject SaveButton;
    [HideInInspector]
    public Text FeedBackText;
    [HideInInspector]
    public Text GoldText;
    private Text EndGameText;
    [HideInInspector]
    public Vector2 CurrentlySelectedBuilding;
    [HideInInspector]
    public bool BuildingRayBool;
    private string UnitSelectedForBuildingFromKeyboardShortcuts = "NONE";
    private int MovableUnits;
    private int SelectedUnitDR;
    private GameControllerScript GCS;
    private DatabaseController DBC;

    private void Awake()
    {
        DBC = DatabaseController.instance;
        GCS = GameControllerScript.instance;
        GetObjectReferances();
    }

    private void Start()
    {
        SetActionButtonsToFalse();
        BuildingPanel.SetActive(false);
        InfoPanel.SetActive(true);
    }

    void Update()
    {
        MoveScreenXandY();
        MoveScreenZ();
        RayCasterForPlayScene();
        KeyBoardShortcuts();
    }

    /// <summary>
    /// Gets objects from objects this script is attatched to
    /// </summary>
    private void GetObjectReferances()
    {
        EndGameText = transform.Find("Canvas").Find("Panel").Find("EndGamePanel").Find("EndGameText").GetComponent<Text>();
        CurrentPlayerTurnText = transform.Find("Canvas").Find("Panel").Find("CurrentPlayerTurnImage").Find("CurrentPlayerTurnText").GetComponent<Text>();
        GoldText = transform.Find("Canvas").Find("Panel").Find("GoldImage").Find("GoldText").GetComponent<Text>();
        FeedBackText = transform.Find("Canvas").Find("Panel").Find("FeedbackText").GetComponent<Text>();
        AttackButton = transform.Find("Canvas").Find("Panel").Find("ActionPanel").Find("AttackButton").gameObject;
        CancelButton = transform.Find("Canvas").Find("Panel").Find("ActionPanel").Find("CancelButton").gameObject;
        CaptureButton = transform.Find("Canvas").Find("Panel").Find("ActionPanel").Find("CaptureButton").gameObject;
        MoveButton = transform.Find("Canvas").Find("Panel").Find("ActionPanel").Find("MoveButton").gameObject;
        WaitButton = transform.Find("Canvas").Find("Panel").Find("ActionPanel").Find("WaitButton").gameObject;
        TerrainImage = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("TerrainImage").gameObject;
        TerrainText = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("TerrainText").gameObject;
        TerrainDescription = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("TerrainDescription").gameObject;
        TerrainToolTipSet = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("TerrainToolTipSet").gameObject;
        TerrainToolTipData = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("TerrainToolTipData").gameObject;
        UnitImage = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitImage").gameObject;
        UnitText = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitText").gameObject;
        UnitDescription = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitDescription").gameObject;
        UnitToolTipSet = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitToolTipSet").gameObject;
        UnitToolTipData = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitToolTipData").gameObject;
        BuildingImage = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("BuildingImage").gameObject;
        BuildingText = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("BuildingText").gameObject;
        BuildingDescription = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("BuildingDescription").gameObject;
        BuildingToolTipSet = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("BuildingToolTipSet").gameObject;
        BuildingToolTipData = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("BuildingToolTipData").gameObject;
        ContentWindowBuilding = transform.Find("Canvas").Find("Panel").Find("BuildingPanel").Find("Viewport").Find("BuildingContent").gameObject;
        CurrentPlayerTurnImage = transform.Find("Canvas").Find("Panel").Find("CurrentPlayerTurnImage").gameObject;
        EndTurnButton = transform.Find("Canvas").Find("Panel").Find("EndTurnButton").gameObject;
        TooltipPanel = transform.Find("Canvas").Find("Panel").Find("ToolTip").gameObject;
        ActionPanel = transform.Find("Canvas").Find("Panel").Find("ActionPanel").gameObject;
        BuildingPanel = transform.Find("Canvas").Find("Panel").Find("BuildingPanel").gameObject;
        EndGamePanel = transform.Find("Canvas").Find("Panel").Find("EndGamePanel").gameObject;
        InfoPanel = transform.Find("Canvas").Find("InfoPanel").gameObject;
        SaveButton = transform.Find("Canvas").Find("Panel").Find("SaveButton").gameObject;
        FeedbackSaveMenu = transform.Find("Canvas").Find("SavePanel").Find("FeedBackText").GetComponent<Text>();
        InputFieldSaveMenu = transform.Find("Canvas").Find("SavePanel").Find("InputField").GetComponent<InputField>();
        SavePanel = transform.Find("Canvas").Find("SavePanel").gameObject;
        MovableUnitsCountText = transform.Find("Canvas").Find("Panel").Find("MovableUnitsImage").Find("MovableUnitsText").GetComponent<Text>();
    }

    /// <summary>
    /// Used to controll the x and y movements of teh camera
    /// </summary>
    private void MoveScreenXandY()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

        Vector3 move = new Vector3(pos.x * DBC.dragSpeedOffset * DBC.DragSpeed * -1, pos.y * DBC.dragSpeedOffset * DBC.DragSpeed * -1, 0);

        transform.Translate(move, Space.World);

        if (gameObject.transform.position.x > GCS.PlayMapSize) { gameObject.transform.position = new Vector3(GCS.PlayMapSize, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y > GCS.PlayMapSize) { gameObject.transform.position = new Vector3(transform.position.x, GCS.PlayMapSize, transform.position.z); }
        if (gameObject.transform.position.x < 0) { gameObject.transform.position = new Vector3(0, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y < 0) { gameObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z); }

    } //controls camera movment y and x

    /// <summary>
    /// Used to control the z movement of the camera
    /// </summary>
    private void MoveScreenZ()
    {
        int z = new int();
        if (Input.GetAxis("Mouse ScrollWheel") > 0) { z = DBC.scrollSpeed; }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) { z = -DBC.scrollSpeed; }

        transform.Translate(new Vector3(0, 0, z), Space.World);

        if (gameObject.transform.position.z > -1) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -1); }
        if (gameObject.transform.position.z < -GCS.PlayMapSize * 2) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -GCS.PlayMapSize * 2); }

    }//controls camera z movement

    /// <summary>
    /// When a player clicks end of turn button this runs
    /// </summary>
    public void EndTurnButtonClicked()
    {
        BuildingPanel.SetActive(false);
        if (GCS.SelectedUnitPlayScene != null)
        {
            GCS.WaitActionPlayScene();
        }
        GCS.PlaySceneTurnChanger();
        UpdateTurnImageColor(GCS.CurrentTeamsTurn.Team);
        EventSystem.current.SetSelectedGameObject(null);
        MoveableUnitCount();
    }

    /// <summary>
    /// Displays or hides attack button.
    /// </summary>
    /// <param name="enemyCount">Number of units in range</param>
    public void AttackButtonController(int enemyCount)
    {
        if (enemyCount >= 1)
        {
            AttackButton.SetActive(true);
        }
        else
        {
            AttackButton.SetActive(false);
        }
        WaitButton.SetActive(true);
    }

    /// <summary>
    /// Runs when the wait button is clicked
    /// </summary>
    public void WaitButtonClicked()
    {
        GCS.WaitActionPlayScene();
        MoveableUnitCount();
    }

    /// <summary>
    /// Runs when attack button is clicked
    /// </summary>
    public void AttackButtonClicked()
    {
        GCS.AttackActionPlayScene();
        AttackButtonSelected = true;

    }

    /// <summary>
    /// Runs when the cancel button is clicked
    /// </summary>
    public void CancelButtonClicked()
    {
        GCS.CancelActionPlayScene();
        AttackButtonSelected = false;

    }

    public void CaptureButtonClicked()
    {
        GCS.CaptureActionPlayScene();
        MoveableUnitCount();
    }

    /// <summary>
    /// Hides all the action buttons
    /// </summary>
    public void SetActionButtonsToFalse()
    {
        AttackButton.SetActive(false);
        WaitButton.SetActive(false);
        CancelButton.SetActive(false);
        CaptureButton.SetActive(false);
        MoveButton.SetActive(false);
        MoveableUnitCount();
    }

    /// <summary>
    /// Used to update tooltip data
    /// </summary>
    public void RayCasterForPlayScene()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject()) //dont want to click through menus
            {
                ////Debug.log("RayHitStarted");
                Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); //GET THEM RAYS
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray);
                BuildingRayBool = false;
                ////Debug.log("Starting play scene ray hits");
                for (int i = 0; i < hits.Length; i++) // GO THROUGH THEM RAYS
                {
                    RaycastHit hit = hits[i];
                    if (hit.transform.tag == DBC.TerrainDictionary[0].Type && !hit.transform.GetComponent<TerrainController>().FogOfWarBool) //did we hit a terrain?
                    {
                        foreach (var kvp in DBC.TerrainDictionary)
                        {
                            if (kvp.Value.Title == hit.transform.name)
                            {
                                TerrainImage.GetComponent<Image>().sprite = DBC.loadSprite(DBC.TerrainDictionary[kvp.Key].ArtworkDirectory[0],DBC.TerrainDictionary[kvp.Key].PixelsPerUnit);
                                TerrainText.GetComponent<Text>().text = kvp.Value.Title;
                                TerrainDescription.GetComponent<Text>().text = kvp.Value.Description;
                                TerrainToolTipData.GetComponent<Text>().text = kvp.Value.DefenceBonus.ToString() + Environment.NewLine + kvp.Value.Walkable.ToString() + Environment.NewLine + kvp.Value.Weight.ToString();
                            }
                        }
                    }
                    if (hit.transform.tag == DBC.UnitDictionary[0].Type && !GCS.TilePos[(Vector2)hit.transform.position].GetComponent<TerrainController>().FogOfWarBool) //did we hit a unit?
                    {
                        foreach (var kvp in DBC.UnitDictionary)
                        {
                            if (kvp.Value.Title == hit.transform.name)
                            {
                                UnitImage.GetComponent<Image>().sprite = DBC.loadSprite(DBC.UnitDictionary[kvp.Key].ArtworkDirectory[0],DBC.UnitDictionary[kvp.Key].PixelsPerUnit);
                                UnitText.GetComponent<Text>().text = kvp.Value.Title;
                                UnitDescription.GetComponent<Text>().text = kvp.Value.Description;
                                UnitToolTipData.GetComponent<Text>().text = kvp.Value.Attack.ToString() + Environment.NewLine + kvp.Value.Defence.ToString() + Environment.NewLine + kvp.Value.AttackRange.ToString() + Environment.NewLine + kvp.Value.MovePoints.ToString();
                            }
                        }
                    }
                    if (hit.transform.tag == DBC.BuildingDictionary[0].Type && !GCS.TilePos[(Vector2)hit.transform.position].GetComponent<TerrainController>().FogOfWarBool) //did we hit a building?
                    {
                        foreach (var kvp in DBC.BuildingDictionary)
                        {
                            if (kvp.Value.Title == hit.transform.name)
                            {
                                BuildingImage.GetComponent<Image>().sprite = DBC.loadSprite(DBC.BuildingDictionary[kvp.Key].ArtworkDirectory[0],DBC.BuildingDictionary[kvp.Key].PixelsPerUnit);
                                BuildingText.GetComponent<Text>().text = kvp.Value.Title;
                                BuildingDescription.GetComponent<Text>().text = kvp.Value.Description;
                                BuildingToolTipData.GetComponent<Text>().text = kvp.Value.DefenceBonus.ToString();
                            }
                        }
                    }
                    if (hit.transform.tag == DBC.BuildingDictionary[0].Type)
                    {
                        if (!BuildingRayBool)
                        {
                            BuildingRayBool = true;
                            //Debug.log("1");
                            if (!hit.transform.GetComponent<BuildingController>().Occupied && hit.transform.GetComponent<BuildingController>().CanBuild && hit.transform.GetComponent<BuildingController>().Team == GCS.CurrentTeamsTurn.Team)
                            {
                                if (GCS.SelectedUnitPlayScene == null && hit.transform.GetComponent<BuildingController>().CanBuildUnits)
                                {
                                    //Debug.log("1.1");
                                    AddUnitButtonsToBuildContent(hit.transform.GetComponent<BuildingController>());
                                    BuildingPanel.SetActive(true);
                                    CurrentlySelectedBuilding = hit.transform.position;
                                }
                            }
                            else
                            {
                                //Debug.log("1.2");
                                BuildingPanel.SetActive(false);
                            }
                        }
                    }
                    else if (hit.transform.tag != DBC.BuildingDictionary[0].Type && !BuildingRayBool)
                    {
                        //Debug.log("2");
                        BuildingPanel.SetActive(false);
                    }
                }
                BuildingRayBool = false;
                FeedBackText.text = "";
            }
        }

    }

    /// <summary>
    /// Adds all units found to building window
    /// </summary>
    private void AddUnitButtonsToBuildContent(BuildingController BuildingHit)
    {
        var childcount = ContentWindowBuilding.transform.childCount;
        for (int i = 0; i < childcount; i++)
        {
            Destroy(ContentWindowBuilding.transform.GetChild(i).gameObject);
        }
        //Debug.log("Adding Unit buttons to content window");
        foreach (KeyValuePair<int, Unit> kvp in DBC.UnitDictionary) //adds a button for each Unit in the database
        {
            if (kvp.Value.Mod == BuildingHit.Mod && BuildingHit.BuildableUnits.Contains(kvp.Value.Title))
            {
                GameObject tempbutton = Instantiate(BuildingButtonPrefab, ContentWindowBuilding.transform); //create button and set its parent to content
                tempbutton.name = kvp.Value.Title; //change name
                tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title + System.Environment.NewLine + kvp.Value.Cost; //change text on button to match sprite and create new line and add cost
                tempbutton.GetComponent<Image>().sprite = DBC.loadSprite(DBC.UnitDictionary[kvp.Key].ArtworkDirectory[0],DBC.UnitDictionary[kvp.Key].PixelsPerUnit); //set sprite
                tempbutton.GetComponent<Button>().onClick.AddListener(CreateUnitController); //adds method to button clicked 
                tempbutton.AddComponent<ButtonProperties>();
                tempbutton.GetComponent<ButtonProperties>().ID = kvp.Key;
            }
        }

    }

    /// <summary>
    /// Creates unit, sets building var buildable to false, removes gold
    /// </summary>
    public void CreateUnitController()
    {
        if (EventSystem.current.currentSelectedGameObject != null) //make sure player clicked on the unit button and did not use keyboard shortcut
        {
            SelectedUnitDR = EventSystem.current.currentSelectedGameObject.GetComponent<ButtonProperties>().ID;
            if (GCS.TeamList[GCS.CurrentTeamsTurn.Team].Gold >= DBC.UnitDictionary[SelectedUnitDR].Cost) //can they afford this unit?
            {
                GameObject GO = DBC.CreateAndSpawnUnit(CurrentlySelectedBuilding, DBC.UnitDictionary[SelectedUnitDR].ID, GCS.CurrentTeamsTurn.Team);
                GCS.AddUnitsToDictionary(GO);
                GO.GetComponent<UnitController>().UnitMovable = false;
                foreach (var unit in GCS.UnitPos)
                {
                    unit.Value.GetComponent<UnitController>().GetTileValues();
                    unit.Value.GetComponent<UnitController>().GetSightTiles();
                }
                BuildingPanel.SetActive(false);
                foreach (var b in GCS.BuildingPos)
                {
                    if ((Vector2)b.Value.transform.position == CurrentlySelectedBuilding)
                    {
                        b.Value.GetComponent<BuildingController>().CanBuild = false;
                    }
                }
                foreach (var t in GCS.TilePos)
                {
                    t.Value.GetComponent<TerrainController>().FogOfWarController();
                }
                GCS.TeamList[GCS.CurrentTeamsTurn.Team].Gold = GCS.TeamList[GCS.CurrentTeamsTurn.Team].Gold - DBC.UnitDictionary[SelectedUnitDR].Cost;
                UpdateGoldThings();
                FeedBackText.text = "";
            }
            else
            {
                FeedBackText.text = "Not enough gold to buy that unit";
            }
        }
        else //keyboard shortcut was used
        {
            if (GCS.TeamList[GCS.CurrentTeamsTurn.Team].Gold >= DBC.UnitDictionary[SelectedUnitDR].Cost)
            {
                GameObject GO = DBC.CreateAndSpawnUnit(CurrentlySelectedBuilding, DBC.UnitDictionary[SelectedUnitDR].ID, GCS.CurrentTeamsTurn.Team);
                GCS.AddUnitsToDictionary(GO);
                GO.GetComponent<UnitController>().UnitMovable = false;
                foreach (var unit in GCS.UnitPos)
                {
                    unit.Value.GetComponent<UnitController>().GetTileValues();
                    unit.Value.GetComponent<UnitController>().GetSightTiles();
                }
                BuildingPanel.SetActive(false);
                foreach (var b in GCS.BuildingPos)
                {
                    if ((Vector2)b.Value.transform.position == CurrentlySelectedBuilding)
                    {
                        b.Value.GetComponent<BuildingController>().CanBuild = false;
                    }
                }
                foreach (var t in GCS.TilePos)
                {
                    t.Value.GetComponent<TerrainController>().FogOfWarController();
                }
                GCS.TeamList[GCS.CurrentTeamsTurn.Team].Gold = GCS.TeamList[GCS.CurrentTeamsTurn.Team].Gold - DBC.UnitDictionary[SelectedUnitDR].Cost;
                UpdateGoldThings();
                UnitSelectedForBuildingFromKeyboardShortcuts = "NONE";
                FeedBackText.text = "";
            }
            else
            {
                FeedBackText.text = "Not enough gold to buy that unit";
            }
        }
    }

    /// <summary>
    /// Sets requred panles to false and displayes winning message
    /// </summary>
    /// <param name="Team"></param>
    public void GameEndController(int Team)
    {
        CurrentPlayerTurnImage.SetActive(false);
        EndTurnButton.SetActive(false);
        TooltipPanel.SetActive(false);
        ActionPanel.SetActive(false);
        BuildingPanel.SetActive(false);
        EndGamePanel.SetActive(true);
        SaveButton.SetActive(false);
        EndGameText.text = "Team: " + Team.ToString() + " has won the game.";

    }

    /// <summary>
    /// Clears scene and then goes back to main menue
    /// </summary>
    public void MainMenuButtonClicked()
    {
        foreach (var GO in GameObject.FindGameObjectsWithTag("Terrain"))
        {
            Destroy(GO);
        }
        foreach (var GO in GameObject.FindGameObjectsWithTag("Unit"))
        {
            Destroy(GO);
        }
        foreach (var GO in GameObject.FindGameObjectsWithTag("Building"))
        {
            Destroy(GO);
        }
        GCS.BuildingPos = new Dictionary<Vector2, GameObject>();
        GCS.TilePos = new Dictionary<Vector2, GameObject>();
        GCS.UnitPos = new Dictionary<Vector2, GameObject>();
        SceneManager.LoadScene("MainMenuScene");

    }

    /// <summary>
    /// Clears scene and reinitalizes the current map
    /// </summary>
    public void ReplayButtonClicked()
    {
        foreach(var t in GCS.TeamList)
        {
            if (t.Active)
            {
                t.Defeated = false;
                t.Gold = 0;
            }
        }
        foreach (var GO in GameObject.FindGameObjectsWithTag("Terrain"))
        {
            Destroy(GO);
        }
        foreach (var GO in GameObject.FindGameObjectsWithTag("Unit"))
        {
            Destroy(GO);
        }
        foreach (var GO in GameObject.FindGameObjectsWithTag("Building"))
        {
            Destroy(GO);
        }
        GCS.BuildingPos = new Dictionary<Vector2, GameObject>();
        GCS.TilePos = new Dictionary<Vector2, GameObject>();
        GCS.UnitPos = new Dictionary<Vector2, GameObject>();
        GCS.LoadMapPlayScene(GCS.MapNameForPlayScene);
        GCS.PlaySceneNewGameInitalizer();
        CurrentPlayerTurnImage.SetActive(true);
        EndTurnButton.SetActive(true);
        TooltipPanel.SetActive(true);
        ActionPanel.SetActive(true);
        EndGamePanel.SetActive(false);
    }

    /// <summary>
    /// Used to update gold related things
    /// </summary>
    public void UpdateGoldThings()
    {
        GoldText.text = "Gold:" + GCS.TeamList[GCS.CurrentTeamsTurn.Team].Gold.ToString();
    }

    public void UpdateTurnImageColor(int team)
    {
        //Black,Blue,Cyan,Gray,Green,Magenta,Red,White,Yellow
        switch (team)
        {
            case 1:
                CurrentPlayerTurnImage.GetComponent<Image>().color = Color.black;
                CurrentPlayerTurnText.color = Color.white;
                break;
            case 2:
                CurrentPlayerTurnImage.GetComponent<Image>().color = Color.blue;
                CurrentPlayerTurnText.color = Color.white;
                break;
            case 3:
                CurrentPlayerTurnImage.GetComponent<Image>().color = Color.cyan;
                CurrentPlayerTurnText.color = Color.black;
                break;
            case 4:
                CurrentPlayerTurnImage.GetComponent<Image>().color = Color.gray;
                CurrentPlayerTurnText.color = Color.black;
                break;
            case 5:
                CurrentPlayerTurnImage.GetComponent<Image>().color = Color.green;
                CurrentPlayerTurnText.color = Color.black;
                break;
            case 6:
                CurrentPlayerTurnImage.GetComponent<Image>().color = Color.magenta;
                CurrentPlayerTurnText.color = Color.black;
                break;
            case 7:
                CurrentPlayerTurnImage.GetComponent<Image>().color = Color.red;
                CurrentPlayerTurnText.color = Color.black;
                break;
            case 8:
                CurrentPlayerTurnImage.GetComponent<Image>().color = Color.white;
                CurrentPlayerTurnText.color = Color.black;
                break;
            case 9:
                CurrentPlayerTurnImage.GetComponent<Image>().color = Color.yellow;
                CurrentPlayerTurnText.color = Color.black;
                break;
        }
    }

    public void HideInfoPanel()
    {
        InfoPanel.SetActive(false);
    }

    public void HideOrShowSaveButton(bool Show)
    {
        if (Show)
        {
            SaveButton.SetActive(true);
        }
        else
        {
            SaveButton.SetActive(false);
        }
    }

    public void SaveButtonMainPanelController()
    {
        SavePanel.SetActive(true);
    }

    public void BackToGameController()
    {
        SavePanel.SetActive(false);
    }

    public void SaveButtonSavePanelController()
    {
        string SaveGameName = InputFieldSaveMenu.text;
        if (!System.IO.File.Exists(Application.dataPath + "/StreamingAssets/Saves/" + SaveGameName + ".json"))
        {
            if (SaveGameName != "")
            {
                if (!Regex.IsMatch(SaveGameName, @"^[a-z][A-Z]+$"))
                {
                    GCS.SaveGame(SaveGameName);
                    FeedbackSaveMenu.text = "Saveing Game.";
                }
                else
                {
                    FeedbackSaveMenu.text = "Use only letters please.";
                }
            }else
            {
                FeedbackSaveMenu.text = "Must enter a name.";
            }
        }else
        {
            FeedbackSaveMenu.text = "A save file with that name already exist, pick a new one or delete old one.";
        }
    }

    public void MoveButtonClicked()
    {
        GCS.MoveActionPlayScene();
        MoveableUnitCount();
    }

    /// <summary>
    /// Used to controll what happens when keyboard shortcuts are used
    /// </summary>
    public void KeyBoardShortcuts()
    {
        if (Input.GetKeyDown(KeyCode.Q) && AttackButton.activeSelf && !MoveButton.activeSelf)
        {
            AttackButtonClicked();
        }
        if (Input.GetKeyDown(KeyCode.Q) && MoveButton.activeSelf)
        {
            MoveButtonClicked();
        }
        if (Input.GetKeyDown(KeyCode.W) && CaptureButton.activeSelf)
        {
            CaptureButtonClicked();
        }
        if (Input.GetKeyDown(KeyCode.E) && WaitButton.activeSelf)
        {
            WaitButtonClicked();
        }
        if (Input.GetKeyDown(KeyCode.R) && CancelButton.activeSelf)
        {
            CancelButtonClicked();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurnButtonClicked();
        }
        if (BuildingPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (ContentWindowBuilding.transform.childCount > 0)
                {
                    SelectedUnitDR = ContentWindowBuilding.transform.GetChild(0).GetComponent<ButtonProperties>().ID;
                    CreateUnitController();
                }
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (ContentWindowBuilding.transform.childCount > 1)
                {
                    SelectedUnitDR = ContentWindowBuilding.transform.GetChild(1).GetComponent<ButtonProperties>().ID;
                    CreateUnitController();
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (ContentWindowBuilding.transform.childCount > 2)
                {
                    SelectedUnitDR = ContentWindowBuilding.transform.GetChild(2).GetComponent<ButtonProperties>().ID;
                    CreateUnitController();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (ContentWindowBuilding.transform.childCount > 3)
                {
                    SelectedUnitDR = ContentWindowBuilding.transform.GetChild(3).GetComponent<ButtonProperties>().ID;
                    CreateUnitController();
                }
            }
        }
    }

    public void MoveableUnitCount()
    {
        MovableUnits = 0;
        foreach (var kvp in GCS.UnitPos)
        {
            if (GCS.CurrentTeamsTurn.Team == kvp.Value.GetComponent<UnitController>().Team)
            {
                if (kvp.Value.GetComponent<UnitController>().UnitMovable)
                {
                    MovableUnits = MovableUnits + 1;
                }
            }
        }
        MovableUnitsCountText.text = MovableUnits.ToString();
    }
}
