using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAUIServer
{
    public class Cregister
    {

        cLogger cLog = cLogger.Instance;

        //Fonction qui écrit dans la base de registre windows
        public void WriteRegistreValue(string key, string value)
        {

            RegistryKey Nkey = Registry.CurrentUser;

            try
            {
                string appname = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                RegistryKey valKey = Nkey.OpenSubKey("Software\\" + appname + "\\" + key, true);

                if (valKey == null)
                {
                    valKey = Nkey.CreateSubKey("Software\\" + appname + "\\" + key);

                }

                valKey.SetValue("MyValue", value);

            }
            catch (Exception er)
            {
                cLog.Error("error 2100 " + er.ToString());
            }
            finally
            {
                Nkey.Close();
            }
        }

        //Fonction qui lit dans la base des registres windows
        public string ReadRegistreValue(string key, string defaut)
        {

            RegistryKey Nkey = Registry.CurrentUser;
            try
            {
                string appname = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                RegistryKey valKey = Nkey.OpenSubKey("Software\\" + appname + "\\" + key, true);

                if (valKey == null)
                {
                    WriteRegistreValue(key, defaut);
                    //valKey.Close();
                    return defaut;
                }
                else
                {
                    string r = (string)valKey.GetValue("MyValue");
                    valKey.Close();
                    return r;
                }
            }
            catch (Exception er)
            {
                cLog.Error("error 2000 " + er.ToString());
                return "0";
            }
            finally
            {

                Nkey.Close();
            }
        }
    }
}
