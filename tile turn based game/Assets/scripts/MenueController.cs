using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenueController : MonoBehaviour {

    public GameObject MainMenuePanel;
    public GameObject MapEditorMenuePanel;
    public GameObject PlayMenuePanel;
    private GameControllerScript GCS;
    public InputField mapsizeIF;
    public Text errorTextField;


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
            if (tempMapSize >= 20 && tempMapSize <= 100)
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

}
