
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;

namespace TileBasedServer
{
    [Serializable]
    public class ServerController
    {
        //static private MySqlConnection connection;
        private static string exitString;
        private static Thread ShutdownThread;
        public static bool exitBool;
        //public static Dictionary<string, TcpClient> ConnectedClients = new Dictionary<string, TcpClient>();
        public static Dictionary<PlayerInfo, TcpClient> ClientsInStartArea = new Dictionary<PlayerInfo, TcpClient>();
        public static List<PlayerInfo> PlayersOnline = new List<PlayerInfo>();

        static void Main() // program starts form here
        {
            ShutdownThread = new Thread(Shutdown);//creat new thread and check if user has typed stop
            ShutdownThread.Start();
            ConnectToUnity.ClientConnectorTCP();
        }

        static void Shutdown()
        {
            do
            {
                exitString = Console.ReadLine();
                if (exitString == "stop")
                {
                    exitBool = true;
                }

            } while (!exitBool);

            Environment.Exit(0);
            //ConnectedClients.Clear();
            ClientsInStartArea.Clear();
            PlayersOnline.Clear();
        }//used to shut down the server


    }

    public class ConnectToUnity
    {
        public static void ClientConnectorTCP()
        {
            try
            {
                /* Initializes Variables */
                TcpListener ServerSocket = new TcpListener(IPAddress.Any, 34000);
                TcpClient ClientSocket = default(TcpClient);
                int ConnectedCount = 0;
                /* start Listener */
                ServerSocket.Start();
                /* start listening for clients */
                Console.WriteLine("The local End point is  :" + ServerSocket.LocalEndpoint);
                Console.WriteLine("Waiting for a connections.....");
                /* runs while loop to keep checking for new clients */
                while (!ServerController.exitBool)
                {
                    /* used to keep track of how many people have connected sense the server was started */
                    ConnectedCount++;
                    /* code will pause here untell a connection comes through */
                    ClientSocket = ServerSocket.AcceptTcpClient();
                    Console.WriteLine("Client No : " + Convert.ToString(ConnectedCount) + " connected to TCP.");
                    /* sends client info to handler */
                    HandleClientsTCP hndleclient = new HandleClientsTCP();
                    hndleclient.HandleClientComm(ClientSocket);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        } /* used to wait for clients to connect, then starts new thread for each client */
    }

    [Serializable]
    public class HandleClientsUDP
    {

    }

    [Serializable]
    public class HandleClientsTCP
    {
        /* initalizes variables */
        TcpClient ClientSocket;
        string type;

        public void HandleClientComm(TcpClient ClntSoc)
        {
            ClientSocket = ClntSoc;
            Thread ClientThread = new Thread(DataManager);
            ClientThread.Start();
        } /* new thread starts here */


        public void DataManager() /* This is new thread */
        {
            /* initalizes variables */
            NetworkStream NtwrkStrm = ClientSocket.GetStream();
            IFormatter MyFormatter = new BinaryFormatter();
            DataBaseHandlerTCP Dbh = new DataBaseHandlerTCP();
            PlayerInfo PlayerClass = new PlayerInfo();
            /* starts loop to check for data being recieved from client */
            while (true)
            {
                try
                {
                    /* client will send data in this order: type of data, info,info,info........ */
                    type = (string)MyFormatter.Deserialize(NtwrkStrm);//Recieve
                    NtwrkStrm.Flush();
                    /* type of data is what i use to know where to send the data on the server side */
                    if (type == "Login")
                    {
                        /* client is attempting to log in, next 7 lines get the data */
                        Console.WriteLine("Type of Data is " + type);
                        string DataUserName = (string)MyFormatter.Deserialize(NtwrkStrm);
                        NtwrkStrm.Flush();
                        string DataPassword = (string)MyFormatter.Deserialize(NtwrkStrm);
                        Console.WriteLine("Checking if user name " + DataUserName + " is already online");
                        NtwrkStrm.Flush();
                        /* gets the responce form methode that checks if a client is already logged into this username and password */
                        string responce = Dbh.CheckIfPlayerIsOnline(DataUserName);

                        if (responce == "Player is not logged in")
                        {
                            /* checks if username and password are in database */
                            responce = Dbh.CheckIfUserExist(DataUserName, DataPassword);
                            if (responce == "Logging in")
                            {
                                /* if user succsefuly loggs on saves the username for later and adds player to currently online players */
                                PlayerClass = Dbh.GetPlayerInfo(DataUserName);
                                Dbh.AddOnlinePlayer(PlayerClass);
                                /* send responce back to the client if responce is = logging in*/
                                Console.WriteLine(responce);
                                MyFormatter.Serialize(NtwrkStrm, type);
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(NtwrkStrm, responce);
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                                NtwrkStrm.Flush();
                            }
                            else
                            {/* send responce back to the client if responce is != logging in , but dont send player info*/
                                MyFormatter.Serialize(NtwrkStrm, type);
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(NtwrkStrm, responce);
                                NtwrkStrm.Flush();
                            }
                        }
                        else
                        {
                            /* sends responce back to the client saying someone is already logged into that username*/
                            Console.WriteLine(responce);
                            MyFormatter.Serialize(NtwrkStrm, type);
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, responce);
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, DataUserName);
                            NtwrkStrm.Flush();
                        }

                    }
                    else if (type == "AddUser") /* the rest of the else if do the same as the first, handle the data. */
                    {
                        Console.WriteLine("Type of Data is " + type);
                        string DataUserName = (string)MyFormatter.Deserialize(NtwrkStrm);
                        NtwrkStrm.Flush();
                        string DataPassword = (string)MyFormatter.Deserialize(NtwrkStrm);
                        Console.WriteLine("Checking if user name " + DataUserName + " is in the database");
                        NtwrkStrm.Flush();
                        string responce = Dbh.AddUser(DataUserName, DataPassword);
                        //send responce back
                        Console.WriteLine(responce);
                        MyFormatter.Serialize(NtwrkStrm, type);
                        NtwrkStrm.Flush();
                        MyFormatter.Serialize(NtwrkStrm, responce);
                        NtwrkStrm.Flush();
                    }
                    else if (type == "CCRequest")
                    {
                        string name = (string)MyFormatter.Deserialize(NtwrkStrm);
                        NtwrkStrm.Flush();
                        string gender = (string)MyFormatter.Deserialize(NtwrkStrm);
                        NtwrkStrm.Flush();
                        string H = (string)MyFormatter.Deserialize(NtwrkStrm);
                        NtwrkStrm.Flush();
                        if (H == "H1" && PlayerClass.H1 == null)
                        {
                            PlayerClass.H1 = new PlayerInfo.Hero();
                            PlayerClass.H1.Name = name;
                            PlayerClass.H1.Gender = gender;

                            MyFormatter.Serialize(NtwrkStrm, type);
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, "Accepted");
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                            NtwrkStrm.Flush();
                            Console.WriteLine("Player " + PlayerClass.UserName + " created a hero in " + H);
                        }
                        else if (H == "H2" && PlayerClass.H2 == null)
                        {
                            PlayerClass.H2 = new PlayerInfo.Hero();
                            PlayerClass.H2.Name = name;
                            PlayerClass.H2.Gender = gender;

                            MyFormatter.Serialize(NtwrkStrm, type);
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, "Accepted");
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                            NtwrkStrm.Flush();
                            Console.WriteLine("Player " + PlayerClass.UserName + " created a hero in " + H);
                        }
                        else if (H == "H3" && PlayerClass.H3 == null)
                        {
                            PlayerClass.H3 = new PlayerInfo.Hero();
                            PlayerClass.H3.Name = name;
                            PlayerClass.H3.Gender = gender;

                            MyFormatter.Serialize(NtwrkStrm, type);
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, "Accepted");
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                            NtwrkStrm.Flush();
                            Console.WriteLine("Player " + PlayerClass.UserName + " created a hero in " + H);
                        }
                        else if (H == "H4" && PlayerClass.H4 == null)
                        {
                            PlayerClass.H4 = new PlayerInfo.Hero();
                            PlayerClass.H4.Name = name;
                            PlayerClass.H4.Gender = gender;

                            MyFormatter.Serialize(NtwrkStrm, type);
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, "Accepted");
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                            NtwrkStrm.Flush();
                            Console.WriteLine("Player " + PlayerClass.UserName + " created a hero in " + H);
                        }
                        else
                        {
                            MyFormatter.Serialize(NtwrkStrm, type);
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, "Declined");
                            NtwrkStrm.Flush();
                            Console.WriteLine("Player " + PlayerClass.UserName + " failed to create a hero in " + H);
                        }
                    }
                    else if (type == "HeroSelectionRequest")
                    {
                        string h = (string)MyFormatter.Deserialize(NtwrkStrm);
                        if (h == "H1")
                        {
                            if (PlayerClass.H1 != null)
                            {
                                PlayerClass.SelectedHero = PlayerClass.H1;
                                MyFormatter.Serialize(NtwrkStrm, "HeroSelectionRequest");
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                                NtwrkStrm.Flush();
                            }
                        }
                        else if (h == "H2")
                        {
                            if (PlayerClass.H2 != null)
                            {
                                PlayerClass.SelectedHero = PlayerClass.H2;
                                MyFormatter.Serialize(NtwrkStrm, "HeroSelectionRequest");
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                                NtwrkStrm.Flush();
                            }
                        }
                        else if (h == "H3")
                        {
                            if (PlayerClass.H3 != null)
                            {
                                PlayerClass.SelectedHero = PlayerClass.H3;
                                MyFormatter.Serialize(NtwrkStrm, "HeroSelectionRequest");
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                                NtwrkStrm.Flush();
                            }
                        }
                        else if (h == "H3")
                        {
                            if (PlayerClass.H4 != null)
                            {
                                PlayerClass.SelectedHero = PlayerClass.H3;
                                MyFormatter.Serialize(NtwrkStrm, "HeroSelectionRequest");
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                                NtwrkStrm.Flush();
                            }
                        }
                    }
                    else if (type == "DeleteRequest")
                    {
                        string H = (string)MyFormatter.Deserialize(NtwrkStrm);
                        NtwrkStrm.Flush();
                        if (H == "H1" && PlayerClass.H1 != null)
                        {
                            Console.WriteLine(PlayerClass.UserName + " deleted " + PlayerClass.H1.Name + " from slot " + H);
                            PlayerClass.H1 = null;
                            MyFormatter.Serialize(NtwrkStrm, type);
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                            NtwrkStrm.Flush();
                        }
                        else if (H == "H2" && PlayerClass.H2 != null)
                        {
                            Console.WriteLine(PlayerClass.UserName + " deleted " + PlayerClass.H2.Name + " from slot " + H);
                            PlayerClass.H2 = null;
                            MyFormatter.Serialize(NtwrkStrm, type);
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                            NtwrkStrm.Flush();
                        }
                        else if (H == "H3" && PlayerClass.H3 != null)
                        {
                            Console.WriteLine(PlayerClass.UserName + " deleted " + PlayerClass.H3.Name + " from slot " + H);
                            PlayerClass.H3 = null;
                            MyFormatter.Serialize(NtwrkStrm, type);
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                            NtwrkStrm.Flush();
                        }
                        else if (H == "H4" && PlayerClass.H4 != null)
                        {
                            Console.WriteLine(PlayerClass.UserName + " deleted " + PlayerClass.H4.Name + " from slot " + H);
                            PlayerClass.H4 = null;
                            MyFormatter.Serialize(NtwrkStrm, type);
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, PlayerClass);
                            NtwrkStrm.Flush();
                        }
                    }
                    else if (type == "LoadLevelRequest")
                    {
                        if (PlayerClass.SelectedHero.Area == null || PlayerClass.SelectedHero.Area == "Starting_Area")
                        {
                            PlayerClass.SelectedHero.Area = "Starting_Area";
                            MyFormatter.Serialize(NtwrkStrm, "LoadStartingArea");
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, PlayerClass.SelectedHero);
                            NtwrkStrm.Flush();

                            foreach (KeyValuePair<PlayerInfo, TcpClient> Kvp in ServerController.ClientsInStartArea)
                            {
                                if (Kvp.Key.UserName != PlayerClass.UserName)
                                {
                                    MyFormatter.Serialize(Kvp.Value.GetStream(), "Player Joined Starting Area");
                                    Kvp.Value.GetStream().Flush();
                                    MyFormatter.Serialize(Kvp.Value.GetStream(), PlayerClass);
                                    Kvp.Value.GetStream().Flush();
                                }
                            }
                        }
                    }
                    else if (type == "LevelWasLoaded")
                    {
                        if (PlayerClass.SelectedHero.Area == "Starting_Area")
                        {
                            Dbh.SceneManagerAdder(PlayerClass, ClientSocket);
                            List<PlayerInfo> responce = Dbh.CheckForPlayerInScene(PlayerClass.SelectedHero.Area) as List<PlayerInfo>;
                            MyFormatter.Serialize(NtwrkStrm, "PlayersInScene");
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, responce);
                            NtwrkStrm.Flush();
                        }
                    }
                    else if (type == "Current Position")
                    {
                        //Console.WriteLine("Type of data is " + type);
                        string name = (string)MyFormatter.Deserialize(NtwrkStrm); //receive
                        NtwrkStrm.Flush();
                        PlayerClass.SelectedHero.xPosition = (float)MyFormatter.Deserialize(NtwrkStrm); //receive
                        NtwrkStrm.Flush();
                        PlayerClass.SelectedHero.yPosition = (float)MyFormatter.Deserialize(NtwrkStrm); //receive
                        NtwrkStrm.Flush();
                        PlayerClass.SelectedHero.zPosition = (float)MyFormatter.Deserialize(NtwrkStrm); //receive
                        NtwrkStrm.Flush();
                        PlayerClass.SelectedHero.Rotation = (float)MyFormatter.Deserialize(NtwrkStrm); //receive
                        NtwrkStrm.Flush();
                        foreach (KeyValuePair<PlayerInfo, TcpClient> KvP in ServerController.ClientsInStartArea)
                        {
                            if (KvP.Key.SelectedHero.Name != name)
                            {
                                MyFormatter.Serialize(KvP.Value.GetStream(), type); //Send
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(KvP.Value.GetStream(), PlayerClass.SelectedHero.Name); //Send
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(KvP.Value.GetStream(), PlayerClass.SelectedHero.xPosition); //Send
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(KvP.Value.GetStream(), PlayerClass.SelectedHero.yPosition); //Send
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(KvP.Value.GetStream(), PlayerClass.SelectedHero.zPosition); //Send
                                NtwrkStrm.Flush();
                                MyFormatter.Serialize(KvP.Value.GetStream(), PlayerClass.SelectedHero.Rotation); //Send
                                NtwrkStrm.Flush();
                            }
                        }
                    }
                }
                catch (Exception e) // if client disconnects stops the thread and tells the console
                {
                    Console.WriteLine("Client " + PlayerClass.UserName + " lost");
                    Dbh.RemoveFromOnlinePLayer(PlayerClass);
                    Dbh.SceneManagerRemover(PlayerClass);
                    Dbh.SavePlayerDataOnDisconect(PlayerClass);
                    //Console.WriteLine(e);
                    return;
                }
            }
        }
    }

    [Serializable]
    public class DataBaseHandlerTCP
    {
        Dictionary<string, string> UserNamesAndPasswords = new Dictionary<string, string>();
        IFormatter MyFormatter = new BinaryFormatter();

        public string CheckIfUserExist(string Login, string Pass)
        {
            try
            {
                if (File.Exists("UserNameAndPasswords"))
                {
                    FileStream stream = File.OpenRead("UserNameAndPasswords");
                    if (stream.Length != 0)
                    {
                        UserNamesAndPasswords = (Dictionary<string, string>)MyFormatter.Deserialize(stream);
                    }
                    stream.Flush();
                    stream.Dispose();
                    if (UserNamesAndPasswords.ContainsKey(Login))
                    {
                        string value;
                        if (UserNamesAndPasswords.TryGetValue(Login, out value))
                        {
                            if (value == Pass)
                            {
                                return "Logging in";
                            }
                            else
                            {
                                return "Password incorect";
                            }
                        }
                        else
                        {
                            return "Could not get password";
                        }
                    }
                    else
                    {
                        return "User name does not exist";
                    }
                }
                else
                {
                    FileStream stream = File.OpenWrite("UserNameAndPasswords");
                    MyFormatter.Serialize(stream, UserNamesAndPasswords);
                    stream.Flush();
                    stream.Dispose();
                    return "YAAAYYY!!! You are the first person to ever use the server!! Sadly you must create an account befor you log in.";

                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                throw;
            }
        } // runs when client attempts to log in

        public string AddUser(string UserName, string Password)
        {
            try
            {
                if (File.Exists("UserNameAndPasswords"))
                {
                    FileStream stream = File.OpenRead("UserNameAndPasswords");
                    //checks to make sure file is not empty befor trying to deseralize it
                    if (stream.Length != 0)
                    {
                        UserNamesAndPasswords = (Dictionary<string, string>)MyFormatter.Deserialize(stream);
                        stream.Flush();
                        stream.Dispose();
                        if (!UserNamesAndPasswords.ContainsKey(UserName))
                        {
                            UserNamesAndPasswords.Add(UserName, Password);
                            Console.WriteLine("user name: " + UserName + " and password added");
                            FileStream streem = File.OpenWrite("UserNameAndPasswords");
                            MyFormatter.Serialize(streem, UserNamesAndPasswords);
                            streem.Flush();
                            streem.Dispose();
                            return "Username Created";
                        }
                        else
                        {
                            return "Username is already taken";
                        }
                    }
                    else
                    {
                        UserNamesAndPasswords.Add(UserName, Password);
                        FileStream streem = File.OpenWrite("UserNameAndPasswords");
                        MyFormatter.Serialize(streem, UserNamesAndPasswords);
                        Console.WriteLine("User name and password file did exist, but was empty, added user");
                        streem.Flush();
                        streem.Dispose();
                        return "Username Created";
                    }


                }
                else
                {
                    Console.WriteLine("User name and password file did not exist, one was created");
                    FileStream stream = File.OpenWrite("UserNameAndPasswords");
                    UserNamesAndPasswords.Add(UserName, Password);
                    MyFormatter.Serialize(stream, UserNamesAndPasswords);
                    stream.Flush();
                    stream.Dispose();
                    return "Username Created: " + UserName;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        } // runs when client attempts to creat username and password

        public void AddOnlinePlayer(PlayerInfo playerClass)
        {
            try
            {
                ServerController.PlayersOnline.Add(playerClass);
                Console.WriteLine("Added " + playerClass.UserName + " to Players Online");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        } // adds player to dictionary containing all the players currently online

        public void RemoveFromOnlinePLayer(PlayerInfo playerInfosRFOP)
        {
            try
            {
                if (ServerController.ClientsInStartArea.ContainsKey(playerInfosRFOP))
                {
                    foreach (KeyValuePair<PlayerInfo, TcpClient> kvp in ServerController.ClientsInStartArea)
                    {
                        if (kvp.Key != playerInfosRFOP)
                        {
                            NetworkStream NtwrkStrm = kvp.Value.GetStream();
                            MyFormatter.Serialize(NtwrkStrm, "Player Disconected");
                            NtwrkStrm.Flush();
                            MyFormatter.Serialize(NtwrkStrm, playerInfosRFOP);
                            NtwrkStrm.Flush();
                            //NtwrkStrm.Dispose();
                        }
                    }
                }
                ServerController.PlayersOnline.Remove(playerInfosRFOP);
                Console.WriteLine("Removed " + playerInfosRFOP.UserName + " from Players Online");

            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        } // does what it says

        public string CheckIfPlayerIsOnline(string username)
        {
            try
            {
                foreach (PlayerInfo PF in ServerController.PlayersOnline)
                {
                    if (PF.UserName == username)
                    {
                        return "Player is already Logged in";
                    }
                }
                return "Player is not logged in";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Player is already Logged in";
            }
        } // checks to make sure 2 people are not logged in with the same username

        public void SceneManagerRemover(PlayerInfo playerClass)
        {
            try
            {
                if (playerClass.SelectedHero != null)
                {
                    if (playerClass.SelectedHero.Area == "Starting_Area")
                    {
                        if (ServerController.ClientsInStartArea.ContainsKey(playerClass))
                        {
                            ServerController.ClientsInStartArea.Remove(playerClass);
                            Console.WriteLine("Removed " + playerClass.UserName + " from " + playerClass.SelectedHero.Area);
                        }
                        else
                        {
                            Console.WriteLine(ServerController.ClientsInStartArea.Count);
                            foreach (KeyValuePair<PlayerInfo, TcpClient> kvp in ServerController.ClientsInStartArea)
                            {
                                Console.WriteLine(kvp.Key);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Scene is: " + playerClass.SelectedHero.Area);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //Console.WriteLine(playerClass.UserName + " - " + playerClass.SelectedHero.Area);
            }
        } // removes user when they leave the scene or disconnect

        public List<PlayerInfo> CheckForPlayerInScene(string scene)
        {
            List<PlayerInfo> TempListToSendToClient = new List<PlayerInfo>();
            try
            {
                if (scene == "Starting_Area")
                {
                    foreach (KeyValuePair<PlayerInfo, TcpClient> keyVpair in ServerController.ClientsInStartArea)
                    {
                        TempListToSendToClient.Add(keyVpair.Key);
                    }
                    return TempListToSendToClient;
                }
                throw new Exception();
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                throw;
            }
        } // checks who is in what scene and returns it to the client

        public void SavePlayerDataOnDisconect(PlayerInfo player)
        {
            try
            {
                if (player.SelectedHero != null)
                {
                    if (player.SelectedHero.Name == player.H1.Name)
                    {
                        player.H1 = player.SelectedHero;
                        player.SelectedHero = null;
                    }
                    else if (player.SelectedHero.Name == player.H2.Name)
                    {
                        player.H2 = player.SelectedHero;
                        player.SelectedHero = null;
                    }
                    else if (player.SelectedHero.Name == player.H3.Name)
                    {
                        player.H3 = player.SelectedHero;
                        player.SelectedHero = null;
                    }
                    else if (player.SelectedHero.Name == player.H4.Name)
                    {
                        player.H4 = player.SelectedHero;
                        player.SelectedHero = null;
                    }
                    if (player.UserName != null)
                    {
                        FileStream stream = File.OpenWrite(player.UserName);
                        player.FileCreated = true;
                        MyFormatter.Serialize(stream, player);
                        stream.Dispose();
                        Console.WriteLine("Player file " + player.UserName + " saved");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        } //saves players current info if player disconnects or quits

        public PlayerInfo GetPlayerInfo(string name)
        {
            try
            {
                if (File.Exists(name))
                {
                    FileStream stream = File.OpenRead(name);
                    PlayerInfo player = (PlayerInfo)MyFormatter.Deserialize(stream);
                    stream.Dispose();
                    Console.WriteLine("Player file " + player.UserName + " loaded");
                    return player;
                }
                else
                {
                    Console.WriteLine("File " + name + " does not exsist, it will be created when the player loggs off or is disconected");
                    PlayerInfo player = new PlayerInfo(name);
                    return player;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        } //loads player info when player logs in

        public void SceneManagerAdder(PlayerInfo player, TcpClient ClientTcp)
        {
            try
            {
                if (player.SelectedHero.Area == "Starting_Area")
                {
                    ServerController.ClientsInStartArea.Add(player, ClientTcp);
                    Console.WriteLine("Added " + player.UserName + " to " + player.SelectedHero.Area);
                }
                else
                {
                    Console.WriteLine("Failed to add " + player.UserName + " to " + player.SelectedHero.Area);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } // adds user to whatever scene he is logging into
    }

    [Serializable]
    public class PlayerInfo
    {
        public string UserName { get; set; }
        public bool FileCreated { get; set; }
        public Hero H1 { get; set; }
        public Hero H2 { get; set; }
        public Hero H3 { get; set; }
        public Hero H4 { get; set; }
        public Hero SelectedHero { get; set; }

        [Serializable]
        public class Hero
        {
            public string Name { get; set; }
            public string Gender { get; set; }
            public int Level { get; set; }
            public int XP { get; set; }
            public float Health { get; set; }
            public String Area { get; set; }
            public float xPosition { get; set; }
            public float yPosition { get; set; }
            public float zPosition { get; set; }
            public float Rotation { get; set; }
        }
        public PlayerInfo()
        {

        }

        public PlayerInfo(string name)
        {
            UserName = name;
        }
    }
}
