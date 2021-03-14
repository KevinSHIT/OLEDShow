using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace OLEDShow
{
    class VThreadCollectionHelper
    {
        public static void AddAll(int setTimeSleep = 500, int setNetworkSleep = 1000, int setTxvLocationSleep = 1000)
        {
            AddSetTime(setTimeSleep);
            AddSetTxvLocation(setTxvLocationSleep);
            AddSetTxvNetwork(setNetworkSleep);
        }

        public static void StopAll()
        {
            foreach (var kv in Shared.VThreadsCollection)
            {
                kv.Value.Stop();
            }
        }

        public static void StartAll(bool abord = true)
        {
            foreach (var kv in Shared.VThreadsCollection)
            {
                kv.Value.Start(abord);
            }
        }


        public static void Add(string name, VThread vt)
        {
            if (Shared.VThreadsCollection.ContainsKey(name))
            {
                Shared.VThreadsCollection["name"].Stop();
                Shared.VThreadsCollection.Remove("name");
            }
            Shared.VThreadsCollection.Add(name, vt);
        }

        public static void AddSetTime(int sleep = 500)
        {
            Add("time", new VThread(() =>
            {
                while (true)
                {
                    Shared.InfoText.Time = DateTime.Now.ToString("hh:mm:ss");
                    Thread.Sleep(sleep);
                }
            }));
        }

        public static void AddSetTxvLocation(int sleep = 500)
        {
            Add("txvlocation", new VThread(() =>
            {
                while (true)
                {
                    Shared.MainActivity.SetTxvLocation();
                    Thread.Sleep(sleep);
                }
            }));
        }

        public static void AddSetTxvNetwork(int sleep = 1000)
        {
            Add("txvlocation", new VThread(() =>
            {
                while (true)
                {
                    StringBuilder sb = new StringBuilder();
                    Ping ping = new Ping();
                    var reply = ping.Send("1.1.1.1");
                    switch (reply.Status)
                    {
                        case IPStatus.Success:
                            sb.Append("OK [")
                              .Append(reply.RoundtripTime)
                              .Append("ms]");
                            break;
                        default:
                            sb.Append("Failed [")
                              .Append(reply.Status)
                              .Append("]");
                            break;
                    }
                    Shared.InfoText.Network = sb.ToString();
                    Thread.Sleep(sleep);
                }
            }));
        }
    }
}