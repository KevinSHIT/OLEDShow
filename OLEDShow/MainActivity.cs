using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Android.Widget;
using AndroidX.Fragment.App;
using Android.Util;

namespace OLEDShow
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : FragmentActivity
    {
        public TextView TxvText;

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

            VThreadCollectionHelper.AddAll();
            VThreadCollectionHelper.StartAll();

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

        public void SetTxvLocation()
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

        public void SetTxvText(string str)
        {
            RunOnUiThread(() =>
                TxvText.Text = str
            );
        }
    }
}
