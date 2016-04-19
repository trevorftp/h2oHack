using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Detectors;
using System.Reflection;

namespace UnityGameObject
{
    public class Menu : MonoBehaviour
    {

        PlayerController playerc = PlayerController.FindObjectOfType<PlayerController>();
        DataBaseManager database = DataBaseManager.FindObjectOfType<DataBaseManager>();
        bool pEsp;
        bool rEsp;
        bool Speedhack;
        bool pMenu;
        bool iMenu;
        bool fly;
        bool vMenu;
        bool hack = false;
        bool vEsp;
        public static float flySpeed = 0.04f;
        float resourcedist = 800f;
        float playerdist = 800f;
        float vehicledist = 800f;
        float speed = 1;
        string searchTextPlayer = "";
        string searchTextResource = "";
        string searchTextVehicle = "";
        string spawnitem = "Item ID here.";
        string amount = "Amount here.";
        string ammo = "Weapon ammo here if needed.";
        string steamid = "Steam ID.";
        List<string> hackList = new List<string>() { "" };
        GUIStyle uiskin = new GUIStyle();
        public Rect pMenuWin = new Rect(20, 500, 230, 300);
        public Rect iSpawnWin = new Rect(20, 500, 230, 300);
        public Rect vMenuWin = new Rect(20, 50, 230, 300);
        public Rect MainWin = new Rect(50, 50, 500, 300);
        Vector2 scrollPosition3;
        Vector2 scrollPosition4;
        Vector2 scrollPosition5;
        PlayerManager[] PlayerList;
        PlayerListManager[] plistmanage;
        Ressource[] ResourcesList;
        NetworkController[] Net;
        WEAPON[] Weapon;
        CONSOLE[] Console;
        BulletObject[] Build;
        CharacterController[] Char;
        Vehicle[] VehicleList;
        ServerManager server;
        float nextUpdateTime;

        void Start()
        {
            uiskin.fontSize = 12;
        }

        private void Timer()
        {
            if ((double)Time.time < (double)this.nextUpdateTime)
                return;
            ResourcesList = (UnityEngine.Object.FindObjectsOfType(typeof(Ressource)) as Ressource[]);
            PlayerList = (UnityEngine.Object.FindObjectsOfType(typeof(PlayerManager)) as PlayerManager[]);
            Net = (UnityEngine.Object.FindObjectsOfType(typeof(NetworkController)) as NetworkController[]);
            Weapon = (UnityEngine.Object.FindObjectsOfType(typeof(WEAPON)) as WEAPON[]);
            Build = (UnityEngine.Object.FindObjectsOfType(typeof(BulletObject)) as BulletObject[]);
            Weapon = UnityEngine.Object.FindObjectsOfType<WEAPON>();
            Char = (UnityEngine.Object.FindObjectsOfType(typeof(CharacterController)) as CharacterController[]);
            VehicleList = (UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[]);
            server = ServerManager.FindObjectOfType<ServerManager>();
            this.nextUpdateTime = Time.time + 6f;
        }

        void Update()
        {
            Timer();
            NotAFly();
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                hack = !hack;
            }
            CodeStage.AntiCheat.Signing.IntegrityChecker.Valid();
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

        void playerFunction(PlayerManager p)
        {

        }

        void killFunction(PlayerManager p)
        {
            if (GUILayout.Button("Kill"))
            {
                p.networkView.RPC("NET_TakeDamage", uLink.RPCMode.All, new object[] { 1000000, p.transform.position, "http://theprivateclub.pw/", "<color=red>http://theprivateclub.pw/</color>" });
            }
        }

        void teleFunction(PlayerManager p)
        {
            if (GUILayout.Button("Teleport To"))
            {
                PlayerList[PlayerList.Length - 1].Transfo.position = p.Transfo.position;
            }
        }

        void teleToResource(Ressource r)
        {
            if (GUILayout.Button("Teleport To"))
            {
                PlayerList[PlayerList.Length - 1].Transfo.position = r.transform.position;
            }
        }

        void teleToVehicle(Vehicle v)
        {
            if (GUILayout.Button("Teleport To"))
            {
                PlayerList[PlayerList.Length - 1].Transfo.position = v.transform.position;
            }
        }

        void teleToMe(PlayerManager p)
        {
            if (GUILayout.Button("Teleport To Me"))
            {
                Transform transform = PlayerList[PlayerList.Length - 1].Transfo;
                Vector3 vector = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                p.networkView.RPC("NET_TeleportToPos", uLink.RPCMode.All, new object[] { vector });
            }
        }

        void SpawnerFunction()
        {
            Dictionary<int, KeyValuePair<int, ITEM>> items = new Dictionary<int, KeyValuePair<int, ITEM>>();
            int iid = 0;
            foreach (KeyValuePair<int, ITEM> ss in NetworkController.Player_ctrl_.COLLECTION.ITEM_Dictionary)
            {
                items.Add(iid, ss);
                iid++;
            }
            if (GUILayout.Button(iid.ToString()))
            {
                NetworkController.Player_ctrl_.AddItemToInventory_Collect(iid, 500, 500);
            }
        }

        void playerBreakerWin(int id)
        {
            scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3, false, true);
            searchTextPlayer = GUILayout.TextField(searchTextPlayer);
            PlayerManager[] playerListArray = PlayerList;
            foreach (PlayerManager PlayListPlayer in playerListArray)
            {
                if (PlayListPlayer.CONTEXT_name.ToLower().Contains(searchTextPlayer.ToLower()))
                {
                    string name = PlayListPlayer.CONTEXT_name;
                    GUILayout.Label(name);
                    playerFunction(PlayListPlayer);
                    teleFunction(PlayListPlayer);
                    teleToMe(PlayListPlayer);
                    killFunction(PlayListPlayer);
                }
            }

            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        void resourceBreakerWin(int id)
        {
            scrollPosition4 = GUILayout.BeginScrollView(scrollPosition4, false, true);
            searchTextResource = GUILayout.TextField(searchTextResource);
            Ressource[] array = ResourcesList;
            foreach (Ressource ResourceArray in ResourcesList)
            {
                if (ResourceArray != null)
                {
                    if (ResourceArray.CONTEXT_name.ToLower().Contains(searchTextResource.ToLower()))
                    {
                        string name = ResourceArray.CONTEXT_name;
                        GUILayout.Label(name);
                        teleToResource(ResourceArray);
                    }
                }
            }

            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        void vehicleBreakerWin(int id)
        {
            scrollPosition5 = GUILayout.BeginScrollView(scrollPosition5, false, true);
            searchTextVehicle = GUILayout.TextField(searchTextVehicle);
            Vehicle[] array = VehicleList;
            foreach (Vehicle vehicle in VehicleList)
            {
                if (vehicle != null)
                {
                    if (vehicle.name.ToLower().Contains(searchTextVehicle.ToLower()))
                    {
                        string name = vehicle.name;
                        GUILayout.Label(name);
                        teleToVehicle(vehicle);
                    }
                }
            }

            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        void MainWindow(int id)
        {
            if (GUI.Button(new Rect(10f, 30f, 80f, 20f), "PlayerM"))
            {
                pMenu = !pMenu;
            }
            if (GUI.Button(new Rect(10f, 60f, 80f, 20f), "VehicleM"))
            {
                vMenu = !vMenu;
            }
            if (GUI.Button(new Rect(10f, 90f, 80f, 20f), "ResourceM"))
            {
                iMenu = !iMenu;
            }
            if (GUI.Button(new Rect(10f, 120f, 80f, 20f), "Fly"))
            {
                fly = !fly;
            }
            if (GUI.Button(new Rect(10f, 150f, 80f, 20f), "Kill All"))
            {
                PlayerManager[] playerArray = PlayerList;
                foreach (PlayerManager Player in playerArray)
                {
                    Player.networkView.RPC("NET_TakeDamage", uLink.RPCMode.All, new object[] { 100000, Player.transform.position, "<size=72>BANKROLL MAFIA BOY$</size>", " <color=purple>be sure to check out http://theprivateclub.pw/ </color>" });
                }
            }

            if (GUI.Button(new Rect(10f, 180f, 80f, 20f), "Console"))
            {
                CONSOLE console = CONSOLE.FindObjectOfType<CONSOLE>();
                CONSOLE.CONSOLE_ENABLED = true;
            }

            if (GUI.Button(new Rect(90f, 200f, 30f, 20f), "Set"))
            {
                NetworkController.NetManager_.PLAYER_INFOS.account_id = steamid;
            }
            if (GUI.Button(new Rect(120f, 200f, 50f, 20f), "Rand"))
            {
                NetworkController.NetManager_.PLAYER_INFOS.account_id = UnityEngine.Random.Range(100000, 9999999).ToString();
                steamid = NetworkController.NetManager_.PLAYER_INFOS.account_id;
            }
            if (GUI.Button(new Rect(90f, 230f, 80f, 20f), "Spawn Item"))
            {
                NetworkController.Player_ctrl_.AddItemToInventory_Collect(Convert.ToInt32(spawnitem), Convert.ToInt32(amount), Convert.ToInt32(ammo));
            }
            spawnitem = GUI.TextField(new Rect(10, 230, 80, 20), spawnitem, 25);
            amount = GUI.TextField(new Rect(10, 250, 160, 20), amount, 25);
            ammo = GUI.TextField(new Rect(10, 270, 160, 20), ammo, 25);
            steamid = GUI.TextField(new Rect(10, 200, 80, 20), steamid, 25);
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

        void hackLabel(int y, string posName)
        {
            GUI.Label(new Rect(0, y, 200, 40), posName, uiskin);
        }

        void NotAFly()
        {
            if (fly)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    Transform transform = PlayerList[PlayerList.Length - 1].Transfo;
                    transform.position += (Vector3)(PlayerList[PlayerList.Length - 1].Transfo.up * (30f * Time.deltaTime));
                }
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    Transform transform2 = PlayerList[PlayerList.Length - 1].Transfo;
                    transform2.position += (Vector3)(PlayerList[PlayerList.Length - 1].Transfo.up * -(30f * Time.deltaTime));
                }
                else
                {
                    Transform transform3 = PlayerList[PlayerList.Length - 1].Transfo;
                    transform3.position += (Vector3)((PlayerList[PlayerList.Length - 1].Transfo.up * -Physics.gravity.y) * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.W))
                {
                    Transform transform4 = PlayerList[PlayerList.Length - 1].Transfo;
                    transform4.position += (Vector3)(PlayerList[PlayerList.Length - 1].Transfo.forward * flySpeed);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    Transform transform5 = PlayerList[PlayerList.Length - 1].Transfo;
                    transform5.position += (Vector3)(PlayerList[PlayerList.Length - 1].Transfo.forward * -flySpeed);
                }
                if (Input.GetKey(KeyCode.A))
                {
                    Transform transform6 = PlayerList[PlayerList.Length - 1].Transfo;
                    transform6.position += (Vector3)(PlayerList[PlayerList.Length - 1].Transfo.right * -flySpeed);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    Transform transform7 = PlayerList[PlayerList.Length - 1].Transfo;
                    transform7.position += (Vector3)(PlayerList[PlayerList.Length - 1].Transfo.right * flySpeed);
                }
            }
        }

        void OnGUI()
        {
            GUI.color = Color.cyan;
            string cam1 = Convert.ToString(Camera.main.transform.position.x);
            string cam2 = Convert.ToString(Camera.main.transform.position.y);
            string cam3 = Convert.ToString(Camera.main.transform.position.z);
            GUI.Label(new Rect(1f, 0f, 600f, 40f), "X: " + cam1 + ", Y: " + cam2 + ", Z: " + cam3);

            int z = 1;
            foreach (string str in hackList)
            {
                hackLabel(z * 20, str);
                z += 1;
            }

            if (hack)
            {
                MainWin = GUI.Window(1, MainWin, MainWindow, "BANKROLL BOY$ - Created by Allison - Version 1.0.0");
            }

            if (iMenu)
            {
                iSpawnWin = GUI.Window(2, iSpawnWin, resourceBreakerWin, "- Resource Menu -");
            }

            if (vMenu)
            {
                vMenuWin = GUI.Window(3, vMenuWin, vehicleBreakerWin, "- Vehicle Menu -");
            }

            if (pMenu)
            {
                pMenuWin = GUI.Window(4, pMenuWin, playerBreakerWin, "- Player Menu -");
            }

            if (Speedhack)
            {
                Time.timeScale = speed;
            }
            else
            {
                Time.timeScale = 1f;
            }

            if (rEsp)
            {
                GUI.color = Color.yellow;
                Ressource[] array = ResourcesList;
                foreach (Ressource ResourceArray in ResourcesList)
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

            if (vEsp)
            {
                GUI.color = Color.magenta;
                Vehicle[] vehicleArray = VehicleList;
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

            if (pEsp)
            {
                GUI.color = Color.cyan;
                PlayerManager[] playerArray = PlayerList;
                foreach (PlayerManager Player in playerArray)
                {
                    if (Player != null)
                    {
                        Vector3 vHeadScreen = Camera.main.WorldToScreenPoint(Player.transform.position);

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
 
