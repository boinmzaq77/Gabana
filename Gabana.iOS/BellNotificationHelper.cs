using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BellNotificationHub.Xamarin.iOS;
using Foundation;
using GlobalToast;
using TinyInsightsLib;
using UIKit;

namespace Gabana.ios
{
    static public class BellNotificationHelper
    {
        static bool BellNotificationRegisted = false;   // เป็นตัวแปลที่ช่วยในการบอกว่าได้ทำการ Register ไปที่ BellNotification แล้วหรือยัง
        internal static async Task RegisterBellNotification(string jwt, int merchantID, int deviceNo)
        {
            if (BellNotificationRegisted) return;

            bool success = false;
            string reasonPhraseError = "";
            // Register with Bell Notification Hubs
            try
            {
                BellNotification bellNotification = new BellNotification();

#if (DEBUG)
                System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaRegisterDeviceAsync(jwt, merchantID, deviceNo, false);
#else
                System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaRegisterDeviceAsync(jwt, merchantID, deviceNo, true);
#endif

                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    success = true;
                    Console.WriteLine("Gabana.iOS::RegisterBellNotification Sucess");
                }
                else
                {
                    reasonPhraseError = httpResponseMessage.ReasonPhrase;
                    _ = TinyInsights.TrackErrorAsync(new Exception(httpResponseMessage.ReasonPhrase.ToString()));
                }
            }
            catch (System.Exception ex)
            {
                reasonPhraseError = ex.Message;
            }

            if (!success)
            {
                Console.WriteLine("Error : RegisterBellNotification (Dev:I0001) : " + reasonPhraseError);
                //Toast.MakeToast(reasonPhraseError)
                //     .SetTitle("Error : RegisterBellNotification (Dev:I0001)")
                //     .SetAnimator(new GlobalToast.Animation.ScaleAnimator())
                //     .SetAppearance(new ToastAppearance
                //     {
                //         Color = UIColor.Red,
                //         CornerRadius = 8,
                //     })
                //     .SetDuration(ToastDuration.Long) // Default is Regular
                //     .Show();
                return;
            }

            BellNotificationRegisted = true;
        }

        internal static async Task UnRegisterBellNotification(string jwt)
        {
            if (!BellNotificationRegisted) return;

            bool success = false;
            string reasonPhraseError = "";
            // UnRegister with Bell Notification Hubs
            try
            {
                BellNotification bellNotification = new BellNotification();
#if (DEBUG)
                System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaUnRegisterDeviceAsync(jwt, false);
#else
                System.Net.Http.HttpResponseMessage httpResponseMessage = await bellNotification.GabanaUnRegisterDeviceAsync(jwt, true);
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
                Console.WriteLine("Error : UnRegisterBellNotification (Dev:I0001) : " + reasonPhraseError);

                Toast.MakeToast(reasonPhraseError)
                     .SetTitle("Error : UnRegisterBellNotification (Dev:I0001)")
                     .SetAnimator(new GlobalToast.Animation.ScaleAnimator())
                     .SetAppearance(new ToastAppearance
                     {
                         Color = UIColor.Red,
                         CornerRadius = 8,
                     })
                     .SetDuration(ToastDuration.Long) // Default is Regular
                     .Show();
                return;
            }

            BellNotificationRegisted = false;
        }


    }
}