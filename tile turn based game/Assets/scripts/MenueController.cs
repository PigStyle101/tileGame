﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class MenueController : MonoBehaviour {
    
    private GameObject MainMenuePanel;
    private GameObject MapEditorMenuePanel;
    private GameObject NewGameMenuePanel;
    private GameObject LoadGamePanel;
    private GameObject MainPanel;
    private GameObject ModPanel;
    private GameObject HeroPanel;
    private InputField mapsizeIF;
    private Text errorTextField;
    private Text ModDescriptionText;
    public GameObject LoadMenueButtonPrefab;
    public GameObject ModsButtonPrefab;
    private GameObject ContentWindowLoad;
    private GameObject ContentWindowNewGame;
    private GameObject ContentWindowMods;
    private GameObject ContentWindowMapEditor;
    private Text FeedBackNewGame;
    private GameObject Team1Image;
    private GameObject Team2Image;
    private GameObject Team3Image;
    private GameObject Team4Image;
    private GameObject Team5Image;
    private GameObject Team6Image;
    private GameObject Team7Image;
    private GameObject Team8Image;
    private GameObject Team9Image;
    private GameObject Team1Dropdown;
    private GameObject Team2Dropdown;
    private GameObject Team3Dropdown;
    private GameObject Team4Dropdown;
    private GameObject Team5Dropdown;
    private GameObject Team6Dropdown;
    private GameObject Team7Dropdown;
    private GameObject Team8Dropdown;
    private GameObject Team9Dropdown;
    private string SaveGameSelectedString;
    private GameObject CurrentlySellectedLoadObject;
    private List<string> ModsList = new List<string>();
    private string CurrentlySellectedMod;
    private string SelectedMapForMapEditor;
    private List<GameObject> ModsInModContentWindow = new List<GameObject>();
    private GameControllerScript GCS;
    private DatabaseController DBC;

    // everything in here is pretty self explanitory.
    void Start()
    {
        MainMenueButtonClicked();
    }

    private void Awake()
    {
        DBC = DatabaseController.instance;
        GCS = GameControllerScript.instance;
        GetObjectReferances();
    }

    public void GetObjectReferances()
    {
        MainMenuePanel = gameObject.transform.Find("MainPanel").Find("MainMenuePanel").gameObject;
        MapEditorMenuePanel = gameObject.transform.Find("MainPanel").Find("MapEditorMenuePanel").gameObject;
        NewGameMenuePanel = gameObject.transform.Find("MainPanel").Find("NewGamePanel").gameObject;
        LoadGamePanel = gameObject.transform.Find("MainPanel").Find("LoadGamePanel").gameObject;
        HeroPanel = gameObject.transform.Find("MainPanel").Find("HeroPanel").gameObject;
        ModPanel = gameObject.transform.Find("MainPanel").Find("ModsPanel").gameObject;
        mapsizeIF = gameObject.transform.Find("MainPanel").Find("MapEditorMenuePanel").Find("InputFieldSize").GetComponent<InputField>();
        errorTextField = gameObject.transform.Find("MainPanel").Find("MapEditorMenuePanel").Find("ErrorHandlertext").GetComponent<Text>();
        ModDescriptionText = gameObject.transform.Find("MainPanel").Find("ModsPanel").Find("DescriptionText").gameObject.GetComponent<Text>();
        ContentWindowLoad = gameObject.transform.Find("MainPanel").Find("LoadGamePanel").Find("LoadGameScrollView").Find("Viewport").Find("LoadGameContent").gameObject;
        ContentWindowNewGame = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("NewGameScrollView").Find("Viewport").Find("NewGameContent").gameObject;
        ContentWindowMods = gameObject.transform.Find("MainPanel").Find("ModsPanel").Find("ModsScrollView").Find("ModsViewport").Find("ModsContent").gameObject;
        ContentWindowMapEditor = gameObject.transform.Find("MainPanel").Find("MapEditorMenuePanel").Find("MapEditorScrollView").Find("Viewport").Find("MapEditorContent").gameObject;
        FeedBackNewGame = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("FeedBackText").GetComponent<Text>();
        Team1Image = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team1Image").gameObject;
        Team2Image = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team2Image").gameObject;
        Team3Image = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team3Image").gameObject;
        Team4Image = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team4Image").gameObject;
        Team5Image = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team5Image").gameObject;
        Team6Image = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team6Image").gameObject;
        Team7Image = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team7Image").gameObject;
        Team8Image = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team8Image").gameObject;
        Team9Image = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team9Image").gameObject;
        Team1Dropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team1Dropdown").gameObject;
        Team2Dropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team2Dropdown").gameObject;
        Team3Dropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team3Dropdown").gameObject;
        Team4Dropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team4Dropdown").gameObject;
        Team5Dropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team5Dropdown").gameObject;
        Team6Dropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team6Dropdown").gameObject;
        Team7Dropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team7Dropdown").gameObject;
        Team8Dropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team8Dropdown").gameObject;
        Team9Dropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team9Dropdown").gameObject;
    }

    public void MapEditorButtonClicked()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(true);
        NewGameMenuePanel.SetActive(false);
        LoadGamePanel.SetActive(false);
        ModPanel.SetActive(false);
        HeroPanel.SetActive(false);
        GetMapsForMapEditorWindow();
    }

    public void MainMenueButtonClicked()
    {
        MainMenuePanel.SetActive(true);
        MapEditorMenuePanel.SetActive(false);
        NewGameMenuePanel.SetActive(false);
        LoadGamePanel.SetActive(false);
        ModPanel.SetActive(false);
        HeroPanel.SetActive(false);
    }

    public void MainMenueButtonClickedFromModScreen()
    {
        if (ModsList.Count >=1)
        {
            DBC.UnitDictionary.Clear();
            DBC.BuildingDictionary.Clear();
            DBC.TerrainDictionary.Clear();
            DBC.ModsLoaded.Clear();
            foreach (string Mod in ModsList)
            {
                DBC.GetTerrianJsons(Mod);
                DBC.GetUnitJsons(Mod);
                DBC.GetBuildingJsons(Mod);
                DBC.ModsLoaded.Add(Mod);
            }
            MainMenuePanel.SetActive(true);
            MapEditorMenuePanel.SetActive(false);
            NewGameMenuePanel.SetActive(false);
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
        NewGameMenuePanel.SetActive(true);
        LoadGamePanel.SetActive(false);
        ModPanel.SetActive(false);
        HeroPanel.SetActive(false);
        GetMapsForLoadGameWindow();
        Team1Image.SetActive(false);
        Team2Image.SetActive(false);
        Team3Image.SetActive(false);
        Team4Image.SetActive(false);
        Team5Image.SetActive(false);
        Team6Image.SetActive(false);
        Team7Image.SetActive(false);
        Team8Image.SetActive(false);
        Team9Image.SetActive(false);
        Team1Dropdown.SetActive(false);
        Team2Dropdown.SetActive(false);
        Team3Dropdown.SetActive(false);
        Team4Dropdown.SetActive(false);
        Team5Dropdown.SetActive(false);
        Team6Dropdown.SetActive(false);
        Team7Dropdown.SetActive(false);
        Team8Dropdown.SetActive(false);
        Team9Dropdown.SetActive(false);
    }

    public void LoadGameButtonClicked()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(false);
        NewGameMenuePanel.SetActive(false);
        LoadGamePanel.SetActive(true);
        ModPanel.SetActive(false);
        HeroPanel.SetActive(false);
        GetSaves();
    }

    public void ModButtonClicked()
    {
        MainMenuePanel.SetActive(false);
        MapEditorMenuePanel.SetActive(false);
        NewGameMenuePanel.SetActive(false);
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
                GCS.CreateNewMapForMapEditor(tempMapSize);
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

    public void GetMapsForLoadGameWindow()
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
            tempbutton.GetComponent<Button>().onClick.AddListener(MapSelectedNewGame); //adds method to button clicked
        }
    }

    public void GetMapsForMapEditorWindow()
    {
        var childcount = ContentWindowMapEditor.transform.childCount;
        for (int i = 0; i < childcount; i++)
        {
            Destroy(ContentWindowMapEditor.transform.GetChild(i).gameObject);
        }
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Maps", "*.json")))
        {
            GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, ContentWindowMapEditor.transform); //create button and set its parent to content
            tempbutton.name = Path.GetFileNameWithoutExtension(file); //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
            tempbutton.GetComponent<Button>().onClick.AddListener(MapSelectedEditor); //adds method to button clicked
        }
    }

    public void GetSaves()
    {
        var childcount = ContentWindowLoad.transform.childCount;
        for (int i = 0; i < childcount; i++)
        {
            Destroy(ContentWindowLoad.transform.GetChild(i).gameObject);
        }
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Saves", "*.json")))
        {
            GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, ContentWindowLoad.transform); //create button and set its parent to content
            tempbutton.name = Path.GetFileNameWithoutExtension(file); //change name
            tempbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
            tempbutton.GetComponent<Button>().onClick.AddListener(SaveGameSelected); //adds method to button clicked
        }
    }

    public void SaveGameSelected()
    {
        SaveGameSelectedString = EventSystem.current.currentSelectedGameObject.name;
        CurrentlySellectedLoadObject = EventSystem.current.currentSelectedGameObject;
    }

    public void DeleteButtonClickedLoadPanel()
    {
        File.Delete(Application.dataPath + "/StreamingAssets/Saves/" + SaveGameSelectedString + ".json");
        Destroy(CurrentlySellectedLoadObject);
        SaveGameSelectedString = null;
        CurrentlySellectedLoadObject = null;
    }

    public void LoadGameButtonClickedLoadPanel()
    {
        if (SaveGameSelectedString != null)
        {
            GCS.PlaySceneLoadStatus = "SavedGame";
            GCS.MapNameForPlayScene = SaveGameSelectedString;
            UnityEngine.SceneManagement.SceneManager.LoadScene("PlayScene");
            //Debug.Log("Loading");
        }
        else
        {
            //Debug.Log("== to null");
        }
    }

    public void StartGameButtonClickedNewGamePanel()
    {
        int aiCount = 0;
        foreach(var t in GCS.TeamList)
        {
            if (t.IsAi)
            {
                aiCount = aiCount + 1;
            }
        }
        List<TeamStuff> TempTeamList = new List<TeamStuff>();
        foreach(var t in GCS.TeamList)
        {
            if (t.Active)
            {
                TempTeamList.Add(t);
            }
        }
        TempTeamList.RemoveAll(GCS.TeamIsNotActive);
        if (aiCount == TempTeamList.Count)
        {
            FeedBackNewGame.text = "Cannot start a game with all ai";
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("PlayScene");
            GCS.PlaySceneLoadStatus = "NewGame";
        }
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

    public void MapSelectedNewGame()
    {
        GCS.MapNameForPlayScene = EventSystem.current.currentSelectedGameObject.name;
        StreamReader SR = new StreamReader(Application.dataPath + "/StreamingAssets/Maps/" + GCS.MapNameForPlayScene + ".json");
        string tempstring = SR.ReadToEnd();
        Map Load = JsonUtility.FromJson<Map>(tempstring);
        GCS.TeamList = Load.Teamlist;
        if (GCS.TeamList[1].Active)
        {
            Team1Image.SetActive(true);
            Team1Dropdown.SetActive(true);
        }
        else
        {
            Team1Image.SetActive(false);
            Team1Dropdown.SetActive(false);
        }
        if (GCS.TeamList[2].Active)
        {
            Team2Image.SetActive(true);
            Team2Dropdown.SetActive(true);
        }
        else
        {
            Team2Image.SetActive(false);
            Team2Dropdown.SetActive(false);
        }
        if (GCS.TeamList[3].Active)
        {
            Team3Image.SetActive(true);
            Team3Dropdown.SetActive(true);
        }
        else
        {
            Team3Image.SetActive(false);
            Team3Dropdown.SetActive(false);
        }
        if (GCS.TeamList[4].Active)
        {
            Team4Image.SetActive(true);
            Team4Dropdown.SetActive(true);
        }
        else
        {
            Team4Image.SetActive(false);
            Team4Dropdown.SetActive(false);
        }
        if (GCS.TeamList[5].Active)
        {
            Team5Image.SetActive(true);
            Team5Dropdown.SetActive(true);
        }
        else
        {
            Team5Image.SetActive(false);
            Team5Dropdown.SetActive(false);
        }
        if (GCS.TeamList[6].Active)
        {
            Team6Image.SetActive(true);
            Team6Dropdown.SetActive(true);
        }
        else
        {
            Team6Image.SetActive(false);
            Team6Dropdown.SetActive(false);
        }
        if (GCS.TeamList[7].Active)
        {
            Team7Image.SetActive(true);
            Team7Dropdown.SetActive(true);
        }
        else
        {
            Team7Image.SetActive(false);
            Team7Dropdown.SetActive(false);
        }
        if (GCS.TeamList[8].Active)
        {
            Team8Image.SetActive(true);
            Team8Dropdown.SetActive(true);
        }
        else
        {
            Team8Image.SetActive(false);
            Team8Dropdown.SetActive(false);
        }
        if (GCS.TeamList[9].Active)
        {
            Team9Image.SetActive(true);
            Team9Dropdown.SetActive(true);
        }
        else
        {
            Team9Image.SetActive(false);
            Team9Dropdown.SetActive(false);
        }
    }

    public void MapSelectedEditor()
    {
        SelectedMapForMapEditor = EventSystem.current.currentSelectedGameObject.name;
    }

    public void LoadMapMapEditor()
    {
        if (SelectedMapForMapEditor != null)
        {
            GCS.LoadMapForMapEditorFromMainMenu(SelectedMapForMapEditor); 
        }
        else
        {
            errorTextField.text = "Must select a map to load";
        }
    }

    public void DropdownTeam1Controller(int index)
    {
        if (index == 1)
        {
            GCS.TeamList[1].IsAi = true;
        }
        else
        {
            GCS.TeamList[1].IsAi = false;
        }
    }

    public void DropdownTeam2Controller(int index)
    {
        if (index == 1)
        {
            GCS.TeamList[2].IsAi = true;
        }
        else
        {
            GCS.TeamList[2].IsAi = false;
        }
    }

    public void DropdownTeam3Controller(int index)
    {
        if (index == 1)
        {
            GCS.TeamList[3].IsAi = true;
        }
        else
        {
            GCS.TeamList[3].IsAi = false;
        }
    }

    public void DropdownTeam4Controller(int index)
    {
        if (index == 1)
        {
            GCS.TeamList[4].IsAi = true;
        }
        else
        {
            GCS.TeamList[4].IsAi = false;
        }
    }

    public void DropdownTeam5Controller(int index)
    {
        if (index == 1)
        {
            GCS.TeamList[5].IsAi = true;
        }
        else
        {
            GCS.TeamList[5].IsAi = false;
        }
    }

    public void DropdownTeam6Controller(int index)
    {
        if (index == 1)
        {
            GCS.TeamList[6].IsAi = true;
        }
        else
        {
            GCS.TeamList[6].IsAi = false;
        }
    }

    public void DropdownTeam7Controller(int index)
    {
        if (index == 1)
        {
            GCS.TeamList[7].IsAi = true;
        }
        else
        {
            GCS.TeamList[7].IsAi = false;
        }
    }

    public void DropdownTeam8Controller(int index)
    {
        if (index == 1)
        {
            GCS.TeamList[8].IsAi = true;
        }
        else
        {
            GCS.TeamList[8].IsAi = false;
        }
    }

    public void DropdownTeam9Controller(int index)
    {
        if (index == 1)
        {
            GCS.TeamList[9].IsAi = true;
        }
        else
        {
            GCS.TeamList[9].IsAi = false;
        }
    }
}
