using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DAUIServer
{
    public class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;
        public cLogger cLog = cLogger.Instance;
        public event EventHandler<OscEventReceiveArg> OscEventReceive;
        private Object lockThis = new Object();

        public class OscEventReceiveArg : EventArgs
        {
            public string key { get; set; }
            public string type { get; set; }
            public float value { get; set; }

        }


        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }

        public void Send(float value, string commande, string type = "i")
        {

            if (_socket.Connected == false)
            {
                cLog.Error("Try to send message but not connected...");
                return;
            }

            //Console.WriteLine("SEND: {0}: {1}, {2}", commande, type, value.ToString());
            List<byte> datas = new List<byte>();
            byte[] separateur = new byte[] { 0 };
            byte[] valBytes = null;

            datas.AddRange(Encoding.ASCII.GetBytes(commande));
            datas.AddRange(separateur);
            valBytes = BitConverter.GetBytes(value);
            if (type == "i")
            {
                int t = (int)value;
                valBytes = BitConverter.GetBytes(t);
            }

            type = "," + type;


            datas.AddRange(Encoding.ASCII.GetBytes(type));
            datas.AddRange(separateur);
            datas.AddRange(separateur);
            Array.Reverse(valBytes);
            datas.AddRange(valBytes);
            _socket.BeginSend(datas.ToArray(), 0, datas.ToArray().Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);

            }, state);
        }

        private void Receive()
        {

            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                try
                {
                    int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                    _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                }
                catch (Exception ex)
                {
                    cLog.Error(ex.ToString());
                    return;
                }
                string name = "";
                int posendname = 0;
                int posendtype = 0;
                string type = "";
                bool check = false;
                byte[] xValue = new byte[] { 0, 0, 0, 0 };
                // extract du nom
                for (int n = 0; n < so.buffer.Length; n++)
                {
                    if (so.buffer[n] != 0)
                    {
                        name = name + (char)so.buffer[n];
                    }
                    else
                    {
                        posendname = n;
                        break;
                    }
                }

                // recherche le type  (i 	int32 f float32 s OSC-string b OSC-blob)
                for (int n = posendname; n < posendname + 6; n++)
                {
                    if (check)
                    {
                        if (so.buffer[n] != 0)
                        {
                            type = type + (char)so.buffer[n];
                            posendtype = n;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (so.buffer[n] == 44) // virgule
                    {
                        check = true;
                    }
                }

                check = false;
                int nb = 3;
                // extraction de la valeur
                for (int n = posendtype + 3; n < posendtype + 7; n++)
                {
                    xValue[nb] = so.buffer[n];
                    nb--;
                }

                float value = 0;
                if (xValue.Length > 0 && type == "f")
                {
                    value = BitConverter.ToSingle(xValue, 0);
                }

                if (xValue.Length > 0 && type == "i")
                {
                    int v = BitConverter.ToInt32(xValue, 0);
                    value = (float)v;
                }

                OscEventReceiveArg args = new OscEventReceiveArg();
                args.key = name;
                args.type = type;
                args.value = value;
                OnOscEventReceive(args);

                //Console.WriteLine("RECV: {0}: {1}, {2}", name, type, value.ToString());

            }, state);

        }

        public virtual void OnOscEventReceive(OscEventReceiveArg e)
        {
            EventHandler<OscEventReceiveArg> handler = OscEventReceive;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
