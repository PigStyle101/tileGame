using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using MoonSharp.Interpreter;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace TileGame
{

    [System.Serializable]
    [MoonSharpUserData]
    public class DatabaseController : MonoBehaviour
    {

        //this is the second main controller script, everything that needs to be loaded into the game from streaming assets folder should be done through here. 

        public static DatabaseController instance = null;

        public Dictionary<int, Terrain> TerrainDictionary = new Dictionary<int, Terrain>(); //stores the classes based on there id
        public Dictionary<int, MouseOverlays> MouseDictionary = new Dictionary<int, MouseOverlays>();
        public Dictionary<int, Unit> UnitDictionary = new Dictionary<int, Unit>();
        public Dictionary<int, Building> BuildingDictionary = new Dictionary<int, Building>();
        public Dictionary<int, FogOfWar> FogOfWarDictionary = new Dictionary<int, FogOfWar>();
        public Dictionary<string, Hero> HeroDictionary = new Dictionary<string, Hero>();
        public Dictionary<int, HeroRace> HeroRaceDictionary = new Dictionary<int, HeroRace>();
        public Dictionary<int, HeroClass> HeroClassDictionary = new Dictionary<int, HeroClass>();
        public Dictionary<int, Spell> SpellDictionary = new Dictionary<int, Spell>();
        public Master MasterData = new Master();
        public List<string> ModsLoaded = new List<string>();
        public Dictionary<string, string> LuaCoreScripts = new Dictionary<string, string>();
        private GameObject NewTile;
        [HideInInspector]
        public bool Initalisation = true;
        public float dragSpeedOffset;
        [HideInInspector]
        public int DragSpeed;
        [HideInInspector]
        public int scrollSpeed;
        private int NextTerrainDicIndex;
        private int NextUnitDicIndex;
        private int NextBuildingIndex;
        private int NextMouseOverlayIndex;
        private int NextFogIndex;
        private int NextHeroRaceIndex;
        private int NextHeroClassIndex;
        private int NextSpellIndex;
        public GameObject UnitHealthOverlay;
        public GameObject BuildingHealthOverlay;
        private GameControllerScript GCS;
        public int LoadState;
        private bool Loading;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            UserData.RegisterAssembly();
            LoadState = 1;
            Loading = true;
            GCS = GameControllerScript.instance;
            StartCoroutine(DataGettier());
            DataGettier();
        }

        private void Update()
        {
            if (Loading)
            {
                switch (LoadState)
                {
                    case 1:
                        GCS.LoadingUpdater(.1f, "Getting Terrain data");
                        break;
                    case 2:
                        GCS.LoadingUpdater(.2f, "Getting Unit data");
                        break;
                    case 3:
                        GCS.LoadingUpdater(.3f, "Getting Hero data");
                        break;
                    case 4:
                        GCS.LoadingUpdater(.4f, "Getting Building data");
                        break;
                    case 5:
                        GCS.LoadingUpdater(.5f, "Trying to understand the data");
                        break;
                    case 6:
                        GCS.LoadingUpdater(.6f, "This is data?");
                        break;
                    case 7:
                        GCS.LoadingUpdater(.8f, "Winging it");
                        break;
                    case 8:
                        GCS.LoadingUpdater(1f, "Anything start smoking?");
                        Loading = false;
                        break;
                }
            }
        }
        [MoonSharpHidden]
        public IEnumerator DataGettier()
        {
            GetLuaCoreScripts();
            GetMasterJson();
            LoadState = 1;
            yield return null;
            ResetNextIndexes();
            GetTerrianData("Core");
            LoadState = 2;
            yield return null;
            GetUnitData("Core");
            LoadState = 3;
            yield return null;
            GetHeroRaceData("Core");
            GetHeroClassData("Core");
            LoadState = 4;
            yield return null;
            GetBuildingData("Core");
            LoadState = 5;
            yield return null;
            ModsLoaded.Add("Core");
            LoadState = 6;
            yield return null;
            GetMouseJson();
            LoadState = 7;
            yield return null;
            GetFogOfWarJson();
            LoadState = 8;
        }
        [MoonSharpHidden]
        /// <summary>
        /// Gets Master Json file, this is used for any global game settings
        /// </summary>
        public void GetMasterJson()
        {
            //Debug.log("Fetching json master file");
            var grassstring = File.ReadAllText(Application.dataPath + "/StreamingAssets/MasterData/Master.json"); //temp string to hold the json data
            MasterData = JsonUtility.FromJson<Master>(grassstring); //this converts from json string to unity object
            scrollSpeed = MasterData.ScrollSpeed;
            DragSpeed = MasterData.DragSpeed;
            //Debug.Log("Finished fetching json master file");
        }
        [MoonSharpHidden]
        /// <summary>
        /// Gets Terrain Json files. Pulls from data first then searches for sprites.
        /// </summary>
        /// <param name="Mod">Mod folder name to look for</param>
        public void GetTerrianData(string Mod)
        {
            if (Directory.Exists(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Terrain"))
            {
                //Debug.log("Fetching json terrain files");
                foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Terrain/Data/", "*.json")) //gets only json files form this path
                {
                    var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                    Terrain t = JsonUtility.FromJson<Terrain>(Tempstring);
                    t.ID = NextTerrainDicIndex;
                    t.GetSprites();
                    TerrainDictionary.Add(t.ID, t);
                    NextTerrainDicIndex += 1;
                }
            }
        } //not done
        [MoonSharpHidden]
        /// <summary>
        /// Gets Unit Json files. Pulls from data first then searches for sprites.
        /// </summary>
        /// <param name="Mod">Mod folder name to look for</param>
        public void GetUnitData(string Mod)
        {
            if (Directory.Exists(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units"))
            {
                //Debug.log("Fetching json unit files");
                foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units/Data/", "*.json")) //gets only json files form this path
                {
                    var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                    Unit u = JsonUtility.FromJson<Unit>(Tempstring);
                    u.ID = NextUnitDicIndex;
                    u.GetSprites();
                    UnitDictionary.Add(u.ID, u);
                    NextUnitDicIndex += 1;
                }
            }
        } //done
        [MoonSharpHidden]
        /// <summary>
        /// Gets Hero Json files. Pulls from data first then searches for sprites.
        /// </summary>
        /// <param name="Mod">Mod folder name to look for</param>
        public void GetHeroRaceData(string Mod)
        {
            if (Directory.Exists(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes/RaceData"))
            {
                //Debug.Log("Fetching json hero files");
                foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes/RaceData/", "*.json")) //gets only lua files form this path
                {
                    var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                    HeroRace hr = JsonUtility.FromJson<HeroRace>(Tempstring);
                    hr.ID = NextHeroRaceIndex;
                    hr.GetSprites();
                    HeroRaceDictionary.Add(hr.ID, hr);
                    NextHeroRaceIndex += 1;
                }
            }
        } //not done

        public void GetHeroClassData(string Mod)
        {
            if (Directory.Exists(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes/ClassData"))
            {
                //Debug.Log("Fetching json hero files");
                foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes/ClassData/", "*.json")) //gets only lua files form this path
                {
                    var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                    HeroClass hc = JsonUtility.FromJson<HeroClass>(Tempstring);
                    hc.ID = NextHeroClassIndex;
                    HeroClassDictionary.Add(hc.ID, hc);
                    NextHeroClassIndex += 1;
                }
            }
        }

        [MoonSharpHidden]
        public void GetSpellData(string Mod)
        {
            if (Directory.Exists(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Spells/SpellData"))
            {
                //Debug.Log("Fetching json hero files");
                foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Spells/SpellData/", "*.json")) //gets only json files form this path
                {
                    var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                    Spell s = JsonUtility.FromJson<Spell>(Tempstring);
                    s.ID = NextSpellIndex;
                    s.GetSprites();
                    SpellDictionary.Add(s.ID, s);
                    NextSpellIndex += 1;
                }
            }
        } //not done
          //[MoonSharpHidden]
        /// <summary>
        /// Gets Building Json files. Pulls from data first then searches for sprites.
        /// </summary>
        /// <param name="Mod">Mod folder name to look for</param>
        public void GetBuildingData(string Mod)
        {
            if (Directory.Exists(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Buildings"))
            {
                //Debug.log("Fetching json building files");
                foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Buildings/Data/", "*.json")) //gets only lua files form this path
                {
                    var Tempstring = File.ReadAllText(file); //temp string to hold the json data
                    Building b = JsonUtility.FromJson<Building>(Tempstring);
                    b.ID = NextBuildingIndex;
                    b.GetSprites();
                    BuildingDictionary.Add(b.ID, b);
                    NextBuildingIndex += 1;
                }
            }
        } //done
        [MoonSharpHidden]
        /// <summary>
        /// Used to get mouse overlays for hovering over terrain tiles
        /// </summary>
        public void GetMouseJson()
        {
            //Debug.log("Fetching json mouse overlay files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/MouseOverlays/Data/", "*.json"))
            {
                var TempString = File.ReadAllText(file);
                var tempjson = JsonUtility.FromJson<MouseOverlays>(TempString);
                //Debug.Log("Adding: " + tempjson.Slug + " to database");
                tempjson.ID = NextMouseOverlayIndex;
                MouseDictionary.Add(tempjson.ID, tempjson);
                tempjson.GetSprites();
                NextMouseOverlayIndex = NextMouseOverlayIndex + 1;
            }
        }//same as getTerrainJson
        [MoonSharpHidden]
        public void GetFogOfWarJson()
        {
            //Debug.log("Fetching json mouse overlay files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/FogOfWar/Data/", "*.json"))
            {
                var TempString = File.ReadAllText(file);
                var tempjson = JsonUtility.FromJson<FogOfWar>(TempString);
                // Debug.Log("Adding: " + tempjson.Slug + " to database");
                tempjson.ID = NextFogIndex;
                FogOfWarDictionary.Add(tempjson.ID, tempjson);
                tempjson.GetSprites();
                NextFogIndex = NextFogIndex + 1;
            }
        }

        public void GetLuaCoreScripts()
        {
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/LuaCoreScripts/", "*.lua"))
            {
                LuaCoreScripts.Add(Path.GetFileNameWithoutExtension(file), file);
            }
        }

        [MoonSharpHidden]
        /// <summary>
        /// Loads a sprite
        /// </summary>
        /// <param name="FilePath">Location to get file from</param>
        /// <returns></returns>
        public Sprite loadSprite(string FilePath, int PPU)
        {
            Texture2D Tex2D;
            Sprite TempSprite;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(64, 64); // Create new "empty" texture, this requires the size to be added, it get changed in teh next method
                Tex2D.LoadImage(FileData);// Load the imagedata into the texture (size is set automatically)
                TempSprite = Sprite.Create(Tex2D, new Rect(0, 0, Tex2D.width, Tex2D.height), new Vector2(0.5f, 0.5f), PPU);
                return TempSprite;
            }
            return null;
        }// checks for images that are to be used for artwork stuffs
        [MoonSharpHidden]
        public void ResetNextIndexes()
        {
            NextBuildingIndex = 0;
            NextFogIndex = 0;
            NextHeroClassIndex = 0;
            NextHeroRaceIndex = 0;
            NextMouseOverlayIndex = 0;
            NextTerrainDicIndex = 0;
            NextUnitDicIndex = 0;
            NextSpellIndex = 0;
        }

        /// <summary>
        /// Creates a building and adds all needed components.
        /// </summary>
        /// <param name="location">Location to spawn unit at</param>
        /// <param name="index">Unit index in unit dictionary</param>
        /// <param name="team">Team to assign to this unit</param>
        /// <returns></returns>
        public GameObject CreateAndSpawnBuilding(Vector2 location, int index, int team)
        {
            GameObject TGO = new GameObject();
            TGO.name = instance.BuildingDictionary[index].Title;
            TGO.AddComponent<SpriteRenderer>();
            TGO.GetComponent<SpriteRenderer>().sprite = DatabaseController.instance.BuildingDictionary[index].ArtworkDirectory[0];
            TGO.GetComponent<SpriteRenderer>().sortingLayerName = DatabaseController.instance.BuildingDictionary[index].Type;
            TGO.AddComponent<BoxCollider>();
            TGO.GetComponent<BoxCollider>().size = new Vector3(.95f, .95f, .1f);
            TGO.AddComponent<BuildingController>();
            TGO.GetComponent<BuildingController>().Team = team;
            TGO.GetComponent<BuildingController>().Mod = DatabaseController.instance.BuildingDictionary[index].Mod;
            TGO.GetComponent<BuildingController>().Health = DatabaseController.instance.BuildingDictionary[index].Health;
            TGO.GetComponent<BuildingController>().MaxHealth = DatabaseController.instance.BuildingDictionary[index].Health;
            TGO.GetComponent<BuildingController>().DefenceBonus = DatabaseController.instance.BuildingDictionary[index].DefenceBonus;
            TGO.GetComponent<BuildingController>().ID = index;
            TGO.GetComponent<BuildingController>().CanBuildUnits = DatabaseController.instance.BuildingDictionary[index].CanBuildUnits;
            TGO.GetComponent<BuildingController>().BuildableUnits = DatabaseController.instance.BuildingDictionary[index].BuildableUnits;
            TGO.GetComponent<BuildingController>().ID = index;
            TGO.tag = DatabaseController.instance.BuildingDictionary[index].Type;
            TGO.transform.position = location;
            GameObject TempCan = Instantiate(DatabaseController.instance.BuildingHealthOverlay, TGO.transform);
            TempCan.transform.localPosition = new Vector3(0, 0, 0);
            TempCan.GetComponentInChildren<Text>().text = DatabaseController.instance.BuildingDictionary[index].Health.ToString();
            TempCan.GetComponent<Canvas>().sortingLayerName = DatabaseController.instance.UnitDictionary[0].Type;
            return TGO;
        }

        /// <summary>
        /// Creates a terrain tile and adds all needed components.
        /// </summary>
        /// <param name="location">Location to spawn terrain tile at</param>
        /// <param name="index">Tile index in Terrain Dictionary</param>
        /// <returns></returns>
        public GameObject CreateAndSpawnTerrain(Vector2 location, int index)
        {
            GameObject TGO = new GameObject();                                                                                  //create gameobject
            TGO.name = DatabaseController.instance.TerrainDictionary[index].Title;                                                                          //change the name
            TGO.AddComponent<SpriteRenderer>();                                                                                 //add a sprite controller
            TGO.GetComponent<SpriteRenderer>().sprite = DatabaseController.instance.TerrainDictionary[index].ArtworkDirectory[0];               //set the sprite to the texture
            TGO.GetComponent<SpriteRenderer>().sortingLayerName = DatabaseController.instance.TerrainDictionary[index].Type;
            TGO.AddComponent<BoxCollider>();
            TGO.GetComponent<BoxCollider>().size = new Vector3(.95f, .95f, .1f);
            TGO.tag = (DatabaseController.instance.TerrainDictionary[index].Type);
            //TGO.AddComponent<RectTransform>();

            GameObject MouseOverlayGO = new GameObject();                                                                       //creating the child object for mouse overlay
            MouseOverlayGO.AddComponent<SpriteRenderer>().sprite = DatabaseController.instance.MouseDictionary[0].ArtworkDirectory[0];          //adding it to sprite
            MouseOverlayGO.GetComponent<SpriteRenderer>().sortingLayerName = DatabaseController.instance.MouseDictionary[0].SortingLayer;
            MouseOverlayGO.GetComponent<SpriteRenderer>().sortingOrder = 4;                                                     //making it so its on top of the default sprite
            MouseOverlayGO.transform.parent = TGO.transform;                                                                    //setting its parent to the main game object
            MouseOverlayGO.name = "MouseOverlay";                                                                               //changing the name

            GameObject FogOverlay = new GameObject();
            FogOverlay.AddComponent<SpriteRenderer>().sprite = DatabaseController.instance.FogOfWarDictionary[0].ArtworkDirectory[0];
            FogOverlay.GetComponent<SpriteRenderer>().sortingLayerName = DatabaseController.instance.FogOfWarDictionary[0].SortingLayer;
            FogOverlay.GetComponent<SpriteRenderer>().sortingOrder = 3;
            FogOverlay.transform.parent = TGO.transform;
            FogOverlay.name = "FogOfWar";

            GameObject MouseOverlaySelected = new GameObject();
            MouseOverlaySelected.AddComponent<SpriteRenderer>().sprite = DatabaseController.instance.MouseDictionary[1].ArtworkDirectory[0];
            MouseOverlaySelected.GetComponent<SpriteRenderer>().sortingLayerName = DatabaseController.instance.MouseDictionary[1].SortingLayer;
            MouseOverlaySelected.GetComponent<SpriteRenderer>().sortingOrder = 5;
            MouseOverlaySelected.transform.parent = TGO.transform;
            MouseOverlaySelected.name = "MouseOverlaySelected";

            TGO.AddComponent<TerrainController>();                                                                               //adding the sprite controller script to it
            TGO.GetComponent<TerrainController>().Weight = DatabaseController.instance.TerrainDictionary[index].Weight;
            TGO.GetComponent<TerrainController>().Walkable = DatabaseController.instance.TerrainDictionary[index].Walkable;
            TGO.GetComponent<TerrainController>().DefenceBonus = DatabaseController.instance.TerrainDictionary[index].DefenceBonus;
            TGO.GetComponent<TerrainController>().BlocksSight = DatabaseController.instance.TerrainDictionary[index].BlocksSight;
            TGO.GetComponent<TerrainController>().Overlays = DatabaseController.instance.TerrainDictionary[index].Overlays;
            TGO.GetComponent<TerrainController>().Connectable = DatabaseController.instance.TerrainDictionary[index].Connectable;
            TGO.GetComponent<TerrainController>().IdleAnimations = DatabaseController.instance.TerrainDictionary[index].IdleAnimations;
            TGO.GetComponent<TerrainController>().ID = index;
            if (DatabaseController.instance.TerrainDictionary[index].IdleAnimations)
            {
                TGO.GetComponent<TerrainController>().IdleAnimationsDirectory = DatabaseController.instance.TerrainDictionary[index].IdleAnimationDirectory;
            }
            TGO.transform.position = location;
            return TGO;
        } //used to spawn terrian from database

        /// <summary>
        /// Creates a unit and adds all needed components.
        /// </summary>
        /// <param name="location">Location to spawn unit at</param>
        /// <param name="index">Unit index in unit dictionary</param>
        /// <param name="team">Team to assign to this unit</param>
        /// <returns></returns>
        public GameObject CreateAndSpawnUnit(Vector2 location, int index, int team)
        {
            GameObject TGO = new GameObject();
            TGO.name = UnitDictionary[index].Title;
            TGO.AddComponent<SpriteRenderer>();
            TGO.GetComponent<SpriteRenderer>().sprite = UnitDictionary[index].IconSprite;
            TGO.GetComponent<SpriteRenderer>().sortingLayerName = UnitDictionary[index].Type;
            TGO.AddComponent<BoxCollider>();
            TGO.GetComponent<BoxCollider>().size = new Vector3(.95f, .95f, .1f);
            TGO.AddComponent<UnitController>();
            TGO.GetComponent<UnitController>().Team = team;
            TGO.GetComponent<UnitController>().MovePoints = UnitDictionary[index].MovePoints;
            TGO.GetComponent<UnitController>().Attack = UnitDictionary[index].Attack;
            TGO.GetComponent<UnitController>().Defence = UnitDictionary[index].Defence;
            TGO.GetComponent<UnitController>().MaxHealth = UnitDictionary[index].Health;
            TGO.GetComponent<UnitController>().Health = UnitDictionary[index].Health;
            TGO.GetComponent<UnitController>().AttackRange = UnitDictionary[index].AttackRange;
            TGO.GetComponent<UnitController>().CanConvert = UnitDictionary[index].CanConvert;
            TGO.GetComponent<UnitController>().SightRange = UnitDictionary[index].SightRange;
            TGO.GetComponent<UnitController>().UnitIdleAnimation = UnitDictionary[index].IdleAnimations;
            TGO.GetComponent<UnitController>().UnitAttackAnimation = UnitDictionary[index].AttackAnimations;
            TGO.GetComponent<UnitController>().UnitHurtAnimation = UnitDictionary[index].HurtAnimations;
            TGO.GetComponent<UnitController>().UnitMoveAnimation = UnitDictionary[index].MoveAnimations;
            TGO.GetComponent<UnitController>().UnitDiedAnimation = UnitDictionary[index].DiedAnimations;
            TGO.GetComponent<UnitController>().MoveAnimationSpeed = UnitDictionary[index].MoveAnimationSpeed;
            TGO.GetComponent<UnitController>().MoveAnimationOffset = UnitDictionary[index].MoveAnimationOffset;
            TGO.GetComponent<UnitController>().Position = location;
            TGO.GetComponent<UnitController>().ID = index;
            if (UnitDictionary[index].IdleAnimations)
            {
                TGO.GetComponent<UnitController>().IdleAnimationSpeed = UnitDictionary[index].IdleAnimationSpeed;
            }
            if (UnitDictionary[index].AttackAnimations)
            {
                TGO.GetComponent<UnitController>().AttackAnimationSpeed = UnitDictionary[index].AttackAnimationSpeed;
            }
            if (UnitDictionary[index].HurtAnimations)
            {
                TGO.GetComponent<UnitController>().HurtAnimationSpeed = UnitDictionary[index].HurtAnimationSpeed;
            }
            if (UnitDictionary[index].DiedAnimations)
            {
                TGO.GetComponent<UnitController>().DiedAnimationSpeed = UnitDictionary[index].DiedAnimationSpeed;
            }
            if (UnitDictionary[index].CanConvert)
            {
                TGO.GetComponent<UnitController>().ConversionSpeed = UnitDictionary[index].ConversionSpeed;
            }
            TGO.tag = UnitDictionary[index].Type;
            TGO.transform.position = location;

            GameObject TempCan = Instantiate(UnitHealthOverlay, TGO.transform);
            TempCan.transform.localPosition = new Vector3(0, 0, 0);
            TempCan.GetComponentInChildren<Text>().text = UnitDictionary[index].Health.ToString();
            TempCan.GetComponent<Canvas>().sortingLayerName = UnitDictionary[index].Type;
            int CaseInt = TGO.GetComponent<UnitController>().Team;
            switch (CaseInt)
            {
                case 1:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.black;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.white;
                    break;
                case 2:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.blue;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 3:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.cyan;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 4:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.gray;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 5:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.green;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 6:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.magenta;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 7:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.red;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 8:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.white;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 9:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.yellow;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
            }

            TGO.GetComponent<UnitController>().UnitMovable = false;
            TGO.GetComponent<UnitController>().UnitMoved = true;
            TGO.GetComponent<UnitController>().CanMoveAndAttack = UnitDictionary[index].CanMoveAndAttack;
            //GCS.AddUnitsToDictionary(TGO);
            TGO.GetComponent<UnitController>().TeamSpriteController();
            return TGO;
        } //used to spawn units form database

        public GameObject CreateAndSpawnHero(Vector2 location, string Hname, int team)
        {
            GameControllerScript GCS = GameControllerScript.instance;
            GameObject TGO = new GameObject();
            Hero thero = HeroDictionary[Hname];
            TGO.name = Hname;
            TGO.AddComponent<SpriteRenderer>();
            TGO.GetComponent<SpriteRenderer>().sprite = HeroRaceDictionary[thero.RaceID].IconSprite;
            TGO.GetComponent<SpriteRenderer>().sortingLayerName = UnitDictionary[thero.RaceID].Type;
            TGO.AddComponent<BoxCollider>();
            TGO.GetComponent<BoxCollider>().size = new Vector3(.95f, .95f, .1f);
            TGO.AddComponent<UnitController>();
            TGO.GetComponent<UnitController>().Hero = true;
            TGO.GetComponent<UnitController>().Team = team;
            TGO.GetComponent<UnitController>().Health = HeroDictionary[thero.Name].MaxHealth;
            TGO.GetComponent<UnitController>().AttackRange = HeroRaceDictionary[thero.RaceID].AttackRange;
            TGO.GetComponent<UnitController>().CanConvert = HeroRaceDictionary[thero.RaceID].CanConvert;
            TGO.GetComponent<UnitController>().SightRange = HeroRaceDictionary[thero.RaceID].SightRange;
            TGO.GetComponent<UnitController>().HClass = HeroDictionary[Hname];
            TGO.GetComponent<UnitController>().HClass.Health = HeroDictionary[thero.Name].MaxHealth;
            TGO.GetComponent<UnitController>().Level = HeroDictionary[Hname].Level;
            TGO.GetComponent<UnitController>().XP = HeroDictionary[Hname].XP;
            TGO.GetComponent<UnitController>().UnitIdleAnimation = HeroRaceDictionary[thero.RaceID].IdleAnimations;
            TGO.GetComponent<UnitController>().UnitAttackAnimation = HeroRaceDictionary[thero.RaceID].AttackAnimations;
            TGO.GetComponent<UnitController>().UnitHurtAnimation = HeroRaceDictionary[thero.RaceID].HurtAnimations;
            TGO.GetComponent<UnitController>().UnitMoveAnimation = HeroRaceDictionary[thero.RaceID].MoveAnimations;
            TGO.GetComponent<UnitController>().UnitDiedAnimation = HeroRaceDictionary[thero.RaceID].DiedAnimations;
            TGO.GetComponent<UnitController>().ID = thero.RaceID;
            TGO.GetComponent<UnitController>().UpdateHeroStats();
            TGO.GetComponent<UnitController>().Position = location;
            if (HeroRaceDictionary[thero.RaceID].IdleAnimations)
            {
                TGO.GetComponent<UnitController>().IdleAnimationSpeed = HeroRaceDictionary[thero.RaceID].IdleAnimationSpeed;
            }
            if (HeroRaceDictionary[thero.RaceID].AttackAnimations)
            {
                TGO.GetComponent<UnitController>().AttackAnimationSpeed = HeroRaceDictionary[thero.RaceID].AttackAnimationSpeed;
            }
            if (HeroRaceDictionary[thero.RaceID].HurtAnimations)
            {
                TGO.GetComponent<UnitController>().HurtAnimationSpeed = HeroRaceDictionary[thero.RaceID].HurtAnimationSpeed;
            }
            if (HeroRaceDictionary[thero.RaceID].DiedAnimations)
            {
                TGO.GetComponent<UnitController>().DiedAnimationSpeed = HeroRaceDictionary[thero.RaceID].DiedAnimationSpeed;
            }
            if (HeroRaceDictionary[thero.RaceID].MoveAnimations)
            {
                TGO.GetComponent<UnitController>().MoveAnimationSpeed = HeroRaceDictionary[thero.RaceID].MoveAnimationSpeed;
                TGO.GetComponent<UnitController>().MoveAnimationOffset = HeroRaceDictionary[thero.RaceID].MoveAnimationOffset;
            }
            if (HeroRaceDictionary[thero.RaceID].CanConvert)
            {
                TGO.GetComponent<UnitController>().ConversionSpeed = HeroRaceDictionary[thero.RaceID].ConversionSpeed;
            }
            TGO.tag = HeroRaceDictionary[thero.RaceID].Type;
            TGO.transform.position = location;

            GameObject TempCan = Instantiate(UnitHealthOverlay, TGO.transform);
            TempCan.transform.localPosition = new Vector3(0, 0, 0);
            TempCan.GetComponentInChildren<Text>().text = HeroDictionary[thero.Name].Health.ToString();
            TempCan.GetComponent<Canvas>().sortingLayerName = UnitDictionary[0].Type;
            int CaseInt = TGO.GetComponent<UnitController>().Team;
            switch (CaseInt)
            {
                case 1:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.black;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.white;
                    break;
                case 2:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.blue;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 3:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.cyan;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 4:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.gray;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 5:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.green;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 6:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.magenta;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 7:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.red;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 8:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.white;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
                case 9:
                    TempCan.transform.Find("Image").GetComponent<Image>().color = Color.yellow;
                    TempCan.transform.Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                    break;
            }

            TGO.GetComponent<UnitController>().UnitMovable = false;
            TGO.GetComponent<UnitController>().UnitMoved = true;
            TGO.GetComponent<UnitController>().CanMoveAndAttack = HeroRaceDictionary[thero.RaceID].CanMoveAndAttack;
            //GCS.AddUnitsToDictionary(TGO);
            TGO.GetComponent<UnitController>().TeamSpriteController();
            return TGO;
        } //used to spawn units form database

        /// <summary>
        /// Saves Hero data by serializing it to binary and then savinf it to text file
        /// </summary>
        /// <param name="h">Hero to be saved</param>
        public void SaveHeroToFile(Hero h)
        {
            if (!File.Exists(Application.dataPath + "/StreamingAssets/HeroList/" + h.Name + ".json"))
            {
                FileStream fs = new FileStream(Application.dataPath + "/StreamingAssets/HeroList/" + h.Name + ".txt", FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, h);
                fs.Flush();
                fs.Close();
            }
        }

        /// <summary>
        /// Loads all the heros from file, deserializes them and then adds them to dictionary
        /// </summary>
        public void LoadHerosFromFile()
        {
            foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/HeroList/", "*.txt")))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(file,FileMode.Open);
                Hero h = (Hero)bf.Deserialize(fs);
                fs.Flush();
                fs.Close();
                HeroDictionary.Add(h.Name, h);
                GCS.HeroCurrentlySelectedP1 = h;
            }
        }
    }

    [MoonSharpUserData]
    public class Terrain
    {
        public int ID; //used as dictionary referance
        public string Mod;
        public string Title;
        public string Type;
        public bool Walkable;
        public string Description;
        public int DefenceBonus;
        public int Weight;
        public int PixelsPerUnit;
        public bool BlocksSight;
        public bool Overlays;
        public bool Connectable;
        public bool IdleAnimations;
        public List<Sprite> ArtworkDirectory = new List<Sprite>();
        public List<Sprite> OverlayArtworkDirectory = new List<Sprite>();
        public List<Sprite> IdleAnimationDirectory = new List<Sprite>();

        /// <summary>
        /// Gets location of sprites and saves them to a list
        /// </summary>
        public void GetSprites()
        {
            //Debug.Log("Getting sprites for: " + Title);
            int count = new int(); //used for debug
            foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Terrain/Sprites/" + Title + "/", "*.png"))) //could actaully put all png and json in same folder... idea for later
            {
                if (file.Contains(Title) && file.Contains("Overlay")) //checks if any of the files found contains the right words
                {
                    OverlayArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                }
                else if (file.Contains(Title) && file.Contains("Idle"))
                {
                    IdleAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                }
                else if (file.Contains(Title))
                {
                    ArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit)); //adds if they do
                    count = count + 1;
                }
                //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
            }
            //Debug.Log("Sprites found for " + Title + ": " + count + " in mod " + Mod);
            if (Title == "Water")
            {
                //Debug.Log("Overlays Contains:" + OverlayArtworkDirectory.Count);
                //Debug.Log("Idle Contains:" + IdleAnimationDirectory.Count);
                //Debug.Log("Artwork Contains:" + ArtworkDirectory.Count);
            }
            count = 0;
        }  //checks how many sprites are in folder and adds them to the directory
    }

    [MoonSharpUserData]
    public class Unit
    {
        public int ID;
        public string Mod;
        public int Attack;
        public int Defence;
        public int AttackRange;
        public int Health;
        public int MovePoints;
        public int Cost;
        public string Title;
        public string Description;
        public string Slug;
        public string Type;
        public bool IdleAnimations;
        public bool AttackAnimations;
        public bool HurtAnimations;
        public bool DiedAnimations;
        public bool MoveAnimations;
        public Sprite IconSprite;
        public List<Sprite> IdleAnimationDirectory = new List<Sprite>();
        public List<Sprite> AttackAnimationDirectory = new List<Sprite>();
        public List<Sprite> HurtAnimationDirectory = new List<Sprite>();
        public List<Sprite> DiedAnimationDirectory = new List<Sprite>();
        public List<Sprite> MoveAnimationDirectory = new List<Sprite>();
        public float IdleAnimationSpeed;
        public float AttackAnimationSpeed;
        public float HurtAnimationSpeed;
        public float DiedAnimationSpeed;
        public float MoveAnimationSpeed; //controls how fast the animation plays
        public float MoveAnimationOffset; //controls how long the moving animation plays for
        public int Team;
        public int ConversionSpeed;
        public int PixelsPerUnit;
        public bool CanConvert;
        public bool CanMoveAndAttack;
        public int SightRange;

        /// <summary>
        /// Gets location of sprites and saves them to a list
        /// </summary>
        public void GetSprites()
        {
            //Debug.Log("Getting sprites for: " + Title);
            int count = new int();
            if (IdleAnimations)
            {
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units/Sprites/" + Title + "/Idle/", "*.png")))
                {
                    IdleAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                    //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
                }
            }
            if (AttackAnimations)
            {
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units/Sprites/" + Title + "/Attack/", "*.png")))
                {
                    AttackAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                    //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
                }
            }
            if (HurtAnimations)
            {
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units/Sprites/" + Title + "/Hurt/", "*.png")))
                {
                    HurtAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                    //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
                }
            }
            if (DiedAnimations)
            {
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units/Sprites/" + Title + "/Died/", "*.png")))
                {
                    DiedAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                    //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
                }
            }
            if (MoveAnimations)
            {
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units/Sprites/" + Title + "/Move/", "*.png")))
                {
                    MoveAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                    //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
                }
            }
            //Debug.Log("Sprites found for " + Title + ": " + count + " in mod " + Mod);
            count = 0;

            foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Units/Sprites/" + Title + "/", "*.png")))
            {
                IconSprite = DatabaseController.instance.loadSprite(file, PixelsPerUnit);
                count = count + 1;
            } //this is used to get the default sprite for icons and stuff, only needs one.
        }
    }//same use as terrian class

    [MoonSharpUserData]
    public class Building
    {
        public int ID;
        public string Mod;
        public List<string> BuildableUnits;
        public string Title;
        public string Description;
        public string Slug;
        public int DefenceBonus;
        public string Type;
        public List<Sprite> ArtworkDirectory = new List<Sprite>();
        public int Team;
        public int Health;
        public int PixelsPerUnit;
        public bool Walkable;
        public bool Capturable;
        public bool CanBuildUnits;
        public bool BlocksSight;
        public bool OnlyOneAllowed;
        public bool HeroSpawnPoint;
        public bool MainBase;

        /// <summary>
        /// Gets location of sprites and saves them to a list
        /// </summary>
        public void GetSprites()
        {
            //Debug.log("Getting sprites for: " + Title);
            int count = new int();
            foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Buildings/Sprites/" + Title + "/", "*.png")))
            {
                if (file.Contains(Title))
                {
                    ArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                }
                //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
            }
            //Debug.Log("Sprites found for " + Title + ": " + count + " in mod " + Mod);
            count = 0;
        }
    }//same use as terrian class

    public class MouseOverlays
    {
        public int ID;
        public string Title;
        public string Slug;
        public string Type;
        public string SortingLayer = "Unit";
        public int PixelsPerUnit;
        public List<Sprite> ArtworkDirectory;

        /// <summary>
        /// Gets location of sprites and saves them to a list
        /// </summary>
        public void GetSprites()
        {
            //Debug.log("Getting sprites for: " + Title);
            int count = new int();
            foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/MouseOverlays/Sprites", "*.png")))
            {
                if (file.Contains(Title))
                {
                    ArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                }
                //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
            }
            //Debug.Log("Sprites found for MO: " + count);
            count = 0;
        }
    }//same use as terrian class

    public class FogOfWar
    {
        public int ID;
        public string Title;
        public string Slug;
        public string SortingLayer = "Unit";
        public int PixelsPerUnit;
        public List<Sprite> ArtworkDirectory;

        /// <summary>
        /// Gets location of sprites and saves them to a list
        /// </summary>
        public void GetSprites()
        {
            //Debug.log("Getting sprites for: " + Title);
            int count = new int();
            foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/FogOfWar/Sprites", "*.png")))
            {
                if (file.Contains(Title))
                {
                    ArtworkDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                }
                //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
            }
            // Debug.Log("Sprites found for FogOfWar: " + count);
            count = 0;
        }
    }//same use as terrian class

    [Serializable]
    public class Master
    {
        public int DragSpeed;
        public int ScrollSpeed;
        public string HeroSelectedWhenGameClosed;
    }

    [Serializable]
    public class Hero
    {
        //Populated by GetHeroesJson();
        public int RaceID;
        public int ClassID;
        //Populated by CreateNewHero();
        public string Name;
        //Populated by CreateNewHero(), and then updated when the hero lvls.
        public int MaxHealth;
        public int Intelligance;
        public int Strenght;
        public int Dexterity;
        public int Charisma;
        //Populated from json
        public string Title;
        public string Slug;
        public string Description;
        public string Type;
        public string Mod;
        public int PixelsPerUnit;
        public int Attack;
        public int Defence;
        public int Health;
        public int AttackRange;
        public int MovePoints;
        public int ConversionSpeed;
        public int SightRange;
        public int Mana;
        public int HealthRegen;
        public int ManaRegen;
        public bool CanConvert;
        public bool CanMoveAndAttack;
        public int BaseIntelligance;
        public int BaseStrenght;
        public int BaseDexterity;
        public int BaseCharisma;
        public bool IdleAnimations;
        public bool AttackAnimations;
        public bool HurtAnimations;
        public bool DiedAnimations;
        public bool MoveAnimations;
        public float IdleAnimationSpeed;
        public float AttackAnimationSpeed;
        public float HurtAnimationSpeed;
        public float DiedAnimationSpeed;
        public float MoveAnimationSpeed;
        public float MoveAnimationOffset;
        //Blank tell used in game
        public int Team;
        public int Level;
        public int XP;
    }

    [MoonSharpUserData]
    public class HeroRace
    {
        //Populated by GetHeroesJson();
        public int ID;
        //Populated by CreateNewHero();
        public string Name;
        //Populated by CreateNewHero(), and then updated when the hero lvls.
        public int MaxHealth;
        public int Intelligance;
        public int Strenght;
        public int Dexterity;
        public int Charisma;
        //Populated from json
        public string Title;
        public string Slug;
        public string Description;
        public string Type;
        public string Mod;
        public int PixelsPerUnit;
        public int Attack;
        public int Defence;
        public int Health;
        public int AttackRange;
        public int MovePoints;
        public int ConversionSpeed;
        public int SightRange;
        public int Mana;
        public int HealthRegen;
        public int ManaRegen;
        public bool CanConvert;
        public bool CanMoveAndAttack;
        public bool IdleAnimations;
        public bool AttackAnimations;
        public bool HurtAnimations;
        public bool DiedAnimations;
        public bool MoveAnimations;
        public Sprite IconSprite;
        public List<Sprite> IdleAnimationDirectory = new List<Sprite>();
        public List<Sprite> AttackAnimationDirectory = new List<Sprite>();
        public List<Sprite> HurtAnimationDirectory = new List<Sprite>();
        public List<Sprite> DiedAnimationDirectory = new List<Sprite>();
        public List<Sprite> MoveAnimationDirectory = new List<Sprite>();
        public float IdleAnimationSpeed;
        public float AttackAnimationSpeed;
        public float HurtAnimationSpeed;
        public float DiedAnimationSpeed;
        public float MoveAnimationSpeed;
        public float MoveAnimationOffset;
        //Blank tell used in game
        public int Team;
        public int Level;
        public int XP;

        /// <summary>
        /// Gets location of sprites and saves them to a list
        /// </summary>
        public void GetSprites()
        {
            //Debug.log("Getting sprites for: " + Title);
            int count = new int();
            if (IdleAnimations)
            {
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes/Sprites/" + Title + "/Idle/", "*.png")))
                {
                    IdleAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                    //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
                }
            }
            if (AttackAnimations)
            {
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes/Sprites/" + Title + "/Attack/", "*.png")))
                {
                    AttackAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                    //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
                }
            }
            if (HurtAnimations)
            {
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes/Sprites/" + Title + "/Hurt/", "*.png")))
                {
                    HurtAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                    //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
                }
            }
            if (DiedAnimations)
            {
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes/Sprites/" + Title + "/Died/", "*.png")))
                {
                    DiedAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                    //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
                }
            }
            if (MoveAnimations)
            {
                foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/" + Mod + "/Heroes/Sprites/" + Title + "/Move/", "*.png")))
                {
                    MoveAnimationDirectory.Add(DatabaseController.instance.loadSprite(file, PixelsPerUnit));
                    count = count + 1;
                    //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
                }
            }
            foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/Core/Heroes/Sprites/" + Title + "/", "*.png")))
            {
                if (file.Contains(Title))
                {
                    IconSprite = DatabaseController.instance.loadSprite(file, PixelsPerUnit);
                    count = count + 1;
                }
                //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
            }
            //Debug.Log("Sprites found: " + count);
            count = 0;
        }
    }

    [MoonSharpUserData]
    public class HeroClass
    {
        public int ID;
        public string Mod;
        public string Title;
        public string Description;
        public List<string> SpellClasses;
        public int IntelliganceModifier;
        public int StrenghtModifier;
        public int DexterityModifier;
        public int CharismaModifier;
        public bool CanConvertModifier;
        public bool CanMoveAndAttackModifier;
        public int MovePointsModifier;
        public int SightModifier;
        public int AttackRangeModifier;
    }

    public class Spell
    {
        public string Title;
        public int ID;
        public string Slug;
        public string Description;
        public string Type;
        public string Mod;
        public int PixelsPerUnit;
        public List<string> Effects;
        public int ManaCost;
        public int Range;
        public int Damage;
        public int Healing;
        public bool SpawnUnit;
        public int UnitID;
        public Sprite Sprite;

        public void GetSprites()
        {
            //Debug.log("Getting sprites for: " + Title);
            int count = new int();
            foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Mods/Core/Spells/Sprites/" + Title + "/", "*.png")))
            {
                if (file.Contains(Title))
                {
                    Sprite = DatabaseController.instance.loadSprite(file, PixelsPerUnit);
                    count = count + 1;
                }
                //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
            }
            //Debug.Log("Sprites found: " + count);
            count = 0;
        }
    }
}

/// Inteligence affects: 
/// Mana Regen
/// Mana Max
/// Xp Gain
/// Spellcasting Chance
/// 
/// Strenght affects:
/// Attack
/// Health Points
/// Health Regen
/// 
/// Dexterity affects:
/// Move Points
/// Defence
/// Amount of capture per turn
/// 
/// Charisma affects:
/// Cost of stuff
/// Troops Attack (moral)
/// Units in return

