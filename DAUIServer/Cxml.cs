using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using static DAUIServer.Cparams;


namespace DAUIServer
{
    public class Cxml
    {
        public cLogger cLog = cLogger.Instance;
        public Cparams cPar = Cparams.Instance;

        public void LoadXMLConfig()
        {
            
            XmlNodeList paramsChildNode;
            XmlDocument xdoc = new XmlDocument();
            try
            {
                string ConfFile = Application.StartupPath + @"\configuration.xml";
                cLog.Info("try to open conf file :  " + ConfFile);
                xdoc.Load(ConfFile);
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }


            try
            {
                paramsChildNode = xdoc.GetElementsByTagName("param");
                foreach (XmlNode paramChildNode in paramsChildNode)
                {
                    if (paramChildNode.Attributes["name"] != null && paramChildNode.Attributes["value"] != null)
                    {
                        string name = paramChildNode.Attributes["name"].Value.ToString();
                        string value = paramChildNode.Attributes["value"].Value.ToString();
                        if (!cPar.Params.ContainsKey(name))
                        {
                            cPar.Params.Add(name, value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }           

        }

        public void Loadxtouchconfig()
        {
            XmlNodeList channelsChildNode;
            XmlDocument xdoc = new XmlDocument();
            try
            {
                string ConfFile = Application.StartupPath + @"\x-touch-extender.xml";
                cLog.Info("try to open conf file :  " + ConfFile);
                xdoc.Load(ConfFile);
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
                return;
            }
            try
            {
                channelsChildNode = xdoc.GetElementsByTagName("channel");
                foreach (XmlNode channelChildNode in channelsChildNode)
                {
                    int idchanel = -1;
                    if (channelChildNode.Attributes["id"] != null) idchanel = int.Parse(channelChildNode.Attributes["id"].Value.ToString());
                    // parcour les channels 
                    foreach (XmlNode inout in channelChildNode.ChildNodes)
                    {
                        XTouch xt = new XTouch();
                        string key = "";

                        if (inout.Name.ToLower() == "input" || inout.Name.ToLower() == "output")
                        {

                            if (inout.Attributes["id"] != null) key = inout.Attributes["id"].Value.ToString();
                            if (inout.Attributes["type"] != null) xt.type = inout.Attributes["type"].Value.ToString();
                            xt.channel = idchanel;

                            if (idchanel != -1 && key != "")
                            {
                                if (inout.Name.ToLower() == "input")
                                {
                                    if (!cPar.XtInput.ContainsKey(key))
                                    {
                                        cPar.XtInput.Add(key, xt);
                                    }
                                }
                                else
                                {
                                    if (!cPar.XtOutput.ContainsKey(key))
                                    {
                                        cPar.XtOutput.Add(key, xt);
                                    }
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




            public void LoadXMLSnap()
        {

            XmlNodeList paramsChildNode;
            XmlDocument xdoc = new XmlDocument();
            try
            {
                string ConfFile = Application.StartupPath + @"\configuration.xml";
                cLog.Info("try to open conf file :  " + ConfFile);
                xdoc.Load(ConfFile);
            }
            catch (Exception ex)
            {
                cLog.Error(ex.ToString());
            }

            try
            {
                paramsChildNode = xdoc.GetElementsByTagName("snap");
                foreach (XmlNode paramChildNode in paramsChildNode)
                {
                    SnapShot Sn = new SnapShot();
                    Sn.DashBank = new List<SnapDashBank>();
                    Sn.Pages = new List<Page>();                     

                    if (paramChildNode.Attributes["id"] != null) Sn.id = int.Parse(paramChildNode.Attributes["id"].Value.ToString());
                    if (paramChildNode.Attributes["name"] != null) Sn.name = paramChildNode.Attributes["name"].Value.ToString();
                    if (paramChildNode.Attributes["type"] != null) Sn.type = int.Parse(paramChildNode.Attributes["type"].Value.ToString());
                    if (paramChildNode.Attributes["actif"] != null) Sn.actif = bool.Parse(paramChildNode.Attributes["actif"].Value.ToString());
                    if (!cPar.Snapshots.ContainsKey(Sn.id))
                    {
                        cPar.Snapshots.Add(Sn.id, Sn);
                    }

                    // parcour les channels 
                    foreach(XmlNode channels in paramChildNode.ChildNodes)
                    {
                        if (channels.Name.ToLower() == "comment")
                        {                            
                            Sn.comment = channels.InnerText;
                            cPar.Snapshots[Sn.id] = Sn;
                        }

                            if (channels.Name.ToLower() == "channels")
                        {
                            Page Pg = new Page();
                            Pg.channels = new List<Channel>();
                            if (channels.Attributes["page"] != null) Pg.id = int.Parse(channels.Attributes["page"].Value.ToString());
                            if (channels.Attributes["name"] != null) Pg.name = channels.Attributes["name"].Value.ToString();
                            // parcour les channel 
                            foreach (XmlNode channel in channels.ChildNodes)
                            {                                
                                if (channel.Name.ToLower() == "channel")
                                {
                                    Channel CH = new Channel();
                                    if (channel.Attributes["id"] != null) CH.id = int.Parse(channel.Attributes["id"].Value.ToString());
                                    if (channel.Attributes["type"] != null) CH.type = channel.Attributes["type"].Value.ToString();
                                    if (channel.Attributes["dasident"] != null) CH.dasident = channel.Attributes["dasident"].Value.ToString();
                                    if (channel.Attributes["name"] != null) CH.dspName = channel.Attributes["name"].Value.ToString();
                                    if (channel.Attributes["color"] != null) CH.color = int.Parse(channel.Attributes["color"].Value.ToString());
                                    Pg.channels.Add(CH);
                                }
                            }
                            Sn.Pages.Add(Pg);
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
                paramsChildNode = xdoc.GetElementsByTagName("button");
                foreach (XmlNode paramChildNode in paramsChildNode)
                {
                    button Bt = new button();
                    Bt.name = "";
                    Bt.index = -1;
                    Bt.statut = 0;
                    Bt.color = null;
                    Bt.shtcut = new List<shortcut>();
                    if (paramChildNode.Attributes["id"] != null) Bt.index = int.Parse(paramChildNode.Attributes["id"].Value.ToString());
                    if (paramChildNode.Attributes["bank"] != null) Bt.bank = paramChildNode.Attributes["bank"].Value.ToString();
                    if (paramChildNode.Attributes["scene"] != null) Bt.scene = paramChildNode.Attributes["scene"].Value.ToString();
                    if (paramChildNode.Attributes["uiname"] != null) Bt.uiname = paramChildNode.Attributes["uiname"].Value.ToString();
                    if (paramChildNode.Attributes["flash"] != null) Bt.flash = bool.Parse(paramChildNode.Attributes["flash"].Value.ToString());
                    if (paramChildNode.Attributes["color"] != null) Bt.color = paramChildNode.Attributes["color"].Value.ToString();
              
                    if (Bt.index != -1 && !cPar.Buttons.ContainsKey(Bt.index))
                    {
                        cPar.Buttons.Add(Bt.index, Bt);
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
