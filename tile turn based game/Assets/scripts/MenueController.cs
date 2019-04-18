using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class MenueController : MonoBehaviour {

    public GameObject MainMenuePanel;
    public GameObject MapEditorMenuePanel;
    public GameObject PlayMenuePanel;
    public GameObject LoadGamePanel;
    private GameControllerScript GCS;
    public InputField mapsizeIF;
    public Text errorTextField;
    public GameObject LoadMenueButtonPrefab;
    public GameObject ContentWindowLoad;


    // Use this for initialization
    void Start ()
    {
        GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();	
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

    public void SettingMapSize()
    {
        int tempMapSize = new int();
        if (int.TryParse(mapsizeIF.text, out tempMapSize))
        {
            if (tempMapSize >= 10 && tempMapSize <= 100)
            {
                GCS.CreateNewMap(tempMapSize);
            }
            else
            {
                errorTextField.text = "Map size to big or to small";
            }
        }
        else
        {
            errorTextField.text = "Not a valid input";
        }

    }

    public void GetSaves()
    {
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Saves", "*.dat")))
        {
            GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, ContentWindowLoad.transform); //create button and set its parent to content
            tempbutton.name = file; //change name
            tempbutton.GetComponent<Button>().onClick.AddListener(LoadSelected); //adds method to button clicked
        }
    }

    public void LoadSelected()
    {
        GCS.LoadMap(EventSystem.current.currentSelectedGameObject.name);
    }

}
