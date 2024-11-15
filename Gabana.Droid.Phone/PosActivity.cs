using Android.Animation;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.ListData;
using Gabana.Droid.Phone;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]

    public class PosActivity : AppCompatActivity
    {
        public static PosActivity pos;
        public CartScanActivity scan;
        public static ListCategory listCategory;
        List<MenuTabwithSysCategory> MenuTab { get; set; }
        List<Item> listFavorite = new List<Item>();
        List<Item> DefaultDataItem = new List<Item>();
        List<Item> DefaultDataTopping = new List<Item>();
        public static List<Item> DefaultAllItem = new List<Item>();

        List<Item> listToppingBody;
        List<Item> listItemBody;
        public static ListItem listItem;
        public static List<Category> lstCategory = new List<Category>();
        RecyclerView RecyclerViewHeader;
        RecyclerView RecyclerViewShow;
        RecyclerView.LayoutManager mLayoutManager;
        RecyclerView.LayoutManager HeaderLayoutManager;
        ImageButton imgSelect, imagebtnBack, imgScanbarcode, btnDummy, btnBasket, buttonSearch;
        LinearLayout lnOrder, lnBack;

        static ImageButton btnCustomer1, btnCustomer2;
        LinearLayout lnPayment;
        TextView textSum, txtQuantity, txtNoItem, txtOrderNum;
        EditText txtSearchPos;
        public double priceAmount;
        int fillter;
        bool flagView = false; // falgView = false = row,flagView = true = grid
        public static int? setQuantity;
        public static int? addQuantity;
        public static int totlaItems;
        public static string nameCategory = "", tabSelected;
        public static long sysCategory = 0;
        private static LinearLayout lnNoCustomer, lnHaveCustomer;
        public static TranWithDetailsLocal tranWithDetails = null;
        public static TranDetailItemWithTopping tranDetailItemWithTopping;
        public static Customer selectCus;
        GridLayoutManager gridLayoutManager;
        public static ImageButton imgCreateItem, imgShare;
        public static TextView txtNameCustomer;
        ImageView ImageShow, ImageShowPOS,imageViewRow; 
        Color colorAnimetion;
        CardView cardView;
        View view;
        public static int DetailNo = 0;
        public static bool favoriteMenu, ToppingMenu;
        string SearchItem;
        string CURRENCYSYMBOLS;
        string usernamelogin;
        SwipeRefreshLayout refreshlayout;
        LinearLayout lnGroupButton;
        Category category = new Category();

        public static bool checkNet = false, IsActive = false;
        public static List<Item> AllItem = new List<Item>();
        public static List<Item> AllItemStatusD = new List<Item>();


        protected async override void OnCreate(Bundle savedInstanceState)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                base.OnCreate(savedInstanceState);
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.pos_activity_main);
                pos = this;

                InitiaiUIElement();

                DataCashingAll.flagItemChange = true;
                DataCashingAll.flagCategoryChange = true;

                await CheckJwt();
                Log.Debug("swipapp", "POS OnCreate");

                tabSelected = "All";
                usernamelogin = Preferences.Get("User", "");
                if (Preferences.Get("ViewPos", "") != "Grid")
                {
                    flagView = false;
                }
                else
                {
                    flagView = true;
                }

                RunOnUiThread(() =>
                {
                    CheckOrderBeforeClose();
                    //check กรณีมีการแก้ไข VAT,Service Charge
                    decimal GetVat = 0;
                    if (string.IsNullOrEmpty(DataCashingAll.setmerchantConfig.TAXRATE))
                    {
                        GetVat = 0;
                    }
                    else
                    {
                        GetVat = Convert.ToDecimal(DataCashingAll.setmerchantConfig.TAXRATE);
                    }
                    tranWithDetails.tran.TaxRate = GetVat;
                    tranWithDetails.tran.TranTaxType = char.Parse(DataCashingAll.setmerchantConfig.TAXTYPE?.ToString());
                    tranWithDetails.tran.FmlServiceCharge = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE;
                    DataCashing.SysCustomerID = (int)tranWithDetails.tran.SysCustomerID;
                    BLTrans.Caltran(tranWithDetails);

                    DetailNo = tranWithDetails.tranDetailItemWithToppings.Count;
                    if (setQuantity == null || setQuantity <= 1)
                    {
                        txtQuantity.Text = "x" + 1;
                        addQuantity = 1;
                        btnBasket.SetBackgroundResource(Resource.Drawable.borderqauntity);
                        txtQuantity.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                        DataCashing.setQuantityToCart = (int)addQuantity;
                    }
                    else
                    {
                        btnBasket.SetBackgroundResource(Resource.Drawable.borderqauntityblue);
                        txtQuantity.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                        txtQuantity.Text = "x" + setQuantity.ToString();
                    }
                    SetCustomer();
                });

                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                refreshlayout.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagItemChange = true;
                    DataCashingAll.flagCategoryChange = true;
                    //refresh Online Data

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
                        try
                        {
                            await Task.Run(async () =>
                            {
                                await GetOnlineDataCategory();
                                await GetOnlineDataitem();
                            });
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        }

                        RunOnUiThread(() =>
                        {
                            OnResume();
                        });
                    }

                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }

                _ = TinyInsights.TrackPageViewAsync("OnCreate : PosActivity");

            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("at POS");
                return;
            }
        }

        private void InitiaiUIElement()
        {
            try
            {
                imgSelect = FindViewById<ImageButton>(Resource.Id.buttonShowMenu);
                imgScanbarcode = FindViewById<ImageButton>(Resource.Id.buttonBarcode);
                lnPayment = FindViewById<LinearLayout>(Resource.Id.linearPayment);
                textSum = FindViewById<TextView>(Resource.Id.txtSumary);
                imagebtnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                btnCustomer1 = FindViewById<ImageButton>(Resource.Id.btnCustomer1);
                btnCustomer2 = FindViewById<ImageButton>(Resource.Id.btnCustomer2);
                btnDummy = FindViewById<ImageButton>(Resource.Id.btnDummy);
                lnOrder = FindViewById<LinearLayout>(Resource.Id.lnOrder);
                btnBasket = FindViewById<ImageButton>(Resource.Id.btnBasket);
                txtQuantity = FindViewById<TextView>(Resource.Id.txtQuantity);
                txtNoItem = FindViewById<TextView>(Resource.Id.txtNoItem);
                lnNoCustomer = FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
                lnHaveCustomer = FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);
                txtNameCustomer = FindViewById<TextView>(Resource.Id.txtNameCustomer);
                txtSearchPos = FindViewById<EditText>(Resource.Id.txtSearchPos);
                lnGroupButton = FindViewById<LinearLayout>(Resource.Id.lnGroupButton);
                buttonSearch = FindViewById<ImageButton>(Resource.Id.buttonSearch);
                ImageShowPOS = FindViewById<ImageView>(Resource.Id.imgShowAnime);
                RecyclerViewHeader = FindViewById<RecyclerView>(Resource.Id.recyclerViewHeaderItem);
                RecyclerViewShow = FindViewById<RecyclerView>(Resource.Id.recyclerViewShow);
                txtOrderNum = FindViewById<TextView>(Resource.Id.txtOrderNum);

                buttonSearch.Click += ButtonSearch_Click1;
                imgSelect.Click += ImgSelect_Click;
                imgScanbarcode.Click += ImgScanQR_Click;
                lnPayment.Click += LnPayment_Click;
                imagebtnBack.Click += ImagebtnBack_Click;
                lnBack.Click += ImagebtnBack_Click;
                btnDummy.Click += BtnDummy_Click;
                lnOrder.Click += buttonOrder_Click;
                btnBasket.Click += BtnBasket_Click;
                txtSearchPos.TextChanged += TxtSearchPos_TextChanged;
                txtSearchPos.KeyPress += TxtSearchPos_KeyPress;
                txtSearchPos.Click += TxtSearchPos_Click;
                txtSearchPos.FocusChange += TxtSearchPos_FocusChange;

                btnCustomer1.Click += BtnCustomer_Click;
                btnCustomer2.Click += BtnCustomer_Click;
                lnNoCustomer.Click += BtnCustomer_Click;
                lnHaveCustomer.Click += BtnCustomer_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "InitiaiUIElement " +  ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetTabMenu()
        {
            try
            {
                MenuTab = new List<MenuTabwithSysCategory>
                {
                    new MenuTabwithSysCategory() { NameMenuEn = "All" , NameMenuTh = "ทั้งหมด" , SysCategory = 0},
                    new MenuTabwithSysCategory() { NameMenuEn = "Favorite" , NameMenuTh = "รายการโปรด" ,SysCategory = -2},
                    new MenuTabwithSysCategory() { NameMenuEn = "Extra Topping" , NameMenuTh = "ท็อปปิ้ง" , SysCategory = -3},
                };
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetTabMenu at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void SetTabShowMenu()
        {
            try
            {
                if (string.IsNullOrEmpty(tabSelected))
                {
                    tabSelected = "All";
                }

                // Tab Favorite
                listFavorite = DefaultAllItem?.Where(x => x.FavoriteNo > 0).ToList();
                MenuTab.Remove(MenuTab.FirstOrDefault(x => x.NameMenuEn == "Favorite" && listFavorite?.Count == 0));

                // Tab Topping
                List<Item> listitemTopping = DefaultDataTopping.Where(x => x.SaleItemType == 'T').ToList();
                MenuTab.Remove(MenuTab.FirstOrDefault(x => x.NameMenuEn == "Extra Topping" && listitemTopping?.Count == 0));

                CategoryManage CategoryManage = new CategoryManage();
                lstCategory = await CategoryManage.GetAllCategory();
                List<Category> categoryListNoItem = await CategoryManage.GetAllCategoryhaveitem();

                lstCategory = lstCategory?.Where(x => categoryListNoItem.Select(y => (long)y.SysCategoryID).Contains(x.SysCategoryID)).ToList();

                if (lstCategory != null)
                {
                    foreach (var category in lstCategory)
                    {
                        MenuTab.Add(new Model.MenuTabwithSysCategory { NameMenuEn = category.Name, NameMenuTh = category.Name, SysCategory = category.SysCategoryID });
                    }
                }

                listCategory = new ListCategory(lstCategory);
                RecyclerViewHeader.Visibility = MenuTab.Count <= 1 ? ViewStates.Gone : ViewStates.Visible;

                HeaderLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                RecyclerViewHeader.HasFixedSize = true;
                RecyclerViewHeader.SetLayoutManager(HeaderLayoutManager);
                Pos_Adapter_Header pos_Adapter_Header = new Pos_Adapter_Header(MenuTab);
                RecyclerViewHeader.SetItemViewCacheSize(30);
                RecyclerViewHeader.SetAdapter(pos_Adapter_Header);
                RecyclerViewHeader.SetItemAnimator(null);
                pos_Adapter_Header.ItemClick += Pos_Adapter_Header_ItemClick1;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetTabShowMenu at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Pos_Adapter_Header_ItemClick1(object sender, int e)
        {
            try
            {
                Pos_Adapter_Header.vhselect.NotSelect();
                var z = RecyclerViewHeader.FindViewHolderForAdapterPosition(e) as ListViewCategoryHolder;
                z.Select();
                Pos_Adapter_Header.vhselect = z;

                tabSelected = MenuTab[e].NameMenuEn;
                SetClearSearchText();

                nameCategory = MenuTab[e].NameMenuEn;
                sysCategory = MenuTab[e].SysCategory;

                var category = lstCategory.Where(x => x.Name == nameCategory & x.SysCategoryID == sysCategory).FirstOrDefault();
                if (category != null)
                {
                    var ID = category.SysCategoryID;
                    fillter = (int)ID;
                }
                else
                {
                    switch (nameCategory)
                    {
                        case "All":
                        case "ทั้งหมด":
                            fillter = 0;
                            break;
                        case "Favorite":
                        case "รายการโปรด":
                            fillter = -2;
                            break;
                        case "Extra Topping":
                        case "ท็อปปิ้ง":
                            fillter = -3;
                            break;
                        default:
                            fillter = 0;
                            break;
                    }
                }
                await ShowDetail();
                flagSetViewShow();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Item_Adapter_Header_ItemClick at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void ButtonSearch_Click1(object sender, EventArgs e)
        {
            SetClearSearchText();
            OnResume();
        }

        private void SetClearSearchText()
        {
            SearchItem = "";
            txtSearchPos.Text = string.Empty;
            SetBtnSearchItem();
        }

        private void TxtSearchPos_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(txtSearchPos.Text.Trim()))
            {
                lnGroupButton.Visibility = ViewStates.Gone;
                buttonSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
                buttonSearch.Click += ButtonSearch_Click;
            }
            else
            {
                lnGroupButton.Visibility = ViewStates.Visible;
                buttonSearch.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }

        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            txtSearchPos.Text = string.Empty;
            txtSearchPos.ClearFocus();
            lnGroupButton.Visibility = ViewStates.Visible;
        }

        private void TxtSearchPos_Click(object sender, EventArgs e)
        {
            lnGroupButton.Visibility = ViewStates.Gone;
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void buttonOrder_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(OrderActivity)));
            OrderActivity.SetTranDetail(tranWithDetails);
            //this.Finish();
        }

        public static async void SetCustomer()
        {
            try
            {
                if (DataCashing.SysCustomerID == 999)
                {
                    if (tranWithDetails.tran != null)
                    {
                        if (tranWithDetails.tran.SysCustomerID != 999)
                        {
                            tranWithDetails = await BLTrans.RemovePerson(tranWithDetails);
                        }
                    }

                    btnCustomer1.Visibility = ViewStates.Visible;
                    lnNoCustomer.Visibility = ViewStates.Visible;

                    btnCustomer2.Visibility = ViewStates.Gone;
                    lnHaveCustomer.Visibility = ViewStates.Gone;
                }
                else
                {
                    btnCustomer1.Visibility = ViewStates.Gone;
                    lnNoCustomer.Visibility = ViewStates.Gone;

                    btnCustomer2.Visibility = ViewStates.Visible;
                    lnHaveCustomer.Visibility = ViewStates.Visible;

                    CustomerManage customerManage = new CustomerManage();
                    var listCustomer = new List<Customer>();
                    listCustomer = await customerManage.GetAllCustomer();
                    selectCus = listCustomer.Where(x => x.SysCustomerID == DataCashing.SysCustomerID).FirstOrDefault();
                    if (selectCus == null) return;
                    if (tranWithDetails.tran.SysCustomerID != DataCashing.SysCustomerID)
                    {
                        tranWithDetails.tran.SysCustomerID = selectCus.SysCustomerID;
                        tranWithDetails.tran.CustomerName = selectCus.CustomerName;

                        tranWithDetails = await BLTrans.ChoosePerson(tranWithDetails, selectCus);
                        tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    }
                    txtNameCustomer.Text = tranWithDetails.tran.CustomerName?.ToString();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetCustomer at POS");
                return;
            }
        }

        private void BtnCustomer_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(SelectCustomerActivity)));
            SelectCustomerActivity.SetTranDetail(tranWithDetails);
            SelectCustomerActivity.PageOpen = "Pos";
        }

        private void BtnBasket_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(QuantityActitvity));
            addQuantity = 1;
            QuantityActitvity.SetBackQuantity(addQuantity);
        }

        private void BtnDummy_Click(object sender, EventArgs e)
        {
            DummyActivity.SetTranDetail(tranWithDetails);
            StartActivity(typeof(DummyActivity));
            //this.Finish();
        }

        private async void ImagebtnBack_Click(object sender, EventArgs e)
        {
            //เพิ่ม dialog
            if (tranWithDetails.tran.TranType == 'O')
            {
                if (DataCashing.ModifyTranOrder)
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.pos_dialog_saveorder.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }
                else
                {
                    await Cancel();
                }
            }
            base.OnBackPressed();
        }

        public async override void OnBackPressed()
        {
            try
            {
                //เพิ่ม dialog
                if (tranWithDetails.tran.TranType == 'O')
                {
                    if (DataCashing.ModifyTranOrder)
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_saveorder.ToString();
                        bundle.PutString("message", myMessage);
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        return;
                    }
                    else
                    {
                        await Cancel();
                    }
                }

                //Restart app
                //ClearApplicationData();
                //RestartApplication(); //ใช้แล้วทำให้จ๊อบไม่ทำงาน

                base.OnBackPressed();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBackPressed at POS");
            }
        }

        internal static async void DialogCancelSave()
        {
            try
            {
                await Cancel();
                PosActivity.pos.Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DialogCancelSave");
                return;
            }
        }

        public static async Task Cancel()
        {
            try
            {
                if (tranWithDetails.tran.TranType == 'O')
                {
                    await Utils.CancelTranOrder(tranWithDetails);
                    DataCashing.isCurrentOrder = false;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Cancel");
                return;
            }
        }

        private void LnPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
                {
                    return;
                }
                StartActivity(new Android.Content.Intent(Application.Context, typeof(CartActivity)));
                CartActivity.SetTranDetail(tranWithDetails);
                //this.Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnPayment_Click at POS");
                return;
            }
        }

        private void ImgScanQR_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(CartScanActivity)));
                CartScanActivity.SetTranDetail(tranWithDetails);
                //this.Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                return;
            }
        }

        private void ImgSelect_Click(object sender, System.EventArgs e)
        {
            if (!flagView)
            {
                imgSelect.SetImageResource(Resource.Mipmap.ViewList);
                Preferences.Set("ViewPos", "Grid");
                flagView = true;
            }
            else
            {
                imgSelect.SetImageResource(Resource.Mipmap.ViewGroup);
                Preferences.Set("ViewPos", "Row");
                flagView = false;
            }
            flagSetViewShow();
        }

        private void ClearApplicationData()
        {
            try
            {
                // Clear application data such as cache, databases, shared preferences, etc.
                var cacheDir = CacheDir;
                if (cacheDir != null)
                {
                    var cacheFiles = cacheDir.ListFiles();
                    foreach (var file in cacheFiles)
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
            }
        }

        private void RestartApplication()
        {
            // Restart the application by relaunching the main activity
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);

            // Finish the current activity
            Finish();

            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }


        private void DisplayMemoryUsage()
        {
            TextView textView = FindViewById<TextView>(Resource.Id.textView1);
            // เรียกใช้งานเมทอด GetMemoryInfo เพื่อรับข้อมูลเกี่ยวกับการใช้งาน RAM
            ActivityManager activityManager = (ActivityManager)GetSystemService(Context.ActivityService);
            ActivityManager.MemoryInfo memoryInfo = new ActivityManager.MemoryInfo();
            activityManager.GetMemoryInfo(memoryInfo);

            var totalMemory = memoryInfo.TotalMem / (1024 * 1024); // ขนาดทั้งหมดของ RAM ในเครื่อง (หน่วยเป็น MB)
            // หา usedRAM ของแอปพลิเคชัน
            double usedRAM = Utils.GetUsedRAM(activityManager, memoryInfo); // หน่วยเป็น MB

            JobScheduler jobScheduler = (JobScheduler)GetSystemService(Context.JobSchedulerService);
            // แสดงผลลัพธ์ใน Log
            textView.Text = "usedRAM " + usedRAM + " totalMemory " + totalMemory + "\n" +  " JobStartedCount = " + GabanaJob.getJobStartedCount();

        }

        private async void Pos_Adapter_Grid_ItemClick(object sender, int e)
        {
            try
            {
                if (listItem[e].SysItemID == 0)
                {
                    PosActivity.pos.AddNewItem();
                }
                else
                {
                    int positionClick = e;
                    view = gridLayoutManager.FindViewByPosition(positionClick);
                    //ImageShow = view.FindViewById<ImageView>(Resource.Id.imgShowAnime);
                    cardView = view.FindViewById<CardView>(Resource.Id.cardViewGrid);
                    TextView ItemName = view.FindViewById<TextView>(Resource.Id.txtNameItem);

                    view.Enabled = false;
                    //Toast.MakeText(this, "View is disabled", ToastLength.Short).Show();

                    //ขั้นตอน
                    //เลือกสินค้า
                    //เช็ค Fdisplay 0 ไม่แสดง 1 แสดง
                    //ตกลง  //Animetion   //เพิ่มสินค้า
                    //ยกเลิก //แสดงรายการสินค้า

                    ItemExSizeManage exSizeManage = new ItemExSizeManage();
                    List<ItemExSize> exSizes = new List<ItemExSize>();
                    exSizes = await exSizeManage.GetItemSize(DataCashingAll.MerchantId, (int)listItem[e].SysItemID);
                    if (exSizes == null)
                    {
                        exSizes = new List<ItemExSize>();
                    }
                    if (listItem[e].FDisplayOption == 1 || exSizes.Count > 0)
                    {
                        Log.Debug("POSActivitypass", listItem[e].SysItemID.ToString());
                        DetailNo++;
                        StartActivity(typeof(OptionActivity));
                        OptionActivity.SetDataItemfromPOS(listItem[e]);

                        int indexSelect = tranWithDetails.tranDetailItemWithToppings.Count;
                        indexSelect++;
                        OptionActivity.PositionClick = e;
                        OptionActivity.SetTranDetail(tranWithDetails);
                    }
                    else
                    {
                        TranDetailItemNew DetailItem = new TranDetailItemNew()
                        {
                            SysItemID = listItem[e].SysItemID,
                            MerchantID = DataCashingAll.MerchantId,
                            SysBranchID = DataCashingAll.SysBranchId,
                            TranNo = tranWithDetails.tran.TranNo,
                            ItemName = listItem[e].ItemName,
                            SaleItemType = listItem[e].SaleItemType,
                            FProcess = 1,
                            TaxType = listItem[e].TaxType,
                            Quantity = (decimal)DataCashing.setQuantityToCart,
                            Price = listItem[e].Price,
                            ItemPrice = listItem[e].Price,
                            Discount = 0,
                            EstimateCost = listItem[e].EstimateCost,
                            SizeName = null,
                            Comments = listItem[e].Comments,
                            DetailNo = DetailNo,
                        };

                        DetailNo++;
                        List<TranDetailItemTopping> tranDetailItem = new List<TranDetailItemTopping>();
                        tranDetailItemWithTopping = new TranDetailItemWithTopping()
                        {
                            tranDetailItem = DetailItem,
                            tranDetailItemToppings = tranDetailItem,
                        };

                        var Item = PosActivity.tranWithDetails.tranDetailItemWithToppings.ToList().Where(x => x.tranDetailItem.SysItemID == listItem[e].SysItemID).FirstOrDefault();
                        if (Item != null)
                        {
                            if (DataCashing.setQuantityToCart == 1)
                            {
                                ItemName.Text = (int)Item.tranDetailItem.Quantity + 1 + "x " + listItem[e].ItemName?.ToString();
                            }
                            else
                            {
                                ItemName.Text = (int)Item.tranDetailItem.Quantity + DataCashing.setQuantityToCart + "x " + listItem[e].ItemName?.ToString();
                            }
                        }
                        else
                        {
                            if (DataCashing.setQuantityToCart == 1)
                            {
                                ItemName.Text = "1x " + listItem[e].ItemName?.ToString();
                            }
                            else
                            {
                                ItemName.Text = DataCashing.setQuantityToCart + "x " + listItem[e].ItemName?.ToString();
                            }
                        }

                        string conColor = Utils.SetBackground((int)listItem[e].Colors);
                        colorAnimetion = Android.Graphics.Color.ParseColor(conColor);

                        AnimetionStartGrid();
                        SelectItemtoCart(e);
                    }
                    //Toast.MakeText(this, "View is Enabled", ToastLength.Short).Show();
                    view.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                view.Enabled = true;
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Pos_Adapter_Grid_ItemClick at POS");
                return;
            }
        }

        private async void Pos_Adapter_Row_ItemClick(object sender, int e)
        {
            try
            {
                if (listItem[e].SysItemID == 0)
                {
                    PosActivity.pos.AddNewItem();
                }
                else
                {
                    int positionClick = e;
                    view = mLayoutManager.FindViewByPosition(positionClick);
                    //ImageShow = view.FindViewById<ImageView>(Resource.Id.imgShowAnime);
                    //imageViewRow = view.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
                    TextView ItemName = view.FindViewById<TextView>(Resource.Id.txtNameItem);

                    view.Enabled = false;
                    //ขั้นตอน
                    //เลือกสินค้า
                    //เช็ค Fdisplay 0 ไม่แสดง 1 แสดง
                    //ตกลง  //Animetion   //เพิ่มสินค้า
                    //ยกเลิก //แสดงรายการสินค้า

                    ItemExSizeManage exSizeManage = new ItemExSizeManage();
                    List<ItemExSize> exSizes = new List<ItemExSize>();
                    exSizes = await exSizeManage.GetItemSize(DataCashingAll.MerchantId, (int)listItem[e].SysItemID);
                    if (exSizes == null)
                    {
                        exSizes = new List<ItemExSize>();
                    }
                    if (listItem[e].FDisplayOption == 1 || exSizes.Count > 0)
                    {
                        DetailNo++;
                        StartActivity(typeof(OptionActivity));
                        OptionActivity.SetDataItemfromPOS(listItem[e]);

                        int indexSelect = tranWithDetails.tranDetailItemWithToppings.Count;
                        indexSelect++;
                        OptionActivity.PositionClick = e;
                        OptionActivity.SetTranDetail(tranWithDetails);
                    }
                    else
                    {
                        //OptionActivity.SetDataItemfromPOS(null);

                        TranDetailItemNew DetailItem = new TranDetailItemNew()
                        {
                            SysItemID = listItem[e].SysItemID,
                            MerchantID = DataCashingAll.MerchantId,
                            SysBranchID = DataCashingAll.SysBranchId,
                            TranNo = tranWithDetails.tran.TranNo,
                            ItemName = listItem[e].ItemName,
                            SaleItemType = listItem[e].SaleItemType,
                            FProcess = 1,
                            TaxType = listItem[e].TaxType,
                            Quantity = (decimal)DataCashing.setQuantityToCart,
                            Price = listItem[e].Price,
                            ItemPrice = listItem[e].Price,
                            Discount = 0,
                            EstimateCost = listItem[e].EstimateCost,
                            SizeName = null,
                            Comments = listItem[e].Comments,
                            DetailNo = DetailNo,
                        };

                        DetailNo++;
                        List<TranDetailItemTopping> tranDetailItem = new List<TranDetailItemTopping>();
                        tranDetailItemWithTopping = new TranDetailItemWithTopping()
                        {
                            tranDetailItem = DetailItem,
                            tranDetailItemToppings = tranDetailItem,
                        };

                        string Quantity = string.Empty;
                        string itemName = string.Empty;
                        var Item = PosActivity.tranWithDetails.tranDetailItemWithToppings.ToList().Where(x => x.tranDetailItem.SysItemID == listItem[e].SysItemID).FirstOrDefault();
                        if (Item != null)
                        {
                            if (DataCashing.setQuantityToCart == 1)
                            {
                                Quantity = (int)Item.tranDetailItem.Quantity + 1 + "x " + " ";
                                itemName = listItem[e].ItemName?.ToString();
                            }
                            else
                            {
                                Quantity = (int)Item.tranDetailItem.Quantity + DataCashing.setQuantityToCart + "x " + " ";
                                itemName = listItem[e].ItemName?.ToString();
                            }
                        }
                        else
                        {
                            if (DataCashing.setQuantityToCart == 1)
                            {
                                Quantity = "1x " + " ";
                                itemName = listItem[e].ItemName?.ToString();
                            }
                            else
                            {
                                Quantity = DataCashing.setQuantityToCart + "x " + " ";
                                itemName = listItem[e].ItemName?.ToString();
                            }
                        }

                        Android.Text.SpannableStringBuilder builder = new Android.Text.SpannableStringBuilder();

                        Android.Text.SpannableString redSpannable = new Android.Text.SpannableString(Quantity);
                        redSpannable.SetSpan(new Android.Text.Style.ForegroundColorSpan(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null)), 0, Quantity.Length, 0);
                        builder.Append(redSpannable);

                        Android.Text.SpannableString Spannable = new Android.Text.SpannableString(itemName);
                        Spannable.SetSpan(new Android.Text.Style.ForegroundColorSpan(Application.Context.Resources.GetColor(Resource.Color.textblackcolor, null)), 0, itemName.Length, 0);
                        builder.Append(Spannable);

                        ItemName.SetText(builder, TextView.BufferType.Spannable);

                        string conColor = Utils.SetBackground((int)listItem[e].Colors);
                        colorAnimetion = Android.Graphics.Color.ParseColor(conColor);

                        AnimetionStartRow();
                        SelectItemtoCart(e);
                    }
                    view.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                view.Enabled = true;
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Pos_Adapter_Row_ItemClick at POS");
                return;
            }
        }

        private async void GetnoteCategory()
        {
            try
            {
                NoteManage noteManage = new NoteManage();
                List<NoteCategory> noneCategory = new List<NoteCategory>();
                NoteCategoryManage category = new NoteCategoryManage();
                List<NoteCategory> lstCategoryNote = new List<NoteCategory>();
                List<Note> lstNotefromCate = new List<Note>();

                lstNotefromCate = await noteManage.GetNoteOnlyNoneGroup(DataCashingAll.MerchantId);
                if (lstNotefromCate.Count > 0)
                {
                    if (DataCashing.Language == "th")
                    {
                        noneCategory.Add(new NoteCategory { MerchantID = DataCashingAll.MerchantId, SysNoteCategoryID = 0, Ordinary = null, Name = "ไม่มีกลุ่ม", DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow, DataStatus = 'I', FWaitSending = 1, WaitSendingTime = DateTime.UtcNow });
                    }
                    else
                    {
                        noneCategory.Add(new NoteCategory { MerchantID = DataCashingAll.MerchantId, SysNoteCategoryID = 0, Ordinary = null, Name = "None", DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow, DataStatus = 'I', FWaitSending = 1, WaitSendingTime = DateTime.UtcNow });
                    }
                }

                lstCategoryNote = await category.GetNoteCategoryOption();
                noneCategory.AddRange(lstCategoryNote);
                OptionActivity.SetlistCategoryNote(noneCategory);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetnoteCategory at POS");
                return;
            }
        }

        private Bitmap getScreenShotFromView(int width, int height, string viewshow)
        {
            try
            {
                string show = string.Empty;
                show = viewshow;
                Bitmap screenshot = Android.Graphics.Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
                if (show == "grid")
                {
                    screenshot = Android.Graphics.Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);

                    Canvas canvas = new Canvas(screenshot);
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        canvas.DrawBitmap(screenshot, width, height, null);
                    }
                    else
                    {
                        canvas.SetViewport(width, height);
                    }
                    view.Draw(canvas);
                }
                else
                {
                    screenshot = Android.Graphics.Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

                    Canvas canvas = new Canvas(screenshot);
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        canvas.DrawBitmap(screenshot, 0, 0, null);
                    }
                    else
                    {
                        canvas.SetViewport(width, height);
                    }
                    view.Draw(canvas);
                }
                return screenshot;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                Log.Debug("error", ex.Message);
                Bitmap err;
                return err = Android.Graphics.Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
            }
        }

        async void AnimetionStartGrid()
        {
            try
            {
                await Task.Run(() =>
                {
                    RunOnUiThread(() =>
                    {
                        int itemW = 0;
                        int itemH = 0;
                        int[] originalPos = new int[2];
                        int x = 0;
                        int y = 0;

                        using (var ImageShow = view.FindViewById<ImageView>(Resource.Id.imgShowAnime))
                        {
                            if (ImageShow != null & cardView != null)
                            {
                                ImageShow.GetLocationInWindow(originalPos);
                                x = originalPos[0];
                                y = originalPos[1];

                                itemW = cardView.Width + 60;
                                itemH = cardView.Height + 60;
                            }
                        }

                        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                        var width = mainDisplayInfo.Width;
                        var height = mainDisplayInfo.Height;

                        using (Bitmap bitmap = getScreenShotFromView(itemW, itemH, "grid"))
                        {
                            //Bitmap bitmap = getScreenShotFromView(itemW, itemH, "grid");
                            //Canvas canvas = new Canvas(bitmap);
                            //canvas.DrawBitmap(bitmap, x, y, null);

                            ImageShowPOS.SetBackgroundDrawable(new BitmapDrawable(bitmap));
                            ImageShowPOS.LayoutParameters.Height = itemH;
                            ImageShowPOS.LayoutParameters.Width = itemW;

                            //set position ตอนเริ่มตกใส่ตะกร้า
                            ImageShowPOS.SetY(y - 80);
                            ImageShowPOS.SetX(x - 20);
                            ImageShowPOS.BringToFront();

                            ImageShowPOS.Animate()
                                    .SetDuration(400) //animation duration in milliseconds
                                    .Alpha((float)1)
                                    .TranslationY((float)height / (float)0.5)
                                    .TranslationX((float)width / 2)
                                    .WithStartAction(new Java.Lang.Runnable(() =>
                                    {
                                        ImageShowPOS.ScaleX = (float)0.5;
                                        ImageShowPOS.ScaleY = (float)0.5;
                                    }))
                                    .Start();

                            ImageShowPOS.Animate()
                                    .SetDuration(400)
                                    .Alpha((float)0.7)
                                    .TranslationY((float)height / (float)0.5)
                                    .TranslationX((float)width / 5)
                                    .ScaleX(1)
                                    .ScaleY(1)
                                    .WithEndAction(new Java.Lang.Runnable(() =>
                                    {
                                        //เมื่อ Animetion จบ กำหนดต่ำแหน่ง imageshow เพื่อกดอีกครั้ง
                                        ImageShowPOS.SetBackgroundDrawable(null);
                                        ImageShowPOS.LayoutParameters.Height = itemH / 5;
                                        ImageShowPOS.LayoutParameters.Width = itemW / 5;
                                    }))
                                    .Start();

                            bitmap.Dispose();
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("AnimetionStartGrid at POS");
                return;
            }
        }

        async void AnimetionStartRow()
        {
            try
            {
                await Task.Run(() =>
                {
                    RunOnUiThread(() =>
                    {
                        int itemW = 0;
                        int itemH = 0;
                        int[] originalPos = new int[2];
                        int x = 0;
                        int y = 0;

                        using (var imageViewRow = view.FindViewById<ImageView>(Resource.Id.imageViewcolorItem))
                        {
                            if (imageViewRow != null)
                            {
                                imageViewRow.GetLocationInWindow(originalPos);
                                x = originalPos[0];
                                y = originalPos[1];

                                itemW = imageViewRow.Width + 40;
                                itemH = imageViewRow.Height + 40;
                            }
                        }

                        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                        var width = mainDisplayInfo.Width;
                        var height = mainDisplayInfo.Height;

                        using (Bitmap bitmap = getScreenShotFromView(itemW, itemH, "row"))
                        {
                            Canvas canvas = new Canvas(bitmap);
                            canvas.DrawBitmap(bitmap, x, y, null);

                            //Bitmap bitmap = getScreenShotFromView(itemW, itemH, "row");
                            //Canvas canvas = new Canvas(bitmap);
                            //canvas.DrawBitmap(bitmap, x, y, null);

                            ImageShowPOS.SetBackgroundDrawable(new BitmapDrawable(bitmap));
                            ImageShowPOS.LayoutParameters.Height = itemH;
                            ImageShowPOS.LayoutParameters.Width = itemW;

                            ImageShowPOS.SetY(y - 150);
                            ImageShowPOS.SetX(x);
                            ImageShowPOS.BringToFront();

                            var a = ImageShowPOS.Elevation;
                            var b = lnPayment.Elevation;

                            ImageShowPOS.Animate()
                                    .SetDuration(400)
                                    .Alpha((float)1)
                                    .TranslationY((float)height)
                                    .TranslationX((float)width / (float)1.5)
                                    .Start();

                            ImageShowPOS.LayoutParameters.Height = itemH;
                            ImageShowPOS.LayoutParameters.Width = itemW;

                            ImageShowPOS.Animate()
                                    .SetDuration(400)
                                    .Alpha((float)0.9)
                                    .TranslationY((float)height)
                                    .TranslationX((float)width / 3)
                                    .Start();

                            ImageShowPOS.LayoutParameters.Height = itemH / 2;
                            ImageShowPOS.LayoutParameters.Width = itemW / 2;

                            ImageShowPOS.Animate()
                                   .SetDuration(400)
                                   .Alpha((float)0.8)
                                   .TranslationY((float)height)
                                   .TranslationX((float)width / 4)
                                   .Start();

                            ImageShowPOS.LayoutParameters.Height = itemH / 2;
                            ImageShowPOS.LayoutParameters.Width = itemW / 2;


                            ImageShowPOS.Animate()
                                    .SetDuration(400)
                                    .Alpha((float)0.7)
                                    .TranslationY((float)height)
                                    .TranslationX((float)width / 5)
                                    .WithEndAction(new Java.Lang.Runnable(() =>
                                    {
                                        //เมื่อ Animetion จบ กำหนดต่ำแหน่ง imageshow เพื่อกดอีกครั้ง
                                        ImageShowPOS.SetBackgroundDrawable(null);
                                    }))
                                    .Start();
                            bitmap.Dispose();
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("AnimetionStartRow at POS");
                return;
            }
        }

        public async Task ShowDetail()
        {
            try
            {
                //ข้อมูลปกติ     //0 All -2 Favorite  -3 Extra Topping
                listItemBody = new List<Item>();
                switch (fillter)
                {
                    case 0:
                        listItemBody = DefaultDataItem;/*.Where(x=>x.SaleItemType != 'T' && x.DataStatus != 'D').ToList();*/
                        listToppingBody = DefaultDataTopping;
                        break;
                    case -2:
                        listItemBody = listFavorite;
                        break;
                    case -3:
                        listItemBody = DefaultDataTopping;
                        break;
                    default:
                        listItemBody = await GetItemfromCategory();
                        break;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowDetail at POS");
                return;
            }
        }

        public async Task<List<Item>> GetListItem()
        {
            try
            {
                listItemBody = new List<Item>();
                ItemManage itemManage = new ItemManage();
                listItemBody = await itemManage.GetAllItem();
                if (listItemBody == null)
                {
                    Toast.MakeText(this, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
                    return null;
                }
                return listItemBody;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListItem at POS");
                return null;
            }
        }

        public async Task<List<Item>> GetListTopping()
        {
            try
            {
                listToppingBody = new List<Item>();
                ItemManage itemManage = new ItemManage();
                listToppingBody = await itemManage.GetToppingItem();
                if (listToppingBody == null)
                {
                    Toast.MakeText(this, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
                    return null;
                }
                return listToppingBody;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListItem at POS");
                return null;
            }
        }

        public async Task<List<Item>> GetItemfromCategory()
        {
            try
            {
                List<Item> items = new List<Item>();
                ItemManage itemManage = new ItemManage();
                items = await itemManage.GetItembyCategory(DataCashingAll.MerchantId, fillter);

                // Filter out items with SaleItemType 'T'
                items = items?.Where(x => x.SaleItemType != 'T').ToList();

                return items;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetItemfromCategory at POS");
                return null;
            }
        }

        public void flagSetViewShow()
        {
            try
            {
                // falgView = false = row,flagView = true = grid
                if (listItemBody != null)
                {
                    var addNewItem = listItemBody.Where(x => x.SysItemID == 0).FirstOrDefault();
                    var LoginType = Preferences.Get("LoginType", "");
                    var check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "item");
                    if (addNewItem == null && fillter != -2 && check) // -2 Favorite จะไม่แสดงปุ่มเพิ่มสินค้าใหม่ที่ Favorite
                    {
                        listItemBody.Add(new Item { SysItemID = 0 });
                    }

                    if (flagView == false)
                    {
                        imgSelect.SetImageResource(Resource.Mipmap.ViewGroup);
                        listItem = new ListItem(listItemBody);
                        mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                        Pos_Adapter_Row pos_Adapter_Row = new Pos_Adapter_Row(listItem, checkNet);
                        RecyclerViewShow.HasFixedSize = true;

                        RecyclerViewShow.SetLayoutManager(mLayoutManager);
                        RecyclerViewShow.SetItemViewCacheSize(100);
                        RecyclerViewShow.SetAdapter(pos_Adapter_Row);
                        pos_Adapter_Row.ItemClick += Pos_Adapter_Row_ItemClick;
                    }
                    else
                    {
                        imgSelect.SetImageResource(Resource.Mipmap.ViewList);
                        listItem = new ListItem(listItemBody);
                        Pos_Adapter_Grid pos_Adapter_Grid = new Pos_Adapter_Grid(listItem, checkNet);
                        gridLayoutManager = new GridLayoutManager(this, 3);
                        gridLayoutManager.Orientation = RecyclerView.Vertical;

                        RecyclerViewShow.HasFixedSize = true;
                        RecyclerViewShow.SetLayoutManager(gridLayoutManager);
                        RecyclerViewShow.AddItemDecoration(new SpacesItemDecoration(2));
                        RecyclerViewShow.SetItemViewCacheSize(100);
                        RecyclerViewShow.SetAdapter(pos_Adapter_Grid);
                        pos_Adapter_Grid.ItemClick += Pos_Adapter_Grid_ItemClick;

                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("flagSetViewShow at POS");
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private class MySpanSizeLookup : GridLayoutManager.SpanSizeLookup
        {
            private Pos_Adapter_Grid gridadapter;

            public MySpanSizeLookup(Pos_Adapter_Grid gridadapter)
            {
                this.gridadapter = gridadapter;
            }

            public override int GetSpanSize(int position)
            {
                switch (gridadapter.GetItemViewType(position))
                {
                    case 1: return 1;
                    case 0: return 3;
                    default: return -1;
                }
            }
        }

        private void ImgShare_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ShareActivity)));
        }

        public void SelectItemtoCart(int positionClick)
        {
            try
            {
                if (listItem[positionClick].SysItemID == 0)
                {
                    return;
                }

                var row = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == listItem[positionClick].SysItemID);

                tranWithDetails = BLTrans.ChooseItemTran(tranWithDetails, tranDetailItemWithTopping);
                DataCashing.ModifyTranOrder = true;
                tranWithDetails = BLTrans.Caltran(tranWithDetails);

                // Update UI elements based on transaction details
                UpdateUIBasedOnTransaction(tranWithDetails);

                // Update the quantity text
                UpdateQuantityText();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectItemtoCart at POS");
            }
        }

        private void UpdateUIBasedOnTransaction(TranWithDetailsLocal tranWithDetails)
        {
            var quantity = (int)tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity);

            // Update UI elements based on quantity
            if (quantity > 0)
            {
                lnPayment.SetBackgroundResource(Resource.Drawable.btnblue);
                lnPayment.SetPadding(0, 5, 0, 5);
                textSum.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                txtNoItem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                lnPayment.SetBackgroundResource(Resource.Drawable.btnborderblue);
                lnPayment.SetPadding(0, 5, 0, 5);
                textSum.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                txtNoItem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }

            // Update the text based on the language
            txtNoItem.Text = (DataCashing.Language == "th") ? $"{quantity:#,###} รายการ" : (quantity == 1) ? $"{quantity:#,###} item" : $"{quantity:#,###} items";
        }

        private void UpdateQuantityText()
        {
            txtQuantity.Text = DataCashing.setQuantityToCart.ToString() + "x";
            textSum.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal) + " " + CURRENCYSYMBOLS;
        }

        public void AddNewItem()
        {
            try
            {
                if (nameCategory == "ท็อปปิ้ง" | nameCategory == "Extra Topping")
                {
                    StartActivity(new Intent(Application.Context, typeof(AddExtraToppingActivity)));
                }
                else
                {
                    long Category = 0;
                    if (fillter == 0)
                    {
                        Category = 0;
                    }
                    else
                    {
                        Category = fillter;
                    }

                    StartActivity(new Intent(Application.Context, typeof(AddItemActivity)));
                    AddItemActivity.SetCategoryFromPOS(Category);
                    AddItemActivity.tabSelected = "Item";

                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("AddNewItem at POS");
            }
        }

        public void Resume()
        {
            OnResume();
        }

        protected async override void OnResume()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                base.OnResume();
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                //DisplayMemoryUsage();

                await Task.Run(async () => { await CheckJwt(); });
                Log.Debug("swipapp", "POS OnResume");
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                IsActive = true;
                SetCustomer();

                checkNet = await GabanaAPI.CheckNetWork();

                if (DataCashingAll.flagItemChange || DataCashingAll.flagCategoryChange)
                {
                    //สินค้าทุกตัวนับรวมสินค้าที่ถูกลบด้วย
                    ItemManage itemManage = new ItemManage();
                    AllItem = await itemManage.GetAll(DataCashingAll.MerchantId);
                    AllItemStatusD = AllItem.Where(x => x.DataStatus == 'D').ToList();

                    DefaultDataItem = await GetListItem();
                    DefaultDataTopping = await GetListTopping();

                    DefaultAllItem = new List<Item>();
                    DefaultAllItem.AddRange(DefaultDataItem);
                    DefaultAllItem.AddRange(DefaultDataTopping);

                    SetTabMenu();
                    SetTabShowMenu();
                    GetnoteCategory();

                    DataCashingAll.flagItemChange = false;
                    DataCashingAll.flagCategoryChange = false;
                }

                //POS ScanBarcode
                if (DataCashing.flagScan)
                {
                    DataCashing.flagScan = false;
                }

                //Cart 
                if (DataCashing.flagCart)
                {
                    DataCashing.flagCart = false;
                }

                if (DataCashing.flagDummy)
                {
                    DataCashing.flagDummy = false;
                }

                ShowPayment();

                if (tranWithDetails.tranDetailItemWithToppings.Count == 0 && tranWithDetails.tran?.SysCustomerID == 999)
                {
                    tranWithDetails = null;
                    tranWithDetails = await Utils.initialData();
                }

                await ShowDetail();
                flagSetViewShow();
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at POS");
            }
        }


        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        private async void TxtSearchPos_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                //SetBtnSearchItem();
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    listItemBody = await SearchItemfromCategory();
                    flagSetViewShow();
                    txtSearchPos.ClearFocus();
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
                    txtSearchPos.Text += input;
                    txtSearchPos.SetSelection(txtSearchPos.Text.Length);
                    return;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TxtSearchPos_KeyPress at POS");
            }
        }

        private async void TxtSearchPos_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchItem = txtSearchPos.Text.Trim();
            if (string.IsNullOrEmpty(SearchItem))
            {
                lnGroupButton.Visibility = ViewStates.Visible;
                await ShowDetail();
                flagSetViewShow();
            }
            else
            {
                View view = this.CurrentFocus;
                if (view != null)
                {
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.ShowSoftInputFromInputMethod(view.WindowToken, ShowFlags.Forced);
                }
            }

            SetBtnSearchItem();
        }

        private void SetBtnSearchItem()
        {
            if (string.IsNullOrEmpty(SearchItem))
            {
                buttonSearch.SetBackgroundResource(Resource.Mipmap.Search);
                buttonSearch.Enabled = false;
            }
            else
            {
                buttonSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
                buttonSearch.Enabled = true;
            }
        }

        public async Task<List<Item>> SearchItemfromCategory()
        {
            try
            {
                listItemBody = new List<Item>();
                int merchantId = DataCashingAll.MerchantId;

                if (string.IsNullOrEmpty(SearchItem))
                {
                    return listItemBody;
                }

                Func<Item, bool> searchCondition = x =>
                    x.MerchantID == merchantId &&
                    (x.ItemName.ToLower().Contains(SearchItem.ToLower()) ||
                     (!string.IsNullOrEmpty(x.ItemCode) && x.ItemCode.ToLower().Contains(SearchItem.ToLower())));

                if (fillter == 0)
                {
                    listItemBody = DefaultDataItem.Where(searchCondition).OrderBy(x => x.ItemName).ToList();
                }
                else if (fillter == -2)
                {
                    listItemBody = DefaultDataItem.Where(searchCondition).Where(x => x.FavoriteNo > 0).OrderBy(x => x.ItemName).ToList();
                }
                else if (fillter == -3)
                {
                    listItemBody = DefaultDataTopping.Where(x => x.MerchantID == merchantId && x.ItemName.ToLower().Contains(SearchItem.ToLower())).OrderBy(x => x.ItemName).ToList();
                }
                else
                {
                    listItemBody = DefaultDataItem.Where(x => x.MerchantID == merchantId && x.SysCategoryID == fillter && searchCondition(x)).OrderBy(x => x.ItemName).ToList();
                }

                return listItemBody;

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SearchItemfromCategory at POS");
                return null;
            }
        }

        async void getTranOrderDetail(Tran tran)
        {
            try
            {
                var lsttranDetailItemWithToppings = new List<TranDetailItemWithTopping>();
                TranDetailItemWithTopping detailItemWithTopping = new TranDetailItemWithTopping();

                tranWithDetails = new TranWithDetailsLocal();
                tranWithDetails.tran = tran;
                tranWithDetails.tran.TranDate = Utils.GetTranDate(tranWithDetails.tran.TranDate);
                tranWithDetails.tran.LastDateModified = Utils.GetTranDate(tranWithDetails.tran.LastDateModified);
                tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(tranWithDetails.tran.WaitSendingTime);

                TranDetailItemManage tranDetailItemManage = new TranDetailItemManage();
                var tranDetail = await tranDetailItemManage.GetTranDetailItem(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);

                foreach (var item in tranDetail)
                {
                    //Detail Item
                    TranDetailItemNew DetailItem = new TranDetailItemNew()
                    {
                        Amount = item.Amount,
                        Comments = item.Comments,
                        CumulativeSum = item.CumulativeSum,
                        DetailNo = item.DetailNo,
                        Discount = item.Discount,
                        DiscountPromotion = item.DiscountPromotion,
                        DiscountRedeem = item.DiscountRedeem,
                        EstimateCost = item.EstimateCost,
                        FmlDiscountRow = item.FmlDiscountRow,
                        FProcess = item.FProcess,
                        ItemName = item.ItemName,
                        MerchantID = item.MerchantID,
                        Price = item.Price,
                        PricePerWeight = item.PricePerWeight,
                        ProfitAmount = item.ProfitAmount,
                        Quantity = item.Quantity,
                        RedeemCode = item.RedeemCode,
                        SaleItemType = item.SaleItemType,
                        SizeName = item.SizeName,
                        SubAmount = item.SubAmount,
                        SumToppingEstimateCost = item.SumToppingEstimateCost,
                        SumToppingPrice = item.SumToppingPrice,
                        SysBranchID = item.SysBranchID,
                        SysItemID = item.SysItemID,
                        TaxBaseAmount = item.TaxBaseAmount,
                        TaxType = item.TaxType,
                        TotalPrice = item.TotalPrice,
                        TranNo = item.TranNo,
                        UnitName = item.UnitName,
                        VatAmount = item.VatAmount,
                        Weight = item.Weight,
                        WeightTranDisc = item.WeightTranDisc,
                        WeightUnitName = item.WeightUnitName,
                        ItemPrice = item.ItemPrice,
                    };
                    List<TranDetailItemTopping> lstitemDetail = new List<TranDetailItemTopping>();
                    //Detail ItemTopping
                    TranDetailItemToppingManage toppingManage = new TranDetailItemToppingManage();
                    var tranTopping = await toppingManage.GetTranDetailItemTopping(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo, (int)item.DetailNo);
                    foreach (var itemtopping in tranTopping)
                    {
                        TranDetailItemTopping itemDetail = new TranDetailItemTopping()
                        {
                            MerchantID = itemtopping.MerchantID,
                            SysBranchID = itemtopping.SysBranchID,
                            TranNo = itemtopping.TranNo,
                            DetailNo = itemtopping.DetailNo,
                            ToppingNo = itemtopping.ToppingNo,
                            ItemName = itemtopping.ItemName,//toppping
                            SysItemID = itemtopping.SysItemID,
                            UnitName = itemtopping.UnitName,
                            RegularSizeName = itemtopping.RegularSizeName,
                            Quantity = itemtopping.Quantity,
                            ToppingPrice = itemtopping.ToppingPrice,
                            EstimateCost = itemtopping.EstimateCost,
                            Comments = itemtopping.Comments
                        };
                        lstitemDetail.Add(itemDetail);
                    }

                    detailItemWithTopping = new TranDetailItemWithTopping();
                    detailItemWithTopping.tranDetailItem = DetailItem;
                    detailItemWithTopping.tranDetailItemToppings = lstitemDetail;
                    lsttranDetailItemWithToppings.Add(detailItemWithTopping);

                    lstitemDetail = new List<TranDetailItemTopping>();
                }

                tranWithDetails.tranDetailItemWithToppings.AddRange(lsttranDetailItemWithToppings);

                //Tran Payment
                TranPaymentManage tranPaymentManage = new TranPaymentManage();
                var tranPayment = await tranPaymentManage.GetTranPayment(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
                foreach (var item in tranPayment)
                {
                    tranWithDetails.tranPayments.Add(item);
                }

                //Tran Discount
                TranTradDiscountManage discountManage = new TranTradDiscountManage();
                var tranDiscount = await discountManage.GetTranTradDiscount(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
                foreach (var itemDiscount in tranDiscount)
                {
                    tranWithDetails.tranTradDiscounts.Add(itemDiscount);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("getTranOrderDetail at POS");
            }
        }

        private async void CheckOrderBeforeClose()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }
                //Check order ว่าได้เลือกก่อนปิดแอปหรือไม่
                TransManage transManage = new TransManage();
                var checkOder = await transManage.GetTranOrderBeforeClose(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                if (checkOder == null)
                {
                    if (tranWithDetails == null || tranWithDetails?.tran == null || tranWithDetails?.tran.TranType == 'O')
                    {
                        DataCashing.SysCustomerID = 999;
                        tranWithDetails = null;
                        tranWithDetails = await Utils.initialData();
                    }
                }
                else
                {
                    if (!DataCashing.isCurrentOrder)
                    {
                        getTranOrderDetail(checkOder);
                    }
                }
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckOrderBeforeClose at POS");
            }
        }

        private async Task GetOnlineDataitem()
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
                List<SystemRevisionNo> listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = listRivision.FirstOrDefault(x => x.SystemID == 30);
                if (revisionNo == null)
                {
                    return;
                }

                int maxItemRevision = 0;

                #region Item                   
                List<Item> GetAllitem = new List<Item>();
                List<Gabana3.JAM.Items.ItemWithItemExSizes> UpdateItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                List<Gabana3.JAM.Items.ItemWithItemExSizes> InsertItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                try
                {
                    var allItem = await GabanaAPI.GetDataItem((int)revisionNo.LastRevisionNo, 0);

                    if (allItem == null || (allItem?.ItemsWithItemExSizes.Count == 0))
                    {
                        return;
                    }

                    int round = 0, addrount = 0;
                    round = allItem.totalItems / 100;
                    addrount = round + 1;
                    double increaseProgress = 0;
                    increaseProgress = 25 / addrount;

                    for (int j = 0; j < addrount; j++)
                    {
                        allItem = await GabanaAPI.GetDataItem((int)revisionNo.LastRevisionNo, j);

                        if (allItem == null || (allItem.totalItems == 0))
                        {
                            break;
                        }

                        allItem.ItemsWithItemExSizes.ToList().OrderBy(x => x.ItemStatus.item.RevisionNo);
                        var maxItem = allItem.ItemsWithItemExSizes.ToList().Max(x => x.ItemStatus.item.RevisionNo);// OrderByDescending(x => x.item.RevisionNo).First();                            

                        GetAllitem = await itemManage.GetAllItemType();
                        UpdateItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                        InsertItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();

                        UpdateItem.AddRange(allItem.ItemsWithItemExSizes.Where(x => GetAllitem.Select(y => (long)y.SysItemID).ToList().Contains(x.ItemStatus.item.SysItemID)).ToList());
                        InsertItem.AddRange(allItem.ItemsWithItemExSizes.Where(x => !(GetAllitem.Select(y => (long)y.SysItemID).ToList().Contains(x.ItemStatus.item.SysItemID)) && x.ItemStatus.DataStatus != 'D').ToList());

                        List<Item> Bulkitem = new List<Item>();
                        List<ItemExSize> BulkitemExsize = new List<ItemExSize>();

                        //Insert Item
                        if (InsertItem.Count > 0)
                        {
                            foreach (var item in InsertItem)
                            {
                                string thumnailPath = string.Empty;
                                try
                                {
                                    if (!string.IsNullOrEmpty(item.ItemStatus.item.PicturePath))
                                    {
                                        string pathImage = await Utils.InsertLocalPictureItemMaster(item.ItemStatus.item.PicturePath);
                                        thumnailPath = pathImage ?? "";
                                    }
                                    else
                                    {
                                        thumnailPath = "";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Update RevisionNo ที่ผิดพลาด เพื่อเรียกข้อมูลใหม่
                                    var errorRevison = InsertItem.Select(x => x.ItemStatus.item.RevisionNo).Min();
                                    maxItemRevision = (errorRevison == 0) ? 0 : errorRevison + 1;
                                    Log.Error("connecterror", "Bulkitem - Image : " + ex.Message);
                                    thumnailPath = "";
                                }

                                Bulkitem.Add(new Item()
                                {
                                    MerchantID = item.ItemStatus.item.MerchantID,
                                    SysItemID = item.ItemStatus.item.SysItemID,
                                    ItemName = item.ItemStatus.item.ItemName,
                                    Ordinary = item.ItemStatus.item.Ordinary,
                                    SysCategoryID = item.ItemStatus.item.SysCategoryID,
                                    ItemCode = item.ItemStatus.item.ItemCode,
                                    ShortName = item.ItemStatus.item.ShortName,
                                    PicturePath = item.ItemStatus.item.PicturePath,
                                    ThumbnailPath = item.ItemStatus.item.ThumbnailPath,
                                    PictureLocalPath = "",
                                    ThumbnailLocalPath = thumnailPath,
                                    Colors = item.ItemStatus.item.Colors,
                                    FavoriteNo = item.ItemStatus.item.FavoriteNo,
                                    UnitName = item.ItemStatus.item.UnitName,
                                    RegularSizeName = item.ItemStatus.item.RegularSizeName,
                                    EstimateCost = item.ItemStatus.item.EstimateCost,
                                    Price = item.ItemStatus.item.Price,
                                    OptSalePrice = item.ItemStatus.item.OptSalePrice,
                                    TaxType = item.ItemStatus.item.TaxType,
                                    SellBy = item.ItemStatus.item.SellBy,
                                    FTrackStock = item.ItemStatus.item.FTrackStock,
                                    TrackStockDateTime = item.ItemStatus.item.TrackStockDateTime,
                                    SaleItemType = item.ItemStatus.item.SaleItemType,
                                    Comments = item.ItemStatus.item.Comments,
                                    LastDateModified = item.ItemStatus.item.LastDateModified,
                                    UserLastModified = item.ItemStatus.item.UserLastModified,
                                    DataStatus = item.ItemStatus.DataStatus,
                                    FWaitSending = 0,
                                    WaitSendingTime = DateTime.UtcNow,
                                    LinkProMaxxItemID = item.ItemStatus.item.LinkProMaxxItemID,
                                    LinkProMaxxItemUnit = item.ItemStatus.item.LinkProMaxxItemUnit,
                                    FDisplayOption = item.ItemStatus.item.FDisplayOption
                                });

                                var itemSizes = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemExSizes).FirstOrDefault() ?? new List<ORM.Master.ItemExSize>();

                                itemSizes.ForEach(itemSize => BulkitemExsize.Add(new ItemExSize()
                                {
                                    MerchantID = itemSize.MerchantID,
                                    SysItemID = itemSize.SysItemID,
                                    EstimateCost = itemSize.EstimateCost,
                                    ExSizeName = itemSize.ExSizeName,
                                    ExSizeNo = itemSize.ExSizeNo,
                                    Price = itemSize.Price,
                                    Comments = itemSize.Comments
                                }));
                            }

                            using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                            {
                                await db.BeginTransactionAsync();
                                try
                                {
                                    await db.BulkCopyAsync(Bulkitem);
                                    await db.BulkCopyAsync(BulkitemExsize);
                                    await db.CommitTransactionAsync();
                                }
                                catch (Exception ex)
                                {
                                    await db.RollbackTransactionAsync();
                                    //Update RevisionNo ที่ผิดพลาด เพื่อเรียกข้อมูลใหม่
                                    var errorRevison = InsertItem.Select(x => x.ItemStatus.item.RevisionNo).Min();
                                    maxItemRevision = errorRevison;
                                    Utils.DeletePictureItemMaster(Bulkitem.Select(x => x.ThumbnailLocalPath).ToList());
                                    Log.Error("connecterror", "Bulkitem,BulkitemExsize : " + ex.Message);
                                    throw ex;
                                }
                            }
                        }

                        //Update Item
                        if (UpdateItem.Count > 0)
                        {
                            UpdateItem.ForEach(async item =>
                            {
                                char itemStatus = item.ItemStatus.DataStatus;
                                List<ORM.Master.ItemOnBranch> itemOnBranch = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemOnBranchs).FirstOrDefault() ?? new List<ORM.Master.ItemOnBranch>();
                                var data = await itemManage.GetItem((int)item.ItemStatus.item.MerchantID, (int)item.ItemStatus.item.SysItemID);

                                if (itemStatus == 'D')
                                {
                                    await Utils.DeleteItem(data, itemOnBranch);
                                }
                                else
                                {
                                    string thumnailLocalPath = string.Empty;
                                    if (!string.IsNullOrEmpty(item.ItemStatus.item.PicturePath))
                                    {
                                        if (item.ItemStatus.item.PicturePath != data.PicturePath)
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

                                            string pathImage = await Utils.InsertLocalPictureItemMaster(item.ItemStatus.item.PicturePath);
                                            thumnailLocalPath = pathImage ?? "";
                                        }
                                        else
                                        {
                                            thumnailLocalPath = data?.ThumbnailLocalPath;
                                        }
                                    }

                                    Item updateItem = new Item()
                                    {
                                        MerchantID = item.ItemStatus.item.MerchantID,
                                        SysItemID = item.ItemStatus.item.SysItemID,
                                        ItemName = item.ItemStatus.item.ItemName,
                                        Ordinary = item.ItemStatus.item.Ordinary,
                                        SysCategoryID = item.ItemStatus.item.SysCategoryID,
                                        ItemCode = item.ItemStatus.item.ItemCode,
                                        ShortName = item.ItemStatus.item.ShortName,
                                        PicturePath = item.ItemStatus.item.PicturePath,
                                        ThumbnailPath = item.ItemStatus.item.ThumbnailPath,
                                        PictureLocalPath = "",
                                        ThumbnailLocalPath = thumnailLocalPath,
                                        Colors = item.ItemStatus.item.Colors,
                                        FavoriteNo = item.ItemStatus.item.FavoriteNo,
                                        UnitName = item.ItemStatus.item.UnitName,
                                        RegularSizeName = item.ItemStatus.item.RegularSizeName,
                                        EstimateCost = item.ItemStatus.item.EstimateCost,
                                        Price = item.ItemStatus.item.Price,
                                        OptSalePrice = item.ItemStatus.item.OptSalePrice,
                                        TaxType = item.ItemStatus.item.TaxType,
                                        SellBy = item.ItemStatus.item.SellBy,
                                        FTrackStock = item.ItemStatus.item.FTrackStock,
                                        TrackStockDateTime = item.ItemStatus.item.TrackStockDateTime,
                                        SaleItemType = item.ItemStatus.item.SaleItemType,
                                        Comments = item.ItemStatus.item.Comments,
                                        LastDateModified = item.ItemStatus.item.LastDateModified,
                                        UserLastModified = item.ItemStatus.item.UserLastModified,
                                        DataStatus = item.ItemStatus.DataStatus,
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow,
                                        LinkProMaxxItemID = item.ItemStatus.item.LinkProMaxxItemID,
                                        LinkProMaxxItemUnit = item.ItemStatus.item.LinkProMaxxItemUnit,
                                        FDisplayOption = item.ItemStatus.item.FDisplayOption
                                    };

                                    var insertOrreplace = await itemManage.UpdateItem(updateItem);

                                    var itemSizes = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemExSizes).FirstOrDefault() ?? new List<ORM.Master.ItemExSize>();

                                    ItemExSizeManage itemExSizeManage = new ItemExSizeManage();
                                    itemSizes.ForEach(async itemSize =>
                                    {
                                        await itemExSizeManage.InsertOrReplaceItemSize(new ItemExSize()
                                        {
                                            MerchantID = itemSize.MerchantID,
                                            SysItemID = itemSize.SysItemID,
                                            EstimateCost = itemSize.EstimateCost,
                                            ExSizeName = itemSize.ExSizeName,
                                            ExSizeNo = itemSize.ExSizeNo,
                                            Price = itemSize.Price,
                                            Comments = itemSize.Comments
                                        });
                                    });
                                }
                                maxItemRevision = item.ItemStatus.item.RevisionNo;
                            });
                        }

                        await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxItem);
                    }
                    Log.Debug("connectpass", "listRivisionItem");
                    DataCashingAll.flagItemChange = true;
                }
                catch (Exception ex)
                {
                    Log.Debug("connecterror", "listRivisionItem : " + ex.Message);
                    await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxItemRevision);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineDataitem at ite m");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task GetOnlineDataCategory()
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
                List<SystemRevisionNo> listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = listRivision.FirstOrDefault(x => x.SystemID == 20);
                if (revisionNo == null)
                {
                    return;
                }

                int maxCategoryRevision = 0;
                int maxCategory = 0;

                #region Category
                try
                {
                    //Get Category API
                    var allcategory = await GabanaAPI.GetDataCategory((int)revisionNo.LastRevisionNo);

                    if (allcategory == null || (allcategory.Categories.Count == 0 && allcategory.CategoryBins.Count == 0))
                    {
                        return;
                    }

                    CategoryManage CategoryManage = new CategoryManage();
                    allcategory.Categories.ToList().OrderBy(x => x.RevisionNo);
                    maxCategory = allcategory.Categories.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();

                    //check ว่ามีไหม

                    List<Category> GetallCate = await CategoryManage.GetAllCategory();
                    List<ORM.Master.Category> UpdateCategory = new List<ORM.Master.Category>();
                    List<ORM.Master.Category> InsertCategory = new List<ORM.Master.Category>();
                    UpdateCategory.AddRange(allcategory.Categories.Where(x => GetallCate.Select(y => (long)y.SysCategoryID).ToList().Contains(x.SysCategoryID)).ToList());
                    InsertCategory.AddRange(allcategory.Categories.Where(x => !(GetallCate.Select(y => (long)y.SysCategoryID).ToList().Contains(x.SysCategoryID))).ToList());

                    //Insert Category
                    if (InsertCategory.Count > 0)
                    {
                        List<Category> BulkCategory = new List<Category>();
                        foreach (var category in InsertCategory)
                        {
                            BulkCategory.Add(new Category()
                            {
                                MerchantID = category.MerchantID,
                                SysCategoryID = category.SysCategoryID,
                                Ordinary = category.Ordinary,
                                Name = category.Name,
                                DateCreated = category.DateCreated,
                                DateModified = category.DateModified,
                                DataStatus = 'I',
                                FWaitSending = 0,
                                WaitSendingTime = DateTime.UtcNow,
                                LinkProMaxxID = category.LinkProMaxxID
                            });
                            maxCategoryRevision = category.RevisionNo;
                        }

                        using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                        {
                            try
                            {
                                await db.BulkCopyAsync(BulkCategory);
                            }
                            catch (Exception ex)
                            {
                                var errorRevison = InsertCategory.Select(x => x.RevisionNo).Min();
                                maxCategory = errorRevison;
                                Log.Error("connecterror", "BulkCategory :" + ex.Message);
                                throw ex;
                            }
                        }
                    }

                    //Update Category
                    if (UpdateCategory.Count > 0)
                    {
                        foreach (var item in UpdateCategory)
                        {
                            //insertorreplace
                            category = new Category()
                            {
                                MerchantID = item.MerchantID,
                                SysCategoryID = item.SysCategoryID,
                                Ordinary = item.Ordinary,
                                Name = item.Name,
                                DateCreated = item.DateCreated,
                                DateModified = item.DateModified,
                                DataStatus = 'I',
                                FWaitSending = 0,
                                WaitSendingTime = DateTime.UtcNow,
                                LinkProMaxxID = item.LinkProMaxxID
                            };
                            var insertOrreplace = await CategoryManage.InsertOrReplaceCategory(category);
                            maxCategoryRevision = item.RevisionNo;
                        }
                    }

                    allcategory.CategoryBins.ToList().OrderBy(x => x.RevisionNo);
                    maxCategory = allcategory.CategoryBins.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();
                    foreach (var item in allcategory.CategoryBins)
                    {
                        //UpdateItem
                        List<Item> UpdateItemCate = new List<Item>();
                        UpdateItemCate = await itemManage.GetItembyCategory(item.MerchantID, item.SysCategoryID);
                        if (UpdateItemCate != null)
                        {
                            foreach (var update in UpdateItemCate)
                            {
                                update.SysCategoryID = null;
                                var resultUpdate = await itemManage.UpdateItem(update);
                            }
                        }
                        //delete
                        var delete = await CategoryManage.DeleteCategory(item.MerchantID, item.SysCategoryID);
                        if (!delete)
                        {
                            var data = await CategoryManage.GetCategory(item.MerchantID, item.SysCategoryID);
                            if (data != null)
                            {
                                data.DataStatus = 'D';
                                data.FWaitSending = 0;
                                await CategoryManage.UpdateCategory(category);
                            }
                        }
                        maxCategoryRevision = item.RevisionNo;
                    }

                    await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxCategory);
                    Log.Debug("connectpass", "listRivisionCategory");
                    DataCashingAll.flagCategoryChange = true;
                }
                catch (Exception ex)
                {
                    Log.Debug("connecterror", "listRivisionCategory : " + ex.Message);
                    await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxCategoryRevision);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineDataCategory at item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public void ShowDialogRemove()
        {
            //Show Dialog
            MainDialog dialog = new MainDialog();
            Bundle bundle = new Bundle();
            String myMessage = Resource.Layout.offline_dialog_main.ToString();
            bundle.PutString("message", myMessage);
            bundle.PutString("confirmRemove", "confirmRemove");
            dialog.Arguments = bundle;
            dialog.Show(SupportFragmentManager, myMessage);
        }

        private void ShowPayment()
        {
            try
            {
                if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
                {
                    HandleNoItems();
                }
                else
                {
                    HandleItemsPresent();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowPayment at POS");
            }
        }

        private void HandleNoItems()
        {
            try
            {
                textSum.Text = $"0.00 {CURRENCYSYMBOLS}";
                string noItemText = DataCashing.Language == "th" ? "ไม่มีสินค้า" : "No item";
                txtNoItem.Text = noItemText;
                SetQuantityToCartUI();
                SetPaymentUI(Resource.Drawable.btnborderblue, Resource.Color.primaryDark);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void HandleItemsPresent()
        {
            try
            {
                var quantity = tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity);
                textSum.Text = $"{Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal)} {CURRENCYSYMBOLS}";
                string itemsText = DataCashing.Language == "th"
                    ? $"{quantity.ToString("#,###")} รายการ"
                    : quantity == 1 ? $"{quantity.ToString("#,###")} item" : $"{quantity.ToString("#,###")} items";
                txtNoItem.Text = itemsText;
                SetQuantityToCartUI();
                SetPaymentUI(Resource.Drawable.btnblue, Resource.Color.textIcon);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetPaymentUI(int backgroundResource, int textColor)
        {
            try
            {
                lnPayment.SetBackgroundResource(backgroundResource);
                lnPayment.SetPadding(0, 5, 0, 5);
                textSum.SetTextColor(Application.Context.Resources.GetColor(textColor, null));
                txtNoItem.SetTextColor(Application.Context.Resources.GetColor(textColor, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetQuantityToCartUI()
        {
            try
            {
                txtQuantity.Text = $"{DataCashing.setQuantityToCart}x";
                if (DataCashing.setQuantityToCart == 1)
                {
                    btnBasket.SetBackgroundResource(Resource.Drawable.borderqauntity);
                    txtQuantity.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;

        async Task CheckJwt()
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

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();

                imgSelect?.Dispose();
                imgScanbarcode?.Dispose();
                //lnPayment?.Dispose();
                textSum?.Dispose();
                imagebtnBack?.Dispose();
                //lnBack?.Dispose();
                //btnCustomer1?.Dispose();
                //btnCustomer2?.Dispose();
                btnDummy?.Dispose();
                //lnOrder?.Dispose();
                btnBasket?.Dispose();
                txtQuantity?.Dispose();
                txtNoItem?.Dispose();
                //lnNoCustomer?.Dispose();
                //lnHaveCustomer?.Dispose();
                txtNameCustomer?.Dispose();
                txtSearchPos?.Dispose();
                //lnGroupButton?.Dispose();
                buttonSearch?.Dispose();
                RecyclerViewHeader?.Dispose();
                RecyclerViewShow?.Dispose();
                txtOrderNum?.Dispose();
                //ImageShowPOS?.Dispose();
                //view?.Dispose();
                //cardView?.Dispose();

                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnDestroy at POS");
            }
        }
    }
}