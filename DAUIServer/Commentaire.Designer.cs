namespace DAUIServer
{
    partial class Commentaires
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
            this.webdsp = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webdsp
            // 
            this.webdsp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webdsp.Location = new System.Drawing.Point(0, 0);
            this.webdsp.MinimumSize = new System.Drawing.Size(20, 20);
            this.webdsp.Name = "webdsp";
            this.webdsp.ScrollBarsEnabled = false;
            this.webdsp.Size = new System.Drawing.Size(800, 450);
            this.webdsp.TabIndex = 0;
            // 
            // Commentaires
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.webdsp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Commentaires";
            this.Text = "Commentaire";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Commentaires_FormClosed);
            this.Load += new System.EventHandler(this.Commentaires_Load);
            this.VisibleChanged += new System.EventHandler(this.Commentaires_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webdsp;
    }
}