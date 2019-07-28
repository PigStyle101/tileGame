using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class BuildingController : MonoBehaviour
{
    private MapEditMenueCamController MEMCC;
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
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }
    
    public void TeamSpriteUpdater()
    {
        try
        {
            foreach (var kvp in DatabaseController.instance.BuildingDictionary)
            {
                if (gameObject.name == kvp.Value.Title)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.BuildingDictionary[kvp.Value.ID].ArtworkDirectory[Team]);
                }
            }
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
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
                foreach (KeyValuePair<int, Building> kvp in DatabaseController.instance.BuildingDictionary)
                {
                    if (MEMCC.SelectedButton == kvp.Value.Title) //checks through dictionary for matching tile to button name
                    {
                        //Debug.Log("Changing tile to " + kvp.Value.Title);
                        gameObject.name = kvp.Value.Title;//change name of tile
                        gameObject.GetComponent<SpriteRenderer>().sprite = DatabaseController.instance.loadSprite(DatabaseController.instance.BuildingDictionary[kvp.Key].ArtworkDirectory[0]); //change sprite of tile
                    }
                }
            }
        }
        catch (Exception e)
        {
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }

    public void BuildingRoundUpdater()
    {
        try
        {
            foreach (var kvp in GameControllerScript.instance.UnitPos)
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
            GameControllerScript.instance.LogController(e.ToString());
            throw;
        }
    }
}
