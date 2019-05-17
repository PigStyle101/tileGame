using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class UnitController : MonoBehaviour
{
    private GameControllerScript GCS;
    private MapEditMenueCamController MEMCC;
    private DatabaseController DBC;
    //[HideInInspector]
    public int Team;
    [HideInInspector]
    public bool UnitMovable;
    [HideInInspector]
    public bool UnitMoved = false;
    [HideInInspector]
    public List<Vector2> FloodFillList;
    [HideInInspector]
    public int MovePoints;
    [HideInInspector]
    public int Range;
    [HideInInspector]
    public int Health;
    [HideInInspector]
    public int MaxHealth;
    [HideInInspector]
    public int Attack;
    [HideInInspector]
    public int Defence;
    public Dictionary<Vector2, int[]> TilesWeights = new Dictionary<Vector2, int[]>();
    public Dictionary<Vector2, int> TilesChecked = new Dictionary<Vector2, int>();
    public Dictionary<Vector2, int> EnemyUnitsInRange = new Dictionary<Vector2, int>();

    private void Awake()
    {
        try
        {
            if (SceneManager.GetActiveScene().name == "MapEditorScene")
            {
                MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
                Team = MEMCC.SelectedTeam;
            }
            GCS = GameObject.Find("GameController").GetComponent<GameControllerScript>();
            DBC = GameObject.Find("GameController").GetComponent<DatabaseController>();
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void UnitRoundUpdater()
    {
        try
        {
            if (Team == GCS.CurrentTeamsTurn) //object is on the current team
            {
                if (!UnitMoved)
                {
                    UnitMovable = true;
                }
                else
                {
                    UnitMovable = false;
                }
                foreach (var kvp in DBC.UnitDictionary)
                {
                    if (gameObject.transform.name == kvp.Value.Title)
                    {
                        GetTileValues();
                    }
                }
                UnitMoved = false;
            }
            else
            {
                UnitMovable = false;
                UnitMoved = false;
            }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void TeamSpriteController()
    {
        try
        {
            foreach (var kvp in DBC.UnitDictionary)
            {
                if (gameObject.name == kvp.Value.Title)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.UnitDictionary[kvp.Value.ID].ArtworkDirectory[Team]);
                }
            }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }

    public void GetTileValues()
    {
        try
        {
            //Debug.Log("MP = " + MovePoints);
            List<Vector2> Directions = new List<Vector2>();  //list for holding north,south,east,west
            Vector2 Position = gameObject.transform.position; //need original position of unit
            Directions.Add(new Vector2(0, 1));
            Directions.Add(new Vector2(1, 0));
            Directions.Add(new Vector2(0, -1));
            Directions.Add(new Vector2(-1, 0));
            int count = 1; //using this to make the algorith go more rounds then needed, as the most any unit should need is 6 rounds
            TilesWeights = new Dictionary<Vector2, int[]>();

            foreach (var dir in Directions)
            {
                if (GCS.TilePos.ContainsKey(Position + dir) && !TilesWeights.ContainsKey(Position + dir) && GCS.TilePos[Position + dir].GetComponent<TerrainController>().Walkable) //tile there?  already added to tilewhieght? can the tile be walked on?
                {
                    if (GCS.TilePos[Position + dir].GetComponent<TerrainController>().Weight <= MovePoints && !GCS.TilePos[Position + dir].GetComponent<TerrainController>().Occupied) //is the wheight less then the total movement points? is there already a unit there?
                    {
                        int[] temparray = new int[2]; //array to store count and mp used to get to that tile, the count will be needed later for when a ai is added.
                        temparray[0] = count;
                        temparray[1] = GCS.TilePos[Position].GetComponent<TerrainController>().Weight; //this is the movement points used so far
                        TilesWeights.Add(Position + dir, temparray);
                    }
                }
            }
            //count = count + 1;
            while (count <= 20)
            {
                int[] tempIntArray = new int[2];
                Dictionary<Vector2, int[]> Temp = new Dictionary<Vector2, int[]>();
                foreach (var kvp in TilesWeights) //we want to check all tiles in tile weight, maybe could change this to only check tiles that were last added?
                {
                    foreach (var dir in Directions) //for each tile we are checking we need to look in each direction.
                    {
                        if (GCS.TilePos.ContainsKey(kvp.Key + dir)) //is there a tile there?
                        {
                            if (!Temp.ContainsKey(kvp.Key + dir)) //does the temp already contain the tile?
                            {
                                if (kvp.Value[1] + GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Weight <= MovePoints && !GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Occupied) //is the wegiht of the tile + move points used already < move points total? unit there?
                                {
                                    if (GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Walkable) // can the tile be walked on?
                                    {
                                        tempIntArray[0] = count;
                                        tempIntArray[1] = GCS.TilePos[kvp.Key + dir].transform.GetComponent<TerrainController>().Weight + kvp.Value[1]; //sets this to mp used so far + weight of tile.
                                        Temp.Add(kvp.Key + dir, tempIntArray);
                                    }
                                } 
                            }
                            else //if temp already contains a key for this tile but more mp were used to get there then we want to replace the second value of the array with the lower number
                            {
                                if (kvp.Value[1] + GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Weight <= MovePoints && !GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Occupied)
                                {
                                    if (GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Walkable)
                                    {
                                        if (kvp.Value[1] + GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Weight < Temp[kvp.Key + dir][1])
                                        {
                                            Temp[kvp.Key + dir][1] = kvp.Value[1] + GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Weight;
                                        }
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
                        if (kvp.Value[1] < TilesWeights[kvp.Key][1])
                        {
                            TilesWeights[kvp.Key][1] = kvp.Value[1]; // if tileWheights already contains info for this block, but the mp count is higher replace with lower.
                        }
                    }
                    else
                    {
                        TilesWeights.Add(kvp.Key, kvp.Value); // other wise just add info
                    }
                }
                if (count == 25)
                {
                    Debug.Log("Broke, count at:" + count); //used to make sure while loop dont get stuck some how, wierdly it doe with out this.....
                    break;
                }
                count = count + 1;
            }
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }

    }

    public void ChangeUnit()
    {
        try
        {
            if (MEMCC.SelectedButton == "Delete Unit")
            {
                Debug.Log("Deleting Unit");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("ChangUnit activated");
                foreach (KeyValuePair<int, Unit> kvp in DBC.UnitDictionary)
                {
                    if (MEMCC.SelectedButton == kvp.Value.Title) //checks through dictionary for matching tile to button name
                    {
                        //Debug.Log("Changing tile to " + kvp.Value.Title);
                        gameObject.name = kvp.Value.Title;//change name of tile
                        gameObject.GetComponent<SpriteRenderer>().sprite = DBC.loadSprite(DBC.UnitDictionary[kvp.Key].ArtworkDirectory[0]); //change sprite of tile
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

    public int GetEnemyUnitsInRange()
    {
        try
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
                        if (GCS.UnitPos[Position + dir].GetComponent<UnitController>().Team != gameObject.GetComponent<UnitController>().Team)
                        {
                            EnemyUnitsInRange.Add(Position + dir, count); //add to dictionary 
                        }
                    }
                    TilesChecked.Add(Position + dir, count); //add to tiles checked
                }
            }
            count = count + 1;
            while (count <= Range)
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
                                if (GCS.UnitPos[kvp.Key + dir].GetComponent<UnitController>().Team != gameObject.GetComponent<UnitController>().Team) //is it not on the same team?
                                {
                                    EnemyUnitsInRange.Add(kvp.Key + dir, count);
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
                    Debug.Log("Broke, count at:" + count);
                    break;
                }
                count = count + 1;
            }
            Debug.Log("Enemys in range:" + EnemyUnitsInRange.Count);
            return EnemyUnitsInRange.Count;
        }
        catch (Exception e)
        {
            GCS.LogController(e.ToString());
            throw;
        }
    }
}
