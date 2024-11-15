using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class BillHistoryFilterActivity : AppCompatActivity
    {
        Android.App.DatePickerDialog dialogStartDate, dialogEndDate;
        TextView txtStartDate, txtEndDate;
        private string startDate, endDate, textDate;
        LinearLayout lnStartDate;
        LinearLayout lnEndDate;
        TextView txtItem;
        internal static bool chooseAllItem = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                // Create your application here
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.billhistory_activity_filter);

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                txtStartDate = FindViewById<TextView>(Resource.Id.txtStartDate);
                txtEndDate = FindViewById<TextView>(Resource.Id.txtEndDate);

                CheckJwt();
                DateTime today = DateTime.Now;
                txtStartDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                txtEndDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                startDate = Utils.ChangeDateTimeReport(DateTime.UtcNow);
                lnStartDate = FindViewById<LinearLayout>(Resource.Id.lnStartDate);
                lnStartDate.Click += LnStartDate_Click;
                lnEndDate = FindViewById<LinearLayout>(Resource.Id.lnEndDate);
                lnEndDate.Enabled = false;
                lnEndDate.Click += LnEndDate_Click;
                LinearLayout lnSelectItem = FindViewById<LinearLayout>(Resource.Id.lnSelectItem);
                lnSelectItem.Click += LnSelectItem_Click;
                txtItem = FindViewById<TextView>(Resource.Id.txtItem);
                dialogStartDate = new DatePickerDialog(this, Android.Resource.Style.ThemeHoloLightDialogMinWidth, DatePickerDialog_StartDate,
                                                       today.Year,
                                                       today.Month - 1,
                                                       today.Day);
                dialogStartDate.DatePicker.MinDate = (long)(DataCashingAll.Merchant.Merchant.DateCreated - new DateTime(1970, 1, 1)).TotalMilliseconds;
                dialogStartDate.DatePicker.MaxDate = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds; ;

                dialogEndDate = new DatePickerDialog(this, Android.Resource.Style.ThemeHoloLightDialogMinWidth, DatePickerDialog_EndDate,
                                                        today.Year,
                                                        today.Month - 1,
                                                        today.Day);

                dialogEndDate.DatePicker.MinDate = (long)(DataCashingAll.Merchant.Merchant.DateCreated - new DateTime(1970, 1, 1)).TotalMilliseconds;
                dialogEndDate.DatePicker.MaxDate = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;


                _ = TinyInsights.TrackPageViewAsync("OnCreate : BillHistoryFilterActivity");

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("at ReportDaily");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnSelectItem_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(BillFilterItemActivity)));
        }

        private void LnEndDate_Click(object sender, EventArgs e)
        {
            dialogEndDate.Show();
        }
        private void LnStartDate_Click(object sender, EventArgs e)
        {
            dialogStartDate.Show();
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private void DatePickerDialog_EndDate(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            try
            {
                txtEndDate.Text = e.Date.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                endDate = Utils.ChangeDateTimeReport(e.Date);
                txtEndDate.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                textDate = txtStartDate.Text + "-" + txtEndDate.Text;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DatePickerDialog_StartDate(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            try
            {
                startDate = Utils.ChangeDateTimeReport(e.Date);
                txtStartDate.Text = e.Date.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                if (dialogEndDate.DatePicker.MinDate < (long)(e.Date - new DateTime(1970, 1, 1)).TotalMilliseconds)
                {
                    txtEndDate.Text = e.Date.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                }
                dialogEndDate.DatePicker.MinDate = (long)(e.Date - new DateTime(1970, 1, 1)).TotalMilliseconds;
                if (DateTime.UtcNow >= e.Date.AddMonths(3))
                {
                    dialogEndDate.DatePicker.MaxDate = (long)(e.Date.AddMonths(3) - new DateTime(1970, 1, 1)).TotalMilliseconds;
                }
                else
                {
                    dialogEndDate.DatePicker.MaxDate = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

                }
                lnEndDate.Enabled = true;
                textDate = txtStartDate.Text + "-" + txtEndDate.Text;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        ItemManage itemManage = new ItemManage();
        List<Item> items = new List<Item>();
        async Task GetItemList()
        {
            try
            {
                items = await itemManage.GetAllItem();
                if (items == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    items = new List<Item>();
                }
                //await SetItemData();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine(ex.Message);
                Log.Debug("error", ex.Message);
            }
        }
        public static List<Item> chooseItem = new List<Item>();
        protected override async void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();

                if (chooseAllItem)
                {
                    txtItem.Text = GetString(Resource.String.billfilter_activity_allitem);
                }
                else
                {
                    await GetItemList();
                    txtItem.Text = "";
                    foreach (var item in chooseItem)
                    {
                        if (txtItem.Text != "")
                        {
                            txtItem.Text += "," + item.ItemName;

                        }
                        else
                        {
                            txtItem.Text = item.ItemName;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
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
    }
}