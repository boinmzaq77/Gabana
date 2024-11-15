using CoreGraphics;
using Foundation;
using System;
using UIKit;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System.Collections.Generic;
using Polly;
using System.Threading.Tasks; 
using Xamarin.Essentials;
using Gabana.iOS.Test;
using Gabana.POS.Cart;
using Gabana.POS;
using System.Linq;
using Gabana.ios;
using Newtonsoft.Json;
using Gabana3.JAM.Merchant;
using Gabana.ShareSource.Manage;
using LinqToDB.Common;
using System.IO;
using StoreKit;
using Plugin.InAppBilling;
using Gabana.Model;
using Gabana.AppSetting;
using System.Globalization;
using TinyInsightsLib;
using Acr.Logging;
using LinqToDB.SqlQuery;
using GlobalToast;
using CoreImage;

namespace Gabana.iOS
{
    public partial class MainController : UIViewController    {
        UIBarButtonItem backButton;

        UIImageView menuImg, logoImg, profileImg;
        UIView topBar, profileView;
        UILabel lbluserName, lblMerchantName, lblBranch;
        UIView ChangeBranchView;
        UIImageView BranchImg;
        UILabel lblChangeBranch;
        
        ItemsController itemsPage = null;
        public static CustomerController customerPage = null;
        UICollectionView mainMenuCollectionView;
        MyDataSource myDataSource;
        public static bool POS;
        public static Gabana3.JAM.Merchant.Merchants merchant;
        public static Merchant merchantlocal;
        DashBoardController dashBoardPage = null;
        ChangePasswordController changePage = null;
        PackageController packpage = null;
        LanguageSettingController langPage = null;
        SeniorContactController contactPage = null;
        BillHistoryController BillHisPage=null;
        ReportController ReportPage = null; 
        ChangeBranchController ChangeBranchPage = null;
        MyQrController myQrPage = null;
        TermSettingController TermPage = null;
        UIView contentBarView,closeSideBarView;
        SettingController settingPage = null;
        UIView UpperView, PasswordView, LanguageView, ContactUSView, TermView, VersionView, LogoutView;
        UIImageView logoImgSide, gabanatxtImg, PassImg, LangImg, ContactImg, TermImg, VersionImg, LogoutImg;
        UILabel lblPassword, lblLang, lblContact, lblTerm, lblVersion, lblLogout, lblVersionNo;
        InAppPurchaseManager iap;
        CustomPaymentObserver theObserver;
        PoolManage pool = new PoolManage();
        bool pricesLoaded = false;
        private string LoginType;
        NSObject priceObserver, succeededObserver, failedObserver, requestObserver;
        public static string Buy5ProductId = "com.seniorsoft.Gabana.1644832165";
        public static InAppBillingPurchase inAppBillingPurchase = new InAppBillingPurchase();
        string[] ProductId = new string[]
                {
                "01",
                "02",
                "03",
                "04"
                };
        List<string> products;
        private List<InAppBillingPurchase> purchesesold;
        private UIView PackView;
        private UIImageView PackImg;
        private UILabel lblPack;
        private UILabel lblprice;
        private UILabel lblpackage;
        string SubscripttionType;

        public static bool checknet = true;
        //NSObject priceObserver;
        public MainController(Gabana3.JAM.Merchant.Merchants merchant)
        {
            //this.merchant = merchant; 
            products = new List<string>() { Buy5ProductId };
            iap = new InAppPurchaseManager();
            theObserver = new CustomPaymentObserver(iap);

            // Call this once upon startup of in-app-purchase activities
            // This also kicks off the TransactionObserver which handles the various communications
            SKPaymentQueue.DefaultQueue.AddTransactionObserver(theObserver);


        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            // 
            try
            {
                //var lang = NSBundle.MainBundle.PreferredLocalizations[0];

                this.NavigationController.SetNavigationBarHidden(true, false);


                if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
                {

                    var navBarAppearance = new UINavigationBarAppearance();
                    navBarAppearance.ConfigureWithOpaqueBackground();
                    navBarAppearance.TitleTextAttributes = new UIStringAttributes()
                    {
                        ForegroundColor = UIColor.White
                    };
                    navBarAppearance.LargeTitleTextAttributes = new UIStringAttributes()
                    {
                        ForegroundColor = UIColor.White
                    };
                    navBarAppearance.BackgroundColor = UIColor.White;
                    this.NavigationController.NavigationBar.StandardAppearance = navBarAppearance;
                    this.NavigationController.NavigationBar.ScrollEdgeAppearance = navBarAppearance;
                    this.NavigationController.NavigationBar.TintColor = UIColor.Black;
                }
                else
                {
                    this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                    this.NavigationController.NavigationBar.TintColor = UIColor.Black;
                    this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
                    {
                        ForegroundColor = UIColor.White
                    };
                }



                if (POS)
                {
                    POS = false;
                    Utils.SetTitle(this.NavigationController, "POS");
                    DataCaching.posPage = new POSController();
                    this.NavigationController.PushViewController(DataCaching.posPage, false);
                }
                priceObserver = NSNotificationCenter.DefaultCenter.AddObserver(
              InAppPurchaseManager.InAppPurchaseManagerProductsFetchedNotification,
            (notification) => {
                var info = notification.UserInfo;
            });

                //var text2 = NSBundle.MainBundle.PathForResource("Localizable", "strings", "th.lproj");
                //var x = NSBundle.FromPath(text2);
                //var text3 = x.GetLocalizedString(id, defaulttext);

                iap.RequestProductData(products);

            if (await GabanaAPI.CheckNetWork())
            {
                bool Expiry = await CheckExpireDate();
                if (Expiry)
                {
                        var okCancelAlertController = UIAlertController.Create("", "Your package has expired.", UIAlertControllerStyle.Alert);
                        okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                            alert => logOutSetting()));
                        PresentViewController(okCancelAlertController, true, null);
                }
                await GetmerchantConfig();
            }

            var province = await pool.GetProvinces();
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            await LoadUserProfile();

            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                var pinCodePage = new PinCodeController("Pincode");
                pinCodePage.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                await this.PresentViewControllerAsync(pinCodePage, false);
            }

                //priceObserver = NSNotificationCenter.DefaultCenter.AddObserver(InAppPurchaseManager.InAppPurchaseManagerProductsFetchedNotification,
                //    (notification) => {
                //        var info = notification.UserInfo;
                //        if (info == null)
                //            return;

                //        var NSBuy5ProductId = new NSString(Buy5ProductId);

                //        if (info.ContainsKey(NSBuy5ProductId))
                //        {
                //            pricesLoaded = true;

                //            var product = (SKProduct)info[NSBuy5ProductId];
                //            Print(product);
                //            //SetVisualState(buy5Button, buy5Title, buy5Description, product);
                //        }

                //        var NSBuy10ProductId = new NSString(Buy5ProductId);
                //        if (info.ContainsKey(NSBuy10ProductId))
                //        {
                //            pricesLoaded = true;

                //            var product = (SKProduct)info[NSBuy10ProductId];
                //            Print(product);
                //            //SetVisualState(buy10Button, buy10Title, buy10Description, product);
                //        }
                //    });

                //// only if we can make payments, request the prices
                //if (iap.CanMakePayments())
                //{
                //    // now go get prices, if we don't have them already
                //    if (!pricesLoaded)
                //        iap.RequestProductData(new List<string>() { "D8788BKXA7" }); // async request via StoreKit -> App Store
                //}
                //else
                //{
                //    // can't make payments (purchases turned off in Settings?)
                //    //buy5Button.SetTitle("AppStore disabled", UIControlState.Disabled);
                //    //buy10Button.SetTitle("AppStore disabled", UIControlState.Disabled);
                //}

                //balanceLabel.Text = String.Format(Balance, CreditManager.Balance());// + " monkey credits";

                //succeededObserver = NSNotificationCenter.DefaultCenter.AddObserver(InAppPurchaseManager.InAppPurchaseManagerTransactionSucceededNotification,
                //(notification) => {
                //    balanceLabel.Text = String.Format(Balance, CreditManager.Balance());// + " monkey credits";
                //});
                //failedObserver = NSNotificationCenter.DefaultCenter.AddObserver(InAppPurchaseManager.InAppPurchaseManagerTransactionFailedNotification,
                //(notification) => {
                //    // TODO:
                //    Console.WriteLine("Transaction Failed");
                //});

                //requestObserver = NSNotificationCenter.DefaultCenter.AddObserver(InAppPurchaseManager.InAppPurchaseManagerRequestFailedNotification,
                //                                                                 (notification) => {
                //                                                                     // TODO:
                //                                                                     Console.WriteLine("Request Failed");
                //                                                                     buy5Button.SetTitle("Network down?", UIControlState.Disabled);
                //                                                                     buy10Button.SetTitle("Network down?", UIControlState.Disabled);
                //                                                                 });
                //bool Expiry = await CheckExpireDate();
                //if (Expiry)
                //{
                //    if (LoginType.ToLower() == "owner")
                //    {

                //        var okCancelAlertController = UIAlertController.Create("", "Are you sure you want to void bill?", UIAlertControllerStyle.Alert);
                //        okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                //            alert => OpenPackage()));
                //        okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
                //        PresentViewController(okCancelAlertController, true, null);
                //    }
                //    else
                //    {
                //        var okCancelAlertController = UIAlertController.Create("", "Are you sure you want to void bill?", UIAlertControllerStyle.Alert);
                //        okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                //            alert => logOutSetting()));
                //        okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
                //        PresentViewController(okCancelAlertController, true, null);
                //    }
                //}
                
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                await TinyInsightsLib.TinyInsights.TrackErrorAsync(ex);
            }
        }

        

        private void OpenPackage()
        {
            packpage = new PackageController(); 
            //var packpage = new PackageDetailController();
            DataCaching.MainNavigation.PushViewController(packpage, false);
        }

        public override void ViewWillDisappear(bool animated)
        {
            //NSNotificationCenter.DefaultCenter.RemoveObserver(priceObserver);
            //NSNotificationCenter.DefaultCenter.RemoveObserver(succeededObserver);
            //NSNotificationCenter.DefaultCenter.RemoveObserver(failedObserver);
            //NSNotificationCenter.DefaultCenter.RemoveObserver(requestObserver);
            base.ViewWillDisappear(animated);

        }
        public override async void ViewDidLoad()
        {
            try
            {
                UIView.AnimationsEnabled = true;
                LoginType = Preferences.Get("LoginType", "");
                this.NavigationController.SetNavigationBarHidden(true, false);
                base.ViewDidLoad();
                //UIView.SetAnimationBeginsFromCurrentState(false);
                AppDelegate.notificationCount = 0;

                View.BackgroundColor = UIColor.White;
                topBar = new UIView();
                topBar.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(topBar);

                backButton = new UIBarButtonItem();
                backButton.TintColor = UIColor.FromRGB(64, 64, 64);
                this.NavigationController.NavigationBar.TopItem.BackBarButtonItem = backButton;
                DataCaching.TitlePage = backButton;
                await Utils.CheckJWT();
                //GetListPackage();
                //GetstatusPackage();

                #region UpperBar
                menuImg = new UIImageView();
                menuImg.Image = UIImage.FromBundle("Menu");
                menuImg.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(menuImg);

                menuImg.UserInteractionEnabled = true;
                var tapGesture0 = new UITapGestureRecognizer(this,
                  new ObjCRuntime.Selector("Menu:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                menuImg.AddGestureRecognizer(tapGesture0);

                logoImg = new UIImageView();
                logoImg.Image = UIImage.FromBundle("GabanaMain");
                logoImg.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(logoImg);
                #endregion

                #region profileView
                profileView = new UIView();
                profileView.TranslatesAutoresizingMaskIntoConstraints = false;
                profileView.BackgroundColor = new UIColor(red: 241 / 255f, green: 250 / 255f, blue: 255 / 255f, alpha: 1);
                profileView.Layer.CornerRadius = 11;
                profileView.Layer.ShadowColor = new UIColor(red: 0 / 255f, green: 0 / 255f, blue: 0 / 255f, alpha: 0.16f).CGColor;
                profileView.Layer.ShadowOffset = new CGSize(width: 0, height: 0.3);//Here you control x and y
                profileView.Layer.ShadowOpacity = 1f;
                profileView.Layer.ShadowRadius = 5f; //Here your control your blur
                View.AddSubview(profileView);

                profileImg = new UIImageView();
                profileImg.TranslatesAutoresizingMaskIntoConstraints = false;
                profileImg.Layer.CornerRadius = 50;
                profileImg.Layer.BorderWidth = 3f;
                profileImg.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1f).CGColor;
                profileImg.ClipsToBounds = true;
                //profileImg.UserInteractionEnabled = true;
                //var tapGesture = new UITapGestureRecognizer(this,
                //        new ObjCRuntime.Selector("ADD:"))
                //{
                //    NumberOfTapsRequired = 1 // change number as you want 
                //};
                //profileImg.AddGestureRecognizer(tapGesture);


                View.AddSubview(profileImg);

                lbluserName = new UILabel();
                lbluserName.Font = lbluserName.Font.WithSize(16);
                lbluserName.TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1f);
                lbluserName.Lines = 1;
                lbluserName.TextAlignment = UITextAlignment.Center;
                lbluserName.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(lbluserName);

                lblBranch = new UILabel();
                lblBranch.Font = lblBranch.Font.WithSize(12);
                lblBranch.TextColor = new UIColor(red: 136 / 255f, green: 136 / 255f, blue: 136 / 255f, alpha: 1f);
                lblBranch.Lines = 1;
                lblBranch.TextAlignment = UITextAlignment.Center;
                lblBranch.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(lblBranch);

                lblMerchantName = new UILabel();
                lblMerchantName.Font = lblMerchantName.Font.WithSize(12);
                lblMerchantName.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1f);
                lblMerchantName.Lines = 1;
                lblMerchantName.TextAlignment = UITextAlignment.Center;
                lblMerchantName.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(lblMerchantName);
                #endregion

                #region MainMenuCollectionView
                UICollectionViewFlowLayout flowLayout = new UICollectionViewFlowLayout();
                flowLayout.SectionInset = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);
                flowLayout.ItemSize = new CoreGraphics.CGSize(width: ((View.Frame.Width - 68) / 3) - 5, height: ((View.Frame.Width - 70) / 3) - 5);

                mainMenuCollectionView = new UICollectionView(frame: View.Frame, layout: flowLayout);
                mainMenuCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
                mainMenuCollectionView.RegisterClassForCell(cellType: typeof(MainMenuCollectionViewCell), reuseIdentifier: "mainMenuCell");
                mainMenuCollectionView.BackgroundColor = UIColor.White;
                myDataSource = new MyDataSource();
                mainMenuCollectionView.DataSource = myDataSource;
                MainMenuCollectionDelegate myCollectionDelegate = new MainMenuCollectionDelegate();


                myCollectionDelegate.OnItemSelected += async (indexPath) => {
                    // do somthing
                    var x = (indexPath).Item;
                    if (x == 0)
                    {
                        //GetListPackage();

                        Utils.SetTitle(this.NavigationController, "POS");
                        DataCaching.posPage = new POSController();
                        this.NavigationController.PushViewController(DataCaching.posPage, false);
                    }
                    else if (x == 1)
                    {
                        //BuyPack(0);
                        // item
                        Utils.SetTitle(this.NavigationController, Utils.TextBundle("item", "Items"));
                        if (itemsPage == null)
                        {
                            itemsPage = new ItemsController();
                            DataCaching.itempage = itemsPage;
                        }
                        itemsPage.FormMain = true;
                        this.NavigationController.PushViewController(itemsPage, false);
                    }
                    else if (x == 2)
                    {
                        //BuyPack(1);
                        // customer
                        Utils.SetTitle(this.NavigationController, Utils.TextBundle("customer", "Customer"));
                        
                        customerPage = new CustomerController();
                        
                        this.NavigationController.PushViewController(customerPage, false);
                    }

                    else if (x == 3)
                    {
                        //BuyPack(2);
                        //EMPLOYEE
                        Utils.SetTitle(this.NavigationController, Utils.TextBundle("employee", "Employee"));
                        DataCaching.employeeController = new EmployeeController();
                        this.NavigationController.PushViewController(DataCaching.employeeController, false);
                    }
                    else if (x == 4)
                    {
                        //BuyPack(3);
                        //DASHBOARD
                        Utils.SetTitle(this.NavigationController, Utils.TextBundle("dashboard", "Dashboard"));
                        dashBoardPage = new DashBoardController();
                        this.NavigationController.PushViewController(dashBoardPage, false);
                    }
                    else if (x == 5)
                    {
                        //GetstatusPackage();
                        Utils.SetTitle(this.NavigationController, Utils.TextBundle("report", "Report"));
                        if (ReportPage == null)
                        {
                            ReportPage = new ReportController();
                        }
                        this.NavigationController.PushViewController(ReportPage, false);

                    }
                    else if (x == 6)
                    {
                        //SETTING

                        //UpdatePack(3);
                        Utils.SetTitle(this.NavigationController, Utils.TextBundle("setting", "Setting"));
                        if (settingPage == null)
                        {
                            settingPage = new SettingController();
                        }
                        this.NavigationController.PushViewController(settingPage, false);
                    }
                    else if (x == 7)
                    {
                        //BILL HISTORY
                        Utils.SetTitle(this.NavigationController, Utils.TextBundle("menubillhis", "BillHistory"));
                        
                            BillHisPage = new BillHistoryController();
                        
                        //BillHisPage.FormMain = true;
                        this.NavigationController.PushViewController(BillHisPage, false);

                    }
                    else if (x == 8)
                    {
                        //iap.PurchaseProduct(Buy5ProductId);
                        //MY QR
                        Utils.SetTitle(this.NavigationController, Utils.TextBundle("menumyqr", "MyQR"));
                        //if (myQrPage == null)
                        //{
                            myQrPage = new MyQrController(false);
                        //}
                        this.NavigationController.PushViewController(myQrPage, false);
                    }
                };
                mainMenuCollectionView.Delegate = myCollectionDelegate;
                View.AddSubview(mainMenuCollectionView);
                #endregion

                SetupAutoLayout();
              //  LoadUserProfile();
                sideBarView();
                string Username = Preferences.Get("User", "");
                UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
                var Data = await accountInfoManage.GetUserAccount(DataCashingAll.MerchantId, Preferences.Get("User", ""));
                if (Data?.FUsePincode == 1&& !string.IsNullOrEmpty(Data?.PinCode))
                {
                    DataCashingAll.UsePinCode = true;
                }
                else
                {
                    DataCashingAll.UsePinCode = false;
                }

//                var purchaseHistory = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.Subscription);

//#if DEBUG
//                inAppBillingPurchase = purchaseHistory.Where(x => x.State == Plugin.InAppBilling.PurchaseState.Purchased).OrderByDescending(x => x.TransactionDateUtc).FirstOrDefault();
//#else
//                inAppBillingPurchase = purchaseHistory.Where(x => x.State == Plugin.InAppBilling.PurchaseState.Purchased).OrderByDescending(x => x.TransactionDateUtc).FirstOrDefault(); 
//#endif
//                if (inAppBillingPurchase == null)
//                {
//                    lblpackage.Text = "Package : 1Branch/5Users (Free)";
//                }
//                else
//                {
//                    List<string> detail = Utils.SetDetailPackage(inAppBillingPurchase.ProductId);
//                    //var detail = inAppBillingPurchase.ProductId.Replace("package", "");
//                    //detail = detail.Replace("branch", "");
//                    //detail = detail.Replace("user", "");
//                    lblpackage.Text = "แพ็กเกจปัจจุบัน : "
//                                        + detail[1] + " สาขา "
//                                        + detail[0] + " พนักงาน";
//                }
                
                //SetDetailPackage();
                checknet = await GabanaAPI.CheckNetWork();
            }
            catch (Exception ex)
            {
                 await TinyInsightsLib.TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        private void SetDetailPackage()
        {
            try
            {
                SubscripttionType = DataCashingAll.setmerchantConfig?.SUBSCRIPTION_TYPE;
                int PackageIDCurrent = 1;
                PackageIDCurrent = Utils.SetPackageID(DataCashingAll.GetGabanaInfo.TotalBranch, DataCashingAll.GetGabanaInfo.TotalUser);
                List<string> detail = Utils.SetDetailPackage(PackageIDCurrent.ToString());
                switch (SubscripttionType)
                {
                    case "P":
                    case "A":
                    case "F":
                    case "U":
                    case "B":
                        //รายละเอียดจาก gabanaInfo
                        lblpackage.Text = Utils.TextBundle("nowpackage","") + " : "
                                            + detail[1] + " " + Utils.TextBundle("branch", "") + " "
                                            + detail[0] + " " + Utils.TextBundle("user", "");
                        break;
                    default:
                        lblpackage.Text = "Package : 1Branch/5Users (Free)";
                        break;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDetailPackageOnDB");
            }
        }
        private async void GetListPackage()
        {
            try
            {
                var connect = await CrossInAppBilling.Current.ConnectAsync();
                if (!connect)
                {
                    return;
                }
                var re = await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.Subscription, ProductId);
            }
            catch (Exception ex)
            {
                
            }
            finally 
            {
                 await CrossInAppBilling.Current.DisconnectAsync();
            }
        }
        private async void GetstatusPackage()
        {
            try
            {
                var connect = await CrossInAppBilling.Current.ConnectAsync();
                if (!connect)
                {
                    return;
                }

                purchesesold = (await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.Subscription)).ToList();
                
            }
            catch (Exception ex)
            {

            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }
        private async void BuyPack(int numpack)
        {
            try
            {
                var connect = await CrossInAppBilling.Current.ConnectAsync();
                if (!connect)
                {
                    return;
                }
                var re = await CrossInAppBilling.Current.PurchaseAsync(ProductId[numpack],ItemType.Subscription);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }
        private async void UpdatePack(int numpack)
        {
            try
            {
                var connect = await CrossInAppBilling.Current.ConnectAsync();
                if (!connect)
                {
                    return;
                }
                var re = await CrossInAppBilling.Current.UpgradePurchasedSubscriptionAsync(ProductId[numpack], purchesesold[purchesesold.Count-1].PurchaseToken);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            if (itemsPage == null)
            {
                itemsPage = new ItemsController();
                DataCaching.itempage = itemsPage;
            }
            this.NavigationController.PushViewController(itemsPage, false);
        }

        public void RequestProductData(List<string> productIds)
        {
            var array = new NSString[productIds.Count];
            for (var i = 0; i < productIds.Count; i++)
            {
                array[i] = new NSString(productIds[i]);
            }
            NSSet productIdentifiers = NSSet.MakeNSObjectSet<NSString>(array);
            var productsRequest = new SKProductsRequest(productIdentifiers);
            //productsRequest.Delegate = this; // for SKProductsRequestDelegate.ReceivedResponse
            productsRequest.Start();
        }
        [Export("ADD:")]
        public void ADD(UIGestureRecognizer sender)
        {
            var path = merchantlocal.LogoPath;
            string realpath; 
            if (!string.IsNullOrEmpty(path))
            {
                
                realpath = path;
            }
            else
            {
                realpath = "LogoDefault.png";
            }
            //GabanaShowImage.SharedInstance.Show(this,realpath);
            
        }
        [Export("Close2:")]
        public void Close2(UIGestureRecognizer sender)
        {
            GabanaShowImage.SharedInstance.Hide();
        }
        void sideBarView()
        {
            #region intitial
            contentBarView = new UIView();
            contentBarView.TranslatesAutoresizingMaskIntoConstraints = false;
            contentBarView.BackgroundColor = UIColor.White;
            contentBarView.Hidden = true;
            View.AddSubview(contentBarView);
            View.BringSubviewToFront(contentBarView);

            closeSideBarView = new UIView();
            closeSideBarView.TranslatesAutoresizingMaskIntoConstraints = false;
            closeSideBarView.BackgroundColor = UIColor.Black;
            closeSideBarView.Hidden = true;
            closeSideBarView.Layer.Opacity = 0.4f;
            View.AddSubview(closeSideBarView);
            View.BringSubviewToFront(closeSideBarView);

            #region UpperView
            UpperView = new UIView();
            UpperView.BackgroundColor = UIColor.FromRGB(241, 250, 255);
            UpperView.TranslatesAutoresizingMaskIntoConstraints = false;
            contentBarView.AddSubview(UpperView);

            //  logoImg,gabanatxtImg;
            logoImgSide = new UIImageView();
            logoImgSide.TranslatesAutoresizingMaskIntoConstraints = false;

            logoImgSide.Image = UIImage.FromBundle("GabanaMain.png");

            UpperView.AddSubview(logoImgSide);

            gabanatxtImg = new UIImageView();
            gabanatxtImg.TranslatesAutoresizingMaskIntoConstraints = false;

            gabanatxtImg.Image = UIImage.FromBundle("GabanaTxt.png");
            gabanatxtImg.ContentMode = UIViewContentMode.ScaleAspectFit;

            UpperView.AddSubview(gabanatxtImg);

            lblpackage = new UILabel();
            lblpackage.Text = "เริ่มต้น 750 บาท/เดือน";
            lblpackage.TranslatesAutoresizingMaskIntoConstraints = false;
            lblpackage.TextAlignment = UITextAlignment.Left;
            lblpackage.Font = lblpackage.Font.WithSize(15);
            lblpackage.TextColor = UIColor.FromRGB(162, 162, 162);
            UpperView.AddSubview(lblpackage);
            #endregion

            #region PasswordView
            PasswordView = new UIView();
            PasswordView.TranslatesAutoresizingMaskIntoConstraints = false;
            PasswordView.BackgroundColor = UIColor.White;
            contentBarView.AddSubview(PasswordView);

            PassImg = new UIImageView();
            PassImg.TranslatesAutoresizingMaskIntoConstraints = false;
            PassImg.Image = UIImage.FromBundle("MenuPassword");
            PasswordView.AddSubview(PassImg);

            lblPassword = new UILabel();
            lblPassword.Text = "Change Password";
            lblPassword.TranslatesAutoresizingMaskIntoConstraints = false;
            lblPassword.TextAlignment = UITextAlignment.Left;
            lblPassword.Font = lblPassword.Font.WithSize(15);
            lblPassword.TextColor = UIColor.FromRGB(64, 64, 64);
            PasswordView.AddSubview(lblPassword);

            PasswordView.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Pass:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            PasswordView.AddGestureRecognizer(tapGesture);
            #endregion

            #region ChangeBranchView
            ChangeBranchView = new UIView();
            ChangeBranchView.TranslatesAutoresizingMaskIntoConstraints = false;
            ChangeBranchView.BackgroundColor = UIColor.White;
            contentBarView.AddSubview(ChangeBranchView);

            BranchImg = new UIImageView();
            BranchImg.TranslatesAutoresizingMaskIntoConstraints = false;
            BranchImg.Image = UIImage.FromBundle("SettingBranch");
            ChangeBranchView.AddSubview(BranchImg);

            lblChangeBranch = new UILabel();
            lblChangeBranch.Text = "Change Branch";
            lblChangeBranch.TranslatesAutoresizingMaskIntoConstraints = false;
            lblChangeBranch.TextAlignment = UITextAlignment.Left;
            lblChangeBranch.Font = lblPassword.Font.WithSize(15);
            lblChangeBranch.TextColor = UIColor.FromRGB(64, 64, 64);
            ChangeBranchView.AddSubview(lblChangeBranch);

            ChangeBranchView.UserInteractionEnabled = true;
            var tapGestureChange = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("ChangeBranch:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ChangeBranchView.AddGestureRecognizer(tapGestureChange);
            #endregion

            //#region LanguageView
            //LanguageView = new UIView();
            //LanguageView.TranslatesAutoresizingMaskIntoConstraints = false;
            //LanguageView.BackgroundColor = UIColor.White;
            //LanguageView.Alpha = 0.5f;
            //contentBarView.AddSubview(LanguageView);

            //LangImg = new UIImageView();
            //LangImg.TranslatesAutoresizingMaskIntoConstraints = false;
            //LangImg.Image = UIImage.FromBundle("MenuLanguage");
            //LanguageView.AddSubview(LangImg);

            //lblLang = new UILabel();
            //lblLang.Text = "Language Setting";
            //lblLang.TranslatesAutoresizingMaskIntoConstraints = false;
            //lblLang.TextAlignment = UITextAlignment.Left;
            //lblLang.Font = lblLang.Font.WithSize(15);
            //lblLang.TextColor = UIColor.FromRGB(64, 64, 64);
            //LanguageView.AddSubview(lblLang);

            //LanguageView.UserInteractionEnabled = true;
            //var tapGesture1 = new UITapGestureRecognizer(this,
            //   new ObjCRuntime.Selector("Lang:"))
            //{
            //    NumberOfTapsRequired = 1 // change number as you want 
            //};
            ////LanguageView.AddGestureRecognizer(tapGesture1);
            //#endregion

            #region ContactUSView
            ContactUSView = new UIView();
            ContactUSView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContactUSView.BackgroundColor = UIColor.White;
            ContactUSView.Alpha = 0.5f;
            contentBarView.AddSubview(ContactUSView);

            ContactImg = new UIImageView();
            ContactImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ContactImg.Image = UIImage.FromBundle("MenuContact");
            ContactUSView.AddSubview(ContactImg);

            lblContact = new UILabel();
            lblContact.Text = "Contact Us";
            lblContact.TranslatesAutoresizingMaskIntoConstraints = false;
            lblContact.TextAlignment = UITextAlignment.Left;
            lblContact.Font = lblContact.Font.WithSize(15);
            lblContact.TextColor = UIColor.FromRGB(64, 64, 64);
            ContactUSView.AddSubview(lblContact);

            ContactUSView.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Contact:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ContactUSView.AddGestureRecognizer(tapGesture2);
            #endregion

            #region TermView
            TermView = new UIView();
            TermView.TranslatesAutoresizingMaskIntoConstraints = false;
            TermView.BackgroundColor = UIColor.White;
            contentBarView.AddSubview(TermView);

            TermImg = new UIImageView();
            TermImg.TranslatesAutoresizingMaskIntoConstraints = false;
            TermImg.Image = UIImage.FromBundle("MenuPolicy");
            TermView.AddSubview(TermImg);

            lblTerm = new UILabel();
            lblTerm.Text = "Term & Conditions";
            lblTerm.TranslatesAutoresizingMaskIntoConstraints = false;
            lblTerm.TextAlignment = UITextAlignment.Left;
            lblTerm.Font = lblTerm.Font.WithSize(15);
            lblTerm.TextColor = UIColor.FromRGB(64, 64, 64);
            TermView.AddSubview(lblTerm);

            TermView.UserInteractionEnabled = true;
            var tapGesture3 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Term:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            TermView.AddGestureRecognizer(tapGesture3);
            #endregion

            #region VersionView
            VersionView = new UIView();
            VersionView.TranslatesAutoresizingMaskIntoConstraints = false;
            VersionView.BackgroundColor = UIColor.White;
            contentBarView.AddSubview(VersionView);

            VersionImg = new UIImageView();
            VersionImg.TranslatesAutoresizingMaskIntoConstraints = false;
            VersionImg.Image = UIImage.FromBundle("MenuVersion");
            VersionView.AddSubview(VersionImg);

            lblVersion = new UILabel();
            lblVersion.Text = "Version";
            lblVersion.TranslatesAutoresizingMaskIntoConstraints = false;
            lblVersion.TextAlignment = UITextAlignment.Left;
            lblVersion.Font = lblVersion.Font.WithSize(15);
            lblVersion.TextColor = UIColor.FromRGB(64, 64, 64);
            VersionView.AddSubview(lblVersion);

            lblVersionNo = new UILabel();
            var version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString(); 
            var version2 = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
            lblVersionNo.Text = version + "."+version2;
            lblVersionNo.TranslatesAutoresizingMaskIntoConstraints = false;
            lblVersionNo.TextAlignment = UITextAlignment.Right;
            lblVersionNo.Font = lblVersionNo.Font.WithSize(15);
            lblVersionNo.TextColor = UIColor.FromRGB(162, 162, 162);
            VersionView.AddSubview(lblVersionNo);
            #endregion

            #region LogoutView
            LogoutView = new UIView();
            LogoutView.TranslatesAutoresizingMaskIntoConstraints = false;
            LogoutView.BackgroundColor = UIColor.White;
            contentBarView.AddSubview(LogoutView);

            LogoutImg = new UIImageView();
            LogoutImg.TranslatesAutoresizingMaskIntoConstraints = false;
            LogoutImg.Image = UIImage.FromBundle("MenuLogout");
            LogoutView.AddSubview(LogoutImg);

            lblLogout = new UILabel();
            lblLogout.Text = "Log Out";
            lblLogout.TranslatesAutoresizingMaskIntoConstraints = false;
            lblLogout.TextAlignment = UITextAlignment.Left;
            lblLogout.Font = lblLogout.Font.WithSize(15);
            lblLogout.TextColor = UIColor.FromRGB(64, 64, 64);
            LogoutView.AddSubview(lblLogout);

            LogoutView.UserInteractionEnabled = true;
            var tapGesture4 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Logout:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            LogoutView.AddGestureRecognizer(tapGesture4);
            #endregion

            #region PackView
            PackView = new UIView();
            PackView.TranslatesAutoresizingMaskIntoConstraints = false;
            PackView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            PackView.Layer.CornerRadius = 15;
            contentBarView.AddSubview(PackView);

            PackImg = new UIImageView();
            PackImg.TranslatesAutoresizingMaskIntoConstraints = false;
            PackImg.Image = UIImage.FromBundle("MenuPackage");
            PackView.AddSubview(PackImg);

            lblPack = new UILabel();
            lblPack.Text = "Package & Pricing";
            lblPack.TranslatesAutoresizingMaskIntoConstraints = false;
            lblPack.TextAlignment = UITextAlignment.Left;
            lblPack.Font = lblPack.Font.WithSize(15);
            lblPack.TextColor = UIColor.White;
            PackView.AddSubview(lblPack);

            lblprice = new UILabel();
            lblprice.Text = "เริ่มต้น 750 บาท/เดือน";
            lblprice.TranslatesAutoresizingMaskIntoConstraints = false;
            lblprice.TextAlignment = UITextAlignment.Left;
            lblprice.Font = lblprice.Font.WithSize(15);
            lblprice.TextColor = UIColor.White;
            PackView.AddSubview(lblprice);

            PackView.UserInteractionEnabled = true;
            var tapGesturepack = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("pack:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            PackView.AddGestureRecognizer(tapGesturepack);
            #endregion


            #endregion
            #region SetupLayout
            contentBarView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            contentBarView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            contentBarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            contentBarView.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 867) / 1000).Active = true;

            closeSideBarView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            closeSideBarView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            closeSideBarView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            closeSideBarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;

            #region UpperView
            UpperView.TopAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            UpperView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            UpperView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            UpperView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 149) / 1000).Active = true;

            logoImgSide.CenterYAnchor.ConstraintEqualTo(UpperView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            logoImgSide.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 128) / 1000).Active = true;
            logoImgSide.LeftAnchor.ConstraintEqualTo(UpperView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            logoImgSide.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 128) / 1000).Active = true;

            gabanatxtImg.CenterYAnchor.ConstraintEqualTo(UpperView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;

            gabanatxtImg.WidthAnchor.ConstraintEqualTo(100).Active = true;

            gabanatxtImg.LeftAnchor.ConstraintEqualTo(logoImgSide.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            //gabanatxtImg.RightAnchor.ConstraintEqualTo(UpperView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            gabanatxtImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 45) / 1000).Active = true;


            lblpackage.TopAnchor.ConstraintEqualTo(logoImgSide.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            lblpackage.RightAnchor .ConstraintEqualTo(UpperView.RightAnchor).Active = true;
            lblpackage.LeftAnchor.ConstraintEqualTo(gabanatxtImg.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            lblpackage.HeightAnchor.ConstraintEqualTo(23).Active = true;



            //gabanatxtImg.BackgroundColor = UIColor.Red;
            #endregion

            #region PasswordView
            PasswordView.TopAnchor.ConstraintEqualTo(UpperView.SafeAreaLayoutGuide.BottomAnchor, 8).Active = true;
            PasswordView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            PasswordView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            PasswordView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            PassImg.CenterYAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            PassImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            PassImg.LeftAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            PassImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblPassword.CenterYAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblPassword.LeftAnchor.ConstraintEqualTo(PassImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblPassword.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region ChangeBranchView
            ChangeBranchView.TopAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ChangeBranchView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            ChangeBranchView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            ChangeBranchView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            BranchImg.CenterYAnchor.ConstraintEqualTo(ChangeBranchView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            BranchImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            BranchImg.LeftAnchor.ConstraintEqualTo(ChangeBranchView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            BranchImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblChangeBranch.CenterYAnchor.ConstraintEqualTo(ChangeBranchView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblChangeBranch.LeftAnchor.ConstraintEqualTo(BranchImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblChangeBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            //#region LanguageView
            //LanguageView.TopAnchor.ConstraintEqualTo(ChangeBranchView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            //LanguageView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            //LanguageView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            //LanguageView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            //LangImg.CenterYAnchor.ConstraintEqualTo(LanguageView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            //LangImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            //LangImg.LeftAnchor.ConstraintEqualTo(LanguageView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            //LangImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            //lblLang.CenterYAnchor.ConstraintEqualTo(LanguageView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            //lblLang.LeftAnchor.ConstraintEqualTo(LangImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            //lblLang.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //#endregion

            #region ContactUSView
            ContactUSView.TopAnchor.ConstraintEqualTo(ChangeBranchView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ContactUSView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            ContactUSView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            ContactUSView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            ContactImg.CenterYAnchor.ConstraintEqualTo(ContactUSView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ContactImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            ContactImg.LeftAnchor.ConstraintEqualTo(ContactUSView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            ContactImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblContact.CenterYAnchor.ConstraintEqualTo(ContactUSView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblContact.LeftAnchor.ConstraintEqualTo(ContactImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblContact.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region TermView
            TermView.TopAnchor.ConstraintEqualTo(ContactUSView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            TermView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            TermView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            TermView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            TermImg.CenterYAnchor.ConstraintEqualTo(TermView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            TermImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            TermImg.LeftAnchor.ConstraintEqualTo(TermView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            TermImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblTerm.CenterYAnchor.ConstraintEqualTo(TermView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblTerm.LeftAnchor.ConstraintEqualTo(TermImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblTerm.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region VersionView
            VersionView.TopAnchor.ConstraintEqualTo(TermView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            VersionView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            VersionView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            VersionView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            VersionImg.CenterYAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            VersionImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            VersionImg.LeftAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            VersionImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblVersion.CenterYAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblVersion.LeftAnchor.ConstraintEqualTo(VersionImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblVersion.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblVersionNo.CenterYAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblVersionNo.RightAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            lblVersionNo.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region LogoutView
            LogoutView.TopAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            LogoutView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            LogoutView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            LogoutView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            LogoutImg.CenterYAnchor.ConstraintEqualTo(LogoutView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            LogoutImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            LogoutImg.LeftAnchor.ConstraintEqualTo(LogoutView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            LogoutImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblLogout.CenterYAnchor.ConstraintEqualTo(LogoutView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblLogout.LeftAnchor.ConstraintEqualTo(LogoutImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblLogout.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region packView
            PackView.BottomAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            PackView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor,-5).Active = true;
            PackView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor,5).Active = true;
            PackView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            PackImg.CenterYAnchor.ConstraintEqualTo(PackView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            PackImg.WidthAnchor.ConstraintEqualTo(34).Active = true;
            PackImg.LeftAnchor.ConstraintEqualTo(PackView.SafeAreaLayoutGuide.LeftAnchor, 70).Active = true;
            PackImg.HeightAnchor.ConstraintEqualTo(34).Active = true;

            lblPack.TopAnchor.ConstraintEqualTo(PackImg.SafeAreaLayoutGuide.TopAnchor).Active = true;
            lblPack.LeftAnchor.ConstraintEqualTo(PackImg.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            lblPack.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblprice.TopAnchor.ConstraintEqualTo(lblPack.SafeAreaLayoutGuide.BottomAnchor,3).Active = true;
            lblprice.LeftAnchor.ConstraintEqualTo(PackImg.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            lblprice.HeightAnchor.ConstraintEqualTo(14).Active = true;
            #endregion
            #endregion

            closeSideBarView.UserInteractionEnabled = true;
            var TapColse = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Close:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            closeSideBarView.AddGestureRecognizer(TapColse);
        }
        async Task LoadUserProfile()
        {

            string branchID = Preferences.Get("Branch", "");
            DataCashingAll.SysBranchId = Convert.ToInt32(branchID);
            if (DataCashingAll.SysBranchId != 0)
            {

                BranchManage branchManage = new BranchManage();
                var result = await branchManage.GetBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                if (result != null)
                {
                    lblBranch.Text = "Branch : " + result.BranchName;
                }
            }
            lblMerchantName.Text = DataCashingAll.MerchantLocal.MerchantID + ", " + DataCashingAll.MerchantLocal.Name;
            string Username = Preferences.Get("User", "");
            lbluserName.Text = "Hi, " + Username ?? "";
            

            ShareSource.Manage.MerchantManage merchantManage = new ShareSource.Manage.MerchantManage();
            merchantlocal = await merchantManage.GetMerchant(DataCashingAll.MerchantId);
            var path = merchantlocal.LogoLocalPath;

            if (!string.IsNullOrEmpty(path))
            {
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                Utils.SetImage(profileImg, Path.Combine(docFolder, path));
            }
            else
            {
                profileImg.Image = UIImage.FromFile("LogoDefault.png");
            }

            //var qrGen = new CIQRCodeGenerator();
            //qrGen.Message = NSData.FromString("sss");
            //qrGen.CorrectionLevel = CIQRCodeErrorCorrectionLevel.M.ToString();
            //var context = CIContext.FromOptions(null);
            //var img = UIImage.FromImage(context.CreateCGImage(qrGen.OutputImage, qrGen.OutputImage.Extent));
            //var  centerTransform = CoreGraphics.CGAffineTransform.MakeTranslation(extent.midX - (image.extent.size.width / 2), extent.midY - (image.extent.size.height / 2))
            //profileImg.Image = img; 
        }

        protected static Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries = 10)
        {
            return Policy.Handle<SQLite.SQLiteException>().WaitAndRetryAsync(numRetries, pollyRetryAttempt).ExecuteAsync(action);

            TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromMilliseconds(Math.Pow(2, attemptNumber));
        }
        void SetupAutoLayout()
        {
            topBar.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            topBar.HeightAnchor.ConstraintEqualTo(59).Active = true;
            topBar.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            topBar.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;

            profileView.TopAnchor.ConstraintEqualTo(logoImg.SafeAreaLayoutGuide.BottomAnchor, 13).Active = true;
            profileView.HeightAnchor.ConstraintEqualTo((((int)View.Frame.Height*27)/100)).Active = true;
            profileView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            profileView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            mainMenuCollectionView.TopAnchor.ConstraintEqualTo(profileView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            mainMenuCollectionView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -25).Active = true;
            mainMenuCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            mainMenuCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;

            //  profileImg.CenterYAnchor.ConstraintEqualTo(profileView.SafeAreaLayoutGuide.CenterYAnchor, -(((int)View.Frame.Height * 27) / 100)/4).Active = true;
            profileImg.CenterYAnchor.ConstraintEqualTo(profileView.SafeAreaLayoutGuide.CenterYAnchor, -25).Active = true;
            profileImg.CenterXAnchor.ConstraintEqualTo(profileView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            //profileImg.HeightAnchor.ConstraintEqualTo(profileView.HeightAnchor,multiplier: 0.5f,100).Active = true;
            profileImg.HeightAnchor.ConstraintEqualTo(100).Active = true;
            profileImg.WidthAnchor.ConstraintEqualTo(100).Active = true;

            menuImg.TopAnchor.ConstraintEqualTo(topBar.SafeAreaLayoutGuide.TopAnchor,21).Active = true;
            menuImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            menuImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            menuImg.LeftAnchor.ConstraintEqualTo(topBar.SafeAreaLayoutGuide.LeftAnchor,14).Active = true;

            logoImg.TopAnchor.ConstraintEqualTo(topBar.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            logoImg.HeightAnchor.ConstraintEqualTo(50).Active = true;
            logoImg.WidthAnchor.ConstraintEqualTo(50).Active = true;
            logoImg.CenterXAnchor.ConstraintEqualTo(topBar.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lbluserName.TopAnchor.ConstraintEqualTo(profileImg.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
           // lbluserName.HeightAnchor.ConstraintEqualTo(19).Active = true;
            lbluserName.CenterXAnchor.ConstraintEqualTo(profileView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblMerchantName.TopAnchor.ConstraintEqualTo(lbluserName.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
          //  lblMerchantName.HeightAnchor.ConstraintEqualTo(14).Active = true;
            lblMerchantName.CenterXAnchor.ConstraintEqualTo(profileView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblBranch.TopAnchor.ConstraintEqualTo(lblMerchantName.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
          //  lblBranch.HeightAnchor.ConstraintEqualTo(14).Active = true;
            lblBranch.CenterXAnchor.ConstraintEqualTo(profileView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
        }

        #region sidebar
        [Export("Menu:")]
        public void Menu(UIGestureRecognizer sender)
        {
            contentBarView.Hidden = false;
            closeSideBarView.Hidden = false;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
        [Export("Close:")]
        public void Close(UIGestureRecognizer sender)
        {
            contentBarView.Hidden = true;
            closeSideBarView.Hidden = true;
        }
        [Export("Pass:")]
        public void Pass(UIGestureRecognizer sender)
        {

            Utils.SetTitle3(this.NavigationController,"Change Password");
            
            changePage = new ChangePasswordController();
            
            DataCaching.MainNavigation.PushViewController(changePage, false);
        }
        [Export("pack:")]
        public void Pack(UIGestureRecognizer sender)
        {

            // Utils.SetTitle3(this.NavigationController, "Change Password");
            //packpage = new PackageController();
            ////var packpage = new PackageDetailController(); 
            //DataCaching.MainNavigation.PushViewController(packpage, false);
        }
        [Export("ChangeBranch:")]
        public void ChangeBranch(UIGestureRecognizer sender)
        {
            Utils.SetTitle3(this.NavigationController,"Change Branch");
            //if (ChangeBranchPage == null)
            //{
                ChangeBranchPage = new ChangeBranchController();
            //}
            DataCaching.MainNavigation.PushViewController(ChangeBranchPage, false);
        }
        [Export("Lang:")]
        public void Langs(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController,"Language Setting");
            if (langPage == null)
            {
                langPage = new LanguageSettingController();
            }
            DataCaching.MainNavigation.PushViewController(langPage, false);
        }
        [Export("Contact:")]
        public void Contact(UIGestureRecognizer sender)
        {
            
            //Utils.SetTitle(this.NavigationController, "Contact Us");

            //if (contactPage == null)
            //{
            //    contactPage = new SeniorContactController();
            //}
            //DataCaching.MainNavigation.PushViewController(contactPage, false);
        }
        [Export("Term:")]
        public void Term(UIGestureRecognizer sender)
        {
            //TermPage
            Utils.SetTitle(this.NavigationController,"Terms of Service & Privacy Policy");
            if (TermPage == null)
            {
                TermPage = new TermSettingController(false);
            }
            DataCaching.MainNavigation.PushViewController(TermPage, false);
        }
        [Export("Logout:")]
        public void Logout(UIGestureRecognizer sender)
        {
            var okCancelAlertController = UIAlertController.Create("", "Are you sure you want to exit the application?", UIAlertControllerStyle.Alert);
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                alert => logOutSetting()));
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
            PresentViewController(okCancelAlertController, true, null);
        }
        public async void logOutSetting()
        {
            Preferences.Set("AppState", "logout");
            Preferences.Set("Branch", "");
            POSController.tranWithDetails = null; 
            await BellNotificationHelper.UnRegisterBellNotification(GabanaAPI.gbnJWT);

            SplashLoadingController SplashLoading = new SplashLoadingController();
            UIWindow uIWindowRoot = UIApplication.SharedApplication.Windows.First();
            uIWindowRoot.RootViewController = SplashLoading;
        }
        #endregion

        private async Task<bool> CheckExpireDate()
        {
            try
            {
                GabanaInfo gabanaInfo = new GabanaInfo();
                gabanaInfo = await GabanaAPI.GetDataGabanaInfo();
                DataCashingAll.GetGabanaInfo = gabanaInfo;
                var GabanaInfo = JsonConvert.SerializeObject(gabanaInfo);
                Preferences.Set("GabanaInfo", GabanaInfo);

                //ActiveUntilDate = null
                var expire = DataCashingAll.GetGabanaInfo?.ActiveUntilDate;
                if ((expire != null) && (DateTime.Now.Date > expire.Value.Date)) //datenow > expire 19/10 > 07/11
                {
                    return true; //หมดอายุแล้ว
                }
                else
                {
                    return false; //ยังไม่หมดอยุ
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckExpireDate at main");
                return true;
            }
        }

        async Task GetmerchantConfig()
        {
            try
            {
                List<MerchantConfig> lstconfig = new List<MerchantConfig>();
                SetMerchantConfig setconfig = new SetMerchantConfig();
                MerchantConfigManage merchantconfigManage = new MerchantConfigManage();


                List<ORM.Master.MerchantConfig> listmerchantConfig = new List<ORM.Master.MerchantConfig>();
                listmerchantConfig = await GabanaAPI.GetDataMerchantConfig();
                if (listmerchantConfig == null)
                {
                    return;
                }
                if (listmerchantConfig.Count > 0)
                {
                    foreach (var item in listmerchantConfig)
                    {
                        MerchantConfig config = new MerchantConfig()
                        {
                            MerchantID = item.MerchantID,
                            CfgKey = item.CfgKey,
                            CfgInteger = item.CfgInteger,
                            CfgFloat = item.CfgFloat,
                            CfgString = item.CfgString,
                            CfgDate = item.CfgDate
                        };
                        var InsertorReplace = await merchantconfigManage.InsertorReplacrMerchantConfig(config);
                        if (InsertorReplace)
                        {
                            lstconfig.Add(config);
                        }
                    }

                    #region merchantConfig
                    var TAXTYPE = lstconfig.Where(x => x.CfgKey == "TAXTYPE").FirstOrDefault();
                    if (TAXTYPE != null)
                    {
                        setconfig.TAXTYPE = TAXTYPE.CfgString;
                    }

                    var TAXRATE = lstconfig.Where(x => x.CfgKey == "TAXRATE").FirstOrDefault();
                    if (TAXRATE != null)
                    {
                        if (TAXRATE.CfgFloat != null)
                        {
                            setconfig.TAXRATE = TAXRATE.CfgFloat.ToString();
                        }
                    }

                    var CURRENCY_SYMBOLS = lstconfig.Where(x => x.CfgKey == "CURRENCY_SYMBOLS").FirstOrDefault();
                    if (CURRENCY_SYMBOLS != null)
                    {
                        setconfig.CURRENCY_SYMBOLS = CURRENCY_SYMBOLS.CfgString;
                    }

                    var DECIMAL_POINT_CALC = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_CALC").FirstOrDefault();
                    if (DECIMAL_POINT_CALC != null)
                    {
                        if (DECIMAL_POINT_CALC.CfgInteger != null)
                        {
                            setconfig.DECIMAL_POINT_CALC = DECIMAL_POINT_CALC.CfgInteger.ToString();
                        }
                    }

                    var DECIMAL_POINT_DISPLAY = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_DISPLAY").FirstOrDefault();
                    if (DECIMAL_POINT_DISPLAY != null)
                    {
                        if (DECIMAL_POINT_DISPLAY.CfgInteger != null)
                        {
                            setconfig.DECIMAL_POINT_DISPLAY = DECIMAL_POINT_DISPLAY.CfgInteger.ToString();
                        }
                    }

                    var OPTION_ROUNDING = lstconfig.Where(x => x.CfgKey == "OPTION_ROUNDING").FirstOrDefault();
                    if (OPTION_ROUNDING != null)
                    {
                        setconfig.OPTION_ROUNDING_STRING = OPTION_ROUNDING.CfgString;
                        if (OPTION_ROUNDING.CfgInteger != null)
                        {
                            setconfig.OPTION_ROUNDING_INT = OPTION_ROUNDING.CfgInteger.ToString();
                        }
                    }

                    var SERVICECHARGE_TYPE = lstconfig.Where(x => x.CfgKey == "SERVICECHARGE_TYPE").FirstOrDefault();
                    if (SERVICECHARGE_TYPE != null)
                    {
                        setconfig.SERVICECHARGE_TYPE = SERVICECHARGE_TYPE.CfgString;
                    }

                    var SERVICECHARGE_RATE = lstconfig.Where(x => x.CfgKey == "SERVICECHARGE_RATE").FirstOrDefault();
                    if (SERVICECHARGE_RATE != null)
                    {
                        setconfig.SERVICECHARGE_RATE = SERVICECHARGE_RATE.CfgString;
                    }

                    var PRINTER_DEFAULT = lstconfig.Where(x => x.CfgKey == "PRINTER_DEFAULT").FirstOrDefault();
                    if (PRINTER_DEFAULT != null)
                    {
                        setconfig.PRINTER_DEFAULT = PRINTER_DEFAULT.CfgString;
                    }

                    var SUBSCRIPTION_TYPE = lstconfig.Where(x => x.CfgKey == "SUBSCRIPTION_TYPE").FirstOrDefault();
                    if (SUBSCRIPTION_TYPE != null)
                    {
                        setconfig.SUBSCRIPTION_TYPE = SUBSCRIPTION_TYPE.CfgString;
                    }

                    #endregion

                    var merchantConfig = JsonConvert.SerializeObject(setconfig);
                    Preferences.Set("SetmerchantConfig", merchantConfig);
                    var setmerchantConfig = Preferences.Get("SetmerchantConfig", "");
                    if (setmerchantConfig != "")
                    {
                        var Config = JsonConvert.DeserializeObject<SetMerchantConfig>(setmerchantConfig);
                        DataCashingAll.setmerchantConfig = Config;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetmerchantConfig");
                Log.Error("connecterror", "GetmerchantConfig : " + ex.Message);
                throw;
            }
        }
    }
}