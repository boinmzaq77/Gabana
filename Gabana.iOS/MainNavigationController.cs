
using Foundation;
using Gabana.iOS.ITEMS;
using System;
using UIKit;
using Gabana3.JAM.Merchant;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Gabana.ShareSource;
using Gabana.POS;
using System.Timers;
using BellNotificationHub.Xamarin.iOS;
using Gabana.ios;

namespace Gabana.iOS
{
    public partial class MainNavigationController : UINavigationController
    {
        public event EventHandler PoppedViewController;
        Merchants merchant;
        string page; 
        public MainNavigationController(string page)
        {
            this.page = page;
        }
        public MainNavigationController(string page , Merchants merchant)
        {
            this.merchant = merchant;
            this.page = page;
        }
        public override UIViewController PopViewController(bool animated)
        {
            PoppedViewController?.Invoke(this, null);
            return base.PopViewController(animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();



            //Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            //Timer timer = new Timer();
            //timer.Interval = 600000;
            //timer.Elapsed += (sender, e) => { TimerResentData(); };
            //timer.Start();

            UIViewController controller = new UIViewController();
            switch (page)
            {
                case "main":
                    controller = new MainController(merchant);
                    DataCaching.mainpage = controller;
                    Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
                    Timer timer = new Timer();
                    timer.Interval = 600000;
                    timer.Elapsed += (sender, e) => { TimerResentData(); };
                    timer.Start();
                    break;
                case "choosebranch":
                    //Utils.SetTitle(this, "");
                    controller = new BranchController(merchant);
                    break;
                case "updatemerchant":
                    controller = new UpdatProfileController();
                    break;
                case "term":
                    controller = new TermSettingController(true);
                    break;
                default:
                    controller = new MainController(merchant);
                    DataCaching.mainpage = controller; 
                    break;
            }
            
            this.PushViewController(controller, true);
        }
        public void TimerResentData()
        {
            InvokeOnMainThread(async () =>
            {
                try
                {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        if (!BellNotification.IsRegisted())
                        {
                            var tokenbell = Preferences.Get("NotificationDeviceToken", "");
                            if (!string.IsNullOrEmpty(tokenbell))
                            {
                                BellNotificationHelper.RegisterBellNotification(GabanaAPI.gbnJWT, DataCashingAll.MerchantId, DataCashingAll.DeviceNo);
                            }
                        }
                        ResendData();
                    }
                }
                catch (Exception ex)
                {
                    _ = TinyInsightsLib.TinyInsights.TrackErrorAsync(ex, null);
                    _ = TinyInsightsLib.TinyInsights.TrackPageViewAsync("TimerResentData at Main");
                    Utils.ShowMessage(ex.Message);
                }
            });
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                var access = e.NetworkAccess;
                var profiles = e.ConnectionProfiles;
                DataCashingAll.CheckConnectInternet = true;
                //Toast.MakeText(this, "Internet Connect", ToastLength.Long).Show();

                //-----------------------------------------------------------
                //Resend Fwaiting = 2
                //-----------------------------------------------------------
                InvokeOnMainThread(() =>
                {
                     ResendData();
                });
            }
            else
            {
                DataCashingAll.CheckConnectInternet = false;
                //Toast.MakeText(this, "No Internet", ToastLength.Long).Show();
            }
        }
        private async Task ResendData()
        {
            //Item
            await Utils.ResentItem();

            //Category
            await Utils.ResentCategory();

            //Tran
            await Utils.ResentTran();

            //Customer
            await Utils.ResentCustomer();

            //NoteCategory
            await Utils.ResentNoteCategory();

            //Note
            await Utils.ResentNote();

            //Trant PrinCounter
            await Utils.ResentPrintCounter();
        }
    }
}