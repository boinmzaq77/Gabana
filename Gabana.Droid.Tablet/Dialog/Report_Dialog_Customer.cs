using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Report;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Report_Dialog_Customer : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Report_Dialog_Customer NewInstance()
        {
            var frag = new Report_Dialog_Customer { Arguments = new Bundle() };
            return frag;
        }
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.report_dialog_customer, container, false);
            try
            {
                CombinUI();
                SetUIEvent();

                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Report Dialog Customer");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }
        public override void OnResume()
        {
            base.OnResume();
            if (string.IsNullOrEmpty(searchCustomer))
            {
                GetCustomerData();
                SetBtnSearch();
            }
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public static string customerSelect;
        public static List<Customer> listChooseCustomer = new List<Customer>();
        private static List<Customer> lstCustomer, listSearchCustomer;

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
        private void SetShowButton()
        {
            if (customerSelect == "")
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            if (listChooseCustomer.Count > 0)
            {

                btnApply.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
        }
        private static ListCustomer listCustomer;
        async void SetCustomerData()
        {
            try
            {
                listCustomer = new ListCustomer(lstCustomer);
                Report_Adapter_ChooseCustomer report_adapter_customer = new Report_Adapter_ChooseCustomer(listCustomer);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListCustomer.SetAdapter(report_adapter_customer);
                rcvListCustomer.HasFixedSize = true;
                rcvListCustomer.SetLayoutManager(mLayoutManager);
                report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetCustomerData at Reportcus");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        public static Customer FocusCustomer;

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
        async Task<List<Customer>> GetListCustomer()
        {
            try
            {
                lstCustomer = new List<Customer>();
                CustomerManage customerManage = new CustomerManage();
                lstCustomer = await customerManage.GetAllCustomer();
                if (lstCustomer == null)
                {
                    Toast.MakeText(this.Activity, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
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

        private void SetUIEvent()
        {
            lnBack.Click += LnBack_Click;
            btnAll.Click += BtnAll_Click;
            btnApply.Click += BtnApply_Click;
            btnSearch.Click += BtnSearchCustomer_Click;
            textSearch.TextChanged += TextSearch_TextChanged; 
            textSearch.FocusChange += TextSearch_FocusChange; 
            textSearch.KeyPress += TextSearch_KeyPress;
            SetShowButton();
        }
        private void SetBtnSearch()
        {
            if (string.IsNullOrEmpty(searchCustomer))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.Search);
                btnSearch.Enabled = false;
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
                btnSearch.Enabled = true;
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
                Report_Adapter_ChooseCustomer report_adapter_customer = new Report_Adapter_ChooseCustomer(listCustomer);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListCustomer.SetAdapter(report_adapter_customer);
                rcvListCustomer.HasFixedSize = true;
                rcvListCustomer.SetLayoutManager(mLayoutManager);
                report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;

                if (report_adapter_customer.ItemCount == 0 && !string.IsNullOrEmpty(searchCustomer))
                {
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                    rcvListCustomer.Visibility = ViewStates.Gone;

                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    rcvListCustomer.Visibility = ViewStates.Visible;
                }

                SetBtnSearch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterCustomerData at Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TextSearch_KeyPress(object sender, View.KeyEventArgs e)
        {
            SetBtnSearch();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterCustomerData();
            }
            View view = this.Activity.CurrentFocus;
            if (view != null)
            {
                if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
                {
                    MainActivity.main_activity.CloseContextMenu();
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
                textSearch.Text += input;
                textSearch.SetSelection(textSearch.Text.Length);
                return;
            }
        }

        private void TextSearch_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearch.Text.Trim()))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }

        private void TextSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchCustomer = textSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchCustomer))
            {
                SetCustomerData();
            }
            SetBtnSearch();
        }

        private void BtnSearchCustomer_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            SetCustomerData();
        }
        string searchCustomer;

        private void SetClearSearchText()
        {
            searchCustomer = "";
            textSearch.Text = string.Empty;
        }
        internal static void SetSelectCustomer(List<Customer> l, string t)
        {
            listChooseCustomer = l;
            customerSelect = t;
        }
        private async void BtnApply_Click(object sender, EventArgs e)
        {
            if (customerSelect != string.Empty)
            {
                Report_Dialog_Custom.listChooseCustomer = listChooseCustomer;
                await Report_Dialog_Custom.dialog_Custom.SetDataCustomer();
                this.Dialog.Dismiss();
            }
            else
            {
                Toast.MakeText(this.Activity, "กรุณาเลือกลูกค้า", ToastLength.Short).Show();
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }
        LinearLayout lnBack;
        FrameLayout lnSearch;
        ImageButton btnSearch;
        EditText textSearch;
        Button btnAll;
        RecyclerView rcvListCustomer;
        LinearLayout lnNoDataSearch;
        Button btnApply;
        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnSearch = view.FindViewById<FrameLayout>(Resource.Id.lnSearch);
            btnSearch = view.FindViewById<ImageButton>(Resource.Id.btnSearch);
            textSearch = view.FindViewById<EditText>(Resource.Id.textSearch);
            btnAll = view.FindViewById<Button>(Resource.Id.btnAll);
            rcvListCustomer = view.FindViewById<RecyclerView>(Resource.Id.rcvListCustomer);
            btnApply = view.FindViewById<Button>(Resource.Id.btnApply);

        }
    }
}