using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenueController : MonoBehaviour {

    public GameObject MainMenuePanel;
    public GameObject MapEditorMenuePanel;
    public GameObject PlayMenuePanel;
    private GameControllerScript GCS;


	// Use this for initialization
	void Start ()
    {
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MapEditorPanelControlller()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(true);
        PlayMenuePanel.SetActive(false);
    }

    public void MainMenuePanelControlller ()
    {
        MainMenuePanel.SetActive(true);
        MapEditorMenuePanel.SetActive(false);
        PlayMenuePanel.SetActive(false);
    }

    public void PlayGamePanelController ()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(false);
        PlayMenuePanel.SetActive(true);
    }

    public void QuitGameController ()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void EditorMapSizePicker ()
    {

    }

}
