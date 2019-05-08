using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

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

    private void Awake()
    {
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();
    }

    private void Start()
    {
        SetActionButtonsToFalse();
    }

    void Update()
    {
        MoveScreenXandY();
        MoveScreenZ();
        RayCasterForPlayScene();
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

        if (gameObject.transform.position.x > GCS.PlayMapSize) { gameObject.transform.position = new Vector3(GCS.PlayMapSize, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y > GCS.PlayMapSize) { gameObject.transform.position = new Vector3(transform.position.x, GCS.PlayMapSize, transform.position.z); }
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
        if (gameObject.transform.position.z < -GCS.PlayMapSize * 2) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -GCS.PlayMapSize * 2); }
    }//controls camera z movement

    public void EndTurnButtonClicked()
    {
        GCS.PlaySceneTurnChanger();
    }

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
        CancelButton.SetActive(true);
    }

    public void WaitButtonClicked()
    {
        GCS.WaitActionPlayScene();
    }

    public void AttackButtonClicked()
    {
        GCS.AttackActionPlayScene();
        AttackButtonSelected = true;
    }

    public void CancelButtonClicked()
    {
        GCS.CancelActionPlayScene();
    }

    public void SetActionButtonsToFalse()
    {
        AttackButton.SetActive(false);
        WaitButton.SetActive(false);
        CancelButton.SetActive(false);
    }

    public void RayCasterForPlayScene()
    {
        if (Input.GetMouseButtonDown(0)) //are we in play scene?
        {
            if (!EventSystem.current.IsPointerOverGameObject()) //dont want to click through menus
            {
                Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); //GET THEM RAYS
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray);
                Debug.Log("Starting play scene ray hits");
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
                    else if (hit.transform.tag == DBC.UnitDictionary[0].Type)
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
                    else if (hit.transform.tag == DBC.BuildingDictionary[0].Type)
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
                }
            }
        }
    }
}
