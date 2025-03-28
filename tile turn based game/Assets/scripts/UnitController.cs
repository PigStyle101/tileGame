﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MoonSharp.Interpreter;

namespace TileGame
{
    [MoonSharpUserData]
    public class UnitController : MonoBehaviour
    {
        //json and stat stuff hero specific
        [HideInInspector]
        public bool Hero;
        [HideInInspector]
        public Hero HClass;
        //json and stat stuff for all units
        [HideInInspector]
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
        //[HideInInspector]
        public int AttackRange;
        [HideInInspector]
        public int SightRange;
        //[HideInInspector]
        public int Health;
        [HideInInspector]
        public int MaxHealth;
        [HideInInspector]
        public int Attack;
        [HideInInspector]
        public int Defence;
        [HideInInspector]
        public int XP;
        [HideInInspector]
        public int Level;
        //[HideInInspector]
        public int ConversionSpeed;
        [HideInInspector]
        public bool CanConvert;
        [HideInInspector]
        public bool CanMoveAndAttack;
        //[HideInInspector]
        public float MoveAnimationTime;
        //method stuff
        public Dictionary<Vector2, int> TilesWeights = new Dictionary<Vector2, int>();
        public Dictionary<Vector2, int> SightTiles = new Dictionary<Vector2, int>();
        public Dictionary<Vector2, int> TilesChecked = new Dictionary<Vector2, int>();
        public Dictionary<Vector2, int> EnemyUnitsInRange = new Dictionary<Vector2, int>();
        public Dictionary<Vector2, int> FriendlyUnitsInRange = new Dictionary<Vector2, int>();
        public Dictionary<Vector2, int> UnitsInRangeSpell = new Dictionary<Vector2, int>();
        [HideInInspector]
        public List<Vector2> Directions = new List<Vector2>();//list for holding north,south,east,west
        [HideInInspector]
        public Vector2 OriginalPosition;
        [HideInInspector]
        public List<Vector2> vectorlist;
        [HideInInspector]
        public int VectorState = 0;
        [HideInInspector]
        public Vector2 Position;
        [HideInInspector]
        private bool AttackOverlay = false;
        //Animations stuff
        [HideInInspector]
        public bool UnitIdleAnimation;
        [HideInInspector]
        public bool UnitAttackAnimation;
        //[HideInInspector]
        public bool UnitHurtAnimation;
        //[HideInInspector]
        public bool UnitDiedAnimation;
        [HideInInspector]
        public bool UnitMoveAnimation;
        private float IdleTimerFloat;
        private float AttackTimerFloat;
        private float HurtTimerFloat;
        private float DiedTimerFloat;
        private float MoveTimerFloat;
        private float MoveAnimationTimerFloat;
        private int IdleState;
        private int AttackState;
        private int HurtState;
        private int DiedState;
        private int MoveState;
        [HideInInspector]
        public float IdleAnimationSpeed;
        [HideInInspector]
        public float AttackAnimationSpeed;
        [HideInInspector]
        public float HurtAnimationSpeed;
        [HideInInspector]
        public float DiedAnimationSpeed;
        [HideInInspector]
        public float MoveAnimationSpeed;
        private bool Attacking;
        private bool Moving;
        public bool Dying;
        public bool Hurt;
        public bool UseOwnUpdate;
        //Script stuff
        private DatabaseController DBC;
        private GameControllerScript GCS;
        private MapEditMenueCamController MEMCC;

        [MoonSharpHidden]
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
            UseOwnUpdate = false;
        }
        [MoonSharpHidden]
        private void Update()
        {
            if (UseOwnUpdate)
            {
                AnimationTimers();
                RayCastForAttackRange();
            }
        }
        public void CustomUpdate()
        {
            AnimationTimers();
            RayCastForAttackRange();
        }
        public void UnitRoundUpdater()
        {
            try
            {
                if (Team == GCS.CurrentTeamsTurn.Team) //object is on the current team
                {
                    UnitMoved = false;
                    UnitMovable = true;
                    if (Hero)
                    {
                        if (Health < MaxHealth) //is health less then maxhealth
                        {
                            Health = Health + HClass.HealthRegen; //get us some regen
                            if (Health > MaxHealth) //did we over regenerate?
                            {
                                Health = MaxHealth; //remove that extra regen
                            }
                        }
                        else
                        {
                            Health = MaxHealth; //if health gets over max somehow we need to fix that.
                        }
                        gameObject.GetComponentInChildren<Text>().text = Health.ToString();
                    }
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        } //Potential lua method
        public void GetTileValues()
        {
            try
            {
                ////Debug.log("MP = " + MovePoints);
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
                        else if (GCS.UnitPos.ContainsKey(Position + dir))
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
                                    {//enough mp? Is tile walkable? not the position of the unit currently?
                                        if (!GCS.UnitPos.ContainsKey(kvp.Key + dir)) //is there not a unit there?
                                        {
                                            tempInt = GCS.TilePos[kvp.Key + dir].transform.GetComponent<TerrainController>().Weight + kvp.Value; //sets this to mp used so far + weight of tile.
                                            Temp.Add(kvp.Key + dir, tempInt);
                                        }
                                        else
                                        {
                                            if (GCS.UnitPos[kvp.Key + dir].GetComponent<UnitController>().Team == Team) //is this unit on are team?
                                            {
                                                tempInt = GCS.TilePos[kvp.Key + dir].transform.GetComponent<TerrainController>().Weight + kvp.Value; //this is the movement points used so far
                                                Temp.Add(kvp.Key + dir, tempInt);
                                            }
                                            else if (GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().FogOfWarBool) //if enemy are they in the fog of war?
                                            {
                                                tempInt = GCS.TilePos[kvp.Key + dir].transform.GetComponent<TerrainController>().Weight + kvp.Value; //this is the movement points used so far
                                                Temp.Add(kvp.Key + dir, tempInt);
                                            }
                                        }
                                    }
                                }
                                else //if temp already contains a key for this tile but more mp were used to get there then we want to replace the second value of the array with the lower number
                                {
                                    if (kvp.Value + GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Weight <= MovePoints && GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Walkable)
                                    {
                                        if (kvp.Value + GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Weight < Temp[kvp.Key + dir])
                                        {
                                            if (kvp.Key + dir != (Vector2)gameObject.transform.position)
                                            {
                                                Temp[kvp.Key + dir] = kvp.Value + GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().Weight;
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        } //Potential lua method
        public void GetSightTiles()
        {
            try
            {
                ////Debug.log("MP = " + MovePoints);
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        } //Potential lua method
        public void ChangeUnit()
        {
            try
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
                    gameObject.GetComponent<SpriteRenderer>().sprite = DBC.UnitDictionary[MEMCC.SelectedButtonDR].IconSprite; //change sprite of tile
                    Health = DBC.UnitDictionary[MEMCC.SelectedButtonDR].Health;
                    ID = MEMCC.SelectedButtonDR;
                    gameObject.transform.Find("UnitHealthOverlay(Clone)").Find("Image").Find("Text").GetComponent<Text>().text = Health.ToString();
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }   //Potential lua method
        public int GetEnemyUnitsInRange()
        {
            try
            {
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        } //Potential lua method
        public int GetUnitsInRangeForSpell(int range, bool targetsfriendly, bool targetsenemy)
        {
            try
            {
                List<Vector2> Directions = new List<Vector2>();
                Directions.Add(new Vector2(0, 1));
                Directions.Add(new Vector2(1, 0));
                Directions.Add(new Vector2(0, -1));
                Directions.Add(new Vector2(-1, 0));
                int count = 1;
                UnitsInRangeSpell = new Dictionary<Vector2, int>();
                TilesChecked = new Dictionary<Vector2, int>();

                if (targetsfriendly && !targetsenemy) //only targets friendly units
                {
                    foreach (var dir in Directions)
                    {
                        if (GCS.TilePos.ContainsKey(Position + dir)) //is the tile on the map?
                        {
                            if (!UnitsInRangeSpell.ContainsKey(Position + dir) && GCS.UnitPos.ContainsKey(Position + dir)) //is the key already claimed?
                            {
                                if (!GCS.TilePos[Position + dir].GetComponent<TerrainController>().FogOfWarBool) // if you cannot see the unit, you cannot attack it
                                {
                                    if (GCS.UnitPos[Position + dir].GetComponent<UnitController>().Team == gameObject.GetComponent<UnitController>().Team) //is unit on are team?
                                    {
                                        UnitsInRangeSpell.Add(Position + dir, count); //add to dictionary 
                                    }
                                }
                            }
                            TilesChecked.Add(Position + dir, count); //add to tiles checked
                        }
                    }
                    count = count + 1;
                    while (count <= range)
                    {
                        Dictionary<Vector2, int> Temp = new Dictionary<Vector2, int>();
                        foreach (var kvp in TilesChecked)
                        {
                            foreach (var dir in Directions)
                            {
                                if (!TilesChecked.ContainsKey(kvp.Key + dir) && GCS.TilePos.ContainsKey(kvp.Key + dir) && !UnitsInRangeSpell.ContainsKey(kvp.Key + dir) && !Temp.ContainsKey(kvp.Key + dir)) //is teh tile there?is key claimed?
                                {
                                    if (GCS.UnitPos.ContainsKey(kvp.Key + dir))//does tilesChecked not contain key? is there a unit there?
                                    {
                                        if (!GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().FogOfWarBool) //if we cannot see the unit, then we cannot attack it
                                        {
                                            if (GCS.UnitPos[kvp.Key + dir].GetComponent<UnitController>().Team == gameObject.GetComponent<UnitController>().Team) //is it on the same team?
                                            {
                                                UnitsInRangeSpell.Add(kvp.Key + dir, count);
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
                            Debug.Log("F Broke, count at:" + count);
                            break;
                        }
                        count = count + 1;
                    } 
                }
                else if (targetsenemy && !targetsfriendly) //only targets enemyunits
                {
                    foreach (var dir in Directions)
                    {
                        if (GCS.TilePos.ContainsKey(Position + dir)) //is the tile on the map?
                        {
                            if (!UnitsInRangeSpell.ContainsKey(Position + dir) && GCS.UnitPos.ContainsKey(Position + dir)) //is the key already claimed?
                            {
                                if (!GCS.TilePos[Position + dir].GetComponent<TerrainController>().FogOfWarBool) // if you cannot see the unit, you cannot attack it
                                {
                                    if (GCS.UnitPos[Position + dir].GetComponent<UnitController>().Team != gameObject.GetComponent<UnitController>().Team) //is unit on are team?
                                    {
                                        UnitsInRangeSpell.Add(Position + dir, count); //add to dictionary 
                                    }
                                }
                            }
                            TilesChecked.Add(Position + dir, count); //add to tiles checked
                        }
                    }
                    count = count + 1;
                    while (count <= range)
                    {
                        Dictionary<Vector2, int> Temp = new Dictionary<Vector2, int>();
                        foreach (var kvp in TilesChecked)
                        {
                            foreach (var dir in Directions)
                            {
                                if (!TilesChecked.ContainsKey(kvp.Key + dir) && GCS.TilePos.ContainsKey(kvp.Key + dir) && !UnitsInRangeSpell.ContainsKey(kvp.Key + dir) && !Temp.ContainsKey(kvp.Key + dir)) //is teh tile there?is key claimed?
                                {
                                    if (GCS.UnitPos.ContainsKey(kvp.Key + dir))//does tilesChecked not contain key? is there a unit there?
                                    {
                                        if (!GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().FogOfWarBool) //if we cannot see the unit, then we cannot attack it
                                        {
                                            if (GCS.UnitPos[kvp.Key + dir].GetComponent<UnitController>().Team != gameObject.GetComponent<UnitController>().Team) //is it on the same team?
                                            {
                                                UnitsInRangeSpell.Add(kvp.Key + dir, count);
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
                            Debug.Log("E Broke, count at:" + count);
                            break;
                        }
                        count = count + 1;
                    }
                }
                else //spell targets both enemy and friends
                {
                    foreach (var dir in Directions)
                    {
                        if (GCS.TilePos.ContainsKey(Position + dir)) //is the tile on the map?
                        {
                            if (!UnitsInRangeSpell.ContainsKey(Position + dir) && GCS.UnitPos.ContainsKey(Position + dir)) //is the key already claimed?
                            {
                                if (!GCS.TilePos[Position + dir].GetComponent<TerrainController>().FogOfWarBool) // if you cannot see the unit, you cannot attack it
                                {
                                    UnitsInRangeSpell.Add(Position + dir, count); //add to dictionary 
                                }
                            }
                            TilesChecked.Add(Position + dir, count); //add to tiles checked
                        }
                    }
                    count = count + 1;
                    while (count <= range)
                    {
                        Dictionary<Vector2, int> Temp = new Dictionary<Vector2, int>();
                        foreach (var kvp in TilesChecked)
                        {
                            foreach (var dir in Directions)
                            {
                                if (!TilesChecked.ContainsKey(kvp.Key + dir) && GCS.TilePos.ContainsKey(kvp.Key + dir) && !UnitsInRangeSpell.ContainsKey(kvp.Key + dir) && !Temp.ContainsKey(kvp.Key + dir)) //is teh tile there?is key claimed?
                                {
                                    if (GCS.UnitPos.ContainsKey(kvp.Key + dir))//does tilesChecked not contain key? is there a unit there?
                                    {
                                        if (!GCS.TilePos[kvp.Key + dir].GetComponent<TerrainController>().FogOfWarBool) //if we cannot see the unit, then we cannot attack it
                                        {
                                            UnitsInRangeSpell.Add(kvp.Key + dir, count);
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
                            Debug.Log("FE Broke, count at:" + count);
                            break;
                        }
                        count = count + 1;
                    }
                }
                //Debug.log("Enemys in range:" + EnemyUnitsInRange.Count);
                return UnitsInRangeSpell.Count;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        } //potential lua method

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
        [MoonSharpHidden]
        public void HealUnitIfOnFriendlyBuilding()
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }  //Potential lua method
        public void MovementController()
        {
            try
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
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        } //Potential lua method
        [MoonSharpHidden]
        public void AnimationTimers()
        {
            try
            {
                if (!Hero)
                {
                    if (UnitIdleAnimation && !Attacking && !Dying && !Hurt && !Moving)
                    {
                        IdleTimerFloat += Time.deltaTime;
                        if (IdleTimerFloat >= IdleAnimationSpeed)
                        {
                            if ((DBC.UnitDictionary[ID].IdleAnimationDirectory.Count - 1) > IdleState)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.UnitDictionary[ID].IdleAnimationDirectory[IdleState];
                                IdleState = IdleState + 1;
                            }
                            else
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.UnitDictionary[ID].IdleAnimationDirectory[IdleState];
                                IdleState = 0;
                            }
                            IdleTimerFloat = 0;
                        }
                    }
                    else if (Attacking)
                    {
                        AttackTimerFloat += Time.deltaTime;
                        if (AttackTimerFloat >= AttackAnimationSpeed && UnitAttackAnimation)
                        {
                            if ((DBC.UnitDictionary[ID].AttackAnimationDirectory.Count - 1) > AttackState)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.UnitDictionary[ID].AttackAnimationDirectory[AttackState];
                                AttackState = AttackState + 1;
                            }
                            else
                            {
                                AttackState = 0;
                                Attacking = false;
                            }
                            AttackTimerFloat = 0;
                        }
                        else if (!UnitAttackAnimation)
                        {
                            Attacking = false;
                        }
                    }
                    else if (Hurt)
                    {
                        HurtTimerFloat += Time.deltaTime;
                        if (HurtTimerFloat >= HurtAnimationSpeed && UnitHurtAnimation)
                        {
                            if ((DBC.UnitDictionary[ID].HurtAnimationDirectory.Count - 1) > HurtState)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.UnitDictionary[ID].HurtAnimationDirectory[HurtState];
                                HurtState = HurtState + 1;
                            }
                            else
                            {
                                HurtState = 0;
                                Hurt = false;
                            }
                            HurtTimerFloat = 0;
                        }
                        else if (!UnitHurtAnimation)
                        {
                            Hurt = false;
                        }
                    }
                    else if (Dying)
                    {
                        DiedTimerFloat += Time.deltaTime;
                        if (DiedTimerFloat >= DiedAnimationSpeed && UnitDiedAnimation)
                        {
                            if ((DBC.UnitDictionary[ID].DiedAnimationDirectory.Count - 1) > DiedState)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.UnitDictionary[ID].DiedAnimationDirectory[DiedState];
                                DiedState = DiedState + 1;
                            }
                            else
                            {
                                DiedState = 0;
                                Dying = false;
                                Destroy(gameObject);
                            }
                            DiedTimerFloat = 0;
                        }
                        else if (!UnitDiedAnimation)
                        {
                            Destroy(gameObject);
                        }
                    }
                    else if (Moving)
                    {
                        float step = MoveAnimationTime * Time.deltaTime; //get time that we want to move for
                        transform.position = Vector2.MoveTowards(transform.position, vectorlist[VectorState], step); //apply move
                        MoveAnimationTimerFloat += Time.deltaTime; // getting current time that we have moved for
                        if (MoveAnimationTimerFloat >= 1 / MoveAnimationTime) //have we moved to this position for the required time?
                        {
                            if ((vectorlist.Count - 1) > VectorState)
                            {
                                VectorState += 1;
                            }
                            else
                            {
                                VectorState = 0;
                                Moving = false;
                                transform.position = Position;
                            }
                            MoveAnimationTimerFloat = 0;
                        }
                        MoveTimerFloat += Time.deltaTime;
                        if (MoveTimerFloat >= MoveAnimationSpeed && UnitMoveAnimation)
                        {
                            if ((DBC.UnitDictionary[ID].MoveAnimationDirectory.Count - 1) > MoveState)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.UnitDictionary[ID].MoveAnimationDirectory[MoveState];
                                MoveState = MoveState + 1;
                            }
                            else
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.UnitDictionary[ID].MoveAnimationDirectory[MoveState];
                                MoveState = 0;
                            }
                            MoveTimerFloat = 0;
                        }
                    }
                }
                else
                {
                    if (UnitIdleAnimation && !Attacking && !Dying && !Hurt && !Moving)
                    {
                        IdleTimerFloat += Time.deltaTime;
                        if (IdleTimerFloat >= IdleAnimationSpeed)
                        {
                            if ((DBC.HeroRaceDictionary[ID].IdleAnimationDirectory.Count - 1) > IdleState)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.HeroRaceDictionary[ID].IdleAnimationDirectory[IdleState];
                                IdleState = IdleState + 1;
                            }
                            else
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.HeroRaceDictionary[ID].IdleAnimationDirectory[IdleState];
                                IdleState = 0;
                            }
                            IdleTimerFloat = 0;
                        }
                    }
                    else if (Attacking)
                    {
                        AttackTimerFloat += Time.deltaTime;
                        if (AttackTimerFloat >= AttackAnimationSpeed && UnitAttackAnimation)
                        {
                            if ((DBC.HeroRaceDictionary[ID].AttackAnimationDirectory.Count - 1) > AttackState)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.HeroRaceDictionary[ID].AttackAnimationDirectory[AttackState];
                                AttackState = AttackState + 1;
                            }
                            else
                            {
                                AttackState = 0;
                                Attacking = false;
                            }
                            AttackTimerFloat = 0;
                        }
                        else if (!UnitAttackAnimation)
                        {
                            Attacking = false;
                        }
                    }
                    else if (Hurt)
                    {
                        HurtTimerFloat += Time.deltaTime;
                        if (HurtTimerFloat >= HurtAnimationSpeed && UnitHurtAnimation)
                        {
                            if ((DBC.HeroRaceDictionary[ID].HurtAnimationDirectory.Count - 1) > HurtState)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.HeroRaceDictionary[ID].HurtAnimationDirectory[HurtState];
                                HurtState = HurtState + 1;
                            }
                            else
                            {
                                HurtState = 0;
                                Hurt = false;
                            }
                            HurtTimerFloat = 0;
                        }
                        else if (!UnitHurtAnimation)
                        {
                            Hurt = false;
                        }
                    }
                    else if (Dying && !Hurt)
                    {
                        DiedTimerFloat += Time.deltaTime;
                        if (DiedTimerFloat >= DiedAnimationSpeed && UnitDiedAnimation)
                        {
                            if ((DBC.HeroRaceDictionary[ID].DiedAnimationDirectory.Count - 1) > DiedState)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.HeroRaceDictionary[ID].DiedAnimationDirectory[DiedState];
                                DiedState = DiedState + 1;
                            }
                            else
                            {
                                DiedState = 0;
                                //Dying = false;
                                Destroy(gameObject);
                            }
                            DiedTimerFloat = 0;
                        }
                        else if (!UnitDiedAnimation)
                        {
                            Destroy(gameObject);
                        }
                    }
                    else if (Moving)
                    {
                        float step = MoveAnimationTime * Time.deltaTime; //get time that we want to move for
                        transform.position = Vector2.MoveTowards(transform.position, vectorlist[VectorState], step);
                        MoveAnimationTimerFloat += Time.deltaTime;
                        if (MoveAnimationTimerFloat >= 1 / MoveAnimationTime)
                        {
                            if ((vectorlist.Count - 1) > VectorState)
                            {
                                VectorState += 1;
                            }
                            else
                            {
                                VectorState = 0;
                                Moving = false;
                                transform.position = Position;
                            }
                            MoveAnimationTimerFloat = 0;
                        }
                        MoveTimerFloat += Time.deltaTime;
                        if (MoveTimerFloat >= MoveAnimationSpeed && UnitMoveAnimation)
                        {
                            if ((DBC.HeroRaceDictionary[ID].MoveAnimationDirectory.Count - 1) > MoveState)
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.HeroRaceDictionary[ID].MoveAnimationDirectory[MoveState];
                                MoveState = MoveState + 1;
                            }
                            else
                            {
                                gameObject.GetComponent<SpriteRenderer>().sprite = DBC.HeroRaceDictionary[ID].MoveAnimationDirectory[MoveState];
                                MoveState = 0;
                            }
                            MoveTimerFloat = 0;
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
        [MoonSharpHidden]
        public void KilledEnemyUnit(UnitController UC)
        {
            try
            {
                if (UC.Hero)
                {
                    XP += 5;
                }
                else
                {
                    XP += 1;
                }
                if (XP >= Math.Pow(2, Level + 1)) //this makes it double the amount of xp needed to lvl up every lvl, starting with 1 ---- takes 1xp to get to lvl2, 2xp for 3, 4xp for 4, total of 7xp to get to lvl 4
                {
                    XP = 0;
                    Level += 1;
                    if (!Hero)
                    {
                        Defence += 1;
                        if (Level % 2 == 0) //if remainder of level/2 is = to 0 ----so that will increase this stat every 2 levels
                        {
                            ConversionSpeed += 1;
                        }
                        if (Level % 3 == 0)
                        {
                            Attack += 1;
                        }
                        if (Level % 4 == 0)
                        {
                            MovePoints += 1;
                        }
                    }
                    else
                    {
                        UpdateHeroStats();
                    }
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        } //Potential lua method
        [MoonSharpHidden]
        public void RayCastForAttackRange()
        {
            try
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
                {
                    if (!EventSystem.current.IsPointerOverGameObject()) //dont want to click through menus
                    {
                        ////Debug.log("RayHitStarted");
                        Ray ray = GameObject.Find("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); //GET THEM RAYS
                        RaycastHit[] hits;
                        hits = Physics.RaycastAll(ray);
                        for (int i = 0; i < hits.Length; i++) // GO THROUGH THEM RAYS
                        {
                            RaycastHit hit = hits[i];
                            if (hit.transform == gameObject.transform && AttackOverlay == false) //did we hit a this unit?
                            {
                                GetEnemyUnitsInRange();
                                foreach (var kvp in TilesChecked)
                                {
                                    GCS.TilePos[kvp.Key].transform.GetComponent<TerrainController>().AttackOverlay = true;
                                }
                                AttackOverlay = true;
                                break;
                            }
                            /*else if (hit.transform == transform && AttackOverlay)
                            {
                                foreach (var kvp in TilesChecked)
                                {
                                    GCS.TilePos[kvp.Key].transform.GetComponent<TerrainController>().AttackOverlay = false;
                                }
                                AttackOverlay = false;
                                break;
                            }*/
                        }
                    }
                }
                else if (!Input.GetKey(KeyCode.LeftControl) && AttackOverlay)
                {
                    foreach (var kvp in TilesChecked)
                    {
                        GCS.TilePos[kvp.Key].transform.GetComponent<TerrainController>().AttackOverlay = false;
                    }
                    AttackOverlay = false;
                }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }
        [MoonSharpHidden]
        public void UpdateHeroStats()
        {
            try
            {
                HClass.Intelligance = HClass.BaseIntelligance * Level;
                HClass.Strenght = HClass.BaseStrenght * Level;
                HClass.Dexterity = HClass.BaseDexterity * Level;
                HClass.Charisma = HClass.BaseCharisma * Level;
                HClass.Attack = (int)Math.Ceiling((double)HClass.Strenght / 2) + 1;
                HClass.MaxHealth = HClass.Strenght + 1;
                HClass.HealthRegen = (int)Math.Ceiling((double)HClass.Strenght / 3);
                HClass.MovePoints = (int)Math.Ceiling((double)HClass.Dexterity / 3) + 1;
                HClass.Defence = HClass.Dexterity;
                HClass.ConversionSpeed = (int)Math.Ceiling((double)HClass.Dexterity / 3) + 1;
                HClass.Mana = HClass.Intelligance;
                HClass.ManaRegen = (int)Math.Ceiling((double)HClass.Intelligance / 3) + 1;
                HClass.XPGain = (int)Math.Ceiling((double)HClass.Intelligance / 3);
                HClass.UnitCost = (int)Math.Ceiling((double)HClass.Charisma / 3);
                HClass.MoralBonus = (int)Math.Ceiling((double)HClass.Charisma / 3);
                HClass.ReturnAmount = (int)Math.Ceiling((double)HClass.Charisma / 3);
                Attack = HClass.Attack;
                MaxHealth = HClass.MaxHealth;
                MovePoints = HClass.MovePoints;
                Defence = HClass.Defence;
                ConversionSpeed = HClass.ConversionSpeed;
                HClass.Level = Level;
                HClass.XP = XP;
                DBC.HeroDictionary[HClass.Name] = HClass;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        } //Potential lua method

        public bool DoDamage(int damage)
        {
            try
            {
                Health -= damage;
                if (Health > 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void DoHeal(int heal)
        {
            try
            {
                Health += heal;
                if (Health > MaxHealth)
                {
                    Health = MaxHealth;
                }
            }
            catch(Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }
        public void StartAttackAnimation()
        {
            Attacking = true;
        }

        public void StartHurtAnimation()
        {
            Hurt = true;
        }

        public void StartMovingAnimation()
        {
            Moving = true;
        }

        public void StartDeadAnimation()
        {
            Dying = true;
        }
    }
}


/// 
/// state/count
/// 1/5 = .2
/// 
/// 
/// 

