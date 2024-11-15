using Android.App;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Net;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
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

                if (string.IsNullOrEmpty(Preferences.Get("NotificationDeviceToken", "")))
                {
                    _ = TinyInsights.TrackEventAsync("RegisterWithBellNotificationHub DeviceToken is null", null);
                    Log.Debug(TAG, $" DeviceToken is null");
                    return;
                }
                _ = TinyInsights.TrackEventAsync("RegisterWithBellNotificationHub value : " +jwt +","+ merchantID + ","+ deviceNo);
                Log.Debug(TAG, $"RegisterWithBellNotificationHub value : " + merchantID + "," + deviceNo);
                BellNotificationHub.Xamarin.Android.BellNotification bellNotification = new BellNotificationHub.Xamarin.Android.BellNotification(activity);
#if DEBUG
                System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaRegisterDeviceAsync(jwt, merchantID, deviceNo, false);
#else
                System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaRegisterDeviceAsync(jwt, merchantID, deviceNo, true);
#endif
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    success = true;
                    _ = TinyInsights.TrackEventAsync("RegisterWithBellNotificationHub Complete", null);
                    Log.Debug(TAG, $"RegisterWithBellNotificationHub Complete");
                }
                else
                {
                    reasonPhraseError = httpResponseMessage.ReasonPhrase;
                    _ = TinyInsights.TrackEventAsync("RegisterWithBellNotificationHub Error = " + httpResponseMessage.StatusCode + reasonPhraseError, null);
                    Log.Debug(TAG, $"RegisterWithBellNotificationHub  Error = " + httpResponseMessage.StatusCode + reasonPhraseError);
                }
            }
            catch (System.Exception ex)
            {
                reasonPhraseError = ex.Message;
                _ = TinyInsights.TrackErrorAsync(ex);
            }

            if (!success)
            {
                activity.RunOnUiThread(() =>
                {
                    Toast toast = Toast.MakeText(activity, "Register Bell is " + reasonPhraseError, ToastLength.Short);
                    View view = toast.View;
                    if (view == null) return;
                    view.Background.SetColorFilter(Color.DarkRed, PorterDuff.Mode.SrcIn);
                    TextView text = (TextView)view.FindViewById(Android.Resource.Id.Message);
                    text.SetTextColor(Color.White);

                    //toast.Show();
                    Log.Debug("Delay Onload", "4-!success");
                });
                Log.Error(TAG, $"Can't RegisterDevice : reasonPhrase : {reasonPhraseError}");
                _ = TinyInsights.TrackEventAsync($"UnRegisterWithBellNotificationHub  Error = : {reasonPhraseError}");
                return;
            }

            BellNotificationRegisted = true;
        }

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
                System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaUnRegisterDeviceAsync(jwt, true);
#endif 
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    success = true;
                    _ = TinyInsights.TrackEventAsync("UnRegisterWithBellNotificationHub Complete", null);
                    Log.Debug(TAG, $"UnRegisterWithBellNotificationHub Complete");
                }
                else
                {
                    reasonPhraseError = httpResponseMessage.ReasonPhrase; 
                    _ = TinyInsights.TrackEventAsync("RegisterWithBellNotificationHub Error = " + httpResponseMessage.StatusCode + reasonPhraseError, null);
                    Log.Debug(TAG, $"UnRegisterWithBellNotificationHub  Error = " + httpResponseMessage.StatusCode + reasonPhraseError);
                }
            }
            catch (System.Exception ex)
            {
                reasonPhraseError = ex.Message;
                _ = TinyInsights.TrackErrorAsync(ex);
            }

            if (!success)
            {
                activity.RunOnUiThread(() =>
                {
                    Toast toast = Toast.MakeText(activity, "UnRegister Bell is " + reasonPhraseError, ToastLength.Short);
#pragma warning disable CS0618 // Type or member is obsolete
                    View view = toast.View;
#pragma warning restore CS0618 // Type or member is obsolete
                    //Gets the actual oval background of the Toast then sets the colour filter
#pragma warning disable CS0618 // Type or member is obsolete
                    view.Background.SetColorFilter(Color.DarkRed, PorterDuff.Mode.SrcIn);
#pragma warning restore CS0618 // Type or member is obsolete
                    //Gets the TextView from the Toast so it can be editted
                    TextView text = (TextView)view.FindViewById(Android.Resource.Id.Message);
                    text.SetTextColor(Color.White);

                    //toast.Show();
                });
                Log.Error(TAG, $"Can't UnRegisterDevice : reasonPhrase : {reasonPhraseError}");
                _ = TinyInsights.TrackEventAsync($"UnRegisterWithBellNotificationHub  Error = : {reasonPhraseError}");
                return;
            }

            BellNotificationRegisted = false;
        }
    }


}
