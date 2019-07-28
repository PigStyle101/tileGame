using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class MapEditMenueCamController : MonoBehaviour {

    
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
    private Text CurrentSelectedButtonText;
    [HideInInspector]
    public Text SaveFeedback;
    private Text LoadFeedback;
    private InputField SaveInputField;
    [HideInInspector]
    public string SelectedTab;
    [HideInInspector]
    public string SelectedButton;
    [HideInInspector]
    public int SelectedTeam;

    // this script is currently back up to date
    void Start ()
    {
        try
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
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }

	void Update ()
    {
        try
        {
            MoveScreenXandY();
            MoveScreenZ();
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
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
        CurrentSelectedButtonText = transform.Find("Canvas").Find("Panel").Find("MainPanel").GetComponentInChildren<Text>();
        var Temptext = transform.Find("Canvas").Find("Panel").Find("SaveGamePanel").GetComponentsInChildren<Text>();
        SaveFeedback = Temptext[1];
        LoadFeedback = transform.Find("Canvas").Find("Panel").Find("LoadGamePanel").GetComponentInChildren<Text>();
        SaveInputField = transform.Find("Canvas").Find("Panel").Find("SaveGamePanel").GetComponentInChildren<InputField>();
    }

    private void MoveScreenXandY()
    {
        try
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
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //controls camera movment y and x

    private void MoveScreenZ()
    {
        try
        {
            int z = new int();
            if (Input.GetAxis("Mouse ScrollWheel") > 0) { z = DatabaseController.instance.scrollSpeed; }
            if (Input.GetAxis("Mouse ScrollWheel") < 0) { z = -DatabaseController.instance.scrollSpeed; }

            transform.Translate(new Vector3(0, 0, z), Space.World);

            if (gameObject.transform.position.z > -1) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -1); }
            if (gameObject.transform.position.z < -GameControllerScript.instance.EditorMapSize * 2) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -GameControllerScript.instance.EditorMapSize * 2); }
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }//controls camera z movement

    void ChangeSelectedButton()
    {
        try
        {
            SelectedButton = EventSystem.current.currentSelectedGameObject.name;
            CurrentSelectedButtonText.text = "Currently Selected: " + EventSystem.current.currentSelectedGameObject.name;
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //changes to whatever button is clicked

    private void AddTerrainButtonsToContent()
    {
        try
        {
            Debug.Log("Adding terrain buttons to content window");
            foreach (KeyValuePair<int, Terrain> kvp in DatabaseController.instance.TerrainDictionary) //adds a button for each terrain in the database
            {
                GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowTerrain.transform); //create button and set its parent to content
                tempbutton.name = kvp.Value.Title; //change name
                tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
                tempbutton.GetComponent<Image>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.TerrainDictionary[kvp.Key].ArtworkDirectory[0]); //set sprite
                tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton); //adds method to button clicked
            }
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //populates the tile selection bar

    private void AddUnitButtonsToContent()
    {
        try
        {
            Debug.Log("Adding unit buttons to content window");
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
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //populates the tile selection bar

    private void AddBuildingButtonsToContent()
    {
        try
        {
            Debug.Log("Adding building buttons to content window");
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
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //populates the tile selection bar

    private void AddLoadButtonsToContent()
    {
        try
        {
            string[] files = Directory.GetFiles(Application.dataPath + "/StreamingAssets/Maps/", "*.json");
            foreach (string file in files)
            {
                //Debug.Log(Path.GetFileNameWithoutExtension(file));
                GameObject temploadbutton = Instantiate(LoadButtonPrefab, ContentWindowLoadButtons.transform);
                temploadbutton.name = Path.GetFileNameWithoutExtension(file);
                temploadbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
                temploadbutton.GetComponent<Button>().onClick.AddListener(LoadFileButtonClicked);
            }
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //searches saves file and adds a button for each save

    private void RemoveLoadButtonsFromContent()
    {
        try
        {
            var childcount = ContentWindowLoadButtons.transform.childCount;
            for (int i = 0; i < childcount; i++)
            {
                Destroy(ContentWindowLoadButtons.transform.GetChild(i).gameObject);
            }
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //this is needed so that ever time the AddLoadButtonsToContent is called the buttons will be refreshed

    public void TerrainButtonClicked()
    {
        try
        {
            ScrollWindowTerrain.SetActive(true);
            ScrollWindowBuilding.SetActive(false);
            ScrollWindowUnits.SetActive(false);
            SelectedTab = "Terrain";
            CurrentSelectedButtonText.text = "Currently Selected: " + DatabaseController.instance.TerrainDictionary[0].Title;
            SelectedButton = DatabaseController.instance.TerrainDictionary[0].Title;
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //sets the terrian content window as the active one

    public void BuildingButtonClicked()
    {
        try
        {
            ScrollWindowTerrain.SetActive(false);
            ScrollWindowBuilding.SetActive(true);
            ScrollWindowUnits.SetActive(false);
            SelectedTab = "Building";
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //sets the building content window as the active one

    public void UnitButtonClicked()
    {
        try
        {
            ScrollWindowTerrain.SetActive(false);
            ScrollWindowBuilding.SetActive(false);
            ScrollWindowUnits.SetActive(true);
            SelectedTab = "Unit";
            CurrentSelectedButtonText.text = "Currently Selected: " + DatabaseController.instance.UnitDictionary[0].Title;
            SelectedButton = DatabaseController.instance.UnitDictionary[0].Title;
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //sets the unit content window as the active one

    public void MainSaveButtonClicked()
    {
        try
        {
            SavePanel.SetActive(true);
            MainPanel.SetActive(false);
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //opens save panel

    public void MainLoadButtonClicked()
    {
        try
        {
            RemoveLoadButtonsFromContent();
            AddLoadButtonsToContent();
            MainPanel.SetActive(false);
            LoadPanel.SetActive(true);
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //opens load panel

    public void LoadPanelBackButtonClicked()
    {
        try
        {
            LoadPanel.SetActive(false);
            MainPanel.SetActive(true);
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //cloese load panel

    public void SavePanelBackButtonClicked()
    {
        try
        {
            SavePanel.SetActive(false);
            MainPanel.SetActive(true);
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //closes save panel

    public void SavePanelSaveButtonClicked()
    {
        try
        {
            GameControllerScript.instance.SaveMap(GameControllerScript.instance.TilePos, GameControllerScript.instance.UnitPos, GameControllerScript.instance.BuildingPos, SaveInputField.text);
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //activates save script in GameControllerScript

    public void LoadButtonClicked()
    {
        try
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
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }  //checks if any load buttons ahve been selected and then runs load script form GameControllerScript

    public void LoadFileButtonClicked()
    {
        try
        {
            CurrentlySelectedLoadFile = EventSystem.current.currentSelectedGameObject.name;
            CurrentlySelectedLoadGameObject = EventSystem.current.currentSelectedGameObject;
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    } //sets variable to the name of whatever button was clicked

    public void LoadPanelDeleteButtonClicked()
    {
        try
        {
            File.Delete(Application.dataPath + "/StreamingAssets/Maps/" + CurrentlySelectedLoadFile + ".json");
            Destroy(CurrentlySelectedLoadGameObject);
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }

    public void TeamButtonClicked()
    {
        try
        {
            SelectedTeam = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }

    public void MainMenuButtonClicked()
    {
        try
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
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }
}
