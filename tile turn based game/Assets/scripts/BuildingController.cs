using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class BuildingController : MonoBehaviour
{
    private GameControllerScript GCS;
    private MapEditMenueCamController MEMCC;
    private DatabaseController DBC;
    //[HideInInspector]
    public int Team;
    //[HideInInspector]
    public bool Occupied = false;
    //[HideInInspector]
    public bool CanBuild;

    private void Awake()
    {
        try
        {
            if (SceneManager.GetActiveScene().name == "MapEditorScene")
            {
                MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
                Team = MEMCC.SelectedTeam;
            }
            GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
            DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }
    
    public void TeamSpriteUpdater()
    {
        try
        {
            foreach (var kvp in DBC.BuildingDictionary)
            {
                if (gameObject.name == kvp.Value.Title)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.BuildingDictionary[kvp.Value.ID].ArtworkDirectory[Team]);
                }
            }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void ChangeBuilding()
    {
        try
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
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void BuildingRoundUpdater()
    {
        try
        {
            foreach (var kvp in GCS.UnitPos)
            {
                if (kvp.Key == (Vector2)gameObject.transform.position)
                {
                    Occupied = true;
                }
                else
                {
                    Occupied = false;
                }
            }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }
}
