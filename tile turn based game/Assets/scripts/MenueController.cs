using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

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
        private GameObject MainMenuePanel;
        private GameObject MapEditorMenuePanel;
        private GameObject NewGameMenuePanel;
        private GameObject LoadGamePanel;
        private GameObject ModPanel;
        private GameObject NewHeroPanel;
        private GameObject LoadHeroPanel;
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

        // everything in here is pretty self explanitory.
        void Start()
        {
            ClassDropDown.onValueChanged.AddListener(delegate { ClassDropDownChanged(ClassDropDown); });
            ClassDropDown.onValueChanged.AddListener(delegate { ClassDropDownChanged(ClassDropDown); });
            MainMenueButtonClicked();
            if (DBC.MasterData.HeroSelectedWhenGameClosed != "" && DBC.HeroDictionary.ContainsKey(DBC.MasterData.HeroSelectedWhenGameClosed))
            {
                GCS.HeroCurrentlySelectedP1 = DBC.HeroDictionary[DBC.MasterData.HeroSelectedWhenGameClosed];
            }
            else
            {
                GCS.HeroCurrentlySelectedP1 = null;
            }
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
            NewHeroPanel = gameObject.transform.Find("MainPanel").Find("NewHeroPanel").gameObject;
            LoadHeroPanel = gameObject.transform.Find("MainPanel").Find("LoadHeroPanel").gameObject;
            ModPanel = gameObject.transform.Find("MainPanel").Find("ModsPanel").gameObject;
            DeleteHeroNeverMind = gameObject.transform.Find("MainPanel").Find("LoadHeroPanel").Find("DeleteNoButton").gameObject;
            YesDeleteHero = gameObject.transform.Find("MainPanel").Find("LoadHeroPanel").Find("DeleteYesButon").gameObject;
            NewHeroPreview = gameObject.transform.Find("MainPanel").Find("NewHeroPanel").Find("Preview").GetComponent<Image>();
            LoadHeroPreview = gameObject.transform.Find("MainPanel").Find("LoadHeroPanel").Find("Preview").GetComponent<Image>();
            mapsizeIF = gameObject.transform.Find("MainPanel").Find("MapEditorMenuePanel").Find("InputFieldSize").GetComponent<InputField>();
            HeroInputField = gameObject.transform.Find("MainPanel").Find("NewHeroPanel").Find("HeroInputField").GetComponent<InputField>();
            FeedBackNewGame = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("FeedBackText").GetComponent<Text>();
            LoadHeroFeedBackText = gameObject.transform.Find("MainPanel").Find("LoadHeroPanel").Find("LoadHeroFeedback").GetComponent<Text>();
            errorTextField = gameObject.transform.Find("MainPanel").Find("MapEditorMenuePanel").Find("ErrorHandlertext").GetComponent<Text>();
            ModDescriptionText = gameObject.transform.Find("MainPanel").Find("ModsPanel").Find("DescriptionText").gameObject.GetComponent<Text>();
            NewHeroFeedBackText = gameObject.transform.Find("MainPanel").Find("NewHeroPanel").Find("FeedbackText").gameObject.GetComponent<Text>();
            NewHeroIntValue = gameObject.transform.Find("MainPanel").Find("NewHeroPanel").Find("IntImage").Find("IntValue").gameObject.GetComponent<Text>();
            NewHeroStrValue = gameObject.transform.Find("MainPanel").Find("NewHeroPanel").Find("StrImage").Find("StrValue").gameObject.GetComponent<Text>();
            NewHeroDexValue = gameObject.transform.Find("MainPanel").Find("NewHeroPanel").Find("DexImage").Find("DexValue").gameObject.GetComponent<Text>();
            NewHeroCharValue = gameObject.transform.Find("MainPanel").Find("NewHeroPanel").Find("CharImage").Find("CharValue").gameObject.GetComponent<Text>();
            LoadHeroIntValue = gameObject.transform.Find("MainPanel").Find("LoadHeroPanel").Find("IntImage").Find("IntValue").gameObject.GetComponent<Text>();
            LoadHeroStrValue = gameObject.transform.Find("MainPanel").Find("LoadHeroPanel").Find("StrImage").Find("StrValue").gameObject.GetComponent<Text>();
            LoadHeroDexValue = gameObject.transform.Find("MainPanel").Find("LoadHeroPanel").Find("DexImage").Find("DexValue").gameObject.GetComponent<Text>();
            LoadHeroCharValue = gameObject.transform.Find("MainPanel").Find("LoadHeroPanel").Find("CharImage").Find("CharValue").gameObject.GetComponent<Text>();
            ContentWindowLoad = gameObject.transform.Find("MainPanel").Find("LoadGamePanel").Find("LoadGameScrollView").Find("Viewport").Find("LoadGameContent").gameObject;
            ContentWindowNewGame = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("NewGameScrollView").Find("Viewport").Find("NewGameContent").gameObject;
            ContentWindowMods = gameObject.transform.Find("MainPanel").Find("ModsPanel").Find("ModsScrollView").Find("ModsViewport").Find("ModsContent").gameObject;
            ContentWindowMapEditor = gameObject.transform.Find("MainPanel").Find("MapEditorMenuePanel").Find("MapEditorScrollView").Find("Viewport").Find("MapEditorContent").gameObject;
            ContentWindowLoadHero = gameObject.transform.Find("MainPanel").Find("LoadHeroPanel").Find("LoadHeroScrollView").Find("HeroViewport").Find("LoadHeroContent").gameObject;
            ContentWindowNewHero = gameObject.transform.Find("MainPanel").Find("NewHeroPanel").Find("NewHeroScrollView").Find("HeroViewport").Find("NewHeroContent").gameObject;
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
            Team1HeroDropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team1HeroDropdown").gameObject;
            Team2HeroDropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team2HeroDropdown").gameObject;
            Team3HeroDropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team3HeroDropdown").gameObject;
            Team4HeroDropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team4HeroDropdown").gameObject;
            Team5HeroDropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team5HeroDropdown").gameObject;
            Team6HeroDropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team6HeroDropdown").gameObject;
            Team7HeroDropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team7HeroDropdown").gameObject;
            Team8HeroDropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team8HeroDropdown").gameObject;
            Team9HeroDropdown = gameObject.transform.Find("MainPanel").Find("NewGamePanel").Find("TeamPanel").Find("Team9HeroDropdown").gameObject;
            ClassDropDown = gameObject.transform.Find("MainPanel").Find("NewHeroPanel").Find("ClassDropDown").GetComponent<Dropdown>();
        }

        public void MapEditorButtonClicked()
        {
            MainMenuePanel.SetActive(false);
            MapEditorMenuePanel.SetActive(true);
            NewGameMenuePanel.SetActive(false);
            LoadGamePanel.SetActive(false);
            ModPanel.SetActive(false);
            NewHeroPanel.SetActive(false);
            LoadHeroPanel.SetActive(false);
            GetMapsForMapEditorWindow();
        }

        public void MainMenueButtonClicked()
        {
            MainMenuePanel.SetActive(true);
            MapEditorMenuePanel.SetActive(false);
            NewGameMenuePanel.SetActive(false);
            LoadGamePanel.SetActive(false);
            ModPanel.SetActive(false);
            NewHeroPanel.SetActive(false);
            LoadHeroPanel.SetActive(false);
        }

        public void MainMenueButtonClickedFromModScreen()
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
                    foreach (string Mod in ModsList)
                    {
                        DBC.GetTerrianData(Mod);
                        DBC.GetUnitData(Mod);
                        DBC.GetBuildingData(Mod);
                        DBC.ModsLoaded.Add(Mod);
                    }
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
                            MainMenuePanel.SetActive(true);
                            MapEditorMenuePanel.SetActive(false);
                            NewGameMenuePanel.SetActive(false);
                            LoadGamePanel.SetActive(false);
                            ModPanel.SetActive(false);
                            NewHeroPanel.SetActive(false);
                            LoadHeroPanel.SetActive(false);
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
                else
                {
                    MainMenuePanel.SetActive(true);
                    MapEditorMenuePanel.SetActive(false);
                    NewGameMenuePanel.SetActive(false);
                    LoadGamePanel.SetActive(false);
                    ModPanel.SetActive(false);
                    NewHeroPanel.SetActive(false);
                    LoadHeroPanel.SetActive(false);
                }
            }
            else
            {
                ModDescriptionText.text = "Must have at least one mod loaded!";
            }
        }

        public void NewGameButtonClicked()
        {
            MainMenuePanel.SetActive(false);
            MapEditorMenuePanel.SetActive(false);
            NewGameMenuePanel.SetActive(true);
            LoadGamePanel.SetActive(false);
            ModPanel.SetActive(false);
            NewHeroPanel.SetActive(false);
            LoadHeroPanel.SetActive(false);
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

        public void LoadGameButtonClicked()
        {
            MainMenuePanel.SetActive(false);
            MapEditorMenuePanel.SetActive(false);
            NewGameMenuePanel.SetActive(false);
            LoadGamePanel.SetActive(true);
            ModPanel.SetActive(false);
            NewHeroPanel.SetActive(false);
            LoadHeroPanel.SetActive(false);
            GetSaves();
        }

        public void ModButtonClicked()
        {
            MainMenuePanel.SetActive(false);
            MapEditorMenuePanel.SetActive(false);
            NewGameMenuePanel.SetActive(false);
            LoadGamePanel.SetActive(false);
            ModPanel.SetActive(true);
            NewHeroPanel.SetActive(false);
            LoadHeroPanel.SetActive(false);
            GetMods();
        }

        public void QuitGameButtonClicked()
        {
            Master m = JsonUtility.FromJson<Master>(File.ReadAllText(Application.dataPath + "/StreamingAssets/MasterData/Master.json"));
            m.HeroSelectedWhenGameClosed = GCS.HeroCurrentlySelectedP1.Name;
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

        public void NewHeroButtonClicked()
        {
            MainMenuePanel.SetActive(false);
            MapEditorMenuePanel.SetActive(false);
            NewGameMenuePanel.SetActive(false);
            LoadGamePanel.SetActive(false);
            ModPanel.SetActive(false);
            NewHeroPanel.SetActive(true);
            LoadHeroPanel.SetActive(false);
            GetHeroesNew();
            GetHeroClassesNew();
        }

        public void LoadHeroButtonClicked()
        {
            MainMenuePanel.SetActive(false);
            MapEditorMenuePanel.SetActive(false);
            NewGameMenuePanel.SetActive(false);
            LoadGamePanel.SetActive(false);
            ModPanel.SetActive(false);
            NewHeroPanel.SetActive(false);
            LoadHeroPanel.SetActive(true);
            YesDeleteHero.SetActive(false);
            DeleteHeroNeverMind.SetActive(false);
            GetHeroesLoad();
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

        public void GetMapsFornewGameWindow()
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
                    GameObject tempbutton = Instantiate(LoadMenueButtonPrefab, ContentWindowNewGame.transform); //create button and set its parent to content
                    tempbutton.name = Path.GetFileNameWithoutExtension(file); //change name
                    tempbutton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileNameWithoutExtension(file);
                    tempbutton.GetComponent<Button>().onClick.AddListener(MapSelectedNewGame); //adds method to button clicked
                }
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

        public void GetSaves()
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
                    tempbutton.GetComponent<Button>().onClick.AddListener(SaveGameSelected); //adds method to button clicked 
                }
            }
        }

        public void GetMods()
        {
            var childcount = ContentWindowMods.transform.childCount;
            for (int i = 0; i < childcount; i++)
            {
                Destroy(ContentWindowMods.transform.GetChild(i).gameObject);
            }
            ModsInModContentWindow.Clear();
            foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/")))
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

        public void GetHeroesNew()
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

        public void GetHeroClassesNew()
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

        public void GetHeroesForNewGame(GameObject dropGM, int team)
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

        public void GetHeroesLoad()
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
            }
        }

        public void NewHeroSelectedButton()
        {
            NewHeroCurrentlySelected = EventSystem.current.currentSelectedGameObject.transform.GetComponent<ButtonProperties>().ID;
            NewHeroPreview.sprite = DBC.HeroRaceDictionary[NewHeroCurrentlySelected].IconSprite;
            ChangeNewHeroStats();
        }

        public void LoadHeroSelectedButton()
        {
            GCS.HeroCurrentlySelectedP1 = DBC.HeroDictionary[EventSystem.current.currentSelectedGameObject.name];
            LoadHeroPreview.sprite = DBC.HeroRaceDictionary[GCS.HeroCurrentlySelectedP1.RaceID].IconSprite;
            LoadHeroIntValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Intelligance.ToString();
            LoadHeroStrValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Strenght.ToString();
            LoadHeroDexValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Dexterity.ToString();
            LoadHeroCharValue.text = DBC.HeroDictionary[GCS.HeroCurrentlySelectedP1.Name].Charisma.ToString();
            LoadHeroFeedBackText.text = "Current hero selected: " + GCS.HeroCurrentlySelectedP1.Name;
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
                UpdateModsActive();
            }
            else
            {
                ModsList.Add(CurrentlySellectedMod);
                UpdateModsActive();
            }
        }

        public void RemoveModButtonClicked()
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

        public void UpdateModsActive()
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

        public void NewHeroCreateButtonClicked()
        {
            if (!File.Exists(Application.dataPath + "/StreamingAssets/HeroList/" + HeroInputField.text + ".txt"))
            {
                if (HeroInputField.text != "")
                {
                    if (!Regex.IsMatch(HeroInputField.text, @"^[a-z][A-Z]+$"))
                    {
                        GCS.CreateNewHero(NewHeroCurrentlySelected,NewHeroClassCurrentlySelected, HeroInputField.text);
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

        public void DeleteHeroButtonClicked()
        {
            YesDeleteHero.SetActive(true);
            DeleteHeroNeverMind.SetActive(true);
            LoadHeroFeedBackText.text = "You sure???";
        }

        public void YesDelete()
        {
            YesDeleteHero.SetActive(false);
            DeleteHeroNeverMind.SetActive(false);
            File.Delete(Application.dataPath + "/StreamingAssets/HeroList/" + GCS.HeroCurrentlySelectedP1.Name + ".txt");
            DBC.HeroDictionary.Remove(GCS.HeroCurrentlySelectedP1.Name);
            GCS.HeroCurrentlySelectedP1 = null;
            LoadHeroFeedBackText.text = "Current hero selected: None";
            GetHeroesLoad();
        }

        public void Nevermind()
        {
            YesDeleteHero.SetActive(false);
            DeleteHeroNeverMind.SetActive(false);
            LoadHeroFeedBackText.text = "";
        }

        public void ClassDropDownChanged(Dropdown change)
        {
            //Debug.Log(DBC.HeroClassDictionary[change.value].Title);
            NewHeroClassCurrentlySelected = change.value;
            ChangeNewHeroStats();
        }

        public void ChangeNewHeroStats()
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

        public void Team1HeroDropdownController(int index)
        {
            GCS.HeroCurrentlySelectedP1 = DBC.HeroDictionary[Team1HeroDropdown.GetComponent<Dropdown>().options[index].text];
        }

        public void Team2HeroDropdownController(int index)
        {
            GCS.HeroCurrentlySelectedP2 = DBC.HeroDictionary[Team2HeroDropdown.GetComponent<Dropdown>().options[index].text];
        }

        public void Team3HeroDropdownController(int index)
        {
            GCS.HeroCurrentlySelectedP3 = DBC.HeroDictionary[Team3HeroDropdown.GetComponent<Dropdown>().options[index].text];
        }

        public void Team4HeroDropdownController(int index)
        {
            GCS.HeroCurrentlySelectedP4 = DBC.HeroDictionary[Team4HeroDropdown.GetComponent<Dropdown>().options[index].text];
        }

        public void Team5HeroDropdownController(int index)
        {
            GCS.HeroCurrentlySelectedP5 = DBC.HeroDictionary[Team5HeroDropdown.GetComponent<Dropdown>().options[index].text];
        }

        public void Team6HeroDropdownController(int index)
        {
            GCS.HeroCurrentlySelectedP6 = DBC.HeroDictionary[Team6HeroDropdown.GetComponent<Dropdown>().options[index].text];
        }

        public void Team7HeroDropdownController(int index)
        {
            GCS.HeroCurrentlySelectedP7 = DBC.HeroDictionary[Team7HeroDropdown.GetComponent<Dropdown>().options[index].text];
        }

        public void Team8HeroDropdownController(int index)
        {
            GCS.HeroCurrentlySelectedP8 = DBC.HeroDictionary[Team8HeroDropdown.GetComponent<Dropdown>().options[index].text];
        }

        public void Team9HeroDropdownController(int index)
        {
            GCS.HeroCurrentlySelectedP9 = DBC.HeroDictionary[Team9HeroDropdown.GetComponent<Dropdown>().options[index].text];
        }
    }
}
