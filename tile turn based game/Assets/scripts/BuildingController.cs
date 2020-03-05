using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour
{
    private MapEditMenueCamController MEMCC;
    //[HideInInspector]
    public int Team;
    //[HideInInspector]
    public bool Occupied = false;
    //[HideInInspector]
    public bool CanBuild;
    [HideInInspector]
    public string Mod;
    [HideInInspector]
    public int Health;
    [HideInInspector]
    public int MaxHealth;
    [HideInInspector]
    public int DefenceBonus;
    [HideInInspector]
    public int ID;
    [HideInInspector]
    public List<string> BuildableUnits;
    [HideInInspector]
    public bool CanBuildUnits;
    private PlaySceneCamController PSCC;
    private GameControllerScript GCS;
    private DatabaseController DBC;

    private void Awake()
    {
        DBC = DatabaseController.instance;
        GCS = GameControllerScript.instance;
        if (SceneManager.GetActiveScene().name == "MapEditorScene")
        {
            MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
            Team = MEMCC.SelectedTeam;
        }
    }

    public void TeamSpriteUpdater()
    {
        //gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.BuildingDictionary[ID].ArtworkDirectory[Team], DBC.BuildingDictionary[ID].PixelsPerUnit);
        switch (Team)
        {
            case 1:
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.black;
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.white;
                break;
            case 2:
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.blue;
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.white;
                break;
            case 3:
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.cyan;
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 4:
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.gray;
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 5:
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.green;
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 6:
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.magenta;
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 7:
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.red;
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 8:
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.white;
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 9:
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.yellow;
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 0:
                Color brown = new Color(.5f, .25f, 0, 255);
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = brown;
                gameObject.transform.Find("BuildingHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
        }
    }

    public void ChangeBuilding()
    {
        if (MEMCC.SelectedButtonDR == -1)
        {
            //Debug.log("Deleting Building");
            Destroy(gameObject);
        }
        else
        {
            ////Debug.log("Changing tile to " + kvp.Value.Title);
            gameObject.name = DBC.BuildingDictionary[MEMCC.SelectedButtonDR].Title;//change name of tile
            Team = MEMCC.SelectedTeam;
            CanBuildUnits = DBC.BuildingDictionary[MEMCC.SelectedButtonDR].CanBuildUnits;
            //BuildableUnits = DBC.BuildingDictionary[MEMCC.SelectedButtonDR].BuildableUnits;
            ID = DBC.BuildingDictionary[MEMCC.SelectedButtonDR].ID;
            ID = MEMCC.SelectedButtonDR;
            //gameObject.GetComponent<SpriteRenderer>().sprite = DBC.BuildingDictionary[MEMCC.SelectedButtonDR].ArtworkDirectory[0]; //change sprite of tile

            if (GCS.UnitPos.ContainsKey(gameObject.transform.position) && DBC.BuildingDictionary[ID].HeroSpawnPoint)
            {
                Destroy(GCS.UnitPos[gameObject.transform.position]);
                GCS.UnitPos.Remove(gameObject.transform.position);
                MEMCC.CurrentSelectedButtonText.text = "Cannot have unit on hero spawn point";
            }
        }
    }

    public void BuildingRoundUpdater()
    {
        if (GCS.UnitPos.ContainsKey((Vector2)gameObject.transform.position))
        {
            Occupied = true;
        }
        else
        {
            Occupied = false;
        }
    }

    /*public void BuildingAiController()
    {
        foreach(var kvp in DBC.UnitDictionary)
        {
            if (kvp.Value.Cost <= GCS.TeamList[GCS.CurrentTeamsTurn.Team].Gold && CanBuild && !Occupied && CanBuildUnits)
            {
                GameObject GO = DBC.CreateAndSpawnUnit(gameObject.transform.position, kvp.Value.ID, Team);
                GCS.AddUnitsToDictionary(GO);
                GO.GetComponent<UnitController>().UnitMovable = false;
                foreach (var unit in GCS.UnitPos)
                {
                    unit.Value.GetComponent<UnitController>().GetTileValues();
                }
                GCS.TeamList[GCS.CurrentTeamsTurn.Team].Gold = GCS.TeamList[GCS.CurrentTeamsTurn.Team].Gold - kvp.Value.Cost;
                PSCC = GameObject.Find("MainCamera").GetComponent<PlaySceneCamController>();
                PSCC.UpdateGoldThings();
                CanBuild = false;
            }
        }
    } */

    public void HealIfFriendlyUnitOnBuilding()
    {
        if (GCS.UnitPos.ContainsKey(gameObject.transform.position))
        {
            if (Health < MaxHealth && GCS.UnitPos[gameObject.transform.position].GetComponent<UnitController>().Team == Team)
            {
                Health = Health + 1;
                gameObject.GetComponentInChildren<Text>().text = Health.ToString();
            }
        }
    }
}
