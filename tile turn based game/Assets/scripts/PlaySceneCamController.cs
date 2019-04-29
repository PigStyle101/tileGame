using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySceneCamController : MonoBehaviour
{
    Vector3 dragOrigin;
    private GameControllerScript GCS;
    private DatabaseController DBC;
    public Text CurrentPlayerTurnText;

    private void Awake()
    {
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        MoveScreenXandY();
        MoveScreenZ();
    }

    private void MoveScreenXandY()
    {

        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

        Vector3 move = new Vector3(pos.x * DBC.dragSpeedOffset * DBC.DragSpeed * -1, pos.y * DBC.dragSpeedOffset * DBC.DragSpeed * -1, 0);

        transform.Translate(move, Space.World);

        if (gameObject.transform.position.x > GCS.PlayMapSize) { gameObject.transform.position = new Vector3(GCS.PlayMapSize, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y > GCS.PlayMapSize) { gameObject.transform.position = new Vector3(transform.position.x, GCS.PlayMapSize, transform.position.z); }
        if (gameObject.transform.position.x < 0) { gameObject.transform.position = new Vector3(0, transform.position.y, transform.position.z); }
        if (gameObject.transform.position.y < 0) { gameObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z); }
    } //controls camera movment y and x

    private void MoveScreenZ()
    {
        int z = new int();
        if (Input.GetAxis("Mouse ScrollWheel") > 0) { z = DBC.scrollSpeed; }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) { z = -DBC.scrollSpeed; }

        transform.Translate(new Vector3(0, 0, z), Space.World);

        if (gameObject.transform.position.z > -1) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -1); }
        if (gameObject.transform.position.z < -GCS.PlayMapSize * 2) { gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, -GCS.PlayMapSize * 2); }
    }//controls camera z movement

    public void EndTurnButtonClicked()
    {
        GCS.PlaySceneTurnChanger();
    }
}
