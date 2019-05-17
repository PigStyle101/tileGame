using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

public class PlaySceneCamController : MonoBehaviour
{
    Vector3 dragOrigin;
    private GameControllerScript GCS;
    private DatabaseController DBC;
    [HideInInspector]
    public Text CurrentPlayerTurnText;
    public bool AttackButtonSelected = false;
    public GameObject AttackButton;
    public GameObject CancelButton;
    public GameObject WaitButton;
    public GameObject TerrainImage;
    public GameObject TerrainText;
    public GameObject TerrainDescription;
    public GameObject TerrainToolTipSet;
    public GameObject TerrainToolTipData;
    public GameObject UnitImage;
    public GameObject UnitText;
    public GameObject UnitDescription;
    public GameObject UnitToolTipSet;
    public GameObject UnitToolTipData;
    public GameObject BuildingImage;
    public GameObject BuildingText;
    public GameObject BuildingDescription;
    public GameObject BuildingToolTipSet;
    public GameObject BuildingToolTipData;
    public GameObject BuildingButtonPrefab;
    public GameObject ContentWindowBuilding;
    public GameObject CurrentPlayerTurnImage;
    public GameObject EndTurnButton;
    public GameObject TooltipPanel;
    public GameObject ActionPanel;
    public GameObject BuildingPanel;
    public GameObject EndGamePanel;
    public Text EndGameText;
    [HideInInspector]
    public Vector2 CurrentlySelectedBuilding;
    [HideInInspector]
    public bool BuildingRayBool;

    private void Awake()
    {
        try
        {
            GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
            DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    private void Start()
    {
        try
        {
            SetActionButtonsToFalse();
            BuildingPanel.SetActive(false);
            AddUnitButtonsToBuildContent();
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    void Update()
    {
        try
        {
            MoveScreenXandY();
            MoveScreenZ();
            RayCasterForPlayScene();
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

            if (gameObject.transform.position.x > GCS.PlayMapSize) { gameObject.transform.position = new Vector3(GCS.PlayMapSize, transform.position.y, transform.position.z); }
            if (gameObject.transform.position.y > GCS.PlayMapSize) { gameObject.transform.position = new Vector3(transform.position.x, GCS.PlayMapSize, transform.position.z); }
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
            if (gameObject.transform.position.z < -GCS.PlayMapSize * 2) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -GCS.PlayMapSize * 2); }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }//controls camera z movement

    public void EndTurnButtonClicked()
    {
        try
        {
            GCS.PlaySceneTurnChanger();
            BuildingPanel.SetActive(false);
            if (GCS.SelectedUnitPlayScene != null)
            {
                GCS.WaitActionPlayScene();
            }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void AttackButtonController(int enemyCount)
    {
        try
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
            CancelButton.SetActive(true);
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void WaitButtonClicked()
    {
        try
        {
            GCS.WaitActionPlayScene();
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void AttackButtonClicked()
    {
        try
        {
            GCS.AttackActionPlayScene();
            AttackButtonSelected = true;
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void CancelButtonClicked()
    {
        try
        {
            GCS.CancelActionPlayScene();
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void SetActionButtonsToFalse()
    {
        try
        {
            AttackButton.SetActive(false);
            WaitButton.SetActive(false);
            CancelButton.SetActive(false);
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void RayCasterForPlayScene()
    {
        try
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject()) //dont want to click through menus
                {
                    //Debug.Log("RayHitStarted");
                    Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); //GET THEM RAYS
                    RaycastHit[] hits;
                    hits = Physics.RaycastAll(ray);
                    BuildingRayBool = false;
                    //Debug.Log("Starting play scene ray hits");
                    for (int i = 0; i < hits.Length; i++) // GO THROUGH THEM RAYS
                    {
                        RaycastHit hit = hits[i];
                        if (hit.transform.tag == DBC.TerrainDictionary[0].Type)
                        {
                            foreach (var kvp in DBC.TerrainDictionary)
                            {
                                if (kvp.Value.Title == hit.transform.name)
                                {
                                    TerrainImage.GetComponent<Image>().sprite = DBC.loadSprite(DBC.TerrainDictionary[kvp.Key].ArtworkDirectory[0]);
                                    TerrainText.GetComponent<Text>().text = kvp.Value.Title;
                                    TerrainDescription.GetComponent<Text>().text = kvp.Value.Description;
                                    TerrainToolTipData.GetComponent<Text>().text = kvp.Value.DefenceBonus.ToString() + Environment.NewLine + kvp.Value.Walkable.ToString() + Environment.NewLine + kvp.Value.Weight.ToString();
                                }
                            }
                        }
                        if (hit.transform.tag == DBC.UnitDictionary[0].Type)
                        {
                            foreach (var kvp in DBC.UnitDictionary)
                            {
                                if (kvp.Value.Title == hit.transform.name)
                                {
                                    UnitImage.GetComponent<Image>().sprite = DBC.loadSprite(DBC.UnitDictionary[kvp.Key].ArtworkDirectory[0]);
                                    UnitText.GetComponent<Text>().text = kvp.Value.Title;
                                    UnitDescription.GetComponent<Text>().text = kvp.Value.Description;
                                    UnitToolTipData.GetComponent<Text>().text = kvp.Value.Attack.ToString() + Environment.NewLine + kvp.Value.Defence.ToString() + Environment.NewLine + kvp.Value.Range.ToString() + Environment.NewLine + kvp.Value.MovePoints.ToString();
                                }
                            }
                        }
                        if (hit.transform.tag == DBC.BuildingDictionary[0].Type)
                        {
                            foreach (var kvp in DBC.BuildingDictionary)
                            {
                                if (kvp.Value.Title == hit.transform.name)
                                {
                                    BuildingImage.GetComponent<Image>().sprite = DBC.loadSprite(DBC.BuildingDictionary[kvp.Key].ArtworkDirectory[0]);
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
                                Debug.Log("1");
                                if (!hit.transform.GetComponent<BuildingController>().Occupied && hit.transform.GetComponent<BuildingController>().CanBuild && hit.transform.GetComponent<BuildingController>().Team == GCS.CurrentTeamsTurn)
                                {
                                    Debug.Log("1.1");
                                    BuildingPanel.SetActive(true);
                                    CurrentlySelectedBuilding = hit.transform.position;
                                }
                                else
                                {
                                    Debug.Log("1.2");
                                    BuildingPanel.SetActive(false);
                                }
                            }
                        }
                        else if (hit.transform.tag != DBC.BuildingDictionary[0].Type && !BuildingRayBool)
                        {
                            Debug.Log("2");
                            BuildingPanel.SetActive(false);
                        }
                    }
                    BuildingRayBool = false;
                }
            }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    private void AddUnitButtonsToBuildContent()
    {
        try
        {
            Debug.Log("Adding terrain buttons to content window");
            foreach (KeyValuePair<int, Unit> kvp in DBC.UnitDictionary) //adds a button for each terrain in the database
            {
                GameObject tempbutton = Instantiate(BuildingButtonPrefab, ContentWindowBuilding.transform); //create button and set its parent to content
                tempbutton.name = kvp.Value.Title; //change name
                tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title; //change text on button to match sprite
                tempbutton.GetComponent<Image>().sprite = DBC.loadSprite(DBC.UnitDictionary[kvp.Key].ArtworkDirectory[0]); //set sprite
                tempbutton.GetComponent<Button>().onClick.AddListener(CreateUnitController); //adds method to button clicked
            }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    } //populates the tile selection bar

    public void CreateUnitController()
    {
        try
        {
            foreach (var kvp in DBC.UnitDictionary)
            {
                if (kvp.Value.Title == EventSystem.current.currentSelectedGameObject.name)
                {
                    DBC.CreateAndSpawnUnit(CurrentlySelectedBuilding, kvp.Value.ID, GCS.CurrentTeamsTurn);
                    GCS.AllRoundUpdater();
                    BuildingPanel.SetActive(false);
                    foreach (var b in GCS.BuildingPos)
                    {
                        if ((Vector2)b.Value.transform.position == CurrentlySelectedBuilding)
                        {
                            b.Value.GetComponent<BuildingController>().CanBuild = false;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void GameEndController(int Team)
    {
        try
        {
            CurrentPlayerTurnImage.SetActive(false);
            EndTurnButton.SetActive(false);
            TooltipPanel.SetActive(false);
            ActionPanel.SetActive(false);
            BuildingPanel.SetActive(false);
            EndGamePanel.SetActive(true);
            EndGameText.text = "Team: " + Team.ToString() + " has won the game.";
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

    public void ReplayButtonClicked()
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
            GCS.LoadMapPlayScene(GCS.MapNameForPlayScene);
            GCS.PlaySceneNewGameInitalizer();
            CurrentPlayerTurnImage.SetActive(true);
            EndTurnButton.SetActive(true);
            TooltipPanel.SetActive(true);
            ActionPanel.SetActive(true);
            EndGamePanel.SetActive(false);
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }
}
