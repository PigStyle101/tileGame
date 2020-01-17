﻿using System.Collections;
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
        gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.BuildingDictionary[DictionaryReferance].ArtworkDirectory[Team], DBC.BuildingDictionary[DictionaryReferance].PixelsPerUnit);
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
            BuildableUnits = DBC.BuildingDictionary[MEMCC.SelectedButtonDR].BuildableUnits;
            ID = DBC.BuildingDictionary[MEMCC.SelectedButtonDR].ID;
            DictionaryReferance = MEMCC.SelectedButtonDR;
            gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.BuildingDictionary[MEMCC.SelectedButtonDR].ArtworkDirectory[0], DBC.BuildingDictionary[MEMCC.SelectedButtonDR].PixelsPerUnit); //change sprite of tile

            if (GCS.UnitPos.ContainsKey(gameObject.transform.position) && DBC.BuildingDictionary[DictionaryReferance].HeroSpawnPoint)
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

    public void BuildingAiController()
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
    }

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
