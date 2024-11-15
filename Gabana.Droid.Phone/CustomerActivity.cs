using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.Helper;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class CustomerActivity : AppCompatActivity
    {
        static RecyclerView recyclerview_listcustomer;
        RecyclerView.LayoutManager mLayoutManager;
        CustomerManage CustomerManage = new CustomerManage();
        Customer_Adapter_Main Customer_Adapter_Main;
        ImageButton btnback, btnSearchCustomer;
        ImageButton btnAddCustomer;
        static List<Customer> listCustomer, listSearchCustomer;
        EditText textSearchCustomer;
        ListCustomer lstCustomer;
        LinearLayout lnBack, lnNoCustomer;
        public static CustomerActivity customerActivity;
        string SearchName, LoginType;
        SwipeRefreshLayout refreshlayout;
        public static Context context;
        List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        CustomerManage customerManage = new CustomerManage();
        Customer customer = new Customer();
        int maxCustomerRevision = 0;
        public static Customer FocusCustomer;
        public static bool checkNet = false;
        LinearLayout lnNoDataSearch;
        public static bool checkManinRole;
        DialogLoading dialogLoading = new DialogLoading();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.customer_activity_main);
                customerActivity = this;
                context = Android.App.Application.Context;

                recyclerview_listcustomer = FindViewById<RecyclerView>(Resource.Id.recyclerview_listcustomer);
                btnback = FindViewById<ImageButton>(Resource.Id.btnBack);
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnNoDataSearch = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
                lnNoCustomer = FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
                btnSearchCustomer = FindViewById<ImageButton>(Resource.Id.btnSearchTopping);
                btnSearchCustomer.Click += BtnSearchCustomer_Click;
                textSearchCustomer = FindViewById<EditText>(Resource.Id.textSearchCustomer);

                lnBack.Click += LnBack_Click;
                btnback.Click += LnBack_Click;
                textSearchCustomer.TextChanged += TextSearchCustomer_TextChanged;
                textSearchCustomer.KeyPress += TextSearchCustomer_KeyPress;
                textSearchCustomer.FocusChange += TextSearchCustomer_FocusChange;

                btnAddCustomer = FindViewById<ImageButton>(Resource.Id.btnAddCustomer);
                btnAddCustomer.Click += BtnAddCustomer_Click;
                LoginType = Preferences.Get("LoginType", "");
                checkManinRole = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "customer");
                if (checkManinRole)
                {
                    btnAddCustomer.SetBackgroundResource(Resource.Mipmap.Add);
                    btnAddCustomer.Enabled = true;
                }
                else
                {
                    btnAddCustomer.SetBackgroundResource(Resource.Mipmap.AddMax);
                    btnAddCustomer.Enabled = false;
                }

                CheckJwt();

                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                refreshlayout.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagCustomerChange = true;                   
                    if (!await GabanaAPI.CheckNetWork())
                    {
                        Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }
                    else if (!await GabanaAPI.CheckSpeedConnection())
                    {
                        Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
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
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var w = mainDisplayInfo.Width;
                var Width = w / 5;
                UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "customer");
                MySwipeHelper mySwipe = new MyImplementSwipeHelper(this, recyclerview_listcustomer, (int)Width);
                DataCashingAll.flagCustomerChange = true;
                Log.Debug("connectpass", "Customer" + "OnCreate");

                _ = TinyInsights.TrackPageViewAsync("OnCreate : CustomerActivity");

            }
            catch (Exception ex)
            {
                btnAddCustomer.Enabled = true;
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("oncreate at Customer");
                Log.Debug("connectpass", ex.Message + "Error OnCreate");
            }
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

        private async void BtnSearchCustomer_Click(object sender, EventArgs e)
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

        private void SetBtnSearch()
        {
            SearchName = textSearchCustomer?.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                btnSearchCustomer.SetBackgroundResource(Resource.Mipmap.Search);
            }
            else
            {
                btnSearchCustomer.SetBackgroundResource(Resource.Mipmap.DelTxt);
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
        private async void TextSearchCustomer_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
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
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
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

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            FocusCustomer = null;
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        private void BtnAddCustomer_Click(object sender, EventArgs e)
        {
            btnAddCustomer.Enabled = false;
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "customer");
            if (!check)
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                btnAddCustomer.Enabled = true;
                return;
            }
            StartActivity(new Android.Content.Intent(context, typeof(AddCustomerActivity)));
            btnAddCustomer.Enabled = true;
        }

        async Task GetDataCustomer()
        {
            try
            {
                listCustomer = new List<Customer>();
                CustomerManage customerManage = new CustomerManage();
                listCustomer = await customerManage.GetAllCustomer();
                if (listCustomer == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    listCustomer = new List<Customer>();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDataCustomer at Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async Task SetDataCustomer()
        {
            try
            {
                lstCustomer = new ListCustomer(listCustomer);
                mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                //recyclerview_listcustomer.HasFixedSize = true;                
                recyclerview_listcustomer.SetLayoutManager(mLayoutManager);
                recyclerview_listcustomer.HasFixedSize = false;
                Customer_Adapter_Main = new Customer_Adapter_Main(lstCustomer, checkNet);
                recyclerview_listcustomer.SetItemViewCacheSize(listCustomer.Count + 1);                
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
                _ = TinyInsights.TrackPageViewAsync("SetDataCustomer at Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Customer_Adapter_Main_ItemClick(object sender, int e)
        {
            try
            {
                Customer customer = new Customer();
                if (string.IsNullOrEmpty(SearchName))
                {
                    customer = listCustomer[e];
                }
                else
                {
                    customer = listSearchCustomer[e];
                }
                StartActivity(new Intent(this, typeof(AddCustomerActivity)));
                AddCustomerActivity.setCustomer(customer.SysCustomerID);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Customer_Adapter_Main_ItemClick at Customer");
                Toast.MakeText(context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void ImageCustomer_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "this test", ToastLength.Short).Show();
        }

        private async void SetFilterCustomerData()
        {
            try
            {
                listSearchCustomer = new List<Customer>();
                if (string.IsNullOrEmpty(SearchName))
                {
                    return;
                }

                listSearchCustomer = listCustomer.Where(x => x.CustomerName.ToLower().Contains(SearchName.ToLower()) | (x.Mobile != null && x.Mobile.Contains(SearchName)) | (x.CustomerID != null && x.CustomerID.Contains(SearchName))).ToList();

                if (listSearchCustomer.Count > 0)
                {
                    listSearchCustomer = listSearchCustomer.OrderBy(x => x.CustomerName).ToList();
                }

                lstCustomer = new ListCustomer(listSearchCustomer);
                mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listcustomer.HasFixedSize = false;
                //recyclerview_listcustomer.HasFixedSize = true;
                recyclerview_listcustomer.SetLayoutManager(mLayoutManager);
                recyclerview_listcustomer.SetItemViewCacheSize(listCustomer.Count + 1);
                Customer_Adapter_Main = new Customer_Adapter_Main(lstCustomer, checkNet);
                recyclerview_listcustomer.SetAdapter(Customer_Adapter_Main);
                Customer_Adapter_Main.ItemClick += Customer_Adapter_Main_ItemClick;

                if (Customer_Adapter_Main.ItemCount == 0)
                {
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                    lnNoCustomer.Visibility = ViewStates.Gone;
                    recyclerview_listcustomer.Visibility = ViewStates.Gone;
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
                _ = TinyInsights.TrackPageViewAsync("SetFilterCustomerData at Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        protected override async void OnResume()
        {
            base.OnResume();
            CheckJwt();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                checkNet = await GabanaAPI.CheckNetWork();
                
                if (DataCashingAll.flagCustomerChange)
                {
                    if (string.IsNullOrEmpty(SearchName))
                    {
                        await GetDataCustomer();
                        await SetDataCustomer();
                        SetBtnSearch();
                    }
                    DataCashingAll.flagCustomerChange = false;
                }
                CustomerFocus();
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                Log.Debug("connectpass", "Customer" + "OnResume");
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("onresume at Customer");
            }
        }

        public void Resume()
        {
            try
            {
                OnResume();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Resume at Customer");
            }
        }

        public static async void OpenDialogImage(Customer Customer)
        {
            try
            {
                string path = "";
                if (await GabanaAPI.CheckSpeedConnection())
                {
                    //MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.dialog_item.ToString();
                    bundle.PutString("message", myMessage);
                    if (!string.IsNullOrEmpty(Customer.PicturePath))
                    {
                        if (Customer.PicturePath.Contains("http"))
                        {
                            bundle.PutString("OpenCloudPicture", Customer.PicturePath);
                            path = Customer.PicturePath;
                        }
                        else
                        {
                            Java.IO.File imgFile = new Java.IO.File(Customer.PictureLocalPath);
                            if (imgFile != null)
                            {
                                bundle.PutString("OpenCloudPicture", imgFile.AbsolutePath);
                                path = imgFile.AbsolutePath;
                            }
                        }
                    }

                    //dialog.Arguments = bundle;
                    //dialog.Show(customerActivity.SupportFragmentManager, myMessage);

                    Show_Dialog_Customer dialog_Item = Show_Dialog_Customer.NewInstance(path);
                    dialog_Item.Show(customerActivity.SupportFragmentManager, myMessage);

                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OpenDialogImage at Customer");
                Toast.MakeText(CustomerActivity.customerActivity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private class MyImplementSwipeHelper : MySwipeHelper
        {
            Context context;
            RecyclerView recyclerView;
            int buttonWidth;
            public MyImplementSwipeHelper(Context context, RecyclerView recyclerView, int buttonWidth) : base(context, recyclerView, buttonWidth)
            {
                this.context = context;
                this.recyclerView = recyclerView;
                this.buttonWidth = buttonWidth;
            }

            public override void InstantiateMybutton(RecyclerView.ViewHolder viewHolder, List<MyButton> buffer)
            {
                buffer.Add(new MyButton(context,
                    "Delete",
                    0,
                    Resource.Mipmap.DeleteBt,
                    "#33AAE1",
                    new MyDeleteButtonClick(this)));
            }

            private class MyDeleteButtonClick : MyButtonClickListener
            {
                private MyImplementSwipeHelper myImplementSwipeHelper;

                public MyDeleteButtonClick(MyImplementSwipeHelper myImplementSwipeHelper)
                {
                    this.myImplementSwipeHelper = myImplementSwipeHelper;
                }
                public async void OnClick(int position)
                {
                    try
                    {
                        var LoginType = Preferences.Get("LoginType", "");
                        bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "customer");
                        if (check)
                        {
                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                            bundle.PutString("message", myMessage);
                            bundle.PutInt("systemID", (int)listCustomer[position].SysCustomerID);
                            bundle.PutString("deleteType", "customer");
                            bundle.PutString("fromPage", "main");
                            dialog.Arguments = bundle;
                            dialog.Show(customerActivity.SupportFragmentManager, myMessage);
                        }
                        else
                        {
                            Toast.MakeText(myImplementSwipeHelper.context, myImplementSwipeHelper.context.GetString(Resource.String.notperm), ToastLength.Short).Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("customer");
                        Toast.MakeText(myImplementSwipeHelper.context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
            }

        }

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
                    _ = Task.Factory.StartNew(() => Utils.InsertPictureLocalCustomer(lstCustomerImage));
                    Log.Debug("connectpass", "UpdateImageCustomer(UpdateCustomer) UpdateCustomer : " + UpdateCustomer.Count);
                    _ = Task.Factory.StartNew(() => Utils.UpdateImageCustomer(UpdateCustomer));

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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

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

        private async void CustomerFocus()
        {
            try
            {
                if (FocusCustomer != null)
                {
                    int index = -1;                   
                    if (listCustomer != null)
                    {
                        if (listCustomer.Count == 0)
                        {
                            listCustomer.Add(FocusCustomer);
                            await SetDataCustomer();
                            FocusCustomer = null;
                            return;
                        }
                        index = listCustomer.FindIndex(x => x.SysCustomerID == FocusCustomer.SysCustomerID);
                        if (index != -1)
                        {
                            listCustomer.RemoveAt(index);
                        }
                        listCustomer.Insert(0, FocusCustomer);
                    }
                    if (listSearchCustomer?.Count > 0)
                    {
                        index = listSearchCustomer.FindIndex(x => x.SysCustomerID == FocusCustomer.SysCustomerID);
                        if (index != -1)
                        {
                            listSearchCustomer.RemoveAt(index);
                        }
                        listSearchCustomer.Insert(0, FocusCustomer);
                    }
                    Customer_Adapter_Main.NotifyDataSetChanged();
                    FocusCustomer = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CustomerFocus at Customer");
                Toast.MakeText(customerActivity, ex.Message, ToastLength.Short).Show();
            }
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'CustomerActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'CustomerActivity.openPage' is assigned but its value is never used
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

            CheckJwt();
        }

    }
}

