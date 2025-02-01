using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using static System.Net.Mime.MediaTypeNames;
using static DAUIServer.Cparams;
using static DAUIServer.FrmMain;
using static DAUIServer.UDPSocket;

namespace DAUIServer
{
    public sealed class CMidi
    {
        public struct Message
        {
            public string up;
            public string dwn;
        }
        public Dictionary<int, Message> Messages = new Dictionary<int, Message>();

        private cLogger cLog = cLogger.Instance;
        private Cparams cPar = Cparams.Instance;
        

        private static CMidi instance = null;
        private static readonly object padlock = new object();
        public IOutputDevice _outputDevice;
        public IInputDevice _inputDevice;
        public bool inpDevok = false;
        public bool outDevOk = false;
        private System.Windows.Forms.Timer Connect;
        public delegate void evtError(String log);
        public event evtError ErrorEvt;


        public delegate void evtInConnect();
        public event evtInConnect InConnectEvt;

        public delegate void evtOutConnect();
        public event evtOutConnect OutConnectEvt;


        public static CMidi Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new CMidi();
                    }
                    return instance;
                }
            }
        }

        public CMidi()
        {
            this.Connect = new System.Windows.Forms.Timer();
            Connect.Enabled = true;
            Connect.Interval = 500;
            Connect.Tick += new System.EventHandler(Connect_Tick);
            Connect_Tick(null, null);
        }
        #region envoi information note
        public void SendNote(int note, int Velocity)
        {
            try
            {
                if (inpDevok && outDevOk)
                {
                    _outputDevice.SendEvent(new NoteOnEvent(new Melanchall.DryWetMidi.Common.SevenBitNumber((byte)note), new Melanchall.DryWetMidi.Common.SevenBitNumber((byte)Velocity)));
                    _outputDevice.SendEvent(new NoteOffEvent(new Melanchall.DryWetMidi.Common.SevenBitNumber((byte)note), new Melanchall.DryWetMidi.Common.SevenBitNumber(0)));
                }
            }
            catch (Exception ex)
            {
                cLog.Info("error 3500 " + ex.ToString());
                _outputDevice.Dispose();
                outDevOk = false;
                inpDevok = false;
                ErrorEvt?.Invoke(ex.ToString());
            }

        }
        #endregion

        #region affiche information LCD
        // color 0:black, 1 red, 2 green, 3 yellow, 4 blue, 5 magenta, 6 cyan, 7 white
        public void SendMsg(int afficheur, int color, bool l1_inv, bool l2_inv, string message_up, string message_dwn)
        {
            try
            {
                if (inpDevok && outDevOk)
                {
                    int res = 0;
                    if (afficheur > 7) afficheur = 7;
                    if (color > 7) color = 7;
                    res = color;
                    if (l1_inv) res += 16;
                    if (l2_inv) res += 32;
                    byte[] trame = new byte[] { 0x00, 0x20, 0x32, 0x15, 0x4C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF7 };
                    trame[5] = (byte)afficheur;
                    trame[6] = (byte)res;
                    if (message_up == "" && Messages.ContainsKey(afficheur))
                    {
                        if (Messages.ContainsKey(afficheur))
                        {
                            message_up = Messages[afficheur].up;
                        }
                        else
                        {
                            message_up = "       ";
                        }
                    }
                    if (message_dwn == "" )
                    {
                        if (Messages.ContainsKey(afficheur))
                        {
                            message_dwn = Messages[afficheur].dwn;
                        }
                        else
                        {
                            message_dwn = "       ";
                        }
                    }

                    if (message_up.Length > 7) message_up = message_up.Substring(0, 6);
                    if (message_dwn.Length > 7) message_dwn = message_dwn.Substring(0, 6);
                    message_up = centre(message_up,7);
                    message_dwn = centre(message_dwn,7);
                    Message M = new Message();
                    M.up = message_up;
                    M.dwn = message_dwn;
                    Messages[afficheur] = M;

                    byte[] ba = Encoding.Default.GetBytes(message_up+message_dwn);
                    int pos = 7;
                    foreach (byte car in ba)
                    {
                        trame[pos] = car;
                        pos++;
                    }
                    _outputDevice.SendEvent(new NormalSysExEvent(trame));
                }
            }
            catch (Exception ex)
            {
                cLog.Info("error 3500 " + ex.ToString());
                _outputDevice.Dispose();
                outDevOk = false;
                inpDevok = false;
                ErrorEvt?.Invoke(ex.ToString());
            }

        }
        #endregion

        #region centre un text
        public string centre(string txt, int len)
        {
            try
            {
                int td = txt.Length;
                if (td > len)
                {
                    txt = txt.Substring(0, len);
                }
                else
                {
                    txt = txt.PadLeft(((len - td) / 2) + td);
                    txt = (txt + "       ").Substring(0, len);
                }
                return txt;
            }
            catch (Exception ex)
            {
                cLog.Info("" + ex.ToString());
                return "";
            }
        }
        #endregion

        #region envoi information controlchange (pot electrique...)

        public void SendCC(string note, int Velocity)
        {
            string N = note.ToLower().Replace("cc", "").Replace("note", "");
            SendCC(int.Parse(N),Velocity);
        }
        
        
        public void SendCC(int note, int Velocity)
        {
            try
            {
                if (inpDevok && outDevOk)
                {
                    _outputDevice.SendEvent(new ControlChangeEvent(new Melanchall.DryWetMidi.Common.SevenBitNumber((byte)note), new Melanchall.DryWetMidi.Common.SevenBitNumber((byte)Velocity)));
                }
            }
            catch (Exception ex)
            {
                cLog.Info("error 3500 " + ex.ToString());
                _outputDevice.Dispose();
                outDevOk = false;
                inpDevok = false;
                ErrorEvt?.Invoke(ex.ToString());
            }
            
        }
        #endregion

        #region timer de connexion
        private void Connect_Tick(object sender, EventArgs e)
        {
            if (InputDevice.GetAll().Count == 0 && inpDevok)
            {
                cLog.Info("error 3501 ");
                _outputDevice.Dispose();
                outDevOk = false;
                inpDevok = false;
                ErrorEvt?.Invoke("disconnected");

            }
            // connection MIDI ouptut device
            try
            {
                if (!outDevOk && OutputDevice.GetAll().Count > 0)
                {
                    foreach (var outDevice in OutputDevice.GetAll())
                    {
                        if (cPar.Params["ui_output_device"] == outDevice.Name)
                        {
                            _outputDevice = null;
                            _outputDevice = OutputDevice.GetByName(cPar.Params["ui_output_device"]);
                            outDevOk = true;
                            cLog.Info("Midi output device open.");
                            OutConnectEvt();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            try
            {
                
                if (cPar.Params.Count == 0)
                    return;
                // connexion MIDI input device
                if (!inpDevok && InputDevice.GetAll().Count > 0)
                {
                   foreach (var inputDevice in InputDevice.GetAll())
                    {
                        if (cPar.Params["ui_input_device"] == inputDevice.Name)
                        {
                            _inputDevice = InputDevice.GetByName(cPar.Params["ui_input_device"]);


                            cLog.Info("Midi input device open.");
                            _inputDevice.StartEventsListening();
                            inpDevok = true;
                            InConnectEvt();
                           
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            

            //if(outDevOk && inpDevok && Connect.Interval == 1000)
            //{
            //    Connect.Interval = 10000;
            //}

        }
        #endregion
    }
}
