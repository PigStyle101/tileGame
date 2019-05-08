﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingController : MonoBehaviour
{
    private GameControllerScript GCS;
    private MapEditMenueCamController MEMCC;
    private DatabaseController DBC;
    [HideInInspector]
    public int Team;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MapEditorScene")
        {
            MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
            Team = MEMCC.SelectedTeam;
        }
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();
    }
    
    public void TeamSpriteUpdater()
    {
        foreach (var kvp in DBC.BuildingDictionary)
        {
            if (gameObject.name == kvp.Value.Title)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.BuildingDictionary[kvp.Value.ID].ArtworkDirectory[Team]);
            }
        }
    }

    public void ChangeBuilding()
    {
        if (MEMCC.SelectedButton == "Delete Building")
        {
            Debug.Log("Deleting Building");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("ChangBuilding activated");
            foreach (KeyValuePair<int, Building> kvp in DBC.BuildingDictionary)
            {
                if (MEMCC.SelectedButton == kvp.Value.Title) //checks through dictionary for matching tile to button name
                {
                    //Debug.Log("Changing tile to " + kvp.Value.Title);
                    gameObject.name = kvp.Value.Title;//change name of tile
                    gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.BuildingDictionary[kvp.Key].ArtworkDirectory[0]); //change sprite of tile
                }
            }
        }
    }
}
