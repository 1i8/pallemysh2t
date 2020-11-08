using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PallemyForwarder
{
    
    public partial class Login : MaterialSkin.Controls.MaterialForm
    {
        public Login()
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
        public static string LoginAcc(string username, string password)
        {
            string url = "http://45.155.124.146/S4UCKSMYLIFE/login.php";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string data = "username=" + username + "&password=" + password;
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
        public string CheckAccount(string username)
        {
            string url = "http://45.155.124.146/S4UCKSMYLIFE/check.php";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string data = "username=" + username;
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
        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public string CheckHWID(string username, string password)
        {
            string getEnv = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") + Environment.GetEnvironmentVariable("PROCESSOR_LEVEL") + Environment.GetEnvironmentVariable("PROCESSOR_REVISION") + Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS") + Environment.GetEnvironmentVariable("USERPROFILE");
            string url = "http://45.155.124.146/S4UCKSMYLIFE/hwidcheck.php";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string data = "username=" + username + "&hwid=" + CreateMD5(getEnv);
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

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            string validate = CheckAccount(user.Text);
            if (validate == "true")
            {
                string xD = LoginAcc(user.Text, pass.Text);
                if (xD.Contains("false"))
                {
                    MessageBox.Show("Username or password do not match.", "Pallemy Forwarder", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {

                    string zz = CheckHWID(user.Text, pass.Text);
                    if (zz.Contains("disabled"))
                    {
                        if (xD.Contains("true"))
                        {
                            Form1.usr = user.Text;
                            Form1.pw = pass.Text;
                            Form1 z = new Form1();
                            z.Show();
                            Hide();
                        }
                        else if (xD.Contains("Blacklisted"))
                        {
                            MessageBox.Show("Your account has been blacklisted.", "Pallemy Forwarder", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                    if (zz.Contains("true"))
                    {
                        if (xD.Contains("Blacklisted"))
                        {
                            MessageBox.Show("Your account has been blacklisted.", "Pallemy Forwarder", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                        else
                        {
                            if (xD.Contains("true"))
                            {
                                Form1.usr = user.Text;
                                Form1.pw = pass.Text;
                                Form1 z = new Form1();
                                z.Show();
                                Hide();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Your account has been blacklisted.", "Pallemy Forwarder", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
            else
            {
                MessageBox.Show("No user with this name was found.", "Pallemy Forwarder", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

        }
        private void Login_Load(object sender, EventArgs e)
        {
            string serial = "";
            string serial2 = "";

            user.Select();

            Registry.CurrentUser.CreateSubKey(@"Pallemy", true);
            try
            {
                RegistryKey keys = Registry.CurrentUser.OpenSubKey(@"Pallemy", true);
                serial = keys.GetValue("user").ToString();
                serial2 = keys.GetValue("pass").ToString();

                user.Text = serial;
                pass.Text = serial2;
            }
            catch
            {
                user.Text = "Username";
                pass.Text = "Password";
            }
            if(serial == "" || serial2 == "")
            {
                user.Text = "Username";
                pass.Text = "Password";
            }


        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Register reg = new Register();
            reg.Show();
            Hide();
        }
        bool ilktıklamauser = false;
        bool ilktıklamapassword = false;
        private void user_Click(object sender, EventArgs e)
        {
            if (ilktıklamauser == true)
            {

            }
            else
            {
            }
            ilktıklamauser = true;

        }

        private void pass_Click(object sender, EventArgs e)
        {
            if (ilktıklamapassword == true)
            {

            }
            else
            {
                user.Text = "";
            }
            ilktıklamapassword = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void user_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
