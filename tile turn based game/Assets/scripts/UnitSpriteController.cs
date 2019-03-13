using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpriteController : MonoBehaviour
{
    private GameControllerScript GCS;
    private MapEditMenueCamController MEMCC;

    // Start is called before the first frame update
    void Start()
    {
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && MEMCC.SelectedTab == "Unit")
        {
            GCS.UnitSelectedController(gameObject);
        }
            
    }
}
