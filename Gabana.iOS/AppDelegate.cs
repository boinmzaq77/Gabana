
using BellNotificationHub.Xamarin.iOS;
using Foundation;
using Gabana.iOS.Test;
using GlobalToast;
using System;
using System.Threading;
using TinyInsightsLib;
using TinyInsightsLib.ApplicationInsights;
using UIKit;
using UserNotifications;

namespace Gabana.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register ("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate {

        
        [Export("window")]
        public UIWindow Window { get; set; }
        public static int notificationCount = 0;

        [Export ("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            Console.WriteLine("Gabana.iOS::FinishedLaunching");
            AppDelegate.notificationCount = 0; 
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            JobQueue.Default = new JobQueue();

            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
            this.Window = new UIWindow(UIScreen.MainScreen.Bounds);
            InitThaiCalendarCrashFix();

            // ถ้า iOS ต่ำกว่า 13 จะต้องเปิด Main Windows ที่นี่
#if (DEBUG)
            ApplicationInsightsProvider appInsightsProvider = new ApplicationInsightsProvider("30fdd00a-0dcb-4dd5-be28-062f8765466b", "GabanaIOSAppLab");
#else
            ApplicationInsightsProvider appInsightsProvider = new ApplicationInsightsProvider("30fdd00a-0dcb-4dd5-be28-062f8765466b", "GabanaIOSApp");
#endif
            appInsightsProvider.LogLevel = TinyInsightsLib.TinyLogLevel.Infomation;
            TinyInsightsLib.TinyInsights.Configure(appInsightsProvider);

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Sound |
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge, null);
                    application.RegisterUserNotificationSettings(settings);
                    application.RegisterForRemoteNotifications();
            }
            else
            {
                application.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Badge |
                    UIRemoteNotificationType.Sound | UIRemoteNotificationType.Alert);
            }
            //Thread.Sleep(5000);
            Console.WriteLine("Gabana.iOS::FinishedLaunching 2 ");
            if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                Window = new UIWindow();
                Window.RootViewController = new SplashLoadingController();
                Window.MakeKeyAndVisible();
            }
            return true;
        }
        private static void InitThaiCalendarCrashFix()
        {
            var localeIdentifier = NSLocale.CurrentLocale.LocaleIdentifier;
            if (localeIdentifier == "th_TH")
            {
                new System.Globalization.ThaiBuddhistCalendar();
            }
        }

        [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
        public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Console.WriteLine("Gabana.iOS::RegisteredForRemoteNotifications");
            BellNotification.KeepDeviceToken(deviceToken);
        }

        
        [Export("application:openURL:options:")]
        public virtual bool OpenUrl(UIKit.UIApplication app, Foundation.NSUrl url, Foundation.NSDictionary options) 
        {

            return true;
        }

        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public void DidReceiveRemoteNotification(UIKit.UIApplication application, Foundation.NSDictionary userInfo, Action<UIKit.UIBackgroundFetchResult> completionHandler)
        {
            try
            {

            
                notificationCount++; 
                UIApplication.SharedApplication.ApplicationIconBadgeNumber = notificationCount;
                if (null != userInfo && userInfo.ContainsKey(new NSString("data")))
                {
                    //Get the aps dictionary
                    NSDictionary data = userInfo.ObjectForKey(new NSString("data")) as NSDictionary;

                    string message = string.Empty;
                    string codeno = string.Empty;
                    string action = string.Empty;

                    if (data.ContainsKey(new NSString("message")))
                    {
                        message = (data[new NSString("message")] as NSString).ToString();
                    }

                    if (data.ContainsKey(new NSString("codeno")))
                    {
                        codeno = (data[new NSString("codeno")] as NSString).ToString();
                    }

                    if (userInfo.ContainsKey(new NSString("action")))
                    {
                        action = (userInfo[new NSString("action")] as NSString).ToString();
                    }


                    if (UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active)
                    {

                        NotificationManager notificationManager = new NotificationManager();
                        InvokeOnMainThread(() =>
                        {
                            notificationManager.Noti(action, message);
                        });
                        //InvokeOnMainThread notificationManager.Noti(action, message);
                        completionHandler(UIBackgroundFetchResult.NewData);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(message))
                        {
                            // create the notification
                            var notification = new UILocalNotification();

                            // set the fire date (the date time in which it will fire)
                            notification.FireDate = NSDate.FromTimeIntervalSinceNow(2);

                            // configure the alert
                            notification.AlertAction = "Gabana";
                            notification.AlertBody = message;

                            // modify the badge
                            //notification.ApplicationIconBadgeNumber = 2;

                            // set the sound to be the default sound
                            notification.SoundName = UILocalNotification.DefaultSoundName;

                            // userinfo
                            notification.UserInfo = new NSDictionary(
                                new NSString("action"), new NSString(action),
                                new NSString("codeno"), new NSString(codeno)
                            );

                            // schedule it
                            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                        }
                        completionHandler(UIBackgroundFetchResult.NewData);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        public void ShowMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Toast.MakeToast(message)
                     .SetPosition(ToastPosition.Top) // Default is Bottom
                     .SetAnimator(new GlobalToast.Animation.ScaleAnimator())
                     .SetDuration(ToastDuration.Long) // Default is Regular
                     .Show();

            }
        }

        [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
        public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            System.Console.WriteLine("Gabana.iOS::FailedToRegisterForRemoteNotifications : Error ****" + error.LocalizedDescription + "****");

            UIViewController viewController = DataCaching.SplashLoadingController;
            if (viewController != null)
            {
                Utils.ShowAlert(viewController, "Error : FailedToRegisterForRemoteNotifications (Dev:I0002)", error.LocalizedDescription);
            }
        }


        // UISceneSession Lifecycle

        [Export ("application:configurationForConnectingSceneSession:options:")]
        public UISceneConfiguration GetConfiguration (UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            // Called when a new scene session is being created.
            // Use this method to select a configuration to create the new scene with.
            return UISceneConfiguration.Create ("Default Configuration", connectingSceneSession.Role);
        }

        [Export ("application:didDiscardSceneSessions:")]
        public void DidDiscardSceneSessions (UIApplication application, NSSet<UISceneSession> sceneSessions)
        {
            // Called when the user discards a scene session.
            // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
            // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        }



    }
}

