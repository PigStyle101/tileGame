using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using System.Threading;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TileGame
{
    public class MenueController : MonoBehaviour
    {
        public GameObject LoadMenueButtonPrefab;
        public GameObject ModsButtonPrefab;
        private GameObject CurrentlySellectedLoadObject;
        private string SaveGameSelectedString;
        private string CurrentlySellectedMod;
        private string SelectedMapForMapEditor;
        private int NewHeroCurrentlySelected;
        private int NewHeroClassCurrentlySelected;
        private List<GameObject> ModsInModContentWindow = new List<GameObject>();
        private List<string> ModsList = new List<string>();
        private GameControllerScript GCS;
        private DatabaseController DBC;
        public int LoadState;
        private bool Loading;

        private List<GameObject> menuPanel = new List<GameObject>();
        private List<GameObject> menuStuff = new List<GameObject>();

        // everything in here is pretty self explanitory.
        void Start()
        {
            try
            {
                FindMenuStuff("NewHeroClassDropDown").GetComponent<Dropdown>().onValueChanged.AddListener(delegate { ClassDropDownChanged(FindMenuStuff("NewHeroClassDropDown").GetComponent<Dropdown>()); });
                //FindMenuStuff("ClassDropDown").GetComponent<Dropdown>().onValueChanged.AddListener(delegate { ClassDropDownChanged(FindMenuStuff("ClassDropDown").GetComponent<Dropdown>()); });
                ReturnToMainMenuClicked();
                if (DBC.MasterData.HeroSelectedWhenGameClosed != "" && DBC.HeroDictionary.ContainsKey(DBC.MasterData.HeroSelectedWhenGameClosed))
                {
                    GCS.HeroCurrentlySelectedP1 = DBC.HeroDictionary[DBC.MasterData.HeroSelectedWhenGameClosed];
                }
                else
                {
                    GCS.HeroCurrentlySelectedP1 = null;
                }
            }
            catch (Exception e)
            {
                //If you get this error you probably need to switch to the initalization screen
                GCS.LogController(e.ToString());
                throw;
            }
        }

        private void Awake()
        {
            try
            {
                DBC = DatabaseController.instance;
                GCS = GameControllerScript.instance;
                GetObjectReferances();
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        private void Update()
        {
            if (Loading)
            {
                switch (LoadState)
                {
                    case 0:
                        ModLoadingUpdater(0f, "Getting Terrain data");
                        break;
                    case 1:
                        ModLoadingUpdater(.1f, "Getting Unit data");
                        break;
                    case 2:
                        ModLoadingUpdater(.2f, "Getting Hero data");
                        break;
                    case 3:
                        ModLoadingUpdater(.3f, "Looking for bugs");
                        break;
                    case 4:
                        ModLoadingUpdater(1f, "Anything start smoking?");
                        Loading = false;
                        break;
                }
            }
        }

        public void GetObjectReferances()
        {
            try
            {
                foreach (GameObject myPanel in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (myPanel.tag == "MenuPanel")
                    {
                        menuPanel.Add(myPanel);
                        //UnityEngine.Debug.Log("Added a panel " + myPanel.name);
                    }
                }

                foreach (GameObject stuff in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (stuff.tag == "MenuStuff")
                    {
                        menuStuff.Add(stuff);
                    }
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        [MoonSharpHidden]
        public IEnumerator ModLoader()
        {
            Loading = true;
            foreach (string Mod in ModsList)
            {
                LoadState = 0;
                yield return null;
                DBC.GetTerrianData(Mod);
                LoadState = 1;
                yield return null;
                DBC.GetUnitData(Mod);
                LoadState = 2;
                yield return null;
                DBC.GetBuildingData(Mod);
                LoadState = 3;
                yield return null;
                DBC.GetHeroClassData(Mod);
                DBC.GetHeroRaceData(Mod);
                DBC.GetSpellData(Mod);
                DBC.GetMasterJson();
                DBC.GetMouseJson();
                DBC.GetFogOfWarJson();
                DBC.GetLuaCoreScripts();
                DBC.ModsLoaded.Add(Mod);
                LoadState = 4;
                yield return null;
            }
            Loading = false;
            LoadState = 0;
            SetOnePanelActive(FindPanel("LoadingModsPanel"), false);
            LoadingModsFinished();
        }

        public void ModLoadingUpdater(float f, string State)
        {
            try
            {
                var myLocalPanel = FindPanel("LoadingModsPanel");
                Slider LoadingSlider = myLocalPanel.transform.Find("Slider").GetComponent<Slider>();
                Text LoadingText = myLocalPanel.transform.Find("Text").GetComponent<Text>();
                LoadingSlider.value = f;
                LoadingText.text = State;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void MainMenueButtonClickedFromModScreen()
        {
            try
            {
                bool ModsSame = false;
                if (ModsList.Count >= 1)
                {
                    if (ModsList.Count == DBC.ModsLoaded.Count) //if the mod count is the same we need to make sure they are the same ones
                    {
                        foreach (var mod in ModsList)
                        {
                            if (!DBC.ModsLoaded.Contains(mod)) //if any mod is not the same break
                            {
                                ModsSame = false;
                                break;
                            }
                            else
                            {
                                ModsSame = true;
                            }
                        }
                    }
                    if (!ModsSame) //if they are not the same
                    {
                        DBC.UnitDictionary.Clear();
                        DBC.BuildingDictionary.Clear();
                        DBC.TerrainDictionary.Clear();
                        DBC.ModsLoaded.Clear();
                        DBC.ResetNextIndexes();
                        ModsList.Sort();
                        SetOnePanelActive(FindPanel("LoadingModsPanel"), true);
                        StartCoroutine(ModLoader());
                        ModLoader();
                    }
                    else
                    {
                        ReturnToMainMenuClicked();
                    }
                }
                else
                {
                    FindMenuStuff("ModDescriptionText").GetComponent<Text>().text = "Must have at least one mod loaded!";
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void LoadingModsFinished()
        {
            try
            {
                bool HasMainBase = false;
                bool HasHeroSP = false;
                foreach (var kvp in DBC.BuildingDictionary)
                {
                    if (kvp.Value.MainBase && !HasMainBase)
                    {
                        HasMainBase = true;
                    }
                    if (kvp.Value.HeroSpawnPoint && !HasHeroSP)
                    {
                        HasHeroSP = true;
                    }
                }
                if (HasMainBase)
                {
                    if (HasHeroSP)
                    {
                        ReturnToMainMenuClicked();
                    }
                    else
                    {
                        FindMenuStuff("ModDescriptionText").GetComponent<Text>().text = "Must have at least one building with property HeroSpawnPoint = true";
                    }
                }
                else
                {
                    FindMenuStuff("ModDescriptionText").GetComponent<Text>().text = "Must have at least one building with property MainBase = true";
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void NewGameButtonClicked()
        {
            try
            {
                SetOnePanelActive(FindPanel("NewGamePanel"), true);
                GetMapsFornewGameWindow();
                FindMenuStuff("Team1Image").SetActive(false);
                FindMenuStuff("Team2Image").SetActive(false);
                FindMenuStuff("Team3Image").SetActive(false);
                FindMenuStuff("Team4Image").SetActive(false);
                FindMenuStuff("Team5Image").SetActive(false);
                FindMenuStuff("Team6Image").SetActive(false);
                FindMenuStuff("Team7Image").SetActive(false);
                FindMenuStuff("Team8Image").SetActive(false);
                FindMenuStuff("Team9Image").SetActive(false);
                FindMenuStuff("Team1Dropdown").SetActive(false);
                FindMenuStuff("Team2Dropdown").SetActive(false);
                FindMenuStuff("Team3Dropdown").SetActive(false);
                FindMenuStuff("Team4Dropdown").SetActive(false);
                FindMenuStuff("Team5Dropdown").SetActive(false);
                FindMenuStuff("Team6Dropdown").SetActive(false);
                FindMenuStuff("Team7Dropdown").SetActive(false);
                FindMenuStuff("Team8Dropdown").SetActive(false);
                FindMenuStuff("Team9Dropdown").SetActive(false);
                FindMenuStuff("Team1HeroDropdown").SetActive(false);
                FindMenuStuff("Team2HeroDropdown").SetActive(false);
                FindMenuStuff("Team3HeroDropdown").SetActive(false);
                FindMenuStuff("Team4HeroDropdown").SetActive(false);
                FindMenuStuff("Team5HeroDropdown").SetActive(false);
                FindMenuStuff("Team6HeroDropdown").SetActive(false);
                FindMenuStuff("Team7HeroDropdown").SetActive(false);
                FindMenuStuff("Team8HeroDropdown").SetActive(false);
                FindMenuStuff("Team9HeroDropdown").SetActive(false);
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void ReturnToMainMenuClicked()
        {
            SetOnePanelActive(FindPanel("MainMenuePanel"), true);
        }

        private GameObject FindPanel(String myPanelName)
        {
            UnityEngine.Debug.Log("Looking for " + myPanelName);
            foreach (GameObject myPanel in menuPanel)
            {
                if (myPanel.name == myPanelName)
                {
                    UnityEngine.Debug.Log("Found" + myPanel.name);
                    return myPanel;
                }
            }
            // if nothing was found
            // UGLY, FIX IT
            return menuPanel[0];
        }

        public GameObject FindMenuStuff (string stuffName)
        {
            UnityEngine.Debug.Log("Looking for " + stuffName);
            foreach (GameObject stuff in menuStuff)
            {
                if (stuff.name == stuffName)
                {
                    UnityEngine.Debug.Log("Found " + stuff.name);
                    return stuff;
                }
            }
            UnityEngine.Debug.Log("FindMenuStuff Failed");
            return null;
        }

        public void SubMenuButtonClicked(GameObject gameObject)
        {
            SetOnePanelActive(gameObject, true);
        }

        private void SetOnePanelActive(GameObject gameObject, Boolean isActive)
        {
            try
            {
                foreach (GameObject myPanel in menuPanel)
                {
                    if (gameObject == myPanel)
                        myPanel.SetActive(isActive);
                    else
                        myPanel.SetActive(!isActive);
                }
                if (gameObject.name == "ModsPanel")
                {
                    GetMods();
                }
                else if (gameObject.name == "NewHeroPanel")
                {
                    GetHeroesNew();
                    GetHeroClassesNew();
                }
                else if (gameObject.name == "LoadHeroPanel")
                {
                    GetHeroesLoad();
                }
                else if (gameObject.name == "NewGamePanel")
                {
                    GetMapsFornewGameWindow();
                }
                else if (gameObject.name == "LoadGamePanel")
                {
                    GetSaves();
                }
                else if (gameObject.name == "MapEditorMenuePanel")
                {
                    GetMapsForMapEditorWindow();
                }
                else if (gameObject.name == "MultiplayerPanel")
                {
                    MultiplayerController.instance.Connect();
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void QuitGameButtonClicked()
        {
            try
            {
                Master m = JsonUtility.FromJson<Master>(File.ReadAllText(Application.dataPath + "/StreamingAssets/MasterData/Master.json"));
                if (GCS.HeroCurrentlySelectedP1 != null)
                {
                    m.HeroSelectedWhenGameClosed = GCS.HeroCurrentlySelectedP1.Name;
                }
                string tempjson = JsonUtility.ToJson(m, true);
                FileStream fs = File.Create(Application.dataPath + "/StreamingAssets/MasterData/Master.json");
                StreamWriter sr = new StreamWriter(fs);
                sr.Write(tempjson);
                sr.Close();
                sr.Dispose();
                fs.Close();
                fs.Dispose();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void CreateMapButtonClickedMapEditorScreen()
        {
            try
            {
                int tempMapSize = new int();
                if (int.TryParse(FindMenuStuff("MapEditorInputField").GetComponent<Text>().text, out tempMapSize))
                {
                    if (tempMapSize >= 10 && tempMapSize <= 100)
                    {
                        GCS.CreateNewMapForMapEditor(tempMapSize);
                    }
                    else
                    {
                        FindMenuStuff("MapEditorErrorText").GetComponent<Text>().text = "Map size to big or to small";
                    }
                }
                else
                {
                    FindMenuStuff("MapEditorErrorText").GetComponent<Text>().text = "Not a valid input";
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void GetMapsFornewGameWindow()
        {
            try
            {
                var childcount = FindMenuStuff("NewGameContent").transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(FindMenuStuff("NewGameContent").transform.GetChild(i).gameObject);
                }
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Maps", "*.json")))
                {
                    var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                    Save tempjson = JsonUtility.FromJson<Save>(Tempstring); //this converts from json string to unity object
                    bool SameMods;
                    bool firstLoop = true;
                    if (DBC.ModsLoaded.Count == tempjson.Mods.Count) // is there the same number of mods when the save was made?
                    {
                        SameMods = true; //set this to true, and then check through mods
                        foreach (var item in DBC.ModsLoaded)
                        {
                            if (tempjson.Mods.Contains(item) && SameMods)
                            {
                                SameMods = true;
                            }
                            else
                            {
                                SameMods = false; //if a mod is not found go to false, ending the loop
                            }
                        }
                    }
                    else
                    {
                        SameMods = false;
                    }
                    if (SameMods) //must have same mods as when save was made to be able to load the save
                    {
                        GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, FindMenuStuff("NewGameContent").transform); //create button and set its parent to content
                        tempbutton.name = Path.GetFileNameWithoutExtension(file); //change name
                        tempbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
                        tempbutton.GetComponent<Button>().onClick.AddListener(MapSelectedNewGame); //adds method to button clicked
                        if (firstLoop)
                        {
                            SelectMapOnNewGamePanelLoad(tempbutton.name);
                            firstLoop = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void GetMapsForMapEditorWindow()
        {
            try
            {
                var childcount = FindMenuStuff("MapEditorContent").transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(FindMenuStuff("MapEditorContent").transform.GetChild(i).gameObject);
                }
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Maps", "*.json")))
                {
                    var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                    Save tempjson = JsonUtility.FromJson<Save>(Tempstring); //this converts from json string to unity object
                    bool SameMods;
                    if (DBC.ModsLoaded.Count == tempjson.Mods.Count)
                    {
                        SameMods = true;
                        foreach (var item in DBC.ModsLoaded)
                        {
                            if (tempjson.Mods.Contains(item) && SameMods)
                            {
                                SameMods = true;
                            }
                            else
                            {
                                SameMods = false;
                            }
                        }
                    }
                    else
                    {
                        SameMods = false;
                    }
                    if (SameMods)
                    {
                        GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, FindMenuStuff("MapEditorContent").transform); //create button and set its parent to content
                        tempbutton.name = Path.GetFileNameWithoutExtension(file); //change name
                        tempbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
                        tempbutton.GetComponent<Button>().onClick.AddListener(MapSelectedEditor); //adds method to button clicked 
                    }
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void GetSaves()
        {
            try
            {
                var childcount = FindMenuStuff("LoadGameContent").transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(FindMenuStuff("LoadGameContent").transform.GetChild(i).gameObject);
                }
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Saves", "*.json")))
                {
                    var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                    Save tempjson = JsonUtility.FromJson<Save>(Tempstring); //this converts from json string to unity object
                    bool SameMods;
                    if (DBC.ModsLoaded.Count == tempjson.Mods.Count) // is there the same number of mods when the save was made?
                    {
                        SameMods = true; //set this to true, and then check through mods
                        foreach (var item in DBC.ModsLoaded)
                        {
                            if (tempjson.Mods.Contains(item) && SameMods)
                            {
                                SameMods = true;
                            }
                            else
                            {
                                SameMods = false; //if a mod is not found go to false, ending teh loop
                            }
                        }
                    }
                    else
                    {
                        SameMods = false;
                    }
                    if (SameMods) //must have same mods as when save was made to be able to load the save
                    {
                        GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, FindMenuStuff("LoadGameContent").transform); //create button and set its parent to content
                        tempbutton.name = Path.GetFileNameWithoutExtension(file); //change name
                        tempbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
                        tempbutton.GetComponent<Button>().onClick.AddListener(SavedGameSelected); //adds method to button clicked 
                    }
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void GetMods()
        {
            try
            {
                var childcount = FindMenuStuff("ModsContent").transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(FindMenuStuff("ModsContent").transform.GetChild(i).gameObject);
                }
                ModsInModContentWindow.Clear();
                foreach (string file in Directory.GetDirectories(Application.dataPath + "/StreamingAssets/Mods/"))
                {
                    GameObject tempbutton = Instantiate(ModsButtonPrefab, FindMenuStuff("ModsContent").transform); //create button and set its parent to content
                    tempbutton.name = Path.GetFileNameWithoutExtension(file); //change name
                    tempbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
                    tempbutton.GetComponent<Button>().onClick.AddListener(ModSelected); //adds method to button clicked
                    ModsInModContentWindow.Add(tempbutton);

                }
                ModsList.Clear();
                foreach (var mod in DBC.ModsLoaded)
                {
                    ModsList.Add(mod);
                }
                UpdateModsActive();
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void GetHeroesNew()
        {
            try
            {
                var childcount = FindMenuStuff("NewHeroContent").transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(FindMenuStuff("NewHeroContent").transform.GetChild(i).gameObject);
                }
                foreach (var kvp in DBC.HeroRaceDictionary)
                {
                    GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, FindMenuStuff("NewHeroContent").transform); //create button and set its parent to content
                    tempbutton.name = kvp.Value.Title; //change name
                    tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title;
                    tempbutton.GetComponent<Button>().onClick.AddListener(NewHeroSelectedButton); //adds method to button clicked 
                    tempbutton.AddComponent<ButtonProperties>();
                    tempbutton.GetComponent<ButtonProperties>().ID = kvp.Value.ID;
                }
                NewHeroCurrentlySelected = 0;
                FindMenuStuff("NewHeroPreview").GetComponent<Image>().sprite = DBC.HeroRaceDictionary[NewHeroCurrentlySelected].IconSprite;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void GetHeroClassesNew()
        {
            try
            {
                FindMenuStuff("NewHeroClassDropDown").GetComponent<Dropdown>().ClearOptions();
                foreach (var kvp in DBC.HeroClassDictionary)
                {
                    //Debug.Log("Adding:" + kvp.Value.Title);
                    FindMenuStuff("NewHeroClassDropDown").GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = kvp.Value.Title });
                }
                FindMenuStuff("NewHeroClassDropDown").GetComponent<Dropdown>().RefreshShownValue();
                NewHeroClassCurrentlySelected = 0;
                ChangeNewHeroStats();
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void GetHeroesForNewGame(GameObject dropGM, int team)
        {
            try
            {
                Dropdown drop = dropGM.GetComponent<Dropdown>();
                drop.ClearOptions();
                foreach (var kvp in DBC.HeroDictionary)
                {
                    //Debug.Log("Adding:" + kvp.Value.Title);
                    drop.options.Add(new Dropdown.OptionData() { text = kvp.Value.Name });
                }
                drop.RefreshShownValue();
                switch (team)
                {
                    case 1:
                        GCS.HeroCurrentlySelectedP1 = DBC.HeroDictionary[drop.options[0].text];
                        break;
                    case 2:
                        GCS.HeroCurrentlySelectedP2 = DBC.HeroDictionary[drop.options[0].text];
                        break;
                    case 3:
                        GCS.HeroCurrentlySelectedP3 = DBC.HeroDictionary[drop.options[0].text];
                        break;
                    case 4:
                        GCS.HeroCurrentlySelectedP4 = DBC.HeroDictionary[drop.options[0].text];
                        break;
                    case 5:
                        GCS.HeroCurrentlySelectedP5 = DBC.HeroDictionary[drop.options[0].text];
                        break;
                    case 6:
                        GCS.HeroCurrentlySelectedP6 = DBC.HeroDictionary[drop.options[0].text];
                        break;
                    case 7:
                        GCS.HeroCurrentlySelectedP7 = DBC.HeroDictionary[drop.options[0].text];
                        break;
                    case 8:
                        GCS.HeroCurrentlySelectedP8 = DBC.HeroDictionary[drop.options[0].text];
                        break;
                    case 9:
                        GCS.HeroCurrentlySelectedP9 = DBC.HeroDictionary[drop.options[0].text];
                        break;
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void GetHeroesLoad()
        {
            try
            {
                var childcount = FindMenuStuff("LoadHeroContent").transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(FindMenuStuff("LoadHeroContent").transform.GetChild(i).gameObject);
                }
                foreach (var kvp in DBC.HeroDictionary)
                {
                    GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, FindMenuStuff("LoadHeroContent").transform); //create button and set its parent to content
                    tempbutton.name = Path.GetFileNameWithoutExtension(kvp.Value.Name); //change name
                    tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Name;
                    tempbutton.GetComponent<Button>().onClick.AddListener(LoadHeroSelectedButton); //adds method to button clicked 
                }
                if (GCS.HeroCurrentlySelectedP1 != null)
                {
                    FindMenuStuff("LoadHeroPreview").GetComponent<Image>().sprite = DBC.HeroRaceDictionary[GCS.HeroCurrentlySelectedP1.RaceID].IconSprite;
                    FindMenuStuff("LoadHeroIntValue").GetComponent<Text>().text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Intelligance.ToString();
                    FindMenuStuff("LoadHeroStrValue").GetComponent<Text>().text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Strenght.ToString();
                    FindMenuStuff("LoadHeroDexValue").GetComponent<Text>().text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Dexterity.ToString();
                    FindMenuStuff("LoadHeroCharValue").GetComponent<Text>().text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Charisma.ToString();
                    FindMenuStuff("LoadHeroFeedback").GetComponent<Text>().text = "Current hero selected: " + GCS.HeroCurrentlySelectedP1.Name;
                    FindMenuStuff("LoadHeroRaceText").GetComponent<Text>().text = "Race:" + DBC.HeroRaceDictionary[GCS.HeroCurrentlySelectedP1.RaceID].Title;
                    FindMenuStuff("LoadHeroClassText").GetComponent<Text>().text = "Class:" + DBC.HeroClassDictionary[GCS.HeroCurrentlySelectedP1.ClassID].Title;
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void NewHeroSelectedButton()
        {
            try
            {
                NewHeroCurrentlySelected = EventSystem.current.currentSelectedGameObject.transform.GetComponent<ButtonProperties>().ID;
                FindMenuStuff("NewHeroPreview").GetComponent<Image>().sprite = DBC.HeroRaceDictionary[NewHeroCurrentlySelected].IconSprite;
                ChangeNewHeroStats();
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void LoadHeroSelectedButton()
        {
            try
            {
                GCS.HeroCurrentlySelectedP1 = DBC.HeroDictionary[EventSystem.current.currentSelectedGameObject.name];
                FindMenuStuff("LoadHeroPreview").GetComponent<Image>().sprite = DBC.HeroRaceDictionary[GCS.HeroCurrentlySelectedP1.RaceID].IconSprite;
                FindMenuStuff("LoadHeroIntValue").GetComponent<Text>().text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Intelligance.ToString();
                FindMenuStuff("LoadHeroStrValue").GetComponent<Text>().text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Strenght.ToString();
                FindMenuStuff("LoadHeroDexValue").GetComponent<Text>().text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Dexterity.ToString();
                FindMenuStuff("LoadHeroCharValue").GetComponent<Text>().text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Charisma.ToString();
                FindMenuStuff("LoadHeroFeedback").GetComponent<Text>().text = "Current hero selected: " + GCS.HeroCurrentlySelectedP1.Name;
                FindMenuStuff("LoadHeroRaceText").GetComponent<Text>().text = "Race:" + DBC.HeroRaceDictionary[GCS.HeroCurrentlySelectedP1.RaceID].Title;
                FindMenuStuff("LoadHeroClassText").GetComponent<Text>().text = "Class:" + DBC.HeroClassDictionary[GCS.HeroCurrentlySelectedP1.ClassID].Title;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void SavedGameSelected()
        {
            try
            {
                SaveGameSelectedString = EventSystem.current.currentSelectedGameObject.name;
                CurrentlySellectedLoadObject = EventSystem.current.currentSelectedGameObject;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DeleteButtonClickedLoadPanel()
        {
            try
            {
                File.Delete(Application.dataPath + "/StreamingAssets/Saves/" + SaveGameSelectedString + ".json");
                Destroy(CurrentlySellectedLoadObject);
                SaveGameSelectedString = null;
                CurrentlySellectedLoadObject = null;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void LoadGameButtonClickedLoadPanel()
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void StartGameButtonClickedNewGamePanel()
        {
            try
            {
                int aiCount = 0;
                foreach (var t in GCS.TeamList)
                {
                    if (t.IsAi)
                    {
                        aiCount = aiCount + 1;
                    }
                }
                List<TeamStuff> TempTeamList = new List<TeamStuff>();
                foreach (var t in GCS.TeamList)
                {
                    if (t.Active)
                    {
                        TempTeamList.Add(t);
                    }
                }
                TempTeamList.RemoveAll(GCS.TeamIsNotActive);
                if (aiCount == TempTeamList.Count)
                {
                    FindMenuStuff("NewGameFeedBack").GetComponent<Text>().text = "Cannot start a game with all ai";
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("PlayScene");
                    GCS.PlaySceneLoadStatus = "NewGame";
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void ModSelected()
        {
            try
            {
                CurrentlySellectedMod = EventSystem.current.currentSelectedGameObject.name;
                FindMenuStuff("ModDescriptionText").GetComponent<Text>().text = File.ReadAllText(Application.dataPath + "/StreamingAssets/Mods/" + EventSystem.current.currentSelectedGameObject.name + "/Description.json");
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void AddModButtonClicked()
        {
            try
            {
                if (ModsList.Contains(CurrentlySellectedMod))
                {
                    FindMenuStuff("ModDescriptionText").GetComponent<Text>().text = "Mod " + CurrentlySellectedMod + "is already added.";
                    UpdateModsActive();
                }
                else
                {
                    ModsList.Add(CurrentlySellectedMod);
                    UpdateModsActive();
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void RemoveModButtonClicked()
        {
            try
            {
                if (ModsList.Contains(CurrentlySellectedMod))
                {
                    ModsList.Remove(CurrentlySellectedMod);
                    UpdateModsActive();
                }
                else
                {
                    FindMenuStuff("ModDescriptionText").GetComponent<Text>().text = "Mod " + CurrentlySellectedMod + " cannot be romoved as it is not on the list";
                    UpdateModsActive();
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void UpdateModsActive()
        {
            try
            {
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void MapSelectedNewGame()
        {
            try
            {
                GCS.MapNameForPlayScene = EventSystem.current.currentSelectedGameObject.name;
                StreamReader SR = new StreamReader(Application.dataPath + "/StreamingAssets/Maps/" + GCS.MapNameForPlayScene + ".json");
                string tempstring = SR.ReadToEnd();
                Map Load = JsonUtility.FromJson<Map>(tempstring);
                GCS.TeamList = Load.Teamlist;
                if (GCS.TeamList[1].Active)
                {
                    FindMenuStuff("Team1Image").SetActive(true);
                    FindMenuStuff("Team1Dropdown").SetActive(true);
                    FindMenuStuff("Team1HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team1HeroDropdown"), 1);
                }
                else
                {
                    FindMenuStuff("Team1Image").SetActive(false);
                    FindMenuStuff("Team1Dropdown").SetActive(false);
                    FindMenuStuff("Team1HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[2].Active)
                {
                    FindMenuStuff("Team2Image").SetActive(true);
                    FindMenuStuff("Team2Dropdown").SetActive(true);
                    FindMenuStuff("Team2HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team2HeroDropdown"), 2);
                }
                else
                {
                    FindMenuStuff("Team2Image").SetActive(false);
                    FindMenuStuff("Team2Dropdown").SetActive(false);
                    FindMenuStuff("Team2HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[3].Active)
                {
                    FindMenuStuff("Team3Image").SetActive(true);
                    FindMenuStuff("Team3Dropdown").SetActive(true);
                    FindMenuStuff("Team3HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team3HeroDropdown"), 3);
                }
                else
                {
                    FindMenuStuff("Team3Image").SetActive(false);
                    FindMenuStuff("Team3Dropdown").SetActive(false);
                    FindMenuStuff("Team3HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[4].Active)
                {
                    FindMenuStuff("Team4Image").SetActive(true);
                    FindMenuStuff("Team4Dropdown").SetActive(true);
                    FindMenuStuff("Team4HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team4HeroDropdown"), 4);
                }
                else
                {
                    FindMenuStuff("Team4Image").SetActive(false);
                    FindMenuStuff("Team4Dropdown").SetActive(false);
                    FindMenuStuff("Team4HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[5].Active)
                {
                    FindMenuStuff("Team5Image").SetActive(true);
                    FindMenuStuff("Team5Dropdown").SetActive(true);
                    FindMenuStuff("Team5HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team5HeroDropdown"), 5);
                }
                else
                {
                    FindMenuStuff("Team5Image").SetActive(false);
                    FindMenuStuff("Team5Dropdown").SetActive(false);
                    FindMenuStuff("Team5HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[6].Active)
                {
                    FindMenuStuff("Team6Image").SetActive(true);
                    FindMenuStuff("Team6Dropdown").SetActive(true);
                    FindMenuStuff("Team6HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team6HeroDropdown"), 6);
                }
                else
                {
                    FindMenuStuff("Team6Image").SetActive(false);
                    FindMenuStuff("Team6Dropdown").SetActive(false);
                    FindMenuStuff("Team6HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[7].Active)
                {
                    FindMenuStuff("Team7Image").SetActive(true);
                    FindMenuStuff("Team7Dropdown").SetActive(true);
                    FindMenuStuff("Team7HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team7HeroDropdown"), 7);
                }
                else
                {
                    FindMenuStuff("Team7Image").SetActive(false);
                    FindMenuStuff("Team7Dropdown").SetActive(false);
                    FindMenuStuff("Team7HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[8].Active)
                {
                    FindMenuStuff("Team8Image").SetActive(true);
                    FindMenuStuff("Team8Dropdown").SetActive(true);
                    FindMenuStuff("Team8HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team8HeroDropdown"), 8);
                }
                else
                {
                    FindMenuStuff("Team8Image").SetActive(false);
                    FindMenuStuff("Team8Dropdown").SetActive(false);
                    FindMenuStuff("Team8HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[9].Active)
                {
                    FindMenuStuff("Team9Image").SetActive(true);
                    FindMenuStuff("Team9Dropdown").SetActive(true);
                    FindMenuStuff("Team9HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team9HeroDropdown"), 9);
                }
                else
                {
                    FindMenuStuff("Team9Image").SetActive(false);
                    FindMenuStuff("Team9Dropdown").SetActive(false);
                    FindMenuStuff("Team9HeroDropdown").SetActive(false);
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void SelectMapOnNewGamePanelLoad(string MapName)
        {
            try
            {
                GCS.MapNameForPlayScene = MapName;
                StreamReader SR = new StreamReader(Application.dataPath + "/StreamingAssets/Maps/" + GCS.MapNameForPlayScene + ".json");
                string tempstring = SR.ReadToEnd();
                Map Load = JsonUtility.FromJson<Map>(tempstring);
                GCS.TeamList = Load.Teamlist;
                if (GCS.TeamList[1].Active)
                {
                    FindMenuStuff("Team1Image").SetActive(true);
                    FindMenuStuff("Team1Dropdown").SetActive(true);
                    FindMenuStuff("Team1HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team1HeroDropdown"), 1);
                }
                else
                {
                    FindMenuStuff("Team1Image").SetActive(false);
                    FindMenuStuff("Team1Dropdown").SetActive(false);
                    FindMenuStuff("Team1HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[2].Active)
                {
                    FindMenuStuff("Team2Image").SetActive(true);
                    FindMenuStuff("Team2Dropdown").SetActive(true);
                    FindMenuStuff("Team2HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team2HeroDropdown"), 2);
                }
                else
                {
                    FindMenuStuff("Team2Image").SetActive(false);
                    FindMenuStuff("Team2Dropdown").SetActive(false);
                    FindMenuStuff("Team2HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[3].Active)
                {
                    FindMenuStuff("Team3Image").SetActive(true);
                    FindMenuStuff("Team3Dropdown").SetActive(true);
                    FindMenuStuff("Team3HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team3HeroDropdown"), 3);
                }
                else
                {
                    FindMenuStuff("Team3Image").SetActive(false);
                    FindMenuStuff("Team3Dropdown").SetActive(false);
                    FindMenuStuff("Team3HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[4].Active)
                {
                    FindMenuStuff("Team4Image").SetActive(true);
                    FindMenuStuff("Team4Dropdown").SetActive(true);
                    FindMenuStuff("Team4HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team4HeroDropdown"), 4);
                }
                else
                {
                    FindMenuStuff("Team4Image").SetActive(false);
                    FindMenuStuff("Team4Dropdown").SetActive(false);
                    FindMenuStuff("Team4HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[5].Active)
                {
                    FindMenuStuff("Team5Image").SetActive(true);
                    FindMenuStuff("Team5Dropdown").SetActive(true);
                    FindMenuStuff("Team5HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team5HeroDropdown"), 5);
                }
                else
                {
                    FindMenuStuff("Team5Image").SetActive(false);
                    FindMenuStuff("Team5Dropdown").SetActive(false);
                    FindMenuStuff("Team5HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[6].Active)
                {
                    FindMenuStuff("Team6Image").SetActive(true);
                    FindMenuStuff("Team6Dropdown").SetActive(true);
                    FindMenuStuff("Team6HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team6HeroDropdown"), 6);
                }
                else
                {
                    FindMenuStuff("Team6Image").SetActive(false);
                    FindMenuStuff("Team6Dropdown").SetActive(false);
                    FindMenuStuff("Team6HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[7].Active)
                {
                    FindMenuStuff("Team7Image").SetActive(true);
                    FindMenuStuff("Team7Dropdown").SetActive(true);
                    FindMenuStuff("Team7HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team7HeroDropdown"), 7);
                }
                else
                {
                    FindMenuStuff("Team7Image").SetActive(false);
                    FindMenuStuff("Team7Dropdown").SetActive(false);
                    FindMenuStuff("Team7HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[8].Active)
                {
                    FindMenuStuff("Team8Image").SetActive(true);
                    FindMenuStuff("Team8Dropdown").SetActive(true);
                    FindMenuStuff("Team8HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team8HeroDropdown"), 8);
                }
                else
                {
                    FindMenuStuff("Team8Image").SetActive(false);
                    FindMenuStuff("Team8Dropdown").SetActive(false);
                    FindMenuStuff("Team8HeroDropdown").SetActive(false);
                }
                if (GCS.TeamList[9].Active)
                {
                    FindMenuStuff("Team9Image").SetActive(true);
                    FindMenuStuff("Team9Dropdown").SetActive(true);
                    FindMenuStuff("Team9HeroDropdown").SetActive(true);
                    GetHeroesForNewGame(FindMenuStuff("Team9HeroDropdown"), 9);
                }
                else
                {
                    FindMenuStuff("Team9Image").SetActive(false);
                    FindMenuStuff("Team9Dropdown").SetActive(false);
                    FindMenuStuff("Team9HeroDropdown").SetActive(false);
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void MapSelectedEditor()
        {
            try
            {
                SelectedMapForMapEditor = EventSystem.current.currentSelectedGameObject.name;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void LoadMapButtonClickedMapEditorScreen()
        {
            try
            {
                if (SelectedMapForMapEditor != null)
                {
                    GCS.LoadMapForMapEditorFromMainMenu(SelectedMapForMapEditor);
                }
                else
                {
                    FindMenuStuff("MapEditorErrorText").GetComponent<Text>().text = "Must select a map to load";
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void NewHeroCreateButtonClicked()
        {
            try
            {
                if (!File.Exists(Application.dataPath + "/StreamingAssets/HeroList/" + FindMenuStuff("NewHeroInputField").GetComponent<Text>().text + ".txt"))
                {
                    if (FindMenuStuff("NewHeroInputField").GetComponent<Text>().text != "")
                    {
                        if (!Regex.IsMatch(FindMenuStuff("NewHeroInputField").GetComponent<Text>().text, @"^[a-z][A-Z]+$"))
                        {
                            GCS.CreateNewHero(NewHeroCurrentlySelected, NewHeroClassCurrentlySelected, FindMenuStuff("NewHeroInputField").GetComponent<Text>().text);
                            FindMenuStuff("NewHeroFeedback").GetComponent<Text>().text = "New hero created.";
                        }
                        else
                        {
                            FindMenuStuff("NewHeroFeedback").GetComponent<Text>().text = "Only use letters";
                        }
                    }
                    else
                    {
                        FindMenuStuff("NewHeroFeedback").GetComponent<Text>().text = "Cannot leave name blank";
                    }
                }
                else
                {
                    FindMenuStuff("NewHeroFeedback").GetComponent<Text>().text = "Hero With this name already exist.";
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DeleteHeroButtonClicked()
        {
            try
            {
                FindMenuStuff("LoadHeroDeleteYesButton").SetActive(true);
                FindMenuStuff("LoadHeroDeleteNoButton").SetActive(true);
                FindMenuStuff("LoadHeroFeedback").GetComponent<Text>().text = "You sure???";
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void YesDelete()
        {
            try
            {
                FindMenuStuff("LoadHeroDeleteYesButton").SetActive(false);
                FindMenuStuff("LoadHeroDeleteNoButton").SetActive(false);
                File.Delete(Application.dataPath + "/StreamingAssets/HeroList/" + GCS.HeroCurrentlySelectedP1.Name + ".txt");
                DBC.HeroDictionary.Remove(GCS.HeroCurrentlySelectedP1.Name);
                GCS.HeroCurrentlySelectedP1 = null;
                FindMenuStuff("LoadHeroFeedback").GetComponent<Text>().text = "Current hero selected: None";
                GetHeroesLoad();
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void Nevermind()
        {
            try
            {
                FindMenuStuff("LoadHeroDeleteYesButton").SetActive(false);
                FindMenuStuff("LoadHeroDeleteNoButton").SetActive(false);
                FindMenuStuff("LoadHeroFeedback").GetComponent<Text>().text = "";
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void ClassDropDownChanged(Dropdown change)
        {
            try
            {
                //Debug.Log(DBC.HeroClassDictionary[change.value].Title);
                NewHeroClassCurrentlySelected = change.value;
                ChangeNewHeroStats();
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void ChangeNewHeroStats()
        {
            try
            {
                var intell = DBC.HeroClassDictionary[NewHeroClassCurrentlySelected].IntelliganceModifier + DBC.HeroRaceDictionary[NewHeroCurrentlySelected].Intelligance;
                var dext = DBC.HeroClassDictionary[NewHeroClassCurrentlySelected].DexterityModifier + DBC.HeroRaceDictionary[NewHeroCurrentlySelected].Dexterity;
                var stren = DBC.HeroClassDictionary[NewHeroClassCurrentlySelected].StrenghtModifier + DBC.HeroRaceDictionary[NewHeroCurrentlySelected].Strenght;
                var charis = DBC.HeroClassDictionary[NewHeroClassCurrentlySelected].CharismaModifier + DBC.HeroRaceDictionary[NewHeroCurrentlySelected].Charisma;
                if (intell < 0)
                {
                    intell = 0;
                }
                if (dext < 0)
                {
                    dext = 0;
                }
                if (stren < 0)
                {
                    stren = 0;
                }
                if (charis < 0)
                {
                    charis = 0;
                }
                FindMenuStuff("NewHeroIntValue").GetComponent<Text>().text = intell.ToString();
                FindMenuStuff("NewHeroDexValue").GetComponent<Text>().text = dext.ToString();
                FindMenuStuff("NewHeroStrValue").GetComponent<Text>().text = stren.ToString();
                FindMenuStuff("NewHeroCharValue").GetComponent<Text>().text = charis.ToString();
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void ReloadDataButonClicked()
        {
            try
            {
                DBC.UnitDictionary.Clear();
                DBC.BuildingDictionary.Clear();
                DBC.TerrainDictionary.Clear();
                DBC.HeroClassDictionary.Clear();
                DBC.HeroRaceDictionary.Clear();
                DBC.SpellJsonDictionary.Clear();
                DBC.SpellLuaDictionary.Clear();
                DBC.MouseDictionary.Clear();
                DBC.FogOfWarDictionary.Clear();
                DBC.LuaCoreScripts.Clear();
                ModsList = DBC.ModsLoaded;
                DBC.ModsLoaded = new List<string>();
                DBC.ResetNextIndexes();
                //ModsList.Sort();
                SetOnePanelActive(FindPanel("LoadingModsPanel"), true);
                StartCoroutine(ModLoader());
                ModLoader();
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DropdownTeam1Controller(int index)
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DropdownTeam2Controller(int index)
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DropdownTeam3Controller(int index)
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DropdownTeam4Controller(int index)
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DropdownTeam5Controller(int index)
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DropdownTeam6Controller(int index)
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DropdownTeam7Controller(int index)
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DropdownTeam8Controller(int index)
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DropdownTeam9Controller(int index)
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void Team1HeroDropdownController(int index)
        {
            try
            {
                GCS.HeroCurrentlySelectedP1 = DBC.HeroDictionary[FindMenuStuff("Team1HeroDropdown").GetComponent<Dropdown>().options[index].text];
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void Team2HeroDropdownController(int index)
        {
            try
            {
                GCS.HeroCurrentlySelectedP2 = DBC.HeroDictionary[FindMenuStuff("Team2HeroDropdown").GetComponent<Dropdown>().options[index].text];
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void Team3HeroDropdownController(int index)
        {
            try
            {
                GCS.HeroCurrentlySelectedP3 = DBC.HeroDictionary[FindMenuStuff("Team3HeroDropdown").GetComponent<Dropdown>().options[index].text];
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void Team4HeroDropdownController(int index)
        {
            try
            {
                GCS.HeroCurrentlySelectedP4 = DBC.HeroDictionary[FindMenuStuff("Team4HeroDropdown").GetComponent<Dropdown>().options[index].text];
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void Team5HeroDropdownController(int index)
        {
            try
            {
                GCS.HeroCurrentlySelectedP5 = DBC.HeroDictionary[FindMenuStuff("Team5HeroDropdown").GetComponent<Dropdown>().options[index].text];
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void Team6HeroDropdownController(int index)
        {
            try
            {
                GCS.HeroCurrentlySelectedP6 = DBC.HeroDictionary[FindMenuStuff("Team6HeroDropdown").GetComponent<Dropdown>().options[index].text];
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void Team7HeroDropdownController(int index)
        {
            try
            {
                GCS.HeroCurrentlySelectedP7 = DBC.HeroDictionary[FindMenuStuff("Team7HeroDropdown").GetComponent<Dropdown>().options[index].text];
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void Team8HeroDropdownController(int index)
        {
            try
            {
                GCS.HeroCurrentlySelectedP8 = DBC.HeroDictionary[FindMenuStuff("Team8HeroDropdown").GetComponent<Dropdown>().options[index].text];
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void Team9HeroDropdownController(int index)
        {
            try
            {
                GCS.HeroCurrentlySelectedP9 = DBC.HeroDictionary[FindMenuStuff("Team9HeroDropdown").GetComponent<Dropdown>().options[index].text];
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void CreateUserNameButtonClicked()
        {
            if (FindMenuStuff("UserNameInputField").GetComponent<Text>().text != "")
            {

            }
        }
    }
}
