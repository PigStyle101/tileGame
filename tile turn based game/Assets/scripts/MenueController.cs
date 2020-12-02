﻿using System.Collections;
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

        private Image NewHeroPreview;
        private Image LoadHeroPreview;
        private InputField mapsizeIF;
        private InputField HeroInputField;
        private Text errorTextField;
        private Text ModDescriptionText;
        private Text FeedBackNewGame;
        public Text NewHeroFeedBackText;
        private Text NewHeroIntValue;
        private Text NewHeroStrValue;
        private Text NewHeroDexValue;
        private Text NewHeroCharValue;
        private Text LoadHeroFeedBackText;
        private Text LoadHeroIntValue;
        private Text LoadHeroStrValue;
        private Text LoadHeroDexValue;
        private Text LoadHeroCharValue;
        private Text LoadHeroClassText;
        private Text LoadHeroRaceText;
        public GameObject LoadMenueButtonPrefab;
        public GameObject ModsButtonPrefab;
        private GameObject ContentWindowLoad;
        private GameObject ContentWindowNewGame;
        private GameObject ContentWindowMods;
        private GameObject ContentWindowMapEditor;
        private GameObject ContentWindowNewHero;
        private GameObject ContentWindowLoadHero;
        private GameObject DeleteHeroNeverMind;
        private GameObject YesDeleteHero;
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
        private GameObject Team1HeroDropdown;
        private GameObject Team2HeroDropdown;
        private GameObject Team3HeroDropdown;
        private GameObject Team4HeroDropdown;
        private GameObject Team5HeroDropdown;
        private GameObject Team6HeroDropdown;
        private GameObject Team7HeroDropdown;
        private GameObject Team8HeroDropdown;
        private GameObject Team9HeroDropdown;
        private Dropdown ClassDropDown;
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

        // everything in here is pretty self explanitory.
        void Start()
        {
            try
            {
                ClassDropDown.onValueChanged.AddListener(delegate { ClassDropDownChanged(ClassDropDown); });
                ClassDropDown.onValueChanged.AddListener(delegate { ClassDropDownChanged(ClassDropDown); });
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

                DeleteHeroNeverMind = transform.Find("Canvas").Find("MainPanel").Find("LoadHeroPanel").Find("DeleteNoButton").gameObject;
                YesDeleteHero = transform.Find("Canvas").Find("MainPanel").Find("LoadHeroPanel").Find("DeleteYesButon").gameObject;
                NewHeroPreview = transform.Find("Canvas").Find("MainPanel").Find("NewHeroPanel").Find("Preview").GetComponent<Image>();
                LoadHeroPreview = transform.Find("Canvas").Find("MainPanel").Find("LoadHeroPanel").Find("Preview").GetComponent<Image>();
                mapsizeIF = transform.Find("Canvas").Find("MainPanel").Find("MapEditorMenuePanel").Find("InputFieldSize").GetComponent<InputField>();
                HeroInputField = transform.Find("Canvas").Find("MainPanel").Find("NewHeroPanel").Find("HeroInputField").GetComponent<InputField>();
                FeedBackNewGame = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("FeedBackText").GetComponent<Text>();
                LoadHeroFeedBackText = transform.Find("Canvas").Find("MainPanel").Find("LoadHeroPanel").Find("LoadHeroFeedback").GetComponent<Text>();
                LoadHeroClassText = transform.Find("Canvas").Find("MainPanel").Find("LoadHeroPanel").Find("RaceImage").Find("LoadHeroClassText").GetComponent<Text>();
                LoadHeroRaceText = transform.Find("Canvas").Find("MainPanel").Find("LoadHeroPanel").Find("ClassImage").Find("LoadHeroRaceText").GetComponent<Text>();
                errorTextField = transform.Find("Canvas").Find("MainPanel").Find("MapEditorMenuePanel").Find("ErrorHandlertext").GetComponent<Text>();
                ModDescriptionText = transform.Find("Canvas").Find("MainPanel").Find("ModsPanel").Find("DescriptionText").gameObject.GetComponent<Text>();
                NewHeroFeedBackText = transform.Find("Canvas").Find("MainPanel").Find("NewHeroPanel").Find("FeedbackText").gameObject.GetComponent<Text>();
                NewHeroIntValue = transform.Find("Canvas").Find("MainPanel").Find("NewHeroPanel").Find("IntImage").Find("IntValue").gameObject.GetComponent<Text>();
                NewHeroStrValue = transform.Find("Canvas").Find("MainPanel").Find("NewHeroPanel").Find("StrImage").Find("StrValue").gameObject.GetComponent<Text>();
                NewHeroDexValue = transform.Find("Canvas").Find("MainPanel").Find("NewHeroPanel").Find("DexImage").Find("DexValue").gameObject.GetComponent<Text>();
                NewHeroCharValue = transform.Find("Canvas").Find("MainPanel").Find("NewHeroPanel").Find("CharImage").Find("CharValue").gameObject.GetComponent<Text>();
                LoadHeroIntValue = transform.Find("Canvas").Find("MainPanel").Find("LoadHeroPanel").Find("IntImage").Find("IntValue").gameObject.GetComponent<Text>();
                LoadHeroStrValue = transform.Find("Canvas").Find("MainPanel").Find("LoadHeroPanel").Find("StrImage").Find("StrValue").gameObject.GetComponent<Text>();
                LoadHeroDexValue = transform.Find("Canvas").Find("MainPanel").Find("LoadHeroPanel").Find("DexImage").Find("DexValue").gameObject.GetComponent<Text>();
                LoadHeroCharValue = transform.Find("Canvas").Find("MainPanel").Find("LoadHeroPanel").Find("CharImage").Find("CharValue").gameObject.GetComponent<Text>();
                ContentWindowLoad = transform.Find("Canvas").Find("MainPanel").Find("LoadGamePanel").Find("LoadGameScrollView").Find("Viewport").Find("LoadGameContent").gameObject;
                ContentWindowNewGame = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("NewGameScrollView").Find("Viewport").Find("NewGameContent").gameObject;
                ContentWindowMods = transform.Find("Canvas").Find("MainPanel").Find("ModsPanel").Find("ModsScrollView").Find("ModsViewport").Find("ModsContent").gameObject;
                ContentWindowMapEditor = transform.Find("Canvas").Find("MainPanel").Find("MapEditorMenuePanel").Find("MapEditorScrollView").Find("Viewport").Find("MapEditorContent").gameObject;
                ContentWindowLoadHero = transform.Find("Canvas").Find("MainPanel").Find("LoadHeroPanel").Find("LoadHeroScrollView").Find("HeroViewport").Find("LoadHeroContent").gameObject;
                ContentWindowNewHero = transform.Find("Canvas").Find("MainPanel").Find("NewHeroPanel").Find("NewHeroScrollView").Find("HeroViewport").Find("NewHeroContent").gameObject;
                Team1Image = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team1Image").gameObject;
                Team2Image = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team2Image").gameObject;
                Team3Image = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team3Image").gameObject;
                Team4Image = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team4Image").gameObject;
                Team5Image = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team5Image").gameObject;
                Team6Image = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team6Image").gameObject;
                Team7Image = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team7Image").gameObject;
                Team8Image = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team8Image").gameObject;
                Team9Image = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team9Image").gameObject;
                Team1Dropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team1Dropdown").gameObject;
                Team2Dropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team2Dropdown").gameObject;
                Team3Dropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team3Dropdown").gameObject;
                Team4Dropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team4Dropdown").gameObject;
                Team5Dropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team5Dropdown").gameObject;
                Team6Dropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team6Dropdown").gameObject;
                Team7Dropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team7Dropdown").gameObject;
                Team8Dropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team8Dropdown").gameObject;
                Team9Dropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team9Dropdown").gameObject;
                Team1HeroDropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team1HeroDropdown").gameObject;
                Team2HeroDropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team2HeroDropdown").gameObject;
                Team3HeroDropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team3HeroDropdown").gameObject;
                Team4HeroDropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team4HeroDropdown").gameObject;
                Team5HeroDropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team5HeroDropdown").gameObject;
                Team6HeroDropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team6HeroDropdown").gameObject;
                Team7HeroDropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team7HeroDropdown").gameObject;
                Team8HeroDropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team8HeroDropdown").gameObject;
                Team9HeroDropdown = transform.Find("Canvas").Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team9HeroDropdown").gameObject;
                ClassDropDown = transform.Find("Canvas").Find("MainPanel").Find("NewHeroPanel").Find("ClassDropDown").GetComponent<Dropdown>();
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
                    ModDescriptionText.text = "Must have at least one mod loaded!";
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
                        ModDescriptionText.text = "Must have at least one building with property HeroSpawnPoint = true";
                    }
                }
                else
                {
                    ModDescriptionText.text = "Must have at least one building with property MainBase = true";
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
                Team1HeroDropdown.SetActive(false);
                Team2HeroDropdown.SetActive(false);
                Team3HeroDropdown.SetActive(false);
                Team4HeroDropdown.SetActive(false);
                Team5HeroDropdown.SetActive(false);
                Team6HeroDropdown.SetActive(false);
                Team7HeroDropdown.SetActive(false);
                Team8HeroDropdown.SetActive(false);
                Team9HeroDropdown.SetActive(false);
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
                var childcount = ContentWindowNewGame.transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(ContentWindowNewGame.transform.GetChild(i).gameObject);
                }
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Maps", "*.json")))
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
                        GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, ContentWindowNewGame.transform); //create button and set its parent to content
                        tempbutton.name = Path.GetFileNameWithoutExtension(file); //change name
                        tempbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
                        tempbutton.GetComponent<Button>().onClick.AddListener(MapSelectedNewGame); //adds method to button clicked
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
                var childcount = ContentWindowMapEditor.transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(ContentWindowMapEditor.transform.GetChild(i).gameObject);
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
                        GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, ContentWindowMapEditor.transform); //create button and set its parent to content
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
                var childcount = ContentWindowLoad.transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(ContentWindowLoad.transform.GetChild(i).gameObject);
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
                        GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, ContentWindowLoad.transform); //create button and set its parent to content
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
                var childcount = ContentWindowMods.transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(ContentWindowMods.transform.GetChild(i).gameObject);
                }
                ModsInModContentWindow.Clear();
                foreach (string file in Directory.GetDirectories(Application.dataPath + "/StreamingAssets/Mods/"))
                {
                    GameObject tempbutton = Instantiate(ModsButtonPrefab, ContentWindowMods.transform); //create button and set its parent to content
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
                var childcount = ContentWindowNewHero.transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(ContentWindowNewHero.transform.GetChild(i).gameObject);
                }
                foreach (var kvp in DBC.HeroRaceDictionary)
                {
                    GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, ContentWindowNewHero.transform); //create button and set its parent to content
                    tempbutton.name = kvp.Value.Title; //change name
                    tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Title;
                    tempbutton.GetComponent<Button>().onClick.AddListener(NewHeroSelectedButton); //adds method to button clicked 
                    tempbutton.AddComponent<ButtonProperties>();
                    tempbutton.GetComponent<ButtonProperties>().ID = kvp.Value.ID;
                }
                NewHeroCurrentlySelected = 0;
                NewHeroPreview.sprite = DBC.HeroRaceDictionary[NewHeroCurrentlySelected].IconSprite;
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
                ClassDropDown.ClearOptions();
                foreach (var kvp in DBC.HeroClassDictionary)
                {
                    //Debug.Log("Adding:" + kvp.Value.Title);
                    ClassDropDown.options.Add(new Dropdown.OptionData() { text = kvp.Value.Title });
                }
                ClassDropDown.RefreshShownValue();
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
                var childcount = ContentWindowLoadHero.transform.childCount;
                for (int i = 0; i < childcount; i++)
                {
                    Destroy(ContentWindowLoadHero.transform.GetChild(i).gameObject);
                }
                foreach (var kvp in DBC.HeroDictionary)
                {
                    GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, ContentWindowLoadHero.transform); //create button and set its parent to content
                    tempbutton.name = Path.GetFileNameWithoutExtension(kvp.Value.Name); //change name
                    tempbutton.transform.GetChild(0).GetComponent<Text>().text = kvp.Value.Name;
                    tempbutton.GetComponent<Button>().onClick.AddListener(LoadHeroSelectedButton); //adds method to button clicked 
                }
                if (GCS.HeroCurrentlySelectedP1 != null)
                {
                    LoadHeroPreview.sprite = DBC.HeroRaceDictionary[GCS.HeroCurrentlySelectedP1.RaceID].IconSprite;
                    LoadHeroIntValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Intelligance.ToString();
                    LoadHeroStrValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Strenght.ToString();
                    LoadHeroDexValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Dexterity.ToString();
                    LoadHeroCharValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Charisma.ToString();
                    LoadHeroFeedBackText.text = "Current hero selected: " + GCS.HeroCurrentlySelectedP1.Name;
                    LoadHeroRaceText.text = "Race:" + DBC.HeroRaceDictionary[GCS.HeroCurrentlySelectedP1.RaceID].Title;
                    LoadHeroClassText.text = "Class:" + DBC.HeroClassDictionary[GCS.HeroCurrentlySelectedP1.ClassID].Title;
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
                NewHeroPreview.sprite = DBC.HeroRaceDictionary[NewHeroCurrentlySelected].IconSprite;
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
                LoadHeroPreview.sprite = DBC.HeroRaceDictionary[GCS.HeroCurrentlySelectedP1.RaceID].IconSprite;
                LoadHeroIntValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Intelligance.ToString();
                LoadHeroStrValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Strenght.ToString();
                LoadHeroDexValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Dexterity.ToString();
                LoadHeroCharValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Charisma.ToString();
                LoadHeroFeedBackText.text = "Current hero selected: " + GCS.HeroCurrentlySelectedP1.Name;
                LoadHeroRaceText.text = "Race:" + DBC.HeroRaceDictionary[GCS.HeroCurrentlySelectedP1.RaceID].Title;
                LoadHeroClassText.text = "Class:" + DBC.HeroClassDictionary[GCS.HeroCurrentlySelectedP1.ClassID].Title;
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
                    FeedBackNewGame.text = "Cannot start a game with all ai";
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
                ModDescriptionText.text = File.ReadAllText(Application.dataPath + "/StreamingAssets/Mods/" + EventSystem.current.currentSelectedGameObject.name + "/Description.json");
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
                    ModDescriptionText.text = "Mod " + CurrentlySellectedMod + "is already added.";
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
                    ModDescriptionText.text = "Mod " + CurrentlySellectedMod + " cannot be romoved as it is not on the list";
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
                    Team1Image.SetActive(true);
                    Team1Dropdown.SetActive(true);
                    Team1HeroDropdown.SetActive(true);
                    GetHeroesForNewGame(Team1HeroDropdown, 1);
                }
                else
                {
                    Team1Image.SetActive(false);
                    Team1Dropdown.SetActive(false);
                    Team1HeroDropdown.SetActive(false);
                }
                if (GCS.TeamList[2].Active)
                {
                    Team2Image.SetActive(true);
                    Team2Dropdown.SetActive(true);
                    Team2HeroDropdown.SetActive(true);
                    GetHeroesForNewGame(Team2HeroDropdown, 2);
                }
                else
                {
                    Team2Image.SetActive(false);
                    Team2Dropdown.SetActive(false);
                    Team2HeroDropdown.SetActive(false);
                }
                if (GCS.TeamList[3].Active)
                {
                    Team3Image.SetActive(true);
                    Team3Dropdown.SetActive(true);
                    Team3HeroDropdown.SetActive(true);
                    GetHeroesForNewGame(Team3HeroDropdown, 3);
                }
                else
                {
                    Team3Image.SetActive(false);
                    Team3Dropdown.SetActive(false);
                    Team3HeroDropdown.SetActive(false);
                }
                if (GCS.TeamList[4].Active)
                {
                    Team4Image.SetActive(true);
                    Team4Dropdown.SetActive(true);
                    Team4HeroDropdown.SetActive(true);
                    GetHeroesForNewGame(Team4HeroDropdown, 4);
                }
                else
                {
                    Team4Image.SetActive(false);
                    Team4Dropdown.SetActive(false);
                    Team4HeroDropdown.SetActive(false);
                }
                if (GCS.TeamList[5].Active)
                {
                    Team5Image.SetActive(true);
                    Team5Dropdown.SetActive(true);
                    Team5HeroDropdown.SetActive(true);
                    GetHeroesForNewGame(Team5HeroDropdown, 5);
                }
                else
                {
                    Team5Image.SetActive(false);
                    Team5Dropdown.SetActive(false);
                    Team5HeroDropdown.SetActive(false);
                }
                if (GCS.TeamList[6].Active)
                {
                    Team6Image.SetActive(true);
                    Team6Dropdown.SetActive(true);
                    Team6HeroDropdown.SetActive(true);
                    GetHeroesForNewGame(Team6HeroDropdown, 6);
                }
                else
                {
                    Team6Image.SetActive(false);
                    Team6Dropdown.SetActive(false);
                    Team6HeroDropdown.SetActive(false);
                }
                if (GCS.TeamList[7].Active)
                {
                    Team7Image.SetActive(true);
                    Team7Dropdown.SetActive(true);
                    Team7HeroDropdown.SetActive(true);
                    GetHeroesForNewGame(Team7HeroDropdown, 7);
                }
                else
                {
                    Team7Image.SetActive(false);
                    Team7Dropdown.SetActive(false);
                    Team7HeroDropdown.SetActive(false);
                }
                if (GCS.TeamList[8].Active)
                {
                    Team8Image.SetActive(true);
                    Team8Dropdown.SetActive(true);
                    Team8HeroDropdown.SetActive(true);
                    GetHeroesForNewGame(Team8HeroDropdown, 8);
                }
                else
                {
                    Team8Image.SetActive(false);
                    Team8Dropdown.SetActive(false);
                    Team8HeroDropdown.SetActive(false);
                }
                if (GCS.TeamList[9].Active)
                {
                    Team9Image.SetActive(true);
                    Team9Dropdown.SetActive(true);
                    Team9HeroDropdown.SetActive(true);
                    GetHeroesForNewGame(Team9HeroDropdown, 9);
                }
                else
                {
                    Team9Image.SetActive(false);
                    Team9Dropdown.SetActive(false);
                    Team9HeroDropdown.SetActive(false);
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
                    errorTextField.text = "Must select a map to load";
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
                if (!File.Exists(Application.dataPath + "/StreamingAssets/HeroList/" + HeroInputField.text + ".txt"))
                {
                    if (HeroInputField.text != "")
                    {
                        if (!Regex.IsMatch(HeroInputField.text, @"^[a-z][A-Z]+$"))
                        {
                            GCS.CreateNewHero(NewHeroCurrentlySelected, NewHeroClassCurrentlySelected, HeroInputField.text);
                            NewHeroFeedBackText.text = "New hero created.";
                        }
                        else
                        {
                            NewHeroFeedBackText.text = "Only use letters";
                        }
                    }
                    else
                    {
                        NewHeroFeedBackText.text = "Cannot leave name blank";
                    }
                }
                else
                {
                    NewHeroFeedBackText.text = "Hero With this name already exist.";
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
                YesDeleteHero.SetActive(true);
                DeleteHeroNeverMind.SetActive(true);
                LoadHeroFeedBackText.text = "You sure???";
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
                YesDeleteHero.SetActive(false);
                DeleteHeroNeverMind.SetActive(false);
                File.Delete(Application.dataPath + "/StreamingAssets/HeroList/" + GCS.HeroCurrentlySelectedP1.Name + ".txt");
                DBC.HeroDictionary.Remove(GCS.HeroCurrentlySelectedP1.Name);
                GCS.HeroCurrentlySelectedP1 = null;
                LoadHeroFeedBackText.text = "Current hero selected: None";
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
                YesDeleteHero.SetActive(false);
                DeleteHeroNeverMind.SetActive(false);
                LoadHeroFeedBackText.text = "";
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
                NewHeroIntValue.text = intell.ToString();
                NewHeroDexValue.text = dext.ToString();
                NewHeroStrValue.text = stren.ToString();
                NewHeroCharValue.text = charis.ToString();
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
                GCS.HeroCurrentlySelectedP1 = DBC.HeroDictionary[Team1HeroDropdown.GetComponent<Dropdown>().options[index].text];
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
                GCS.HeroCurrentlySelectedP2 = DBC.HeroDictionary[Team2HeroDropdown.GetComponent<Dropdown>().options[index].text];
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
                GCS.HeroCurrentlySelectedP3 = DBC.HeroDictionary[Team3HeroDropdown.GetComponent<Dropdown>().options[index].text];
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
                GCS.HeroCurrentlySelectedP4 = DBC.HeroDictionary[Team4HeroDropdown.GetComponent<Dropdown>().options[index].text];
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
                GCS.HeroCurrentlySelectedP5 = DBC.HeroDictionary[Team5HeroDropdown.GetComponent<Dropdown>().options[index].text];
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
                GCS.HeroCurrentlySelectedP6 = DBC.HeroDictionary[Team6HeroDropdown.GetComponent<Dropdown>().options[index].text];
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
                GCS.HeroCurrentlySelectedP7 = DBC.HeroDictionary[Team7HeroDropdown.GetComponent<Dropdown>().options[index].text];
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
                GCS.HeroCurrentlySelectedP8 = DBC.HeroDictionary[Team8HeroDropdown.GetComponent<Dropdown>().options[index].text];
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
                GCS.HeroCurrentlySelectedP9 = DBC.HeroDictionary[Team9HeroDropdown.GetComponent<Dropdown>().options[index].text];
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }
    }
}
