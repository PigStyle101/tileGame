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
    public InputField mapsizeIF;
    public Text errorTextField;
    public GameObject LoadMenueButtonPrefab;
    public GameObject ContentWindowLoad;
    public GameObject ContentWindowNewGame;
    public GameObject MainPanel;


    // everything in here is pretty self explanitory.
    void Start()
    {
        MainPanel.SetActive(true);
        MainMenueButtonClicked();
    }

    public void MapEditorButtonClicked()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(true);
        PlayMenuePanel.SetActive(false);
        LoadGamePanel.SetActive(false);
    }

    public void MainMenueButtonClicked ()
    {
        MainMenuePanel.SetActive(true);
        MapEditorMenuePanel.SetActive(false);
        PlayMenuePanel.SetActive(false);
        LoadGamePanel.SetActive(false);
    }

    public void NewGameButtonClicked ()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(false);
        PlayMenuePanel.SetActive(true);
        LoadGamePanel.SetActive(false);
        GetMaps();
    }

    public void LoadGameButtonClicked()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(false);
        PlayMenuePanel.SetActive(false);
        LoadGamePanel.SetActive(true);
    }

    public void QuitGameButtonClicked ()
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
                GameControllerScript.instance.CreateNewMapForMapEditor(tempMapSize);
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

    public void GetMaps()
    {
        var childcount = ContentWindowNewGame.transform.childCount;
        for (int i = 0; i < childcount; i++)
        {
            Destroy(ContentWindowNewGame.transform.GetChild(i).gameObject);
        }
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Maps", "*.json")))
        {
            GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, ContentWindowNewGame.transform); //create button and set its parent to content
            tempbutton.name = Path.GetFileNameWithoutExtension(file); //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
            tempbutton.GetComponent<Button>().onClick.AddListener(LoadSelected); //adds method to button clicked
        }
    }

    public void LoadSelected()
    {
        GameControllerScript.instance.MapNameForPlayScene = EventSystem.current.currentSelectedGameObject.name;
        UnityEngine.SceneManagement.SceneManager.LoadScene("PlayScene");
        GameControllerScript.instance.PlaySceneLoadStatus = "NewGame";
    }
}
