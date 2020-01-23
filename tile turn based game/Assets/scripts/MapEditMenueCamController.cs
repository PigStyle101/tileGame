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
    private GameObject ChangeAllButton;
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
    public int SelectedButtonDR; //Dictionary Referance
    [HideInInspector]
    public int SelectedTeam;
    private Text TeamText;
    private DatabaseController DBC;
    private GameControllerScript GCS;

    private void Awake()
    {
        DBC = DatabaseController.instance;
        GCS = GameControllerScript.instance;
    }
    // this script is currently back up to date
    void Start()
    {
        GetObjectReferances();
        AddTerrainButtonsToContent();
        AddBuildingButtonsToContent();
        AddUnitButtonsToContent();
        SelectedTab = DBC.TerrainDictionary[0].Type;
        SelectedButtonDR = 0;
        SelectedTeam = 1;
        SaveFeedback.text = "Use only letters, cannot save with name that is already in use";
        SavePanel.SetActive(false);
        CurrentSelectedButtonText.text = "Currently Selected: " + DBC.TerrainDictionary[0].Title;
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
        ChangeAllButton = transform.Find("Canvas").Find("Panel").Find("MainPanel").Find("ChangeAllButton").gameObject;
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

        Vector3 move = new Vector3(pos.x * DBC.dragSpeedOffset * DBC.DragSpeed * -1, pos.y * DBC.dragSpeedOffset * DBC.DragSpeed * -1, 0);

        transform.Translate(move, Space.World);

        if (gameObject.transform.position.x > GCS.EditorMapSize) { gameObject.transform.position = new Vector3(GCS.EditorMapSize, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y > GCS.EditorMapSize) { gameObject.transform.position = new Vector3(transform.position.x, GCS.EditorMapSize, transform.position.z); }
        if (gameObject.transform.position.x < 0) { gameObject.transform.position = new Vector3(0, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y < 0) { gameObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z); }

    } //controls camera movment y and x

    private void MoveScreenZ()
    {
        int z = new int();
        if (Input.GetAxis("Mouse ScrollWheel") > 0) { z = DBC.scrollSpeed; }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) { z = -DBC.scrollSpeed; }

        transform.Translate(new Vector3(0, 0, z), Space.World);

        if (gameObject.transform.position.z > -1) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -1); }
        if (gameObject.transform.position.z < -GCS.EditorMapSize * 2) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -GCS.EditorMapSize * 2); }

    }//controls camera z movement

    void ChangeSelectedButton()
    {
        SelectedButtonDR = EventSystem.current.currentSelectedGameObject.GetComponent<ButtonProperties>().ID;
        CurrentSelectedButtonText.text = "Currently Selected: " + EventSystem.current.currentSelectedGameObject.name;
    } //changes to whatever button is clicked

    private void AddTerrainButtonsToContent()
    {
        //Debug.log("Adding terrain buttons to content window");
        foreach (KeyValuePair<int, Terrain> kvp in DBC.TerrainDictionary) //adds a button for each terrain in the database
        {
            GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowTerrain.transform); //create button and set its parent to content
            tempbutton.name = kvp.Value.Title; //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
            tempbutton.GetComponent<Image>().sprite = DBC.TerrainDictionary[kvp.Key].ArtworkDirectory[0]; //set sprite
            tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton); //adds method to button clicked
            tempbutton.AddComponent<ButtonProperties>();
            tempbutton.GetComponent<ButtonProperties>().ID = kvp.Key;
        }
    } //populates the tile selection bar

    private void AddUnitButtonsToContent()
    {
        //Debug.log("Adding unit buttons to content window");
        foreach (KeyValuePair<int, Unit> kvp in DBC.UnitDictionary) //adds a button for each terrain in the database
        {
            GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowUnits.transform); //create button and set its parent to content
            tempbutton.name = kvp.Value.Title; //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
            tempbutton.GetComponent<Image>().sprite = DBC.UnitDictionary[kvp.Key].ArtworkDirectory[0]; //set sprite
            tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton); //adds method to button clicked
            tempbutton.AddComponent<ButtonProperties>();
            tempbutton.GetComponent<ButtonProperties>().ID = kvp.Key;
        }

        GameObject temppbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowUnits.transform);
        temppbutton.name = "Delete Unit";
        temppbutton.transform.GetChild(0).GetComponent<Text>().text = "Delete Unit";
        temppbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton);
        temppbutton.AddComponent<ButtonProperties>();
        temppbutton.GetComponent<ButtonProperties>().ID = -1;

    } //populates the tile selection bar

    private void AddBuildingButtonsToContent()
    {
        //Debug.log("Adding building buttons to content window");
        foreach (KeyValuePair<int, Building> kvp in DBC.BuildingDictionary) //adds a button for each terrain in the database
        {
            GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowBuilding.transform); //create button and set its parent to content
            tempbutton.name = kvp.Value.Title; //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
            tempbutton.GetComponent<Image>().sprite = DBC.BuildingDictionary[kvp.Key].ArtworkDirectory[0]; //set sprite
            tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton); //adds method to button clicked
            tempbutton.AddComponent<ButtonProperties>();
            tempbutton.GetComponent<ButtonProperties>().ID = kvp.Key;
        }

        GameObject temppbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowBuilding.transform);
        temppbutton.name = "Delete Building";
        temppbutton.transform.GetChild(0).GetComponent<Text>().text = "Delete Building";
        temppbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton);
        temppbutton.AddComponent<ButtonProperties>();
        temppbutton.GetComponent<ButtonProperties>().ID = -1;

    } //populates the tile selection bar

    private void AddLoadButtonsToContent()
    {
        var childcount = ContentWindowLoadButtons.transform.childCount;
        for (int i = 0; i < childcount; i++)
        {
            Destroy(ContentWindowLoadButtons.transform.GetChild(i).gameObject);
        }
        string[] files = Directory.GetFiles(Application.dataPath + "/StreamingAssets/Maps/", "*.json");
        foreach (string file in files)
        {
            var Tempstring = File.ReadAllText(file); //temp string to hold the json data
            Save tempjson = JsonUtility.FromJson<Save>(Tempstring); //this converts from json string to unity object
            bool SameMods;
            if (DBC.ModsLoaded.Count == tempjson.Mods.Count)
            {
                SameMods = true;
                foreach (var item in DBC.ModsLoaded)
                {
                    if (tempjson.Mods.Contains(item) && SameMods)
                    {
                        SameMods = true;
                    }
                    else
                    {
                        SameMods = false;
                    }
                }
            }
            else
            {
                SameMods = false;
            }
            if (SameMods)
            {
                ////Debug.log(Path.GetFileNameWithoutExtension(file));
                GameObject temploadbutton = Instantiate(LoadButtonPrefab, ContentWindowLoadButtons.transform);
                temploadbutton.name = Path.GetFileNameWithoutExtension(file);
                temploadbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
                temploadbutton.GetComponent<Button>().onClick.AddListener(LoadFileButtonClicked); 
            }
        }
    } //searches saves file and adds a button for each save

    public void TerrainButtonClicked()
    {
        ScrollWindowTerrain.SetActive(true);
        ScrollWindowBuilding.SetActive(false);
        ScrollWindowUnits.SetActive(false);
        ChangeAllButton.SetActive(true);
        SelectedTab = "Terrain";
        CurrentSelectedButtonText.text = "Currently Selected: " + DBC.TerrainDictionary[0].Title;
        SelectedButtonDR = 0;
    } //sets the terrian content window as the active one

    public void BuildingButtonClicked()
    {
        ScrollWindowTerrain.SetActive(false);
        ScrollWindowBuilding.SetActive(true);
        ScrollWindowUnits.SetActive(false);
        ChangeAllButton.SetActive(false);
        SelectedTab = "Building";
        CurrentSelectedButtonText.text = "Currently Selected: " + DBC.BuildingDictionary[0].Title;
        SelectedButtonDR = 0;
    } //sets the building content window as the active one

    public void UnitButtonClicked()
    {
        ScrollWindowTerrain.SetActive(false);
        ScrollWindowBuilding.SetActive(false);
        ScrollWindowUnits.SetActive(true);
        ChangeAllButton.SetActive(false);
        SelectedTab = "Unit";
        CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowUnits.transform.GetChild(0).name;
        SelectedButtonDR = ContentWindowUnits.transform.GetChild(0).GetComponent<ButtonProperties>().ID;
    } //sets the unit content window as the active one

    public void MainSaveButtonClicked()
    {
        SavePanel.SetActive(true);
        MainPanel.SetActive(false);
    } //opens save panel

    public void MainLoadButtonClicked()
    {
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
        GCS.SaveMap(GCS.TilePos, GCS.UnitPos, GCS.BuildingPos, SaveInputField.text);

    } //activates save script in GameControllerScript

    public void LoadButtonClicked()
    {
        if (CurrentlySelectedLoadFile != null)
        {
            GCS.LoadMapMapEditor(CurrentlySelectedLoadFile);
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
        if (SelectedTeam < DBC.UnitDictionary[0].ArtworkDirectory.Count - 1)
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
            SelectedTeam = DBC.UnitDictionary[0].ArtworkDirectory.Count - 1;
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
        GCS.BuildingPos = new Dictionary<Vector2, GameObject>();
        GCS.TilePos = new Dictionary<Vector2, GameObject>();
        GCS.UnitPos = new Dictionary<Vector2, GameObject>();
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
        if (ScrollWindowTerrain.activeSelf)
        {
            if (ContentWindowTerrain.transform.childCount > 0 && Input.GetKeyDown(KeyCode.Q))
            {
                SelectedButtonDR = ContentWindowTerrain.transform.GetChild(0).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowTerrain.transform.GetChild(0).name;
            }
            if (ContentWindowTerrain.transform.childCount > 1 && Input.GetKeyDown(KeyCode.W))
            {
                SelectedButtonDR = ContentWindowTerrain.transform.GetChild(1).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowTerrain.transform.GetChild(1).name;
            }
            if (ContentWindowTerrain.transform.childCount > 2 && Input.GetKeyDown(KeyCode.E))
            {
                SelectedButtonDR = ContentWindowTerrain.transform.GetChild(2).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowTerrain.transform.GetChild(2).name;
            }
            if (ContentWindowTerrain.transform.childCount > 3 && Input.GetKeyDown(KeyCode.R))
            {
                SelectedButtonDR = ContentWindowTerrain.transform.GetChild(3).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowTerrain.transform.GetChild(3).name;
            }
            if (ContentWindowTerrain.transform.childCount > 4 && Input.GetKeyDown(KeyCode.T))
            {
                SelectedButtonDR = ContentWindowTerrain.transform.GetChild(4).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowTerrain.transform.GetChild(4).name;
            }
            if (ContentWindowTerrain.transform.childCount > 5 && Input.GetKeyDown(KeyCode.Y))
            {
                SelectedButtonDR = ContentWindowTerrain.transform.GetChild(5).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowTerrain.transform.GetChild(5).name;
            }
            if (ContentWindowTerrain.transform.childCount > 6 && Input.GetKeyDown(KeyCode.U))
            {
                SelectedButtonDR = ContentWindowTerrain.transform.GetChild(6).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowTerrain.transform.GetChild(6).name;
            }
            if (ContentWindowTerrain.transform.childCount > 7 && Input.GetKeyDown(KeyCode.I))
            {
                SelectedButtonDR = ContentWindowTerrain.transform.GetChild(7).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowTerrain.transform.GetChild(7).name;
            }
            if (ContentWindowTerrain.transform.childCount > 8 && Input.GetKeyDown(KeyCode.O))
            {
                SelectedButtonDR = ContentWindowTerrain.transform.GetChild(8).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowTerrain.transform.GetChild(8).name;
            }
            if (ContentWindowTerrain.transform.childCount > 9 && Input.GetKeyDown(KeyCode.P))
            {
                SelectedButtonDR = ContentWindowTerrain.transform.GetChild(9).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowTerrain.transform.GetChild(9).name;
            }
        }
        if (ScrollWindowUnits.activeSelf)
        {
            if (ContentWindowUnits.transform.childCount > 0 && Input.GetKeyDown(KeyCode.Q))
            {
                SelectedButtonDR = ContentWindowUnits.transform.GetChild(0).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowUnits.transform.GetChild(0).name;
            }
            if (ContentWindowUnits.transform.childCount > 1 && Input.GetKeyDown(KeyCode.W))
            {
                SelectedButtonDR = ContentWindowUnits.transform.GetChild(1).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowUnits.transform.GetChild(1).name;
            }
            if (ContentWindowUnits.transform.childCount > 2 && Input.GetKeyDown(KeyCode.E))
            {
                SelectedButtonDR = ContentWindowUnits.transform.GetChild(2).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowUnits.transform.GetChild(2).name;
            }
            if (ContentWindowUnits.transform.childCount > 3 && Input.GetKeyDown(KeyCode.R))
            {
                SelectedButtonDR = ContentWindowUnits.transform.GetChild(3).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowUnits.transform.GetChild(3).name;
            }
            if (ContentWindowUnits.transform.childCount > 4 && Input.GetKeyDown(KeyCode.T))
            {
                SelectedButtonDR = ContentWindowUnits.transform.GetChild(4).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowUnits.transform.GetChild(4).name;
            }
            if (ContentWindowUnits.transform.childCount > 5 && Input.GetKeyDown(KeyCode.Y))
            {
                SelectedButtonDR = ContentWindowUnits.transform.GetChild(5).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowUnits.transform.GetChild(5).name;
            }
            if (ContentWindowUnits.transform.childCount > 6 && Input.GetKeyDown(KeyCode.U))
            {
                SelectedButtonDR = ContentWindowUnits.transform.GetChild(6).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowUnits.transform.GetChild(6).name;
            }
            if (ContentWindowUnits.transform.childCount > 7 && Input.GetKeyDown(KeyCode.I))
            {
                SelectedButtonDR = ContentWindowUnits.transform.GetChild(7).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowUnits.transform.GetChild(7).name;
            }
            if (ContentWindowUnits.transform.childCount > 8 && Input.GetKeyDown(KeyCode.O))
            {
                SelectedButtonDR = ContentWindowUnits.transform.GetChild(8).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowUnits.transform.GetChild(8).name;
            }
            if (ContentWindowUnits.transform.childCount > 9 && Input.GetKeyDown(KeyCode.P))
            {
                SelectedButtonDR = ContentWindowUnits.transform.GetChild(9).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowUnits.transform.GetChild(9).name;
            }
        }
        if (ScrollWindowBuilding.activeSelf)
        {
            if (ContentWindowBuilding.transform.childCount > 0 && Input.GetKeyDown(KeyCode.Q))
            {
                SelectedButtonDR = ContentWindowBuilding.transform.GetChild(0).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowBuilding.transform.GetChild(0).name;
            }
            if (ContentWindowBuilding.transform.childCount > 1 && Input.GetKeyDown(KeyCode.W))
            {
                SelectedButtonDR = ContentWindowBuilding.transform.GetChild(1).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowBuilding.transform.GetChild(1).name;
            }
            if (ContentWindowBuilding.transform.childCount > 2 && Input.GetKeyDown(KeyCode.E))
            {
                SelectedButtonDR = ContentWindowBuilding.transform.GetChild(2).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowBuilding.transform.GetChild(2).name;
            }
            if (ContentWindowBuilding.transform.childCount > 3 && Input.GetKeyDown(KeyCode.R))
            {
                SelectedButtonDR = ContentWindowBuilding.transform.GetChild(3).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowBuilding.transform.GetChild(3).name;
            }
            if (ContentWindowBuilding.transform.childCount > 4 && Input.GetKeyDown(KeyCode.T))
            {
                SelectedButtonDR = ContentWindowBuilding.transform.GetChild(4).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowBuilding.transform.GetChild(4).name;
            }
            if (ContentWindowBuilding.transform.childCount > 5 && Input.GetKeyDown(KeyCode.Y))
            {
                SelectedButtonDR = ContentWindowBuilding.transform.GetChild(5).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowBuilding.transform.GetChild(5).name;
            }
            if (ContentWindowBuilding.transform.childCount > 6 && Input.GetKeyDown(KeyCode.U))
            {
                SelectedButtonDR = ContentWindowBuilding.transform.GetChild(6).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowBuilding.transform.GetChild(6).name;
            }
            if (ContentWindowBuilding.transform.childCount > 7 && Input.GetKeyDown(KeyCode.I))
            {
                SelectedButtonDR = ContentWindowBuilding.transform.GetChild(7).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowBuilding.transform.GetChild(7).name;
            }
            if (ContentWindowBuilding.transform.childCount > 8 && Input.GetKeyDown(KeyCode.O))
            {
                SelectedButtonDR = ContentWindowBuilding.transform.GetChild(8).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowBuilding.transform.GetChild(8).name;
            }
            if (ContentWindowBuilding.transform.childCount > 9 && Input.GetKeyDown(KeyCode.P))
            {
                SelectedButtonDR = ContentWindowBuilding.transform.GetChild(9).GetComponent<ButtonProperties>().ID;
                CurrentSelectedButtonText.text = "Currently Selected: " + ContentWindowBuilding.transform.GetChild(9).name;
            }
        }
    }

    public void ChangeAllButtonClicked()
    {
        foreach (var kvp in GCS.TilePos)
        {
            if (kvp.Value.GetComponent<TerrainController>().ID != SelectedButtonDR)
            {
                kvp.Value.GetComponent<TerrainController>().ChangeTile();
            }
        }
        GCS.SpriteUpdateActivator();
    }
}
