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
                if (conv_string.StartsWith("val"))
                {
                    var string_parts = conv_string.Split('#');
                    while(change_lock) {}
                    if (change.ContainsKey(string_parts[0]))
                    {
                        change[string_parts[0]] = string_parts[1];
                    } else {
                        change.Add(string_parts[0], string_parts[1]);
                    }
                }
        
                EndPoint endpoint = target;//new IPEndPoint(target, 0);
                udpSock.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endpoint, udpCallback, udpSock);
            } catch {}
        }

        static library.var_int state = new library.var_int(0, "/media/ramdisk/state");
        static library.var_int cycletime = new library.var_int(0, "/media/ramdisk/cycletime");
        /*static library.var_int val = new library.var_int(0, "/media/ramdisk/process/val");
        static bool val_flag = false;
        static library.var_int val2 = new library.var_int(0, "/media/ramdisk/process/val2");
        static bool val2_flag = false;*/
        static System.Collections.Generic.Dictionary<string, library.var_int> vals = new System.Collections.Generic.Dictionary<string, library.var_int>();
        static System.Collections.Generic.Dictionary<string, string> change = new System.Collections.Generic.Dictionary<string, string>();
        static int state_work, state_target, state_ending;
        static int variables;
        static System.Net.IPEndPoint target;
        static bool change_lock;

        static void Main(string[] args)
        {
            state_work = Int32.Parse(args[0]);
            state_target = Int32.Parse(args[1]);
            state_ending = Int32.Parse(args[2]);
            variables = Int32.Parse(args[3]);
            target = new System.Net.IPEndPoint(IPAddress.Parse(args[4]), 1337);

            for(int i = 0; i < variables; ++i)
            {
                vals.Add("val" + i.ToString(), new library.var_int(0, "/media/ramdisk/process/val" + i.ToString()));
                vals.Add("val_return" + i.ToString(), new library.var_int(0, "/media/ramdisk/process/val_return" + i.ToString()));
            }

            udpStart();
            long curr_ticks = 0;
            
            while(true)
            {
                state.WertLesen();

                if (state == state_work)
                {
                    curr_ticks = System.DateTime.Now.Ticks;

                    /*if (val_flag)
                    {
                        val.WertSchreiben();
                        val_flag = false;
                    }
                    if (val2_flag)
                    {
                        val2.WertSchreiben();
                        val2_flag = false;
                    }*/

                    change_lock = true;
                    foreach (var entry in change)
                    {
                        vals[entry.Key].WertAusString(entry.Value);
                        vals[entry.Key].WertSchreiben();
                    }
                    change.Clear();
                    change_lock = false;
                    
                    state.value = state_target;
                    state.WertSchreiben();
                    continue;
                }

                if (state == state_ending)
                {
                    foreach (var entry in vals)
                    {
                        entry.Value.WertLesen();
                        if (entry.Value.aenderung)
                        {
                            var snd = System.Text.UTF8Encoding.UTF8.GetBytes(entry.Key + "#" + entry.Value.WertSerialisieren());
                            udpSock.BeginSendTo(snd, 0, snd.Length, 0, target, null, null);//   (snd, new System.Net.IPEndPoint(IPAddress.Any, 1337));
                            entry.Value.aenderung = false;
                        }
                    }
                    /*val.WertLesen();
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
                    }*/

                    var curr_ticks_new = System.DateTime.Now.Ticks;
                    var ticks_span = curr_ticks_new - curr_ticks;
                    cycletime.value = (int)ticks_span;
                    cycletime.WertSchreiben();

                    state.value = state_work;
                    state.WertSchreiben();

                    //System.Threading.Thread.Sleep(1);
                }
            }
        }
    }
}
