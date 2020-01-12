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
    [HideInInspector]
    public int DictionaryReferance;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MapEditorScene")
        {
            MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
            Team = MEMCC.SelectedTeam;
        }
    }

    public void TeamSpriteUpdater()
    {

        foreach (var kvp in DatabaseController.instance.BuildingDictionary)
        {
            if (gameObject.name == kvp.Value.Title)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.BuildingDictionary[kvp.Value.ID].ArtworkDirectory[Team]);
            }
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
            gameObject.name = DatabaseController.instance.BuildingDictionary[MEMCC.SelectedButtonDR].Title;//change name of tile
            Team = MEMCC.SelectedTeam;
            CanBuildUnits = DatabaseController.instance.BuildingDictionary[MEMCC.SelectedButtonDR].CanBuildUnits;
            BuildableUnits = DatabaseController.instance.BuildingDictionary[MEMCC.SelectedButtonDR].BuildableUnits;
            ID = DatabaseController.instance.BuildingDictionary[MEMCC.SelectedButtonDR].ID;
            DictionaryReferance = MEMCC.SelectedButtonDR;
            gameObject.GetComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.BuildingDictionary[MEMCC.SelectedButtonDR].ArtworkDirectory[0]); //change sprite of tile
        }
    }

    public void BuildingRoundUpdater()
    {
        foreach (var kvp in GameControllerScript.instance.UnitPos)
        {
            if (kvp.Key == (Vector2)gameObject.transform.position)
            {
                Occupied = true;
                break;
            }
            else
            {
                Occupied = false;
            }
        }
    }

    public void BuildingAiController()
    {
        foreach(var kvp in DatabaseController.instance.UnitDictionary)
        {
            if (kvp.Value.Cost <= GameControllerScript.instance.TeamList[GameControllerScript.instance.CurrentTeamsTurn.Team].Gold && CanBuild && !Occupied && CanBuildUnits)
            {
                GameObject GO = DatabaseController.instance.CreateAndSpawnUnit(gameObject.transform.position, kvp.Value.ID, Team);
                GameControllerScript.instance.AddUnitsToDictionary(GO);
                GO.GetComponent<UnitController>().UnitMovable = false;
                foreach (var unit in GameControllerScript.instance.UnitPos)
                {
                    unit.Value.GetComponent<UnitController>().GetTileValues();
                }
                GameControllerScript.instance.TeamList[GameControllerScript.instance.CurrentTeamsTurn.Team].Gold = GameControllerScript.instance.TeamList[GameControllerScript.instance.CurrentTeamsTurn.Team].Gold - kvp.Value.Cost;
                PSCC = GameObject.Find("MainCamera").GetComponent<PlaySceneCamController>();
                PSCC.UpdateGoldThings();
                CanBuild = false;
            }
        }
    }

    public void HealIfFriendlyUnitOnBuilding()
    {
        foreach (var u in GameControllerScript.instance.UnitPos)
        {
            if (Health < MaxHealth && u.Key == (Vector2)gameObject.transform.position && u.Value.GetComponent<UnitController>().Team == Team)
            {
                Health = Health + 1;
                gameObject.GetComponentInChildren<Text>().text = Health.ToString();
            }
        }
    }
}
