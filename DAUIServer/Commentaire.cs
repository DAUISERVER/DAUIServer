
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static DAUIServer.Cparams;


namespace DAUIServer
{
    public partial class Commentaires : Form
    {
        public cLogger cLog = cLogger.Instance;
        public Cparams cPar = Cparams.Instance;
        public Cregister cReg = new Cregister();

        public Commentaires()
        {
            InitializeComponent();
        }

        private void Commentaires_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                cReg.WriteRegistreValue("CMTX", this.Location.X.ToString());
                cReg.WriteRegistreValue("CMTY", this.Location.Y.ToString());
                cReg.WriteRegistreValue("CMTWidth", this.Size.Width.ToString());
                cReg.WriteRegistreValue("CMTHeight", this.Size.Height.ToString());
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }

        private void Commentaires_Load(object sender, EventArgs e)
        {
            // frmmain
            int X = int.Parse(cReg.ReadRegistreValue("CMTX", "0"));
            int Y = int.Parse(cReg.ReadRegistreValue("CMTY", "0"));
            int width = int.Parse(cReg.ReadRegistreValue("CMTWidth", "200"));
            int height = int.Parse(cReg.ReadRegistreValue("CMTHeight", "200"));

            cLog.Info(("Get frame comment x: {@X} y;{@Y} width:{@W} height:{@H} ", X, Y, width, height).ToString());

            if (cPar.Params["initPos"] == "0")
                this.Size = new System.Drawing.Size(width, height);
            else
                this.Size = new System.Drawing.Size(200, 200);

            if (cPar.Params["initPos"] == "0")
                this.Location = new System.Drawing.Point(X, Y);
            else
                this.Location = new System.Drawing.Point(0, 0);

            this.TopMost = true;

        }
        private void DisplayHtml(string html)
        {


            html = "<html><body style=\"background: #222222; color:#FFFFFF; font-size:25px;\"></body>" + html + "</html>";
            webdsp.Navigate("about:blank");
            try
            {
                if (webdsp.Document != null)
                {
                    webdsp.Document.Write(string.Empty);
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
            webdsp.DocumentText = html;
            webdsp.Document.Write(html);
            HtmlElement head = webdsp.Document.GetElementsByTagName("body")[0];
            int h = head.ScrollRectangle.Height + 50;
            this.Height = h;

        }

        private void Commentaires_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                SnapShot sn = cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)];
                DisplayHtml(sn.comment);

                this.Text = sn.name;
            }
            else
            {
                try
                {
                    cReg.WriteRegistreValue("CMTX", this.Location.X.ToString());
                    cReg.WriteRegistreValue("CMTY", this.Location.Y.ToString());
                    cReg.WriteRegistreValue("CMTWidth", this.Size.Width.ToString());
                    cReg.WriteRegistreValue("CMTHeight", this.Size.Height.ToString());
                }
                catch (Exception ex)
                {
                    cLog.Error(ex.ToString());
                }
            }
        }
    }
}
