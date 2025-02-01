using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DAUIServer.UDPSocket;
using static DAUIServer.Cparams;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Security.Policy;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System.Runtime.InteropServices;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Melanchall.DryWetMidi.Interaction;

namespace DAUIServer
{
    public partial class FrmMain : Form
    {
        public cLogger cLog = cLogger.Instance;
        public Cparams cPar = Cparams.Instance;
        public Cregister cReg = new Cregister();
        public Cxml cXml = new Cxml();
        public Csnapshot cSnps = new Csnapshot();
        public CUi Cui = CUi.Instance;

        public Commentaires FRMCMD = new Commentaires();

        public UDPSocket OSCOut = new UDPSocket();
        public UDPSocket OSCIn = new UDPSocket();
        public CMidi cMidi = CMidi.Instance;

        public OscEventReceiveArg _oscevt = null;
        public delegate void dispatch();
        public dispatch OSCDelegate;

        public MidiEvent _midievt = null;
        public delegate void MProcess();
        public MProcess MIDIDelegate;

        private int MenuSelected = -1;
        private int MenuAutoClose = 0;

        private bool FromOsc = false;
        private bool InitChannels = false;
        private int buttonHeight = 80;
        private int sep = 10;
        private bool ShowProcess = false;

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll", SetLastError = true)]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        [DllImport("user32")]
        public static extern int GetProcessId(IntPtr handle);

        public FrmMain()
        {
            // ne pas modifier l'ordre.
            InitializeComponent();
            cXml.LoadXMLConfig();
            cXml.Loadxtouchconfig();
            cXml.LoadXMLSnap();
            CreatButton();
            InitialisationApp();
            cSnps.ReadDas();
            DisplaySnapsGrid();

            string result = cSnps.checkDasSnap(bool.Parse(cPar.Params["dasupdate"]));
            if (result != "")
            {
                MessageBox.Show(result, "Lecture fichier Das5", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            SelectSnap(cReg.ReadRegistreValue("currentSnap", "1"));
            SelectScene(cReg.ReadRegistreValue("currentScene", "1"));
            OscParam();

            Cui.setWebBrowser(UIWeb);
            // open ui web
            cLog.Info("OPEN URI " + cPar.Params["URL_UI"]);
            UIWeb.Url = new Uri(cPar.Params["URL_UI"]);

            Flash.Enabled = true;

            // init fenêtre commentaires            
            FRMCMD.Show();
            FRMCMD.Visible = false;

            // evt midi connected
            cMidi.InConnectEvt += OnInMidiConnected;
            cMidi.OutConnectEvt += OnOutMidiConnected;
        }

        #region paramétrage OSC
        private void OscParam()
        {
            cLog.Info("Try to opren OSC in " + cPar.Params["OSC_IP"] + " - " + cPar.Params["OSC_Inc_Port"]);
            OSCIn.Server(cPar.Params["OSC_IP"], int.Parse(cPar.Params["OSC_Inc_Port"]));

            cLog.Info("Try to opren OSC out " + cPar.Params["OSC_IP"] + " - " + cPar.Params["OSC_Out_Port"]);
            OSCOut.Client(cPar.Params["OSC_IP"], int.Parse(cPar.Params["OSC_Out_Port"]));
            try
            {
                OSCIn.OscEventReceive += OnOscREceive;
            }
            catch (Exception ex)
            {
                cLog.Error("error 1600 " + ex.ToString());
                System.Diagnostics.Debug.Print("error 500 " + ex.ToString());
            }
        }
        #endregion

        #region reception OSC
        public void OnOscREceive(object sender, OscEventReceiveArg e)
        {
            _oscevt = e;
            this.Invoke(this.OSCDelegate);
        }

        public void dispatchOSC()
        {
            try
            {
                var P = _oscevt as OscEventReceiveArg;
                if (P != null)
                {
                    // recherche si bouton
                    if (cPar.ButtonResults.ContainsKey(P.key))
                    {
                        button Bt = cPar.Buttons[cPar.ButtonResults[P.key]];
                        Bt.statut = (int)P.value;
                        cPar.Buttons[cPar.ButtonResults[P.key]] = Bt;

                        return;
                    }

                    // recherche dans la liste des scenes
                    SnapResult SR = new SnapResult();
                    SR.bankid = -1;
                    if (P.value > 0)
                    {
                        if (cPar.SnapResults.ContainsKey(P.key))
                        {
                            SR = cPar.SnapResults[P.key];
                        }
                        if (SR.bankid != -1)
                        {
                            // c'est le snap courant ?
                            if (SR.snapkey.ToString() != cPar.tagCurrentSnap)
                            {
                                // changement de snap 
                                FromOsc = true;
                                SelectSnap(SR.snapkey.ToString());
                            }
                            // c'est la scene courante ?
                            if (SR.sceneid.ToString() != cPar.tagCurrentScene)
                            {
                                FromOsc = true;
                                SelectScene(SR.sceneid.ToString());
                            }

                        }
                        FromOsc = false;
                    }
                }

            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            try
            {

                // si channel memorise au cas ou pas de channel en cours sur la x-Touch
                string sc = cPar.GetDasIdentBySc(_oscevt.key);
                // fader ?
                if (sc != null && cPar.Faders.ContainsKey(sc) && !cPar.Channels.ContainsKey(sc))
                {
                    Cparams.fader fad = cPar.Faders[sc];
                    if (fad.value != _oscevt.value)
                    {
                        fad.value = _oscevt.value;
                        cPar.Faders[sc] = fad;
                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

        }
        #endregion

        #region initialisation générale
        private void InitialisationApp()
        {
            try
            {
                cLog.Info("Start initialisation App.");
                // frmmain
                int X = int.Parse(cReg.ReadRegistreValue("MyX", "0"));
                int Y = int.Parse(cReg.ReadRegistreValue("MyY", "0"));
                int width = int.Parse(cReg.ReadRegistreValue("MyWidth", "200"));
                int height = int.Parse(cReg.ReadRegistreValue("MyHeight", "200"));

                cLog.Info(("Get frame info x: {@X} y;{@Y} width:{@W} height:{@H} ", X, Y, width, height).ToString());

                if (cPar.Params["initPos"] == "0")
                    this.Size = new System.Drawing.Size(width, height);
                else
                    this.Size = new System.Drawing.Size(200, 200);

                if (cPar.Params["initPos"] == "0")
                    this.Location = new System.Drawing.Point(X, Y);
                else
                    this.Location = new System.Drawing.Point(0, 0);

                this.TopMost = false;
                if (cPar.Params["TopMost"] == "1")
                    this.TopMost = true;


                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                if (cPar.Params["borderstate"] != "")
                    this.FormBorderStyle = (FormBorderStyle)int.Parse(cPar.Params["borderstate"]);

                this.WindowState = (FormWindowState)int.Parse(cPar.Params["windowstate"]);

                OSCDelegate = new dispatch(dispatchOSC);
                MIDIDelegate = new MProcess(MidiProcess);

                // get handle of this app
                cLog.Info("Get Handle for this application ");






                Process[] processes = Process.GetProcessesByName(System.Windows.Forms.Application.ProductName);
                cPar.UIhWnd = processes[0].MainWindowHandle;

                if (cPar.UIhWnd != IntPtr.Zero)
                {
                    cLog.Info("Handle ok");
                }


            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

        }
        #endregion

        #region enregistrement des position et taille frmmain
        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {

                // enregistre les positions
                cReg.WriteRegistreValue("MyX", this.Location.X.ToString());
                cReg.WriteRegistreValue("MyY", this.Location.Y.ToString());
                cReg.WriteRegistreValue("MyWidth", this.Size.Width.ToString());
                cReg.WriteRegistreValue("MyHeight", this.Size.Height.ToString());

                // delete les channels
                List<string> keys = new List<string>(cPar.Channels.Keys);
                foreach (string key in keys)
                {

                    if (cPar.Channels[key] != null)
                    {
                        cPar.Channels[key].release();
                    }
                    cPar.Channels[key] = null;

                    GC.Collect(0);

                }
                cPar.Channels.Clear();

            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region selectionne un snap
        public void SelectSnap(string tag)
        {
            try
            {
                for (int i = 0; i < SnapGrid.Rows.Count; i++)
                {
                    if (SnapGrid.Rows[i].Cells[1].Tag.ToString() == tag)
                    {
                        SnapGrid.Rows[i].Cells[1].Selected = true;
                        cPar.tagCurrentScene = "";
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region selectionne une scene
        public void SelectScene(string tag)
        {
            try
            {
                for (int i = 0; i < SceneGrid.Rows.Count; i++)
                {
                    if (SceneGrid.Rows[i].Cells[1].Tag.ToString() == tag)
                    {

                        SceneGrid.Rows[i].Cells[0].Selected = true;
                        cPar.tagCurrentScene = tag;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region mise à jour de la liste des snaps
        public void DisplaySnapsGrid()
        {
            try
            {
                SnapGrid.Rows.Clear();
                foreach (KeyValuePair<int, SnapShot> snap in cPar.Snapshots)
                {
                    if (snap.Value.actif && snap.Value.type == 0)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        row.CreateCells(SnapGrid);
                        row.Cells[1].Value = snap.Value.name;
                        row.Cells[1].Tag = snap.Key;
                        row.MinimumHeight = int.Parse(cPar.Params["minimumrowheight"]);
                        row.Cells[1].Style.BackColor = ColorTranslator.FromHtml("#123456");
                        row.Cells[1].Style.ForeColor = Color.White;
                        row.Cells[1].ReadOnly = false;
                        row.DividerHeight = 5;
                        if (snap.Value.DashBank == null || snap.Value.DashBank.Count() == 0)
                        {
                            row.Cells[0].Style.BackColor = Color.Red;
                            row.Cells[1].ReadOnly = true;
                        }

                        if (snap.Value.UIName == null || snap.Value.UIName == "")
                        {
                            row.Cells[0].Style.BackColor = Color.DarkOrange;
                            row.Cells[1].ReadOnly = true;
                        }
                        row.Cells[0].Style.SelectionBackColor = row.Cells[0].Style.BackColor;
                        row.Cells[1].Style.SelectionBackColor = Color.BlueViolet;
                        SnapGrid.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region création des boutons 
        private void CreatButton()
        {
            int buttonHeight = 80;
            int sep = 10;
            try
            {
                int W = ((this.Width - SnapGrid.Size.Width) / cPar.Buttons.Count) - sep;
                for (int num = 1; num <= cPar.Buttons.Count; num++)
                {
                    button but = cPar.Buttons[num];
                    System.Windows.Forms.Button button = new System.Windows.Forms.Button();
                    //button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                    button.BackColor = ColorTranslator.FromHtml("#123456");
                    button.ForeColor = System.Drawing.Color.White;
                    button.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#666666");
                    button.FlatAppearance.BorderSize = 3;
                    button.Name = "option" + num.ToString();
                    button.Size = new System.Drawing.Size(W - sep, buttonHeight);
                    button.Location = new System.Drawing.Point((num - 1) * W, 0);
                    button.Text = but.scene;
                    button.UseVisualStyleBackColor = false;
                    button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_MouseDown);
                    button.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button_MouseUp);
                    button.Tag = num;
                    but.Btname = button.Name;
                    this.Controls.Add(button);
                    cPar.Buttons[num] = but;
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            // renseigne le menu
            try
            {
                creatMenu(0);

            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region Création menu  
        private void creatMenu(int type)
        {
            // renseigne le menu
            try
            {
                DauiMenu.Rows.Clear();
                DauiMenu.BackgroundColor = ColorTranslator.FromHtml("#123456");
                DauiMenu.RowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#123456");
                DauiMenu.RowsDefaultCellStyle.SelectionBackColor = Color.BlueViolet;



                DauiMenu.GridColor = ColorTranslator.FromHtml("#123456");

                if (type == 0)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 40;
                    row.Cells[0].Value = "";
                    row.Cells[1].Value = "Update snapshot";
                    row.Cells[1].Tag = "0";
                    DauiMenu.Rows.Add(row);
                    //row = new DataGridViewRow();
                    //row.CreateCells(DauiMenu);
                    //row.MinimumHeight = 40;
                    //row.Cells[0].Value = "";
                    //row.Cells[1].Value = "Init web interface";
                    //row.Cells[1].Tag = "1";
                    //DauiMenu.Rows.Add(row);
                    row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 40;
                    row.Cells[0].Value = "<";
                    row.Cells[1].Value = "Change Snap";
                    row.Cells[1].Tag = "2";
                    DauiMenu.Rows.Add(row);
                    if (int.Parse(cPar.tagCurrentScene) + 1 < SceneGrid.Rows.Count)
                    {
                        if (cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0].scenes[int.Parse(cPar.tagCurrentScene)].dasuid != "-1")
                        {
                            row = new DataGridViewRow();
                            row.CreateCells(DauiMenu);
                            row.MinimumHeight = 40;
                            row.Cells[0].Value = "<";
                            row.Cells[1].Value = "Add virtual scene after '" + SceneGrid.Rows[int.Parse(cPar.tagCurrentScene)].Cells[1].Value + "'";
                            row.Cells[1].Tag = "8";
                            DauiMenu.Rows.Add(row);
                        }
                        else
                        {
                            row = new DataGridViewRow();
                            row.CreateCells(DauiMenu);
                            row.MinimumHeight = 40;
                            row.Cells[0].Value = "";
                            row.Cells[1].Value = "Delete virtual scene";
                            row.Cells[1].Tag = "9";
                            DauiMenu.Rows.Add(row);
                        }
                    }
                    row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 40;
                    row.Cells[0].Value = "";
                    row.Cells[1].Value = "Reset Web Size";
                    row.Cells[1].Tag = "6";
                    DauiMenu.Rows.Add(row);
                    row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 40;
                    row.Cells[0].Value = "";
                    row.Cells[1].Value = "Minimize";
                    row.Cells[1].Tag = "7";
                    DauiMenu.Rows.Add(row);
                    row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 40;
                    row.Cells[0].Value = "<";
                    row.Cells[1].Value = "Quitter";
                    row.Cells[1].Tag = "3";
                    DauiMenu.Rows.Add(row);
                    row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 20;
                    row.Cells[0].Value = "";
                    row.Cells[1].Value = "____________________";
                    row.Cells[1].Tag = "";
                    DauiMenu.Rows.Add(row);
                    row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 40;
                    row.Cells[0].Value = "";
                    row.Cells[1].Value = "Exit";
                    row.Cells[1].Tag = "4";
                    DauiMenu.Rows.Add(row);
                }
                if (type == 1)
                {
                    DataGridViewRow row;
                    foreach (KeyValuePair<int, SnapShot> snap in cPar.Snapshots)
                    {
                        if (snap.Value.actif && snap.Value.type == 0)
                        {
                            row = new DataGridViewRow();
                            row.CreateCells(SnapGrid);
                            row.Cells[0].Value = "";
                            if (snap.Key.ToString() == cPar.tagCurrentSnap)
                            {
                                row.Cells[1].Value = "[ " + snap.Value.name + " ]";
                            }
                            else
                            {
                                row.Cells[1].Value = snap.Value.name;
                            }
                            row.Cells[1].Tag = "SNAP" + snap.Key;
                            row.MinimumHeight = 50;
                            DauiMenu.Rows.Add(row);
                            if (snap.Key.ToString() == cPar.tagCurrentSnap)
                            {
                                DauiMenu.Rows[DauiMenu.Rows.Count - 1].Cells[0].Selected = true;
                            }
                        }
                    }
                    row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 40;
                    row.Cells[1].Value = "____________________";
                    row.Cells[1].Tag = "";
                    DauiMenu.Rows.Add(row);
                    row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 50;
                    row.Cells[1].Value = "Exit";
                    row.Cells[1].Tag = "4";
                    DauiMenu.Rows.Add(row);
                }
                if (type == 2)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 40;
                    row.Cells[0].Value = "";
                    row.Cells[1].Value = "Vous confirmer quitter";
                    row.Cells[1].Tag = "5";
                    DauiMenu.Rows.Add(row);
                    row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 40;
                    row.Cells[0].Value = "";
                    row.Cells[1].Value = "Annuler quitter";
                    row.Cells[1].Tag = "4";
                    DauiMenu.Rows.Add(row);
                }
                if (type == 3)
                {
                    DataGridViewRow row = new DataGridViewRow();              
                    for(int i = 1; i<cPar.Buttons.Count;i++)
                    {
                        if (cPar.Buttons[i].color != null)
                        {
                            row = new DataGridViewRow();
                            row.CreateCells(DauiMenu);
                            row.MinimumHeight = 40;
                            row.Cells[0].Value = "";
                            row.Cells[1].Value = cPar.Buttons[i].scene;
                            row.Cells[1].Tag = "bt_" + i; ToString();
                            DauiMenu.Rows.Add(row);
                        }
                    }
                    row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 40;
                    row.Cells[1].Value = "____________________";
                    row.Cells[1].Tag = "";
                    DauiMenu.Rows.Add(row);
                    row = new DataGridViewRow();
                    row.CreateCells(DauiMenu);
                    row.MinimumHeight = 50;
                    row.Cells[1].Value = "Exit";
                    row.Cells[1].Tag = "4";
                    DauiMenu.Rows.Add(row);



                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region positionne les grilles et création des boutons, infomessage (pas possible en auto ?
        private void FrmMain_Resize(object sender, EventArgs e)
        {
            int W = ((this.Width - (SnapGrid.Size.Width)) / (cPar.Buttons.Count + 1)) - sep;
            try
            {
                SnapGrid.Location = new Point((this.Width - SnapGrid.Size.Width) - 16, 0);
                SnapGrid.Size = new Size(SnapGrid.Size.Width, this.Height / 2);
                SceneGrid.Location = new Point(SnapGrid.Location.X, SnapGrid.Size.Height + 20);
                SceneGrid.Size = new Size(SnapGrid.Size.Width, (this.Height / 2) - 70);
               
                button_menu.Size  = new System.Drawing.Size((int) (W/2),buttonHeight);
                button_menu.Location = new Point((SnapGrid.Location.X - button_menu.Size.Width) - 8, 0);

            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            try
            {

               

                for (int num = 1; num <= cPar.Buttons.Count; num++)
                {
                    button but = cPar.Buttons[num];
                    if (but.Btname != "")
                    {
                        System.Windows.Forms.Button Bt = (System.Windows.Forms.Button)this.Controls[but.Btname];

                        Bt.Size = new System.Drawing.Size(W - sep, buttonHeight);
                        Bt.Location = new System.Drawing.Point((num - 1) * W, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            // uiWeb
            try
            {
                UIWeb.Location = new System.Drawing.Point(0, buttonHeight + sep);
                UIWeb.Size = new System.Drawing.Size(SnapGrid.Location.X - sep, this.Height - (buttonHeight + (cPar.Params["borderstate"] == "0" ? 5 : 7 * sep)));
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            // menu
            try
            {
                MenuPanel.Location = new System.Drawing.Point((this.Width / 2) - (DauiMenu.Width / 2), ((this.Height / 2) - (DauiMenu.Height / 2)));
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            // infomessage
            try
            {
                InfoMessage.Location = new System.Drawing.Point((this.Width / 2) - (InfoMessage.Width / 2), ((this.Height / 2) - (InfoMessage.Height / 2)));
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

        }
        #endregion

        #region Changement de selection dans le snap -> mise à jour des scenes -> update channels
        private void SnapGrid_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                bool Frmcmdopen = false;
                if (FRMCMD.Visible)
                {
                    FRMCMD.Visible = false;
                    Frmcmdopen = true;
                }

                updatescene();

                if (cMidi.inpDevok && cMidi.outDevOk && cPar.Channels.Count != 0 && !cPar.PresentInProgress)
                {

                    cPar.CurrentPage = 0;
                    if (Frmcmdopen)
                    {
                        FRMCMD.Visible = true;
                        Frmcmdopen = false;
                    }
                    SetChannels();
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }


        void updatescene()
        {
            // rechercher qui à la selection
            for (int i = 0; i < SnapGrid.Rows.Count; i++)
            {
                if (SnapGrid.Rows[i].Cells[0].Selected)
                {

                    if (i >= 3)
                    {
                        SnapGrid.FirstDisplayedScrollingRowIndex = i - 3;
                    }


                    SceneGrid.Rows.Clear();
                    cPar.tagCurrentSnap = SnapGrid.Rows[i].Cells[1].Tag.ToString();
                    int tag = int.Parse(SnapGrid.Rows[i].Cells[1].Tag.ToString());
                    // met a jour l'ui et l'xtouch
                    if (Cui.Connected && !cPar.PresentInProgress)
                    {
                        Cui.snapUpsdate(cPar.Snapshots[tag].UIName);

                    }
                   
                    // mémorise 
                    cReg.WriteRegistreValue("currentSnap", tag.ToString());

                    // création des scenes
                    if (cPar.Snapshots[tag].DashBank.Count > 0)
                    {
                        SnapDashBank B = cPar.Snapshots[tag].DashBank[0];
                        for (int sceneid = 0; sceneid < B.scenes.Count; sceneid++)
                        {
                            SnapDashScene sce = B.scenes[sceneid];

                            DataGridViewRow row = new DataGridViewRow();
                            row.CreateCells(SceneGrid);
                            row.Cells[1].Value = sce.name;
                            row.Cells[1].Tag = sceneid.ToString();
                            row.MinimumHeight = int.Parse(cPar.Params["minimumrowheight"]);
                            if (sce.dasuid != "-1")
                            {
                                row.Cells[1].Style.BackColor = ColorTranslator.FromHtml("#050505");
                            }
                            else
                            {
                                row.Cells[1].Style.BackColor = ColorTranslator.FromHtml("#0505FF");
                            }
                            //row.Cells[1].Style.ForeColor = ColorTranslator.FromHtml( S.color);
                            row.Cells[0].Style.SelectionBackColor = ColorTranslator.FromHtml(sce.color);
                            row.Cells[0].Style.BackColor = ColorTranslator.FromHtml(sce.color);
                            row.DividerHeight = 5;
                            SceneGrid.Rows.Add(row);
                        }
                    }
                    break;
                }
            }
        }

        #endregion

        #region Changement de selection dans la scene -> memorise
        private void SceneGrid_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                // recherche la selection
                string tag = "";
                for (int i = 0; i < SceneGrid.Rows.Count; i++)
                {
                    if (SceneGrid.Rows[i].Cells[1].Selected)
                    {

                        if (i >= 3)
                        {
                            SceneGrid.FirstDisplayedScrollingRowIndex = i - 3;
                        }

                        tag = SceneGrid.Rows[i].Cells[1].Tag.ToString();
                        cPar.tagCurrentScene = tag;
                        cReg.WriteRegistreValue("currentScene", tag);
                        break;
                    }
                }
                SnapDashScene SDS = cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0].scenes[int.Parse(tag)];
                if (!FromOsc && tag != "")
                {
                    // recup le shortcut
                    try
                    {
                        
                        if(SDS.shtcut != null)
                        {
                            string Sc = cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0].scenes[int.Parse(tag)].shtcut[0].eventData;
                            OSCOut.Send(1, Sc);
                        }
                    }
                    catch (Exception ex)
                    {
                        cLog.Error(ex.ToString());
                    }
                }
                // virtual scene ?
                if(int.Parse(cPar.tagCurrentScene) >1 && FromOsc && (cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0].scenes[int.Parse(cPar.tagCurrentScene) -1].dasuid == "-1" || cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0].scenes[int.Parse(cPar.tagCurrentScene)].dasuid == "-1"))
                {
                    string Sc = "";
                    if (cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0].scenes[int.Parse(cPar.tagCurrentScene) - 1].dasuid == "-1")
                    {
                        Sc = cPar.Buttons[int.Parse(cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0].scenes[int.Parse(cPar.tagCurrentScene) - 1].jump_target)].shtcut[0].eventData;
                    }
                    else
                    {
                        Sc = cPar.Buttons[int.Parse(cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0].scenes[int.Parse(cPar.tagCurrentScene)].jump_target)].shtcut[0].eventData;
                    }
                    if (Sc != "")
                    {
                        OSCOut.Send(1, Sc);
                    }
                }





            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }

        #endregion

        #region fait clignoter la bordure d'un bouton si statut = 2 + changement snap si necessaire (présentation) + menu autoclose + surveillance zoom

        private void slash_Tick(object sender, EventArgs e)
        {
            for (int num = 1; num <= cPar.Buttons.Count; num++)
            {

                button but = cPar.Buttons[num];
                if (but.Btname != "")
                {
                    System.Windows.Forms.Button Bt = (System.Windows.Forms.Button)this.Controls[but.Btname];

                    if (but.statut == 0 && Bt.BackColor != ColorTranslator.FromHtml("#123456"))
                    {
                        Bt.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#666666");
                        Bt.BackColor = ColorTranslator.FromHtml("#123456");


                    }

                    if (but.statut == 1 && Bt.BackColor != Color.BlueViolet)
                    {
                        Bt.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#666666");
                        Bt.BackColor = Color.BlueViolet;

                        // met a jour l'UI
                        if (Cui.Connected && cPar.Buttons[int.Parse(Bt.Tag.ToString())].uiname != "")
                        {

                  
                        }
                    }


                    if (but.statut == 2 && Bt.FlatAppearance.BorderColor == System.Drawing.Color.Magenta)
                    {
                        Bt.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#666666");
                        Bt.BackColor = ColorTranslator.FromHtml("#123457");
                    }
                    else
                    {
                        if (but.statut == 2 && Bt.FlatAppearance.BorderColor != System.Drawing.Color.Magenta)
                        {
                            Bt.FlatAppearance.BorderColor = System.Drawing.Color.Magenta;
                            Bt.BackColor = ColorTranslator.FromHtml("#123457");
                        }
                    }
                }
            }

            // check si Presentation en cours
            bool check = false;


            System.Windows.Forms.Button Btx = null;

            for (int n = 1; n <= cPar.Buttons.Count; n++)
            {
                if (cPar.Buttons[n].statut == 1)
                {
                    
                        check = true; 
                        Btx = (System.Windows.Forms.Button)this.Controls[cPar.Buttons[n].Btname];
                    break;

                }

            }

            if (check != cPar.PresentInProgress)
            {
                if (check)
                {
                    cPar.PresentInProgress = true;
                    if (Btx != null)
                    {
                        Cui.snapUpsdate(cPar.Buttons[int.Parse(Btx.Tag.ToString())].uiname);
                        cPar.PresentId = Btx.Tag.ToString();
                        SetChannels(cSnps.getSnapIndex(cPar.Buttons[int.Parse(Btx.Tag.ToString())].uiname));
                        
                    }
                }
                else
                {
                    cPar.PresentInProgress = false;
                    Cui.snapUpsdate(cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].UIName);
                    cPar.PresentId = "";
                    SetChannels();

                }
            }

        


            // autoclose
            if (MenuAutoClose > 0)
            {
                MenuAutoClose--;
                if (MenuAutoClose == 1)
                {
                    MenuPanel.Visible = false;
                }
            }

            // surveillance du zoom
            var parameters = new object[] { };
            if (Cui.Connected && Cui.Document.InvokeScript("GetZoomLevel", parameters).ToString() != "1")
            {
                UIWeb.Document.Focus();
                SendKeys.Send("^{MULTIPLY}");
            }

        }
        #endregion

        #region mouse up/down on buttons
        private void button_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Button Bt = (System.Windows.Forms.Button)sender;
            if (Bt.Tag != null && cPar.Buttons.ContainsKey(int.Parse(Bt.Tag.ToString())) && cPar.Buttons[int.Parse(Bt.Tag.ToString())].shtcut.Count > 0)
            {

                string Sc = cPar.Buttons[int.Parse(Bt.Tag.ToString())].shtcut[0].eventData;
                if (Sc != "")
                {
                    OSCOut.Send(1, Sc);
                }
            }

        }

        private void button_MouseUp(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Button Bt = (System.Windows.Forms.Button)sender;
            if (Bt.Tag != null && cPar.Buttons.ContainsKey(int.Parse(Bt.Tag.ToString())) && cPar.Buttons[int.Parse(Bt.Tag.ToString())].shtcut.Count > 0)
            {
                string Sc = cPar.Buttons[int.Parse(Bt.Tag.ToString())].shtcut[0].eventData;
                if (Sc != "")
                {
                    OSCOut.Send(0, Sc);

                }
            }

        }

        #endregion

        #region timer de connexion
        private void Connect_Tick(object sender, EventArgs e)
        {
            try
            {
                // ouverture UI
                if (!Cui.Connected)
                {
                    if (UIWeb.Document != null)
                    {
                        if (UIWeb.Document.Title == "Navigation annulée" || UIWeb.Document.Title == "Nous ne pouvons pas accéder à cette page")
                        {

                            // open ui web
                            cLog.Info("Retry to open URI " + cPar.Params["URL_UI"]);
                            UIWeb.Url = new Uri(cPar.Params["URL_UI"]);
                            //return;
                        }
                    }
                    if (UIWeb.Document.Title.Contains("Ui"))
                    {

                        Cui.AddJs(); // JS
                        Cui.getSnaps(); // Snaps ui
                        DisplaySnapsGrid();
                        SelectSnap(cReg.ReadRegistreValue("currentSnap", "1"));
                        SelectScene(cReg.ReadRegistreValue("currentScene", "1"));
                        Cui.Connected = true;
                        this.Text = "DAUIServer";
                        this.Activate();

                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            // surveillance UI
            try
            {
                if (Cui.Connected)
                {
                    Cui.Connected = (bool)UIWeb.Document.InvokeScript("isSocketConnected");
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            // création des channels si osc connecté et liste channels vide
            if (cMidi.inpDevok && cMidi.outDevOk && !InitChannels)
            {
                InitChannels = true;
                Connect.Interval = 10000;
                if (cPar.Channels.Count == 0)
                {
                    SetChannels();
                }

            }

            #region récupération du handle de l'application OSC si paramétre
            //get handle of oscApp
            if (cPar.OSChWnd == IntPtr.Zero && !cPar.getHwnd)
            {

                cLog.Info("Get Handle for application " + cPar.Params["OSCFrameToFocus"]);
                Process[] processes;
                processes = Process.GetProcessesByName(cPar.Params["OSCFrameToFocus"]);
                if (processes.Count() > 0)
                {

                    cPar.OSChWnd = processes[0].MainWindowHandle;
                    cLog.Info("Handle ok");
                    if (cPar.OSChWnd != IntPtr.Zero)
                    {
                        cPar.getHwnd = true;
                        if (cPar.Params["MoveDas"].ToLower() == "true")
                        {
                            SetWindowPos(cPar.OSChWnd, 0, this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height, 0x2000);
                        }
                        this.Activate();

                    }
                }
                // création 1 fois de la liste des process pour le log afin de parramétrer correctement la recharceh de ce process
                if (!cPar.getHwnd && !ShowProcess)
                {
                    ShowProcess = true;
                    string lp = "";
                    processes = Process.GetProcesses();
                    foreach (Process P in processes)
                    {
                        lp = lp + P.ProcessName + "-";
                    }
                    cLog.Info("Process Liste : " + lp);
                }
            }
            #endregion

        }
        #endregion

        #region création des channels
        public void SetChannels(int snapId = -1)
        {
            try
            {
                List<string> keys = new List<string>(cPar.Channels.Keys);

                foreach (string key in keys)
                {

                    if (cPar.Channels[key] != null)
                    {
                        cPar.Channels[key].release();
                    }
                    cPar.Channels[key] = null;

                    GC.Collect(0);

                }

                cPar.Channels.Clear();

                if (cPar.tagCurrentSnap != "")
                {
                    int snap = int.Parse(cPar.tagCurrentSnap);
                    if (snapId != -1)
                    {
                        snap = snapId;
                    }

                    foreach (Channel c in cPar.Snapshots[snap].Pages[cPar.CurrentPage].channels)
                    {
                        CChannel CH = new CChannel(c, OSCIn, OSCOut, this);
                        string k = c.dasident == null ? c.dspName : c.dasident;
                        if (k == "")
                        {
                            k = "empty_" + c.id;
                        }
                        cPar.Channels.Add(k, CH);

                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            // N° page -> level meter CC97
            int n = (125 / 8) * (cPar.CurrentPage + 1);
            cMidi.SendCC(97, n);

        }
        #endregion

        #region evenement de connection midi
        public void OnInMidiConnected()
        {
            // midi receive 
            cMidi._inputDevice.EventReceived += OnMidiREceive;
            this.SetChannels();
        }
        public void OnOutMidiConnected()
        {
            this.SetChannels();
        }
        #endregion

        #region reception MIDI affichage menu
        public void OnMidiREceive(object sender, MidiEventReceivedEventArgs e)
        {
            _midievt = e.Event;
            this.Invoke(this.MIDIDelegate);
        }

        public void MidiProcess()
        {
            try
            {
                // Note
                var note = _midievt as NoteOnEvent;
                if (note != null && cPar.XtInput.ContainsKey("Note" + note.NoteNumber) && cPar.XtInput["Note" + note.NoteNumber].channel == 7)
                {
                    // ouverture/fermeture du menu par codeur channel 8
                    string type = cPar.XtInput["Note" + note.NoteNumber].type;
                    if (type == "EP" && note.Velocity == 127)
                    {
                        if (!MenuPanel.Visible)
                        {
                            creatMenu(0);
                            DauiMenu.Rows[0].Selected = true;
                            MenuSelected = 0;
                            MenuAutoClose = 10;
                            ShowUiFrm();
                        }
                        else
                        {
                            string tag = DauiMenu.Rows[MenuSelected].Cells[1].Tag.ToString();

                            // add virtual scene
                            if (DauiMenu.Rows[MenuSelected].Cells[1].Tag.ToString().Contains("bt_"))
                            {
                                string elementid = DauiMenu.Rows[MenuSelected].Cells[1].Tag.ToString().Substring(3);

                                // recup DASUID after Scene
                                string after = cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0].scenes[int.Parse(cPar.tagCurrentScene)].dasuid;
                                // recup DASUID Scene to add
                                button Bt = cPar.Buttons[int.Parse(elementid)];
                                string result = cSnps.AddVirtualScene(Bt, after);
                                



                                string oldScene = cPar.tagCurrentScene;
                                if (result == "")
                                {
                                    updatescene();
                                }

                                //reselectionne la scene
                                SelectScene(oldScene);

                                // décallage des snapkeys
                                List<string> keytomodif = new List<string>();
                                
                                if (result == "")
                                {
                                    foreach (KeyValuePair<string, SnapResult> snap in cPar.SnapResults)
                                    {
                                        SnapResult SR = snap.Value;
                                        if (SR.snapkey == int.Parse(cPar.tagCurrentSnap) && SR.sceneid > int.Parse(cPar.tagCurrentScene))
                                        {
                                            keytomodif.Add(snap.Key.ToString());
                                        }
                                    }

                                    foreach (string K in keytomodif)
                                    {
                                        SnapResult SR = cPar.SnapResults[K];
                                        SR.sceneid++;
                                        cPar.SnapResults[K] = SR;
                                    }
                                }

                            }
                            // remove virtual scene
                            if (tag == "9") 
                            {
                                // efface la scene
                                SnapDashBank BNK = cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0];
                                BNK.scenes.RemoveAt(int.Parse(cPar.tagCurrentScene));
                                SnapDashScene SCN = BNK.scenes[int.Parse(cPar.tagCurrentScene) - 1];
                                SCN.color = cPar.Params["ColorStd"];
                                BNK.scenes[int.Parse(cPar.tagCurrentScene) - 1] = SCN;
                                cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0] = BNK;
                                string oldScene = "0";
                                if (int.Parse(cPar.tagCurrentScene) > 0)
                                {
                                   oldScene = (int.Parse(cPar.tagCurrentScene) - 1).ToString();
                                }
                                string result = cSnps.RemoveVirtualScene(SCN);
                                if(int.Parse(cPar.tagCurrentScene) > 0)
                                {
                                    cPar.tagCurrentScene =  (int.Parse(cPar.tagCurrentScene)-1).ToString();
                                }

                                if (result == "")
                                {
                                    updatescene();
                                }
                                //reselectionne la scene
                                SelectScene(oldScene);
                                // décallage des snapkeys
                                List<string> keytomodif = new List<string>();
                                if (result == "")
                                {
                                    foreach (KeyValuePair<string, SnapResult> snap in cPar.SnapResults)
                                    {
                                        SnapResult SR = snap.Value;
                                        if (SR.snapkey == int.Parse(cPar.tagCurrentSnap) && SR.sceneid > int.Parse(cPar.tagCurrentScene))
                                        {
                                            keytomodif.Add(snap.Key.ToString());
                                        }
                                    }

                                    foreach (string K in keytomodif)
                                    {
                                        SnapResult SR = cPar.SnapResults[K];
                                        SR.sceneid--;
                                        cPar.SnapResults[K] = SR;
                                    }
                                }
                            }

                            if (DauiMenu.Rows[MenuSelected].Cells[1].Tag.ToString().Contains("SNAP"))
                            {
                                SelectSnap(DauiMenu.Rows[MenuSelected].Cells[1].Tag.ToString().Substring(4));
                                MenuPanel.Visible = false;
                                return;
                            }
                            if (tag == "0")  // update snap
                            {
                                Cui.snapUpdate();
                                MenuPanel.Visible = false;
                                return;
                            }

                            //if (tag == "1") // init interface web
                            //{
                            //    InitInterWeb();
                            //    MenuPanel.Visible = false;
                            //    return;
                            //}
                            if (tag == "2") // affiche le menu des snap
                            {
                                creatMenu(1);

                                return;
                            }
                            if (tag == "3") // affiche le menu pour quitter
                            {
                                creatMenu(2);

                                return;
                            }

                            if (tag == "8") // affiche le menu des elements possibles
                            {
                                creatMenu(3);

                                return;
                            }


                            if (tag == "5") // quitte l'application
                            {
                                this.Close();

                                return;
                            }

                            if (tag == "4") // exit menu
                            {
                                MenuPanel.Visible = false;
                                return;
                            }

                            if (tag == "6") // reset size
                            {

                                UIWeb.Document.Focus();
                                SendKeys.Send("^{MULTIPLY}");

                                MenuPanel.Visible = false;
                                return;
                            }

                            if (tag == "7") // minimize
                            {

                                this.WindowState = FormWindowState.Minimized;
                                MenuPanel.Visible = false;
                                return;
                            }





                        }
                        MenuPanel.Visible = !MenuPanel.Visible;
                    }

                    // page + - page -
                    string name = cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].Pages[cPar.CurrentPage].channels[cPar.XtInput["Note" + note.NoteNumber].channel].dspName;
                    if (name == "m" || name == "D MAST")
                    {
                        // page +
                        if (type == "SOLO" && note.Velocity == 127)
                        {
                            cPar.CurrentPage++;
                            if (cPar.CurrentPage > cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].Pages.Count() - 1) { cPar.CurrentPage = 0; }
                            SetChannels();
                        }
                        // page -
                        if (type == "MUTE" && note.Velocity == 127)
                        {
                            cPar.CurrentPage--;
                            if (cPar.CurrentPage < 0) { cPar.CurrentPage = cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].Pages.Count() - 1; }
                            SetChannels();
                        }
                        ShowUiFrm();
                    }
                }
                // codeur ?
                var CC = _midievt as ControlChangeEvent;
                if (CC != null && CC.EventType.ToString() == "ControlChange" && cPar.XtInput.ContainsKey("CC" + CC.ControlNumber) && 7 == cPar.XtInput["CC" + CC.ControlNumber].channel && cPar.XtInput["CC" + CC.ControlNumber].type == "ER")
                {
                    if (!MenuPanel.Visible)
                    {
                        DauiMenu.Rows[0].Selected = true;
                        MenuSelected = 0;
                        MenuAutoClose = 10;
                        creatMenu(0);
                        MenuPanel.Visible = true;
                        ShowUiFrm();
                        return;
                    }
                    if (CC.ControlValue == 1)
                    {
                        if (MenuSelected > 0) { MenuSelected--; }
                        if (DauiMenu.Rows[MenuSelected].Cells[1].Tag.ToString() == "") { MenuSelected--; }
                    }
                    else
                    {
                        if (MenuSelected < DauiMenu.Rows.Count - 1) { MenuSelected++; }
                        if (DauiMenu.Rows[MenuSelected].Cells[1].Tag.ToString() == "") { MenuSelected++; }
                    }
                    DauiMenu.Rows[MenuSelected].Selected = true;
                    if (MenuSelected >= 3)
                    {
                        DauiMenu.FirstDisplayedScrollingRowIndex = MenuSelected - 3;
                    }
                    MenuAutoClose = 10;
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region selection dans le menu
        private void Menu_SelectionChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < DauiMenu.Rows.Count; i++)
            {
                if (DauiMenu.Rows[i].Cells[1].Selected)
                {
                    MenuSelected = i;
                    if (DauiMenu.Rows[i].Cells[1].Tag.ToString() == "")
                    {
                        DauiMenu.Rows[i + 1].Selected = true;
                        MenuSelected = i + 1;
                    }
                }
            }
        }
        #endregion

        #region Intialisation de l'interface web
        private void InitInterWeb()
        {
            try
            {
                this.Text = "INIT WEB";
                Connect.Interval = 1000;
                Cui.Connected = false;
                UIWeb.Dispose();
                this.UIWeb = new System.Windows.Forms.WebBrowser();
                this.UIWeb.AllowWebBrowserDrop = false;
                this.UIWeb.Anchor = System.Windows.Forms.AnchorStyles.None;
                this.UIWeb.IsWebBrowserContextMenuEnabled = false;
                this.UIWeb.Location = new System.Drawing.Point(1, 2);
                this.UIWeb.MinimumSize = new System.Drawing.Size(20, 20);
                this.UIWeb.Name = "UIWeb";
                this.UIWeb.ScrollBarsEnabled = false;
                this.UIWeb.Size = new System.Drawing.Size(637, 447);
                this.UIWeb.TabIndex = 3;
                this.UIWeb.Url = new System.Uri("", System.UriKind.Relative);
                this.UIWeb.WebBrowserShortcutsEnabled = false;


                this.Controls.Add(this.UIWeb);
                UIWeb.Location = new System.Drawing.Point(0, buttonHeight + sep);
                UIWeb.Size = new System.Drawing.Size(SnapGrid.Location.X - sep, this.Height - (buttonHeight + (cPar.Params["borderstate"] == "0" ? 5 : 7 * sep)));
                UIWeb.Url = new Uri(cPar.Params["URL_UI"]);

                this.Focus();
                UIWeb.Focus();


                InitChannels = false;
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

        }
        #endregion

        #region Show frm
        public void ShowUiFrm()
        {
            if (cPar.OSCTOP && Cui.Connected && cMidi.inpDevok && cMidi.outDevOk)
            {
                cPar.OSCTOP = false;
                //FrmMain.SetForegroundWindow(cPar.UIhWnd);
               // this.Activate();
                FrmMain.SwitchToThisWindow(cPar.UIhWnd, true);
                cLog.Info("Set UI on Top with UIhWnd : "+ cPar.UIhWnd);
                this.TopMost = true;
                this.TopMost = false;
            }
        }

        #endregion
       
        #region bouton ouverture du menu
        private void button_menu_Click(object sender, EventArgs e)
        {
            NoteEvent note = new NoteOnEvent()  ;
            note.Velocity = (Melanchall.DryWetMidi.Common.SevenBitNumber) 127;
            note.NoteNumber = (Melanchall.DryWetMidi.Common.SevenBitNumber) 7;
            _midievt = note;
            MenuAutoClose = 10;
            MidiProcess();
        }
        #endregion

        #region click sur élément du menu
        private void DauiMenu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            NoteEvent note = new NoteOnEvent();
            note.Velocity = (Melanchall.DryWetMidi.Common.SevenBitNumber)127;
            note.NoteNumber = (Melanchall.DryWetMidi.Common.SevenBitNumber)7;
            _midievt = note;
            MenuAutoClose = 10;
            MidiProcess();

        }
        #endregion
    }
}
