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
    public GameObject MainPanel;
    public GameObject ModPanel;
    public GameObject HeroPanel;
    public InputField mapsizeIF;
    public InputField TeamCountIF;
    public Text errorTextField;
    public Text ModDescriptionText;
    public GameObject LoadMenueButtonPrefab;
    public GameObject ModsButtonPrefab;
    public GameObject ContentWindowLoad;
    public GameObject ContentWindowNewGame;
    public GameObject ContentWindowMods;
    private List<string> ModsList = new List<string>();
    private string CurrentlySellectedMod;
    private List<GameObject> ModsInModContentWindow = new List<GameObject>();

    // everything in here is pretty self explanitory.
    void Start()
    {
        MainMenueButtonClicked();
    }

    public void MapEditorButtonClicked()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(true);
        PlayMenuePanel.SetActive(false);
        LoadGamePanel.SetActive(false);
        ModPanel.SetActive(false);
        HeroPanel.SetActive(false);
    }

    public void MainMenueButtonClicked()
    {
        MainMenuePanel.SetActive(true);
        MapEditorMenuePanel.SetActive(false);
        PlayMenuePanel.SetActive(false);
        LoadGamePanel.SetActive(false);
        ModPanel.SetActive(false);
        HeroPanel.SetActive(false);
    }

    public void MainMenueButtonClickedFromModScreen()
    {
        if (ModsList.Count >=1)
        {
            DatabaseController.instance.UnitDictionary.Clear();
            DatabaseController.instance.BuildingDictionary.Clear();
            DatabaseController.instance.TerrainDictionary.Clear();
            DatabaseController.instance.ModsLoaded.Clear();
            foreach (string Mod in ModsList)
            {
                DatabaseController.instance.GetTerrianJsons(Mod);
                DatabaseController.instance.GetUnitJsons(Mod);
                DatabaseController.instance.GetBuildingJsons(Mod);
                DatabaseController.instance.ModsLoaded.Add(Mod);
            }
            MainMenuePanel.SetActive(true);
            MapEditorMenuePanel.SetActive(false);
            PlayMenuePanel.SetActive(false);
            LoadGamePanel.SetActive(false);
            ModPanel.SetActive(false);
            HeroPanel.SetActive(false); 
        }
        else
        {
            ModDescriptionText.text = "Must have at least one mod loaded!";
        }
    }

    public void NewGameButtonClicked ()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(false);
        PlayMenuePanel.SetActive(true);
        LoadGamePanel.SetActive(false);
        ModPanel.SetActive(false);
        HeroPanel.SetActive(false);
        GetMaps();
    }

    public void LoadGameButtonClicked()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(false);
        PlayMenuePanel.SetActive(false);
        LoadGamePanel.SetActive(true);
        ModPanel.SetActive(false);
        HeroPanel.SetActive(false);
    }

    public void ModButtonClicked()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(false);
        PlayMenuePanel.SetActive(false);
        LoadGamePanel.SetActive(false);
        ModPanel.SetActive(true);
        HeroPanel.SetActive(false);
        GetMods();
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

    public void GetMods()
    {
        var childcount = ContentWindowMods.transform.childCount;
        for (int i = 0; i < childcount; i++)
        {
            Destroy(ContentWindowMods.transform.GetChild(i).gameObject);
        }
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/")))
        {
            GameObject tempbutton = Instantiate(ModsButtonPrefab, ContentWindowMods.transform); //create button and set its parent to content
            tempbutton.name = Path.GetFileNameWithoutExtension(file); //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
            tempbutton.GetComponent<Button>().onClick.AddListener(ModSelected); //adds method to button clicked
            ModsInModContentWindow.Add(tempbutton);
        }
    }

    public void ModSelected()
    {
        CurrentlySellectedMod = EventSystem.current.currentSelectedGameObject.name;
        ModDescriptionText.text = File.ReadAllText(Application.dataPath + "/StreamingAssets/Mods/" + EventSystem.current.currentSelectedGameObject.name + "/Description.json");
    }

    public void AddModButtonClicked()
    {
        if (ModsList.Contains(CurrentlySellectedMod))
        {
            ModDescriptionText.text = "Mod " + CurrentlySellectedMod + "is already added.";
        }
        else
        {
            ModsList.Add(CurrentlySellectedMod);
        }
        foreach (GameObject Button in ModsInModContentWindow)
        {
            if (ModsList.Contains(Button.name))
            {
                Button.GetComponent<Image>().color = new Color(0, 0.5f, 0);
            }
            else
            {
                Button.GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
    }

    public void RemoveModButtonClicked()
    {
        if (ModsList.Contains(CurrentlySellectedMod))
        {
            ModsList.Remove(CurrentlySellectedMod);
        }
        else
        {
            ModDescriptionText.text = "Mod " + CurrentlySellectedMod + " cannot be romoved as it is not on the list";
        }
        foreach (GameObject Button in ModsInModContentWindow)
        {
            if (ModsList.Contains(Button.name))
            {
                Button.GetComponent<Image>().color = new Color(0, 0.5f, 0);
            }
            else
            {
                Button.GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
    }
}
