using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
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
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SelectCustomerActivity : AppCompatActivity
    {
        RecyclerView recyclerview_listcustomer;
        RecyclerView.LayoutManager mLayoutManager;
        CustomerManage CustomerManage = new CustomerManage();
        SelectCustomer_Adapter_Main Customer_Adapter_Main;

        ImageButton btnAddCustomer, imgSelect, btnSearchCustomer;
        LinearLayout lnBack, lnAddCustomer, btnBack;
        Button btnApply;
        List<Customer> listCustomer, listSearchCustomer;
        ListCustomer lstCustomer;
        ListCustomer lstSerchCustomer;

        public static long? selectCustomer;
        public static TranWithDetailsLocal tranWithDetails;
        Customer SelectDataCustomer;
        public static string PageOpen;
        EditText textSearchCustomer;
        private string SearchName;
        public static bool checkNet = false;
        DialogLoading dialogLoading = new DialogLoading();
        LinearLayout lnNoDataSearch, lnNoCustomer;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.selectcustomer_activity_main);

                btnBack = FindViewById<LinearLayout>(Resource.Id.btnBack);
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += BtnBack_Click;
                btnBack.Click += BtnBack_Click;
                btnAddCustomer = FindViewById<ImageButton>(Resource.Id.btnAddCustomer);
                imgSelect = FindViewById<ImageButton>(Resource.Id.imgSelect);
                btnApply = FindViewById<Button>(Resource.Id.btnCancel);
                btnApply.Click += BtnApply_Click;
                lnAddCustomer = FindViewById<LinearLayout>(Resource.Id.lnAddCustomer);
                lnNoDataSearch = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
                lnNoCustomer = FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
                lnAddCustomer.Click += BtnAddCustomer_Click;
                btnAddCustomer.Click += BtnAddCustomer_Click;
                recyclerview_listcustomer = FindViewById<RecyclerView>(Resource.Id.recyclerview_listcustomer);


                btnSearchCustomer = FindViewById<ImageButton>(Resource.Id.btnSearchCustomer);
                textSearchCustomer = FindViewById<EditText>(Resource.Id.textSearchCustomer);
                btnSearchCustomer.Click += BtnSearchCustomer_Click;
                textSearchCustomer.TextChanged += TextSearchCustomer_TextChanged;
                textSearchCustomer.KeyPress += TextSearchCustomer_KeyPress;
                textSearchCustomer.FocusChange += TextSearchCustomer_FocusChange;

                CheckJwt();
                if (DataCashing.SysCustomerID != null)
                {
                    selectCustomer = DataCashing.SysCustomerID;
                }
                else
                {
                    selectCustomer = 999;
                }
                SetBtnApply();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at SelectCustomer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TextSearchCustomer_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
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
                    textSearchCustomer.Text += input;
                    textSearchCustomer.SetSelection(textSearchCustomer.Text.Length);
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        private async void SetFilterCustomerData()
        {
            try
            {
                listSearchCustomer = new List<Customer>();
                listSearchCustomer = listCustomer.Where(x => x.CustomerName.ToLower().Contains(SearchName.ToLower()) | (x.Mobile != null && x.Mobile.Contains(SearchName)) | (x.CustomerID != null && x.CustomerID.Contains(SearchName))).ToList();
                if (listSearchCustomer.Count > 0)
                {
                    listSearchCustomer = listSearchCustomer.OrderBy(x => x.CustomerName).ToList();
                }
                lstSerchCustomer = new ListCustomer(listSearchCustomer);

                SelectCustomer_Adapter_Main selectCustomer_adapter_serarch = new SelectCustomer_Adapter_Main(lstSerchCustomer, checkNet);
                mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listcustomer.HasFixedSize = true;
                recyclerview_listcustomer.SetLayoutManager(mLayoutManager);
                recyclerview_listcustomer.SetAdapter(selectCustomer_adapter_serarch);
                selectCustomer_adapter_serarch.ItemClick += SelectCustomer_adapter_serarch_ItemClick; ;

                if (selectCustomer_adapter_serarch.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(SearchName))
                    {
                        lnNoDataSearch.Visibility = ViewStates.Visible;
                        lnNoCustomer.Visibility = ViewStates.Gone;
                        recyclerview_listcustomer.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearch.Visibility = ViewStates.Gone;
                        lnNoCustomer.Visibility = ViewStates.Visible;
                        recyclerview_listcustomer.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    lnNoCustomer.Visibility = ViewStates.Gone;
                    recyclerview_listcustomer.Visibility = ViewStates.Visible;
                }


                SetBtnSearch();
                SetBtnApply();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterCustomerData at Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SelectCustomer_adapter_serarch_ItemClick(object sender, int e)
        {
            if (lstSerchCustomer[e].SysCustomerID == DataCashing.SysCustomerID)
            {
                SelectDataCustomer = null;
                selectCustomer = lstSerchCustomer[e].SysCustomerID;
            }
            else
            {
                SelectDataCustomer = listCustomer[e];
                selectCustomer = lstSerchCustomer[e].SysCustomerID;
            }

            lstSerchCustomer = new ListCustomer(listSearchCustomer);
            SelectCustomer_Adapter_Main selectCustomer_adapter_serarch = new SelectCustomer_Adapter_Main(lstSerchCustomer, checkNet);
            mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            recyclerview_listcustomer.HasFixedSize = true;
            recyclerview_listcustomer.SetLayoutManager(mLayoutManager);
            recyclerview_listcustomer.SetAdapter(selectCustomer_adapter_serarch);
            selectCustomer_adapter_serarch.ItemClick += SelectCustomer_adapter_serarch_ItemClick; ;

            if (e > 6)
            {
                recyclerview_listcustomer.ScrollToPosition(e);
            }

            if (selectCustomer_adapter_serarch.ItemCount > 0)
            {
                //lnNoCustomer.Visibility = ViewStates.Gone;
                recyclerview_listcustomer.Visibility = ViewStates.Visible;
            }
            else
            {
                //lnNoCustomer.Visibility = ViewStates.Visible;
                recyclerview_listcustomer.Visibility = ViewStates.Gone;
            }
            SetBtnSearch();
            SetBtnApply();
        }
        private void TextSearchCustomer_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearchCustomer.Text.Trim()))
            {
                btnSearchCustomer.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearchCustomer.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }
        private async void TextSearchCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchName = textSearchCustomer.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                await SetDataCustomer();
            }
            SetBtnSearch();
        }
        private async void BtnSearchCustomer_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            await SetDataCustomer();
            textSearchCustomer.ClearFocus();
        }
        private void SetClearSearchText()
        {
            SearchName = "";
            textSearchCustomer.Text = string.Empty;
            SetBtnSearch();
        }
        private void SetBtnSearch()
        {
            if (string.IsNullOrEmpty(SearchName))
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
        private void SetBtnApply()
        {
            if (DataCashing.SysCustomerID == selectCustomer)
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            else
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }

            if (SelectDataCustomer == null && DataCashing.SysCustomerID == 999)
            {
                btnApply.Text = GetString(Resource.String.textcancle);
            }
            else if (SelectDataCustomer == null && DataCashing.SysCustomerID != 999)
            {
                btnApply.Text = GetString(Resource.String.selectcustomer_activity_remove);
            }
            else
            {
                btnApply.Text = GetString(Resource.String.selectcustomer_activity_apply);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        private async void BtnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectDataCustomer == null && DataCashing.SysCustomerID != 999)
                {
                    DataCashing.SysCustomerID = 999;
                }
                else
                {

                    DataCashing.SysCustomerID = selectCustomer;
                }
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnApply_Click at SelectCustomer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnAddCustomer_Click(object sender, EventArgs e)
        {
            var LoginType = Preferences.Get("LoginType", "");
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "customer");
            if (check)
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(AddCustomerActivity)));
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
            }
        }

        async Task SetDataCustomer()
        {
            try
            {
                listCustomer = await GetListCustomer();
                lstCustomer = new ListCustomer(listCustomer);
                mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listcustomer.HasFixedSize = true;
                recyclerview_listcustomer.SetLayoutManager(mLayoutManager);
                recyclerview_listcustomer.SetItemViewCacheSize(listCustomer.Count + 1);
                Customer_Adapter_Main = new SelectCustomer_Adapter_Main(lstCustomer, checkNet);
                recyclerview_listcustomer.SetAdapter(Customer_Adapter_Main);
                Customer_Adapter_Main.ItemClick += Customer_Adapter_Main_ItemClick;

                if (Customer_Adapter_Main.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(SearchName))
                    {
                        lnNoDataSearch.Visibility = ViewStates.Visible;
                        lnNoCustomer.Visibility = ViewStates.Gone;
                        recyclerview_listcustomer.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearch.Visibility = ViewStates.Gone;
                        lnNoCustomer.Visibility = ViewStates.Visible;
                        recyclerview_listcustomer.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    lnNoCustomer.Visibility = ViewStates.Gone;
                    recyclerview_listcustomer.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetDataCustomer at SelectCustomer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async void Customer_Adapter_Main_ItemClick(object sender, int e)
        {
            try
            {
                if (listCustomer[e].SysCustomerID == DataCashing.SysCustomerID)
                {
                    SelectDataCustomer = null;
                    selectCustomer = listCustomer[e].SysCustomerID;
                }
                else
                {
                    SelectDataCustomer = listCustomer[e];
                    selectCustomer = listCustomer[e].SysCustomerID;
                }
                mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listcustomer.HasFixedSize = true;
                recyclerview_listcustomer.SetLayoutManager(mLayoutManager);
                recyclerview_listcustomer.SetItemViewCacheSize(listCustomer.Count + 1);
                Customer_Adapter_Main = new SelectCustomer_Adapter_Main(lstCustomer, checkNet);
                recyclerview_listcustomer.SetAdapter(Customer_Adapter_Main);
                Customer_Adapter_Main.ItemClick += Customer_Adapter_Main_ItemClick;
                if (e > 6)
                {
                    recyclerview_listcustomer.ScrollToPosition(e);
                }
                SetBtnApply();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Customer_Adapter_Main_ItemClick at SelectCustomer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        async Task<List<Customer>> GetListCustomer()
        {
            try
            {
                listCustomer = new List<Customer>();
                CustomerManage customerManage = new CustomerManage();
                listCustomer = await customerManage.GetAllCustomer();
                if (listCustomer == null)
                {
                    Toast.MakeText(this, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
                    return null;
                }
                Log.Debug("Customer", JsonConvert.SerializeObject(listCustomer));
                return listCustomer;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListCustomer at SelectCustomer");
                Console.WriteLine(ex.Message);
                Log.Debug("error", ex.Message);
                return null;
            }
        }
        protected async override void OnResume()
        {
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                base.OnResume();

                CheckJwt();
                checkNet = await GabanaAPI.CheckSpeedConnection();
                
                //await GetListCustomer();
                await SetDataCustomer();
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

                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at SelectCustomer");
                base.OnRestart();

            }
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'SelectCustomerActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'SelectCustomerActivity.openPage' is assigned but its value is never used
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

