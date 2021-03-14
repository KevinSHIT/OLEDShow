using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace OLEDShow
{
    class VThreadCollectionHelper
    {
        public static void AddAll(int setTimeSleep = 500, int setNetworkSleep = 5000,
            int setTxvLocationSleep = 30 * 60 * 1000, int networkAvgTime = 3, string networkTarget = "1.1.1.1", int networkShortSleep = 500)
        {
            AddSetTime(setTimeSleep);
            AddSetTxvLocation(setTxvLocationSleep);
            AddSetTxvNetwork(setNetworkSleep, networkShortSleep, networkAvgTime, networkTarget);
        }

        public static void Clear()
        {
            Shared.VThreadsCollection.Clear();
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
                Shared.VThreadsCollection[name].Stop();
                Shared.VThreadsCollection.Remove(name);
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

        public static void AddSetTxvNetwork(int sleep = 1000, int shortSleep = 500, int avgTime = 3, string target = "1.1.1.1")
        {
            Add("txvnetwork", new VThread(() =>
            {
                while (true)
                {
                    StringBuilder sb = new StringBuilder();
                    long[] pingValue = new long[avgTime];
                    HashSet<IPStatus> status = new HashSet<IPStatus>();
                    int validCount = 0;
                    for (int i = 0; i < avgTime; ++i)
                    {
                        Ping ping = new Ping();
                        var reply = ping.Send(target);
                        switch (reply.Status)
                        {
                            case IPStatus.Success:
                                ++validCount;
                                pingValue[i] = reply.RoundtripTime;
                                break;
                            default:
                                status.Add(reply.Status);
                                break;
                        }
                    }
                    if (validCount > 0)
                    {
                        long total = 0;
                        foreach (var v in pingValue)
                        {
                            total = total + v;
                        }
                        total = total / validCount;
                        sb.Append("OK [")
                                      .Append(total)
                                      .Append("ms ")
                                      .Append(validCount)
                                      .Append("/")
                                      .Append(avgTime)
                                      .Append("]");
                    }
                    else
                    {
                        sb.Append("Failed [");
                        sb.Append(string.Join(',', status));
                        sb.Append("]");

                    }

                    Shared.InfoText.Network = sb.ToString();
                    Thread.Sleep(sleep);
                }
            }));
        }
    }
}