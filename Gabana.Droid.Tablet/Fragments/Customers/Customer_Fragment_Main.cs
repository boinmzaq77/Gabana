 using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Gabana.Droid.Tablet.Adapter.Customers;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;
using static AndroidX.RecyclerView.Widget.RecyclerView;

namespace Gabana.Droid.Tablet.Fragments.Customers
{
    public class Customer_Fragment_Main : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Customer_Fragment_Main NewInstance()
        {
            Customer_Fragment_Main frag = new Customer_Fragment_Main();
            return frag;
        }
        string SearchName, LoginType;
        View view;
        public static Customer_Fragment_Main fragment_main;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.customer_fragment_main, container, false);
            try
            {
                fragment_main = this;
                CombineUI();

                LoginType = Preferences.Get("LoginType", "");

                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "customer");
                if (check)
                {
                    addCustomer.SetBackgroundResource(Resource.Mipmap.Add);
                }
                else
                {
                    addCustomer.SetBackgroundResource(Resource.Mipmap.AddMax);
                }
                refreshlayout.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagCustomerChange = true;
                    if (!DataCashing.CheckNet)
                    {
                        Toast.MakeText(Application.Context, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }
                    else if (!await GabanaAPI.CheckSpeedConnection())
                    {
                        Toast.MakeText(Application.Context, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    }
                    else
                    {
                        await GetOnlineDataCustomer();
                        OnResume();
                    }
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }

        public static Customer FocusCustomer;
        internal static void SetFocusCustomer(Customer customer)
        {
            try
            {
                FocusCustomer = customer;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFocusCustomer at customer");
            }
        }
        public override async void OnResume()
        {
            try
            {
                base.OnResume();


                if (!IsAdded)
                {
                    return;
                }

                //if (!IsVisible)
                //{
                //    return;
                //}

                

                //if (UserVisibleHint)
                //{
                //    // ทำสิ่งที่ต้องการเมื่อ Fragment กำลังแสดง
                //}

                var checkManinRole = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "customer");
                if (checkManinRole)
                {
                    addCustomer.SetBackgroundResource(Resource.Mipmap.Add);
                    addCustomer.Enabled = true;
                }
                else
                {
                    addCustomer.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addCustomer.Enabled = false;
                }
                await GetDataCustomer();
                SetBtnSearch();
                
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFocusCustomer at customer");
            }
        }

        public static List<Customer> listCustomer = new List<Customer>();
        static List<Customer> listSearchCustomer = new List<Customer>();
        private async Task GetDataCustomer()
        {
            try
            {
                listCustomer = new List<Customer>();
                CustomerManage customerManage = new CustomerManage();
                listCustomer = await customerManage.GetAllCustomer();
                listCustomer = listCustomer.OrderBy(x => x.CustomerName).ToList();
                if (listCustomer == null)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    listCustomer = new List<Customer>();
                }

                await SetDataCustomer();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDataCustomer at Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        LayoutManager mLayoutManager;
        ListCustomer lstCustomer;
        Customer_Adapter_Main customer_adapter;

       async Task SetDataCustomer()
        {
            try
            {
                lstCustomer = new ListCustomer(listCustomer);
                mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvCustomer.HasFixedSize = true;
                rcvCustomer.SetLayoutManager(mLayoutManager);
                rcvCustomer.SetItemViewCacheSize(listCustomer.Count + 1);
                customer_adapter = new Customer_Adapter_Main(lstCustomer, checkNet);
                rcvCustomer.SetAdapter(customer_adapter);
                customer_adapter.ItemClick += Customer_adapter_ItemClick;

                if (customer_adapter.ItemCount > 0)
                {
                    lnNoCustomer.Visibility = ViewStates.Gone;
                    rcvCustomer.Visibility = ViewStates.Visible;
                }
                else
                {
                    lnNoCustomer.Visibility = ViewStates.Visible;
                    rcvCustomer.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetDataCustomer at Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Customer_adapter_ItemClick(object sender, int e)
        {
            try
            {
                if (Customer_Fragment_AddCustomer.flagdatachange == true)
                {
                    if (DataCashing.EditCus == null)
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.add_dialog_back.ToString();
                        Add_Dialog_Back.SetPage("customer");
                        dialog.Arguments = bundle;
                        dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                        return;
                    }
                    else
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        Edit_Dialog_Back.SetPage("customer");
                        String myMessage = Resource.Layout.edit_dialog_back.ToString();
                        bundle.PutString("message", myMessage);
                        dialog.Arguments = bundle;
                        dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                        return;
                    }
                }

                Customer customer = new Customer();
                if (string.IsNullOrEmpty(SearchName))
                {
                    customer = listCustomer[e];
                }
                else
                {
                    customer = listSearchCustomer[e];
                }

                DataCashing.flagChooseMedia = false;
                DataCashing.EditCus = customer;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnCustomer, "customer", "addcustomer");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Customer_Adapter_Main_ItemClick at Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }
        List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        CustomerManage customerManage = new CustomerManage();
        int maxCustomerRevision = 0;
        private async Task GetOnlineDataCustomer()
        {
            try
            {
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 50).FirstOrDefault();
                #region Customer
                try
                {
                    //Get Customer API
                    var allcustomer = await GabanaAPI.GetDataCustomer((int)revisionNo.LastRevisionNo, 0);

                    if (allcustomer == null)
                    {
                        return;
                    }

                    if (allcustomer.totalCustomer == 0)
                    {
                        return;
                    }

                    //check ว่ามีไหม
                    List<Gabana3.JAM.Customer.CustomerStatus> UpdateCustomer = new List<Gabana3.JAM.Customer.CustomerStatus>();
                    List<Gabana3.JAM.Customer.CustomerStatus> InsertCustomer = new List<Gabana3.JAM.Customer.CustomerStatus>();
                    List<Customer> lstCustomerImage = new List<Customer>();
                    int round = 0, addrount = 0;
                    round = allcustomer.totalCustomer / 100;
                    addrount = round + 1;
                    for (int j = 0; j < addrount; j++)
                    {
                        allcustomer = await GabanaAPI.GetDataCustomer((int)revisionNo.LastRevisionNo, j);

                        if (allcustomer == null)
                        {
                            break;
                        }

                        if (allcustomer.totalCustomer == 0)
                        {
                            break;
                        }

                        allcustomer.CustomerStatus.ToList().OrderBy(x => x.Customers.RevisionNo);
                        var maxCustomer = allcustomer.CustomerStatus.ToList().Max(x => x.Customers.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();

                        List<Customer> GetallCustomer = new List<Customer>();
                        GetallCustomer = await customerManage.GetAllCustomer();
                        UpdateCustomer.AddRange(allcustomer.CustomerStatus.Where(x => GetallCustomer.Select(y => (long)y.SysCustomerID).ToList().Contains(x.Customers.SysCustomerID)).ToList());
                        InsertCustomer.AddRange(allcustomer.CustomerStatus.Where(x => !(GetallCustomer.Select(y => (long)y.SysCustomerID).ToList().Contains(x.Customers.SysCustomerID)) && x.DataStatus != 'D').ToList());

                        //Insert Customer
                        if (InsertCustomer.Count > 0)
                        {
                            List<Customer> BulkCustomer = new List<Customer>();
                            foreach (var customer in InsertCustomer)
                            {
                                string thumnailPath = string.Empty;

                                BulkCustomer.Add(new Customer()
                                {
                                    MerchantID = customer.Customers.MerchantID,
                                    SysCustomerID = customer.Customers.SysCustomerID,
                                    CustomerName = customer.Customers.CustomerName,
                                    Ordinary = customer.Customers.Ordinary,
                                    ShortName = customer.Customers.ShortName,
                                    PictureLocalPath = "",
                                    ThumbnailLocalPath = thumnailPath,
                                    EMail = customer.Customers.EMail,
                                    Mobile = customer.Customers.Mobile,
                                    Gender = customer.Customers.Gender,
                                    BirthDate = customer.Customers.BirthDate,
                                    Address = customer.Customers.Address,
                                    ProvincesId = customer.Customers.ProvincesId,
                                    AmphuresId = customer.Customers.AmphuresId,
                                    DistrictsId = customer.Customers.DistrictsId,
                                    PicturePath = customer.Customers.PicturePath, //Clound Image
                                    IDCard = customer.Customers.IDCard,
                                    Comments = customer.Customers.Comments,
                                    LastDateModified = customer.Customers.LastDateModified,
                                    UserLastModified = customer.Customers.UserLastModified,
                                    DataStatus = customer.DataStatus,
                                    FWaitSending = 0,
                                    WaitSendingTime = DateTime.UtcNow,
                                    LinkProMaxxID = customer.Customers.LinkProMaxxID,
                                    MemberTypeNo = customer.Customers.MemberTypeNo,
                                    CustomerID = customer.Customers.CustomerID,
                                    LineID = customer.Customers.LineID,
                                    ThumbnailPath = customer.Customers.ThumbnailPath, //Clound Image
                                });
                                maxCustomerRevision = customer.Customers.RevisionNo;
                            }

                            using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                            {
                                try
                                {
                                    await db.BulkCopyAsync(BulkCustomer);
                                }
                                catch (Exception ex)
                                {
                                    var errorRevison = InsertCustomer.Select(x => x.Customers.RevisionNo).Min();
                                    maxCustomerRevision = errorRevison;
                                    Log.Error("connecterror", "BulkCustomer :" + ex.Message);
                                    throw ex;
                                }
                            }

                            lstCustomerImage.AddRange(BulkCustomer);
                        }

                        //Update Customer
                        if (UpdateCustomer.Count > 0)
                        {
                            foreach (var customer in UpdateCustomer)
                            {
                                var data = await customerManage.GetCustomer(customer.Customers.MerchantID, customer.Customers.SysCustomerID);

                                if (customer.DataStatus == 'D')
                                {
                                    //delete รูป
                                    if (!string.IsNullOrEmpty(data?.ThumbnailLocalPath))
                                    {
                                        Java.IO.File imgTempFile = new Java.IO.File(data?.ThumbnailLocalPath);

                                        if (System.IO.File.Exists(imgTempFile.AbsolutePath))
                                        {
                                            System.IO.File.Delete(imgTempFile.AbsolutePath);
                                        }
                                    }
                                    //delete
                                    var delete = await customerManage.DeleteCustomer(customer.Customers.MerchantID, customer.Customers.SysCustomerID);
                                    if (!delete)
                                    {
                                        if (data != null)
                                        {
                                            data.DataStatus = 'D';
                                            data.FWaitSending = 0;
                                            await customerManage.UpdateCustomer(data);
                                        }
                                    }
                                }
                                else
                                {
                                    string thumnailLocalPath = string.Empty;

                                    //insertorreplace
                                    Customer _customer = new Customer()
                                    {
                                        MerchantID = customer.Customers.MerchantID,
                                        SysCustomerID = customer.Customers.SysCustomerID,
                                        CustomerName = customer.Customers.CustomerName,
                                        Ordinary = customer.Customers.Ordinary,
                                        ShortName = customer.Customers.ShortName,
                                        PictureLocalPath = "",
                                        ThumbnailLocalPath = thumnailLocalPath,
                                        EMail = customer.Customers.EMail,
                                        Mobile = customer.Customers.Mobile,
                                        Gender = customer.Customers.Gender,
                                        BirthDate = customer.Customers.BirthDate,
                                        Address = customer.Customers.Address,
                                        ProvincesId = customer.Customers.ProvincesId,
                                        AmphuresId = customer.Customers.AmphuresId,
                                        DistrictsId = customer.Customers.DistrictsId,
                                        PicturePath = customer.Customers.PicturePath, //Clound Image
                                        IDCard = customer.Customers.IDCard,
                                        Comments = customer.Customers.Comments,
                                        LastDateModified = customer.Customers.LastDateModified,
                                        UserLastModified = customer.Customers.UserLastModified,
                                        DataStatus = customer.DataStatus,
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow,
                                        LinkProMaxxID = customer.Customers.LinkProMaxxID,
                                        MemberTypeNo = customer.Customers.MemberTypeNo,
                                        CustomerID = customer.Customers.CustomerID,
                                        LineID = customer.Customers.LineID,
                                        ThumbnailPath = customer.Customers.ThumbnailPath, //Clound Image

                                    };
                                    var insertOrreplace = await customerManage.InsertOrReplaceCustomer(_customer);
                                }

                                maxCustomerRevision = customer.Customers.RevisionNo;
                            }
                        }
                        await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxCustomer);
                    }
                    //insert Image to Local เมื่อเพิ่มข้อมูลทั้งหมดสำเร็จ ทั้งเคสเพิ่มและเคสอัปเดต
                    Log.Debug("connectpass", "InsertPictureLocalCustomer(lstCustomerImage) lstCustomerImage : " + lstCustomerImage.Count);
                    Task.Factory.StartNew(() => Utils.InsertPictureLocalCustomer(lstCustomerImage));
                    Log.Debug("connectpass", "UpdateImageCustomer(UpdateCustomer) UpdateCustomer : " + UpdateCustomer.Count);
                    Task.Factory.StartNew(() => Utils.UpdateImageCustomer(UpdateCustomer));

                    Log.Debug("connectpass", "listRivisionCustomer");
                }
                catch (Exception ex)
                {
                    Log.Debug("connecterror", "listRivisionCustomer : " + ex.Message);
                    await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxCustomerRevision);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CustomerChange");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        ImageButton btnSearch;
        EditText textSearchCustomer;
        SwipeRefreshLayout refreshlayout;
        RecyclerView rcvCustomer;
        LinearLayout lnNoCustomer;
        internal ImageButton addCustomer;
        private bool checkNet;

        private void CombineUI()
        {
            btnSearch = view.FindViewById<ImageButton>(Resource.Id.btnSearch);
            textSearchCustomer = view.FindViewById<EditText>(Resource.Id.textSearchCustomer);
            refreshlayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
            rcvCustomer = view.FindViewById<RecyclerView>(Resource.Id.rcvCustomer);
            lnNoCustomer = view.FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
            addCustomer = view.FindViewById<ImageButton>(Resource.Id.addCustomer);

            addCustomer.Click += AddCustomer_Click;
            textSearchCustomer.TextChanged += TextSearchCustomer_TextChanged;
            textSearchCustomer.KeyPress += TextSearchCustomer_KeyPress;
            textSearchCustomer.FocusChange += TextSearchCustomer_FocusChange;
            btnSearch.Click += BtnSearch_Click;
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            await SetDataCustomer();
        }

        private void SetClearSearchText()
        {
            SearchName = "";
            textSearchCustomer.Text = string.Empty;
            SetBtnSearch();
        }

        private void TextSearchCustomer_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus && !string.IsNullOrEmpty(textSearchCustomer.Text))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.Search);
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
                    SetBtnSearch();
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
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TextSearchCustomer_KeyPress at Customer");
            }
        }

        private async void TextSearchCustomer_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchName = textSearchCustomer.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                await SetDataCustomer();
            }
            SetBtnSearch();
        }

        private void SetBtnSearch()
        {
            //SearchName = textSearchCustomer?.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.Search);
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
        }

        private async void AddCustomer_Click(object sender, EventArgs e)
        {
            addCustomer.Enabled = false;
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "customer");
            if (!check)
            {
                addCustomer.Enabled = true;
                Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                return;
            }
            
            DataCashing.EditCus = null;
            Customer_Fragment_AddCustomer.keepCropedUri = null;
            Customer_Fragment_AddCustomer.customerEdit = null;
            DataCashing.flagChooseMedia = false;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnCustomer, "customer", "addcustomer");
            addCustomer.Enabled = true;
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

                lstCustomer = new ListCustomer(listSearchCustomer);
                mLayoutManager = new LinearLayoutManager(Application.Context, LinearLayoutManager.Vertical, false);
                rcvCustomer.HasFixedSize = false;
                //recyclerview_listcustomer.HasFixedSize = true;
                rcvCustomer.SetLayoutManager(mLayoutManager);
                rcvCustomer.SetItemViewCacheSize(listCustomer.Count + 1);
                customer_adapter = new Customer_Adapter_Main(lstCustomer, checkNet);
                rcvCustomer.SetAdapter(customer_adapter);
                customer_adapter.ItemClick += Customer_adapter_ItemClick;

                if (customer_adapter.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(SearchName))
                    {
                        lnNoCustomer.Visibility = ViewStates.Visible;
                        lnNoCustomer.Visibility = ViewStates.Gone;
                        rcvCustomer.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoCustomer.Visibility = ViewStates.Gone;
                        lnNoCustomer.Visibility = ViewStates.Visible;
                        rcvCustomer.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoCustomer.Visibility = ViewStates.Gone;
                    lnNoCustomer.Visibility = ViewStates.Gone;
                    rcvCustomer.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterCustomerData at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public  void DeleteCustomer(Customer DeleteCustomer)
        {
            try
            {
                int index = 0;
                index = listCustomer.FindIndex(x => x.SysCustomerID == DeleteCustomer.SysCustomerID);
                if (index == -1)
                {
                    return;
                }

                listCustomer.RemoveAt(index);
                customer_adapter.NotifyItemRemoved(index);
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnCustomer, "customer", "default");
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _= TinyInsights.TrackPageViewAsync("DeleteCustomer at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void ReloadCustomer(Customer NewCustomer)
        {
            try
            {
                int index = 0;
                index = listCustomer.FindIndex(x => x.SysCustomerID == NewCustomer.SysCustomerID);
                if (index > -1)
                {
                    listCustomer[index] = NewCustomer;
                    customer_adapter.NotifyItemChanged(index);
                    return;
                }

                listCustomer.Insert(0, NewCustomer);
                rcvCustomer.SmoothScrollToPosition(0);
                customer_adapter.NotifyItemInserted(0);
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _= TinyInsights.TrackPageViewAsync("ReloadCustomer at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

    }
}