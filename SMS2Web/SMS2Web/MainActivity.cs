using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;

namespace SMS2Web
{
    
    [Activity(Label = "SMS2Web", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        bool state = false;
        SMSBroadcastReceiver receiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Set our view from the "main" layout resource
            base.OnCreate(savedInstanceState);

            receiver = new SMSBroadcastReceiver();
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.startButton);

           button.Click += delegate {

               Button start = FindViewById<Button>(Resource.Id.startButton);
               if (state)
               {
                   UnregisterReceiver(receiver);
                   start.Text = GetString(Resource.String.start);
                   state = false;
               }
               else
               {

                   EditText address = FindViewById<EditText>(Resource.Id.serverAddressText);
                   if (address.Text.Length == 0)
                   {
                       return;
                   }

                   string addressString = address.Text;
                   try
                   {
                       AsynchronousSocketSender.SetAddress(addressString);
                   }
                   catch(Exception ex)
                   {
                       Vibrator vibrator = (Vibrator)GetSystemService(Context.VibratorService);
                       vibrator.Vibrate(100);
                       return;
                   }
                   this.RegisterReceiver(receiver, new IntentFilter("android.provider.Telephony.SMS_RECEIVED"));
                   start.Text = GetString(Resource.String.stop);
                   state = true;

               }
            };
        }
    }
}

