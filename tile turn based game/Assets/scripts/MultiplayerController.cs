using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System.Threading;
using System.Linq;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using ClassForNetworking;

public class MultiplayerController : MonoBehaviour
{
    public static MultiplayerController instance = null;
    public TcpClient TcpClnt = new TcpClient();
    private GameObject plyermngerobject;
    public List<PlayerInfo> Lisst = new List<PlayerInfo>();
    public bool LevelLoaded;
    public PlayerInfo PlayerClass = new PlayerInfo();
    private Dictionary<PlayerInfo, GameObject> SpawnedPlayers = new Dictionary<PlayerInfo, GameObject>();
    public GameObject MalePrefab;
    public GameObject FemalePrefab;
    public bool ClientSpawned;
    private GameObject ClientCharacter;

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
        LevelLoaded = false;
    }

    private void FixedUpdate()
    {
        ServerResponceHandler();
    }

    public void Connect()
    {
        try
        {
            Debug.Log("Connecting.....");
            TcpClnt.Connect("50.83.15.160", 34000); // uses ipaddress for the server program
        }
        catch (Exception e)
        {
            Debug.Log("Could not connect to server.");
            Debug.Log(e);
        }
        if (TcpClnt.Connected == true) // checks if connection was sucsessfull and runs connected script in main menu if it was.
        {

            //MMScript.Connected();
            Debug.Log("Connected to server.");
        }
    } // trys to connect the player to the server

    public void SendLoginData(string UserNme, string Password)
    {

        try
        {
            if (UserNme == "" || Password == "")
            {
                //MMScript.LoginResponseHandler("User name or password is blank.");
            }
            else
            {
                NetworkStream NtwrkStrm = TcpClnt.GetStream();
                IFormatter MyFormatter = new BinaryFormatter();

                //Debug.Log("Login Send Started");
                string type = "Login";
                MyFormatter.Serialize(NtwrkStrm, type);
                NtwrkStrm.Flush();
                MyFormatter.Serialize(NtwrkStrm, UserNme);
                NtwrkStrm.Flush();
                MyFormatter.Serialize(NtwrkStrm, Password);
                NtwrkStrm.Flush();
            }
            //UnityEngine.//Debug.Log("Failed to send");
        }
        catch (Exception e)
        {
            //Debug.Log(e);
        }
    } //1

    public void LoadLevel()
    {
        //SceneManager.LoadScene(PlayerClass.SelectedHero.Area);
    }

    public void SendLoadLevelRequest()
    {
        try
        {
            NetworkStream NtwrkStrm = TcpClnt.GetStream();
            IFormatter MyFormatter = new BinaryFormatter();
            MyFormatter.Serialize(NtwrkStrm, "LoadLevelRequest");
            NtwrkStrm.Flush();
        }
        catch (Exception e)
        {
            //Debug.Log(e);
        }
    } // LL1

    public void SendLevelWasLoaded()
    {
        NetworkStream NtwrkStrm = TcpClnt.GetStream();
        IFormatter MyFormatter = new BinaryFormatter();

        MyFormatter.Serialize(NtwrkStrm, "LevelWasLoaded");
        NtwrkStrm.Flush();
    }

    public void ServerResponceHandler()
    {
        if (TcpClnt.Connected)
        {
            NetworkStream NtwrkStrm = TcpClnt.GetStream();
            IFormatter MyFormatter = new BinaryFormatter();
            if (NtwrkStrm.DataAvailable)
            {
                try
                {
                    string type = (string)MyFormatter.Deserialize(NtwrkStrm);
                    NtwrkStrm.Flush();
                    if (type == "Login")
                    {
                        //Debug.Log("Login Info Recived");
                        string response = (string)MyFormatter.Deserialize(NtwrkStrm);
                        NtwrkStrm.Flush();
                        //MMScript.LoginResponseHandler(response);
                        if (response == "Logging in")
                        {
                            PlayerClass = (PlayerInfo)MyFormatter.Deserialize(NtwrkStrm);
                            NtwrkStrm.Flush();
                            //Debug.Log("PlayerClass.Name = " + PlayerClass.UserName);
                            //LoadCCLevel();
                        }
                        return;
                    }//2
                    else if (type == "AddUser")
                    {
                        string response = (string)MyFormatter.Deserialize(NtwrkStrm);
                        NtwrkStrm.Flush();
                        //MMScript.AddUserResponseHandler(response);
                        return;
                    }
                    else if (type == "PlayersInScene")
                    {
                        Lisst = (List<PlayerInfo>)MyFormatter.Deserialize(NtwrkStrm);
                        //SpawnPlayer(Lisst);
                    }
                    else if (type == "Player Joined Starting Area")
                    {
                        //Debug.Log("Player joined starting area");
                        Lisst.Add((PlayerInfo)MyFormatter.Deserialize(NtwrkStrm));
                        //SpawnPlayer(Lisst);
                    }
                    else if (type == "LoadStartingArea")
                    {
                        PlayerClass.SelectedHero = (PlayerInfo.Hero)MyFormatter.Deserialize(NtwrkStrm);
                        LoadLevel();
                    }
                    else if (type == "Player Disconected")
                    {
                        PlayerInfo DCPlayer = (PlayerInfo)MyFormatter.Deserialize(NtwrkStrm);
                        foreach (KeyValuePair<PlayerInfo, GameObject> kvp in SpawnedPlayers)
                        {
                            if (kvp.Key.UserName == DCPlayer.UserName)
                            {
                                //Debug.Log("Removing Player " + kvp.Key.UserName + " from the game");
                                SpawnedPlayers.Remove(kvp.Key);
                                Destroy(kvp.Value);
                                Lisst.Remove(kvp.Key);
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    //Debug.Log(e);
                    NtwrkStrm.Flush();
                    return;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }
    }

    public void GetMainMenueScript()
    {
        //this is were we would find main menue if we need to
    }
}
