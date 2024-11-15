
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware.Display;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Telecom;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Gabana.Droid.Tablet.Adapter.POS;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
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

namespace Gabana.Droid.Tablet.Fragments.POS
{
    public class POS_Fragment_Main : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static POS_Fragment_Main NewInstance()
        {
            POS_Fragment_Main frag = new POS_Fragment_Main();
            return frag;
        }

        View view;
        public static List<Item> AllItem = new List<Item>();
        public static List<Item> AllItemStatusD = new List<Item>();
        public static List<Category> lstCategory = new List<Category>();
        List<Item> DefaultDataItem = new List<Item>();
        List<Item> DefaultDataTopping = new List<Item>();
        List<Item> DefaultAllItem = new List<Item>();
        EditText txtSearchPos;
        string usernamelogin;
        public static int DetailNo = 0;
        SwipeRefreshLayout refreshlayout;
        public static int? setQuantity;
        public static int? addQuantity;
        public static int totlaItems;
        public static POS_Fragment_Main fragment_main;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.pos_fragment_main, container, false);
            try
            {
                fragment_main = this;
                ComBineUI();
                SetUIEvent();
                CheckJwt();

                DataCashingAll.flagItemChange = true;
                DataCashingAll.flagCategoryChange = true;

                SetTabMenu();
                //SetTabShowMenu();
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


                tranWithDetails = MainActivity.tranWithDetails;  
                DetailNo = tranWithDetails.tranDetailItemWithToppings.Count;
                if (setQuantity == null || setQuantity <= 1)
                {
                    addQuantity = 1;
                }
                DataCashing.setQuantityToCart = (int)addQuantity;
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                
                refreshlayout.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagItemChange = true;
                    DataCashingAll.flagCategoryChange = true;
                    //refresh Online Data
                    if (!await GabanaAPI.CheckNetWork())
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }
                    else
                    {
                        await Task.Run(async () => {
                            await MainActivity.main_activity.GetOnlineDataitem();
                            await MainActivity.main_activity.GetOnlineDataCategory();

                            //เรียกการโหลดที่หน้าหลักใหม่ เพื่ออัปเดตเป็นค่าล่าสุด
                            await MainActivity.main_activity.GetAllData();
                            Item_Fragment_Main.fragment_main.OnResume();
                        });

                        this.Activity.RunOnUiThread(() =>
                        {
                            OnResume();
                        });
                    }                  
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                _ = TinyInsights.TrackPageViewAsync("OnCreateView : POS_Fragment_Main");
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }

        private void SetUIEvent()
        {
            btnShowMenu.Click += BtnShowMenu_Click;
            btnBarcode.Click += BtnBarcode_Click;
            btnOrder.Click += BtnOrder_Click;
            btnDummy.Click += BtnDummy_Click;
            btnBasket.Click += BtnBasket_Click;
            txtSearchPos.TextChanged += TxtSearchPos_TextChanged;
            txtSearchPos.KeyPress += TxtSearchPos_KeyPress;
            txtSearchPos.Click += TxtSearchPos_Click;
            txtSearchPos.FocusChange += TxtSearchPos_FocusChange;
        }

        public async override void OnResume()
        {
            //DialogLoading dialogLoading = new DialogLoading();
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                if (!IsAdded)
                {
                    return;
                }

                //if (dialogLoading.Cancelable != false)
                //{
                //    dialogLoading.Cancelable = false;
                //    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                //}

                CheckJwt();

                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                tranWithDetails = MainActivity.tranWithDetails;

                #region //ถ้ามีการแก้ไขตั้งค่า ตอนเลือกสินค้าใส่ตะกร้า
                //เพิ่มเงื่อนไข check กรณีมีการแก้ไข VAT,Service Charge

                //Service Charge
                if (tranWithDetails.tran.FmlServiceCharge.ToString() != DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE.ToString())
                {
                    tranWithDetails.tran.FmlServiceCharge = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE;
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    MainActivity.tranWithDetails = tranWithDetails;
                }

                //vat
                decimal.TryParse(tranWithDetails.tran.TaxRate?.ToString(), out decimal tranvat);
                decimal.TryParse(DataCashingAll.setmerchantConfig.TAXRATE, out decimal merchantvat);
                if (tranvat != merchantvat)
                {
                    tranWithDetails.tran.TaxRate = merchantvat;
                    tranWithDetails.tran.TranTaxType = char.Parse(DataCashingAll.setmerchantConfig.TAXTYPE?.ToString());
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    MainActivity.tranWithDetails = tranWithDetails;
                } 
                #endregion

                AllItemStatusD = MainActivity.allData.AllItemStatusD;
                DefaultDataItem = MainActivity.allData.DefaultDataItem;
                DefaultDataTopping = MainActivity.allData.DefaultDataTopping;
                IsActive = true;

                checkNet = DataCashing.CheckNet;

                SetTabMenu();
                SetTabShowMenu();
                GetnoteCategory();

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

                await ShowDetail();

                //dialogLoading?.DismissAllowingStateLoss();
                //dialogLoading?.Dismiss();
            }
            catch (Exception ex)
            {
                //dialogLoading?.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at POS_Fragment_Main");
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

                lstCategoryNote = await category.GetNoteCategoryOption();
                noneCategory.AddRange(lstCategoryNote);

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
                POS_Dialog_Option.SetlistCategoryNote(noneCategory);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetnoteCategory at POS");
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

        internal void SettextQuantity(int setQuanity)
        {
            txtQuantity.Text = "x" + setQuanity.ToString("#,###");
        }
                
        TranTradDiscountManage discountManage = new TranTradDiscountManage();
        TranDetailItemManage tranDetailItemManage = new TranDetailItemManage();
        TranDetailItemToppingManage toppingManage = new TranDetailItemToppingManage();
        TranPaymentManage tranPaymentManage = new TranPaymentManage();
        
        RecyclerView rcvCategoryItem, rcvItem;
        ImageButton btnSearch;
        ImageButton btnShowMenu;
        TextView txtNameItem;
        CardView cardView;
        ImageView ImageShow;
        public static string nameCategory = "", tabSelected;
        List<Item> listItemBody;
        List<Item> listToppingBody;
        List<Item> listFavorite = new List<Item>();                
        public static ListCategory listCategory;
        public static ListItem listItem;
        CategoryManage CategoryManage = new CategoryManage();
        string SearchItem;
        string CURRENCYSYMBOLS;
        public int fillter;
        public static long sysCategory = 0;
        public static bool checkNet = false, IsActive = false;
        public static TranWithDetailsLocal tranWithDetails;
        RecyclerView.LayoutManager mLayoutManager;
        public static bool favoriteMenu, ToppingMenu;
        Pos_Adapter_Row pos_Adapter_Row; //Body Show Row Menu
        Pos_Adapter_Grid pos_Adapter_Grid; //Body Show Grid Menu
        GridLayoutManager gridLayoutManager;
        ImageButton btnBarcode, btnOrder, btnDummy;
        public static ImageButton btnBasket;
        public static TextView txtQuantity;
        LinearLayout lnGroupButton;

        private void ComBineUI()
        {
            rcvCategoryItem = view.FindViewById<RecyclerView>(Resource.Id.rcvCategoryItem);
            rcvItem = view.FindViewById<RecyclerView>(Resource.Id.rcvItem);
            txtSearchPos = view.FindViewById<EditText>(Resource.Id.txtSearchPos);
            btnSearch = view.FindViewById<ImageButton>(Resource.Id.btnSearch);
            btnShowMenu = view.FindViewById<ImageButton>(Resource.Id.btnShowMenu);
            ImageShow = view.FindViewById<ImageView>(Resource.Id.imgShowAnime);
            cardView = view.FindViewById<CardView>(Resource.Id.cardViewGrid);
            txtNameItem = view.FindViewById<TextView>(Resource.Id.txtNameItem);
            btnBarcode = view.FindViewById<ImageButton>(Resource.Id.btnBarcode);
            btnOrder = view.FindViewById<ImageButton>(Resource.Id.btnOrder);
            btnDummy = view.FindViewById<ImageButton>(Resource.Id.btnDummy);
            btnBasket = view.FindViewById<ImageButton>(Resource.Id.btnBasket);
            txtQuantity = view.FindViewById<TextView>(Resource.Id.txtQuantity);
            lnGroupButton = view.FindViewById<LinearLayout>(Resource.Id.lnGroupButton);
            refreshlayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
        }

        private void TxtSearchPos_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(txtSearchPos.Text.Trim()))
            {
                lnGroupButton.Visibility = ViewStates.Gone;
                btnSearch.SetBackgroundResource(Color.Transparent);
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
                btnSearch.Click += BtnSearch_Click;
            }
            else
            {
                lnGroupButton.Visibility = ViewStates.Visible;
                btnSearch.SetBackgroundResource(Color.Transparent);
                btnSearch.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            txtSearchPos.Text = string.Empty;
            txtSearchPos.ClearFocus();
            lnGroupButton.Visibility = ViewStates.Visible;
        }

        private void TxtSearchPos_Click(object sender, EventArgs e)
        {
            lnGroupButton.Visibility = ViewStates.Gone;
        }

        private void TxtSearchPos_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                //SetBtnSearchItem();
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    listItemBody = SearchItemfromCategory();
                    flagViewShow();
                }
                View view = this.Activity.CurrentFocus;
                if (view != null)
                {
                    MainActivity.main_activity.CloseKeyboard(view);
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
                flagViewShow();
            }
            SetBtnSearchItem();
        }

        private void BtnBasket_Click(object sender, EventArgs e)
        {
            var fragment = new POS_Dialog_Quantity();
            fragment.Show(Activity.SupportFragmentManager, nameof(POS_Dialog_Quantity));
            addQuantity = 1;
            POS_Dialog_Quantity.SetBackQuantity(addQuantity);
        }

        private void BtnDummy_Click(object sender, EventArgs e)
        {
            var fragment = new POS_Dialog_Dummy();
            fragment.Show(Activity.SupportFragmentManager, nameof(POS_Dialog_Dummy));           
        }

        private void BtnOrder_Click(object sender, EventArgs e)
        {
            var fragment = new Pos_Dialog_Order();            
            fragment.Show(Activity.SupportFragmentManager, nameof(Pos_Dialog_Order));            
        }

        private void BtnBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                bool check = MainActivity.main_activity.CheckPermission();
                if (check)
                {
                    var fragment = new POS_Dialog_Scan() { Cancelable = false };
                    fragment.Show(Activity.SupportFragmentManager, nameof(POS_Dialog_Scan));
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnShowMenu_Click(object sender, EventArgs e)
        {
            if (!flagView)
            {
                btnShowMenu.SetImageResource(Resource.Mipmap.ViewList);
                Preferences.Set("ViewPos", "Grid");
                flagView = true;
            }
            else
            {
                btnShowMenu.SetImageResource(Resource.Mipmap.ViewGroup);
                Preferences.Set("ViewPos", "Row");
                flagView = false;
            }
            flagViewShow();
        }

        List<MenuTabwithSysCategory> MenuTab { get; set; }
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
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
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

                #region Old code
                //listFavorite = new List<Item>();
                //List<Item> lst = new List<Item>();

                //if (DefaultDataItem != null || DefaultDataTopping != null)
                //{
                //    //Tab Favorite
                //    if (DefaultDataItem.Count > 0 || DefaultDataTopping.Count > 0)
                //    {
                //        lst.AddRange(DefaultDataItem);
                //        lst.AddRange(DefaultDataTopping);
                //    }
                //    listFavorite = lst.Where(x => x.FavoriteNo > 0).ToList();
                //    if (listFavorite.Count == 0)
                //    {
                //        MenuTab.Remove(MenuTab.Where(x => x.NameMenuEn == "Favorite").FirstOrDefault());
                //    }

                //    //Tab Topping               
                //    List<Item> listitemTopping = new List<Item>();
                //    listitemTopping = DefaultDataTopping.Where(x => x.SaleItemType == 'T').ToList();
                //    if (listitemTopping.Count == 0)
                //    {
                //        MenuTab.Remove(MenuTab.Where(x => x.NameMenuEn == "Extra Topping").FirstOrDefault());
                //    }
                //}

                //lstCategory = new List<Category>();
                //lstCategory = MainActivity.allData.DefaultDataCategory;

                ////หมวดหมู่ที่ไม่มีสินค้า
                //List<Category> lstCatenoItem = new List<Category>();
                //lstCatenoItem = await CategoryManage.GetAllCategoryhaveitem();
                ////หมวดหมู่ที่ไม่มีสินค้าจะไม่แสดง
                //lstCategory = lstCategory.Where(x => lstCatenoItem.Select(y => (long)y.SysCategoryID).ToList().Contains(x.SysCategoryID)).ToList();

                //if (lstCategory != null)
                //{
                //    foreach (var category in lstCategory)
                //    {
                //        MenuTab.Add(new Model.MenuTabwithSysCategory { NameMenuEn = category.Name, NameMenuTh = category.Name, SysCategory = category.SysCategoryID });
                //    }
                //}
                //listCategory = new ListCategory(lstCategory);
                //if (MenuTab.Count <= 1)
                //{
                //    rcvCategoryItem.Visibility = ViewStates.Gone;
                //}
                //else
                //{
                //    rcvCategoryItem.Visibility = ViewStates.Visible;
                //} 
                #endregion

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
                rcvCategoryItem.Visibility = MenuTab.Count <= 1 ? ViewStates.Gone : ViewStates.Visible;

                LinearLayoutManager HeaderLayoutManager = new LinearLayoutManager(this.Context, LinearLayoutManager.Horizontal, false);
                rcvCategoryItem.HasFixedSize = true;
                rcvCategoryItem.SetLayoutManager(HeaderLayoutManager);
                Pos_Adapter_Header pos_Adapter_Header = new Pos_Adapter_Header(MenuTab);
                rcvCategoryItem.SetItemViewCacheSize(30);
                rcvCategoryItem.SetAdapter(pos_Adapter_Header);
                rcvCategoryItem.SetItemAnimator(null);
                pos_Adapter_Header.ItemClick += Pos_Adapter_Header_ItemClick1;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetTabShowMenu at Item");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SetClearSearchText()
        {
            SearchItem = "";
            txtSearchPos.Text = string.Empty;
            SetBtnSearchItem();
        }
        private void SetBtnSearchItem()
        {
            if (string.IsNullOrEmpty(SearchItem))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.Search);
                btnSearch.Enabled = false;
            }
            else
            {
                //btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
                btnSearch.Enabled = true;
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
                        listItemBody = DefaultDataItem.Where(x => x.SaleItemType != 'T' && x.DataStatus != 'D').ToList();
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

                //ข้อมูลเมื่อทำการค้นหา
                if (!string.IsNullOrEmpty(SearchItem))
                {
                    listItemBody = SearchItemfromCategory();
                }

                flagViewShow();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowDetail at POS");
                return;
            }
        }
        public static bool flagView = false; // falgView = false = row,flagView = true = grid
        public void flagViewShow()
        {
            try
            {
                // falgView = false = row,flagView = true = grid
                if (listItemBody != null)
                {
                    var addNewItem = listItemBody.Where(x => x.SysItemID == 0).FirstOrDefault();
                    if (addNewItem == null && fillter != -2) // -2 Favorite จะไม่แสดงปุ่มเพิ่มสินค้าใหม่ที่ Favorite
                    {
                        listItemBody.Add(new Item { SysItemID = 0 });
                    }

                    if (flagView == false)
                    {
                        btnShowMenu.SetImageResource(Resource.Mipmap.ViewGroup);
                        listItem = new ListItem(listItemBody);
                        mLayoutManager = new LinearLayoutManager(this.Context, LinearLayoutManager.Vertical, false);
                        pos_Adapter_Row = new Pos_Adapter_Row(listItem, checkNet);
                        rcvItem.HasFixedSize = true;

                        rcvItem.SetLayoutManager(mLayoutManager);
                        rcvItem.SetItemViewCacheSize(100);
                        rcvItem.SetAdapter(pos_Adapter_Row);
                        pos_Adapter_Row.ItemClick += Pos_Adapter_Row_ItemClick;
                    }
                    else
                    {
                        btnShowMenu.SetImageResource(Resource.Mipmap.ViewList);
                        listItem = new ListItem(listItemBody);
                        pos_Adapter_Grid = new Pos_Adapter_Grid(listItem, checkNet);
                        gridLayoutManager = new GridLayoutManager(this.Context, 4);
                        gridLayoutManager.Orientation = RecyclerView.Vertical;

                        rcvItem.HasFixedSize = true;
                        rcvItem.SetLayoutManager(gridLayoutManager);
                        rcvItem.AddItemDecoration(new SpacesItemDecoration(2));
                        rcvItem.SetItemViewCacheSize(100);
                        rcvItem.SetAdapter(pos_Adapter_Grid);
                        pos_Adapter_Grid.ItemClick += Pos_Adapter_Grid_ItemClick;
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("flagViewShow at POS");
            }
        }
        ImageView imageViewRow;
        public static TextView ItemName;
        public static TranDetailItemWithTopping tranDetailItemWithTopping;
        Color colorAnimetion;
        public static POS_Dialog_Option fragment_option;
        public static POS_Dialog_AddTopping pos_dialog_addtopping;
        public static POS_Dialog_AddItem pos_dialog_additem;

        private async void Pos_Adapter_Row_ItemClick(object sender, int e)
        {
            try
            {
                if (listItem[e].SysItemID == 0)
                {
                    AddNewItem();
                }
                else
                {
                    int positionClick = e;
                    view = mLayoutManager.FindViewByPosition(positionClick);
                    ImageShow = view.FindViewById<ImageView>(Resource.Id.imgShowAnime);
                    imageViewRow = view.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
                    ItemName = view.FindViewById<TextView>(Resource.Id.txtNameItem);

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

                        if (fragment_option != null)
                        {
                            return;
                        }
                        fragment_option = new POS_Dialog_Option();
                        fragment_option.Cancelable = false;
                        fragment_option.Show(Activity.SupportFragmentManager, nameof(POS_Dialog_Option));
                        POS_Dialog_Option.SetDataItemfromPOS(listItem[e]);
                        POS_Dialog_Option.PositionClick = e;                        
                        int indexSelect = tranWithDetails.tranDetailItemWithToppings.Count;
                        indexSelect++;
                        view.Enabled = true;
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

                        string Quantity = string.Empty;
                        string itemName = string.Empty;
                        var Item = MainActivity.tranWithDetails.tranDetailItemWithToppings.ToList().Where(x => x.tranDetailItem.SysItemID == listItem[e].SysItemID).FirstOrDefault();
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
                        redSpannable.SetSpan(new Android.Text.Style.ForegroundColorSpan(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null)), 0, Quantity.Length, 0);
                        builder.Append(redSpannable);

                        Android.Text.SpannableString Spannable = new Android.Text.SpannableString(itemName);
                        Spannable.SetSpan(new Android.Text.Style.ForegroundColorSpan(Application.Context.Resources.GetColor(Resource.Color.eclipse, null)), 0, itemName.Length, 0);
                        builder.Append(Spannable);

                        ItemName.SetText(builder, TextView.BufferType.Spannable);

                        //string conColor = Utils.SetBackground((int)listItem[e].Colors);
                        //colorAnimetion = Android.Graphics.Color.ParseColor(conColor);
                        SelectItemtoCart(e);
                        view.Enabled = true;
                    }                   
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
        public static void SelectItemtoCart(int positionClick)
        {
            try
            {
                if (listItem[positionClick].SysItemID == 0)
                {
                    return;
                }

                var row = MainActivity.tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == listItem[positionClick].SysItemID);
                if (row != -1)
                {

                }
                tranWithDetails =  MainActivity.tranWithDetails;
                tranWithDetails = BLTrans.ChooseItemTran(tranWithDetails, tranDetailItemWithTopping);
                DataCashing.ModifyTranOrder = true;
                tranWithDetails = BLTrans.Caltran(tranWithDetails);

                DataCashing.setQuantityToCart = 1;
                setQuantity = 1;
                txtQuantity.Text = "x"  + DataCashing.setQuantityToCart.ToString();
                btnBasket.SetBackgroundResource(Resource.Drawable.borderqauntity);
                MainActivity.tranWithDetails = tranWithDetails;
                POS_Fragment_Cart.fragment_cart.OnResume();

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectItemtoCart at POS");
            }
        }

        private async void Pos_Adapter_Grid_ItemClick(object sender, int e)
        {
            try
            {
                if (listItem[e].SysItemID == 0)
                {
                    view.Enabled = false;
                    AddNewItem();
                    view.Enabled = true;
                }
                else
                {
                    int positionClick = e;
                    view = gridLayoutManager.FindViewByPosition(positionClick);
                    ImageShow = view.FindViewById<ImageView>(Resource.Id.imgShowAnime);
                    imageViewRow = view.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
                    ItemName = view.FindViewById<TextView>(Resource.Id.txtNameItem);

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

                        if (fragment_option != null)
                        {
                            return;
                        }
                        fragment_option = new POS_Dialog_Option();
                        fragment_option.Cancelable = false;
                        fragment_option.Show(Activity.SupportFragmentManager, nameof(POS_Dialog_Option));
                        POS_Dialog_Option.SetDataItemfromPOS(listItem[e]);
                        POS_Dialog_Option.PositionClick = e;
                        int indexSelect = tranWithDetails.tranDetailItemWithToppings.Count;
                        indexSelect++;
                        view.Enabled = true;
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

                        var Item = MainActivity.tranWithDetails.tranDetailItemWithToppings.ToList().Where(x => x.tranDetailItem.SysItemID == listItem[e].SysItemID).FirstOrDefault();
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


                        //string conColor = Utils.SetBackground((int)listItem[e].Colors);
                        //colorAnimetion = Android.Graphics.Color.ParseColor(conColor);

                        //AnimetionStartGrid();
                        SelectItemtoCart(e);
                        view.Enabled = true;
                    }
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
        private async void Pos_Adapter_Header_ItemClick1(object sender, int e)
        {
            try
            {
                Pos_Adapter_Header.vhselect.NotSelect();
                var z = rcvCategoryItem.FindViewHolderForAdapterPosition(e) as ListViewCategoryHolder;
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
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Item_Adapter_Header_ItemClick at Item");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        public void AddNewItem()
        {
            try
            {
                if (nameCategory == "ท็อปปิ้ง" | nameCategory == "Extra Topping")
                {
                    if (pos_dialog_addtopping != null)
                    {
                        return;
                    }
                    pos_dialog_addtopping = new  POS_Dialog_AddTopping();
                    pos_dialog_addtopping.Cancelable = false;
                    pos_dialog_addtopping.Show(MainActivity.main_activity.SupportFragmentManager, nameof(pos_dialog_addtopping));
                }
                else
                {
                    if (pos_dialog_additem != null)
                    {
                        return;
                    }
                    pos_dialog_additem = new POS_Dialog_AddItem();
                    pos_dialog_additem.Cancelable = false;
                    pos_dialog_additem.Show(MainActivity.main_activity.SupportFragmentManager, nameof(pos_dialog_additem));
                    long Category = 0;
                    if (fillter == 0)
                    {
                        Category = 0;
                    }
                    else
                    {
                        Category = fillter;
                    }                    
                    POS_Dialog_AddItem.SetCategoryFromPOS(Category);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("AddNewItem at POS");
            }
        }

        public  List<Item> SearchItemfromCategory()
        {
            try
            {
                listItemBody = new List<Item>();
                int merchantId = DataCashingAll.MerchantId;
                if (fillter == 0)
                {
                    listItemBody = DefaultDataItem.Where(x => x.MerchantID == merchantId && (x.ItemName.ToLower().Contains(SearchItem.ToLower()) || !string.IsNullOrEmpty(x.ItemCode) && x.ItemCode.ToLower().Contains(SearchItem.ToLower()))).OrderBy(x => x.ItemName).ToList();
                }
                else if (fillter == -2)
                {
                    listItemBody = DefaultDataItem.Where(x => x.MerchantID == merchantId && (x.ItemName.ToLower().Contains(SearchItem.ToLower()) || !string.IsNullOrEmpty(x.ItemCode) && x.ItemCode.ToLower().Contains(SearchItem.ToLower())) & x.FavoriteNo > 0).OrderBy(x => x.ItemName).ToLis​t();
                }
                else if (fillter == -3)
                {
                    listItemBody = DefaultDataTopping.Where(x => x.MerchantID == merchantId && x.ItemName.ToLower().Contains(SearchItem.ToLower())).OrderBy(x => x.ItemName).ToList();
                }
                else
                {
                    listItemBody = DefaultDataItem.Where(x => x.MerchantID == merchantId && x.SysCategoryID == fillter && (x.ItemName.ToLower().Contains(SearchItem.ToLower()) || !string.IsNullOrEmpty(x.ItemCode) && x.ItemCode.ToLower().Contains(SearchItem.ToLower()))).OrderBy(x => x.ItemName).ToList();
                }

                if (listItemBody == null)
                {
                    listItemBody = new List<Item>();
                    Toast.MakeText(this.Activity, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();                    
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

        private async void CheckJwt()
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
    }

   

    internal class SpacesItemDecoration : RecyclerView.ItemDecoration
    {
        private int space;

        public SpacesItemDecoration(int _space)
        {
            this.space = _space;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            base.GetItemOffsets(outRect, view, parent, state);
            //outRect.Top = space;
            //outRect.Bottom = space;
            //= outRect.Left = outRect.Right = v;

        }
    }
}