using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapEditMenueCamController : MonoBehaviour
{


    private Vector3 dragOrigin;
    private string CurrentlySelectedLoadFile;
    private GameObject CurrentlySelectedLoadGameObject;
    public GameObject MapEditorTilesButtonPrefab;
    private GameObject ContentWindowTerrain;
    private GameObject ContentWindowUnits;
    private GameObject ContentWindowBuilding;
    private GameObject ScrollWindowTerrain;
    private GameObject ScrollWindowUnits;
    private GameObject ScrollWindowBuilding;
    private GameObject MainPanel;
    private GameObject LoadPanel;
    private GameObject SavePanel;
    public GameObject LoadButtonPrefab;
    private GameObject ContentWindowLoadButtons;
    [HideInInspector]
    public Text CurrentSelectedButtonText;
    [HideInInspector]
    public Text SaveFeedback;
    private Text LoadFeedback;
    private InputField SaveInputField;
    //[HideInInspector]
    public string SelectedTab;
    //[HideInInspector]
    public string SelectedButton;
    [HideInInspector]
    public int SelectedTeam;
    private Text TeamText;

    // this script is currently back up to date
    void Start()
    {
            GetObjectReferances();
            AddTerrainButtonsToContent();
            AddBuildingButtonsToContent();
            AddUnitButtonsToContent();
            SelectedTab = "Terrain";
            SelectedButton = "Grass";
            SelectedTeam = 1;
            SaveFeedback.text = "Use only letters, cannot save with name that is already in use";
            SavePanel.SetActive(false);
            CurrentSelectedButtonText.text = "Currently Selected: " + DatabaseController.instance.TerrainDictionary[0].Title;
            LoadPanelBackButtonClicked();
    }

    void Update()
    {
        MoveScreenXandY();
        MoveScreenZ();
        KeyBoardShortCuts();
    }

    private void GetObjectReferances()
    {
        ContentWindowTerrain = transform.Find("Canvas").Find("Panel").Find("MainPanel").Find("ScrollViewTerrain").Find("Viewport").Find("TerrainContent").gameObject;
        ContentWindowUnits = transform.Find("Canvas").Find("Panel").Find("MainPanel").Find("ScrollViewUnits").Find("Viewport").Find("UnitContent").gameObject;
        ContentWindowBuilding = transform.Find("Canvas").Find("Panel").Find("MainPanel").Find("ScrollViewBuildings").Find("Viewport").Find("BuildingContent").gameObject;
        ScrollWindowTerrain = transform.Find("Canvas").Find("Panel").Find("MainPanel").Find("ScrollViewTerrain").gameObject;
        ScrollWindowUnits = transform.Find("Canvas").Find("Panel").Find("MainPanel").Find("ScrollViewUnits").gameObject;
        ScrollWindowBuilding = transform.Find("Canvas").Find("Panel").Find("MainPanel").Find("ScrollViewBuildings").gameObject;
        MainPanel = transform.Find("Canvas").Find("Panel").Find("MainPanel").gameObject;
        SavePanel = transform.Find("Canvas").Find("Panel").Find("SaveGamePanel").gameObject;
        LoadPanel = transform.Find("Canvas").Find("Panel").Find("LoadGamePanel").gameObject;
        ContentWindowLoadButtons = transform.Find("Canvas").Find("Panel").Find("LoadGamePanel").Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        CurrentSelectedButtonText = transform.Find("Canvas").Find("Panel").Find("MainPanel").Find("CurrentlySelectedButtonText").GetComponent<Text>();
        TeamText = transform.Find("Canvas").Find("Panel").Find("MainPanel").Find("TeamText").GetComponent<Text>();
        SaveFeedback = transform.Find("Canvas").Find("Panel").Find("SaveGamePanel").Find("FeedbackText").GetComponent<Text>();
        LoadFeedback = transform.Find("Canvas").Find("Panel").Find("LoadGamePanel").Find("LoadPanelFeedbackText").GetComponent<Text>();
        SaveInputField = transform.Find("Canvas").Find("Panel").Find("SaveGamePanel").GetComponentInChildren<InputField>();
    }

    private void MoveScreenXandY()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

        Vector3 move = new Vector3(pos.x * DatabaseController.instance.dragSpeedOffset * DatabaseController.instance.DragSpeed * -1, pos.y * DatabaseController.instance.dragSpeedOffset * DatabaseController.instance.DragSpeed * -1, 0);

        transform.Translate(move, Space.World);

        if (gameObject.transform.position.x > GameControllerScript.instance.EditorMapSize) { gameObject.transform.position = new Vector3(GameControllerScript.instance.EditorMapSize, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y > GameControllerScript.instance.EditorMapSize) { gameObject.transform.position = new Vector3(transform.position.x, GameControllerScript.instance.EditorMapSize, transform.position.z); }
        if (gameObject.transform.position.x < 0) { gameObject.transform.position = new Vector3(0, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y < 0) { gameObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z); }

    } //controls camera movment y and x

    private void MoveScreenZ()
    {
        int z = new int();
        if (Input.GetAxis("Mouse ScrollWheel") > 0) { z = DatabaseController.instance.scrollSpeed; }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) { z = -DatabaseController.instance.scrollSpeed; }

        transform.Translate(new Vector3(0, 0, z), Space.World);

        if (gameObject.transform.position.z > -1) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -1); }
        if (gameObject.transform.position.z < -GameControllerScript.instance.EditorMapSize * 2) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -GameControllerScript.instance.EditorMapSize * 2); }

    }//controls camera z movement

    void ChangeSelectedButton()
    {
        SelectedButton = EventSystem.current.currentSelectedGameObject.name;
        CurrentSelectedButtonText.text = "Currently Selected: " + EventSystem.current.currentSelectedGameObject.name;
    } //changes to whatever button is clicked

    private void AddTerrainButtonsToContent()
    {
        //Debug.log("Adding terrain buttons to content window");
        foreach (KeyValuePair<int, Terrain> kvp in DatabaseController.instance.TerrainDictionary) //adds a button for each terrain in the database
        {
            GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowTerrain.transform); //create button and set its parent to content
            tempbutton.name = kvp.Value.Title; //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
            tempbutton.GetComponent<Image>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[kvp.Key].ArtworkDirectory[0]); //set sprite
            tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton); //adds method to button clicked
        }
    } //populates the tile selection bar

    private void AddUnitButtonsToContent()
    {
        //Debug.log("Adding unit buttons to content window");
        foreach (KeyValuePair<int, Unit> kvp in DatabaseController.instance.UnitDictionary) //adds a button for each terrain in the database
        {
            GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowUnits.transform); //create button and set its parent to content
            tempbutton.name = kvp.Value.Title; //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
            tempbutton.GetComponent<Image>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.UnitDictionary[kvp.Key].ArtworkDirectory[0]); //set sprite
            tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton); //adds method to button clicked
        }

        GameObject temppbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowUnits.transform);
        temppbutton.name = "Delete Unit";
        temppbutton.transform.GetChild(0).GetComponent<Text>().text = "Delete Unit";
        temppbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton);

    } //populates the tile selection bar

    private void AddBuildingButtonsToContent()
    {
        //Debug.log("Adding building buttons to content window");
        foreach (KeyValuePair<int, Building> kvp in DatabaseController.instance.BuildingDictionary) //adds a button for each terrain in the database
        {
            GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowBuilding.transform); //create button and set its parent to content
            tempbutton.name = kvp.Value.Title; //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
            tempbutton.GetComponent<Image>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.BuildingDictionary[kvp.Key].ArtworkDirectory[0]); //set sprite
            tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton); //adds method to button clicked
        }

        GameObject temppbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowBuilding.transform);
        temppbutton.name = "Delete Building";
        temppbutton.transform.GetChild(0).GetComponent<Text>().text = "Delete Building";
        temppbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton);

    } //populates the tile selection bar

    private void AddLoadButtonsToContent()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/StreamingAssets/Maps/", "*.json");
        foreach (string file in files)
        {
            ////Debug.log(Path.GetFileNameWithoutExtension(file));
            GameObject temploadbutton = Instantiate(LoadButtonPrefab, ContentWindowLoadButtons.transform);
            temploadbutton.name = Path.GetFileNameWithoutExtension(file);
            temploadbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
            temploadbutton.GetComponent<Button>().onClick.AddListener(LoadFileButtonClicked);
        }
    } //searches saves file and adds a button for each save

    private void RemoveLoadButtonsFromContent()
    {
        var childcount = ContentWindowLoadButtons.transform.childCount;
        for (int i = 0; i < childcount; i++)
        {
            Destroy(ContentWindowLoadButtons.transform.GetChild(i).gameObject);
        }
    } //this is needed so that ever time the AddLoadButtonsToContent is called the buttons will be refreshed

    public void TerrainButtonClicked()
    {
        ScrollWindowTerrain.SetActive(true);
        ScrollWindowBuilding.SetActive(false);
        ScrollWindowUnits.SetActive(false);
        SelectedTab = "Terrain";
        CurrentSelectedButtonText.text = "Currently Selected: " + DatabaseController.instance.TerrainDictionary[0].Title;
        SelectedButton = DatabaseController.instance.TerrainDictionary[0].Title;
    } //sets the terrian content window as the active one

    public void BuildingButtonClicked()
    {
        ScrollWindowTerrain.SetActive(false);
        ScrollWindowBuilding.SetActive(true);
        ScrollWindowUnits.SetActive(false);
        SelectedTab = "Building";
    } //sets the building content window as the active one

    public void UnitButtonClicked()
    {
        ScrollWindowTerrain.SetActive(false);
        ScrollWindowBuilding.SetActive(false);
        ScrollWindowUnits.SetActive(true);
        SelectedTab = "Unit";
        CurrentSelectedButtonText.text = "Currently Selected: " + DatabaseController.instance.UnitDictionary[0].Title;
        SelectedButton = DatabaseController.instance.UnitDictionary[0].Title;
    } //sets the unit content window as the active one

    public void MainSaveButtonClicked()
    {
        SavePanel.SetActive(true);
        MainPanel.SetActive(false);
    } //opens save panel

    public void MainLoadButtonClicked()
    {
        RemoveLoadButtonsFromContent();
        AddLoadButtonsToContent();
        MainPanel.SetActive(false);
        LoadPanel.SetActive(true);
    } //opens load panel

    public void LoadPanelBackButtonClicked()
    {
        LoadPanel.SetActive(false);
        MainPanel.SetActive(true);
    } //cloese load panel

    public void SavePanelBackButtonClicked()
    {
        SavePanel.SetActive(false);
        MainPanel.SetActive(true);
    } //closes save panel

    public void SavePanelSaveButtonClicked()
    {
        GameControllerScript.instance.SaveMap(GameControllerScript.instance.TilePos, GameControllerScript.instance.UnitPos, GameControllerScript.instance.BuildingPos, SaveInputField.text);

    } //activates save script in GameControllerScript

    public void LoadButtonClicked()
    {
        if (CurrentlySelectedLoadFile != null)
        {
            GameControllerScript.instance.LoadMapMapEditor(CurrentlySelectedLoadFile);
            LoadFeedback.text = "Loaded " + CurrentlySelectedLoadFile;
        }
        else
        {
            LoadFeedback.text = "Please select map to load";
        }
    }  //checks if any load buttons ahve been selected and then runs load script form GameControllerScript

    public void LoadFileButtonClicked()
    {
        CurrentlySelectedLoadFile = EventSystem.current.currentSelectedGameObject.name;
        CurrentlySelectedLoadGameObject = EventSystem.current.currentSelectedGameObject;

    } //sets variable to the name of whatever button was clicked

    public void LoadPanelDeleteButtonClicked()
    {
        File.Delete(Application.dataPath + "/StreamingAssets/Maps/" + CurrentlySelectedLoadFile + ".json");
        Destroy(CurrentlySelectedLoadGameObject);
    }

    public void NextTeamButtonClicked()
    {
        if (SelectedTeam < DatabaseController.instance.UnitDictionary[0].ArtworkDirectory.Count - 1)
        {
            SelectedTeam = SelectedTeam + 1;
            TeamText.text = "Team:" + SelectedTeam;
        }
        else
        {
            SelectedTeam = 0;
            TeamText.text = "Team:" + SelectedTeam;
        }
    }

    public void PreviousTeamButtonClicked()
    {
        if (SelectedTeam > 0)
        {
            SelectedTeam = SelectedTeam - 1;
            TeamText.text = "Team:" + SelectedTeam;
        }
        else
        {
            SelectedTeam = DatabaseController.instance.UnitDictionary[0].ArtworkDirectory.Count - 1;
            TeamText.text = "Team:" + SelectedTeam;
        }
    }

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
        GameControllerScript.instance.BuildingPos = new Dictionary<Vector2, GameObject>();
        GameControllerScript.instance.TilePos = new Dictionary<Vector2, GameObject>();
        GameControllerScript.instance.UnitPos = new Dictionary<Vector2, GameObject>();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void KeyBoardShortCuts()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectedTeam = 1;
            TeamText.text = "Team:" + SelectedTeam;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectedTeam = 2;
            TeamText.text = "Team:" + SelectedTeam;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectedTeam = 3;
            TeamText.text = "Team:" + SelectedTeam;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectedTeam = 4;
            TeamText.text = "Team:" + SelectedTeam;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectedTeam = 5;
            TeamText.text = "Team:" + SelectedTeam;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectedTeam = 6;
            TeamText.text = "Team:" + SelectedTeam;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectedTeam = 7;
            TeamText.text = "Team:" + SelectedTeam;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SelectedTeam = 8;
            TeamText.text = "Team:" + SelectedTeam;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SelectedTeam = 9;
            TeamText.text = "Team:" + SelectedTeam;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SelectedTeam = 0;
            TeamText.text = "Team:" + SelectedTeam;
        }
    }
}
