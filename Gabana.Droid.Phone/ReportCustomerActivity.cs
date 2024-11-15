using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ReportCustomerActivity : AppCompatActivity
    {
        public static ReportCustomerActivity reportcustomeractivity;
        private static ListCustomer listCustomer;
        LinearLayout lnBack;
        RecyclerView mRecycleView;
        private static List<Customer> lstCustomer, listSearchCustomer;
        public static List<Customer> listChooseCustomer = new List<Customer>();
        Button btnAll, btnApply;
        public static string customerSelect;
        ImageButton btnSearchCustomer;
        EditText txtSearchCustomer;
        string searchCustomer;
        public static Customer FocusCustomer;
        LinearLayout lnNoDataSearch;
        Report_Adapter_Customer report_adapter_customer;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.report_activity_customer);
                reportcustomeractivity = this;

                CheckJwt();
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                btnAll = FindViewById<Button>(Resource.Id.btnAll);
                btnAll.Click += BtnAll_Click;
                mRecycleView = FindViewById<RecyclerView>(Resource.Id.recyclerview_listcustomer);
                btnApply = FindViewById<Button>(Resource.Id.btnApply);
                btnApply.Click += BtnApply_Click;
                btnSearchCustomer = FindViewById<ImageButton>(Resource.Id.btnSearchTopping);
                btnSearchCustomer.Click += BtnSearchCustomer_Click;
                txtSearchCustomer = FindViewById<EditText>(Resource.Id.textSearchTopping);
                txtSearchCustomer.TextChanged += TxtSearchCustomer_TextChanged;
                txtSearchCustomer.FocusChange += TxtSearchCustomer_FocusChange;
                txtSearchCustomer.KeyPress += TxtSearchCustomer_KeyPress;
                lnNoDataSearch = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);

                //customerSelect = "All";
                //GetCustomerData();
                SetShowButton();
                _ = TinyInsights.TrackPageViewAsync("OnCreate : ReportCustomerActivity");
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Reportcus");
                Log.Debug("Error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtSearchCustomer_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchCustomer = txtSearchCustomer.Text.Trim();
            if (string.IsNullOrEmpty(searchCustomer))
            {
                SetCustomerData();
            }
            SetBtnSearch();
        }

        private void TxtSearchCustomer_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            SetBtnSearch();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterCustomerData();
            }
            View view = this.CurrentFocus;
            if (view != null)
            {
                if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
                {
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(view.WindowToken, 0);
                }
            }

            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Del)
            {
                e.Handled = false;
            }

            e.Handled = false;
            if (e.Handled)
            {
                string input = string.Empty;
                switch (e.KeyCode)
                {
                    case Keycode.Num0:
                        input += "0";
                        break;
                    case Keycode.Num1:
                        input += "1";
                        break;
                    case Keycode.Num2:
                        input += "2";
                        break;
                    case Keycode.Num3:
                        input += "3";
                        break;
                    case Keycode.Num4:
                        input += "4";
                        break;
                    case Keycode.Num5:
                        input += "5";
                        break;
                    case Keycode.Num6:
                        input += "6";
                        break;
                    case Keycode.Num7:
                        input += "7";
                        break;
                    case Keycode.Num8:
                        input += "8";
                        break;
                    case Keycode.Num9:
                        input += "9";
                        break;
                    default:
                        break;
                }
                //e.Handled = false;
                txtSearchCustomer.Text += input;
                txtSearchCustomer.SetSelection(txtSearchCustomer.Text.Length);
                return;
            }
        }

        private void TxtSearchCustomer_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(txtSearchCustomer.Text.Trim()))
            {
                btnSearchCustomer.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearchCustomer.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }

        private void BtnSearchCustomer_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            SetCustomerData();
        }

        private void SetClearSearchText()
        {
            searchCustomer = "";
            txtSearchCustomer.Text = string.Empty;
        }

        private void SetBtnSearch()
        {
            if (string.IsNullOrEmpty(searchCustomer))
            {
                btnSearchCustomer.SetBackgroundResource(Resource.Mipmap.Search);
                btnSearchCustomer.Enabled = false;
            }
            else
            {
                btnSearchCustomer.SetBackgroundResource(Resource.Mipmap.DelTxt);
                btnSearchCustomer.Enabled = true;
            }
        }
        private async void SetFilterCustomerData()
        {
            try
            {
                listSearchCustomer = new List<Customer>();
                listSearchCustomer = lstCustomer.Where(x => x.CustomerName.ToLower().Contains(searchCustomer.ToLower()) | (x.Mobile != null && x.Mobile.Contains(searchCustomer)) | (x.CustomerID != null && x.CustomerID.Contains(searchCustomer))).ToList();
                if (listSearchCustomer.Count > 0)
                {
                    listSearchCustomer = listSearchCustomer.OrderBy(x => x.CustomerName).ToList();
                }

                listCustomer = new ListCustomer(listSearchCustomer);
                report_adapter_customer = new Report_Adapter_Customer(listCustomer);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleView.SetAdapter(report_adapter_customer);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetLayoutManager(mLayoutManager);
                report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;

                if (report_adapter_customer.ItemCount == 0 && !string.IsNullOrEmpty(searchCustomer))
                {
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                    mRecycleView.Visibility = ViewStates.Gone;

                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    mRecycleView.Visibility = ViewStates.Visible;
                }

                SetBtnSearch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterCustomerData at Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SetShowButton()
        {
            if (customerSelect == "")
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            if (listChooseCustomer.Count > 0)
            {

                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
        }
        private void BtnAll_Click(object sender, EventArgs e)
        {
            if (customerSelect != "All Customer" && customerSelect == "")
            {
                customerSelect = "All Customer";
                listChooseCustomer = new List<Customer>();

                listChooseCustomer = lstCustomer;
            }
            else
            {
                listChooseCustomer = new List<Customer>();
                customerSelect = "";
            }
            SetShowButton();

            SetCustomerData();
        }
        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (customerSelect != string.Empty)
            {
                ReportDailySaleActivity.listChooseCustomer = listChooseCustomer;
                this.Finish();
            }
            else
            {
                Toast.MakeText(this, "กรุณาเลือกลูกค้า", ToastLength.Short).Show();
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        async void GetCustomerData()
        {
            try
            {
                lstCustomer = await GetListCustomer();
                if (listChooseCustomer.Count == 0)
                {
                    listChooseCustomer = lstCustomer;
                }
                SetCustomerData();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetCustomerData at Reportcus");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        async void SetCustomerData()
        {
            try
            {
                listCustomer = new ListCustomer(lstCustomer);
                report_adapter_customer = new Report_Adapter_Customer(listCustomer);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleView.SetAdapter(report_adapter_customer);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetLayoutManager(mLayoutManager);
                report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetCustomerData at Reportcus");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        async Task<List<Customer>> GetListCustomer()
        {
            try
            {
                lstCustomer = new List<Customer>();
                CustomerManage customerManage = new CustomerManage();
                lstCustomer = await customerManage.GetAllCustomer();
                if (lstCustomer == null)
                {
                    Toast.MakeText(this, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
                    return null;
                }
                Log.Debug("Customer", JsonConvert.SerializeObject(listCustomer));
                return lstCustomer;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListCustomer at Reportcus");
                Console.WriteLine(ex.Message);
                Log.Debug("error", ex.Message);
                return null;
            }
        }

        private async void Report_Adapter_Customer_ItemClick(object sender, int e)
        {
            try
            {
                var cusotmer = listCustomer[e];
                FocusCustomer = cusotmer;
                var search = listChooseCustomer.FindIndex(x => x.SysCustomerID == cusotmer.SysCustomerID && x.MerchantID == DataCashingAll.MerchantId);
                if (search == -1)
                {
                    listChooseCustomer.Add(cusotmer);
                }
                else
                {
                    listChooseCustomer.RemoveAt(search);
                }
                customerSelect = "";
                if (listCustomer.Count == listChooseCustomer.Count)
                {
                    customerSelect = "All Customer";
                }
                else
                {
                    foreach (var item in listChooseCustomer)
                    {
                        if (customerSelect != "")
                        {
                            customerSelect += "," + item.CustomerName;
                        }
                        else
                        {
                            customerSelect = item.CustomerName;
                        }
                    }
                }
                SetShowButton();
                lstCustomer = await GetListCustomer();
                SetCustomerData();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Report_Adapter_Customer_ItemClick at Reportcus");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            CheckJwt();
            if (string.IsNullOrEmpty(searchCustomer))
            {
                GetCustomerData();
                SetBtnSearch();
            }
            CustomerFocus();
        }

        internal static void SetSelectCustomer(List<Customer> l, string t)
        {
            listChooseCustomer = l;
            customerSelect = t;
        }

        private void CustomerFocus()
        {
            try
            {
                if (FocusCustomer != null)
                {
                    int index = -1;                   
                    if (lstCustomer != null)
                    {
                        if (lstCustomer.Count == 0)
                        {
                            lstCustomer.Add(FocusCustomer);
                            SetCustomerData();
                            FocusCustomer = null;
                            return;
                        }
                        index = lstCustomer.FindIndex(x => x.SysCustomerID == FocusCustomer.SysCustomerID);
                        if (index == -1)
                        {
                            lstCustomer.RemoveAt(index);
                        }
                        lstCustomer.Insert(0, FocusCustomer);
                    }
                    if (listSearchCustomer?.Count > 0)
                    {
                        index = listSearchCustomer.FindIndex(x => x.SysCustomerID == FocusCustomer.SysCustomerID);
                        if (index == -1)
                        {
                            listSearchCustomer.RemoveAt(index);
                        }
                        listSearchCustomer.Insert(0, FocusCustomer);
                    }

                    report_adapter_customer.NotifyDataSetChanged();
                    FocusCustomer = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemFocus at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'ReportCustomerActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'ReportCustomerActivity.openPage' is assigned but its value is never used
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

