using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class MapEditMenueCamController : MonoBehaviour {

    public float dragSpeed;
    public int scrollSpeed;
    private Vector3 dragOrigin;
    private GameControllerScript GCS;
    private DatabaseController DBC;
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
    public string SelectedTab;
    public string SelectedButton;
    public int SelectedTeam;
    public Text CurrentSelectedButtonText;
    public Text SaveFeedback;
    public Text LoadFeedback;
    public InputField SaveInputField;
    private string CurrentlySelectedLoadFile;
    private GameObject CurrentlySelectedLoadGameObject;

    // this script is currently back up to date
    void Start ()
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

	void Update ()
    {
        MoveScreenXandY();
        MoveScreenZ();
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

        Vector3 move = new Vector3(pos.x * dragSpeed * -1, pos.y * dragSpeed * -1, 0);

        transform.Translate(move, Space.World);

        if (gameObject.transform.position.x > GCS.mapSize) { gameObject.transform.position = new Vector3(GCS.mapSize, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y > GCS.mapSize) { gameObject.transform.position = new Vector3(transform.position.x, GCS.mapSize, transform.position.z); }
        if (gameObject.transform.position.x < 0) { gameObject.transform.position = new Vector3(0, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y < 0) { gameObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z); }
    } //controls camera movment y and x

    private void MoveScreenZ()
    {
        int z = new int();
        if (Input.GetAxis("Mouse ScrollWheel") > 0) { z = scrollSpeed; }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) { z = -scrollSpeed; }
        
        transform.Translate(new Vector3(0, 0, z), Space.World);

        if (gameObject.transform.position.z > -1) { gameObject.transform.position = new Vector3(transform.position.x,transform.position.y, -1); }
        if (gameObject.transform.position.z < -GCS.mapSize * 2) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -GCS.mapSize * 2); }
    }//controls camera z movement

    void ChangeSelectedButton()
    {
        SelectedButton = EventSystem.current.currentSelectedGameObject.name;
        //UnityEngine.Debug.Log("Selected tile changed too: " + SelectedButton);
        CurrentSelectedButtonText.text = "Currently Selected: " + EventSystem.current.currentSelectedGameObject.name;
    } //changes to whatever button is clicked

    private void AddTerrainButtonsToContent()
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
    } //populates the tile selection bar

    private void AddUnitButtonsToContent()
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
    } //populates the tile selection bar

    private void AddBuildingButtonsToContent()
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
    } //populates the tile selection bar

    private void AddLoadButtonsToContent()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/StreamingAssets/Saves/", "*.json");
        foreach(string file in files)
        {
            //Debug.Log(Path.GetFileNameWithoutExtension(file));
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
        CurrentSelectedButtonText.text = "Currently Selected: " + DBC.TerrainDictionary[0].Title;
        SelectedButton = DBC.TerrainDictionary[0].Title;
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
        CurrentSelectedButtonText.text = "Currently Selected: " + DBC.UnitDictionary[0].Title;
        SelectedButton = DBC.UnitDictionary[0].Title;
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
        GCS.SaveMap(GCS.TilePos, GCS.UnitPos,GCS.BuildingPos,SaveInputField.text);
    } //activates save script in GameControllerScript

    public void LoadButtonClicked()
    {
        if (CurrentlySelectedLoadFile != null)
        {
            GCS.LoadMap(CurrentlySelectedLoadFile);
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
        File.Delete(Application.dataPath + "/StreamingAssets/Saves/" + CurrentlySelectedLoadFile + ".json");
        Destroy(CurrentlySelectedLoadGameObject);
    }

    public void TeamButtonClicked()
    {
        SelectedTeam = int.Parse(EventSystem.current.currentSelectedGameObject.name);
    }
}
