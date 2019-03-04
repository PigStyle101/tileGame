using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class DatabaseController : MonoBehaviour {
    public Dictionary<int, Terrain> TerrainDictionary = new Dictionary<int, Terrain>();
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
            foreach (string file in Directory.GetFiles(Application.dataPath + "/StreamingAssets/Terrain/Data/", "*.json"))
            {
                var grassstring = File.ReadAllText(file);
                var tempjson = JsonUtility.FromJson<Terrain>(grassstring);
                Debug.Log("Adding: " + tempjson.Slug + " to database");
                TerrainDictionary.Add(tempjson.ID, tempjson);
                tempjson.GetSprites();
                Debug.Log("Finished adding: " + tempjson.Slug + " to database");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }

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
    }

    public void spawnSomeShitTest(Vector2 location, int index)
    {

        GameObject TGO = new GameObject();
        TGO.name = TerrainDictionary[index].Title;
        TGO.AddComponent<SpriteRenderer>();
        Texture2D temptextur = loadTexture(TerrainDictionary[index].ArtworkDirectory[0]);
        TGO.GetComponent<SpriteRenderer>().sprite = Sprite.Create(temptextur, new Rect(0, 0, temptextur.width, temptextur.height), new Vector2(0, 0), 64);
        TGO.AddComponent<BoxCollider2D>();
        TGO.AddComponent<snaptogrid>();
        NewTile = Instantiate(TGO, location, Quaternion.Euler(0, 0, 0));
        NewTile.name = "Grass";
    }

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
    }

}

public class Terrain
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
        int count = new int();
        foreach (string file in (Directory.GetFiles(Application.dataPath + "/StreamingAssets/Terrain/Sprites/", "*.png")))
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
}

