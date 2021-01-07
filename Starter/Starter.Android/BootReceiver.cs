using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Starter.Droid
{
    //[BroadcastReceiver]
    //[IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted })]
    //[IntentFilter(new[] { Intent.ActionBootCompleted }, Priority = (int)IntentFilterPriority.LowPriority)]
    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
    [IntentFilter(new string[] {
    Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted, "android.intent.action.QUICKBOOT_POWERON"
})]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {

            if (intent.Action == Intent.ActionBootCompleted)
            {                
                Intent i = new Intent(context, typeof(MainActivity));
                i.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(i);
            }

            
        }
    }
}