using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using static DAUIServer.UDPSocket;

using System.Timers;
using System.Linq;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace DAUIServer
{

    // represente un currseur de l'UI
    public class CChannel
    {
        public int ChannelNumber = -1;
        private string ChannelDasIdent = "";
        private string ChannelName = "";
        private string ChannelType = "";
        private System.Timers.Timer cTimer;

        private cLogger cLog = cLogger.Instance;
        private Cparams cPar = Cparams.Instance;
        private CMidi cMidi = CMidi.Instance;
        private CUi Cui = CUi.Instance;
        private UDPSocket OSCIn = null;
        private UDPSocket OSCOut = null;
        private int ChannelColor = 0;
        private bool initUI = false;
        private float currentvalue = -1;
        public bool ChannelRecord = false;
        MidiEvent _evt;

        FrmMain parent;

        public delegate void MProcess();
        public MProcess TimerDelegate;

        public delegate void MiProcess();
        public MiProcess MidiDelegate;

        private int XtouchInPro = 0;
        private string key = "";
        private int index = 0;
        private string UISolo = "";
        private string UIMute = "";
        private string UIGain = "";
        public int mode = -1; // -1 = mix; 0 = equ; 1 = comp; 2=fx; 3= retour 

        private string MemInfoChannelUI = "";
        private Dictionary<string, string> InfoChannel = new Dictionary<string, string>();

        public CChannel(Cparams.Channel C, UDPSocket OscIn, UDPSocket OscOut, FrmMain FRM)
        {


            try
            {
                TimerDelegate = new MProcess(TimerProcess);
                MidiDelegate = new MiProcess(MidiProcess);

                ChannelNumber = C.id;
                ChannelDasIdent = C.dasident;
                ChannelType = C.type.ToLower();
                ChannelName = C.dspName;
                ChannelColor = C.color;
                parent = FRM;

                // init voyants
                cMidi.SendNote(int.Parse(cPar.GetOutputByType("REC", ChannelNumber).Replace("Note", "")), 0);
                cMidi.SendNote(int.Parse(cPar.GetOutputByType("SOLO", ChannelNumber).Replace("Note", "")), 0);
                cMidi.SendNote(int.Parse(cPar.GetOutputByType("MUTE", ChannelNumber).Replace("Note", "")), 0);
                cMidi.SendNote(int.Parse(cPar.GetOutputByType("SELECT", ChannelNumber).Replace("Note", "")), 0);
                cMidi.SendCC(cPar.GetOutputByType("EL", ChannelNumber), 0);

                if (ChannelName != "")
                {
                    OSCIn = OscIn;
                    OSCOut = OscOut;
                    cTimer = new System.Timers.Timer(200);
                    cTimer.Elapsed += OnTimedEvent;
                    if(cMidi._inputDevice != null)
                        cMidi._inputDevice.EventReceived += OnMidiREceive;
                    string pagename = "";
                    string shotname = "";
                    if (cPar.tagCurrentSnap != "")
                    {

                        shotname = cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].name;
                    }
                    if(cPar.PresentInProgress)
                    {
                        shotname = "Present";
                    }
                    // reset frame ui
                    Cui.setMixMode(0);

                    if (ChannelType == "ui")
                    {
                        string[] e = ChannelName.Split('.');
                        key = e[0];
                        if (e.Count() > 1 && e[1] != "") { index = int.Parse(e[1]); } else { index = 0; }
                        cTimer.Enabled = true;
                        if (key == "m")
                        {

                            // affiche le nom de la page + Snapname pour le canal master
                            pagename = cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].Pages[cPar.CurrentPage].name;
                            cMidi.SendMsg(ChannelNumber, ChannelColor, true, false, pagename, shotname);

                            // allume voyant select si comment ouvert
                            if (parent.FRMCMD.Visible == true)
                            {
                                cMidi.SendNote(int.Parse(cPar.GetOutputByType("SELECT", ChannelNumber).Replace("Note", "")), 127);
                            }
                        }
                    }
                    else
                    {
                        OSCIn.OscEventReceive += OnOscREceive;
                        cTimer.Enabled = false;
                        Cparams.fader f = cPar.Faders[ChannelDasIdent];
                        int note = int.Parse(cPar.GetOutputByType("MF", ChannelNumber).Replace("CC", ""));
                        int val = (int)Math.Round(127 * f.value, 0);
                        cMidi.SendCC(note, val);
                        if (ChannelName == "D MAST")
                        {
                            // affiche le nom de la page + Snapname pour le canal master
                            pagename = cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].Pages[cPar.CurrentPage].name;
                            cMidi.SendMsg(ChannelNumber, ChannelColor, true, false, ChannelName, shotname);
                        }
                        else
                        {
                            // affiche le voyant si présentation.
                            if(cPar.PresentInProgress)
                            {
                                if(cPar.PresentId == "1")
                                    cMidi.SendNote(int.Parse(cPar.GetOutputByType("REC", ChannelNumber).Replace("Note", "")), 64);
                                if (cPar.PresentId == "2")
                                    cMidi.SendNote(int.Parse(cPar.GetOutputByType("SOLO", ChannelNumber).Replace("Note", "")), 64);
                                if (cPar.PresentId == "3")
                                    cMidi.SendNote(int.Parse(cPar.GetOutputByType("MUTE", ChannelNumber).Replace("Note", "")), 64);
                            }

                        }
                        

                    }

                    // affiche le nom du canal
                    if (key != "m" && ChannelName != "D MAST")
                    {
                        cMidi.SendMsg(ChannelNumber, ChannelColor, true, false, ChannelType == "das" ? ChannelName : (key == "i" ? "CH " : "Line ") + (index + 1).ToString(), " ");
                    }
                }
                else
                {
                    cMidi.SendMsg(ChannelNumber, 0, false, false, " ", " ");
                    int note = int.Parse(cPar.GetOutputByType("MF", ChannelNumber).Replace("CC", ""));
                    cMidi.SendCC(note, 0);
                }

            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }

        #region relase les anciens channels


        ~CChannel()
        {
            //Console.WriteLine("Out.." + UIDC);
        }

        public void release()
        {
            try
            {
                if (ChannelName != "")
                {
                    if (cMidi._inputDevice != null)
                    {
                        cMidi._inputDevice.EventReceived -= OnMidiREceive;
                    }
                    OSCIn.OscEventReceive -= OnOscREceive;

                    OSCIn = null;
                    OSCOut = null;
                    parent = null;

                    cTimer.Stop();
                    cTimer.Elapsed -= OnTimedEvent;
                    cTimer.Dispose();
                    cTimer = null;

                    TimerDelegate = null;
                    MidiDelegate = null;
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

        }
        #endregion

        #region Timer de surveillance
        private void OnTimedEvent(object sender, EventArgs e)
        {
            parent.Invoke(this.TimerDelegate, null);
        }
        public void TimerProcess()
        {
            try
            {
                if (ChannelType == "ui" && Cui.Connected && parent != null)
                {

                    string r = Cui.getChannel(ChannelName);

                    if (r == "")
                    {
                        return;
                    }
                    if (r != MemInfoChannelUI)
                    {
                        MemInfoChannelUI = r;
                        InfoChannel.Clear();
                        InfoChannel = r.Split(new[] { '~' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(part => part.Split('='))
                        .ToDictionary(split => split[0], split => split[1].Replace(".", ","));

                        // voyant solo                                              
                        if (InfoChannel.ContainsKey(key + "." + index.ToString() + ".solo") && InfoChannel[key + "." + index.ToString() + ".solo"] != UISolo)
                        {
                            UISolo = InfoChannel[key + "." + index.ToString() + ".solo"];
                            cMidi.SendNote(int.Parse(cPar.GetOutputByType("SOLO", ChannelNumber).Replace("Note", "")), int.Parse(UISolo) * 65);
                        }

                        // voyant mute
                        if (InfoChannel.ContainsKey(key + "." + index.ToString() + ".mute") && InfoChannel[key + "." + index.ToString() + ".mute"] != UIMute)
                        {
                            UIMute = InfoChannel[key + "." + index.ToString() + ".mute"];
                            cMidi.SendNote(int.Parse(cPar.GetOutputByType("MUTE", ChannelNumber).Replace("Note", "")), int.Parse(UIMute) * 64);
                        }

                        // gain
                        if (InfoChannel.ContainsKey(key + "." + index.ToString() + ".gain") && InfoChannel[key + "." + index.ToString() + ".gain"] != UIGain)
                        {
                            UIGain = InfoChannel[key + "." + index.ToString() + ".gain"];
                            cMidi.SendCC(cPar.GetOutputByType("EL", ChannelNumber), (int)(float.Parse(UIGain) * 127));
                        }


                    }


                    if (!initUI && key != "m")
                    {
                        cMidi.SendMsg(ChannelNumber, ChannelColor, true, false, "", InfoChannel[ChannelName + ".name"]);
                        initUI = true;
                    }

                    // recupére la valeur
                    float redVal = (float)Math.Round(float.Parse(InfoChannel[ChannelName + ".mix"]), 3);
                    if (currentvalue != redVal && XtouchInPro == 0)
                    {
                        currentvalue = redVal;
                        int note = int.Parse(cPar.GetOutputByType("MF", ChannelNumber).Replace("CC", ""));
                        cMidi.SendCC(note, (int)(127 * redVal));
                    }
                }
                if (XtouchInPro > 0) XtouchInPro--;
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion


        #region reception X-Touch
        public void OnMidiREceive(object sender, MidiEventReceivedEventArgs e)
        {
            _evt = e.Event;

            if (this.MidiDelegate != null)
            {

                parent.Invoke(this.MidiDelegate, null);
            }
        }

        public void MidiProcess()
        {

            try
            {

                // Note
                var note = _evt as NoteOnEvent;
                if (key != "m" && note != null && note.EventType.ToString() == "NoteOn" && cPar.XtInput.ContainsKey("Note" + note.NoteNumber) && ChannelNumber == cPar.XtInput["Note" + note.NoteNumber].channel)
                {

                    string type = cPar.XtInput["Note" + note.NoteNumber].type;
                    if ((type.ToLower() == "mute" || type.ToLower() == "solo") && ChannelType == "ui" && (key != "m" || key != "All") && note.Velocity > 0)
                    {
                        setEditMode(0);
                        float v = 1;
                        if (InfoChannel[key + "." + index.ToString() + "." + type.ToLower()] == "1") { v = 0; }
                        Cui.setvalue(key, index.ToString(), type.ToLower(), v);
                    }

                    // Presentation 1
                    if (type.ToLower() == "rec"  && ChannelType == "das"  && note.Velocity > 0)
                    {
                        string Sc = cPar.Buttons[1].shtcut[0].eventData;
                        if (Sc != "")
                        {
                            if (note.Velocity > 0)
                            {
                                cMidi.SendNote(int.Parse(cPar.GetOutputByType("REC", ChannelNumber).Replace("Note", "")), 65);
                                OSCOut.Send(1, Sc);
                            }
                            else
                            {
                                cMidi.SendNote(int.Parse(cPar.GetOutputByType("REC", ChannelNumber).Replace("Note", "")), 0);
                                OSCOut.Send(0, Sc);
                            }
                        }
                    }

                    // Presentation 2
                    if (type.ToLower() == "solo" && ChannelType == "das" && note.Velocity > 0)
                    {
                        string Sc = cPar.Buttons[2].shtcut[0].eventData;
                        if (Sc != "")
                        {
                            if (note.Velocity > 0)
                            {
                                cMidi.SendNote(int.Parse(cPar.GetOutputByType("SOLO", ChannelNumber).Replace("Note", "")), 65);
                                OSCOut.Send(1, Sc);
                            }
                            else
                            {
                                cMidi.SendNote(int.Parse(cPar.GetOutputByType("SOLO", ChannelNumber).Replace("Note", "")), 0);
                                OSCOut.Send(0, Sc);
                            }
                        }
                    }

                    // Presentation 3
                    if (type.ToLower() == "mute" && ChannelType == "das" && note.Velocity > 0)
                    {
                        string Sc = cPar.Buttons[3].shtcut[0].eventData;
                        if (Sc != "")
                        {
                            if (note.Velocity > 0)
                            {
                                cMidi.SendNote(int.Parse(cPar.GetOutputByType("MUTE", ChannelNumber).Replace("Note", "")), 65);
                                OSCOut.Send(1, Sc);
                            }
                            else
                            {
                                cMidi.SendNote(int.Parse(cPar.GetOutputByType("MUTE", ChannelNumber).Replace("Note", "")), 0);
                                OSCOut.Send(0, Sc);
                            }
                        }
                    }






                    if (type.ToLower() == "select" && note.Velocity > 0)
                    {
                        if (ChannelType == "das" && !cPar.OSCTOP)
                        {
                            cPar.OSCTOP = true;
                            FrmMain.SetForegroundWindow(cPar.OSChWnd);

                            FrmMain.SwitchToThisWindow(cPar.OSChWnd, true);

                            cLog.Info("Set DAS on Top");
                        }

                        if (ChannelType == "ui")
                        {
                            parent.ShowUiFrm();
                        }

                    }
                    if (type.ToLower() == "rec" && note.Velocity > 0)
                    {
                        if (ChannelType == "ui")
                        {
                            //if (!editmode)
                            //{
                            // passe tous les autre "mode" des autre channel à false
                            foreach (KeyValuePair<string, CChannel> ch in cPar.Channels)
                            {
                                if (ch.Value.ChannelNumber != ChannelNumber)
                                {
                                    ch.Value.setEditMode(-1);
                                }
                            }
                            parent.ShowUiFrm();

                            mode++;
                            if (mode > 3)
                            {
                                mode = -1;
                            }
                            if (mode == -1)
                            {
                                Cui.setMixMode(0);
                            }
                            else
                            {
                                Cui.setEditMode(index, mode);
                            }
                            setEditMode(mode);
                            //}
                            //else
                            //{
                            //    Cui.setMixMode(0);
                            //    setEditMode(false);

                            //}

                        }

                    }


                }

                if (key == "m" && note != null && note.EventType.ToString() == "NoteOn" && note.Velocity > 0 && ChannelNumber == cPar.XtInput["Note" + note.NoteNumber].channel)
                {
                    string type = cPar.XtInput["Note" + note.NoteNumber].type.ToLower();

                    if (type == "select")
                    {

                        if (!parent.FRMCMD.Visible)
                        {
                            parent.FRMCMD.Visible = true;
                            cMidi.SendNote(int.Parse(cPar.GetOutputByType("SELECT", ChannelNumber).Replace("Note", "")), 127);
                        }
                        else
                        {
                            parent.FRMCMD.Visible = false;
                            cMidi.SendNote(int.Parse(cPar.GetOutputByType("SELECT", ChannelNumber).Replace("Note", "")), 0);
                        }
                    }


                    if (type.ToLower() == "rec")
                    {

                        // passe tous les "editmode" des autre channel à false
                        foreach (KeyValuePair<string, CChannel> ch in cPar.Channels)
                        {
                            if (ch.Value.ChannelNumber != ChannelNumber)
                            {
                                ch.Value.setEditMode(-1);
                            }
                        }
                        parent.ShowUiFrm();
                        mode++;
                        if (mode > 3)
                        {
                            mode = -1;
                        }

                        if (mode == -1)
                        {
                            Cui.setMixMode(0);
                        }
                        else
                        {
                            Cui.setEditMode(-1, mode);
                        }

                    }
                }

                // CC
                var CC = _evt as ControlChangeEvent;
                if (CC != null && CC.EventType.ToString() == "ControlChange" && cPar.XtInput.ContainsKey("CC" + CC.ControlNumber) && ChannelNumber == cPar.XtInput["CC" + CC.ControlNumber].channel)
                {
                    if (ChannelType == "das")
                    {
                        if (cPar.Faders.ContainsKey(ChannelDasIdent))
                        {
                            Cparams.fader f = cPar.Faders[ChannelDasIdent];
                            float value = (float)Math.Round((float)CC.ControlValue / 127, 3);
                            if (value != f.value)
                            {
                                OSCOut.Send(value, f.shtcut.eventData, "f");
                                f.value = value;
                                cPar.Faders[ChannelDasIdent] = f;


                            }

                        }
                    }
                    if (ChannelType == "ui")
                    {
                        parent.ShowUiFrm();
                        if (cPar.XtInput["CC" + CC.ControlNumber.ToString()].type == "FM")
                        {
                            XtouchInPro = 5;
                            currentvalue = (float)Math.Round((float)CC.ControlValue / 127, 3);
                            Cui.setvalue(key, key == "m" ? "" : index.ToString(), "mix", currentvalue);
                            if (mode > 0)
                            {
                                Cui.setMixMode(0);
                                setEditMode(0);
                            }

                            return;
                        }

                        if (cPar.XtInput["CC" + CC.ControlNumber.ToString()].type == "ER" && UIGain != "")
                        {

                            float G = float.Parse(UIGain);

                            if (CC.ControlValue > 1)
                            {
                                G = G + (float)0.005;
                                if (G > 1) { G = 1; }
                            }
                            else
                            {
                                G = G - (float)0.005;
                                if (G < 0) { G = 0; }
                            }
                            Cui.setvalue(key, index.ToString(), "gain", G);
                            UIGain = G.ToString();
                            return;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

        }

        #endregion

        #region editmode
        public void setEditMode(int value)
        {
            mode = value;
            cMidi.SendNote(int.Parse(cPar.GetOutputByType("REC", ChannelNumber).Replace("Note", "")), value == -1 ? 0 : 126);
        }
        #endregion


        #region reception das par OSC
        public void OnOscREceive(object sender, OscEventReceiveArg e)
        {
            try
            {


                // get shortcut
                string sc = cPar.GetDasIdentBySc(e.key);
                // fader ?
                if (sc != null && cPar.Faders.ContainsKey(sc) && ChannelNumber == cPar.Faders[sc].index)
                {
                    Cparams.fader fad = cPar.Faders[sc];
                    if (fad.value != e.value)
                    {
                        fad.value = e.value;
                        cPar.Faders[sc] = fad;
                        int note = int.Parse(cPar.GetOutputByType("MF", ChannelNumber).Replace("CC", ""));
                        int val = (int)Math.Round(127 * e.value, 0);
                        cMidi.SendCC(note, val);
                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion


    }
}