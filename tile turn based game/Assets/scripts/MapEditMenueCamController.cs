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
    public GameObject ContentWindow;
    private DatabaseController DBC;
    

    // this script is currently back up to date
    void Start ()
    {
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();
        AddButtonsToContent();
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

    void ChangeTileSelectedToButtonTile()
    {
        if (GCS.SelectedTile != null)
        {
            foreach (KeyValuePair<int, Terrain> kvp in DBC.TerrainDictionary)
            {
                if (EventSystem.current.currentSelectedGameObject.name == kvp.Value.Title) //checks through dictionary for matching tile to button name
                {
                    Debug.Log("Changing tile to " + kvp.Value.Title);
                    GCS.SelectedTile.name = kvp.Value.Title;//change name of tile
                    GCS.SelectedTile.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.TerrainDictionary[kvp.Key].ArtworkDirectory[0]); //change sprite of tile
                }
            }
        }
        
    } //changes tile name and sprite to new tile

    private void AddButtonsToContent()
    {
        Debug.Log("Adding buttons to content window");
        foreach (KeyValuePair<int, Terrain> kvp in DBC.TerrainDictionary) //adds a button for each terrain in the database
        {
            GameObject tempbutton = Instantiate(MapEditorTilesButtonPrefab, ContentWindow.transform); //create button and set its parent to content
            tempbutton.name = kvp.Value.Title; //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
            tempbutton.GetComponent<Image>().sprite = DBC.loadSprite(DBC.TerrainDictionary[kvp.Key].ArtworkDirectory[0]); //set sprite
            tempbutton.GetComponent<Button>().onClick.AddListener(ChangeTileSelectedToButtonTile); //adds method to button clicked

        }
    } //populates the tile selection bar

    
}
