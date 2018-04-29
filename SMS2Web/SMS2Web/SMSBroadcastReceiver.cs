using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;

namespace SMS2Web
{
    [BroadcastReceiver(Enabled = true, Label = "SMS Receiver")]
    public class SMSBroadcastReceiver : BroadcastReceiver
    {

        private const string Tag = "SMSBroadcastReceiver";
        private const string IntentAction = "android.provider.Telephony.SMS_RECEIVED";

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action != IntentAction) return;

            SmsMessage[] messages = Telephony.Sms.Intents.GetMessagesFromIntent(intent);

            var sb = new StringBuilder();

            for (var i = 0; i < messages.Length; i++)
            {

                AsynchronousSocketSender.Send(messages[i].MessageBody);
            }

        }
    }
}