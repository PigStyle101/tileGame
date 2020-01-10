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
    public MenueController MMScript;
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

    private void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            LevelLoaded = true;
        }
    }

    public void Connect()
    {
        try
        {
            //Debug.Log("Connecting.....");
            TcpClnt.Connect("50.83.1.113", 34000); // uses ipaddress for the server program
        }
        catch (Exception e)
        {
            //Debug.Log("Could not connect to server.");
            //Debug.Log(e);
        }
        if (TcpClnt.Connected == true) // checks if connection was sucsessfull and runs connected script in main menu if it was.
        {
            //MMScript.Connected();
            //Debug.Log("Connected to server.");
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

    public void SendAddUserData(string usernme, string password)
    {
        try
        {
            if (usernme == "" || password == "")
            {
                //MMScript.AddUserResponseHandler("User name or password is blank.");
            }
            else
            {
                NetworkStream NtwrkStrm = TcpClnt.GetStream();
                IFormatter MyFormatter = new BinaryFormatter();

                //Debug.Log("AddUser Send Started");
                string type = "AddUser";
                MyFormatter.Serialize(NtwrkStrm, type);
                NtwrkStrm.Flush();
                MyFormatter.Serialize(NtwrkStrm, usernme);
                NtwrkStrm.Flush();
                MyFormatter.Serialize(NtwrkStrm, password);
                NtwrkStrm.Flush();
            }
            //UnityEngine.//Debug.Log("Failed to send");
        }
        catch (Exception e)
        {
            //Debug.Log(e);
        }
    }

    public void LoadCCLevel()
    {
        //SceneManager.LoadScene("Character_Pick_Creation");
    }//3

    public void LoadLevel()
    {
        //SceneManager.LoadScene(PlayerClass.SelectedHero.Area);
    }

    public void SendCCRequest(string name, string gender, string H)
    {
        NetworkStream NtwrkStrm = TcpClnt.GetStream();
        IFormatter MyFormatter = new BinaryFormatter();
        string type = "CCRequest";
        MyFormatter.Serialize(NtwrkStrm, type);
        NtwrkStrm.Flush();
        MyFormatter.Serialize(NtwrkStrm, name);
        NtwrkStrm.Flush();
        MyFormatter.Serialize(NtwrkStrm, gender);
        NtwrkStrm.Flush();
        MyFormatter.Serialize(NtwrkStrm, H);
        NtwrkStrm.Flush();
    }

    public void SendDeleteRequest(string H)
    {
        try
        {
            NetworkStream NtwrkStrm = TcpClnt.GetStream();
            IFormatter MyFormatter = new BinaryFormatter();
            MyFormatter.Serialize(NtwrkStrm, "DeleteRequest");
            NtwrkStrm.Flush();
            MyFormatter.Serialize(NtwrkStrm, H);
            NtwrkStrm.Flush();
        }
        catch (Exception e)
        {
            //Debug.Log(e);
            throw;
        }
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

    public void SendHeroSelectionNotice(string h)
    {
        NetworkStream NtwrkStrm = TcpClnt.GetStream();
        IFormatter MyFormatter = new BinaryFormatter();

        MyFormatter.Serialize(NtwrkStrm, "HeroSelectionRequest");
        NtwrkStrm.Flush();
        MyFormatter.Serialize(NtwrkStrm, h);
        NtwrkStrm.Flush();
    }

    public void SendLevelWasLoaded()
    {
        NetworkStream NtwrkStrm = TcpClnt.GetStream();
        IFormatter MyFormatter = new BinaryFormatter();

        MyFormatter.Serialize(NtwrkStrm, "LevelWasLoaded");
        NtwrkStrm.Flush();
    }

    public void SpawnPlayer(List<PlayerInfo> list)
    {
        try
        {
            //Debug.Log(list.Count);
            foreach (PlayerInfo player in list)
            {
                if (player.SelectedHero.Name != PlayerClass.SelectedHero.Name)
                {
                    if (!SpawnedPlayers.ContainsKey(player))
                    {
                        //Debug.Log("Spawning");
                        if (player.SelectedHero.Gender == "Male")
                        {
                            if (PlayerClass.SelectedHero.xPosition != 0 || PlayerClass.SelectedHero.yPosition != 0 || PlayerClass.SelectedHero.zPosition != 0)
                            {
                                GameObject SpawnedPlayer = Instantiate(MalePrefab, new Vector3(PlayerClass.SelectedHero.xPosition, PlayerClass.SelectedHero.yPosition, PlayerClass.SelectedHero.zPosition), Quaternion.Euler(0, PlayerClass.SelectedHero.Rotation, 0));
                                SpawnedPlayer.name = player.SelectedHero.Name;
                                SpawnedPlayers.Add(player, SpawnedPlayer);
                            }
                            else
                            {
                                GameObject SpawnedPlayer = Instantiate(MalePrefab, transform.position, Quaternion.Euler(0, 0, 0));
                                SpawnedPlayer.name = player.SelectedHero.Name;
                                SpawnedPlayers.Add(player, SpawnedPlayer);
                            }
                        }
                        else if (player.SelectedHero.Gender == "Female")
                        {
                            if (PlayerClass.SelectedHero.xPosition != 0 || PlayerClass.SelectedHero.yPosition != 0 || PlayerClass.SelectedHero.zPosition != 0)
                            {
                                GameObject SpawnedPlayer = Instantiate(FemalePrefab, new Vector3(PlayerClass.SelectedHero.xPosition, PlayerClass.SelectedHero.yPosition, PlayerClass.SelectedHero.zPosition), Quaternion.Euler(0, PlayerClass.SelectedHero.Rotation, 0));
                                SpawnedPlayer.name = player.SelectedHero.Name;
                                SpawnedPlayers.Add(player, SpawnedPlayer);
                            }
                            else
                            {
                                GameObject SpawnedPlayer = Instantiate(FemalePrefab, transform.position, Quaternion.Euler(0, 0, 0));
                                SpawnedPlayer.name = player.SelectedHero.Name;
                                SpawnedPlayers.Add(player, SpawnedPlayer);
                            }
                        }
                    }
                }
            }
            if (ClientSpawned != true)
            {
                if (PlayerClass.SelectedHero.Gender == "Male")
                {
                    ClientCharacter = Instantiate(MalePrefab, transform.position, Quaternion.Euler(0, 0, 0));
                    ClientCharacter.name = PlayerClass.SelectedHero.Name;
                    SpawnedPlayers.Add(PlayerClass, ClientCharacter);
                    ClientSpawned = true;
                    //OverlayController OLScript = ClientCharacter.GetComponent<OverlayController>();
                    //OLScript.ChangeBottomOverlayVusials();
                }
                else if (PlayerClass.SelectedHero.Gender == "Female")
                {
                    ClientCharacter = Instantiate(FemalePrefab, transform.position, Quaternion.Euler(0, 0, 0));
                    ClientCharacter.name = PlayerClass.SelectedHero.Name;
                    SpawnedPlayers.Add(PlayerClass, ClientCharacter);
                    ClientSpawned = true;
                    //OverlayController OLScript = ClientCharacter.GetComponent<OverlayController>();
                    //OLScript.ChangeBottomOverlayVusials();
                }
            }
        }
        catch (Exception e)
        {
            //Debug.Log(e);
            throw;
        }
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
                            LoadCCLevel();
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
                    else if (type == "CCRequest")
                    {
                        string responce = (string)MyFormatter.Deserialize(NtwrkStrm);
                        NtwrkStrm.Flush();

                        if (responce == "Accepted")
                        {
                            PlayerClass = (PlayerInfo)MyFormatter.Deserialize(NtwrkStrm);
                        }

                        //CCScript = GameObject.Find("CCCanvas").GetComponent<CharacterCreationScript>();
                        //CCScript.SubmitButtonClickedResponceHandler(responce);
                    }
                    else if (type == "HeroSelectionRequest")
                    {
                        PlayerClass = (PlayerInfo)MyFormatter.Deserialize(NtwrkStrm);
                        NtwrkStrm.Flush();
                        if (PlayerClass.SelectedHero == PlayerClass.H1)
                        {
                            //CCScript = GameObject.Find("CCCanvas").GetComponent<CharacterCreationScript>();
                            //CCScript.Character1ButtonClickedResponceHandler();
                        }
                        else if (PlayerClass.SelectedHero == PlayerClass.H2)
                        {
                            //CCScript = GameObject.Find("CCCanvas").GetComponent<CharacterCreationScript>();
                            //CCScript.Character2ButtonClickedResponceHandler();
                        }
                        else if (PlayerClass.SelectedHero == PlayerClass.H3)
                        {
                            //CCScript = GameObject.Find("CCCanvas").GetComponent<CharacterCreationScript>();
                            //CCScript.Character3ButtonClickedResponceHandler();
                        }
                        else if (PlayerClass.SelectedHero == PlayerClass.H4)
                        {
                            //CCScript = GameObject.Find("CCCanvas").GetComponent<CharacterCreationScript>();
                            //CCScript.Character4ButtonClickedResponceHandler();
                        }
                    }
                    else if (type == "DeleteRequest")
                    {
                        PlayerClass = (PlayerInfo)MyFormatter.Deserialize(NtwrkStrm);
                        NtwrkStrm.Flush();
                        //CCScript = GameObject.Find("CCCanvas").GetComponent<CharacterCreationScript>();
                        //CCScript.DeleteButtonClickedResponceHandler();
                    }
                    else if (type == "PlayersInScene")
                    {
                        Lisst = (List<PlayerInfo>)MyFormatter.Deserialize(NtwrkStrm);
                        SpawnPlayer(Lisst);
                    }
                    else if (type == "Player Joined Starting Area")
                    {
                        //Debug.Log("Player joined starting area");
                        Lisst.Add((PlayerInfo)MyFormatter.Deserialize(NtwrkStrm));
                        SpawnPlayer(Lisst);
                    }
                    else if (type == "LoadStartingArea")
                    {
                        PlayerClass.SelectedHero = (PlayerInfo.Hero)MyFormatter.Deserialize(NtwrkStrm);
                        LoadLevel();
                    }
                    else if (type == "Current Position")
                    {
                        //UnityEngine.//Debug.Log("current Position recieved");
                        string name = (string)MyFormatter.Deserialize(NtwrkStrm); //Recieve
                        NtwrkStrm.Flush();
                        float xin = (float)MyFormatter.Deserialize(NtwrkStrm);//Recieve
                        NtwrkStrm.Flush();
                        float yin = (float)MyFormatter.Deserialize(NtwrkStrm);//Recieve
                        NtwrkStrm.Flush();
                        float zin = (float)MyFormatter.Deserialize(NtwrkStrm);//Recieve
                        NtwrkStrm.Flush();
                        float yrot = (float)MyFormatter.Deserialize(NtwrkStrm);//Recieve
                        NtwrkStrm.Flush();
                        if (LevelLoaded)
                        {
                            //UnityEngine.//Debug.Log("lvlloaded = true");
                            if (ClientSpawned == true)
                            {
                                //UnityEngine.//Debug.Log("player spawned = true");
                                GameObject unit = GameObject.Find(name);
                                unit.transform.SetPositionAndRotation(new Vector3(xin, yin, zin), Quaternion.Euler(0, yrot, 0));
                            }
                        }
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
