using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteController : MonoBehaviour
{

    private GameControllerScript GCS;
    public Vector2 currentPos;
    public Sprite[] Sprits;
    public int counter;
    public SpriteRenderer MouseOverlaySpriteRender;
    public SpriteRenderer MouseOverlaySelectedSpriteRender;


    void Start()
    {
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        MouseOverlaySpriteRender = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>(); //this is working now
        MouseOverlaySelectedSpriteRender = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
        MouseOverlaySpriteRender.enabled = false;
        MouseOverlaySelectedSpriteRender.enabled = false;
    }

    private void addMouseOverlays()
    {

    }

    private void OnMouseUp()
    {
        //WaterSpriteController();
    }
    //WORK IN PROGRESS, actually... need to think about the best way to do this, might be easyer to use overlays?? not sure yet..... currently using a different sprite for things
    /*public void WaterSpriteController()
    {
        UnityEngine.Debug.Log("Starting Water Sprite Controller");
        try
        {
            if (gameObject.name == "Water")
            {
                counter = 0;
                currentPos = transform.position;
                Vector2 topLeftPos = currentPos + new Vector2(-1, 1);
                Vector2 topPos = currentPos + new Vector2(0, 1);
                Vector2 topRightPos = currentPos + new Vector2(1, 1);
                Vector2 rightPos = currentPos + new Vector2(1, 0);
                Vector2 bottomRightPos = currentPos + new Vector2(1, -1);
                Vector2 bottomPos = currentPos + new Vector2(0, -1);
                Vector2 bottomLeftPos = currentPos + new Vector2(-1, -1);
                Vector2 leftPos = currentPos + new Vector2(-1, 0);
                bool Top;
                bool Left;
                bool Right;
                bool Bottom;
                UnityEngine.Debug.Log("Starting if Statments");
                // check how many waters around
                //if (surrounding blocks that are land is = 1) {use waterland1side, then rotate to be correct}
                if (GCS.TilePos.ContainsKey(topPos)) { if (GCS.TilePos[topPos].name == "Grass") { counter = counter + 1; Top = true; } }
                if (GCS.TilePos.ContainsKey(rightPos)) { if (GCS.TilePos[rightPos].name == "Grass") { counter = counter + 1; Right = true; } }
                if (GCS.TilePos.ContainsKey(leftPos)) { if (GCS.TilePos[leftPos].name == "Grass") { counter = counter + 1; Left = true; } }
                if (GCS.TilePos.ContainsKey(bottomPos)) { if (GCS.TilePos[bottomPos].name == "Grass") { counter = counter + 1; Bottom = true; } }

                //add the if statment then check exactly what sides are there to determin rotation
                switch (counter)
                {
                    case 0:
                        UnityEngine.Debug.Log("Case 0");
                        gameObject.GetComponent<SpriteRenderer>().sprite = Sprits[0];
                        break;
                    case 1:
                        UnityEngine.Debug.Log("Case 1");
                        gameObject.GetComponent<SpriteRenderer>().sprite = Sprits[1];
                        break;
                    case 2:
                        UnityEngine.Debug.Log("Case 2");
                        break;
                    case 3:
                        UnityEngine.Debug.Log("Case 3");
                        break;
                    case 4:
                        UnityEngine.Debug.Log("Case 4");
                        break;
                }

            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
            throw;
        }
    }
    */

    private void OnMouseEnter() 
    {
        MouseOverlaySpriteRender.enabled = true;
    }

    private void OnMouseExit()
    {
        MouseOverlaySpriteRender.enabled = false;
    }

    private void OnMouseDown()
    {
        GCS.MouseSelectedController(MouseOverlaySelectedSpriteRender, gameObject);
    }
}
