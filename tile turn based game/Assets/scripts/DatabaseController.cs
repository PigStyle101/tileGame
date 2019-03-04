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
            Debug.Log("Fetching json files");
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
            Debug.Log("Fetching json files");
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

    public void spawnSomeShitTest(Vector2 location, int index)
    {

        GameObject TGO = new GameObject(); //create gameobject
        TGO.name = TerrainDictionary[index].Title; //change the name
        TGO.AddComponent<SpriteRenderer>(); //add a sprite controller
        Texture2D temptextur = loadTexture(TerrainDictionary[index].ArtworkDirectory[0]); //get the texture for the sprite
        TGO.GetComponent<SpriteRenderer>().sprite = Sprite.Create(temptextur, new Rect(0, 0, temptextur.width, temptextur.height), new Vector2(0, 0), 64); //set the sprite to the texture
        TGO.AddComponent<BoxCollider2D>(); //add a box collider

        GameObject MouseOverlayGO = new GameObject(); //creating the cild object for mouse overlay
        Texture2D tempmousetextur = loadTexture(MouseDictionary[index].ArtworkDirectory[0]); //getting the texture
        MouseOverlayGO.AddComponent<SpriteRenderer>().sprite = Sprite.Create(tempmousetextur, new Rect(0, 0, tempmousetextur.width, tempmousetextur.height), new Vector2(0, 0), 64); //adding it to sprite
        MouseOverlayGO.GetComponent<SpriteRenderer>().sortingOrder = 1; //making it so its on top of the default sprite
        MouseOverlayGO.transform.parent = TGO.transform; //setting its parent to the main game object
        MouseOverlayGO.name = "MouseOverlay"; //changing the name
        TGO.AddComponent<SpriteController>(); //adding the sprite controller script to it

        NewTile = Instantiate(TGO, location, Quaternion.Euler(0, 0, 0));
        NewTile.name = TerrainDictionary[index].Title;
    } //used to spawn objects from data base
    public Texture2D loadTexture(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))         // Load the imagedata into the texture (size is set automatically)
            {
                return Tex2D;                      // If data = readable -> return texture
            }
        }
        return null;
    }// checks for images that are to be used for artwork stuffs

}

public class Terrain //this needs to corolate with the json file info
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

