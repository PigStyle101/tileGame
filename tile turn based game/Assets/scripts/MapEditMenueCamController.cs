using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapEditMenueCamController : MonoBehaviour {

    public float dragSpeed;
    public int scrollSpeed;
    private Vector3 dragOrigin;
    private GameControllerScript GCS;
    public GameObject MapEditorTilesButtonPrefab;
    public GameObject ContentWindowTerrain;
    public GameObject ContentWindowUnits;
    public GameObject ContentWindowBuilding;
    public GameObject ScrollWindowTerrain;
    public GameObject ScrollWindowUnits;
    public GameObject ScrollWindowBuilding;
    private DatabaseController DBC;
    public string SelectedTab;
    public string SelectedButton;
    public Text CurrentSelectedButtonText;
    public GameObject SavePanel;
    public Text SaveFeedback;
    public InputField SaveInputField;

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
        SavePanel.SetActive(false);
        CurrentSelectedButtonText.text = "Currently Selected: " + DBC.TerrainDictionary[0].Title;
    }

	void Update ()
    {
        MoveScreenXandY();
        MoveScreenZ();
    }

    private void MoveScreenXandY()
    {

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

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

    void ChangeSelectedTerrainTobutton()
    {
        SelectedButton = EventSystem.current.currentSelectedGameObject.name;
        UnityEngine.Debug.Log("Selected tile changed too: " + SelectedButton);
        CurrentSelectedButtonText.text = "Currently Selected: " + EventSystem.current.currentSelectedGameObject.name;
    }

    void ChangeSelectedButtonUnit()
    {
        SelectedButton = EventSystem.current.currentSelectedGameObject.name;
        UnityEngine.Debug.Log("Selected tile changed too: " + SelectedButton);
        CurrentSelectedButtonText.text = "Currently Selected: " + EventSystem.current.currentSelectedGameObject.name;
    }

    private void AddTerrainButtonsToContent()
    {
        Debug.Log("Adding terrain buttons to content window");
        foreach (KeyValuePair<int, Terrain> kvp in DBC.TerrainDictionary) //adds a button for each terrain in the database
        {
            GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindowTerrain.transform); //create button and set its parent to content
            tempbutton.name = kvp.Value.Title; //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
            tempbutton.GetComponent<Image>().sprite = DBC.loadSprite(DBC.TerrainDictionary[kvp.Key].ArtworkDirectory[0]); //set sprite
            tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedTerrainTobutton); //adds method to button clicked
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
            tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedButtonUnit); //adds method to button clicked
        }
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
            tempbutton.GetComponent<Button>().onClick.AddListener(ChangeSelectedTerrainTobutton); //adds method to button clicked
        }
    } //populates the tile selection bar

    public void TerrainButtonClicked()
    {
        ScrollWindowTerrain.SetActive(true);
        ScrollWindowBuilding.SetActive(false);
        ScrollWindowUnits.SetActive(false);
        SelectedTab = "Terrain";
        CurrentSelectedButtonText.text = "Currently Selected: " + DBC.TerrainDictionary[0].Title;
        SelectedButton = DBC.TerrainDictionary[0].Title;
    }

    public void BuildingButtonClicked()
    {
        ScrollWindowTerrain.SetActive(false);
        ScrollWindowBuilding.SetActive(true);
        ScrollWindowUnits.SetActive(false);
        SelectedTab = "Building";
    }

    public void UnitButtonClicked()
    {
        ScrollWindowTerrain.SetActive(false);
        ScrollWindowBuilding.SetActive(false);
        ScrollWindowUnits.SetActive(true);
        SelectedTab = "Unit";
        CurrentSelectedButtonText.text = "Currently Selected: " + DBC.UnitDictionary[0].Title;
        SelectedButton = DBC.UnitDictionary[0].Title;
    }

    public void MainSaveButtonClicked()
    {
        SavePanel.SetActive(true);
    }

    public void SavePanelBackButtonClicked()
    {
        SavePanel.SetActive(false);
    }

    public void SavePanelSaveButtonClicked()
    {
        GCS.SaveMap(GCS.TilePos, GCS.UnitPos,SaveInputField.text);
    }
}
