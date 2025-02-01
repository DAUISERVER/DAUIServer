using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DAUIServer.Cparams;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Timers;

namespace DAUIServer
{
    public sealed class CUi
    {
        private static CUi instance = null;
        private static readonly object padlock = new object();
        private static System.Windows.Forms.WebBrowser UI2;
        private Cparams cPar = Cparams.Instance;
        private cLogger cLog = cLogger.Instance;
        public HtmlDocument Document = null;
        public Csnapshot cSnps = new Csnapshot();
        public bool Connected = false;

        public static CUi Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new CUi();
                    }
                    return instance;
                }
            }
        }
        public CUi()
        {
            try
            {

            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }

        #region déclaration de l'interface
        public void setWebBrowser(System.Windows.Forms.WebBrowser UI)
        {
            try
            {
                UI2 = UI;
                Document = UI2.Document;


            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

        }
        #endregion

        #region ajoute les codes JS à la page
        public void AddJs()
        {
            try
            {

                HtmlElement head = Document.GetElementsByTagName("head")[0];

                // positionne la page de l'interface suivant le mode et le canal
                HtmlElement s1 = Document.CreateElement("script");
                s1.SetAttribute("text", "function FcMLC(mode,px) { setMasterMode(mode); bottomWidget.gotoIDmid(px); }");
                head.AppendChild(s1);

                // retourne la liste des snap
                HtmlElement s2 = Document.CreateElement("script");
                s2.SetAttribute("text", "function FcGetSL() { return CURRENT_SNAPS_LIST.toString(); }");
                head.AppendChild(s2);

                // retourne le nombre d'éléments, permet de savoir si l'interface est correctement active
                HtmlElement s3 = Document.CreateElement("script");
                s3.SetAttribute("text", "function FcGetNbElem() { if (typeof allStrips !== 'undefined')  return allStrips.length; else return null; }");
                head.AppendChild(s3);

                // retourne le groupe a
                HtmlElement s31 = Document.CreateElement("script");
                s31.SetAttribute("text", "function FcGetVg(a) { return getVG(a).toString(); }");
                head.AppendChild(s31);

                // retourne les infos d'un channel
                HtmlElement s33 = Document.CreateElement("script");
                s33.SetAttribute("text", "function FcGetChannel(a) {var r = ''; " +
                    " for (var b in dataValue) {" +
                    " if ( b.substring(0,a.length) == a){ r = r + b + '='+dataValue[b] + '~';}" +
                    " }  " +
                    " return r;" +
                    "}");
                head.AppendChild(s33);

                // retourne le non d'un canal (i.0.)
                HtmlElement s32 = Document.CreateElement("script");
                s32.SetAttribute("text", "function FcGetNameCan(a) { return allStrips[a].name.toString(); }");
                head.AppendChild(s32);

                // mise à jour du snap
                HtmlElement s4 = Document.CreateElement("script");
                s4.SetAttribute("text", "function SM_SNAP(snap) {  a = getValue(\"var.currentShow\"); " +
                    "sendMessage(E_COMMANDS.SNAP_LOAD + \"^\" + a + \"^\" + snap);}");
                head.AppendChild(s4);

                // sauve le snap courant
                HtmlElement s41 = Document.CreateElement("script");
                s41.SetAttribute("text", "function update_SNAP() { a = getValue(\"var.currentShow\");" +
                    "b = getValue(\"var.currentSnapshot\"); " +
                    "sendMessage(E_COMMANDS.SNAP_SAVE + \"^\" + a + \"^\" + b);" +
                    "showPopupMsg(lang.UPDATE_CURRENT_SNAPSHOT,800); }");
                head.AppendChild(s41);


                // changement de mode dans l'onglet edit
                HtmlElement s5 = Document.CreateElement("script");
                s5.SetAttribute("text", "function setEditMode(mode,ch) { " +
                     " " +
                      "switch (mode) { " +
                     "case 0 : setMode(E_MODE.EDIT); if(ch == -1){ editWidget.setMode(editWidget.E_MODE.eqm); }else{ editWidget.setMode(editWidget.E_MODE.eqch);}  break; " +
                     "case 1 : setMode(E_MODE.EDIT); editWidget.setMode(editWidget.E_MODE.dyn); break; " +
                     "case 2 : setMode(E_MODE.EDIT); editWidget.setMode(editWidget.E_MODE.fx); break; " +
                     "case 3 : setMode(E_MODE.EDIT); editWidget.setMode(editWidget.E_MODE.aux); break; " +
                     "}} ");
                head.AppendChild(s5);

                // affiche la view groupe 0 - 4 ou mix si -1
                HtmlElement s6 = Document.CreateElement("script");
                s6.SetAttribute("text", "function FcDspSg(a) { if(a == -1) { setViewGroup(E_VG.VCAS); }else{ setViewGroup(a); }}");
                head.AppendChild(s6);

                // tous les solos à off si option activée
                HtmlElement s7 = Document.CreateElement("script");
                s7.SetAttribute("text", "function FcRstSolo() {if (0 == getValue(\"settings.multiplesolo\")) for (var b = 0; b < allStrips.length; b++) allStrips[b].type != E_STRIP_TYPE.VCA && allStrips[b].setNameValue(\"solo\", 0);   }");
                head.AppendChild(s7);

                // changement de master mode 
                HtmlElement s8 = Document.CreateElement("script");
                s8.SetAttribute("text", "function setMasterMode(mode) { " +
                     " " +
                      "switch (mode) { " +
                     "case 0 : setMode(E_MODE.MIX); break; " +
                     "case 1 : setMode(E_MODE.AUX); auxWidget.setMode(0); break; " +
                     "case 2 : setMode(E_MODE.AUX); auxWidget.setMode(1); break; " +
                     "case 3 : setMode(E_MODE.AUX); auxWidget.setMode(2); break; " +
                     "case 4 : setMode(E_MODE.AUX); auxWidget.setMode(3); break; " +
                     "case 5 : setMode(E_MODE.AUX); auxWidget.setMode(4); break; " +
                     "case 6 : setMode(E_MODE.AUX); auxWidget.setMode(5); break; " +
                     "case 7 : setMode(E_MODE.FXSENDS);  fxSendsWidget.setMode(0); break; " +
                     "case 8 : setMode(E_MODE.FXSENDS);  fxSendsWidget.setMode(1); break; " +
                     "case 9 : setMode(E_MODE.FXSENDS);  fxSendsWidget.setMode(2); break; " +
                     "case 10 : setMode(E_MODE.FXSENDS);  fxSendsWidget.setMode(3); break; " +
                     "}} ");
                head.AppendChild(s8);


                // retourne le niveau vu master
                HtmlElement s9 = Document.CreateElement("script");
                s9.SetAttribute("text", "function FcGetVu(can) { " +
                    "var ArrCan = can.split(','); " +
                    "var Sep=''; " +
                    "var R = ''; " +
                    "for(var i=0; i<ArrCan.length;i++){" +
                    "   R = R + Sep + allStrips[ArrCan[i]].vu.value; " +
                    "   Sep = ','; " +
                    "} " +
                    "R = R + Sep +  masterWidget.vu.value;" +
                    "return R;}");
                head.AppendChild(s9);


                //// Bloque les touches clavier
                //HtmlElement s10 = Document.CreateElement("script");
                //s10.SetAttribute("text", "document.body.onkeydown = function(event){alert('ok'); document.onkeydown = null; return false; }");
                //head.AppendChild(s10);


                // retourne le zoom level
                HtmlElement s11 = Document.CreateElement("script");
                s11.SetAttribute("text", "function GetZoomLevel() { " +
                    "return screen.deviceXDPI / screen.logicalXDPI;}");
                head.AppendChild(s11);

            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region  met à jour la liste des snaps
        public void getSnaps()
        {
            try
            {

                var ret = Document.InvokeScript("FcGetSL");
                if (ret == null || ret.ToString() == "")
                    return;
                List<string> Snaps = ret.ToString().Split(',').ToList();
                for (int n = 0; n < Snaps.Count; n++)
                {
                    string[] c = Snaps[n].Split('_');
                    string uis = c[c.Count() - 1];

                    // recherche si présent dans la liste des snaps
                    if (uis != "")
                    {
                        int index = cSnps.getSnapIndex(uis);
                        if (index != -1)
                        {
                            SnapShot Snap = cPar.Snapshots[index];
                            Snap.UIName = Snaps[n];
                            cPar.Snapshots[index] = Snap;


                        }

                    }
                }

                var _param = new object[] { "var.currentSnapshot" };
                string cs = Document.InvokeScript("getValue", _param).ToString();
                snapUpsdate(cPar.Snapshots[int.Parse(cPar.tagCurrentSnap)].UIName);

            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region selectionne une piste UI
        public void selectUI(string _sel)
        {
            try
            {
                if (_sel == "")
                    return;
                int sel = int.Parse(_sel);
                var parameters = new object[] { sel, 0, 0 };
                Document.InvokeScript("setSelected", parameters);
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region  met à jour le snap
        public void snapUpsdate(string snp)
        {
            try
            {

                var parameters = new object[] { snp };
                Document.InvokeScript("SM_SNAP", parameters);
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region  enregistre le snap
        public void snapUpdate()
        {
            try
            {

                var parameters = new object[] {};
                Document.InvokeScript("update_SNAP", parameters);
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion



        #region lit une valeur de channel
        public string getChannel(string Channel)
        {
            try
            {

                var _param = new object[] { Channel };
                var resultJson = Document.InvokeScript("FcGetChannel", _param);

                if (resultJson != null)
                {
                    return resultJson.ToString();
                }
                else
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
                return "";
            }
        }
        #endregion


        #region lit une valeur de UI
        public string getvalue(string _cat, string _scat, string _fct)
        {
            try
            {

                var _param = new object[] { _cat + "." + (_scat != "" ? _scat + "." : "") + _fct };
                var resultJson = Document.InvokeScript("getValue", _param);

                if (resultJson != null)
                {
                    return resultJson.ToString();
                }
                else
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
                return "";
            }
        }
        #endregion

        #region retourne le nom du canal c
        public string getNameChannel(int ChaNum)
        {
            var parameters = new object[] { ChaNum };
            string ident = Document.InvokeScript("FcGetNameCan", parameters).ToString();
            return "";

        }
        #endregion

        #region met à jour une valeur
        public void setvalue(string _cat, string _scat, string _fct, float value)
        {
            try
            {
                if (Connected)
                {
                    var parameters = new object[] { _cat + "." + (_scat != "" ? _scat + "." : "") + _fct, value };
                    Document.InvokeScript("setValue", parameters);
                    // raffraichie la page
                    parameters = new object[] { 0 };
                    Document.InvokeScript("setMasterMode", parameters);
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

        #region  met à jour le mode de EDIT   case 0:EDIT-eqch  1:EDIT-dyn 2:EDIT-fx  3:EDIT-aux 4:MIX 5:GAIN 6:AUX 7:FSX
        public void setEditMode(int sel, int DspEditMode)
        {
            try
            {
                var parameters = new object[] { sel, 0, 0 };
                Document.InvokeScript("setSelected", parameters);
                parameters = new object[] { DspEditMode, sel };
                Document.InvokeScript("setEditMode", parameters);

            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion


        #region  met à jour le mode mix
        public void setMixMode(int sel)
        {
            try
            {
                var parameters = new object[] { sel, 0, 0 };
                Document.InvokeScript("setSelected", parameters);
                parameters = new object[] { sel };
                Document.InvokeScript("setMasterMode", parameters);
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }
        }
        #endregion

    }
}
