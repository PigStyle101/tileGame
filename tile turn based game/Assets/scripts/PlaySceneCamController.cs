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
    [HideInInspector]
    public GameObject AttackButton;
    [HideInInspector]
    public GameObject CancelButton;
    [HideInInspector]
    public GameObject WaitButton;
    [HideInInspector]
    public GameObject CaptureButton;
    [HideInInspector]
    public GameObject MoveButton;
    //ToolTip Stuff
    private GameObject TerrainImage;
    private Text TerrainText;
    private Text TerrainDescription;
    private Text TerrainToolTipDefence;
    private Text TerrainToolTipWalkable;
    private Text TerrainToolTipWeight;
    private GameObject UnitImage;
    private Text UnitText;
    private Text UnitDescription;
    private Text UnitToolTipAttack;
    private Text UnitToolTipDefence;
    private Text UnitToolTipAttackRange;
    private Text UnitToolTipMovePoints;
    private Text UnitToolTipSightRange;
    private Text UnitToolTipConversionSpeed;
    private GameObject BuildingImage;
    private Text BuildingText;
    private Text BuildingDescription;
    private Text BuildingToolTipDefence;
    //Info Stats Panel
    private GameObject InfoAndStatsPanel;
    private GameObject HeroStatButton;
    private GameObject InfoPanel;
    private Text TitleText;
    private Text TypeText;
    private Text ModText;
    private Text CanConvertText;
    private Text CanMoveAndAttackText;
    private Text DescriptionText;
    private GameObject StatsPanel;
    private Text LevelText;
    private Text XPText;
    private Text AttackText;
    private Text DefenceText;
    private Text HealthText;
    private Text AttackRangeText;
    private Text MovePointsText;
    private Text ConversionSpeedText;
    private Text SightRangeText;
    private GameObject HeroStatsPanel;
    private Text StrenghtText;
    private Text StrenghtPerLvlText;
    private Text HeroAttackText;
    private Text HeroHealthText;
    private Text HeroHealthRegenText;
    private Text DexterityText;
    private Text DexterityPerLvlText;
    private Text HeroMovePointsText;
    private Text HeroDefenceText;
    private Text HeroConversionSpeedText;
    private Text IntelliganceText;
    private Text IntelligancePerLvlText;
    private Text HeroManaRegenText;
    private Text HeroMaxManaText;
    private Text HeroXpGainText;
    private Text CharismaText;
    private Text CharismaPerLvlText;
    private Text HeroUnitCostText;
    private Text MoraleText;
    private Text ReturnLimitText;
    //Other stuff
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
    private GameObject StartInfoPanel;
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
        StartInfoPanel.SetActive(true);
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
        //tool tip stuff
        TerrainImage = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("TerrainImage").gameObject;
        TerrainText = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("TerrainText").GetComponent<Text>();
        TerrainDescription = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("TerrainDescription").GetComponent<Text>();
        TerrainToolTipDefence = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("TerrainToolTipDef").GetComponent<Text>();
        TerrainToolTipWalkable = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("TerrainToolTipWalkable").GetComponent<Text>();
        TerrainToolTipWeight = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("TerrainToolTipWeight").GetComponent<Text>();
        UnitImage = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitImage").gameObject;
        UnitText = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitText").GetComponent<Text>();
        UnitDescription = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitDescription").GetComponent<Text>();
        UnitToolTipAttack = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitToolTipAttack").GetComponent<Text>();
        UnitToolTipDefence = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitToolTipDefence").GetComponent<Text>();
        UnitToolTipAttackRange = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitToolTipAttackRange").GetComponent<Text>();
        UnitToolTipMovePoints = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitToolTipMovePoints").GetComponent<Text>();
        UnitToolTipSightRange = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitToolTipSightRange").GetComponent<Text>();
        UnitToolTipConversionSpeed = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("UnitToolTipConversionSpeed").GetComponent<Text>();
        BuildingImage = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("BuildingImage").gameObject;
        BuildingText = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("BuildingText").GetComponent<Text>();
        BuildingDescription = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("BuildingDescription").GetComponent<Text>();
        BuildingToolTipDefence = transform.Find("Canvas").Find("Panel").Find("ToolTip").Find("BuildingToolTipDefence").GetComponent<Text>();
        //InfoStat Panel stuff
        InfoAndStatsPanel = transform.Find("Canvas").Find("InfoAndStatPanel").gameObject;
        HeroStatButton = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsButton").gameObject;
        InfoPanel = transform.Find("Canvas").Find("InfoAndStatPanel").Find("InfoPanel").gameObject;
        TitleText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("InfoPanel").Find("Title").GetComponent<Text>();
        TypeText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("InfoPanel").Find("Type").GetComponent<Text>();
        ModText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("InfoPanel").Find("Mod").GetComponent<Text>();
        CanConvertText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("InfoPanel").Find("CanConvert").GetComponent<Text>();
        CanMoveAndAttackText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("InfoPanel").Find("CanMoveAndAttack").GetComponent<Text>();
        DescriptionText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("InfoPanel").Find("Description").GetComponent<Text>();
        StatsPanel = transform.Find("Canvas").Find("InfoAndStatPanel").Find("StatPanel").gameObject;
        LevelText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("StatPanel").Find("Level").GetComponent<Text>();
        XPText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("StatPanel").Find("XP").GetComponent<Text>();
        AttackText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("StatPanel").Find("Attack").GetComponent<Text>();
        DefenceText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("StatPanel").Find("Defence").GetComponent<Text>();
        HealthText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("StatPanel").Find("Health").GetComponent<Text>();
        AttackRangeText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("StatPanel").Find("AttackRange").GetComponent<Text>();
        MovePointsText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("StatPanel").Find("MovePoints").GetComponent<Text>();
        ConversionSpeedText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("StatPanel").Find("ConversionSpeed").GetComponent<Text>();
        SightRangeText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("StatPanel").Find("SightRange").GetComponent<Text>();
        HeroStatsPanel = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").gameObject;
        StrenghtText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("Strenght").GetComponent<Text>();
        StrenghtPerLvlText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("StrenghtPerLvl").GetComponent<Text>();
        HeroAttackText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("Attack").GetComponent<Text>();
        HeroHealthText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("Health").GetComponent<Text>();
        HeroHealthRegenText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("HealthRegen").GetComponent<Text>();
        DexterityText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("Dexterity").GetComponent<Text>();
        DexterityPerLvlText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("DexPerLvl").GetComponent<Text>();
        HeroMovePointsText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("MovePoints").GetComponent<Text>();
        HeroDefenceText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("Defence").GetComponent<Text>();
        HeroConversionSpeedText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("ConversionSpeed").GetComponent<Text>();
        IntelliganceText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("Intelligance").GetComponent<Text>();
        IntelligancePerLvlText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("IntPerLvl").GetComponent<Text>();
        HeroManaRegenText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("ManaRegen").GetComponent<Text>();
        HeroMaxManaText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("MaxMana").GetComponent<Text>();
        HeroXpGainText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("XpGain").GetComponent<Text>();
        CharismaText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("Charisma").GetComponent<Text>();
        CharismaPerLvlText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("CharPerLvl").GetComponent<Text>();
        HeroUnitCostText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("UnitCost").GetComponent<Text>();
        MoraleText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("Morale").GetComponent<Text>();
        ReturnLimitText = transform.Find("Canvas").Find("InfoAndStatPanel").Find("HeroStatsPanel").Find("ReturnLimit").GetComponent<Text>();
        //Other Stuff
        ContentWindowBuilding = transform.Find("Canvas").Find("Panel").Find("BuildingPanel").Find("Viewport").Find("BuildingContent").gameObject;
        CurrentPlayerTurnImage = transform.Find("Canvas").Find("Panel").Find("CurrentPlayerTurnImage").gameObject;
        EndTurnButton = transform.Find("Canvas").Find("Panel").Find("EndTurnButton").gameObject;
        TooltipPanel = transform.Find("Canvas").Find("Panel").Find("ToolTip").gameObject;
        ActionPanel = transform.Find("Canvas").Find("Panel").Find("ActionPanel").gameObject;
        BuildingPanel = transform.Find("Canvas").Find("Panel").Find("BuildingPanel").gameObject;
        EndGamePanel = transform.Find("Canvas").Find("Panel").Find("EndGamePanel").gameObject;
        StartInfoPanel = transform.Find("Canvas").Find("InfoPanel").gameObject;
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

    } //controls camera movment y and x //Potential lua code

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

    }//controls camera z movement //Potential lua code

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
                                TerrainImage.GetComponent<Image>().sprite = DBC.TerrainDictionary[kvp.Key].ArtworkDirectory[0];
                                TerrainText.text = kvp.Value.Title;
                                TerrainDescription.text = kvp.Value.Description;
                                TerrainToolTipDefence.text = "Defence Bonus:" + kvp.Value.DefenceBonus;
                                TerrainToolTipWalkable.text = "Walkable:" + kvp.Value.Walkable;
                                TerrainToolTipWeight.text = "Weight:" + kvp.Value.Weight;
                            }
                        }
                    }
                    if (hit.transform.tag == DBC.UnitDictionary[0].Type && !GCS.TilePos[(Vector2)hit.transform.position].GetComponent<TerrainController>().FogOfWarBool) //did we hit a unit?
                    {
                        UnitController UC = hit.transform.GetComponent<UnitController>();
                        Unit U = DBC.UnitDictionary[UC.ID];
                        UnitImage.GetComponent<Image>().sprite = DBC.UnitDictionary[U.ID].IconSprite;
                        UnitText.text = U.Title;
                        UnitDescription.text = U.Description;
                        UnitToolTipAttack.text = "Attack:" + UC.Attack;
                        UnitToolTipDefence.text = "Defence:" + UC.Defence;
                        UnitToolTipAttackRange.text = "Attack Range:" + UC.AttackRange;
                        UnitToolTipMovePoints.text = "Move Points:" + UC.MovePoints;
                        UnitToolTipSightRange.text = "Sight Range:" + UC.SightRange;
                        UnitToolTipConversionSpeed.text = "Conversion Speed:" + UC.ConversionSpeed;
                    }
                    if (hit.transform.tag == DBC.HeroDictionary[0].Type && !GCS.TilePos[(Vector2)hit.transform.position].GetComponent<TerrainController>().FogOfWarBool) //did we hit a hero?
                    {
                        UnitController UC = hit.transform.GetComponent<UnitController>();
                        Hero THero = UC.HClass;
                        UnitImage.GetComponent<Image>().sprite = DBC.HeroDictionary[UC.ID].IconSprite;
                        UnitText.text = THero.Title;
                        UnitDescription.text = THero.Description;
                        UnitToolTipAttack.text = "Attack:" + THero.Attack;
                        UnitToolTipDefence.text = "Defence:" + THero.Defence;
                        UnitToolTipAttackRange.text = "Attack Range:" + UC.AttackRange;
                        UnitToolTipMovePoints.text = "Move Points:" + THero.MovePoints;
                        UnitToolTipSightRange.text = "Sight Range:" + UC.SightRange;
                        UnitToolTipConversionSpeed.text = "Conversion Speed:" + THero.ConversionSpeed;
                    }
                    if (hit.transform.tag == DBC.BuildingDictionary[0].Type && !GCS.TilePos[(Vector2)hit.transform.position].GetComponent<TerrainController>().FogOfWarBool) //did we hit a building?
                    {
                        foreach (var kvp in DBC.BuildingDictionary)
                        {
                            if (kvp.Value.Title == hit.transform.name)
                            {
                                BuildingImage.GetComponent<Image>().sprite = DBC.BuildingDictionary[kvp.Key].ArtworkDirectory[0];
                                BuildingText.text = kvp.Value.Title;
                                BuildingDescription.text = kvp.Value.Description;
                                BuildingToolTipDefence.text = "Defence:" + kvp.Value.DefenceBonus;
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
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
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
                    if (hit.transform.tag == DBC.UnitDictionary[0].Type && !GCS.TilePos[(Vector2)hit.transform.position].GetComponent<TerrainController>().FogOfWarBool) //did we hit a unit?
                    {
                        UnitController UC = hit.transform.GetComponent<UnitController>();
                        Unit TUnit = DBC.UnitDictionary[UC.ID];
                        
                        TitleText.text = "Title:" + TUnit.Title;
                        TypeText.text = "Type:" + TUnit.Type;
                        ModText.text = "Mod:" + TUnit.Mod;
                        CanConvertText.text = "Can convert buildings:" + UC.CanConvert;
                        CanMoveAndAttackText.text = "Can move and attack:" + UC.CanMoveAndAttack;
                        DescriptionText.text = "Description:" + TUnit.Description;
                        LevelText.text = "Level:" + UC.Level;
                        XPText.text = "XP:" + UC.XP;
                        AttackText.text = "Attack:" + UC.Attack;
                        DefenceText.text = "Defence:" + UC.Defence;
                        HealthText.text = "Health:" + UC.Health;
                        AttackRangeText.text = "Attack Range:" + UC.AttackRange;
                        MovePointsText.text = "Move Points:" + UC.MovePoints;
                        ConversionSpeedText.text = "Conversion Speed:" + UC.ConversionSpeed;
                        SightRangeText.text = "Sight Range:" + UC.SightRange;
                        InfoAndStatsPanel.SetActive(true);
                        InfoButtonClicked();
                        HeroStatButton.SetActive(false);
                    }
                    if (hit.transform.tag == DBC.HeroDictionary[0].Type && !GCS.TilePos[(Vector2)hit.transform.position].GetComponent<TerrainController>().FogOfWarBool) //did we hit a hero?
                    {
                        UnitController UC = hit.transform.GetComponent<UnitController>();
                        Hero THero = UC.HClass;
                        
                        TitleText.text = "Title:" + THero.Title;
                        TypeText.text = "Type:" + THero.Type;
                        ModText.text = "Mod:" + THero.Mod;
                        CanConvertText.text = "Can convert buildings:" + UC.CanConvert;
                        CanMoveAndAttackText.text = "Can move and attack:" + UC.CanMoveAndAttack;
                        DescriptionText.text = "Description:" + THero.Description;
                        LevelText.text = "Level:" + UC.Level;
                        XPText.text = "XP:" + UC.XP;
                        AttackText.text = "Attack:" + THero.Attack;
                        DefenceText.text = "Defence:" + THero.Defence;
                        HealthText.text = "Health:" + THero.Health;
                        AttackRangeText.text = "Attack Range:" + UC.AttackRange;
                        MovePointsText.text = "Move Points:" + THero.MovePoints;
                        ConversionSpeedText.text = "Conversion Speed:" + THero.ConversionSpeed;
                        SightRangeText.text = "Sight Range:" + UC.SightRange;

                        StrenghtText.text = "Strenght:" + THero.Strenght;
                        StrenghtPerLvlText.text = "Strenght pre lvl:" + THero.BaseStrenght;
                        HeroAttackText.text = "Attack:" + THero.Attack;
                        HeroHealthText.text = "Max Health:" + THero.MaxHealth;
                        HeroHealthRegenText.text = "Health Regen:" + THero.HealthRegen;
                        DexterityText.text = "Dexterity:" + THero.Dexterity;
                        DexterityPerLvlText.text = "Dexterity per lvl:" + THero.BaseDexterity;
                        HeroMovePointsText.text = "Move Points:" + THero.MovePoints;
                        HeroDefenceText.text = "Defence:" + THero.Defence;
                        HeroConversionSpeedText.text = "Conversion Speed:" + THero.ConversionSpeed;
                        IntelliganceText.text = "Intelligance:" + THero.Intelligance;
                        IntelligancePerLvlText.text = "Intelligance per lvl:" + THero.BaseIntelligance;
                        HeroManaRegenText.text = "Mana Regen:" + THero.ManaRegen;
                        HeroMaxManaText.text = "Max Mana:" + THero.Mana;
                        HeroXpGainText.text = "Xp Gain:"; //need to set this up still
                        CharismaText.text = "Charisma:" + THero.Charisma;
                        CharismaPerLvlText.text = "Charisma per lvl:" + THero.BaseCharisma;
                        HeroUnitCostText.text = "Unit Cost:"; //need to set this up still
                        MoraleText.text = "Morale:"; //need to set this up still
                        ReturnLimitText.text = "Return Limit:"; //need to set this up still
                        InfoAndStatsPanel.SetActive(true);
                        InfoButtonClicked();
                        HeroStatButton.SetActive(true);
                    }
                }
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
                tempbutton.GetComponent<Image>().sprite = DBC.UnitDictionary[kvp.Key].IconSprite; //set sprite
                tempbutton.GetComponent<Button>().onClick.AddListener(BuyUnitController); //adds method to button clicked 
                tempbutton.AddComponent<ButtonProperties>();
                tempbutton.GetComponent<ButtonProperties>().ID = kvp.Key;
            }
        }

    }

    /// <summary>
    /// Creates unit, sets building var buildable to false, removes gold
    /// </summary>
    public void BuyUnitController()
    {
        if (EventSystem.current.currentSelectedGameObject != null) //make sure player clicked on the unit button and did not use keyboard shortcut
        {
            SelectedUnitDR = EventSystem.current.currentSelectedGameObject.GetComponent<ButtonProperties>().ID;
        }

        if (GCS.TeamList[GCS.CurrentTeamsTurn.Team].Gold >= DBC.UnitDictionary[SelectedUnitDR].Cost) //can they afford this unit?
        {
            GameObject GO = DBC.CreateAndSpawnUnit(CurrentlySelectedBuilding, DBC.UnitDictionary[SelectedUnitDR].ID, GCS.CurrentTeamsTurn.Team);
            GCS.BuildingPos[CurrentlySelectedBuilding].GetComponent<BuildingController>().Occupied = true;
            GCS.AddUnitsToDictionary(GO);
            GO.GetComponent<UnitController>().UnitMovable = true;
            GO.GetComponent<UnitController>().UnitMoved = false;
            GO.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, .5f);
            foreach (var unit in GCS.UnitPos)
            {
                unit.Value.GetComponent<UnitController>().GetTileValues();
                unit.Value.GetComponent<UnitController>().GetSightTiles();
            }
            BuildingPanel.SetActive(false);
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
    }  //potential lua method

    /// <summary>
    /// Sets requred panles to false and displayes winning message
    /// </summary>
    /// <param name="Team"></param>
    public void GameEndController(int Team)
    {
        List<string> templist = new List<string>();
        foreach (var item in GCS.UnitPos)
        {
            if (item.Value.GetComponent<UnitController>().Hero)
            {
                if (!templist.Contains(item.Value.GetComponent<UnitController>().HClass.Name))
                {
                    GCS.SaveHeroData(item.Value.GetComponent<UnitController>().HClass.Name);
                    templist.Add(item.Value.GetComponent<UnitController>().HClass.Name);
                }
            }
        }
        CurrentPlayerTurnImage.SetActive(false);
        EndTurnButton.SetActive(false);
        TooltipPanel.SetActive(false);
        ActionPanel.SetActive(false);
        BuildingPanel.SetActive(false);
        EndGamePanel.SetActive(true);
        SaveButton.SetActive(false);
        EndGameText.text = "Team: " + Team.ToString() + " has won the game.";

    } //potential lua method

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
        StartInfoPanel.SetActive(false);
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
                    BuyUnitController();
                }
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (ContentWindowBuilding.transform.childCount > 1)
                {
                    SelectedUnitDR = ContentWindowBuilding.transform.GetChild(1).GetComponent<ButtonProperties>().ID;
                    BuyUnitController();
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (ContentWindowBuilding.transform.childCount > 2)
                {
                    SelectedUnitDR = ContentWindowBuilding.transform.GetChild(2).GetComponent<ButtonProperties>().ID;
                    BuyUnitController();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (ContentWindowBuilding.transform.childCount > 3)
                {
                    SelectedUnitDR = ContentWindowBuilding.transform.GetChild(3).GetComponent<ButtonProperties>().ID;
                    BuyUnitController();
                }
            }
        }
    } //potential lua code

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

    public void CloseInfoStatPanel()
    {
        InfoAndStatsPanel.SetActive(false);
    }

    public void InfoButtonClicked()
    {
        InfoPanel.SetActive(true);
        StatsPanel.SetActive(false);
        HeroStatsPanel.SetActive(false);
    }

    public void StatButtonClicked()
    {
        InfoPanel.SetActive(false);
        StatsPanel.SetActive(true);
        HeroStatsPanel.SetActive(false);
    }

    public void HeroStatButtonClicked()
    {
        InfoPanel.SetActive(false);
        StatsPanel.SetActive(false);
        HeroStatsPanel.SetActive(true);
    }
}
