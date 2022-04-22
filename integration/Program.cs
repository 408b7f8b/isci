using System;
using System.Net.Sockets;
using System.Net;

namespace integration
{
    class Program
    {
        static System.Net.Sockets.Socket udpSock;
        static byte[] buffer = new byte[1024];
        static EndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
        static void udpStart(){
            udpSock = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSock.Bind(new System.Net.IPEndPoint(IPAddress.Any, 1337));
            udpSock.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endpoint, udpCallback, udpSock);
        }

        static void udpCallback(IAsyncResult asyncResult){
            try
            {
                Socket recvSock = (Socket)asyncResult.AsyncState;
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);
                int length = recvSock.EndReceiveFrom(asyncResult, ref client);
                var conv_string = System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, length);
                conv_string = conv_string.TrimEnd();
                if (conv_string.StartsWith("val2#"))
                {
                    var val_string = conv_string.Substring(5);
                    val2.WertAusString(val_string);
                    val2_flag = true;
                } else if (conv_string.StartsWith("val#"))
                {
                    var val_string = conv_string.Substring(4);
                    val.WertAusString(val_string);
                    val_flag = true;
                }
        
                EndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
                udpSock.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endpoint, udpCallback, udpSock);
            } catch {}
        }

        static library.var_int state = new library.var_int(0, "/media/ramdisk/state");
        static library.var_int cycletime = new library.var_int(0, "/media/ramdisk/cycletime");
        static library.var_int val = new library.var_int(0, "/media/ramdisk/process/val");
        static bool val_flag = false;
        static library.var_int val2 = new library.var_int(0, "/media/ramdisk/process/val2");
        static bool val2_flag = false;

        static void Main(string[] args)
        {
            udpStart();
            long curr_ticks = 0;
            
            while(true)
            {
                state.WertLesen();

                if (state == 1)
                {
                    continue;
                }

                if (state == 0)
                {
                    curr_ticks = System.DateTime.Now.Ticks;

                    if (val_flag)
                    {
                        val.WertSchreiben();
                        val_flag = false;
                    }
                    if (val2_flag)
                    {
                        val2.WertSchreiben();
                        val2_flag = false;
                    }
                    
                    ++state.value;
                    state.WertSchreiben();
                    continue;
                }

                if (state == 2)
                {
                    val.WertLesen();
                    if (val.aenderung)
                    {
                        var snd = System.Text.UTF8Encoding.UTF8.GetBytes("val#" + val.WertSerialisieren());
                        udpSock.SendTo(snd, new System.Net.IPEndPoint(IPAddress.Any, 1337));
                        val.aenderung = false;
                    }
                    val2.WertLesen();
                    if (val2.aenderung)
                    {
                        var snd = System.Text.UTF8Encoding.UTF8.GetBytes("val2#" + val2.WertSerialisieren());
                        udpSock.SendTo(snd, new System.Net.IPEndPoint(IPAddress.Any, 1337));
                        val2.aenderung = false;
                    }

                    var curr_ticks_new = System.DateTime.Now.Ticks;
                    var ticks_span = curr_ticks_new - curr_ticks;
                    cycletime.value = (int)ticks_span;
                    cycletime.WertSchreiben();

                    state.value = 0;
                    state.WertSchreiben();
                }
            }
        }
    }
}
