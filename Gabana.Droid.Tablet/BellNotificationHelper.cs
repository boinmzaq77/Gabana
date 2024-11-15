using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet
{
    static public class BellNotificationHelper
    {
        public const string TAG = "BellNotificationHelper";
        static bool BellNotificationRegisted = false;   // เป็นตัวแปลที่ช่วยในการบอกว่าได้ทำการ Register ไปที่ BellNotification แล้วหรือยัง

        internal static async Task RegisterBellNotification(Activity activity, string jwt, int merchantID, int deviceNo)
        {
            Log.Debug("Delay Onload", "3-RegisterBellNotification");
            Log.Debug(TAG, $"RegisterWithBellNotificationHub");
            if (BellNotificationRegisted) return;
            bool success = false;
            string reasonPhraseError = "";
            try
            {
                BellNotificationHub.Xamarin.Android.BellNotification bellNotification = new BellNotificationHub.Xamarin.Android.BellNotification(activity);
#if DEBUG
                System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaRegisterDeviceAsync(jwt, merchantID, deviceNo, false);
#else
                System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaRegisterDeviceAsync(jwt, merchantID, deviceNo, true);
#endif
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    success = true;
                    await TinyInsights.TrackEventAsync("RegisterWithBellNotificationHub Complete", null);
                }
                else
                {
                    reasonPhraseError = httpResponseMessage.ReasonPhrase;
                    await TinyInsights.TrackEventAsync("RegisterWithBellNotificationHub Error = " + httpResponseMessage.StatusCode + reasonPhraseError, null);
                }
            }
            catch (System.Exception ex)
            {
                reasonPhraseError = ex.Message;
            }

            if (!success)
            {
                activity.RunOnUiThread(() =>
                {
                    Toast toast = Toast.MakeText(activity, "Register Bell is " + reasonPhraseError, ToastLength.Short);
                    View view = toast.View;
                    if (view == null) return;
                    //Gets the actual oval background of the Toast then sets the colour filter
                    view.Background.SetColorFilter(Color.DarkRed, PorterDuff.Mode.SrcIn);
                    //Gets the TextView from the Toast so it can be editted
                    TextView text = (TextView)view.FindViewById(Android.Resource.Id.Message);
                    text.SetTextColor(Color.White);

                    //toast.Show();
                    Log.Debug("Delay Onload", "4-!success");
                });
                Log.Error(TAG, $"Can't RegisterDevice : reasonPhrase : {reasonPhraseError}");
                return;
            }

            BellNotificationRegisted = true;
        }

        [System.Obsolete]
        internal static async Task UnRegisterBellNotification(Activity activity, string jwt)
        {
            Log.Debug(TAG, $"UnRegisterWithBellNotificationHub");
            if (!BellNotificationRegisted) return;

            bool success = false;
            string reasonPhraseError = "";
            try
            {
                BellNotificationHub.Xamarin.Android.BellNotification bellNotification = new BellNotificationHub.Xamarin.Android.BellNotification(activity);
#if DEBUG
                System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaUnRegisterDeviceAsync(jwt, false);
#else   
                System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaUnRegisterDeviceAsync(jwt, false);
                //System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaUnRegisterDeviceAsync(jwt, true);
#endif
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    success = true;
                }
                else
                {
                    reasonPhraseError = httpResponseMessage.ReasonPhrase;
                }
            }
            catch (System.Exception ex)
            {
                reasonPhraseError = ex.Message;
            }

            if (!success)
            {
                activity.RunOnUiThread(() =>
                {
                    Toast toast = Toast.MakeText(activity, "UnRegister Bell is " + reasonPhraseError, ToastLength.Short);
                    View view = toast.View;
                    //Gets the actual oval background of the Toast then sets the colour filter
                    view.Background.SetColorFilter(Color.DarkRed, PorterDuff.Mode.SrcIn);
                    //Gets the TextView from the Toast so it can be editted
                    TextView text = (TextView)view.FindViewById(Android.Resource.Id.Message);
                    text.SetTextColor(Color.White);

                    //toast.Show();
                });
                Log.Error(TAG, $"Can't UnRegisterDevice : reasonPhrase : {reasonPhraseError}");
                return;
            }

            BellNotificationRegisted = false;
        }
    }

}