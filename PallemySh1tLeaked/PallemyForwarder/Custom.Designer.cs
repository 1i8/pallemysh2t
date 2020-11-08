namespace PallemyForwarder
{
    partial class Custom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.u = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.p = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.materialRaisedButton2 = new MaterialSkin.Controls.MaterialRaisedButton();
            this.SuspendLayout();
            // 
            // u
            // 
            this.u.Depth = 0;
            this.u.Hint = "";
            this.u.Location = new System.Drawing.Point(63, 93);
            this.u.MaxLength = 32767;
            this.u.MouseState = MaterialSkin.MouseState.HOVER;
            this.u.Name = "u";
            this.u.PasswordChar = '\0';
            this.u.SelectedText = "";
            this.u.SelectionLength = 0;
            this.u.SelectionStart = 0;
            this.u.Size = new System.Drawing.Size(343, 23);
            this.u.TabIndex = 2;
            this.u.TabStop = false;
            this.u.Text = "GrowID";
            this.u.UseSystemPasswordChar = false;
            // 
            // p
            // 
            this.p.Depth = 0;
            this.p.Hint = "";
            this.p.Location = new System.Drawing.Point(63, 134);
            this.p.MaxLength = 32767;
            this.p.MouseState = MaterialSkin.MouseState.HOVER;
            this.p.Name = "p";
            this.p.PasswordChar = '\0';
            this.p.SelectedText = "";
            this.p.SelectionLength = 0;
            this.p.SelectionStart = 0;
            this.p.Size = new System.Drawing.Size(343, 23);
            this.p.TabIndex = 3;
            this.p.TabStop = false;
            this.p.Text = "Password";
            this.p.UseSystemPasswordChar = false;
            // 
            // materialRaisedButton2
            // 
            this.materialRaisedButton2.AutoSize = true;
            this.materialRaisedButton2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialRaisedButton2.Depth = 0;
            this.materialRaisedButton2.Icon = null;
            this.materialRaisedButton2.Location = new System.Drawing.Point(283, 177);
            this.materialRaisedButton2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialRaisedButton2.Name = "materialRaisedButton2";
            this.materialRaisedButton2.Primary = true;
            this.materialRaisedButton2.Size = new System.Drawing.Size(123, 36);
            this.materialRaisedButton2.TabIndex = 14;
            this.materialRaisedButton2.Text = "Send Account";
            this.materialRaisedButton2.UseVisualStyleBackColor = true;
            this.materialRaisedButton2.Click += new System.EventHandler(this.materialRaisedButton2_Click);
            // 
            // Custom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 254);
            this.Controls.Add(this.materialRaisedButton2);
            this.Controls.Add(this.p);
            this.Controls.Add(this.u);
            this.Name = "Custom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Custom";
            this.Load += new System.EventHandler(this.Custom_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialSingleLineTextField u;
        private MaterialSkin.Controls.MaterialSingleLineTextField p;
        private MaterialSkin.Controls.MaterialRaisedButton materialRaisedButton2;
    }
}