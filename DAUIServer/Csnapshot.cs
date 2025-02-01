using DAUIServer.Properties;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using static DAUIServer.Cparams;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace DAUIServer
{
    public class Csnapshot
    {
        public Cparams cPar = Cparams.Instance;
        public cLogger cLog = cLogger.Instance;

        #region lecture fichier daslight extraction bank scene shortcut
        public void ReadDas()
        {
            XmlDocument xdoc = new XmlDocument();
            try
            {
                string ConfFile = cPar.Params["dasfile"];
                cLog.Info("try to open das file :  " + ConfFile);
                if (!CheckFile(ConfFile))
                {
                    return;
                }

                xdoc.Load(ConfFile);
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }


            // parcour les BANK
            XmlNodeList banksChildNode = xdoc.GetElementsByTagName("BANK");
            XmlNodeList scenesChildNode = null;
            foreach (XmlNode bankChildNode in banksChildNode)
            {
                try
                {
                    if (bankChildNode.Attributes["NAME"] != null)
                    {
                        string name = bankChildNode.Attributes["NAME"].Value;

                        int index = getSnapIndex(name);
                        if (index != -1)
                        {
                            // création de la bank

                            SnapDashBank bank = new SnapDashBank();
                            bank.scenes = new List<SnapDashScene>();
                            bank.shtcut = new List<shortcut>();
                            bank.name = name;
                            if (bankChildNode.Attributes["DASUID"] != null) bank.dasuid = bankChildNode.Attributes["DASUID"].Value.ToString();


                            // parcoure les SCENES

                            scenesChildNode = bankChildNode.ChildNodes;
                            foreach (XmlNode sceneChildNode in scenesChildNode)
                            {
                                try
                                {
                                    if (sceneChildNode.Name.ToLower() == "scene")
                                    {
                                        int virtualButton = -1;

                                        SnapDashScene snapDashScene = new SnapDashScene();
                                        snapDashScene.shtcut = new List<shortcut>();
                                        if (sceneChildNode.Attributes["NAME"] != null) snapDashScene.name = sceneChildNode.Attributes["NAME"].Value.ToString();
                                        if (sceneChildNode.Attributes["DASUID"] != null) snapDashScene.dasuid = sceneChildNode.Attributes["DASUID"].Value.ToString();
                                        if (sceneChildNode.Attributes["COLOR"] != null) snapDashScene.color = sceneChildNode.Attributes["COLOR"].Value.ToString();
                                        if (sceneChildNode.Attributes["PLAY_DIVISION"] != null) snapDashScene.PLAY_DIVISION = sceneChildNode.Attributes["PLAY_DIVISION"].Value.ToString();
                                        if (sceneChildNode.Attributes["SPEED"] != null) snapDashScene.SPEED = sceneChildNode.Attributes["SPEED"].Value.ToString();
                                        if (sceneChildNode.Attributes["SIZE"] != null) snapDashScene.SIZE = sceneChildNode.Attributes["SIZE"].Value.ToString();
                                        if (sceneChildNode.Attributes["FADE_IN"] != null) snapDashScene.FADE_IN = sceneChildNode.Attributes["FADE_IN"].Value.ToString();
                                        if (sceneChildNode.Attributes["FADE_OUT"] != null) snapDashScene.FADE_OUT = sceneChildNode.Attributes["FADE_OUT"].Value.ToString();

                                        // si color correspond à une couleur bouton on ajoute le nom du bouton = scene avec declenchement d'une option bouton.
                                        for (int i = 1; i < cPar.Buttons.Count; i++)
                                        {
                                            if (cPar.Buttons[i].color != null && cPar.Buttons[i].color == snapDashScene.color)
                                            {
                                                virtualButton = i;                                                
                                            }

                                        }
                                        XmlNodeList ssChildNode;
                                        ssChildNode = sceneChildNode.ChildNodes;
                                        foreach (XmlNode sChildNode in ssChildNode)
                                        {
                                            try
                                            {
                                                if (sChildNode.Name.ToLower() == "jump_target")
                                                {
                                                    if (sChildNode.Attributes["DASUID"] != null) snapDashScene.jump_target = sChildNode.Attributes["DASUID"].Value.ToString();
                                                    break;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                cLog.Error(ex.ToString());
                                            }

                                        }
                                        bank.scenes.Add(snapDashScene);

                                        // ajoute une scene virtuelle
                                        if(virtualButton != -1)
                                        {

                                            // ajoute une scene
                                            SnapDashScene newscene = new SnapDashScene();
                                            button Addbutton = cPar.Buttons[virtualButton];
                                            newscene.color = Addbutton.color;
                                            newscene.name = Addbutton.scene;
                                            newscene.jump_target = Addbutton.index.ToString();
                                            newscene.dasuid = "-1";
                                            bank.scenes.Add(newscene);
                                           virtualButton = -1;
                                        }



                                    }

                                }
                                catch (Exception ex)
                                {
                                    cLog.Error(ex.ToString());
                                }
                            }
                            cPar.Snapshots[index].DashBank.Add(bank);
                        }


                        // recherche si bouton
                        scenesChildNode = bankChildNode.ChildNodes;
                        foreach (XmlNode sceneChildNode in scenesChildNode)
                        {
                            try
                            {
                                if (sceneChildNode.Attributes["NAME"] != null)
                                {
                                    int buttkey = getButtonIndex(name, sceneChildNode.Attributes["NAME"].Value.ToString());
                                    if (buttkey != -1)
                                    {
                                        if (sceneChildNode.Attributes["DASUID"] != null)
                                        {
                                            button Bt = cPar.Buttons[buttkey];
                                            Bt.dasuid = sceneChildNode.Attributes["DASUID"].Value.ToString();
                                            cPar.Buttons[buttkey] = Bt;
                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                cLog.Error(ex.ToString());
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    cLog.Error(ex.ToString());
                }

            }

            // parcour les groupes
            XmlNodeList groupsChildNode;
            groupsChildNode = xdoc.GetElementsByTagName("FIXTUREGROUP");
            foreach (XmlNode groups in groupsChildNode)
            {
                try
                {
                    string grpName = "";
                    string dasuid = "";
                    if (groups.Attributes["NAME"] != null) grpName = groups.Attributes["NAME"].Value.ToString().ToLower();
                    if (groups.Attributes["DASUID"] != null) dasuid = groups.Attributes["DASUID"].Value.ToString();

                    if (grpName != "")
                    {
                        foreach (KeyValuePair<int, SnapShot> snap in cPar.Snapshots)
                        {
                            SnapShot sn = snap.Value;
                            foreach (Cparams.Page Pa in sn.Pages)
                            {
                                for(int n = 0; n< Pa.channels.Count; n++) {
                                    Channel Ch = Pa.channels[n];
                                    if (Ch.type.ToLower() == "das" && Ch.dasident.ToLower() == grpName && dasuid != "")
                                    {
                                        Ch.dasuid = dasuid;
                                        Pa.channels[n] = Ch;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    cLog.Error(ex.ToString());
                }

            }

            // parcour les SHORTCUT
            XmlNodeList shortsChildNode;
            shortsChildNode = xdoc.GetElementsByTagName("SHORTCUT");
            foreach (XmlNode shortChildNode in shortsChildNode)
            {
                try
                {
                    if (shortChildNode.Attributes["TYPE"] != null && shortChildNode.Attributes["TYPE"].Value == "3")
                    {
                        XmlNodeList shsChildNode = shortChildNode.ChildNodes;
                        string target = "";
                        shortcut s = new shortcut();
                        foreach (XmlNode sChildNode in shsChildNode)
                        {
                            try
                            {
                                s.type = 3;    
                                if (sChildNode.Name.ToLower() == "event" && sChildNode.Attributes["DATA"] != null) s.eventData = sChildNode.Attributes["DATA"].Value;
                                if (sChildNode.Name.ToLower() == "action" && sChildNode.Attributes["TYPE"] != null) s.ActionType = int.Parse(sChildNode.Attributes["TYPE"].Value);
                                if (sChildNode.Name.ToLower() == "action" && sChildNode.Attributes["TARGET"] != null) target = sChildNode.Attributes["TARGET"].Value;
                                if (sChildNode.Name.ToLower() == "action" && sChildNode.Attributes["TARGETINDEX"] != null) s.TargetIndex = sChildNode.Attributes["TARGETINDEX"].Value;

                            }
                            catch (Exception ex)
                            {
                                cLog.Error(ex.ToString());
                            }
                        }


                        if (s.eventData != null)
                        {
                            string[] evd = null;
                            evd = s.eventData.Split(':');
                            s.eventData = evd[0];

                            // renseigne la liste des Shortcut en cas de génération d'un nouveau shortcut pour eviter les doublons
                            string e = evd[0].Replace("/", "");
                           
                          

                            string[] R = cPar.extractNumChar(e);
                            if (R[0] != "" && R[1] != "")
                            {

                                int cnt = int.Parse(R[1]);
                                if (!cPar.Shortcutnumber.Keys.Contains(R[0]))
                                {
                                    cPar.Shortcutnumber.Add(R[0], cnt);
                                }
                                else
                                {
                                    if (cnt > cPar.Shortcutnumber[R[0]])
                                    {
                                        cPar.Shortcutnumber[R[0]] = cnt;
                                    }
                                }
                            }


                            
                        }

                        if (target != "" && s.eventData != null)
                        {
                            updateShortcut(target, s);
                           // if (!cPar.Shortcutname.Exists(delegate (string x) { return x == s.eventData; })) cPar.Shortcutname.Add(s.eventData);
                            if (cPar.maxTargetIndex < int.Parse(s.TargetIndex)) { cPar.maxTargetIndex = int.Parse(s.TargetIndex); }
                        }
                    }

                }
                catch (Exception ex)
                {
                    cLog.Error(ex.ToString());
                }
            }
        }
        #endregion  

        #region retourne un l'index correspondant au snapshot name ou -1 si inconnue
        public int getSnapIndex(string name)
        {
            try
            {
                int index = -1;
                foreach (KeyValuePair<int, SnapShot> snap in cPar.Snapshots)
                {

                    if (snap.Value.name.ToLower().Contains(name.ToLower()))
                    {
                        return snap.Key;   
                    }


                    if (snap.Value.name.ToLower().Equals(name.ToLower()))
                    {
                        return snap.Key;
                    }
                }
                return index;
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
                return -1;
            }
        }
        #endregion

        #region retourne un l'index correspondant au bouton name/scene ou -1 si inconnue
        private int getButtonIndex(string bank, string scene)
        {
            try
            {
                int index = -1;
                foreach (KeyValuePair<int, button> but in cPar.Buttons)
                {
                    if ((but.Value.bank.ToLower().Equals(bank.ToLower())) && (but.Value.scene.ToLower().Equals(scene.ToLower())))
                    {
                        return but.Key;
                    }
                }
                return index;
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
                return -1;
            }
        }
        #endregion

        #region mise à jour d'un shortcut
        private void updateShortcut(string target, shortcut sch)
        {
            SnapResult Sr = new SnapResult();
            try
            {               
                Sr.bankid = -1;
                // parcour les shots
                foreach (KeyValuePair<int, SnapShot> snap in cPar.Snapshots)
                {
                    int idBank = 0;
                    // parcour les banks
                    foreach (SnapDashBank bank in snap.Value.DashBank)
                    {
                        if (bank.dasuid == target)
                        {
                            bank.shtcut.Add(sch);
                            Sr.snapkey = snap.Key;
                            Sr.bankid = idBank;
                            Sr.sceneid = -1;
                            Sr.shortid = bank.shtcut.Count;
                            cPar.SnapResults.Add(sch.eventData, Sr);
                            return;
                        }
                        int idScene = 0;
                        // parcour les scenes
                        foreach (SnapDashScene scene in bank.scenes)
                        {
                            if (scene.dasuid == target)
                            {
                                scene.shtcut.Add(sch);
                                Sr.snapkey = snap.Key;
                                Sr.bankid = idBank;
                                Sr.sceneid = idScene;
                                Sr.shortid = bank.shtcut.Count;
                                cPar.SnapResults.Add(sch.eventData, Sr);
                                return;
                            }
                            idScene++;
                        }
                        idBank++;
                    }
                    // parcour les pages et channels
                    foreach (Cparams.Page Pa in snap.Value.Pages)
                    {
                        for (int n = 0; n < Pa.channels.Count; n++)
                        {
                            Channel Ch = Pa.channels[n];
                            if (Ch.type.ToLower() == "das" && Ch.dasuid == target)
                            {
                                Ch.dasShortcut = sch;
                                Pa.channels[n] = Ch;
                                if(!cPar.Faders.ContainsKey(Ch.dasident))
                                {
                                    fader f = new fader();
                                    f.shtcut = sch;
                                    f.index = n;
                                    f.value = 1;
                                    cPar.Faders.Add(Ch.dasident, f);
                                }
                            }
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
                // parcour les boutons
                foreach (KeyValuePair<int, button> bt in cPar.Buttons)
                {
                    if (bt.Value.dasuid == target)
                    {
                        if (!bt.Value.shtcut.Contains(sch))
                        {
                            bt.Value.shtcut.Add(sch);
                            string[] ext = sch.eventData.Split(':');

                            cPar.ButtonResults.Add(ext[0], bt.Value.index);
                        }
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


        #region Ajoute la scene AddScene apres la Scene AfterScene
        public string AddVirtualScene(button Addbutton,String AfterScene)
        {
            // recherche l'emplacement de la scene 
            string result = "";
            XmlDocument xdoc = new XmlDocument();
            try
            {
                string dasfile = cPar.Params["dasfile"];
                cLog.Info("try to open das file :  " + dasfile);
                if (!CheckFile(dasfile))
                {
                    return ("Error in file");
                }
                xdoc.Load(dasfile);
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            // parcour les BANKS
            XmlNodeList banksChildNode = xdoc.GetElementsByTagName("BANK");
            XmlNodeList scenesChildNode = null;
            foreach (XmlNode bankChildNode in banksChildNode)
            {
                try
                {
                    if (bankChildNode.Attributes["NAME"] != null)
                    {
                        string name = bankChildNode.Attributes["NAME"].Value;
                        // parcoure les SCENES
                        scenesChildNode = bankChildNode.ChildNodes;

                        foreach (XmlNode sceneChildNode in scenesChildNode)
                        {
                            try
                            {
                                

                                // Positionne à la scene avant insertion
                                if (sceneChildNode.Attributes["DASUID"] != null && sceneChildNode.Attributes["DASUID"].InnerText.ToString() == AfterScene )
                                {
                                    // change la couleur de la scene par la couleur du bouton
                                    sceneChildNode.Attributes["COLOR"].InnerXml = Addbutton.color;
                                    xdoc.Save(cPar.Params["dasfile"]);

                                    // ajoute une scene
                                    SnapDashBank Bank = cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0];
                                    SnapDashScene newscene = new SnapDashScene();

                                    newscene.color = Addbutton.color;
                                    newscene.name = Addbutton.scene;
                                    newscene.jump_target = Addbutton.index.ToString();
                                    newscene.dasuid = "-1";
                                    Bank.scenes.Insert(int.Parse(cPar.tagCurrentScene) + 1, newscene);

                                    cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].DashBank[0] = Bank;
                                    return result;
                                }
                            }
                            catch (Exception ex)
                            {
                                cLog.Error(ex.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    cLog.Error(ex.ToString());
                }
            }
            return result;
        }
        #endregion

        #region Ajoute la scene AddScene apres la Scene AfterScene
        public string RemoveVirtualScene(SnapDashScene AfterScene)
        {
            // recherche l'emplacement de la scene 
            string result = "";
            XmlDocument xdoc = new XmlDocument();
            try
            {
                string dasfile = cPar.Params["dasfile"];
                cLog.Info("try to open das file :  " + dasfile);
                if (!CheckFile(dasfile))
                {
                    return ("Error in file");
                }
                xdoc.Load(dasfile);
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            // parcour les BANKS
            XmlNodeList banksChildNode = xdoc.GetElementsByTagName("BANK");
            XmlNodeList scenesChildNode = null;
            foreach (XmlNode bankChildNode in banksChildNode)
            {
                try
                {
                    if (bankChildNode.Attributes["NAME"] != null)
                    {
                        string name = bankChildNode.Attributes["NAME"].Value;
                        // parcoure les SCENES
                        scenesChildNode = bankChildNode.ChildNodes;

                        foreach (XmlNode sceneChildNode in scenesChildNode)
                        {
                            try
                            {


                                // Positionne à la scene avant insertion
                                if (sceneChildNode.Attributes["DASUID"] != null &&  sceneChildNode.Attributes["DASUID"].InnerText.ToString() == AfterScene.dasuid)
                                {
                                    // change la couleur de la scene par la couleur du bouton
                                    sceneChildNode.Attributes["COLOR"].InnerXml = cPar.Params["ColorStd"];
                                    xdoc.Save(cPar.Params["dasfile"]);
                                    return result;
                                }
                            }
                            catch (Exception ex)
                            {
                                cLog.Error(ex.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    cLog.Error(ex.ToString());
                }
            }
            return result;
        }
        #endregion

        #region traite l'absence de shrtcut dans le fichier Daslight et les ajoute si update = true.
        public string checkDasSnap(bool update)
        {
            string result = "";
            bool upd = false;
            XElement DLMFILE = null;
            XDocument Dashdoc = null;
            string DasFile = cPar.Params["dasfile"];

            if(!CheckFile(DasFile))
            {
                return("");
            }

            XmlDocument xdoc = new XmlDocument();
            try
            {
                Dashdoc = XDocument.Load(DasFile);
                DLMFILE = Dashdoc.Element("DLMFILE");
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }


            try
            {
                // parcour les SnapShot
                foreach (KeyValuePair<int, SnapShot> snap in cPar.Snapshots)
                {
                    if (snap.Value.DashBank.Count == 0 && snap.Value.actif == true && snap.Value.type == 0)
                    {
                        result += "Absence de banque pour le snap " + snap.Value.name + "\n\r";
                    }
                    else
                    {

                        // parcour les banks
                        int idbank = 0;
                        foreach (SnapDashBank bank in snap.Value.DashBank)
                        {
                            if (bank.scenes.Count == 0)
                            {
                                result += "Absence de scene pour la banque " + bank.name + "\n\r";
                            }
                            if (bank.shtcut.Count == 0)
                            {
                                if (!update)
                                {
                                    result += "Absence de racoucie pour la banque " + bank.name + "\n\r";
                                }
                                else
                                {
                                    cPar.maxTargetIndex++;
                                    IEnumerable<XElement> rows = DLMFILE.Descendants("SHORTCUT");
                                    XElement firstRow = rows.First();
                                    string dataevent = MakeEvent("B");
                                    firstRow.AddBeforeSelf(new XElement("SHORTCUT", new XAttribute("TYPE", "3"),
                                        new XElement("EVENT", new XAttribute("DATA", dataevent)),
                                        new XElement("ACTION", new XAttribute("TYPE", "33"), new XAttribute("TARGET", bank.dasuid), new XAttribute("TARGETINDEX", cPar.maxTargetIndex.ToString())),
                                        new XElement("SETTINGS", new XAttribute("SMODE", "0"), new XAttribute("CMODE", "0"), new XAttribute("TMODE", "0"), new XAttribute("MIN", "0"), new XAttribute("MAX", "1"), new XAttribute("INC", "0.001"), new XAttribute("LOOP", "0"), new XAttribute("FLASH", "0"), new XAttribute("INV", "0"))
                                        ));
                                    upd = true;

                                    shortcut sc = new shortcut();
                                    sc.ActionType = 33;
                                    sc.type = 3;
                                    sc.eventData = dataevent;
                                    sc.TargetIndex = cPar.maxTargetIndex.ToString();
                                    bank.shtcut.Add(sc);

                                    SnapResult Sr = new SnapResult();
                                    Sr.snapkey = snap.Key;
                                    Sr.bankid = idbank;
                                    Sr.sceneid = -1;
                                    Sr.shortid = bank.shtcut.Count;
                                    cPar.SnapResults.Add(sc.eventData, Sr);

                                    cLog.Info("Add shortcut for bank " + bank.name);
                                }
                            }

                            // parcour les scenes
                            int sceneid = 0;
                            foreach (SnapDashScene scene in bank.scenes)
                            {

                                if (scene.shtcut != null && scene.shtcut.Count == 0)
                                {
                                    if (!update)
                                    {
                                        result += "Absence de racoucie pour la scene " + scene.name + " banque " + bank.name + "\n\r";
                                    }
                                    else
                                    {
                                        cPar.maxTargetIndex++;
                                        IEnumerable<XElement> rows = DLMFILE.Descendants("SHORTCUT");
                                        XElement firstRow = rows.First();
                                        string dataevent = MakeEvent("S");
                                        firstRow.AddBeforeSelf(new XElement("SHORTCUT", new XAttribute("TYPE", "3"),
                                            new XElement("EVENT", new XAttribute("DATA", dataevent)),
                                            new XElement("ACTION", new XAttribute("TYPE", "107"), new XAttribute("TARGET", scene.dasuid), new XAttribute("TARGETINDEX", cPar.maxTargetIndex.ToString())),
                                            new XElement("SETTINGS", new XAttribute("SMODE", "0"), new XAttribute("CMODE", "0"), new XAttribute("TMODE", "0"), new XAttribute("MIN", "0"), new XAttribute("MAX", "1"), new XAttribute("INC", "0.001"), new XAttribute("LOOP", "0"), new XAttribute("FLASH", "0"), new XAttribute("INV", "0"))
                                            ));
                                        upd = true;
                                        cLog.Info("Add shortcut for scene " + scene.name + " in bank " + bank.name);

                                        shortcut sc = new shortcut();
                                        sc.ActionType = 107;
                                        sc.type = 3;
                                        sc.eventData = dataevent;
                                        sc.TargetIndex = cPar.maxTargetIndex.ToString();
                                        scene.shtcut.Add(sc);

                                        SnapResult Sr = new SnapResult();

                                        Sr.snapkey = snap.Key;
                                        Sr.bankid = idbank;
                                        Sr.sceneid = sceneid;
                                        Sr.shortid = bank.shtcut.Count;
                                        cPar.SnapResults.Add(sc.eventData, Sr);
                                    }
                                }
                                sceneid++;
                            }
                            idbank++;
                        }
                    }

                    // parcour les pages et channels
                    foreach (Cparams.Page Pa in snap.Value.Pages)
                    {
                        for (int n = 0; n < Pa.channels.Count; n++)
                        {
                            Channel Ch = Pa.channels[n];
                            if (Ch.type.ToLower() == "das" && Ch.dasident != "" && Ch.dasShortcut.eventData == null)
                            {
                                if (!update)
                                {
                                    result += "Absence de racoucie pour le groupe " + Ch.dasident + "\n\r";
                                }
                                else
                                {
                                    cPar.maxTargetIndex++;
                                    IEnumerable<XElement> rows = DLMFILE.Descendants("SHORTCUT");
                                    XElement firstRow = rows.First();
                                    string dataevent = MakeEvent("F");
                                    firstRow.AddBeforeSelf(new XElement("SHORTCUT", new XAttribute("TYPE", "3"),
                                        new XElement("EVENT", new XAttribute("DATA", dataevent)),
                                        new XElement("ACTION", new XAttribute("TYPE", "231"), new XAttribute("TARGET", Ch.dasuid), new XAttribute("TARGETINDEX", cPar.maxTargetIndex.ToString())),
                                        new XElement("SETTINGS", new XAttribute("SMODE", "1"), new XAttribute("CMODE", "1"), new XAttribute("TMODE", "0"), new XAttribute("MIN", "0"), new XAttribute("MAX", "1"), new XAttribute("INC", "0.001"), new XAttribute("LOOP", "0"), new XAttribute("FLASH", "0"), new XAttribute("INV", "0"))
                                        ));
                                    upd = true;                                   
                                    shortcut sc = new shortcut();
                                    sc.ActionType = 30;
                                    sc.type = 3;
                                    sc.eventData = dataevent;
                                    sc.TargetIndex = cPar.maxTargetIndex.ToString();
                                    // Ch.dasShortcut =sc;
                                    // ajoute ce shortcut à touts les snaps toutes les pages  et les channels de ce name
                                    foreach (KeyValuePair<int, SnapShot> Updsnap in cPar.Snapshots)
                                    {
                                          foreach (Cparams.Page UpdPa in Updsnap.Value.Pages)
                                        {
                                            for (int m = 0; m < UpdPa.channels.Count; m++)
                                                   {
                                                Channel UpdCh = Pa.channels[m];
                                                if (UpdCh.type.ToLower() == "das" && UpdCh.dasuid == Ch.dasuid)
                                                {
                                                    UpdCh.dasShortcut = sc;
                                                    UpdPa.channels[m] = UpdCh;
                                                    cLog.Info("Add shortcut for Channel " + Ch.dasident + " in Page " + UpdPa.id + " Snapshot " + Updsnap.Value.name + "");
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                // parcour les boutons
                for(int n = 1; n <= cPar.Buttons.Count; n++)
                {
                    button Bt = cPar.Buttons[n];
                    if (Bt.shtcut == null || Bt.shtcut.Count == 0)
                    {
                        if (!update)
                        {
                            result += "Absence de racoucie pour le bouton " + Bt.name  + "\n\r";
                        }
                        else
                        {
                            cPar.maxTargetIndex++;
                            IEnumerable<XElement> rows = DLMFILE.Descendants("SHORTCUT");
                            XElement firstRow = rows.First();
                            string dataevent = MakeEvent("T");
                            string flash = "0";
                            if (Bt.flash) { flash = "1"; }
                            firstRow.AddBeforeSelf(new XElement("SHORTCUT", new XAttribute("TYPE", "3"),
                                new XElement("EVENT", new XAttribute("DATA", dataevent)),
                                new XElement("ACTION", new XAttribute("TYPE", "107"), new XAttribute("TARGET", Bt.dasuid), new XAttribute("TARGETINDEX", cPar.maxTargetIndex.ToString())),
                                new XElement("SETTINGS", new XAttribute("SMODE", "0"), new XAttribute("CMODE", "0"), new XAttribute("TMODE", "0"), new XAttribute("MIN", "0"), new XAttribute("MAX", "1"), new XAttribute("INC", "0.001"), new XAttribute("LOOP", "0"), new XAttribute("FLASH", flash), new XAttribute("INV", "0"))
                                ));
                            upd = true;
                            cLog.Info("Add shortcut for button " + Bt.name );

                            shortcut sc = new shortcut();
                            sc.ActionType = 107;
                            sc.type = 3;
                            sc.eventData = dataevent;
                            sc.TargetIndex = cPar.maxTargetIndex.ToString();
                            Bt.shtcut.Add(sc);
                            
                            cPar.ButtonResults.Add(dataevent, n);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            if (upd)
            {
                try
                {
                    Dashdoc.Save(DasFile);
                    result += "Vous devez recharger le fichier de DasLight 5";
                }
                catch (Exception ex)
                {
                    cLog.Error(ex.ToString());
                }
            }
            return result;

        }
        #endregion

        #region création d'un nom de shortcut
        private string MakeEvent(string type)
        {
            if(!cPar.Shortcutnumber.Keys.Contains(type))
            {
                cPar.Shortcutnumber.Add(type, 0);
            }
            int blc = cPar.Shortcutnumber[type] + 1;
            cPar.Shortcutnumber[type] = blc;
            string evt = "/" + type + "" + blc.ToString();
            return evt;
        }
        #endregion

        

        #region check file
            private bool CheckFile(string file)
        {

            if(!System.IO.File.Exists(file))
            {
                MessageBox.Show("Le fichier "+file+" n'existe pas.","Lecture XML Dash ",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;

            }

            if(new System.IO.FileInfo(file).Attributes.HasFlag(System.IO.FileAttributes.ReadOnly) == true)
            {
                MessageBox.Show("Le fichier " + file + " est en lecture seule", "Lecture XML Dash ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;

            }
            return true;    
        }
        #endregion
    }
}
