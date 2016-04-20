using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Data.SQLite;
using uLink;
using CodeStage.AntiCheat.Detectors;

namespace TediousAntiCheat
{
    public class Main : UnityEngine.MonoBehaviour
    {
        //Controllers / Managers.
        DataBaseManager Database;
        ServerManager Server;
        NetworkController NetworkController;

        //Arrays.
        PlayerManager[] Players;
        Ressource[] Resources;
        Vehicle[] Vehicles;

        //Bools.
        bool DevMenu;
        bool PlayerMenu;
        bool VehicleMenu;
        bool ResourceMenu;
        bool Speedhack;
        bool pEsp;
        bool vEsp;
        bool rEsp;

        //Strings.
        string searchAdminMenu = "";
        string amount = "Item Amount.";
        string itemid = "Item ID.";
        string ammo = "Ammo if needed, 0 otherwise.";
        string steamid = "SteamID";
        List<string> List = new List<string>() { "" };

        //Floats, doubles, ints.
        float nextUpdateTime;
        float speed = 1f;
        float playerdist = 1000f;
        float vehicledist = 1000f;
        float resourcedist = 1000f;

        //GUI Stuff.
        GUIStyle AdminSkin = new GUIStyle();
        public Rect AdminMenu = new Rect(20, 500, 230, 300);
        public Rect MainWin = new Rect(50, 50, 500, 300);

        //Vectors.
        Vector2 scrollPosition;

        //Function to kill player selected.
        //Uses Network.RPC so we can fool the server and let the command run.
        void killFunction(PlayerManager player)
        {
            if (GUILayout.Button("Kill"))
            {
                //Damage, where from, string on kill message, string in chat.
                player.networkView.RPC("NET_TakeDamage", uLink.RPCMode.All, new object[] { 1000000, player.transform.position, "You've been slain by an admin.", " <color=red>was slain by an admin.</color>" });
            }
        }

        //Function to teleport to player selected.
        //We don't need to use Network.RPC to change our position.
        void teleToFunction(PlayerManager player)
        {
            if (GUILayout.Button("Teleport To "+player.CONTEXT_name))
            {
                //PlayerManager.length - 1 is us.
                Players[Players.Length - 1].Transfo.position = player.Transfo.position;
            }
        }

        //Function to teleport player selected to us.
        //We need to use Network.RPC to do anything to other players.
        void teleToMeFunction(PlayerManager player)
        {
            if (GUILayout.Button("Teleport To Me"))
            {
                Transform transform = Players[Players.Length - 1].Transfo;
                Vector3 vector = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                player.networkView.RPC("NET_TeleportToPos", uLink.RPCMode.All, new object[] { vector });
            }
        }

        //Main window that contains all our buttons and gizmos.
        void MainWindow(int id)
        {
            //Toggle player menu.
            if (GUI.Button(new Rect(10f, 30f, 80f, 20f), "PlayerM"))
            {
                PlayerMenu = !PlayerMenu;
            }
            //Toggle vehicle menu.
            if (GUI.Button(new Rect(10f, 60f, 80f, 20f), "VehicleM"))
            {
                VehicleMenu = !VehicleMenu;
            }
            //Toggle resource menu.
            if (GUI.Button(new Rect(10f, 90f, 80f, 20f), "ResourceM"))
            {
                ResourceMenu = !ResourceMenu;
            }
            //Bring MOST players. I say this because PlayerManager doesn't store all the players?
            if (GUI.Button(new Rect(10f, 120f, 80f, 20f), "Bring All"))
            {
                PlayerManager[] playerArray = Players;
                foreach (PlayerManager Player in playerArray)
                {
                    Transform transform = Players[Players.Length - 1].Transfo;
                    Vector3 vector = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    Player.networkView.RPC("NET_TeleportToPos", uLink.RPCMode.All, new object[] { vector });
                }
            }
            //Kill MOST players. I say this because PlayerManager doesn't store all the players?
            if (GUI.Button(new Rect(10f, 150f, 80f, 20f), "Kill All"))
            {
                PlayerManager[] playerArray = Players;
                foreach (PlayerManager Player in playerArray)
                {
                    Player.networkView.RPC("NET_TakeDamage", uLink.RPCMode.All, new object[] { 1000000, Player.transform.position, "You've been slain by an admin.", " <color=red>was slain by an admin.</color>" });
                }
            }
            
            //Button to set the steamid entered in the textbox.
            if (GUI.Button(new Rect(90f, 180f, 30f, 20f), "Set"))
            {
                NetworkController.NetManager_.PLAYER_INFOS.account_id = steamid;
            }
            //Button to randomly generate a set of numbers and then sets it.
            if (GUI.Button(new Rect(120f, 180f, 50f, 20f), "Rand"))
            {
                NetworkController.NetManager_.PLAYER_INFOS.account_id = UnityEngine.Random.Range(100000, 9999999).ToString();
                steamid = NetworkController.NetManager_.PLAYER_INFOS.account_id;
            }
            //Button to spawn item with the id, amount and ammo entered in the textboxes.
            if (GUI.Button(new Rect(90f, 220f, 80f, 20f), "Spawn Item"))
            {
                NetworkController.Player_ctrl_.AddItemToInventory_Collect(Convert.ToInt32(itemid), Convert.ToInt32(amount), Convert.ToInt32(ammo));
            }
            //Textboxes the rest of this is GUI stuff.
            itemid = GUI.TextField(new Rect(10, 220, 80, 20), itemid, 25);
            amount = GUI.TextField(new Rect(10, 240, 160, 20), amount, 25);
            ammo = GUI.TextField(new Rect(10, 260, 160, 20), ammo, 25);
            steamid = GUI.TextField(new Rect(10, 180, 80, 20), steamid, 25);
            vEsp = GUI.Toggle(new Rect(100f, 60f, 100f, 20f), vEsp, "Vehicles");
            pEsp = GUI.Toggle(new Rect(100f, 30f, 100f, 20f), pEsp, "Players");
            rEsp = GUI.Toggle(new Rect(100f, 90f, 100f, 20f), rEsp, "Resources");
            Speedhack = GUI.Toggle(new Rect(100f, 120f, 100f, 20f), Speedhack, "Speedhack");
            GUI.Label(new Rect(180f, 60f, 160f, 20f), "Vehicles: " + vehicledist);
            vehicledist = GUI.HorizontalSlider(new Rect(290f, 65f, 200f, 30f), vehicledist, 100f, 5000f);
            GUI.Label(new Rect(180f, 30f, 160f, 20f), "Players: " + playerdist);
            playerdist = GUI.HorizontalSlider(new Rect(290f, 35f, 200f, 30f), playerdist, 100f, 5000f);
            GUI.Label(new Rect(180f, 90f, 160f, 20f), "Resources: " + resourcedist);
            resourcedist = GUI.HorizontalSlider(new Rect(290f, 95f, 200f, 30f), resourcedist, 100f, 5000f);
            GUI.Label(new Rect(180f, 120f, 160f, 20f), "Speed: x" + speed);
            speed = GUI.HorizontalSlider(new Rect(290f, 125f, 200f, 30f), speed, 1f, 10f);
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        //Useless label.
        void Label(int y, string posName)
        {
            GUI.Label(new Rect(0, y, 200, 40), posName, AdminSkin);
        }

        //Function to draw the menu.
        void DrawAdminMenu(int id)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
            searchAdminMenu = GUILayout.TextField(searchAdminMenu);
            PlayerManager[] playerListArray = Players;
            //This doesn't get all players, maybe a distance thing.
            foreach (PlayerManager PlayListPlayer in playerListArray)
            {
                if (PlayListPlayer.CONTEXT_name.ToLower().Contains(searchAdminMenu.ToLower()))
                {
                    //Why a string? Why not.
                    string name = PlayListPlayer.CONTEXT_name;
                    GUILayout.Label(name);
                    killFunction(PlayListPlayer);
                    teleToFunction(PlayListPlayer);
                    teleToMeFunction(PlayListPlayer);
                }
            }

            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        //Check these and "update" every 6 seconds.
        private void Timer()
        {
            if ((double)Time.time < (double)nextUpdateTime)
                return;
            Resources = (UnityEngine.Object.FindObjectsOfType(typeof(Ressource)) as Ressource[]);
            Players = (UnityEngine.Object.FindObjectsOfType(typeof(PlayerManager)) as PlayerManager[]);
            Vehicles = (UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[]);
            Database = DataBaseManager.FindObjectOfType<DataBaseManager>();
            Server = ServerManager.FindObjectOfType<ServerManager>();
            nextUpdateTime = Time.time + 6f;
        }

        //On start or when injected.
        void Start()
        {
           AdminSkin.fontSize = 12;
        }

        //On update.
        void Update()
        {
            //Call our timer in update.
            Timer();

            //Define some buttons to open our main menu.
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                //Toggles on keypress INSERT
                DevMenu = !DevMenu;
            }

            //Disable everything Anti-Cheat related, I don't even know if they where properly setup anyway.
            //But let's make sure it never comes on.
            if (ObscuredCheatingDetector.Instance.enabled == true)
            {
                ObscuredCheatingDetector.Instance.enabled = false;
            }
            if (SpeedHackDetector.Instance.enabled == true)
            {
                SpeedHackDetector.Instance.enabled = false;
            }
            if (WallHackDetector.Instance.enabled == true)
            {
                WallHackDetector.Instance.enabled = false;
            }
            if (InjectionDetector.Instance.enabled == true)
            {
                InjectionDetector.Instance.enabled = false;
            }
        }

        //On GUI or anything on the screen related to like drawing, just HUD stuff.
        void OnGUI()
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(0, 0, 300, 100), "Tedious Anti-Cheat Developer Build");

            //Math stuff for Player Menu, this is unused.
            int z = 1;
            foreach (string str in List)
            {
                Label(z * 20, str);
                z += 1;
            }

            //Call the function to draw the player menu.
            if (PlayerMenu)
                AdminMenu = GUI.Window(1, AdminMenu, DrawAdminMenu, "Tedious Anti-Cheat GUI");

            //Call the function to draw the main menu.
            if (DevMenu)
                MainWin = GUI.Window(2, MainWin, MainWindow, "Tedious Anti-Cheat GUI - Main Window");

            //Speedhack function makes you go fast, if disabled resets speed to 1f which is default.
            if (Speedhack)
            {
                //Speed is the sliders value.
                Time.timeScale = speed;
            }
            else
            {
                Time.timeScale = 1f;
            }

            //ESP stuff, sloppy code not explaining.
            if (rEsp)
            {
                GUI.color = Color.yellow;
                Ressource[] array = Resources;
                foreach (Ressource ResourceArray in Resources)
                {
                    if (ResourceArray != null)
                    {
                        Vector3 vHeadScreen = Camera.main.WorldToScreenPoint(ResourceArray.transform.position);

                        float dist = Vector3.Distance(ResourceArray.transform.position, Camera.main.transform.position);
                        if (dist < resourcedist)
                        {
                            if (vHeadScreen.z > 0 & vHeadScreen.y < Screen.width - 2)
                            {
                                vHeadScreen.y = Screen.height - (vHeadScreen.y + 1f);
                                GUI.Label(new Rect(vHeadScreen.x, vHeadScreen.y, 200, 40), ResourceArray.CONTEXT_name + string.Format(" [{0:0}m]", (object)dist));
                            }
                        }
                    }
                }
            }

            //Vehicle ESP.
            if (vEsp)
            {
                GUI.color = Color.magenta;
                Vehicle[] vehicleArray = Vehicles;
                foreach (Vehicle vehicle in vehicleArray)
                {
                    if (vehicle != null)
                    {
                        Vector3 vHeadScreen = Camera.main.WorldToScreenPoint(vehicle.transform.position);

                        float dist = Vector3.Distance(vehicle.transform.position, Camera.main.transform.position);
                        if (dist < vehicledist)
                        {
                            if (vHeadScreen.z > 0 & vHeadScreen.y < Screen.width - 2)
                            {
                                vHeadScreen.y = Screen.height - (vHeadScreen.y + 1f);
                                GUI.Label(new Rect(vHeadScreen.x, vHeadScreen.y, 200, 40), vehicle.name + string.Format(" [{0:0}m]", (object)dist));
                            }
                        }
                    }
                }
            }

            //Player ESP.
            if (pEsp)
            {
                GUI.color = Color.cyan;
                PlayerManager[] playerArray = Players;
                foreach (PlayerManager Player in playerArray)
                {
                    if (Player != null)
                    {
                        Vector3 Vector2 = Player.transform.position + new Vector3(0f, 2f, 0);
                        Vector3 vHeadScreen = Camera.main.WorldToScreenPoint(Player.transform.position);
                        Vector3 Vector4 = Camera.main.WorldToScreenPoint(Vector2);

                        float dist = Vector3.Distance(Player.transform.position, Camera.main.transform.position);
                        if (dist < playerdist)
                        {
                            if (vHeadScreen.z > 0 & vHeadScreen.y < Screen.width - 2)
                            {
                                vHeadScreen.y = Screen.height - (vHeadScreen.y + 1f);
                                GUI.Label(new Rect(vHeadScreen.x, vHeadScreen.y, 200, 40), Player.CONTEXT_name + string.Format(" [{0:0}m]", (object)dist));
                            }
                        }
                    }
                }
            }
        }
    }
}
