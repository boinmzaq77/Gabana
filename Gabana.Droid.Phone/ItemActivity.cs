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
using Gabana.Droid.ListData;
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
    //[Activity(Label = "ItemActivity")]
    public class ItemActivity : AppCompatActivity
    {

        public static ItemActivity itemActivity;
        public static Context context;
        ImageButton imageButtonBack;
        LinearLayout tabItem, tabCategory, tabStock, tabExtra, lnBack;
        public static RecyclerView recyclerviewlistitem, recyclerviewlistCategory, recyclerviewlistStock, recyclerviewlistTopping;
        public static ListItem listItem, listExtraItem, listItemStock;
        static ListCategory listCategory;
        ImageButton additem, addcategory, addExtra;
        public static List<Item> items, itemExtra, itemsStock;
        public static List<Item> lstsearchitems, lstsearchitemExtra, lstsearchitemsStock;
        public static List<Category> lstCategory, lstsearchcategory;
        SwipeRefreshLayout refreshlayoutTabitem, refreshlayoutTabCategory, refreshlayoutTabStock, refreshlayoutTopping;
        GridLayoutManager mLayoutManager, menuLayoutManager;
        Item_Adapter_Category Item_Adapter_Category;
        Item_Adapter_Stock Item_Adapter_Stock;
        RecyclerView recyclerHeaderItem;
        EditText textSearchItem, textSearchCategory, textSearchStock, textSearchTopping;
        ImageButton btnSearchItemBarcode;
        ImageButton btnSearchItem, btnSearchTopping, btnSearchStock, btnSearchCategory;
        LinearLayout lnNoItem, lnNoDataSearchItem, lnNoItemExtra, lnNoDataSearchItemExtra, lnNoStock, lnNoDataSearchStock, lnNoCategory, lnNoDataSearchCategory;
        FrameLayout lnSearchItem, lnSearchTopping, lnSearchStock, lnSearchCategory;
        List<MenuTab> MenuTab { get; set; }
        public static Item FocusNewItem;
        public static Category FocusCate;

        public static string tabSelected = "";
        string searchItem, searchCategory, searchStock, searchTopping /*,searchNote*/;
        string searchItemCode;
        string usernamelogin, LoginType;
        Item_Adapter_ItemExtra item_Adapter_Extra;
        Item_Adapter_Item item_Adapter_Item;
        public static GridLayoutManager gridLayoutItem;
        public static GridLayoutManager gridLayoutItemExtra;

        DialogLoading dialogLoading = new DialogLoading();
        private List<ItemOnBranch> itemOnBranch;
        public static List<Item> allitem;

        List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        CategoryManage categoryManage = new CategoryManage();
        Category category = new Category();
        ItemManage itemManage = new ItemManage();
        ItemExSizeManage itemExSizeManage = new ItemExSizeManage();
        Item getItem = new Item();
        ItemExSize getitemSize = new ItemExSize();
        ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
        int maxCategoryRevision = 0;
        int maxItemRevision = 0;
        public static bool checkNet = false;
        public DateTime pauseDate = DateTime.Now;
#pragma warning disable CS0414 // The field 'ItemActivity.ItemScanCode' is assigned but its value is never used
        bool deviceAsleep = false, ItemScanCode = false;
#pragma warning restore CS0414 // The field 'ItemActivity.ItemScanCode' is assigned but its value is never used
        TextView txtStockRevision;
        long LastRevisionNoStock = 0;
        ImageButton btnScollToTop;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.item_activity_main);
                itemActivity = this;
                context = Android.App.Application.Context;

                recyclerHeaderItem = FindViewById<RecyclerView>(Resource.Id.recyclerHeaderItem);
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnNoItem = FindViewById<LinearLayout>(Resource.Id.lnNoItem);
                lnNoDataSearchItem = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearchItem);
                lnNoDataSearchItemExtra = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearchItemExtra);
                lnNoDataSearchStock = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearchStock);
                lnNoDataSearchCategory = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearchCategory);
                lnSearchItem = FindViewById<FrameLayout>(Resource.Id.lnSearchItem);
                lnNoItemExtra = FindViewById<LinearLayout>(Resource.Id.lnNoItemExtra);
                lnSearchStock = FindViewById<FrameLayout>(Resource.Id.lnSearchStock);
                lnSearchTopping = FindViewById<FrameLayout>(Resource.Id.lnSearchTopping);
                lnNoCategory = FindViewById<LinearLayout>(Resource.Id.lnNoCategory);
                lnNoStock = FindViewById<LinearLayout>(Resource.Id.lnNoStock);
                lnSearchCategory = FindViewById<FrameLayout>(Resource.Id.lnSearchCategory);
                txtStockRevision = FindViewById<TextView>(Resource.Id.txtStockRevision);
                imageButtonBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                btnSearchItemBarcode = FindViewById<ImageButton>(Resource.Id.btnSearchItemBarcode);
                btnSearchItem = FindViewById<ImageButton>(Resource.Id.btnSearchItem);
                btnSearchItem.Click += BtnSearchItem_Click;
                btnSearchTopping = FindViewById<ImageButton>(Resource.Id.btnSearchTopping);
                btnSearchTopping.Click += BtnSearchTopping_Click;
                btnSearchCategory = FindViewById<ImageButton>(Resource.Id.btnSearchCategory);
                btnSearchCategory.Click += BtnSearchCategory_Click;
                btnSearchStock = FindViewById<ImageButton>(Resource.Id.btnSearchStock);
                btnSearchStock.Click += BtnSearchStock_Click;

                tabItem = FindViewById<LinearLayout>(Resource.Id.tabItem);
                tabCategory = FindViewById<LinearLayout>(Resource.Id.tabCategory);
                tabExtra = FindViewById<LinearLayout>(Resource.Id.tabTopping);
                tabStock = FindViewById<LinearLayout>(Resource.Id.tabStock);

                tabItem.Visibility = ViewStates.Visible;
                tabCategory.Visibility = ViewStates.Gone;
                tabStock.Visibility = ViewStates.Gone;
                tabExtra.Visibility = ViewStates.Gone;

                recyclerviewlistitem = FindViewById<RecyclerView>(Resource.Id.recyclerview_listitem);
                recyclerviewlistCategory = FindViewById<RecyclerView>(Resource.Id.recyclerview_listCategory);
                recyclerviewlistStock = FindViewById<RecyclerView>(Resource.Id.recyclerview_listStock);
                recyclerviewlistTopping = FindViewById<RecyclerView>(Resource.Id.recyclerview_listTopping);

                additem = FindViewById<ImageButton>(Resource.Id.additem);
                addcategory = FindViewById<ImageButton>(Resource.Id.addcategory);
                addExtra = FindViewById<ImageButton>(Resource.Id.addTopping);

                btnScollToTop = FindViewById<ImageButton>(Resource.Id.btnScollToTop);

                additem.Click += Additem_Click;
                addcategory.Click += Addcategory_Click;
                addExtra.Click += AddExtra_Click;
                imageButtonBack.Click += ImageButtonBack_Click;
                lnBack.Click += ImageButtonBack_Click;
                btnSearchItemBarcode.Click += BtnSearchItemBarcode_Click;
                tabSelected = "Item";

                //txtStockRevision.Text = "Stock Revision No. " + LastRevisionNoStock;
                usernamelogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");

                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "item");
                if (check)
                {
                    additem.SetBackgroundResource(Resource.Mipmap.Add);
                    addcategory.SetBackgroundResource(Resource.Mipmap.Add);
                    addExtra.SetBackgroundResource(Resource.Mipmap.Add);
                }
                else
                {
                    additem.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addcategory.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addExtra.SetBackgroundResource(Resource.Mipmap.AddMax);
                }

                textSearchItem = FindViewById<EditText>(Resource.Id.textSearchItem);
                textSearchCategory = FindViewById<EditText>(Resource.Id.textSearchCategory);
                textSearchStock = FindViewById<EditText>(Resource.Id.textSearchStock);
                textSearchTopping = FindViewById<EditText>(Resource.Id.textSearchTopping);

                textSearchItem.Text = searchItem;
                textSearchItem.TextChanged += TextSearchItem_TextChanged;
                textSearchItem.KeyPress += TextSearchItem_KeyPress;
                textSearchItem.FocusChange += TextSearchItem_FocusChange;

                textSearchCategory.TextChanged += TextSearchCategory_TextChanged;
                textSearchCategory.KeyPress += TextSearchCategory_KeyPress;
                textSearchCategory.FocusChange += TextSearchCategory_FocusChange;

                textSearchTopping.TextChanged += TextSearchTopping_TextChanged;
                textSearchTopping.KeyPress += TextSearchTopping_KeyPress;
                textSearchTopping.FocusChange += TextSearchTopping_FocusChange;

                textSearchStock.TextChanged += TextSearchStock_TextChanged;
                textSearchStock.KeyPress += TextSearchStock_KeyPress;
                textSearchStock.FocusChange += TextSearchStock_FocusChange;

                refreshlayoutTabitem = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayoutTabitem);
                refreshlayoutTabCategory = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayoutTabCategory);
                refreshlayoutTopping = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayoutTopping);
                refreshlayoutTabStock = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayoutTabStock);

                CheckJwt();

                refreshlayoutTabitem.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagItemChange = true;
                    DataCashingAll.flagItemOnBranchChange = true;
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
                        await GetOnlineDataCategory();
                        await GetOnlineDataitem();
                        await GetOnlineDataItemonBranch();
                        OnResume();                       
                    }
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                refreshlayoutTabCategory.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagItemChange = true;
                    DataCashingAll.flagItemOnBranchChange = true;
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
                        await GetOnlineDataCategory();
                        await GetOnlineDataitem();
                        await GetOnlineDataItemonBranch();
                        OnResume();
                    }
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                refreshlayoutTopping.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagItemChange = true;
                    DataCashingAll.flagItemOnBranchChange = true;
                    DataCashingAll.flagCategoryChange = true;
                    
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
                        await GetOnlineDataCategory();
                        await GetOnlineDataitem();
                        await GetOnlineDataItemonBranch();
                        OnResume();
                    }

                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                refreshlayoutTabStock.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagItemChange = true;
                    DataCashingAll.flagItemOnBranchChange = true;
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
                        await GetOnlineDataCategory();
                        await GetOnlineDataitem();
                        await GetOnlineDataItemonBranch();
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
                UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "item");
                MySwipeHelper ItemSwipe = new ItemSwipeHelper(this, recyclerviewlistitem, (int)Width);
                MySwipeHelper ItemExtraSwipe = new ItemExtraSwipeHelper(this, recyclerviewlistTopping, (int)Width);
                MySwipeHelper ItemCategorySwipe = new ItemCategorySwipeHelper(this, recyclerviewlistCategory, (int)Width);

                DataCashingAll.flagItemChange = true;
                DataCashingAll.flagCategoryChange = true;
                DataCashingAll.flagItemOnBranchChange = true;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("Oncreate at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnScollToTop_Click(object sender, EventArgs e)
        {
            recyclerviewlistStock.ScrollToPosition(0);
        }

        private void TextSearchStock_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearchStock.Text.Trim()))
            {
                btnSearchStock.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearchStock.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }

        private void TextSearchStock_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterItemOnStock();
                textSearchStock.ClearFocus();
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
                textSearchStock.Text += input;
                textSearchStock.SetSelection(textSearchStock.Text.Length);
                return;
            }
        }

        private async void SetFilterItemOnStock()
        {
            try
            {
                lstsearchitemsStock = new List<Item>();
                if (string.IsNullOrEmpty(searchStock))
                {
                    return;
                }
                lstsearchitemsStock = itemsStock.Where(m => (m.ItemName.ToLower().Contains(searchStock.ToLower()) || (m.ItemCode != null && m.ItemCode.Contains(searchStock.ToLower())))).ToList();
                if (lstsearchitemsStock.Count > 0)
                {
                    lstsearchitemsStock = lstsearchitemsStock.OrderBy(x => x.ItemName).ToList();
                }

                listItemStock = new ListItem(lstsearchitemsStock);
                Item_Adapter_Stock = new Item_Adapter_Stock(listItemStock, itemOnBranch, checkNet);
                gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                recyclerviewlistStock.SetLayoutManager(gridLayoutItem);
                //recyclerviewlistStock.HasFixedSize = true;
                recyclerviewlistStock.HasFixedSize = false;
                int count = items == null ? 0 : items.Count + 1;
                recyclerviewlistStock.SetItemViewCacheSize(count);
                recyclerviewlistStock.SetAdapter(Item_Adapter_Stock);
                Item_Adapter_Stock.ItemClick += Item_Adapter_Stock_ItemClick;
                SetBtnSearch(btnSearchStock, searchStock);

                recyclerviewlistStock.ScrollChange += RecyclerviewlistStock_ScrollChange;
                btnScollToTop.Click += BtnScollToTop_Click;

                if (item_Adapter_Extra.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchTopping))
                    {
                        lnNoDataSearchStock.Visibility = ViewStates.Visible;
                        lnNoStock.Visibility = ViewStates.Gone;
                        lnSearchStock.Visibility = ViewStates.Visible;
                        recyclerviewlistStock.Visibility = ViewStates.Gone;

                    }
                    else
                    {
                        lnNoDataSearchStock.Visibility = ViewStates.Gone;
                        lnNoStock.Visibility = ViewStates.Visible;
                        lnSearchStock.Visibility = ViewStates.Gone;
                        recyclerviewlistStock.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchStock.Visibility = ViewStates.Gone;
                    lnNoStock.Visibility = ViewStates.Gone;
                    lnSearchStock.Visibility = ViewStates.Visible;
                    recyclerviewlistStock.Visibility = ViewStates.Visible;
                }



            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetFilterItemData at Stock");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void RecyclerviewlistStock_ScrollChange(object sender, View.ScrollChangeEventArgs e)
        {
            if (e.OldScrollY >= 0)
            {
                btnScollToTop.Visibility = ViewStates.Gone;
            }
            else
            {
                btnScollToTop.Visibility = ViewStates.Visible;
            }
        }

        private void TextSearchStock_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchStock = textSearchStock.Text.Trim();
            if (string.IsNullOrEmpty(searchStock))
            {
                SetItemOnStock();
            }
            SetBtnSearch(btnSearchStock, searchStock);
        }

        async void SetItemOnStock()
        {
            try
            {
                listItemStock = new ListItem(itemsStock);
                ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
                itemOnBranch = await onBranchManage.GetAllItemOnBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                Item_Adapter_Stock = new Item_Adapter_Stock(listItemStock, itemOnBranch, checkNet);
                gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                recyclerviewlistStock.SetLayoutManager(gridLayoutItem);
                //recyclerviewlistStock.HasFixedSize = true;
                recyclerviewlistStock.HasFixedSize = false;
                int count = items == null ? 0 : items.Count + 1;
                recyclerviewlistStock.SetItemViewCacheSize(count);
                recyclerviewlistStock.SetAdapter(Item_Adapter_Stock);
                Item_Adapter_Stock.ItemClick += Item_Adapter_Stock_ItemClick;


                recyclerviewlistStock.ScrollChange += RecyclerviewlistStock_ScrollChange;


                if (Item_Adapter_Stock.ItemCount == 0)
                {
                    lnNoStock.Visibility = ViewStates.Visible;
                    lnSearchStock.Visibility = ViewStates.Gone;
                    recyclerviewlistStock.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoStock.Visibility = ViewStates.Gone;
                    lnSearchStock.Visibility = ViewStates.Visible;
                    recyclerviewlistStock.Visibility = ViewStates.Visible;
                    btnScollToTop.Click += BtnScollToTop_Click;

                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                dialogLoading = new DialogLoading();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetItemData at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async Task GetItemOnStock()
        {
            try
            {
                allitem = new List<Item>();
                itemsStock = new List<Item>();

                if (items != null)
                {
                    allitem.AddRange(items);
                }
                if (itemExtra != null)
                {
                    allitem.AddRange(itemExtra);
                }
                //ในเบื้องต้นซอร์ทตามตัวอักษร
                itemsStock = allitem.Where(x => x.FTrackStock == 1).OrderBy(x=>x.ItemName).ToList();

                //await SetItemOnStock();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetItemData at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Item_Adapter_Stock_ItemClick(object sender, int e)
        {
            //18/01/66 ตอนนี้ติด dialog มี 2 อัน
            var SaleItemType = listItemStock[e].SaleItemType;
            if (SaleItemType == 'T')
            {
                var Topping = listItemStock[e];
                StartActivity(new Intent(context, typeof(AddExtraToppingActivity)));
                AddExtraToppingActivity.SetExtraTopping(Topping);
                AddExtraToppingActivity.tabSelected = "Stock";
            }
            else
            {
                StartActivity(new Intent(context, typeof(AddItemActivity)));
                DataCashing.EditItemID = listItemStock[e].SysItemID;
                AddItemActivity.tabSelected = "Stock";
            }
        }

        private void TextSearchTopping_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearchTopping.Text.Trim()))
            {
                btnSearchTopping.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearchTopping.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }

        private void TextSearchCategory_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearchCategory.Text.Trim()))
            {
                btnSearchCategory.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearchCategory.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }

        private void TextSearchItem_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearchItem.Text.Trim()))
            {
                btnSearchItem.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearchItem.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }

        private void BtnSearchStock_Click(object sender, EventArgs e)
        {
            SetClearSearchStock();
            SetItemOnStock();
        }

        private void BtnSearchCategory_Click(object sender, EventArgs e)
        {
            SetClearSearchCategory();
            SetCategoryData();
        }

        private void BtnSearchTopping_Click(object sender, EventArgs e)
        {
            SetClearSearchTopping();
            SetExtra();
        }

        private void BtnSearchItem_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            SetItemData();
        }

        private void SetClearSearchText()
        {
            searchItemCode = "";
            searchItem = "";
            textSearchItem.Text = string.Empty;
        }

        private void SetClearSearchTopping()
        {
            searchTopping = "";
            textSearchTopping.Text = string.Empty;
            SetBtnSearch(btnSearchTopping, searchTopping);
        }

        private void SetClearSearchCategory()
        {
            searchCategory = "";
            textSearchCategory.Text = string.Empty;
            SetBtnSearch(btnSearchCategory, searchCategory);
        }

        private void SetClearSearchStock()
        {
            searchStock = "";
            textSearchStock.Text = string.Empty;
            SetBtnSearch(btnSearchStock, searchStock);
        }

        private void SetBtnSearch(ImageButton button, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                button.SetBackgroundResource(Resource.Mipmap.Search);
            }
            else
            {
                button.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
        }

        public static void SetItemCode(string itemCode)
        {
            itemActivity.ItemScanCode = true;
            itemActivity.searchItem = itemCode;
            itemActivity.textSearchItem.Text = itemCode;
            itemActivity.SetFilterItemData();
        }

        public async void ShowItemFav(long? itemId)
        {
            try
            {
                Item item = new Item();
                int index = 0;
                if (string.IsNullOrEmpty(searchItem))
                {
                    item = items?.Where(x => x.SysItemID == itemId).FirstOrDefault();
                    if (item == null)
                    {
                        return;
                    }
                    index = items.FindIndex(x => x.SysItemID == item.SysItemID);
                    if (index == -1)
                    {
                        return;
                    }
                }
                else
                {
                    item = lstsearchitems?.Where(x => x.SysItemID == itemId).FirstOrDefault();
                    if (item == null)
                    {
                        return;
                    }
                    index = lstsearchitems.FindIndex(x => x.SysItemID == item.SysItemID);
                    if (index == -1)
                    {
                        return;
                    }
                }

                var z = recyclerviewlistitem.FindViewHolderForAdapterPosition(index) as ListViewItemHolder;
                if (z != null)
                {
                    if (item.FavoriteNo == 0)
                    {
                        z.Select();
                        Item_Adapter_Item.vhselect = z;
                    }
                    else
                    {
                        z.NotSelect();
                        Item_Adapter_Item.vhselect = z;
                    }
                }
                await LnFavoriteClick(item);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("ShowFav at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public async Task LnFavoriteClick(Item dataItem)
        {
            try
            {
                Item item = new Item();
                item = dataItem;

                if (item == null)
                {
                    return;
                }
                if (item.FavoriteNo == 0)
                {
                    item.FavoriteNo = 1;
                }
                else
                {
                    item.FavoriteNo = 0;
                }
                item.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                item.UserLastModified = usernamelogin;
                item.DataStatus = 'M';
                item.FWaitSending = 2;
                item.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                item.TrackStockDateTime = Utils.GetTranDate(item.TrackStockDateTime);
                ItemManage ItemManage = new ItemManage();
                var result = await ItemManage.UpdateItem(item);
                if (result)
                {
                    //DataCashingAll.flagItemChange = true;
                    Toast.MakeText(this, GetString(Resource.String.editsucess), ToastLength.Short).Show();
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendItem((int)item.MerchantID, (int)item.SysItemID);
                    }
                    else
                    {
                        item.FWaitSending = 2;
                        await ItemManage.UpdateItem(item);
                    }
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("LnFavoriteClick at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public async void ShowToppingFav(long? itemId)
        {
            try
            {
                Item item = new Item();
                int index = 0;
                if (string.IsNullOrEmpty(searchTopping))
                {
                    item = itemExtra?.Where(x => x.SysItemID == itemId).FirstOrDefault();
                    if (item == null)
                    {
                        return;
                    }
                    index = itemExtra.FindIndex(x => x.SysItemID == item.SysItemID);
                    if (index == -1)
                    {
                        return;
                    }
                }
                else
                {
                    item = lstsearchitemExtra?.Where(x => x.SysItemID == itemId).FirstOrDefault();
                    if (item == null)
                    {
                        return;
                    }
                    index = lstsearchitemExtra.FindIndex(x => x.SysItemID == item.SysItemID);
                    if (index == -1)
                    {
                        return;
                    }
                }
                var z = recyclerviewlistTopping.FindViewHolderForAdapterPosition(index) as ListViewItemHolder;
                if (z != null)
                {
                    if (item.FavoriteNo == 0)
                    {
                        z.Select();
                        Item_Adapter_Item.vhselect = z;
                    }
                    else
                    {
                        z.NotSelect();
                        Item_Adapter_Item.vhselect = z;
                    }
                }
                await LnToppingFavoriteClick(item);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("ShowFav at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public async Task LnToppingFavoriteClick(Item dataItem)
        {
            try
            {
                Item item = new Item();
                item = dataItem;

                if (item == null)
                {
                    return;
                }
                if (item.FavoriteNo == 0)
                {
                    item.FavoriteNo = 1;
                }
                else
                {
                    item.FavoriteNo = 0;
                }
                item.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                item.UserLastModified = usernamelogin;
                item.DataStatus = 'M';
                item.FWaitSending = 2;
                item.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                item.TrackStockDateTime = Utils.GetTranDate(item.TrackStockDateTime);
                ItemManage ItemManage = new ItemManage();
                var result = await ItemManage.UpdateItem(item);
                if (result)
                {
                    //DataCashingAll.flagItemChange = true;
                    Toast.MakeText(this, GetString(Resource.String.editsucess), ToastLength.Short).Show();
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendItem((int)item.MerchantID, (int)item.SysItemID);
                    }
                    else
                    {
                        item.FWaitSending = 2;
                        await ItemManage.UpdateItem(item);
                    }
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("LnToppingFavoriteClick at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TextSearchTopping_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterExtraToppingData();
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
                textSearchTopping.Text += input;
                textSearchTopping.SetSelection(textSearchTopping.Text.Length);
                return;
            }
        }

        private void TextSearchTopping_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchTopping = textSearchTopping.Text.Trim();
            if (string.IsNullOrEmpty(searchTopping))
            {
                SetExtra();
            }
            SetBtnSearch(btnSearchTopping, searchTopping);
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayoutTabitem.Refreshing = false;
            refreshlayoutTabCategory.Refreshing = false;
            refreshlayoutTopping.Refreshing = false;
            refreshlayoutTabStock.Refreshing = false;
        }

        private void TextSearchCategory_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterCategoryData();
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
                textSearchCategory.Text += input;
                textSearchCategory.SetSelection(textSearchCategory.Text.Length);
                return;
            }
        }

        private void TextSearchCategory_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchCategory = textSearchCategory.Text.Trim();
            if (string.IsNullOrEmpty(searchCategory))
            {
                SetCategoryData();
            }
            SetBtnSearch(btnSearchCategory, searchCategory);
        }

        private void BtnSearchItemBarcode_Click(object sender, EventArgs e)
        {
            btnSearchItemBarcode.Enabled = false;
            StartActivity(new Intent(context, typeof(ItemScanActivity)));
            btnSearchItemBarcode.Enabled = true;
        }

        private void TextSearchItem_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterItemData();
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
                textSearchItem.Text += input;
                textSearchItem.SetSelection(textSearchItem.Text.Length);
                return;
            }
        }

        private void TextSearchItem_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchItem = textSearchItem.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchItem))
            {
                SetItemData();
            }
            SetBtnSearch(btnSearchItem, searchItem);
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            await GabanaAPI.CheckSpeedConnection();
        }

        private void SetTabMenu()
        {
            try
            {
                MenuTab = new List<MenuTab>
                {
                    new MenuTab() { NameMenuEn = "Item" , NameMenuTh = "สินค้า" },
                    new MenuTab() { NameMenuEn = "Extra Topping" , NameMenuTh = "ท็อปปิ้งเสริม" },
                    new MenuTab() { NameMenuEn = "Stock" , NameMenuTh = "สต็อก" },
                    new MenuTab() { NameMenuEn = "Category" , NameMenuTh = "หมวดหมู่สินค้า" },
                    //new MenuTab() { NameMenuEn = "Discount" , NameMenuTh = "" },
                    //new MenuTab() { NameMenuEn = "Note" , NameMenuTh = "" },
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

        private void Item_Adapter_Header_ItemClick(object sender, int e)
        {
            try
            {
                tabSelected = MenuTab[e].NameMenuEn;
                SetTabShowMenu();
                SetClearSearchText();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Item_Adapter_Header_ItemClick at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void SetTabShowMenu()
        {
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }
                if (tabSelected == "")
                {
                    tabSelected = "Item";
                }

                menuLayoutManager = new GridLayoutManager(this, 4, 1, false);
                //recyclerHeaderItem.HasFixedSize = true;
                recyclerHeaderItem.HasFixedSize = false;
                recyclerHeaderItem.SetLayoutManager(menuLayoutManager);
                Item_Adapter_Header item_Adapter_Header = new Item_Adapter_Header(MenuTab);
                recyclerHeaderItem.SetAdapter(item_Adapter_Header);
                item_Adapter_Header.ItemClick += Item_Adapter_Header_ItemClick;

                tabItem.Visibility = ViewStates.Gone;
                tabExtra.Visibility = ViewStates.Gone;
                tabStock.Visibility = ViewStates.Gone;
                tabCategory.Visibility = ViewStates.Gone;               

                View view = this.CurrentFocus;
                if (view != null)
                {
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(view.WindowToken, 0);
                }

                switch (tabSelected)
                {
                    case "Item":
                        tabItem.Visibility = ViewStates.Visible;
                        break;
                    case "Extra Topping":
                        tabExtra.Visibility = ViewStates.Visible;
                        break;
                    case "Stock":
                        tabStock.Visibility = ViewStates.Visible;
                        if (checkNet)
                        {
                            CheckJwt();                            
                            await CheckRevisionStock();
                        }         
                        await GetLastRevisionNoStock();
                        break;
                    case "Category":
                        tabCategory.Visibility = ViewStates.Visible;
                        break;
                    case "Discount":
                        break;
                    case "Note":
                        break;
                    default:
                        tabItem.Visibility = ViewStates.Gone;
                        tabCategory.Visibility = ViewStates.Gone;
                        tabExtra.Visibility = ViewStates.Gone;
                        tabStock.Visibility = ViewStates.Gone;
                        break;
                }

                if (dialogLoading != null)
                {
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                if (dialogLoading != null)
                {
                    dialogLoading.Dismiss();
                }
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetTabShowMenu at Item");
                return;
            }
        }

        private void SetExtra()
        {
            try
            {
                listExtraItem = new ListItem(itemExtra);
                item_Adapter_Extra = new Item_Adapter_ItemExtra(listExtraItem, checkNet);
                gridLayoutItemExtra = new GridLayoutManager(this, 1, 1, false);
                recyclerviewlistTopping.SetLayoutManager(gridLayoutItemExtra);
                //recyclerviewlistTopping.HasFixedSize = true;
                recyclerviewlistTopping.HasFixedSize = false;
                int count = items == null ? 0 : itemExtra.Count + 1;
                recyclerviewlistTopping.SetItemViewCacheSize(count);
                recyclerviewlistTopping.SetAdapter(item_Adapter_Extra);

                //Click เพื่อ Update ข้อมูล
                item_Adapter_Extra.ItemClick += item_Adapter_Extra_ItemClick;

                if (item_Adapter_Extra.ItemCount == 0)
                {
                    lnNoItemExtra.Visibility = ViewStates.Visible;
                    lnSearchTopping.Visibility = ViewStates.Gone;
                    recyclerviewlistTopping.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoItemExtra.Visibility = ViewStates.Gone;
                    lnSearchTopping.Visibility = ViewStates.Visible;
                    recyclerviewlistTopping.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetExtra at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void item_Adapter_Extra_ItemClick(object sender, int e)
        {
            try
            {
                var Topping = listExtraItem[e];
                StartActivity(new Intent(context, typeof(AddExtraToppingActivity)));
                AddExtraToppingActivity.SetExtraTopping(Topping);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("item_Adapter_Extra_ItemClick at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async Task GetExtraList()
        {
            try
            {
                itemExtra = new List<Item>();
                ItemManage itemManage = new ItemManage();
                itemExtra = await itemManage.GetToppingItem();
                if (itemExtra == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    itemExtra = new List<Item>();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("item_Adapter_Extra_ItemClick at Item");
            }
        }

        private void ImageButtonBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            FocusNewItem = null;
            FocusCate = null;
        }

        private void AddNote_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(AddNoteActivity));
        }
        private void AddDiscount_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(AddDiscountActivity));
        }

        private void AddExtra_Click(object sender, EventArgs e)
        {
            try
            {
                addExtra.Enabled = false;
                string Role = LoginType;
                bool check = UtilsAll.CheckPermissionRoleUser(Role, "insert", "topping");
                if (!check)
                {
                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                    addExtra.Enabled = true;
                    return;
                }

                Item Topping = null;
                StartActivity(typeof(AddExtraToppingActivity));
                AddExtraToppingActivity.SetExtraTopping(Topping);
                addExtra.Enabled = true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this,ex.Message,ToastLength.Short).Show();
                addExtra.Enabled = true;
            }
        }

        private void Addcategory_Click(object sender, EventArgs e)
        {
            try
            {
                addcategory.Enabled = false;
                string Role = LoginType;
                bool check = UtilsAll.CheckPermissionRoleUser(Role, "insert", "category");
                if (!check)
                {
                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                    addcategory.Enabled = true;
                    return;
                }

                DataCashing.EditSysCategory = string.Empty;
                StartActivity(typeof(AddCategoryActivity));
                addcategory.Enabled = true;
            }
            catch (Exception   ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                addcategory.Enabled = true;
            }
        }

        private void Additem_Click(object sender, EventArgs e)
        {
            try
            {
                additem.Enabled = false;
                bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "item");
                if (!check)
                {
                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                    additem.Enabled = true;
                    return;
                }

                DataCashing.EditItemID = 0;
                StartActivity(typeof(AddItemActivity));
                AddItemActivity.tabSelected = "Item";
                additem.Enabled = true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this,ex.Message,ToastLength.Short).Show();
                additem.Enabled = true;
            }
        }

        async Task GetCategoryData()
        {
            try
            {
                allitem = new List<Item>();
                if (items != null)
                {
                    allitem.AddRange(items);
                }

                if (itemExtra != null)
                {
                    allitem.AddRange(itemExtra);
                }

                CategoryManage category = new CategoryManage();
                lstCategory = await category.GetAllCategory();
                if (lstCategory == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    lstCategory = new List<Category>();
                }

                //SetCategoryData();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetCategoryData at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        async void SetCategoryData()
        {
            try
            {
                listCategory = new ListCategory(lstCategory);
                mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                //recyclerviewlistCategory.HasFixedSize = true;
                recyclerviewlistCategory.HasFixedSize = false;
                recyclerviewlistCategory.SetLayoutManager(mLayoutManager);
                Item_Adapter_Category = new Item_Adapter_Category(listCategory, allitem);
                int count = allitem == null ? 0 : allitem.Count + 1;
                recyclerviewlistCategory.SetItemViewCacheSize(count);
                recyclerviewlistCategory.SetAdapter(Item_Adapter_Category);
                Item_Adapter_Category.ItemClick += Item_Adapter_Category_ItemClick;

                if (Item_Adapter_Category.ItemCount == 0)
                {
                    lnNoCategory.Visibility = ViewStates.Visible;
                    lnSearchCategory.Visibility = ViewStates.Gone;
                    recyclerviewlistCategory.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoCategory.Visibility = ViewStates.Gone;
                    lnSearchCategory.Visibility = ViewStates.Visible;
                    recyclerviewlistCategory.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetCategoryData at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void Item_Adapter_Category_ItemClick(object sender, int e)
        {
            try
            {
                string Role = LoginType;
                bool check = UtilsAll.CheckPermissionRoleUser(Role, "view", "category");
                if (check)
                {
                    DataCashing.EditSysCategory = "EditCategory";
                    var category = listCategory[e];
                    AddCategoryActivity addCategoryActivity = new AddCategoryActivity();
                    DataCashing.EditSysCategoryID = category.SysCategoryID;
                    StartActivity(new Intent(context, typeof(AddCategoryActivity)));
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void SetItemData()
        {
            try
            {
                listItem = new ListItem(items);
                item_Adapter_Item = new Item_Adapter_Item(listItem, checkNet);
                gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                recyclerviewlistitem.SetLayoutManager(gridLayoutItem);
                //recyclerviewlistitem.HasFixedSize = true;
                recyclerviewlistitem.HasFixedSize = false;
                int count = items == null ? 0 : items.Count + 1;
                recyclerviewlistitem.SetItemViewCacheSize(count);
                recyclerviewlistitem.SetAdapter(item_Adapter_Item);
                item_Adapter_Item.ItemClick += Item_Adapter_Item_ItemClick;

                if (item_Adapter_Item.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchItem))
                    {
                        lnNoDataSearchItem.Visibility = ViewStates.Visible;
                        lnNoItem.Visibility = ViewStates.Gone;
                        lnSearchItem.Visibility = ViewStates.Gone;
                        recyclerviewlistitem.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearchItem.Visibility = ViewStates.Gone;
                        lnNoItem.Visibility = ViewStates.Visible;
                        lnSearchItem.Visibility = ViewStates.Gone;
                        recyclerviewlistitem.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchItem.Visibility = ViewStates.Gone;
                    lnNoItem.Visibility = ViewStates.Gone;
                    lnSearchItem.Visibility = ViewStates.Visible;
                    recyclerviewlistitem.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetItemData at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Item_Adapter_Item_ItemClick(object sender, int e)
        {
            try
            {
                var id = listItem[e].SysItemID;
                StartActivity(new Intent(this, typeof(AddItemActivity)));

                DataCashing.EditItemID = id;
                AddItemActivity.tabSelected = "Item";
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("Item_Adapter_Item_ItemClick at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async Task GetItemList()
        {
            try
            {
                items = new List<Item>();
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
                await TinyInsights.TrackPageViewAsync("GetItemList at Item");
                Console.WriteLine(ex.Message);
                Log.Debug("error", ex.Message);
            }
        }
        //---------------------------------------------------------------
        //Filter
        //---------------------------------------------------------------
        private async void SetFilterItemData()
        {
            try
            {
                lstsearchitems = new List<Item>();
                //items ข้อมูลชุดเดิมที่มาจาก onresume
                //
                if (string.IsNullOrEmpty(searchItem))
                {
                    return;
                }

                lstsearchitems = items.Where(m => (m.ItemName.ToLower().Contains(searchItem.ToLower())
                                         || (m.ItemCode != null && m.ItemCode.ToLower().Contains(searchItem.ToLower())))).ToList();
                if (lstsearchitems.Count > 0)
                {
                    lstsearchitems = lstsearchitems.OrderBy(x => x.ItemName).ToList();
                }

                listItem = new ListItem(lstsearchitems);
                item_Adapter_Item = new Item_Adapter_Item(listItem, checkNet);
                gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                recyclerviewlistitem.SetLayoutManager(gridLayoutItem);
                //recyclerviewlistitem.HasFixedSize = true;
                recyclerviewlistitem.HasFixedSize = false;
                int count = items == null ? 0 : items.Count + 1;
                recyclerviewlistitem.SetItemViewCacheSize(count);
                recyclerviewlistitem.SetAdapter(item_Adapter_Item);
                item_Adapter_Item.ItemClick += Item_Adapter_Item_ItemClick;
                SetBtnSearch(btnSearchItem, searchItem);

                if (item_Adapter_Item.ItemCount == 0)
                {
                    lnNoDataSearchItem.Visibility = ViewStates.Visible;
                    lnNoItem.Visibility = ViewStates.Gone;
                    lnSearchItem.Visibility = ViewStates.Visible;
                    recyclerviewlistitem.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoDataSearchItem.Visibility = ViewStates.Gone;
                    lnNoItem.Visibility = ViewStates.Gone;
                    lnSearchItem.Visibility = ViewStates.Visible;
                    recyclerviewlistitem.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetFilterItemData at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void SetFilterCategoryData()
        {
            try
            {
                lstsearchcategory = new List<Category>();
                if (string.IsNullOrEmpty(searchCategory))
                {
                    return;
                }
                lstsearchcategory = lstCategory.Where(m => m.Name.ToLower().Contains(searchCategory.ToLower())).ToList();
                if (lstsearchcategory.Count > 0)
                {
                    lstsearchcategory = lstsearchcategory.OrderBy(x => x.Name).ToList();
                }

                listCategory = new ListCategory(lstsearchcategory);
                mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                //recyclerviewlistCategory.HasFixedSize = true;
                recyclerviewlistCategory.HasFixedSize = false;
                recyclerviewlistCategory.SetLayoutManager(mLayoutManager);
                Item_Adapter_Category = new Item_Adapter_Category(listCategory, allitem);
                int count = allitem == null ? 0 : allitem.Count + 1;
                recyclerviewlistTopping.SetItemViewCacheSize(count);
                recyclerviewlistCategory.SetAdapter(Item_Adapter_Category);
                Item_Adapter_Category.ItemClick += Item_Adapter_Category_ItemClick;
                SetBtnSearch(btnSearchCategory, searchCategory);

                if (Item_Adapter_Category.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchCategory))
                    {
                        lnNoDataSearchCategory.Visibility = ViewStates.Visible;
                        lnNoCategory.Visibility = ViewStates.Gone;
                        lnSearchStock.Visibility = ViewStates.Visible;
                        recyclerviewlistStock.Visibility = ViewStates.Gone;

                    }
                    else
                    {
                        lnNoDataSearchCategory.Visibility = ViewStates.Gone;
                        lnNoCategory.Visibility = ViewStates.Visible;
                        lnSearchStock.Visibility = ViewStates.Gone;
                        recyclerviewlistStock.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchCategory.Visibility = ViewStates.Gone;
                    lnNoCategory.Visibility = ViewStates.Gone;
                    lnSearchStock.Visibility = ViewStates.Visible;
                    recyclerviewlistStock.Visibility = ViewStates.Visible;
                }

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetFilterCategoryData at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void SetFilterExtraToppingData()
        {
            try
            {
                lstsearchitemExtra = new List<Item>();
                if (string.IsNullOrEmpty(searchTopping))
                {
                    return;
                }

                lstsearchitemExtra = itemExtra.Where(m => m.ItemName.ToLower().Contains(searchTopping.ToLower())).ToList();
                if (lstsearchitemExtra.Count > 0)
                {
                    lstsearchitemExtra = lstsearchitemExtra.OrderBy(x => x.ItemName).ToList();
                }

                listExtraItem = new ListItem(lstsearchitemExtra);
                item_Adapter_Extra = new Item_Adapter_ItemExtra(listExtraItem, checkNet);
                gridLayoutItemExtra = new GridLayoutManager(this, 1, 1, false);
                recyclerviewlistTopping.SetLayoutManager(gridLayoutItemExtra);
                //recyclerviewlistTopping.HasFixedSize = true;
                recyclerviewlistTopping.HasFixedSize = false;
                int count = itemExtra == null ? 0 : itemExtra.Count + 1;
                recyclerviewlistTopping.SetItemViewCacheSize(count);
                recyclerviewlistTopping.SetAdapter(item_Adapter_Extra);
                item_Adapter_Extra.ItemClick += item_Adapter_Extra_ItemClick;
                SetBtnSearch(btnSearchTopping, searchTopping);

                if (item_Adapter_Extra.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchTopping))
                    {
                        lnNoDataSearchItemExtra.Visibility = ViewStates.Visible;
                        lnNoItemExtra.Visibility = ViewStates.Gone;
                        lnSearchTopping.Visibility = ViewStates.Visible;
                        recyclerviewlistTopping.Visibility = ViewStates.Gone;

                    }
                    else
                    {
                        lnNoDataSearchItemExtra.Visibility = ViewStates.Gone;
                        lnNoItemExtra.Visibility = ViewStates.Visible;
                        lnSearchTopping.Visibility = ViewStates.Gone;
                        recyclerviewlistTopping.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchItemExtra.Visibility = ViewStates.Gone;
                    lnNoItemExtra.Visibility = ViewStates.Gone;
                    lnSearchTopping.Visibility = ViewStates.Visible;
                    recyclerviewlistTopping.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetFilterExtraToppingData at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        internal static void SetFocusNewItem(Item item)
        {
            try
            {
                FocusNewItem = item;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFocusItem at Item");
            }
        }

        internal static void SetFocusCategory(Category data)
        {
            try
            {
                FocusCate = data;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFocusItem at Item");
            }
        }

        private async void ItemFocus()
        {
            try
            {
                if (FocusNewItem != null)
                {
                    if (FocusNewItem.SaleItemType == 'T')
                    {
                        return;
                    }
                    int index =  -1;
                    if (items != null)
                    {
                        if (items.Count == 0)
                        {
                            items.Add(FocusNewItem);
                            SetItemData();
                            StockFocus();
                            FocusNewItem = null;
                            return;
                        }

                        index = items.FindIndex(x => x.SysItemID == FocusNewItem.SysItemID);
                        if (index != -1)
                        {
                            items.RemoveAt(index);
                        }
                        items.Insert(0, FocusNewItem);
                    }
                    if (lstsearchitems?.Count > 0)
                    {
                        index = lstsearchitems.FindIndex(x => x.SysItemID == FocusNewItem.SysItemID);
                        if (index != -1)
                        {
                            lstsearchitems.RemoveAt(index);
                        }
                        lstsearchitems.Insert(0, FocusNewItem);
                    }
                    item_Adapter_Item.NotifyDataSetChanged();
                    StockFocus();
                    if (FocusNewItem.SysCategoryID != 0)
                    {
                        await GetCategoryData();
                        SetCategoryData();
                    }                    
                    FocusNewItem = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemFocus at Item");
                Toast.MakeText(itemActivity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void ToppingFocus()
        {
            try
            {
                if (FocusNewItem != null)
                {
                    int index = -1;
                    if (itemExtra != null)
                    {
                        if (itemExtra.Count == 0)
                        {
                            itemExtra.Add(FocusNewItem);
                            SetExtra();
                            StockFocus();
                            FocusNewItem = null;
                            return;
                        }

                        index = itemExtra.FindIndex(x => x.SysItemID == FocusNewItem.SysItemID);
                        if (index != -1)
                        {
                            itemExtra.RemoveAt(index);
                        }
                        itemExtra.Insert(0, FocusNewItem);
                    }
                    if (lstsearchitemExtra?.Count > 0)
                    {
                        index = lstsearchitemExtra.FindIndex(x => x.SysItemID == FocusNewItem.SysItemID);
                        if (index != -1)
                        {
                            lstsearchitemExtra.RemoveAt(index);
                        }
                        lstsearchitemExtra.Insert(0, FocusNewItem);
                    }
                    item_Adapter_Extra.NotifyDataSetChanged();
                    StockFocus();
                    if (FocusNewItem.SysCategoryID != 0)
                    {
                        await GetCategoryData();
                        SetCategoryData();
                    }
                    FocusNewItem = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ToppingFocus at Item");
                Toast.MakeText(itemActivity, ex.Message, ToastLength.Short).Show();
            }
        }

        private  void CategoryFocus()
        {
            try
            {
                if (FocusCate != null)
                {
                    int index = -1;                    
                    if (lstCategory != null)
                    {
                        if (lstCategory.Count == 0)
                        {
                            lstCategory.Add(FocusCate);
                            SetCategoryData();
                            FocusCate = null;
                            return;
                        }
                        index = lstCategory.FindIndex(x => x.SysCategoryID == FocusCate.SysCategoryID);
                        if (index != -1)
                        {
                            lstCategory.RemoveAt(index);
                        }
                        lstCategory.Insert(0, FocusCate);
                    }
                    if (lstsearchcategory?.Count > 0)
                    {
                        index = lstsearchcategory.FindIndex(x => x.SysCategoryID == FocusCate.SysCategoryID);
                        if (index != -1)
                        {
                            lstsearchcategory.RemoveAt(index);
                        }
                        lstsearchcategory.Insert(0, FocusCate);
                    }
                    Item_Adapter_Category.NotifyDataSetChanged();
                    FocusCate = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CategoryFocus at Item");
                Toast.MakeText(itemActivity, ex.Message, ToastLength.Short).Show();
            }
        }

        private  void StockFocus()
        {
            try
            {
                if (FocusNewItem != null)
                {
                    int index = -1;
                    if (itemsStock?.Count > 0)
                    {
                        index = itemsStock.FindIndex(x => x.SysItemID == FocusNewItem.SysItemID);
                        if (index != -1)
                        {
                            itemsStock.RemoveAt(index);
                        }
                        itemsStock.Insert(0, FocusNewItem);
                    }
                    if (lstsearchitemsStock?.Count > 0)
                    {
                        index = lstsearchitemsStock.FindIndex(x => x.SysItemID == FocusNewItem.SysItemID);
                        if (index != -1)
                        {
                            lstsearchitemsStock.RemoveAt(index);
                        }
                        lstsearchitemsStock.Insert(0, FocusNewItem);
                    }
                    Item_Adapter_Stock.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemFocus at Item");
                Toast.MakeText(itemActivity, ex.Message, ToastLength.Short).Show();
            }
        }


        //flagChange = true มีการเปลี่ยนแปลงของข้อมูล
        protected override async void OnResume()
        {
            try
            {
                base.OnResume();

                CheckJwt();

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                checkNet = await GabanaAPI.CheckSpeedConnection();
                SetTabMenu();
                SetTabShowMenu();

                if (DataCashingAll.flagItemChange)
                {
                    if (string.IsNullOrEmpty(searchItem) && string.IsNullOrEmpty(searchItemCode))
                    {
                        await GetItemList();
                        SetItemData();
                        SetBtnSearch(btnSearchItem, searchItem);
                    }
                    else
                    {
                        await GetItemList();
                        SetFilterItemData();
                    }

                    if (string.IsNullOrEmpty(searchTopping))
                    {
                        await GetExtraList();
                        SetExtra();
                        SetBtnSearch(btnSearchTopping, searchTopping);
                    }
                    else
                    {
                        await GetExtraList();
                        SetFilterExtraToppingData();
                    }
                    DataCashingAll.flagItemChange = false;
                }
               
                if (DataCashingAll.flagCategoryChange)
                {
                    if (string.IsNullOrEmpty(searchCategory))
                    {
                        await GetCategoryData();
                        SetCategoryData();
                        SetBtnSearch(btnSearchCategory, searchCategory);
                    }
                    else
                    {
                        await GetCategoryData();
                        SetFilterCategoryData();
                    }
                    DataCashingAll.flagCategoryChange = false;
                }

                if (DataCashingAll.flagItemOnBranchChange)
                {
                    if (string.IsNullOrEmpty(searchStock))
                    {
                        await GetItemOnStock();
                        SetItemOnStock();
                        SetBtnSearch(btnSearchStock, searchStock);
                    }
                    else
                    {
                        await GetItemOnStock();
                        SetItemOnStock();
                        SetFilterItemOnStock();
                    }
                    DataCashingAll.flagItemOnBranchChange = false;
                }

                ItemFocus();
                ToppingFocus();
                CategoryFocus();

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                Log.Debug("connectpass", "Item" + "OnResume");
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Item");
                Toast.MakeText(itemActivity, "OnResume at Item" + ex.Message, ToastLength.Short).Show();
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
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            deviceAsleep = true;
            pauseDate = DateTime.Now;
        }

        public void Resume()
        {
            OnResume();
        }
        

        public static async void OpenDialogImage(Item item)
        {
            try
            {
                string path = "";
                //MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.dialog_item.ToString();
                bundle.PutString("message", myMessage);
                if (!string.IsNullOrEmpty(item.PicturePath))
                {
                    if (item.PicturePath.Contains("http"))
                    {
                        bundle.PutString("OpenCloudPicture", item.PicturePath);
                        path = item.PicturePath;
                    }
                    else
                    {
                        Java.IO.File imgFile = new Java.IO.File(item.PictureLocalPath);
                        if (imgFile != null)
                        {
                            bundle.PutString("OpenCloudPicture", imgFile.AbsolutePath);
                            path = imgFile.AbsolutePath;
                        }
                    }
                }
                Show_Dialog_Item dialog_Item = Show_Dialog_Item.NewInstance(path);
                dialog_Item.Show(itemActivity.SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OpenDialogImage at item");
                return;
            }
        }

        private class ItemSwipeHelper : MySwipeHelper
        {
            Context context;
            RecyclerView recyclerView;
            int buttonWidth;
            public ItemSwipeHelper(Context context, RecyclerView recyclerView, int buttonWidth) : base(context, recyclerView, buttonWidth)
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
                    new MyDeleteItemButtonClick(this)));
            }

            private class MyDeleteItemButtonClick : MyButtonClickListener
            {
                private ItemSwipeHelper myImplementSwipeHelper;

                public MyDeleteItemButtonClick(ItemSwipeHelper myImplementSwipeHelper)
                {
                    this.myImplementSwipeHelper = myImplementSwipeHelper;
                }
                public async void OnClick(int position)
                {
                    try
                    {
                        var LoginType = Preferences.Get("LoginType", "");
                        string Role = LoginType;
                        bool check = UtilsAll.CheckPermissionRoleUser(Role, "delete", "item");
                        if (check)
                        {
                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                            bundle.PutString("message", myMessage);
                            bundle.PutInt("systemID", (int)listItem[position].SysItemID);
                            bundle.PutString("deleteType", "item");
                            bundle.PutString("fromPage", "main");
                            dialog.Arguments = bundle;
                            dialog.Show(itemActivity.SupportFragmentManager, myMessage);
                        }
                        else
                        {
                            Toast.MakeText(myImplementSwipeHelper.context, myImplementSwipeHelper.context.GetString(Resource.String.notperm), ToastLength.Short).Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        await TinyInsights.TrackErrorAsync(ex);
                        await TinyInsights.TrackPageViewAsync("ItemSwipeHelper at Item");
                        Toast.MakeText(myImplementSwipeHelper.context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
            }
        }
        private class ItemExtraSwipeHelper : MySwipeHelper
        {
            Context context;
            RecyclerView recyclerView;
            int buttonWidth;
            public ItemExtraSwipeHelper(Context context, RecyclerView recyclerView, int buttonWidth) : base(context, recyclerView, buttonWidth)
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
                    new MyDeleteItemExtraButtonClick(this)));
            }

            private class MyDeleteItemExtraButtonClick : MyButtonClickListener
            {
                private ItemExtraSwipeHelper itemExtraSwipeHelper;

                public MyDeleteItemExtraButtonClick(ItemExtraSwipeHelper itemExtraSwipeHelper)
                {
                    this.itemExtraSwipeHelper = itemExtraSwipeHelper;
                }

                public async void OnClick(int position)
                {
                    try
                    {
                        var LoginType = Preferences.Get("LoginType", "");
                        string Role = LoginType;
                        bool check = UtilsAll.CheckPermissionRoleUser(Role, "delete", "topping");
                        if (check)
                        {
                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                            bundle.PutString("message", myMessage);
                            bundle.PutInt("systemID", (int)listExtraItem[position].SysItemID);
                            bundle.PutString("deleteType", "topping");
                            bundle.PutString("fromPage", "main");
                            dialog.Arguments = bundle;
                            dialog.Show(itemActivity.SupportFragmentManager, myMessage);
                        }
                        else
                        {
                            Toast.MakeText(itemExtraSwipeHelper.context, itemExtraSwipeHelper.context.GetString(Resource.String.notperm), ToastLength.Short).Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        await TinyInsights.TrackPageViewAsync("OnClick at Item");
                        Toast.MakeText(itemExtraSwipeHelper.context, $"Can't delete{ex.Message}", ToastLength.Short).Show();
                        return;
                    }
                }
            }
        }
        private class ItemCategorySwipeHelper : MySwipeHelper
        {
            Context context;
            RecyclerView recyclerView;
            int buttonWidth;
            public ItemCategorySwipeHelper(Context context, RecyclerView recyclerView, int buttonWidth) : base(context, recyclerView, buttonWidth)
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
                   new ItemCategoryDeleteButtonClick(this)));
            }

            private class ItemCategoryDeleteButtonClick : MyButtonClickListener
            {
                private ItemCategorySwipeHelper itemCategorySwipeHelper;

                public ItemCategoryDeleteButtonClick(ItemCategorySwipeHelper itemCategorySwipeHelper)
                {
                    this.itemCategorySwipeHelper = itemCategorySwipeHelper;
                }

                public async void OnClick(int position)
                {
                    DataCashing.EditSysCategoryID = listCategory[position].SysCategoryID;
                    try
                    {
                        var LoginType = Preferences.Get("LoginType", "");
                        string Role = LoginType;
                        bool check = UtilsAll.CheckPermissionRoleUser(Role, "delete", "category");
                        if (check)
                        {
                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                            bundle.PutString("message", myMessage);
                            bundle.PutString("deleteType", "category");
                            bundle.PutString("fromPage", "main");
                            dialog.Arguments = bundle;
                            dialog.Show(itemActivity.SupportFragmentManager, myMessage);
                        }
                        else
                        {
                            Toast.MakeText(itemCategorySwipeHelper.context, itemCategorySwipeHelper.context.GetString(Resource.String.notperm), ToastLength.Short).Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        await TinyInsights.TrackErrorAsync(ex);
                        await TinyInsights.TrackPageViewAsync("ItemCategorySwipeHelper at Item");
                        Toast.MakeText(itemCategorySwipeHelper.context, $"Can't delete{ex.Message}", ToastLength.Short).Show();
                        return;
                    }
                }
            }
        }

        private async Task GetOnlineDataitem()
        {
            try
            {
#pragma warning disable CS0219 // Variable is assigned but its value is never used
                int SysItemIdFocus = 0;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
                List<Item> lstInsertItemImage = new List<Item>();
                List<Item> lstUpdateItemImage = new List<Item>();
                List<Item> GetAllitem = new List<Item>();
                List<Gabana3.JAM.Items.ItemWithItemExSizes> UpdateItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                List<Gabana3.JAM.Items.ItemWithItemExSizes> InsertItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();

                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 30).FirstOrDefault();
                if (revisionNo != null)
                {
                    #region Item                    
                    try
                    {
                        var allItem = await GabanaAPI.GetDataItem((int)revisionNo.LastRevisionNo, 0);

                        if (allItem == null)
                        {
                            return;
                        }
                        else if (allItem?.ItemsWithItemExSizes.Count == 0)
                        {
                            return;
                        }
                        else
                        {
                            double round = 0, addrount = 0;
                            round = allItem.totalItems / 100;
                            addrount = round + 1;
                            double percentage = 0, temp = 0;
                            percentage = (25 / addrount);
                            temp = percentage;
                            percentage = 0;

                            for (int j = 0; j < addrount; j++)
                            {
                                allItem = await GabanaAPI.GetDataItem((int)revisionNo.LastRevisionNo, j);

                                if (allItem == null)
                                {
                                    break;
                                }

                                if (allItem.totalItems == 0)
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                            Utils.DeletePictureItemMaster(Bulkitem.Select(x => x.ThumbnailLocalPath).ToList());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                            Log.Error("connecterror", "Bulkitem,BulkitemExsize : " + ex.Message);
                                            throw ex;
                                        }
                                    }
                                    lstInsertItemImage.AddRange(Bulkitem);
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

                            //insert Image to Local เมื่อเพิ่มข้อมูลทั้งหมดสำเร็จ ทั้งเคสเพิ่มและเคสอัปเดต
                            Log.Debug("connectpass", "InsertPictureLocal(lstItemImage)" + "lstInsertItemImage " + lstInsertItemImage.Count);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            Task.Factory.StartNew(() => Utils.InsertPictureLocalItem(lstInsertItemImage));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            //Function Check Update Image 
                            Log.Debug("connectpass", "UpdateImageItem(UpdateItem)" + "UpdateItem " + UpdateItem.Count);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            Task.Factory.StartNew(() => Utils.UpdateImageItem(UpdateItem));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            Log.Debug("connectpass", "listRivisionItem");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxItemRevision);
                    }
                    #endregion
                }
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
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 20).FirstOrDefault();
                if (revisionNo != null)
                {
                    #region Category
                    try
                    {
                        //Get Category API
                        var allcategory = await GabanaAPI.GetDataCategory((int)revisionNo.LastRevisionNo);

                        if (allcategory == null)
                        {
                            return;
                        }

                        if (allcategory.Categories.Count == 0 && allcategory.CategoryBins.Count == 0)
                        {
                            return;
                        }

                        int maxCategory = 0;
                        if (allcategory.Categories.Count > 0)
                        {
                            allcategory.Categories.ToList().OrderBy(x => x.RevisionNo);
                            maxCategory = allcategory.Categories.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();

                            //check ว่ามีไหม
                            List<Category> GetallCate = await categoryManage.GetAllCategory();
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
                                    var insertOrreplace = await categoryManage.InsertOrReplaceCategory(category);
                                    maxCategoryRevision = item.RevisionNo;
                                }
                            }
                        }

                        if (allcategory.CategoryBins.Count > 0)
                        {
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
                                var delete = await categoryManage.DeleteCategory(item.MerchantID, item.SysCategoryID);
                                if (!delete)
                                {
                                    var data = await categoryManage.GetCategory(item.MerchantID, item.SysCategoryID);
                                    if (data != null)
                                    {
                                        data.DataStatus = 'D';
                                        data.FWaitSending = 0;
                                        await categoryManage.UpdateCategory(category);
                                    }
                                }
                                maxCategoryRevision = item.RevisionNo;
                            }
                        }

                        await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxCategory);
                        Log.Debug("connectpass", "listRivisionCategory");
                        //DataCashingAll.flagCategoryChange = true;
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("connecterror", "listRivisionCategory : " + ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxCategoryRevision);
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineDataCategory at item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task GetOnlineDataItemonBranch()
        {
            try
            {
                int maxItemOnBranchRevision = 0;
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 31).FirstOrDefault();
                if (revisionNo != null)
                {
                    #region ItemOnBranch 
                    try
                    {
                        var allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)revisionNo.LastRevisionNo, 0);

                        if (allItemOnBranch == null)
                        {
                            return;
                        }

                        if (allItemOnBranch.totalItemOnBranch == 0)
                        {
                            return;
                        }

                        if (allItemOnBranch.totalItemOnBranch > 0)
                        {
                            int round = 0, addrount = 0;
                            round = allItemOnBranch.totalItemOnBranch / 100;
                            addrount = round + 1;

                            for (int j = 0; j < addrount; j++)
                            {
                                allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)revisionNo.LastRevisionNo, j);

                                if (allItemOnBranch == null)
                                {
                                    break;
                                }

                                if (allItemOnBranch.totalItemOnBranch == 0)
                                {
                                    break;
                                }

                                allItemOnBranch.ItemOnBranches.OrderBy(x => x.RevisionNo);
                                var maxItemOnBranch = allItemOnBranch.ItemOnBranches.Max(x => x.RevisionNo);

                                //check ว่ามีไหม
                                List<ORM.Master.ItemOnBranch> UpdateItemOnBranch = new List<ORM.Master.ItemOnBranch>();
                                List<ORM.Master.ItemOnBranch> InsertItemOnBranch = new List<ORM.Master.ItemOnBranch>();
                                List<ItemOnBranch> GetallItemonBranch = await onBranchManage.GetAllItemOnBranchOnlyMerchantID(DataCashingAll.MerchantId);
                                UpdateItemOnBranch.AddRange(allItemOnBranch.ItemOnBranches.Where(x => GetallItemonBranch.Select(y => (long)y.SysItemID).ToList().Contains(x.SysItemID)).ToList());
                                InsertItemOnBranch.AddRange(allItemOnBranch.ItemOnBranches.Where(x => !(GetallItemonBranch.Select(y => (long)y.SysItemID).ToList().Contains(x.SysItemID))).ToList());

                                //Insert ItemonBranch
                                if (InsertItemOnBranch.Count > 0)
                                {
                                    List<ItemOnBranch> BulkItemOnBranch = new List<ItemOnBranch>();

                                    foreach (var itemOnBranch in InsertItemOnBranch)
                                    {
                                        BulkItemOnBranch.Add(new ItemOnBranch()
                                        {
                                            MerchantID = itemOnBranch.MerchantID,
                                            SysBranchID = itemOnBranch.SysBranchID,
                                            SysItemID = itemOnBranch.SysItemID,
                                            BalanceStock = itemOnBranch.BalanceStock,
                                            MinimumStock = itemOnBranch.MinimumStock,
                                            LastDateBalanceStock = itemOnBranch.LastDateBalanceStock,
                                        });
                                    }

                                    using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                    {
                                        try
                                        {
                                            await db.BulkCopyAsync(BulkItemOnBranch);
                                        }
                                        catch (Exception ex)
                                        {
                                            var errorRevison = InsertItemOnBranch.Select(x => x.RevisionNo).Min();
                                            maxItemOnBranchRevision = errorRevison;
                                            Log.Error("connecterror", "BulkItemOnBranch :" + ex.Message);
                                            throw ex;
                                        }
                                    }
                                }

                                //Update ItemonBranch
                                if (UpdateItemOnBranch.Count > 0)
                                {
                                    foreach (var item in UpdateItemOnBranch)
                                    {
                                        ItemOnBranch stock = new ItemOnBranch()
                                        {
                                            MerchantID = item.MerchantID,
                                            SysBranchID = item.SysBranchID,
                                            SysItemID = item.SysItemID,
                                            BalanceStock = item.BalanceStock,
                                            MinimumStock = item.MinimumStock,
                                            LastDateBalanceStock = item.LastDateBalanceStock,
                                        };
                                        var insertStock = await onBranchManage.InsertorReplaceItemOnBranch(stock);
                                        maxItemOnBranchRevision = item.RevisionNo;
                                    }
                                }

                                await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxItemOnBranch);
                            }
                            Log.Debug("connectpass", "listRivisionItemOnBranch");
                            //DataCashingAll.flagItemOnBranchChange = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("connecterror", "listRivisionItemOnBranch : " + ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxItemOnBranchRevision);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineDataTemonBranch at item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        async Task CheckRevisionStock()
        {
            try
            {
                //เพิ่มฟังก์ชันสำหรับตรวจสอบ RevisionNo ถ้าน้อยกว่าเรียกข้อมูลใหม่
                MyFirebaseMessagingService myFirebaseMessaging = new MyFirebaseMessagingService();
                List<SystemRevisionNo> lstsystemRevisions = new List<SystemRevisionNo>();
                List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
                SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
                int CloudRevisionNo = 0;
                int DeviceRevisionNo = 0;

                //Get Cloud 
                lstsystemRevisions = await GabanaAPI.GetDataSystemRevisionNo();
                //Get Local 
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();

                if (lstsystemRevisions.Count > 0)
                {
                    foreach (var item in lstsystemRevisions)
                    {
                        if (item.SystemID == 20)
                        {
                            CloudRevisionNo = (int)lstsystemRevisions?.Where(x => x.SystemID == 20).FirstOrDefault()?.LastRevisionNo;
                            DeviceRevisionNo = (int)listRivision?.Where(x => x.SystemID == 20).FirstOrDefault()?.LastRevisionNo;
                            if (CloudRevisionNo > DeviceRevisionNo)
                            {
                                await GetOnlineDataCategory();
                                DataCashingAll.flagItemChange = true;
                                DataCashingAll.flagItemOnBranchChange = true;
                                DataCashingAll.flagCategoryChange = true;
                            }
                        }
                        if (item.SystemID == 30)
                        {
                            CloudRevisionNo = (int)lstsystemRevisions?.Where(x => x.SystemID == 30).FirstOrDefault()?.LastRevisionNo;
                            DeviceRevisionNo = (int)listRivision?.Where(x => x.SystemID == 30).FirstOrDefault()?.LastRevisionNo;
                            if (CloudRevisionNo > DeviceRevisionNo)
                            {
                                await GetOnlineDataitem();
                                DataCashingAll.flagItemChange = true;
                                DataCashingAll.flagItemOnBranchChange = true;
                                DataCashingAll.flagCategoryChange = true;
                            }
                        }
                        if (item.SystemID == 31)
                        {
                            CloudRevisionNo = (int)lstsystemRevisions?.Where(x => x.SystemID == 31).FirstOrDefault()?.LastRevisionNo;
                            DeviceRevisionNo = (int)listRivision?.Where(x => x.SystemID == 31).FirstOrDefault()?.LastRevisionNo;
                            if (CloudRevisionNo > DeviceRevisionNo)
                            {
                                await GetOnlineDataItemonBranch();
                                DataCashingAll.flagItemChange = true;
                                DataCashingAll.flagItemOnBranchChange = true;
                                DataCashingAll.flagCategoryChange = true;
                            }
                        }
                    }

                    if (DataCashingAll.flagItemChange)
                    {
                        if (string.IsNullOrEmpty(searchItem) && string.IsNullOrEmpty(searchItemCode))
                        {
                            await GetItemList();
                            SetItemData();
                            SetBtnSearch(btnSearchItem, searchItem);
                        }
                        else
                        {
                            await GetItemList();
                            SetFilterItemData();
                        }

                        if (string.IsNullOrEmpty(searchTopping))
                        {
                            await GetExtraList();
                            SetExtra();
                            SetBtnSearch(btnSearchTopping, searchTopping);
                        }
                        else
                        {
                            await GetExtraList();
                            SetFilterExtraToppingData();
                        }
                        DataCashingAll.flagItemChange = false;
                    }

                    if (DataCashingAll.flagCategoryChange)
                    {
                        if (string.IsNullOrEmpty(searchCategory))
                        {
                            await GetCategoryData();
                            SetCategoryData();
                            SetBtnSearch(btnSearchCategory, searchCategory);
                        }
                        else
                        {
                            await GetCategoryData();
                            SetFilterCategoryData();
                        }
                        DataCashingAll.flagCategoryChange = false;
                    }

                    if (DataCashingAll.flagItemOnBranchChange)
                    {
                        if (string.IsNullOrEmpty(searchStock))
                        {
                            await GetItemOnStock();
                            SetItemOnStock();
                            SetBtnSearch(btnSearchStock, searchStock);
                        }
                        else
                        {
                            await GetItemOnStock();
                            SetItemOnStock();
                            SetFilterItemOnStock();
                        }
                        DataCashingAll.flagItemOnBranchChange = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckRevisionStock at Item");
            }
        }

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                Log.Debug("Token","Token" + " " + res.gbnJWT);
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

        async Task GetLastRevisionNoStock()
        {
            try
            {
                SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
                SystemRevisionNo RevisionNoStock = new SystemRevisionNo();

                RevisionNoStock = await systemRevisionNoManage.GetSystemRevisionNo(DataCashingAll.MerchantId, 31);
                if (RevisionNoStock != null)
                {
                    LastRevisionNoStock = RevisionNoStock.LastRevisionNo;
                }

                txtStockRevision.Text = "Stock Revision No. " + LastRevisionNoStock;

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetLastRevisionNoStock at Item");
            }
        }


    }

}