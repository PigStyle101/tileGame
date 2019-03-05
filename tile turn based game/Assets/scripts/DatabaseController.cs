using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class DatabaseController : MonoBehaviour {
    public Dictionary<int, Terrain> TerrainDictionary = new Dictionary<int, Terrain>(); //stores the classes based on there id
    public Dictionary<int, MouseOverlays> MouseDictionary = new Dictionary<int, MouseOverlays>();
    private GameObject NewTile;

    private void Start()
    {
        GetTerrianJsons();
        GetMouseJson();
    }

    public void GetTerrianJsons()
    {
        try
        {
            Debug.Log("Fetching json terrain files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Terrain/Data/", "*.json")) //gets only json files form this path
            {
                var grassstring = File.ReadAllText(file); //temp string to hold the json data
                var tempjson = JsonUtility.FromJson<Terrain>(grassstring); //this converts from json string to unity object
                Debug.Log("Adding: " + tempjson.Slug + " to database");
                TerrainDictionary.Add(tempjson.ID, tempjson); //adds
                tempjson.GetSprites(); //details in fucntion
                Debug.Log("Finished adding: " + tempjson.Slug + " to database");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }//gets the json files from the terrain/data folder and converts them to unity object and stores tehm into dictionary

    public void GetMouseJson()
    {
        try
        {
            Debug.Log("Fetching json mouse overlay files");
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/MouseOverlays/Data/", "*.json"))
            {
                var TempString = File.ReadAllText(file);
                var tempjson = JsonUtility.FromJson<MouseOverlays>(TempString);
                Debug.Log("Adding: " + tempjson.Slug + " to database");
                MouseDictionary.Add(tempjson.ID, tempjson);
                tempjson.GetSprites();
                Debug.Log("Finished adding: " + tempjson.Slug + " to database");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }//same as getTerrainJson

    public void CreateAdnSpawnTerrain(Vector2 location, int index)
    {

        GameObject TGO = new GameObject();                                                                                  //create gameobject
        TGO.name = TerrainDictionary[index].Title;                                                                          //change the name
        TGO.AddComponent<SpriteRenderer>();                                                                                 //add a sprite controller
        TGO.GetComponent<SpriteRenderer>().sprite = loadSprite(TerrainDictionary[index].ArtworkDirectory[0]);               //set the sprite to the texture
        TGO.AddComponent<BoxCollider2D>();                                                                                  //add a box collider
                                                                                                                            //////// JUST WORK DAMB IT///////////
        GameObject MouseOverlayGO = new GameObject();                                                                       //creating the cild object for mouse overlay
        MouseOverlayGO.AddComponent<SpriteRenderer>().sprite = loadSprite(MouseDictionary[0].ArtworkDirectory[0]);          //adding it to sprite
        MouseOverlayGO.GetComponent<SpriteRenderer>().sortingOrder = 1;                                                     //making it so its on top of the default sprite
        MouseOverlayGO.transform.parent = TGO.transform;                                                                    //setting its parent to the main game object
        MouseOverlayGO.name = "MouseOverlay";                                                                               //changing the name
        TGO.AddComponent<SpriteController>();                                                                               //adding the sprite controller script to it

        GameObject MouseOverlaySelectedGO = new GameObject();                                                               //creating the cild object for mouse overlay selected
        MouseOverlaySelectedGO.AddComponent<SpriteRenderer>().sprite = loadSprite(MouseDictionary[0].ArtworkDirectory[1]);  //adding it to sprite
        MouseOverlaySelectedGO.GetComponent<SpriteRenderer>().sortingOrder = 1;                                             //making it so its on top of the default sprite
        MouseOverlaySelectedGO.transform.parent = TGO.transform;                                                            //setting its parent to the main game object
        MouseOverlaySelectedGO.name = "MouseOverlaySelected";                                                               //changing the name

        NewTile = Instantiate(TGO, location, Quaternion.Euler(0, 0, 0));
        NewTile.name = TerrainDictionary[index].Title;
    } //used to spawn objects from data base          //////EVERY OTHER TILE SPAWNED IS AT 0,0 DONT KNOW WHY.....

    public Sprite loadSprite(string FilePath)
    {
        Texture2D Tex2D;
        Sprite TempSprite;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            Tex2D.LoadImage(FileData);         // Load the imagedata into the texture (size is set automatically)
            TempSprite = Sprite.Create(Tex2D, new Rect(0, 0, Tex2D.width, Tex2D.height), new Vector2(0, 0), 64);
            return TempSprite;
        }
        return null;
    }// checks for images that are to be used for artwork stuffs

}

public class Terrain //the json file cannot have values that are not stated here, this can have more values then the json
{
    public int ID;
    public string Title;
    public bool Walkable;
    public string Description;
    public int DefenceBonus;
    public string Slug;
    public List<string> ArtworkDirectory;


    public void GetSprites()
    {
        Debug.Log("Getting sprites for: " + Title);
        int count = new int(); //used for debug
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Terrain/Sprites/", "*.png"))) //could actaully put all png and json in same folder... idea for later
        {
            if (file.Contains(Title)) //checks if any of the files found contain the name of the object
            {
                ArtworkDirectory.Add(file); //adds if they do
                count = count + 1;
            }
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
        Debug.Log("Sprites found: " + count);
        count = 0;
    }  //checks how many sprites are in folder and adds them to the directory
}

public class MouseOverlays
{
    public int ID;
    public string Title;
    public string Slug;
    public List<string> ArtworkDirectory;

    public void GetSprites()
    {
        Debug.Log("Getting sprites for: " + Title);
        int count = new int();
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/MouseOverlays/Sprites", "*.png")))
        {
            if (file.Contains(Title))
            {
                ArtworkDirectory.Add(file);
                count = count + 1;
            }
            //var tempname = Path.GetFileNameWithoutExtension(file);  // use this to get file name
        }
        Debug.Log("Sprites found: " + count);
        count = 0;
    }
}//same use as terrian class

