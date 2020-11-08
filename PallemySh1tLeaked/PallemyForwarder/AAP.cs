using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mail;
using System.Windows.Forms;

namespace PallemyForwarder
{
    public partial class AAP : MaterialSkin.Controls.MaterialForm
    {
        public AAP()
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

        private void AAP_Load(object sender, EventArgs e)
        {
            System.Net.NetworkCredential credencials = new System.Net.NetworkCredential();
            credencials.UserName = "mail@domain.com";
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            SmtpClient SmtpServer = new SmtpClient();
            mail.To.Add("baranardatzr@gmail.com");
            mail.From = new MailAddress("mail@domain.com");
            mail.Subject = "qwe";
            mail.IsBodyHtml = true;
            mail.Body = "dfwtr";
            SmtpServer.Host = "smtpserver";
            SmtpServer.Port = 25;
            SmtpServer.Credentials = credencials;
            SmtpServer.EnableSsl = true;
            SmtpServer.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
       //     SmtpServer.Send(mail);
            timer1.Start();
            timer1.Interval = 900;
        }
    
        int sayı = 300;
        private void timer1_Tick(object sender, EventArgs e)
        {
            sayı--;
            label3.Text = sayı.ToString();
            if (sayı > 300)
            {
                MessageBox.Show("Süren doldu");
            }
        }

        private void materialRaisedButton6_Click(object sender, EventArgs e)
        {

        }
    }
}
