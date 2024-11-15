using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Newtonsoft.Json;
using Plugin.InAppBilling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class SettingPackageActivity : AppCompatActivity
    {
        public static SettingPackageActivity setting;
        DialogLoading dialogLoading = new DialogLoading();
        List<PackageProduce> packages = new List<PackageProduce>();
        public static int PackageIdSelected;
        Button btnContact, btnChangeePackage;
        TextView txtPackageName, textDetailBranch, textDetailUser, textPrice, textFree, txtPayby, txtDueDate, textExpireDate;
        string LoginType, SubscripttionType;
        GabanaInfo gabanaInfo = new GabanaInfo();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.package_activity_setting);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += ImagebtnBack_Click;
                setting = this;
                Intent serviceIntent = new Intent("com.android.vending.billing.InAppBillingSevice.BIND");
                serviceIntent.SetPackage("com.android.vending");
                btnChangeePackage = FindViewById<Button>(Resource.Id.btnChangeePackage);
                btnChangeePackage.Click += BtnChangePackage_Click;
                btnContact = FindViewById<Button>(Resource.Id.btnContact);
                btnContact.Click += (sender, e) =>
                {
                    var uri = Android.Net.Uri.Parse("tel:026925899");
                    var intent = new Intent(Intent.ActionView, uri);
                    StartActivity(intent);
                };
                txtPackageName = FindViewById<TextView>(Resource.Id.txtPackageName);
                textDetailBranch = FindViewById<TextView>(Resource.Id.textDetailBranch);
                textDetailUser = FindViewById<TextView>(Resource.Id.textDetailUser);
                textPrice = FindViewById<TextView>(Resource.Id.textPrice);
                textFree = FindViewById<TextView>(Resource.Id.textFree);
                txtPayby = FindViewById<TextView>(Resource.Id.txtPayby);
                txtDueDate = FindViewById<TextView>(Resource.Id.txtDueDate);
                textExpireDate = FindViewById<TextView>(Resource.Id.textExpireDate);

                LoginType = Preferences.Get("LoginType", "");

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }


                txtDueDate.Text = DataCashingAll.Merchant.Merchant.DueDate.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                DateTime expire = DataCashingAll.GetGabanaInfo?.ActiveUntilDate ?? DateTime.MinValue;
                textExpireDate.Text = expire.ToString("dd/MM/yyyy", new CultureInfo("en-US"));

                SubscripttionType = DataCashingAll.setmerchantConfig.SUBSCRIPTION_TYPE;
                switch (SubscripttionType)
                {
                    case "P":
                        txtPayby.Text = "Play Store";
                        break;
                    case "A":
                        txtPayby.Text = "App Store";
                        break;
                    case "F":
                        txtPayby.Text = "Free";
                        break;
                    case "U":
                        txtPayby.Text = "Unscription";
                        break;
                    case "B":
                        txtPayby.Text = "Backend";
                        break;
                    default:
                        break;
                }

                CheckJwt();

                if (!await GabanaAPI.CheckNetWork() || !await GabanaAPI.CheckSpeedConnection())
                {
                    string gabanaInfo = Preferences.Get("GabanaInfo", "");
                    GabanaInfo GabanaInfo = JsonConvert.DeserializeObject<GabanaInfo>(gabanaInfo);
                    DataCashingAll.GetGabanaInfo = GabanaInfo;
                }

                await GetGabanaInfo();
                SetDetailPackage();

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                    dialogLoading = new DialogLoading();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                dialogLoading = new DialogLoading();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SettingPackageActivity");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async Task GetGabanaInfo()
        {
            try
            {
                gabanaInfo = await GabanaAPI.GetDataGabanaInfo();
                DataCashingAll.GetGabanaInfo = gabanaInfo;
                var GabanaInfo = JsonConvert.SerializeObject(gabanaInfo);
                Preferences.Set("GabanaInfo", GabanaInfo);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetCloudProductLicence at Package");
            }
        }

        private async void BtnChangePackage_Click(object sender, EventArgs e)
        {
            var checkNet = await GabanaAPI.CheckSpeedConnection();
            if (checkNet)
            {
                bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "merchant");
                if (check)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(PackageActivity)));
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
            }
        }

        private void ImagebtnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        private void SetDetailPackage()
        {
            try
            {
                int PackageIDCurrent = 1;
                PackageIDCurrent = Utils.SetPackageID(DataCashingAll.GetGabanaInfo.TotalBranch, DataCashingAll.GetGabanaInfo.TotalUser);
                List<string> detail = Utils.SetDetailPackage(PackageIDCurrent.ToString());
                switch (SubscripttionType)
                {
                    case "P":
                        textFree.Visibility = Android.Views.ViewStates.Gone;
                        txtPackageName.Text = GetString(Resource.String.settingpackage_activity_title) + " : "
                                            + detail[1] + " " + GetString(Resource.String.branch) + " "
                                            + detail[0] + " " + GetString(Resource.String.package_activity_user);
                        textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " "
                                            + detail[1] + " " + GetString(Resource.String.branch);
                        textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " "
                                            + detail[0] + " " + GetString(Resource.String.package_activity_user);
                        textPrice.Text = detail[2];
                        break;
                    case "A":
                        textFree.Visibility = Android.Views.ViewStates.Gone;
                        txtPackageName.Text = GetString(Resource.String.settingpackage_activity_title) + " : "
                                            + detail[1] + " " + GetString(Resource.String.branch) + " "
                                            + detail[0] + " " + GetString(Resource.String.package_activity_user);
                        textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " "
                                            + detail[1] + " " + GetString(Resource.String.branch);
                        textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " "
                                            + detail[0] + " " + GetString(Resource.String.package_activity_user);
                        textPrice.Text = detail[2];
                        break;
                    case "F":
                        txtPackageName.Text = "Package : 1Branch/5Users (Free)";
                        textPrice.PaintFlags = Android.Graphics.PaintFlags.StrikeThruText;
                        textPrice.SetTextColor(Android.Graphics.Color.ParseColor("#8AD3F5"));
                        textFree.Visibility = Android.Views.ViewStates.Visible;
                        textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " 1 " + GetString(Resource.String.branch);
                        textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " 5 " + GetString(Resource.String.package_activity_user);
                        break;
                    case "U":
                        textFree.Visibility = Android.Views.ViewStates.Gone;
                        txtPackageName.Text = GetString(Resource.String.settingpackage_activity_title) + " : "
                                            + detail[1] + " " + GetString(Resource.String.branch) + " "
                                            + detail[0] + " " + GetString(Resource.String.package_activity_user);
                        textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " "
                                            + detail[1] + " " + GetString(Resource.String.branch);
                        textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " "
                                            + detail[0] + " " + GetString(Resource.String.package_activity_user);
                        textPrice.Text = detail[2];
                        break;
                    case "B":
                        //รายละเอียดจาก gabanaInfo
                        textFree.Visibility = Android.Views.ViewStates.Gone;
                        textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " "
                                            + DataCashingAll.GetGabanaInfo.TotalBranch + " " + GetString(Resource.String.branch);
                        textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " "
                                            + DataCashingAll.GetGabanaInfo.TotalUser + " " + GetString(Resource.String.package_activity_user);
                        break;
                    default:
                        txtPackageName.Text = "Package : 1Branch/5Users (Free)";
                        textPrice.PaintFlags = Android.Graphics.PaintFlags.StrikeThruText;
                        textPrice.SetTextColor(Android.Graphics.Color.ParseColor("#8AD3F5"));
                        textFree.Visibility = Android.Views.ViewStates.Visible;
                        textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " 1 " + GetString(Resource.String.branch);
                        textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " 5 " + GetString(Resource.String.package_activity_user);
                        break;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDetailPackageOnDB");
            }
        }

        private async Task GetDetailPackage()
        {
            try
            {
                if (!await MakePurchase())
                {
                    return;
                }
                var ProductId = new string[]
                {
                "1",
                "2",
                "3",
                "4"
                };
                var connect = await CrossInAppBilling.Current.ConnectAsync();
                if (!connect)
                {
                    return;
                }

                var purchaseHistory = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.Subscription);
                var products = await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.Subscription, ProductId);
                InAppBillingPurchase inAppBillingPurchase = new InAppBillingPurchase();
                inAppBillingPurchase = purchaseHistory.Where(x => x.State == Plugin.InAppBilling.PurchaseState.Purchased).OrderByDescending(x => x.TransactionDateUtc).FirstOrDefault();
                var verify = await CrossInAppBilling.Current.FinishTransaction(inAppBillingPurchase);

                if (inAppBillingPurchase == null)
                {
                    int PackageIDCurrent = 1;
                    PackageIDCurrent = Utils.SetPackageID(DataCashingAll.GetGabanaInfo.TotalBranch, DataCashingAll.GetGabanaInfo.TotalUser);
                    List<string> detail = Utils.SetDetailPackage(PackageIDCurrent.ToString());
                    switch (SubscripttionType)
                    {
                        case "P":
                            textFree.Visibility = Android.Views.ViewStates.Gone;
                            txtPackageName.Text = GetString(Resource.String.settingpackage_activity_title) + " : "
                                                + detail[1] + " " + GetString(Resource.String.branch) + " "
                                                + detail[0] + " " + GetString(Resource.String.package_activity_user);
                            textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " "
                                                + detail[1] + " " + GetString(Resource.String.branch);
                            textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " "
                                                + detail[0] + " " + GetString(Resource.String.package_activity_user);
                            textPrice.Text = detail[2];
                            break;
                        case "A":
                            textFree.Visibility = Android.Views.ViewStates.Gone;
                            txtPackageName.Text = GetString(Resource.String.settingpackage_activity_title) + " : "
                                                + detail[1] + " " + GetString(Resource.String.branch) + " "
                                                + detail[0] + " " + GetString(Resource.String.package_activity_user);
                            textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " "
                                                + detail[1] + " " + GetString(Resource.String.branch);
                            textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " "
                                                + detail[0] + " " + GetString(Resource.String.package_activity_user);
                            textPrice.Text = detail[2];
                            break;
                        case "F":
                            txtPackageName.Text = "Package : 1Branch/5Users (Free)";
                            textPrice.PaintFlags = Android.Graphics.PaintFlags.StrikeThruText;
                            textPrice.SetTextColor(Android.Graphics.Color.ParseColor("#8AD3F5"));
                            textFree.Visibility = Android.Views.ViewStates.Visible;
                            textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " 1 " + GetString(Resource.String.branch);
                            textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " 5 " + GetString(Resource.String.package_activity_user);
                            break;
                        case "U":
                            txtPackageName.Text = "Package : 1Branch/5Users (Free)";
                            textPrice.PaintFlags = Android.Graphics.PaintFlags.StrikeThruText;
                            textPrice.SetTextColor(Android.Graphics.Color.ParseColor("#8AD3F5"));
                            textFree.Visibility = Android.Views.ViewStates.Visible;
                            textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " 1 " + GetString(Resource.String.branch);
                            textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " 5 " + GetString(Resource.String.package_activity_user);
                            break;
                        case "B":
                            //รายละเอียดจาก gabanaInfo
                            textFree.Visibility = Android.Views.ViewStates.Gone;
                            textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " "
                                                + DataCashingAll.GetGabanaInfo.TotalBranch + " " + GetString(Resource.String.branch);
                            textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " "
                                                + DataCashingAll.GetGabanaInfo.TotalUser + " " + GetString(Resource.String.package_activity_user);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    List<string> detail = Utils.SetDetailPackage(inAppBillingPurchase.ProductId);
                    textFree.Visibility = Android.Views.ViewStates.Gone;
                    txtPackageName.Text = GetString(Resource.String.settingpackage_activity_title) + " : "
                                        + detail[1] + " " + GetString(Resource.String.branch) + " "
                                        + detail[0] + " " + GetString(Resource.String.package_activity_user);
                    textDetailBranch.Text = GetString(Resource.String.settingpackage_activity_text9) + " "
                                        + detail[1] + " " + GetString(Resource.String.branch);
                    textDetailUser.Text = GetString(Resource.String.settingpackage_activity_text10) + " "
                                        + detail[0] + " " + GetString(Resource.String.package_activity_user);
                    textPrice.Text = products.Where(x => x.ProductId == inAppBillingPurchase.ProductId).Select(x => x.OriginalPrice).FirstOrDefault().ToString();
                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                Toast.MakeText(this, purchaseEx.Message, ToastLength.Short).Show();
                return;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("GetDetailPackage");
            }
        }

        private async Task<bool> MakePurchase()
        {
            if (!CrossInAppBilling.IsSupported)
            {
                return false;
            }
            using (var billing = CrossInAppBilling.Current)
            {
                try
                {
                    var connected = await billing.ConnectAsync();
                    if (!connected)
                        return false;
                    else
                    {
                        return true;
                    }
                }
                finally
                {
                    await billing.DisconnectAsync();
                }
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("error OnResume at SettingPackage");
            }
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'SettingPackageActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'SettingPackageActivity.openPage' is assigned but its value is never used
        public DateTime pauseDate = DateTime.Now;
        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }

                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at changePass");
            }
        }

        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (deviceAsleep)
            {
                deviceAsleep = false;
                TimeSpan span = DateTime.Now.Subtract(pauseDate);

                long DISCONNECT_TIMEOUT = 5 * 60 * 1000; // 1 min = 1 * 60 * 1000 ms
                if ((span.Minutes * 60 * 1000) >= DISCONNECT_TIMEOUT)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                    this.Finish();
                    return;
                }
                else
                {
                    pauseDate = DateTime.Now;
                }
            }
            else
            {
                pauseDate = DateTime.Now;

            }
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }

        }
    }
}

