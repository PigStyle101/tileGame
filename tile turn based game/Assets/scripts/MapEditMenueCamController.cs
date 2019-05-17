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
    private GameControllerScript GCS;
    private DatabaseController DBC;
    private string CurrentlySelectedLoadFile;
    private GameObject CurrentlySelectedLoadGameObject;
    public GameObject MapEditorTilesButtonPrefab;
    public GameObject ContentWindowTerrain;
    public GameObject ContentWindowUnits;
    public GameObject ContentWindowBuilding;
    public GameObject ScrollWindowTerrain;
    public GameObject ScrollWindowUnits;
    public GameObject ScrollWindowBuilding;
    public GameObject MainPanel;
    public GameObject LoadPanel;
    public GameObject SavePanel;
    public GameObject LoadButtonPrefab;
    public GameObject ContentWindowLoadButtons;
    public Text CurrentSelectedButtonText;
    public Text SaveFeedback;
    public Text LoadFeedback;
    public InputField SaveInputField;
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
            GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
            DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();
            AddTerrainButtonsToContent();
            AddBuildingButtonsToContent();
            AddUnitButtonsToContent();
            SelectedTab = "Terrain";
            SelectedButton = "Grass";
            SelectedTeam = 1;
            SaveFeedback.text = "Use only letters, cannot save with name that is already in use";
            SavePanel.SetActive(false);
            CurrentSelectedButtonText.text = "Currently Selected: " + DBC.TerrainDictionary[0].Title;
            LoadPanelBackButtonClicked();
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
            throw;
        }
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

            Vector3 move = new Vector3(pos.x * DBC.dragSpeedOffset * DBC.DragSpeed * -1, pos.y * DBC.dragSpeedOffset * DBC.DragSpeed * -1, 0);

            transform.Translate(move, Space.World);

            if (gameObject.transform.position.x > GCS.EditorMapSize) { gameObject.transform.position = new Vector3(GCS.EditorMapSize, transform.position.y, transform.position.z); }
            if (gameObject.transform.position.y > GCS.EditorMapSize) { gameObject.transform.position = new Vector3(transform.position.x, GCS.EditorMapSize, transform.position.z); }
            if (gameObject.transform.position.x < 0) { gameObject.transform.position = new Vector3(0, transform.position.y, transform.position.z); }
            if (gameObject.transform.position.y < 0) { gameObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z); }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    } //controls camera movment y and x

    private void MoveScreenZ()
    {
        try
        {
            int z = new int();
            if (Input.GetAxis("Mouse ScrollWheel") > 0) { z = DBC.scrollSpeed; }
            if (Input.GetAxis("Mouse ScrollWheel") < 0) { z = -DBC.scrollSpeed; }

            transform.Translate(new Vector3(0, 0, z), Space.World);

            if (gameObject.transform.position.z > -1) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -1); }
            if (gameObject.transform.position.z < -GCS.EditorMapSize * 2) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -GCS.EditorMapSize * 2); }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
            throw;
        }
    } //changes to whatever button is clicked

    private void AddTerrainButtonsToContent()
    {
        try
        {
            Debug.Log("Adding terrain buttons to content window");
            foreach (KeyValuePair<int, Terrain> kvp in DBC.TerrainDictionary) //adds a button for each terrain in the database
            {
                GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowTerrain.transform); //create button and set its parent to content
                tempbutton.name = kvp.Value.Title; //change name
                tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
                tempbutton.GetComponent<Image>().sprite = DBC.loadSprite(DBC.TerrainDictionary[kvp.Key].ArtworkDirectory[0]); //set sprite
                tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton); //adds method to button clicked
            }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    } //populates the tile selection bar

    private void AddUnitButtonsToContent()
    {
        try
        {
            Debug.Log("Adding unit buttons to content window");
            foreach (KeyValuePair<int, Unit> kvp in DBC.UnitDictionary) //adds a button for each terrain in the database
            {
                GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowUnits.transform); //create button and set its parent to content
                tempbutton.name = kvp.Value.Title; //change name
                tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
                tempbutton.GetComponent<Image>().sprite = DBC.loadSprite(DBC.UnitDictionary[kvp.Key].ArtworkDirectory[0]); //set sprite
                tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton); //adds method to button clicked
            }

            GameObject temppbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowUnits.transform);
            temppbutton.name = "Delete Unit";
            temppbutton.transform.GetChild(0).GetComponent<Text>().text = "Delete Unit";
            temppbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton);
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    } //populates the tile selection bar

    private void AddBuildingButtonsToContent()
    {
        try
        {
            Debug.Log("Adding building buttons to content window");
            foreach (KeyValuePair<int, Building> kvp in DBC.BuildingDictionary) //adds a button for each terrain in the database
            {
                GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowBuilding.transform); //create button and set its parent to content
                tempbutton.name = kvp.Value.Title; //change name
                tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
                tempbutton.GetComponent<Image>().sprite = DBC.loadSprite(DBC.BuildingDictionary[kvp.Key].ArtworkDirectory[0]); //set sprite
                tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton); //adds method to button clicked
            }

            GameObject temppbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowBuilding.transform);
            temppbutton.name = "Delete Building";
            temppbutton.transform.GetChild(0).GetComponent<Text>().text = "Delete Building";
            temppbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButton);
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
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
            CurrentSelectedButtonText.text = "Currently Selected: " + DBC.TerrainDictionary[0].Title;
            SelectedButton = DBC.TerrainDictionary[0].Title;
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
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
            CurrentSelectedButtonText.text = "Currently Selected: " + DBC.UnitDictionary[0].Title;
            SelectedButton = DBC.UnitDictionary[0].Title;
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
            throw;
        }
    } //closes save panel

    public void SavePanelSaveButtonClicked()
    {
        try
        {
            GCS.SaveMap(GCS.TilePos, GCS.UnitPos, GCS.BuildingPos, SaveInputField.text);
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    } //activates save script in GameControllerScript

    public void LoadButtonClicked()
    {
        try
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
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
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
            GCS.LogController(e.ToString());
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
            GCS.BuildingPos = new Dictionary<Vector2, GameObject>();
            GCS.TilePos = new Dictionary<Vector2, GameObject>();
            GCS.UnitPos = new Dictionary<Vector2, GameObject>();
            SceneManager.LoadScene("MainMenuScene");
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }
}
