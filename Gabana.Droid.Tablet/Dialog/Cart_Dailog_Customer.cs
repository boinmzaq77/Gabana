using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Pos;
using Gabana.Droid.Tablet.Fragments.PayMent;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Trans;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Cart_Dailog_Customer : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Cart_Dailog_Customer NewInstance()
        {
            var frag = new Cart_Dailog_Customer { Arguments = new Bundle() };
            return frag;
        }

        View view;
        public static long? selectCustomer;
        public static Cart_Dailog_AddCustomer cart_dailog_addCustomer;
        public static Cart_Dailog_Customer cart_dailog_customer;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.pos_dialog_customer, container, false);
            try
            {
                cart_dailog_customer = this;
                CombinUI();
                checkNet = DataCashing.CheckNet;
                _ = SetDataCustomer();
                if (DataCashing.SysCustomerID != null)
                {
                    selectCustomer = DataCashing.SysCustomerID;
                }
                else
                {
                    selectCustomer = 999;
                }
                SetBtnApply();
                _ = TinyInsights.TrackPageViewAsync("OnCreateView : Cart_Dailog_Customer");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        LinearLayout lnBack, lnAddCustomer, btnBack;
        ImageButton btnAddCustomer, btnSearchCustomer;
        FrameLayout lnSearchCustomer;
        EditText textSearchCustomer;
        RecyclerView rcvlistcustomer;
        LinearLayout lnNoCustomer, lnNoDataSearch, lnbtnCancle;
        Button btnApply;

        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnAddCustomer = view.FindViewById<LinearLayout>(Resource.Id.lnAddCustomer);
            btnAddCustomer = view.FindViewById<ImageButton>(Resource.Id.btnAddCustomer);
            lnSearchCustomer = view.FindViewById<FrameLayout>(Resource.Id.lnSearchCustomer);
            btnSearchCustomer = view.FindViewById<ImageButton>(Resource.Id.btnSearchCustomer);
            textSearchCustomer = view.FindViewById<EditText>(Resource.Id.textSearchCustomer);
            rcvlistcustomer = view.FindViewById<RecyclerView>(Resource.Id.rcvlistcustomer);
            lnNoCustomer = view.FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
            lnNoDataSearch = view.FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
            btnApply = view.FindViewById<Button>(Resource.Id.btnCancel);

            lnAddCustomer.Click += BtnAddCustomer_Click;
            btnAddCustomer.Click += BtnAddCustomer_Click;
            btnSearchCustomer.Click += BtnSearchCustomer_Click;
            textSearchCustomer.TextChanged += TextSearchCustomer_TextChanged; 
            textSearchCustomer.KeyPress += TextSearchCustomer_KeyPress; 
            textSearchCustomer.FocusChange += TextSearchCustomer_FocusChange; 
            btnApply.Click += BtnApply_Click;

            textSearchCustomer.ClearFocus();
            lnBack.Click += LnBack_Click;

            //MainActivity.main_activity.CloseKeyboard(view);
        }

        //public override async void OnResume()
        //{
        //    try
        //    {
        //        await SetDataCustomer();
        //    }
        //    catch (Exception ex)
        //    {
        //        Toast.MakeText(this.Context,ex.Message,ToastLength.Short).Show();
        //    }
        //}

        public void ReloadCustomer(Customer NewCustomer)
        {
            try
            {
                cart_dailog_customer.OnResume();
                Cart_Adapter_Customer cart_adapter_customer = new Cart_Adapter_Customer(lstCustomer, checkNet);
                rcvlistcustomer.SetAdapter(cart_adapter_customer);
                cart_adapter_customer.ItemClick += Cart_adapter_customer_ItemClick;

                int index = 0;
                index = listCustomer.FindIndex(x => x.SysCustomerID == NewCustomer.SysCustomerID);
                if (index > -1)
                {
                    listCustomer[index] = NewCustomer;
                    cart_adapter_customer.NotifyItemChanged(index);
                    return;
                }

                listCustomer.Insert(0, NewCustomer);
                rcvlistcustomer.SmoothScrollToPosition(0);
                cart_adapter_customer = new Cart_Adapter_Customer(lstCustomer, checkNet);
                cart_adapter_customer.NotifyItemInserted(0);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ReloadCustomer at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            try
            {
                //กรณีกดกลับ แบบไม่ได้เลือก
                if (MainActivity.main_activity != null)
                {
                    MainActivity.tranWithDetails = tranWithDetails;
                    this.Dialog.Dismiss();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void BtnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectDataCustomer == null && DataCashing.SysCustomerID != 999)
                {
                    DataCashing.SysCustomerID = 999;
                    selectCustomer = 999;
                }
                else
                {
                    DataCashing.SysCustomerID = selectCustomer;
                }


                if (DataCashing.SysCustomerID == 999)
                {
                    if (tranWithDetails.tran.SysCustomerID != 999)
                    {
                        tranWithDetails = await BLTrans.RemovePerson(tranWithDetails);

                        PaymentActivity.tranWithDetails = tranWithDetails;
                        MainActivity.tranWithDetails = tranWithDetails;
                    }
                }
                else
                {
                    CustomerManage customerManage = new CustomerManage();
                    var listCustomer = new List<Customer>();
                    listCustomer = await customerManage.GetAllCustomer();
                    Customer selectCus = listCustomer.Where(x => x.SysCustomerID == DataCashing.SysCustomerID).FirstOrDefault();
                    if (selectCus == null) return;
                    if (tranWithDetails.tran.SysCustomerID != DataCashing.SysCustomerID)
                    {
                        tranWithDetails.tran.SysCustomerID = selectCus.SysCustomerID;
                        tranWithDetails.tran.CustomerName = selectCus.CustomerName;

                        tranWithDetails = await BLTrans.ChoosePerson(tranWithDetails, selectCus);
                        tranWithDetails = BLTrans.Caltran(tranWithDetails);

                        PaymentActivity.tranWithDetails = tranWithDetails;
                        MainActivity.tranWithDetails = tranWithDetails;
                    }
                }
               
                if (MainActivity.main_activity != null)
                {
                    MainActivity.tranWithDetails = tranWithDetails;
                }
                if (PaymentActivity.payment_main != null)
                {
                    PaymentActivity.tranWithDetails = tranWithDetails;
                }

                if (POS_Fragment_Main.fragment_main != null)
                {
                    POS_Fragment_Main.fragment_main.OnResume();
                }
                if (POS_Fragment_Cart.fragment_cart != null)
                {
                    POS_Fragment_Cart.fragment_cart.OnResume();
                }
                if (Payment_Fragment_Main.fragment_main != null)
                {
                    Payment_Fragment_Main.fragment_main.OnResume();
                }
                if (Payment_Fragment_Cash.fragment_cash != null)
                {
                    Payment_Fragment_Cash.fragment_cash.OnResume();
                }
                if (Payment_Fragment_Balance.fragment_balance != null)
                {
                    Payment_Fragment_Balance.fragment_balance.OnResume();
                }
                this.Dialog.Dismiss();
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnApply_Click at SelectCustomer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TextSearchCustomer_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                View view = this.Activity.CurrentFocus;
                if (view != null)
                {
                    MainActivity.main_activity.CloseKeyboard(textSearchCustomer);
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
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        ListCustomer lstSerchCustomer;
        private async void SetFilterCustomerData()
        {
            try
            {
                if (string.IsNullOrEmpty(SearchName)) return;
                listSearchCustomer = new List<Customer>();
                listSearchCustomer = listCustomer.Where(x => x.CustomerName.ToLower().Contains(SearchName.ToLower()) | (x.Mobile != null && x.Mobile.Contains(SearchName)) | (x.CustomerID != null && x.CustomerID.Contains(SearchName))).ToList();
                if (listSearchCustomer.Count > 0)
                {
                    listSearchCustomer = listSearchCustomer.OrderBy(x => x.CustomerName).ToList();
                }
                lstSerchCustomer = new ListCustomer(listSearchCustomer);

                Cart_Adapter_Customer cart_adapter_customerSearch = new Cart_Adapter_Customer(lstSerchCustomer, checkNet);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvlistcustomer.HasFixedSize = true;
                rcvlistcustomer.SetLayoutManager(mLayoutManager);
                rcvlistcustomer.SetAdapter(cart_adapter_customerSearch);
                cart_adapter_customerSearch.ItemClick += Cart_adapter_customerSearch_ItemClick; 

                if (cart_adapter_customerSearch.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(SearchName))
                    {
                        lnNoDataSearch.Visibility = ViewStates.Visible;
                        lnNoCustomer.Visibility = ViewStates.Gone;
                        rcvlistcustomer.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearch.Visibility = ViewStates.Gone;
                        lnNoCustomer.Visibility = ViewStates.Visible;
                        rcvlistcustomer.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    lnNoCustomer.Visibility = ViewStates.Gone;
                    rcvlistcustomer.Visibility = ViewStates.Visible;
                }

                SetBtnSearch();
                SetBtnApply();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterCustomerData at Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Cart_adapter_customerSearch_ItemClick(object sender, int e)
        {

            try
            {
                if (lstSerchCustomer[e].SysCustomerID == DataCashing.SysCustomerID)
                {
                    selectDataCustomer = null;
                    selectCustomer = lstSerchCustomer[e].SysCustomerID;
                }
                else
                {
                    selectDataCustomer = listCustomer[e];
                    selectCustomer = lstSerchCustomer[e].SysCustomerID;
                }

                lstSerchCustomer = new ListCustomer(listSearchCustomer);
                Cart_Adapter_Customer cart_adapter_customerSearch = new Cart_Adapter_Customer(lstSerchCustomer, checkNet);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvlistcustomer.HasFixedSize = true;
                rcvlistcustomer.SetLayoutManager(mLayoutManager);
                rcvlistcustomer.SetAdapter(cart_adapter_customerSearch);
                cart_adapter_customerSearch.ItemClick += Cart_adapter_customerSearch_ItemClick;

                if (e > 6)
                {
                    rcvlistcustomer.ScrollToPosition(e);
                }

                if (cart_adapter_customerSearch.ItemCount > 0)
                {
                    //lnNoCustomer.Visibility = ViewStates.Gone;
                    rcvlistcustomer.Visibility = ViewStates.Visible;
                }
                else
                {
                    //lnNoCustomer.Visibility = ViewStates.Visible;
                    rcvlistcustomer.Visibility = ViewStates.Gone;
                }
                SetBtnSearch();
                SetBtnApply();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void TextSearchCustomer_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                SearchName = textSearchCustomer.Text.Trim();
                if (string.IsNullOrEmpty(SearchName))
                {
                    await SetDataCustomer();
                }
                SetBtnSearch();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void BtnSearchCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                SetClearSearchText();
                await SetDataCustomer();
                textSearchCustomer.ClearFocus();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private string SearchName;
        List<Customer> listCustomer, listSearchCustomer;
        public static bool checkNet = false;

        async Task SetDataCustomer()
        {
            try
            {
                listCustomer = await GetListCustomer();
                lstCustomer = new ListCustomer(listCustomer);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvlistcustomer.HasFixedSize = true;
                rcvlistcustomer.SetLayoutManager(mLayoutManager);
                rcvlistcustomer.SetItemViewCacheSize(listCustomer.Count + 1);
                Cart_Adapter_Customer cart_adapter_customer = new Cart_Adapter_Customer(lstCustomer,checkNet);
                rcvlistcustomer.SetAdapter(cart_adapter_customer);
                cart_adapter_customer.ItemClick += Cart_adapter_customer_ItemClick;

                if (cart_adapter_customer.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(SearchName))
                    {
                        lnNoDataSearch.Visibility = ViewStates.Visible;
                        lnNoCustomer.Visibility = ViewStates.Gone;
                        rcvlistcustomer.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearch.Visibility = ViewStates.Gone;
                        lnNoCustomer.Visibility = ViewStates.Visible;
                        rcvlistcustomer.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    lnNoCustomer.Visibility = ViewStates.Gone;
                    rcvlistcustomer.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _= TinyInsights.TrackPageViewAsync("SetDataCustomer at SelectCustomer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        ListCustomer lstCustomer;
        private void Cart_adapter_customer_ItemClick(object sender, int e)
        {
            try
            {
                if (listCustomer[e].SysCustomerID == DataCashing.SysCustomerID)
                {
                    selectDataCustomer = null;
                    selectCustomer = listCustomer[e].SysCustomerID;
                }
                else
                {
                    selectDataCustomer = listCustomer[e];
                    selectCustomer = listCustomer[e].SysCustomerID;
                }
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvlistcustomer.HasFixedSize = true;
                rcvlistcustomer.SetLayoutManager(mLayoutManager);
                rcvlistcustomer.SetItemViewCacheSize(listCustomer.Count + 1);
                Cart_Adapter_Customer cart_adapter_customer = new Cart_Adapter_Customer(lstCustomer, checkNet);
                rcvlistcustomer.SetAdapter(cart_adapter_customer);
                cart_adapter_customer.ItemClick += Cart_adapter_customer_ItemClick;
                if (e > 6)
                {
                    rcvlistcustomer.ScrollToPosition(e);
                }
                SetBtnApply();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async Task<List<Customer>> GetListCustomer()
        {
            try
            {
                var listCustomer = new List<Customer>();
                CustomerManage customerManage = new CustomerManage();
                listCustomer = await customerManage.GetAllCustomer();
                if (listCustomer == null)
                {
                    Toast.MakeText(this.Activity, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
                    return null;
                }
                Log.Debug("Customer", JsonConvert.SerializeObject(listCustomer));
                return listCustomer;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListCustomer at SelectCustomer");
                Console.WriteLine(ex.Message);
                Log.Debug("error", ex.Message);
                return null;
            }
        }

        private void SetClearSearchText()
        {
            try
            {
                SearchName = "";
                textSearchCustomer.Text = string.Empty;
                SetBtnSearch();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetBtnSearch()
        {
            try
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
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        Customer selectDataCustomer;
        private void SetBtnApply()
        {
            try
            {
                if (DataCashing.SysCustomerID == selectCustomer)
                {
                    btnApply.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                    btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                }
                else
                {
                    btnApply.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                    btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                }

                if (selectDataCustomer == null && DataCashing.SysCustomerID == 999)
                {
                    btnApply.Text = GetString(Resource.String.textcancle);
                }
                else if (selectDataCustomer == null && DataCashing.SysCustomerID != 999)
                {
                    btnApply.Text = GetString(Resource.String.selectcustomer_activity_remove);
                }
                else
                {
                    btnApply.Text = GetString(Resource.String.selectcustomer_activity_apply);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnAddCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                btnAddCustomer.Enabled = false;
                var LoginType = Preferences.Get("LoginType", "");
                bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "customer");
                if (!check)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                    return;
                }

                //  เพิ่ม Cart_Dialog_AdddCuestomer

                if (cart_dailog_addCustomer != null)
                {
                    return;
                }
                cart_dailog_addCustomer = new Cart_Dailog_AddCustomer();
                cart_dailog_addCustomer.Cancelable = false;
                cart_dailog_addCustomer.Show(MainActivity.main_activity.SupportFragmentManager, nameof(cart_dailog_addCustomer));
                btnAddCustomer.Enabled = true;
            }
            catch (Exception ex)
            {
                btnAddCustomer.Enabled = true;
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public static TranWithDetailsLocal tranWithDetails;
        internal static void SetTranWithDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }
    }
}