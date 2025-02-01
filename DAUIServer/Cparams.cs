using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DAUIServer
{
    public class Cparams
    {
        private static Cparams instance = null;
        private static readonly object padlock = new object();

        public Dictionary<string, string> Params = new Dictionary<string, string>();         // paramétres application
        public Dictionary<int, SnapShot> Snapshots = new Dictionary<int, SnapShot>();        // liste des snapshots
        public Dictionary<int, button> Buttons = new Dictionary<int, button>();              // liste des boutons
        public Dictionary<string, fader> Faders = new Dictionary<string, fader>();              // liste des boutons

        public Dictionary<string, SnapResult> SnapResults = new Dictionary<string, SnapResult>(); // retourne les infos suivant le shurtcut
        public Dictionary<string, int> ButtonResults = new Dictionary<string, int>(); // retourne les infos suivant le shurtcut

       // public List<CChannel> Channels = new List<CChannel>();

        public Dictionary<string, CChannel> Channels = new Dictionary<string, CChannel>();

        public Dictionary<string, XTouch> XtInput = new Dictionary<string, XTouch>();
        public Dictionary<string, XTouch> XtOutput = new Dictionary<string, XTouch>();
       // public Dictionary<string, float> FaderValue = new Dictionary<string, float>();


        public string tagCurrentSnap = "";      // tag du snap en cours
        public string tagCurrentScene = "";     // tag de la scene en cours
        public bool PresentInProgress = false;  // si true presentation en cours
        public string PresentId = "";               // numéro de la présentation en cours
        public int CurrentPage = 0;            // page courante

        public Dictionary<string,int> Shortcutnumber = new Dictionary<string, int>();
        public int maxTargetIndex = 0; // nombre de raccourcie pour ajouter nouveau


        public IntPtr OSChWnd = IntPtr.Zero;    // adresse de la fenêtre de l'application OSC
        public bool getHwnd = false;
        public bool OSCTOP = false;

        // pour gestion mise en top de l'application UI
        public IntPtr UIhWnd = IntPtr.Zero;    // adresse de la fenêtre de l'application UI
        public bool UITOP = true;


        // declaration xtouch
        public struct XTouch
        {
            public string type;
            public int channel;
        }

        // déclaration snapshots
        public struct SnapShot
        {
            public int id;
            public string name;
            public bool actif;
            public int type;
            public string UIName; // mise à jour par la recherche sur l'UI
            public string comment;
            public List<SnapDashBank> DashBank;
            public List<Page> Pages;
        }

        public struct Page
        {
            public int id;
            public string name;
            public List<Channel> channels;
        }

        public struct Channel
        {
            public int id;
            public string type;
            public string dasident;
            public string dasuid;
            public string dspName;
            public int color;
            public shortcut dasShortcut;
        }

        public struct SnapDashBank
        {
            public string dasuid;
            public string name;
            public int nbscene;
            public List<shortcut> shtcut;
            public List<SnapDashScene> scenes;

        }

        public struct shortcut
        {
            public int type;
            public string eventData;
            public int ActionType;
            public string TargetIndex;
        }


        public struct SnapDashScene
        {
            public string dasuid; // si -1 c'est une scene virtuel de declenchement d'un bouton (presentation ...)
            public string name;
            public string jump_target;
            public string color;
            public List<shortcut> shtcut;
            public string PLAY_DIVISION;
            public string SPEED;
            public string SIZE;
            public string FADE_IN;
            public string FADE_OUT;
        }

        public struct SnapResult
        {
            public int snapkey;
            public int bankid;
            public int sceneid;
            public int shortid;
        }

        // declaration des boutons
        public struct button
        {
            public int index;
            public string name;
            public string bank;
            public string scene;
            public string dasuid;
            public string color;
            public int statut;
            public bool flash;
            public List<shortcut> shtcut;
            public string Btname;
            public string uiname;
        }

        public struct fader
        {
            public int index;
            public string dasident;
            public shortcut shtcut;
            public float value;
        }
        public static Cparams Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Cparams();
                    }
                    return instance;
                }
            }
        }

        
        public Cparams()
        {
            
        }

        #region Retourne ID suivant le type et le channel
        public string GetOutputByType(string type, int channel)
        {
            foreach (KeyValuePair<string, XTouch> xt in XtOutput)
            {
                if (xt.Value.type == type && xt.Value.channel == channel)
                {
                    return xt.Key;
                }


            }
            return null;

        }
        #endregion


        #region Retourne DASIDENT par rapport au shortcut
        public string GetDasIdentBySc(string dasIdent)
        {
            foreach (KeyValuePair<string, fader> fad in Faders)
            {
                if (fad.Value.shtcut.eventData == dasIdent)
                {
                    return fad.Key;

                }


            }
            return null;

        }
        #endregion

        #region retour un tableau avec la partie string et la partie num d'un string
        public string[] extractNumChar(string value)
        {
            char[] chars = value.ToCharArray();
            string[] R = { "", ""};

            for (int i = chars.Count() -1; i >=0; i--)
            {                
                if (Char.IsNumber(chars[i]))
                {
                    R[1]= chars[i] + R[1];
                }
                else
                {
                    break;
                }
            }

            R[0] = value.Substring(0, value.Length - R[1].Length );
            return R;
            
        }
        #endregion
    }
}
