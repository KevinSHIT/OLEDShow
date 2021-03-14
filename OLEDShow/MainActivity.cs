using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Android.Widget;
using System.Threading;
using AndroidX.Fragment.App;
using Android.Util;
using System.Net.NetworkInformation;
using System.Text;

namespace OLEDShow
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : FragmentActivity
    {
        public TextView TxvText;
        Thread ThrSetTime, ThrSetNetwork;
        bool _isSetTimeThreadWork = true;
        bool _isSetNetworkThreadWork = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Shared.MainActivity = this;

            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

            HideSystemUI();


            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            TxvText = FindViewById<TextView>(Resource.Id.TxvText);

            TxvText.Click += TxvText_Click;

            StartSetTimeThread();
            StartSetNetworkThread();

        }

        public void HideSystemUI()
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
            {
                Window.SetDecorFitsSystemWindows(false);
            }
            else
            {
                Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
            }
        }

        public (int, int) GetScreenWidthAndHeight()
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
            {
                WindowMetrics windowMetrics = WindowManager.CurrentWindowMetrics;
                // var insets = windowMetrics.WindowInsets
                //     .GetInsetsIgnoringVisibility(WindowInsets.Type.SystemBars());
                return (windowMetrics.Bounds.Width(), windowMetrics.Bounds.Height());
            }
            else
            {
                DisplayMetrics displayMetrics = new DisplayMetrics();
                WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
                return (displayMetrics.WidthPixels, displayMetrics.HeightPixels);
            }
        }

        private void TxvText_Click(object sender, EventArgs e)
        {
            SetTxvLocation();
        }

        private void SetTxvLocation()
        {
            float dx, dy;

            int txvWidth = TxvText.Width;
            int txvHeight = TxvText.Height;

            (int disWidth, int disHeight) = GetScreenWidthAndHeight();

            do
            {
                Random R = new Random(Guid.NewGuid().GetHashCode());
                var w = GetScreenWidthAndHeight();
                dx = (float)R.NextDouble() * disWidth;
                dy = (float)R.NextDouble() * disHeight;
            }
            while (dx + txvWidth > disWidth || dy + txvHeight > disHeight);
            SetTxvLocation(dx, dy);
        }

        private void SetTxvLocation(float x, float y, long duration = 0)
        {
            RunOnUiThread(() =>
               TxvText.Animate()
                .X(x)
                .Y(y)
                .SetDuration(duration)
                .Start()
            );
        }

        private void StartSetTimeThread()
        {
            try
            {
                ThrSetTime.Abort();
            }
            catch { }

            ThrSetTime = new Thread(() =>
            {
                while (_isSetTimeThreadWork)
                {
                    SetTxvLocation();
                    Shared.InfoText.Time = DateTime.Now.ToString("hh:mm:ss");
                    Thread.Sleep(500);
                }

            });
            ThrSetTime.Start();
        }

        private void StartSetNetworkThread()
        {
            try
            {
                ThrSetNetwork.Abort();
            }
            catch { }

            ThrSetNetwork = new Thread(() =>
            {
                while (_isSetNetworkThreadWork)
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
                    Thread.Sleep(1000);
                }


            });
            ThrSetNetwork.Start();
        }


        public void SetTxvText(string str)
        {
            RunOnUiThread(() =>
                TxvText.Text = str
            );
        }
    }
}
