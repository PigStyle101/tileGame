using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{
    private MapEditMenueCamController MEMCC;
    //[HideInInspector]
    public int Team;
    [HideInInspector]
    public int ID;
    [HideInInspector]
    public bool UnitMovable;
    [HideInInspector]
    public bool UnitMoved = false;
    [HideInInspector]
    public List<Vector2> FloodFillList;
    [HideInInspector]
    public int MovePoints;
    [HideInInspector]
    public int AttackRange;
    [HideInInspector]
    public int SightRange;
    [HideInInspector]
    public int Health;
    [HideInInspector]
    public int MaxHealth;
    [HideInInspector]
    public int Attack;
    [HideInInspector]
    public int Defence;
    [HideInInspector]
    public int ConversionSpeed;
    [HideInInspector]
    public bool CanConvert;
    [HideInInspector]
    public bool CanMoveAndAttack;
    public Dictionary<Vector2, int> TilesWeights = new Dictionary<Vector2, int>();
    public Dictionary<Vector2, int> SightTiles = new Dictionary<Vector2, int>();
    public Dictionary<Vector2, int> TilesChecked = new Dictionary<Vector2, int>();
    public Dictionary<Vector2, int> EnemyUnitsInRange = new Dictionary<Vector2, int>();
    [HideInInspector]
    public List<Vector2> Directions = new List<Vector2>();//list for holding north,south,east,west
    public bool UnitIdleAnimation;
    public List<string> IdleAnimationsDirectory;
    private DatabaseController DBC;
    private GameControllerScript GCS;

    private void Awake()
    {
        DBC = DatabaseController.instance;
        GCS = GameControllerScript.instance;
        if (SceneManager.GetActiveScene().name == "MapEditorScene")
        {
            MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
            Team = MEMCC.SelectedTeam;
        }
        Directions.Add(new Vector2(0, 1));
        Directions.Add(new Vector2(1, 0));
        Directions.Add(new Vector2(0, -1));
        Directions.Add(new Vector2(-1, 0));
    }

    public void UnitRoundUpdater()
    {
        if (Team == GCS.CurrentTeamsTurn.Team) //object is on the current team
        {
            UnitMoved = false;
            UnitMovable = true;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(.5F, .5F, .5F);
            GetTileValues();
            GetSightTiles();
        }
        else
        {
            UnitMovable = false;
            UnitMoved = false;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1F, 1F, 1F);
            GetTileValues();
        }
        
    }

    public void TeamSpriteController()
    {
        //gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.UnitDictionary[ID].ArtworkDirectory[Team], DBC.UnitDictionary[ID].PixelsPerUnit);
        switch (Team)
        {
            case 1:
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.black;
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.white;
                break;
            case 2:
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.blue;
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.white;
                break;
            case 3:
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.cyan;
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 4:
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.gray;
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 5:
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.green;
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 6:
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.magenta;
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 7:
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.red;
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 8:
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.white;
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 9:
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = Color.yellow;
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
            case 0:
                Color brown = new Color(.5f, .25f, 0, 255);
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").GetComponent<Image>().color = brown;
                gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().color = Color.black;
                break;
        }
    }

    public void GetTileValues() 
    {
        ////Debug.log("MP = " + MovePoints);
        Vector2 Position = gameObject.transform.position; //need original position of unitt
        int count = 1; //using this to make the algorith go more rounds then needed, as the most any unit should need is 6 rounds
        TilesWeights = new Dictionary<Vector2, int>();
        foreach (var dir in Directions)
        {
            if (GCS.TilePos.ContainsKey(Position + dir) && GCS.TilePos[Position + dir].GetComponent<TerrainController>().Walkable) //tile there?  already added to tilewhieght? can the tile be walked on?
            {
                if (GCS.TilePos[Position + dir].GetComponent<TerrainController>().Weight <= MovePoints && !GCS.UnitPos.ContainsKey(Position + dir)) //is the wheight less then the total movement points?
                {
                    int temp; //array to store count and mp used to get to that tile, the count will be needed later for when a ai is added.
                    temp = GCS.TilePos[Position + dir].GetComponent<TerrainController>().Weight; //this is the movement points used so far
                    TilesWeights.Add(Position + dir, temp);
                }
                else if (GCS.UnitPos.ContainsKey(Position +dir))
                {
                    if (GCS.UnitPos[Position + dir].GetComponent<UnitController>().Team == Team)
                    {
                        int temp; //array to store count and mp used to get to that tile, the count will be needed later for when a ai is added.
                        temp = GCS.TilePos[Position + dir].GetComponent<TerrainController>().Weight; //this is the movement points used so far
                        TilesWeights.Add(Position + dir, temp);
                    }
                }
            }
        }
        while (count <= MovePoints)
        {
            int tempInt;
            Dictionary<Vector2, int> Temp = new Dictionary<Vector2, int>();
            foreach (var kvp in TilesWeights) //we want to check all tiles in tile weight, maybe could change this to only check tiles that were last added?
            {
                foreach (var dir in Directions) //for each tile we are checking we need to look in each direction.
                {
                    if (GCS.TilePos.ContainsKey(kvp.Key + dir)) //is there a tile there?
                    {
                        if (!Temp.ContainsKey(kvp.Key + dir)) //does the temp already contain the tile?
                        {
                            if (kvp.Value + GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Weight <= MovePoints && GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Walkable && kvp.Key + dir != (Vector2)gameObject.transform.position) //is the wegiht of the tile + move points used already < move points total? Walkable?
                            {
                                if (!GCS.UnitPos.ContainsKey(kvp.Key + dir))
                                {
                                    tempInt = GCS.TilePos[kvp.Key + dir].transform.GetComponent<TerrainController>().Weight + kvp.Value; //sets this to mp used so far + weight of tile.
                                    Temp.Add(kvp.Key + dir, tempInt); 
                                }
                                else
                                {
                                    if (GCS.UnitPos[kvp.Key + dir].GetComponent<UnitController>().Team == Team)
                                    {
                                        tempInt = GCS.TilePos[kvp.Key + dir].transform.GetComponent<TerrainController>().Weight + kvp.Value; //this is the movement points used so far
                                        Temp.Add(kvp.Key + dir, tempInt);
                                    }
                                    else if (GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().FogOfWarBool)
                                    {
                                        tempInt = GCS.TilePos[kvp.Key + dir].transform.GetComponent<TerrainController>().Weight + kvp.Value; //this is the movement points used so far
                                        Temp.Add(kvp.Key + dir, tempInt);
                                    }
                                }
                            }
                        }
                        else //if temp already contains a key for this tile but more mp were used to get there then we want to replace the second value of the array with the lower number
                        {
                            if (kvp.Value + GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Weight <= MovePoints && GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Walkable && kvp.Key + dir != (Vector2)gameObject.transform.position)
                            {
                                if (kvp.Value + GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Weight < Temp[kvp.Key + dir])
                                {
                                    Temp[kvp.Key + dir] = kvp.Value + GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Weight;
                                }
                            }
                        }
                    }
                }
            }
            foreach (var kvp in Temp) // need to put info in temp while we were going through TileWeights
            {
                if (TilesWeights.ContainsKey(kvp.Key))
                {
                    if (kvp.Value < TilesWeights[kvp.Key])
                    {
                        TilesWeights[kvp.Key] = kvp.Value; // if tileWheights already contains info for this block, but the mp count is higher replace with lower.
                    }
                }
                else
                {
                    TilesWeights.Add(kvp.Key, kvp.Value); // other wise just add info
                }
            }
            if (count == MovePoints)
            {
                //Debug.log("Broke, count at:" + count); //used to make sure while loop dont get stuck some how, wierdly it does with out this.....
                break;
            }
            count = count + 1;
        }
    }

    public void GetSightTiles()
    {
        ////Debug.log("MP = " + MovePoints);
        Vector2 Position = gameObject.transform.position; //need original position of unit
        int count = 1; //using this to make the algorith go more rounds then needed
        SightTiles = new Dictionary<Vector2, int>();
        foreach (var dir in Directions)
        {
            if (GCS.TilePos.ContainsKey(Position + dir) && !SightTiles.ContainsKey(Position + dir)) //tile there?  already added to dict?
            {
                int temp; //array to store count and mp used to get to that tile, the count will be needed later for when a ai is added.
                temp = 1; //this is the range so far
                SightTiles.Add(Position + dir, temp);
            }
        }
        while (count <= SightRange)
        {
            int tempInt;
            Dictionary<Vector2, int> Temp = new Dictionary<Vector2, int>();
            foreach (var kvp in SightTiles) //we want to check all tiles in tile weight, maybe could change this to only check tiles that were last added?
            {
                foreach (var dir in Directions) //for each tile we are checking we need to look in each direction.
                {
                    if (GCS.TilePos.ContainsKey(kvp.Key + dir)) //is there a tile there?
                    {
                        if (!Temp.ContainsKey(kvp.Key + dir)) //does the temp already contain the tile?
                        {
                            if (kvp.Value + 1 <= SightRange) //is the wegiht of the tile + move points used already < move points total? unit there?
                            {
                                if (!GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().BlocksSight) // can the tile be walked on?
                                {
                                    tempInt = kvp.Value + 1; //sets this to mp used so far + weight of tile.
                                    Temp.Add(kvp.Key + dir, tempInt);
                                }
                            }
                        }
                        else //if temp already contains a key for this tile but more mp were used to get there then we want to replace the second value of the array with the lower number
                        {
                            if (kvp.Value + 1 <= SightRange && !GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().BlocksSight)
                            {
                                if (kvp.Value + 1 < Temp[kvp.Key + dir])
                                {
                                    Temp[kvp.Key + dir] = kvp.Value + 1;
                                }
                            }
                        }
                    }
                }
            }
            foreach (var kvp in Temp) // need to put info in temp while we were going through TileWeights
            {
                if (SightTiles.ContainsKey(kvp.Key))
                {
                    if (kvp.Value < SightTiles[kvp.Key])
                    {
                        SightTiles[kvp.Key] = kvp.Value; // if tileWheights already contains info for this block, but the mp count is higher replace with lower.
                    }
                }
                else
                {
                    SightTiles.Add(kvp.Key, kvp.Value); // other wise just add info
                }
            }
            if (count == SightRange)
            {
                //Debug.log("Broke, count at:" + count); //used to make sure while loop dont get stuck some how, wierdly it does with out this.....
                break;
            }
            count = count + 1;
        }
    }

    public void ChangeUnit()
    {
        if (MEMCC.SelectedButtonDR == -1)
        {
            //Debug.log("Deleting Unit");
            Destroy(gameObject);
        }
        else
        {
            ////Debug.log("Changing tile to " + kvp.Value.Title);
            gameObject.name = DBC.UnitDictionary[MEMCC.SelectedButtonDR].Title;//change name of tile
            gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.UnitDictionary[MEMCC.SelectedButtonDR].ArtworkDirectory[0],DBC.UnitDictionary[MEMCC.SelectedButtonDR].PixelsPerUnit); //change sprite of tile
            Health = DBC.UnitDictionary[MEMCC.SelectedButtonDR].Health;
            ID = MEMCC.SelectedButtonDR;
            gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().text = Health.ToString();
        }
    }

    public int GetEnemyUnitsInRange()
    {
        Vector2 Position = gameObject.transform.position;
        List<Vector2> Directions = new List<Vector2>();
        Directions.Add(new Vector2(0, 1));
        Directions.Add(new Vector2(1, 0));
        Directions.Add(new Vector2(0, -1));
        Directions.Add(new Vector2(-1, 0));
        int count = 1;
        EnemyUnitsInRange = new Dictionary<Vector2, int>();
        TilesChecked = new Dictionary<Vector2, int>();

        foreach (var dir in Directions)
        {
            if (GCS.TilePos.ContainsKey(Position + dir)) //is the tile on the map?
            {
                if (!EnemyUnitsInRange.ContainsKey(Position + dir) && GCS.UnitPos.ContainsKey(Position + dir)) //is the key already claimed?
                {
                    if (!GCS.TilePos[Position + dir].GetComponent<TerrainController>().FogOfWarBool) // if you cannot see the unit, you cannot attack it
                    {
                        if (GCS.UnitPos[Position + dir].GetComponent<UnitController>().Team != gameObject.GetComponent<UnitController>().Team)
                        {
                            EnemyUnitsInRange.Add(Position + dir, count); //add to dictionary 
                        } 
                    }
                }
                TilesChecked.Add(Position + dir, count); //add to tiles checked
            }
        }
        count = count + 1;
        while (count <= AttackRange)
        {
            Dictionary<Vector2, int> Temp = new Dictionary<Vector2, int>();
            foreach (var kvp in TilesChecked)
            {
                foreach (var dir in Directions)
                {
                    if (!TilesChecked.ContainsKey(kvp.Key + dir) && GCS.TilePos.ContainsKey(kvp.Key + dir) && !EnemyUnitsInRange.ContainsKey(kvp.Key + dir) && !Temp.ContainsKey(kvp.Key + dir)) //is teh tile there?is key claimed?
                    {
                        if (GCS.UnitPos.ContainsKey(kvp.Key + dir))//does tilesChecked not contain key? is there a unit there?
                        {
                            if (!GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().FogOfWarBool) //if we cannot see the unit, then we cannot attack it
                            {
                                if (GCS.UnitPos[kvp.Key + dir].GetComponent<UnitController>().Team != gameObject.GetComponent<UnitController>().Team) //is it not on the same team?
                                {
                                    EnemyUnitsInRange.Add(kvp.Key + dir, count);
                                } 
                            }
                        }
                        Temp.Add(kvp.Key + dir, count);
                    }
                }
            }
            foreach (var kvp in Temp)
            {
                TilesChecked.Add(kvp.Key, kvp.Value);
            }
            if (count == 15)
            {
                //Debug.log("Broke, count at:" + count);
                break;
            }
            count = count + 1;
        }
        //Debug.log("Enemys in range:" + EnemyUnitsInRange.Count);
        return EnemyUnitsInRange.Count;
    }

    /*public void UnitAi()
    {
        GetEnemyUnitsInRange();
        //Debug.log("1");
        Vector2 OriginalPositionOfUnit = gameObject.transform.position;
        Vector2 MoveToPosition;
        Vector2 Target;
        if (EnemyUnitsInRange.Count < 1) //if no enemy in range
        {
            foreach (var kvp in TilesWeights)
            {
                foreach (var dir in Directions)
                {
                    if (GCS.UnitPos.ContainsKey(kvp.Key + dir) && GCS.UnitPos[kvp.Key + dir].GetComponent<UnitController>().Team != Team) // look for enemy in movement range
                    {
                        if (UnitMoved == false)
                        {
                            //Debug.log("1.1"); //attacking enemy unit
                            Target = kvp.Key + dir;
                            MoveToPosition = kvp.Key;
                            gameObject.transform.position = MoveToPosition;
                            int attack = GCS.CombatCalculator(gameObject, GCS.UnitPos[Target]);
                            GCS.UnitPos[Target].GetComponent<UnitController>().Health = GCS.UnitPos[Target].GetComponent<UnitController>().Health - attack;
                            if (GCS.UnitPos[Target].GetComponent<UnitController>().Health <= 0)
                            {
                                GCS.KillUnitPlayScene(GCS.UnitPos[Target]);
                            }
                            else
                            {
                                GCS.UnitPos[Target].GetComponentInChildren<Text>().text = GCS.UnitPos[Target].GetComponent<UnitController>().Health.ToString();
                            }
                            GCS.MoveUnitPositionInDictionary(gameObject, OriginalPositionOfUnit);
                            foreach (var kvvp in GCS.TilePos)
                            {
                                kvvp.Value.GetComponent<TerrainController>().TerrainRoundUpdater();
                            }
                            foreach (var kvpp in GCS.UnitPos)
                            {
                                kvpp.Value.GetComponent<UnitController>().GetTileValues();
                            }
                            UnitMovable = false;
                            UnitMoved = true;
                        }
                    }
                }
            } 
        }
        else //there is a enemy within range
        {
            foreach(var enemy in EnemyUnitsInRange)
            {
                if (!UnitMoved)
                {
                    //Debug.log("2");
                    Target = enemy.Key;
                    int attack = GCS.CombatCalculator(gameObject, GCS.UnitPos[Target]);
                    GCS.UnitPos[Target].GetComponent<UnitController>().Health = GCS.UnitPos[Target].GetComponent<UnitController>().Health - attack;
                    if (GCS.UnitPos[Target].GetComponent<UnitController>().Health <= 0)
                    {
                        GCS.KillUnitPlayScene(GCS.UnitPos[Target]);
                    }
                    else
                    {
                        GCS.UnitPos[Target].GetComponentInChildren<Text>().text = GCS.UnitPos[Target].GetComponent<UnitController>().Health.ToString();
                    }
                    GCS.MoveUnitPositionInDictionary(gameObject, OriginalPositionOfUnit);
                    foreach (var kvvp in GCS.TilePos)
                    {
                        kvvp.Value.GetComponent<TerrainController>().TerrainRoundUpdater();
                    }
                    foreach (var kvpp in GCS.UnitPos)
                    {
                        kvpp.Value.GetComponent<UnitController>().GetTileValues();
                    }
                    UnitMovable = false;
                    UnitMoved = true;
                }
            }
        }
        if (UnitMoved == false) //if standing on a building continue converting it
        {
            foreach(var b in GCS.BuildingPos)
            {
                if (b.Key == (Vector2)transform.position && b.Value.GetComponent<BuildingController>().Team != Team)
                {
                    b.Value.transform.GetComponent<BuildingController>().Health = b.Value.transform.GetComponent<BuildingController>().Health - ConversionSpeed;
                    if (b.Value.transform.GetComponent<BuildingController>().Health <= 0)
                    {
                        b.Value.transform.GetComponent<BuildingController>().Team = Team;
                        b.Value.transform.GetComponent<BuildingController>().Health = 10; // set this to max health variable from building json
                        b.Value.transform.GetComponent<BuildingController>().TeamSpriteUpdater();
                        b.Value.transform.GetComponentInChildren<Text>().text = b.Value.transform.GetComponent<BuildingController>().Health.ToString();
                    }
                    else
                    {
                        b.Value.transform.GetComponentInChildren<Text>().text = b.Value.transform.GetComponent<BuildingController>().Health.ToString();
                    }
                    foreach (var kvvp in GCS.TilePos)
                    {
                        kvvp.Value.GetComponent<TerrainController>().TerrainRoundUpdater();
                    }
                    foreach (var kvpp in GCS.UnitPos)
                    {
                        kvpp.Value.GetComponent<UnitController>().GetTileValues();
                    }
                    UnitMovable = false;
                    UnitMoved = true;
                }
            }
        }
        if (UnitMoved == false) //checks for convertable building
        {
            foreach (var kvp in TilesWeights)
            {
                foreach (var b in GCS.BuildingPos)
                {
                    if (b.Value.GetComponent<BuildingController>().Team != Team && kvp.Key == b.Key)
                    {
                        if (UnitMoved == false)
                        {
                            gameObject.transform.position = kvp.Key;
                            b.Value.transform.GetComponent<BuildingController>().Health = b.Value.transform.GetComponent<BuildingController>().Health - ConversionSpeed;
                            if (b.Value.transform.GetComponent<BuildingController>().Health <= 0)
                            {
                                b.Value.transform.GetComponent<BuildingController>().Team = Team;
                                b.Value.transform.GetComponent<BuildingController>().Health = 10; // set this to max health variable from building json
                                b.Value.transform.GetComponent<BuildingController>().TeamSpriteUpdater();
                                b.Value.transform.GetComponentInChildren<Text>().text = b.Value.transform.GetComponent<BuildingController>().Health.ToString();
                            }
                            else
                            {
                                b.Value.transform.GetComponentInChildren<Text>().text = b.Value.transform.GetComponent<BuildingController>().Health.ToString();
                            }
                            GCS.MoveUnitPositionInDictionary(gameObject, OriginalPositionOfUnit);
                            foreach (var kvvp in GCS.TilePos)
                            {
                                kvvp.Value.GetComponent<TerrainController>().TerrainRoundUpdater();
                            }
                            foreach (var kvpp in GCS.UnitPos)
                            {
                                kvpp.Value.GetComponent<UnitController>().GetTileValues();
                            }
                            UnitMovable = false;
                            UnitMoved = true; 
                        }
                    }
                }
            } 
        }
        if (UnitMoved == false) // If nothing else can be done, move randomly
        {
            //Debug.log("3");

            List<Vector2> Keylist = new List<Vector2>(TilesWeights.Keys);
            System.Random rand = new System.Random();
            Vector2 randkey = Keylist[rand.Next(Keylist.Count)];

            //Debug.log("4.1");
            gameObject.transform.position = randkey;
            GCS.MoveUnitPositionInDictionary(gameObject, OriginalPositionOfUnit);
            foreach (var kvvp in GCS.TilePos)
            {
                kvvp.Value.GetComponent<TerrainController>().TerrainRoundUpdater();
            }
            foreach (var kvpp in GCS.UnitPos)
            {
                kvpp.Value.GetComponent<UnitController>().GetTileValues();
            }
            UnitMovable = false;
            UnitMoved = true;
        }
    }*/   //this will need to be redone and reworked as mechanicks are added to the units. currently set up to work before fog of war was added. 

    public void HealUnitIfOnFriendlyBuilding()
    {
        foreach (var b in GCS.BuildingPos)
        {
            if (b.Key == (Vector2)gameObject.transform.position && b.Value.GetComponent<BuildingController>().Team == Team && Health < MaxHealth && GCS.CurrentTeamsTurn.Team == Team)
            {
                Health = Health + 1;
                gameObject.GetComponentInChildren<Text>().text = Health.ToString();
            }
        }
    } 

    public void MovementController()
    {
        if (UnitMoved && !UnitMovable && Team == GCS.CurrentTeamsTurn.Team)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1F, 1F, 1F);
        }
        else
        {
            if (Team == GCS.CurrentTeamsTurn.Team)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(.5F, .5F, .5F);
            }
        }
    }

    public void IdleAnimationController()
    {
    }
}
