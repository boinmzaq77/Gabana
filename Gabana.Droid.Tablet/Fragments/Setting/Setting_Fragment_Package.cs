using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using LinqToDB.SqlQuery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_Package : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_Package NewInstance()
        {
            Setting_Fragment_Package frag = new Setting_Fragment_Package();
            return frag;
        }

        View view;
        public static Setting_Fragment_Package fragment_package;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_package, container, false);
            try
            {
                CheckJwt();
                CombineUI();
                Intent serviceIntent = new Intent("com.android.vending.billing.InAppBillingSevice.BIND");
                serviceIntent.SetPackage("com.android.vending");
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }

        LinearLayout lnBack;
        Button btnContact, btnChangeePackage;
        string LoginType, SubscripttionType;
        TextView txtPackageName, textDetailBranch, textDetailUser, textPrice, textFree, txtPayby, txtDueDate, textExpireDate;
        private async void CombineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += ImagebtnBack_Click;
            btnChangeePackage = view.FindViewById<Button>(Resource.Id.btnChangeePackage);
            btnChangeePackage.Click += BtnChangePackage_Click;
            btnContact = view.FindViewById<Button>(Resource.Id.btnContact);
            btnContact.Click += (sender, e) =>
            {
                var uri = Android.Net.Uri.Parse("tel:026925899");
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            };

            txtPackageName = view.FindViewById<TextView>(Resource.Id.txtPackageName);
            textDetailBranch = view.FindViewById<TextView>(Resource.Id.textDetailBranch);
            textDetailUser = view.FindViewById<TextView>(Resource.Id.textDetailUser);
            textPrice = view.FindViewById<TextView>(Resource.Id.textPrice);
            textFree = view.FindViewById<TextView>(Resource.Id.textFree);
            txtPayby = view.FindViewById<TextView>(Resource.Id.txtPayby);
            txtDueDate = view.FindViewById<TextView>(Resource.Id.txtDueDate);
            textExpireDate = view.FindViewById<TextView>(Resource.Id.textExpireDate);

            LoginType = Preferences.Get("LoginType", "");

            await SetDetailData();
        }

        private async Task SetDetailData()
        {
            try
            {
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
               
                if (await GabanaAPI.CheckNetWork())
                {
                    await GetGabanaInfo();
                }
                else
                {
                    string gabanaInfo = Preferences.Get("GabanaInfo", "");
                    GabanaInfo GabanaInfo = JsonConvert.DeserializeObject<GabanaInfo>(gabanaInfo);
                    DataCashingAll.GetGabanaInfo = GabanaInfo;
                }
                SetDetailPackage();
            }
            catch (Exception)
            {

                throw;
            }
        }

        GabanaInfo gabanaInfo = new GabanaInfo();
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

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
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
        private async void ImagebtnBack_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }
        private async void BtnChangePackage_Click(object sender, EventArgs e)
        {
            var checkNet = await GabanaAPI.CheckNetWork();
            if (checkNet)
            {
                bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "merchant");
                if (check)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(PackageActivity)));
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
            }
        }

        public override async void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                await SetDetailData();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

    }
}