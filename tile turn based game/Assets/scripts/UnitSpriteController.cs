using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpriteController : MonoBehaviour
{
    private GameControllerScript GCS;
    private MapEditMenueCamController MEMCC;
    private DatabaseController DBC;
    

    void Start()
    {
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
        DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();
    }

    void Update()
    {
        
    }

    public void ChangeUnit()
    {
        if (MEMCC.SelectedButton == "Delete Unit")
        {
            Debug.Log("Deleting Unit");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("ChangUnit activated");
            foreach (KeyValuePair<int, Unit> kvp in DBC.UnitDictionary)
            {
                if (MEMCC.SelectedButton == kvp.Value.Title) //checks through dictionary for matching tile to button name
                {
                    //Debug.Log("Changing tile to " + kvp.Value.Title);
                    gameObject.name = kvp.Value.Title;//change name of tile
                    gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.UnitDictionary[kvp.Key].ArtworkDirectory[0]); //change sprite of tile
                }
            }
        }
    }
}
