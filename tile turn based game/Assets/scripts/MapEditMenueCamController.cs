using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditMenueCamController : MonoBehaviour {

    public float dragSpeed = .5f;
    private Vector3 dragOrigin;
    private GameControllerScript GCS;

    // Use this for initialization
    void Start ()
    {
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        MoveScreen();
    }

    private void MoveScreen()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

        Vector3 move = new Vector3(pos.x * dragSpeed * -1, pos.y * dragSpeed * -1, 0);

        transform.Translate(move, Space.World);

        if (gameObject.transform.position.x > GCS.mapSize) { gameObject.transform.position = new Vector3(GCS.mapSize, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y > GCS.mapSize) { gameObject.transform.position = new Vector3(transform.position.x, GCS.mapSize, transform.position.z); }
        if (gameObject.transform.position.x < 0) { gameObject.transform.position = new Vector3(0, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y < 0) { gameObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z); }
    }

    /*public void ChangeTileSelectedToWater()
    {
        GCS.SelectedTile.name = "Water";
        GCS.SelectedTile.GetComponent<SpriteRenderer>().sprite = WaterSprite;
        foreach (KeyValuePair<Vector2,GameObject> kvp in GCS.TilePos)
        {
            kvp.Value.GetComponent<SpriteController>().WaterSpriteController();
        }
    }
    public void ChangeTileSelectedToGrass()
    {
        GCS.SelectedTile.name = "Grass";
        GCS.SelectedTile.GetComponent<SpriteRenderer>().sprite = GrassSprite;
    }*/
}
