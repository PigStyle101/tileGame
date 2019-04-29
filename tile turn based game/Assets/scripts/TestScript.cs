using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //var MasterString = File.ReadAllText(Application.dataPath + "/StreamingAssets/MasterData/Master.json");
        //Debug.Log(MasterString);

        //string tempjson = JsonHelper.ToJson(save, true);
        FileStream fs = File.Create(Application.dataPath + "/StreamingAssets/MasterData/Master.json");
        StreamWriter sr = new StreamWriter(fs);
        //sr.Write(tempjson);
        sr.Close();
        sr.Dispose();
        fs.Close();
        fs.Dispose();
    } 

    [System.Serializable]
    public class MasterData
    {
        public string[] Classes;

    }
}
