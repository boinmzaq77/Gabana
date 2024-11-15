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
using Firebase.Messaging;
using Gabana.Droid.Tablet.Adapter.Items;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
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

namespace Gabana.Droid.Tablet.Fragments.Items
{
    public class Item_Fragment_Main : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Item_Fragment_Main NewInstance()
        {
            Item_Fragment_Main frag = new Item_Fragment_Main();
            return frag;
        }
        public static Item_Fragment_Main fragment_main;
        View view;
        public static bool checkNet = false;
        string usernamelogin, LoginType;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.item_fragment_main, container, false);
            try
            {
                fragment_main = this;
                tabSelected = "Item";
                usernamelogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
                CombineUI();
                SetUIEvent();
                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "item");
                if (check)
                {
                    additem.SetBackgroundResource(Resource.Mipmap.Add);
                    addCategory.SetBackgroundResource(Resource.Mipmap.Add);
                    addTopping.SetBackgroundResource(Resource.Mipmap.Add);
                }
                else
                {
                    additem.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addCategory.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addTopping.SetBackgroundResource(Resource.Mipmap.AddMax);
                }
                swipTabitem.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagItemChange = true;
                    DataCashingAll.flagItemOnBranchChange = true;
                    DataCashingAll.flagCategoryChange = true;
                    //refresh Online Data                   

                    if (!await GabanaAPI.CheckNetWork())
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }
                    else if (!await GabanaAPI.CheckSpeedConnection())
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    }
                    else
                    {
                        await MainActivity.main_activity.GetOnlineDataCategory();
                        await MainActivity.main_activity.GetOnlineDataitem();
                        await MainActivity.main_activity.GetOnlineDataItemonBranch();
                        //เรียกการโหลดที่หน้าหลักใหม่ เพื่ออัปเดตเป็นค่าล่าสุด
                        await MainActivity.main_activity.GetAllData();
                        POS_Fragment_Main.fragment_main.OnResume();   
                        OnResume();
                    }
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };
                swipTabCategory.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagItemChange = true;
                    DataCashingAll.flagItemOnBranchChange = true;
                    DataCashingAll.flagCategoryChange = true;
                    //refresh Online Data

                    if (!await GabanaAPI.CheckNetWork())
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }
                    else if (!await GabanaAPI.CheckSpeedConnection())
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    }
                    else
                    {
                        await MainActivity.main_activity.GetOnlineDataCategory();
                        await MainActivity.main_activity.GetOnlineDataitem();
                        await MainActivity.main_activity.GetOnlineDataItemonBranch();
                        //เรียกการโหลดที่หน้าหลักใหม่ เพื่ออัปเดตเป็นค่าล่าสุด
                        await MainActivity.main_activity.GetAllData();
                        POS_Fragment_Main.fragment_main.OnResume();
                        OnResume();
                    }
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };
                swipTopping.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagItemChange = true;
                    DataCashingAll.flagItemOnBranchChange = true;
                    DataCashingAll.flagCategoryChange = true;

                    if (!await GabanaAPI.CheckNetWork())
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }
                    else if (!await GabanaAPI.CheckSpeedConnection())
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    }
                    else
                    {
                        await MainActivity.main_activity.GetOnlineDataCategory();
                        await MainActivity.main_activity.GetOnlineDataitem();
                        await MainActivity.main_activity.GetOnlineDataItemonBranch();
                        //เรียกการโหลดที่หน้าหลักใหม่ เพื่ออัปเดตเป็นค่าล่าสุด
                        await MainActivity.main_activity.GetAllData();
                        POS_Fragment_Main.fragment_main.OnResume();
                        OnResume();
                    }

                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };
                swipTabStock.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagItemChange = true;
                    DataCashingAll.flagItemOnBranchChange = true;
                    DataCashingAll.flagCategoryChange = true;
                    //refresh Online Data                    

                    if (!await GabanaAPI.CheckNetWork())
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }
                    else if (!await GabanaAPI.CheckSpeedConnection())
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    }
                    else
                    {
                        await MainActivity.main_activity.GetOnlineDataCategory();
                        await MainActivity.main_activity.GetOnlineDataitem();
                        await MainActivity.main_activity.GetOnlineDataItemonBranch();
                        //เรียกการโหลดที่หน้าหลักใหม่ เพื่ออัปเดตเป็นค่าล่าสุด
                        await MainActivity.main_activity.GetAllData();
                        POS_Fragment_Main.fragment_main.OnResume();
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

        private void SetUIEvent()
        {
            additem.Click += Additem_Click;
            addCategory.Click += Addcategory_Click;
            addTopping.Click += AddTopping_Click;
            btnSearchItemBarcode.Click += BtnSearchItemBarcode_Click;

            btnSearchItem.Click += BtnSearchItem_Click;
            textSearchItem.TextChanged += TextSearchItem_TextChanged;
            textSearchItem.KeyPress += TextSearchItem_KeyPress;
            textSearchItem.FocusChange += TextSearchItem_FocusChange;

            btnSearchTopping.Click += BtnSearchTopping_Click;
            textSearchTopping.TextChanged += TextSearchTopping_TextChanged;
            textSearchTopping.KeyPress += TextSearchTopping_KeyPress;
            textSearchTopping.FocusChange += TextSearchTopping_FocusChange;    

            btnSearchCategory.Click += BtnSearchCategory_Click;
            textSearchCategory.TextChanged += TextSearchCategory_TextChanged;
            textSearchCategory.KeyPress += TextSearchCategory_KeyPress;
            textSearchCategory.FocusChange += TextSearchCategory_FocusChange;

            btnSearchStock.Click += BtnSearchStock_Click;
            textSearchStock.TextChanged += TextSearchStock_TextChanged;
            textSearchStock.KeyPress += TextSearchStock_KeyPress;
            textSearchStock.FocusChange += TextSearchStock_FocusChange;
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

            MainActivity.main_activity.CloseKeyboard(view);

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

        private async void TextSearchStock_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchStock = textSearchStock.Text.Trim();
            if (string.IsNullOrEmpty(searchStock))
            {
                await SetItemOnStock();
            }
            SetBtnSearch(btnSearchStock, searchStock);
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

        private void TextSearchCategory_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterCategoryData();
            }

            //View view = this.CurrentFocus;
            //if (view != null)
            //{
            //    if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
            //    {
            //        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            //        imm.HideSoftInputFromWindow(view.WindowToken, 0);
            //    }
            //}

            MainActivity.main_activity.CloseKeyboard(view);

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

        private void TextSearchTopping_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchTopping = textSearchTopping.Text.Trim();
            if (string.IsNullOrEmpty(searchTopping))
            {
                SetExtraData();
            }
            SetBtnSearch(btnSearchTopping, searchTopping);
        }

        private void TextSearchTopping_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterExtraToppingData();
            }
            //View view = this.CurrentFocus;
            //if (view != null)
            //{
            //    if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
            //    {
            //        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            //        imm.HideSoftInputFromWindow(view.WindowToken, 0);
            //    }
            //}

            MainActivity.main_activity.CloseKeyboard(view);

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

        private void TextSearchItem_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterItemData();
            }

            //View view = this.CurrentFocus;
            //if (view != null)
            //{
            //    if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
            //    {
            //        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            //        imm.HideSoftInputFromWindow(view.WindowToken, 0);
            //    }
            //}

            MainActivity.main_activity.CloseKeyboard(view);

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

        private async void BtnSearchStock_Click(object sender, EventArgs e)
        {
            SetClearSearchStock();
            await SetItemOnStock();
        }

        private void BtnSearchCategory_Click(object sender, EventArgs e)
        {
            SetClearSearchCategory();
            SetCategoryData();
        }

        private void BtnSearchTopping_Click(object sender, EventArgs e)
        {
            SetClearSearchTopping();
            SetExtraData();
        }

        private void BtnSearchItem_Click(object sender, EventArgs e)
        {
            SetClearSearchItem();
            SetItemData();
        }

        private void SetClearSearchItem()
        {
            searchItemCode = "";
            searchItem = "";
            textSearchItem.Text = string.Empty;
            MainActivity.main_activity.CloseKeyboard(view);
        }

        private void SetClearSearchTopping()
        {
            searchTopping = "";
            textSearchTopping.Text = string.Empty;
            SetBtnSearch(btnSearchTopping, searchTopping);
            MainActivity.main_activity.CloseKeyboard(view);
        }

        private void SetClearSearchCategory()
        {
            searchCategory = "";
            textSearchCategory.Text = string.Empty;
            SetBtnSearch(btnSearchCategory, searchCategory);
            MainActivity.main_activity.CloseKeyboard(view);
        }

        private void SetClearSearchStock()
        {
            searchStock = "";
            textSearchStock.Text = string.Empty;
            SetBtnSearch(btnSearchStock, searchStock);
            MainActivity.main_activity.CloseKeyboard(view);
        }

        private async Task SetDetail()
        {
            try
            {
                tabItem.Visibility = ViewStates.Visible;
                tabCategory.Visibility = ViewStates.Gone;
                tabStock.Visibility = ViewStates.Gone;
                tabTopping.Visibility = ViewStates.Gone;

                if (tabSelected == "")
                {
                    tabSelected = "Item";
                }

                CheckJwt();
                SetTabMenu();
                SetTabShowMenu();

                if (string.IsNullOrEmpty(searchItem) && string.IsNullOrEmpty(searchItemCode))
                {
                    items = new List<Item>();
                    items = MainActivity.allData.DefaultDataItem;
                    SetItemData();
                    SetBtnSearch(btnSearchItem, searchItem);
                }
                else
                {
                    items = MainActivity.allData.DefaultDataItem;
                    SetFilterItemData();
                }

                if (string.IsNullOrEmpty(searchTopping))
                {
                    itemExtra = new List<Item>();
                    itemExtra = MainActivity.allData.DefaultDataTopping;
                    SetExtraData();
                    SetBtnSearch(btnSearchTopping, searchTopping);
                }
                else
                {
                    itemExtra = MainActivity.allData.DefaultDataTopping;
                    SetFilterExtraToppingData();
                }

                if (string.IsNullOrEmpty(searchCategory))
                {
                    lstCategory = new List<Category>();
                    lstCategory = MainActivity.allData.DefaultDataCategory;
                    SetCategoryData();
                    SetBtnSearch(btnSearchCategory, searchCategory);
                }
                else
                {
                    lstCategory = MainActivity.allData.DefaultDataCategory;
                    SetFilterCategoryData();
                }

                if (string.IsNullOrEmpty(searchStock))
                {
                    itemsStock = new List<Item>();
                    itemsStock = MainActivity.allData.DefaultDataItemonBranch;
                    await SetItemOnStock();
                    SetBtnSearch(btnSearchStock, searchStock);
                }
                else
                {
                    itemsStock = new List<Item>();
                    itemsStock = MainActivity.allData.DefaultDataItemonBranch;
                    await SetItemOnStock();
                    SetFilterItemOnStock();
                }

                allitemCategory = new List<Item>();
                if (items != null)
                {
                    allitemCategory.AddRange(items);
                }

                if (itemExtra != null)
                {
                    allitemCategory.AddRange(itemExtra);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("SetDetail at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void Additem_Click(object sender, EventArgs e)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "item");
            if (!check)
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                return;
            }
            DataCashing.flagChooseMedia = false;
            DataCashing.EditItem = null;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "additem");
        }
        private void Addcategory_Click(object sender, EventArgs e)
        {
            string Role = LoginType;
            bool check = UtilsAll.CheckPermissionRoleUser(Role, "insert", "category");
            if (!check)
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                return;
            }
            DataCashing.EditCategory = null;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "addcategory");
        }
        private void AddTopping_Click(object sender, EventArgs e)
        {
            string Role = LoginType;
            bool check = UtilsAll.CheckPermissionRoleUser(Role, "insert", "topping");
            if (!check)
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                return;               
            }
            DataCashing.flagChooseMedia = false;
            DataCashing.EditTopping = null;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "addtopping");
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }
        private void BtnSearchItemBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                bool check = MainActivity.main_activity.CheckPermission();
                if (check)
                {
                    Item_Dialog_Scan dialog = Item_Dialog_Scan.NewInstance("item");
                    var fragment = new Item_Dialog_Scan() { Cancelable = false };
                    fragment.Show(Activity.SupportFragmentManager, nameof(Item_Dialog_Scan));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnScanItem_Click at Item Dialog AddItem");
            }
        }

        public static void SetItemCode(string itemCode)
        {
            textSearchItem.Text = itemCode;
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            swipTabitem.Refreshing = false;
            swipTabCategory.Refreshing = false;
            swipTopping.Refreshing = false;
            swipTabStock.Refreshing = false;
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

                var z = rcvListItem.FindViewHolderForAdapterPosition(index) as ListViewItemHolder;
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.editsucess), ToastLength.Short).Show();
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("LnFavoriteClick at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public override async void OnResume()
        {
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

                checkNet = DataCashing.CheckNet;
                await SetDetail();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("OnResume at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        RecyclerView rcvTabMenu, rcvListItem, rcvListTopping, rcvListStock, rcvListCategory;
        LinearLayout tabItem, tabTopping, tabStock, tabCategory;
        SwipeRefreshLayout swipTabitem, swipTopping, swipTabStock, swipTabCategory;
        FrameLayout lnSearchItem, framItem, lnSearchTopping, framTopping, lnSearchStock, framStock;
        FrameLayout lnSearchCategory, framCategory;
        static ImageButton btnSearchItem, btnSearchTopping, btnSearchCategory, btnSearchStock;
        static EditText textSearchItem, textSearchTopping, textSearchStock, textSearchCategory;
        ImageButton btnSearchItemBarcode, additem, addTopping, btnScollToTop, addCategory;
        LinearLayout lnNoItem, lnNoDataSearchItem, lnNoTopping, lnNoDataSearchTopping, lnNoStock, lnNoDataSearchStock;
        LinearLayout lnNoCategory, lnNoDataSearchCategory;
        TextView txtStockRevision;
        private void CombineUI()
        {
            rcvTabMenu = view.FindViewById<RecyclerView>(Resource.Id.rcvTabMenu);

            tabItem = view.FindViewById<LinearLayout>(Resource.Id.tabItem);
            swipTabitem = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipTabitem);
            lnSearchItem = view.FindViewById<FrameLayout>(Resource.Id.lnSearchItem);
            btnSearchItem = view.FindViewById<ImageButton>(Resource.Id.btnSearchItem);
            textSearchItem = view.FindViewById<EditText>(Resource.Id.textSearchItem);
            btnSearchItemBarcode = view.FindViewById<ImageButton>(Resource.Id.btnSearchItemBarcode);
            framItem = view.FindViewById<FrameLayout>(Resource.Id.framItem);
            rcvListItem = view.FindViewById<RecyclerView>(Resource.Id.rcvListItem);
            lnNoItem = view.FindViewById<LinearLayout>(Resource.Id.lnNoItem);
            lnNoDataSearchItem = view.FindViewById<LinearLayout>(Resource.Id.lnNoDataSearchItem);
            additem = view.FindViewById<ImageButton>(Resource.Id.additem);

            tabTopping = view.FindViewById<LinearLayout>(Resource.Id.tabTopping);
            swipTopping = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipTabTopping);
            lnSearchTopping = view.FindViewById<FrameLayout>(Resource.Id.lnSearchTopping);
            btnSearchTopping = view.FindViewById<ImageButton>(Resource.Id.btnSearchTopping);
            textSearchTopping = view.FindViewById<EditText>(Resource.Id.textSearchTopping);
            framTopping = view.FindViewById<FrameLayout>(Resource.Id.framTopping);
            rcvListTopping = view.FindViewById<RecyclerView>(Resource.Id.rcvListTopping);
            lnNoTopping = view.FindViewById<LinearLayout>(Resource.Id.lnNoTopping);
            lnNoDataSearchTopping = view.FindViewById<LinearLayout>(Resource.Id.lnNoDataSearchTopping);
            addTopping = view.FindViewById<ImageButton>(Resource.Id.addTopping);

            tabStock = view.FindViewById<LinearLayout>(Resource.Id.tabStock);
            swipTabStock = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipTabStock);
            lnSearchStock = view.FindViewById<FrameLayout>(Resource.Id.lnSearchStock);
            btnSearchStock = view.FindViewById<ImageButton>(Resource.Id.btnSearchStock);
            textSearchStock = view.FindViewById<EditText>(Resource.Id.textSearchStock);
            txtStockRevision = view.FindViewById<TextView>(Resource.Id.txtStockRevision);
            framStock = view.FindViewById<FrameLayout>(Resource.Id.framStock);
            rcvListStock = view.FindViewById<RecyclerView>(Resource.Id.rcvListStock);
            lnNoStock = view.FindViewById<LinearLayout>(Resource.Id.lnNoStock);
            lnNoDataSearchStock = view.FindViewById<LinearLayout>(Resource.Id.lnNoDataSearchStock);
            btnScollToTop = view.FindViewById<ImageButton>(Resource.Id.btnScollToTop);

            tabCategory = view.FindViewById<LinearLayout>(Resource.Id.tabCategory);
            swipTabCategory = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipTabCategory);
            lnSearchCategory = view.FindViewById<FrameLayout>(Resource.Id.lnSearchCategory);
            btnSearchCategory = view.FindViewById<ImageButton>(Resource.Id.btnSearchCategory);
            textSearchCategory = view.FindViewById<EditText>(Resource.Id.textSearchCategory);
            framCategory = view.FindViewById<FrameLayout>(Resource.Id.framCategory);
            rcvListCategory = view.FindViewById<RecyclerView>(Resource.Id.rcvListCategory);
            lnNoCategory = view.FindViewById<LinearLayout>(Resource.Id.lnNoCategory);
            lnNoDataSearchCategory = view.FindViewById<LinearLayout>(Resource.Id.lnNoDataSearchCategory);
            addCategory = view.FindViewById<ImageButton>(Resource.Id.addCategory);
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
        public static string tabSelected = "";
        List<MenuTab> MenuTab { get; set; }
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void SetTabShowMenu()
        {
            try
            {
                if (tabSelected == "")
                {
                    tabSelected = "Item";
                }

                GridLayoutManager menuLayoutManager = new GridLayoutManager(this.Activity, 4, 1, false);
                //recyclerHeaderItem.HasFixedSize = true;
                rcvTabMenu.HasFixedSize = false;
                rcvTabMenu.SetLayoutManager(menuLayoutManager);
                Item_Adapter_Menu item_adapter_menu = new Item_Adapter_Menu(MenuTab);
                rcvTabMenu.SetAdapter(item_adapter_menu);
                item_adapter_menu.ItemClick += Item_Adapter_Menu_ItemClick;

                tabItem.Visibility = ViewStates.Gone;
                tabTopping.Visibility = ViewStates.Gone;
                tabStock.Visibility = ViewStates.Gone;
                tabCategory.Visibility = ViewStates.Gone;

                View view = this.Activity.CurrentFocus;
                if (view != null)
                {

                }

                switch (tabSelected)
                {
                    case "Item":
                        tabItem.Visibility = ViewStates.Visible;
                        break;
                    case "Extra Topping":
                        tabTopping.Visibility = ViewStates.Visible;
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
                    default:
                        tabItem.Visibility = ViewStates.Gone;
                        tabCategory.Visibility = ViewStates.Gone;
                        tabTopping.Visibility = ViewStates.Gone;
                        tabStock.Visibility = ViewStates.Gone;
                        break;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetTabShowMenu at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        long LastRevisionNoStock = 0;
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
        string searchItem, searchCategory, searchStock, searchTopping;
        string searchItemCode;

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
                                await MainActivity.main_activity.GetOnlineDataCategory();
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
                                await MainActivity.main_activity.GetOnlineDataitem();
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
                                await MainActivity.main_activity.GetOnlineDataItemonBranch();
                                DataCashingAll.flagItemChange = true;
                                DataCashingAll.flagItemOnBranchChange = true;
                                DataCashingAll.flagCategoryChange = true;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(searchItem) && string.IsNullOrEmpty(searchItemCode))
                    {
                        items = MainActivity.allData.DefaultDataItem;
                        SetItemData();
                        SetBtnSearch(btnSearchItem, searchItem);
                    }
                    else
                    {
                        items = MainActivity.allData.DefaultDataItem;
                        SetFilterItemData();
                    }

                    if (string.IsNullOrEmpty(searchTopping))
                    {
                        itemExtra = MainActivity.allData.DefaultDataTopping;
                        SetExtraData();
                        SetBtnSearch(btnSearchTopping, searchTopping);
                    }
                    else
                    {
                        itemExtra = MainActivity.allData.DefaultDataTopping;
                        SetFilterExtraToppingData();
                    }
                    
                    if (string.IsNullOrEmpty(searchCategory))
                    {
                        lstCategory = MainActivity.allData.DefaultDataCategory;
                        SetCategoryData();
                        SetBtnSearch(btnSearchCategory, searchCategory);
                    }
                    else
                    {
                        lstCategory = MainActivity.allData.DefaultDataCategory;
                        SetFilterCategoryData();
                    }
                    
                    if (string.IsNullOrEmpty(searchStock))
                    {
                        itemsStock = MainActivity.allData.DefaultDataItemonBranch;
                        await SetItemOnStock();
                        SetBtnSearch(btnSearchStock, searchStock);
                    }
                    else
                    {
                        itemsStock = MainActivity.allData.DefaultDataItemonBranch;
                        await SetItemOnStock();
                        SetFilterItemOnStock();
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckRevisionStock at Item");
            }
        }

        private async void SetFilterItemOnStock()
        {
            try
            {
                lstsearchitemsStock = new List<Item>();
                lstsearchitemsStock = itemsStock.Where(m => (m.ItemName.ToLower().Contains(searchStock.ToLower()) || (m.ItemCode != null && m.ItemCode.Contains(searchStock.ToLower())))).ToList();
                if (lstsearchitemsStock.Count > 0)
                {
                    lstsearchitemsStock = lstsearchitemsStock.OrderBy(x => x.ItemName).ToList();
                }

                listItemStock = new ListItem(lstsearchitemsStock);
                item_adapter_stock = new Item_Adapter_Stock(listItemStock, itemOnBranch, checkNet);
                gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListStock.SetLayoutManager(gridLayoutItem);
                //recyclerviewlistStock.HasFixedSize = true;
                rcvListStock.HasFixedSize = false;
                int count = items == null ? 0 : items.Count + 1;
                rcvListStock.SetItemViewCacheSize(count);
                rcvListStock.SetAdapter(item_adapter_stock);
                item_adapter_stock.ItemClick += Item_adapter_stock_ItemClick;
                SetBtnSearch(btnSearchStock, searchStock);

                rcvListStock.ScrollChange += RcvListStock_ScrollChange;
                btnScollToTop.Click += BtnScollToTop_Click;

                if (item_adapter_stock.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchStock))
                    {
                        lnNoDataSearchStock.Visibility = ViewStates.Visible;
                        lnNoStock.Visibility = ViewStates.Gone;
                        lnSearchStock.Visibility = ViewStates.Visible;
                        rcvListStock.Visibility = ViewStates.Gone;

                    }
                    else
                    {
                        lnNoDataSearchStock.Visibility = ViewStates.Gone;
                        lnNoStock.Visibility = ViewStates.Visible;
                        lnSearchStock.Visibility = ViewStates.Gone;
                        rcvListStock.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchStock.Visibility = ViewStates.Gone;
                    lnNoStock.Visibility = ViewStates.Gone;
                    lnSearchStock.Visibility = ViewStates.Visible;
                    rcvListStock.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetFilterItemData at Stock");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnScollToTop_Click(object sender, EventArgs e)
        {
            rcvListStock.ScrollToPosition(0);
        }

        private void RcvListStock_ScrollChange(object sender, View.ScrollChangeEventArgs e)
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

        private void Item_adapter_stock_ItemClick(object sender, int e)
        {
            var SaleItemType = listItemStock[e].SaleItemType;
            if (SaleItemType == 'T')
            {
                var Topping = listItemStock[e];
                DataCashing.EditTopping = Topping;
                Item_Fragment_AddTopping.tabSelected = "Stock";
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "addtopping");
            }
            else
            {
                DataCashing.EditItem = listItemStock[e];
                Item_Fragment_AddItem.tabSelected = "Stock";
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "additem");
            }
        }

        Item_Adapter_Stock item_adapter_stock;
        async Task SetItemOnStock()
        {
            try
            {
                listItemStock = new ListItem(itemsStock);
                ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
                itemOnBranch = await onBranchManage.GetAllItemOnBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                item_adapter_stock = new Item_Adapter_Stock(listItemStock, itemOnBranch, checkNet);
                gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListStock.SetLayoutManager(gridLayoutItem);
                //recyclerviewlistStock.HasFixedSize = true;
                rcvListStock.HasFixedSize = false;
                int count = items == null ? 0 : items.Count + 1;
                rcvListStock.SetItemViewCacheSize(count);
                rcvListStock.SetAdapter(item_adapter_stock);
                item_adapter_stock.ItemClick += Item_adapter_stock_ItemClick;
                rcvListStock.ScrollChange += RcvListStock_ScrollChange;

                if (item_adapter_stock.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchStock))
                    {
                        lnNoDataSearchStock.Visibility = ViewStates.Visible;
                        lnNoStock.Visibility = ViewStates.Gone;
                        lnSearchStock.Visibility = ViewStates.Gone;
                        rcvListStock.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearchStock.Visibility = ViewStates.Gone;
                        lnNoStock.Visibility = ViewStates.Visible;
                        lnSearchStock.Visibility = ViewStates.Gone;
                        rcvListStock.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchStock.Visibility = ViewStates.Gone;
                    lnNoStock.Visibility = ViewStates.Gone;
                    lnSearchStock.Visibility = ViewStates.Visible;
                    rcvListStock.Visibility = ViewStates.Visible;
                    btnScollToTop.Click += BtnScollToTop_Click;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetItemStockData at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        
        private async void SetFilterCategoryData()
        {
            try
            {
                lstsearchcategory = new List<Category>();
                lstsearchcategory = lstCategory.Where(m => m.Name.ToLower().Contains(searchCategory.ToLower())).ToList();
                if (lstsearchcategory.Count > 0)
                {
                    lstsearchcategory = lstsearchcategory.OrderBy(x => x.Name).ToList();
                }

                listCategory = new ListCategory(lstsearchcategory);
                mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListCategory.HasFixedSize = false;
                rcvListCategory.SetLayoutManager(mLayoutManager);
                item_adapter_category = new Item_Adapter_Category(listCategory, allitemCategory);
                int count = allitemCategory == null ? 0 : allitemCategory.Count + 1;
                rcvListCategory.SetItemViewCacheSize(count);
                rcvListCategory.SetAdapter(item_adapter_category);
                item_adapter_category.ItemClick += Item_adapter_category_ItemClick;
                SetBtnSearch(btnSearchCategory, searchCategory);

                if (item_adapter_category.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchCategory))
                    {
                        lnNoDataSearchCategory.Visibility = ViewStates.Visible;
                        lnNoCategory.Visibility = ViewStates.Gone;
                        lnSearchCategory.Visibility = ViewStates.Visible;
                        rcvListCategory.Visibility = ViewStates.Gone;

                    }
                    else
                    {
                        lnNoDataSearchCategory.Visibility = ViewStates.Gone;
                        lnNoCategory.Visibility = ViewStates.Visible;
                        lnSearchCategory.Visibility = ViewStates.Gone;
                        rcvListCategory.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchCategory.Visibility = ViewStates.Gone;
                    lnNoCategory.Visibility = ViewStates.Gone;
                    lnSearchCategory.Visibility = ViewStates.Visible;
                    rcvListCategory.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetFilterCategoryData at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        static ListCategory listCategory;       
        GridLayoutManager mLayoutManager, menuLayoutManager;
        Item_Adapter_Category item_adapter_category;
        async void SetCategoryData()
        {
            try
            {
                listCategory = new ListCategory(lstCategory);
                mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListCategory.HasFixedSize = false;
                rcvListCategory.SetLayoutManager(mLayoutManager);
                item_adapter_category = new Item_Adapter_Category(listCategory, allitemCategory);
                int count = allitemCategory == null ? 0 : allitemCategory.Count + 1;
                rcvListCategory.SetItemViewCacheSize(count);
                rcvListCategory.SetAdapter(item_adapter_category);
                item_adapter_category.ItemClick += Item_adapter_category_ItemClick;

                if (item_adapter_category.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchCategory))
                    {
                        lnNoDataSearchCategory.Visibility = ViewStates.Visible;
                        lnNoCategory.Visibility = ViewStates.Gone;
                        lnSearchCategory.Visibility = ViewStates.Gone;
                        rcvListCategory.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearchCategory.Visibility = ViewStates.Gone;
                        lnNoCategory.Visibility = ViewStates.Visible;
                        lnSearchCategory.Visibility = ViewStates.Gone;
                        rcvListCategory.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchCategory.Visibility = ViewStates.Gone;
                    lnNoCategory.Visibility = ViewStates.Gone;
                    lnSearchCategory.Visibility = ViewStates.Visible;
                    rcvListCategory.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetCategoryData at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void Item_adapter_category_ItemClick(object sender, int e)
        {
            try
            {
                if (Item_Fragment_AddCategory.flagdatachange == true)
                {
                    if (DataCashing.EditCategory == null)
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.add_dialog_back.ToString();
                        Add_Dialog_Back.SetPage("category");
                        dialog.Arguments = bundle;
                        dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                        return;
                    }
                    else
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        Edit_Dialog_Back.SetPage("category");
                        String myMessage = Resource.Layout.edit_dialog_back.ToString();
                        bundle.PutString("message", myMessage);
                        dialog.Arguments = bundle;
                        dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                        return;
                    }
                }
                string Role = LoginType;
                bool check = UtilsAll.CheckPermissionRoleUser(Role, "view", "category");
                if (!check)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                    return;
                }

                var category = listCategory[e];
                DataCashing.EditCategory = category;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "addcategory");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        public static List<Item> items, itemExtra, itemsStock , allitemCategory;        
        public static List<Item> lstsearchitems, lstsearchitemExtra, lstsearchitemsStock;
        public static ListItem listItem, listExtraItem, listItemStock;

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
                var z = rcvListTopping.FindViewHolderForAdapterPosition(index) as ListViewItemHolder;
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.editsucess), ToastLength.Short).Show();
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("LnToppingFavoriteClick at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }

        }
        Item_Adapter_Item item_adapter_item;
        public static GridLayoutManager gridLayoutItem;
        private async void SetFilterItemData()
        {
            try
            {
                lstsearchitems = new List<Item>();
                //items ข้อมูลชุดเดิมที่มาจาก onresume                
                lstsearchitems = items.Where(m => (m.ItemName.ToLower().Contains(searchItem.ToLower())
                                         || (m.ItemCode != null && m.ItemCode.ToLower().Contains(searchItem.ToLower())))).ToList();
                if (lstsearchitems.Count > 0)
                {
                    lstsearchitems = lstsearchitems.OrderBy(x => x.ItemName).ToList();
                }

                listItem = new ListItem(lstsearchitems);
                item_adapter_item = new Item_Adapter_Item(listItem, checkNet);
                gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListItem.SetLayoutManager(gridLayoutItem);
                rcvListItem.HasFixedSize = false;
                int count = items == null ? 0 : items.Count + 1;
                rcvListItem.SetItemViewCacheSize(count);
                rcvListItem.SetAdapter(item_adapter_item);
                item_adapter_item.ItemClick += Item_adapter_item_ItemClick;
                SetBtnSearch(btnSearchItem, searchItem);

                if (item_adapter_item.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchItem))
                    {
                        lnNoDataSearchItem.Visibility = ViewStates.Visible;
                        lnNoItem.Visibility = ViewStates.Gone;
                        lnSearchItem.Visibility = ViewStates.Visible;
                        rcvListItem.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearchItem.Visibility = ViewStates.Gone;
                        lnNoItem.Visibility = ViewStates.Visible;
                        lnSearchItem.Visibility = ViewStates.Gone;
                        rcvListItem.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchItem.Visibility = ViewStates.Gone;
                    lnNoItem.Visibility = ViewStates.Gone;
                    lnSearchItem.Visibility = ViewStates.Visible;
                    rcvListItem.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetFilterItemData at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async void Item_adapter_item_ItemClick(object sender, int e)
        {
            try
            {
                if (Item_Fragment_AddItem.flagdatachange == true)
                {
                    if (DataCashing.EditItem == null)
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.add_dialog_back.ToString();
                        Add_Dialog_Back.SetPage("item");
                        dialog.Arguments = bundle;
                        dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                        return;
                    }
                    else
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        Edit_Dialog_Back.SetPage("item");
                        String myMessage = Resource.Layout.edit_dialog_back.ToString();
                        bundle.PutString("message", myMessage);
                        dialog.Arguments = bundle;
                        dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                        return;
                    }
                }

                var id = listItem[e].SysItemID;
                DataCashing.EditItem = listItem[e];
                DataCashing.flagChooseMedia = false;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "additem");
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("Item_Adapter_Item_ItemClick at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        public static List<Category> lstCategory, lstsearchcategory;
        Item_Adapter_Topping item_adapter_topping;
        public static GridLayoutManager gridLayoutItemExtra;
        private async void SetFilterExtraToppingData()
        {
            try
            {
                lstsearchitemExtra = new List<Item>();
                lstsearchitemExtra = itemExtra.Where(m => m.ItemName.ToLower().Contains(searchTopping.ToLower())).ToList();
                if (lstsearchitemExtra.Count > 0)
                {
                    lstsearchitemExtra = lstsearchitemExtra.OrderBy(x => x.ItemName).ToList();
                }

                listExtraItem = new ListItem(lstsearchitemExtra);
                item_adapter_topping = new Item_Adapter_Topping(listExtraItem, checkNet);
                gridLayoutItemExtra = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListTopping.SetLayoutManager(gridLayoutItemExtra);
                //recyclerviewlistTopping.HasFixedSize = true;
                rcvListTopping.HasFixedSize = false;
                int count = itemExtra == null ? 0 : itemExtra.Count + 1;
                rcvListTopping.SetItemViewCacheSize(count);
                rcvListTopping.SetAdapter(item_adapter_topping);
                item_adapter_topping.ItemClick += Item_adapter_topping_ItemClick; 
                SetBtnSearch(btnSearchTopping, searchTopping);

                if (item_adapter_topping.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchTopping))
                    {
                        lnNoDataSearchTopping.Visibility = ViewStates.Visible;
                        lnNoTopping.Visibility = ViewStates.Gone;
                        lnSearchTopping.Visibility = ViewStates.Visible;
                        rcvListTopping.Visibility = ViewStates.Gone;

                    }
                    else
                    {
                        lnNoDataSearchTopping.Visibility = ViewStates.Gone;
                        lnNoTopping.Visibility = ViewStates.Visible;
                        lnSearchTopping.Visibility = ViewStates.Gone;
                        rcvListTopping.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchTopping.Visibility = ViewStates.Gone;
                    lnNoTopping.Visibility = ViewStates.Gone;
                    lnSearchTopping.Visibility = ViewStates.Visible;
                    rcvListTopping.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetFilterExtraToppingData at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async void Item_adapter_topping_ItemClick(object sender, int e)
        {
            try
            {
                if (Item_Fragment_AddTopping.flagdatachange == true)
                {
                    if (DataCashing.EditTopping == null)
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.add_dialog_back.ToString();
                        Add_Dialog_Back.SetPage("topping");
                        dialog.Arguments = bundle;
                        dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                        return;
                    }
                    else
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        Edit_Dialog_Back.SetPage("topping");
                        String myMessage = Resource.Layout.edit_dialog_back.ToString();
                        bundle.PutString("message", myMessage);
                        dialog.Arguments = bundle;
                        dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                        return;
                    }
                }

                var Topping = listExtraItem[e];
                DataCashing.EditTopping = Topping;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "addtopping");
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("item_Adapter_Extra_ItemClick at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void SetItemData()
        {
            try
            {
                listItem = new ListItem(items);
                item_adapter_item = new Item_Adapter_Item(listItem, checkNet);
                gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListItem.SetLayoutManager(gridLayoutItem);
                //rcvListItem.HasFixedSize = true;
                rcvListItem.HasFixedSize = false;
                int count = items == null ? 0 : items.Count + 1;
                rcvListItem.SetItemViewCacheSize(count);
                rcvListItem.SetAdapter(item_adapter_item);
                item_adapter_item.ItemClick += Item_adapter_item_ItemClick;

                if (item_adapter_item.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchItem))
                    {
                        lnNoDataSearchItem.Visibility = ViewStates.Visible;
                        lnNoItem.Visibility = ViewStates.Gone;
                        lnSearchItem.Visibility = ViewStates.Gone;
                        rcvListItem.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearchItem.Visibility = ViewStates.Gone;
                        lnNoItem.Visibility = ViewStates.Visible;
                        lnSearchItem.Visibility = ViewStates.Gone;
                        rcvListItem.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchItem.Visibility = ViewStates.Gone;
                    lnNoItem.Visibility = ViewStates.Gone;
                    lnSearchItem.Visibility = ViewStates.Visible;
                    rcvListItem.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetItemData at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SetExtraData()
        {
            try
            {
                int index = -1;
                index = itemExtra.FindIndex(x => x.SysItemID == 0);
                if (index > -1)
                {
                    itemExtra.RemoveAt(index);
                }
                listExtraItem = new ListItem(itemExtra);
                item_adapter_topping = new Item_Adapter_Topping(listExtraItem, checkNet);
                gridLayoutItemExtra = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListTopping.SetLayoutManager(gridLayoutItemExtra);
                //recyclerviewlistTopping.HasFixedSize = true;
                rcvListTopping.HasFixedSize = false;
                int count = items == null ? 0 : itemExtra.Count + 1;
                rcvListTopping.SetItemViewCacheSize(count);
                rcvListTopping.SetAdapter(item_adapter_topping);

                //Click เพื่อ Update ข้อมูล
                item_adapter_topping.ItemClick += Item_adapter_topping_ItemClick;

                if (item_adapter_topping.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchTopping))
                    {
                        lnNoDataSearchTopping.Visibility = ViewStates.Visible;
                        lnNoTopping.Visibility = ViewStates.Gone;
                        lnSearchTopping.Visibility = ViewStates.Gone;
                        rcvListTopping.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearchTopping.Visibility = ViewStates.Gone;
                        lnNoTopping.Visibility = ViewStates.Visible;
                        lnSearchTopping.Visibility = ViewStates.Gone;
                        rcvListTopping.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearchTopping.Visibility = ViewStates.Gone;
                    lnNoTopping.Visibility = ViewStates.Gone;
                    lnSearchTopping.Visibility = ViewStates.Visible;
                    rcvListTopping.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetExtra at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        private List<ORM.MerchantDB.ItemOnBranch> itemOnBranch;
        ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
        int maxItemRevision = 0;
        ItemExSizeManage itemExSizeManage = new ItemExSizeManage();
        ItemManage itemManage = new ItemManage();
        CategoryManage categoryManage = new CategoryManage();
        int maxCategoryRevision = 0;
        Category category = new Category();


        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                Log.Debug("Token", "Token" + " " + res.gbnJWT);
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
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

        private void Item_Adapter_Menu_ItemClick(object sender, int e)
        {
            try
            {
                tabSelected = MenuTab[e].NameMenuEn;
                SetTabShowMenu();
                SetClearSearchItem();                
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Item_Adapter_Header_ItemClick at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public async void ReloadItem(Item NewItem)
        {
            try
            {
                await MainActivity.main_activity.GetAllData();
                items = MainActivity.allData.DefaultDataItem;
                SetItemData();

                int index = 0;
                index = items.FindIndex(x => x.SysItemID == NewItem.SysItemID);
                if (index > -1)
                {
                    lnSearchItem.Visibility = ViewStates.Visible;
                    lnNoItem.Visibility = ViewStates.Gone;
                    rcvListItem.Visibility = ViewStates.Visible;
                    items.RemoveAt(index);
                    items.Insert(0, NewItem);
                    rcvListItem.SmoothScrollToPosition(0);
                    item_adapter_item.NotifyItemInserted(0);
                }

                POS_Fragment_Main.fragment_main.OnResume();

                int indexAll = allitemCategory.FindIndex(x => x.SysItemID == NewItem.SysItemID);
                if (indexAll > -1)
                {
                    allitemCategory[indexAll] = NewItem;
                }
                else
                {
                    allitemCategory.Add(NewItem);
                }

                //Stock
                if (NewItem.FTrackStock == 1)
                {
                    int indexstock = 0;
                    indexstock = itemsStock.FindIndex(x=>x.SysItemID == NewItem.SysItemID);
                    if (indexstock > -1)
                    {
                        itemsStock[indexstock] = NewItem;
                        item_adapter_stock.NotifyItemChanged(indexstock);
                        lnNoStock.Visibility = ViewStates.Gone;
                        rcvListStock.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        itemsStock.Insert(0, NewItem);
                        rcvListStock.SmoothScrollToPosition(0);
                        item_adapter_stock.NotifyItemInserted(0);
                        lnNoStock.Visibility = ViewStates.Visible;
                        lnSearchStock.Visibility = ViewStates.Visible;
                        rcvListStock.Visibility = ViewStates.Gone;
                    }
                    await SetItemOnStock();
                }

                if (NewItem.SysCategoryID != 0)
                {                    
                    SetCategoryData();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ReloadItem at Item_Fragment_Main");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }


        public async Task ReloadTopping(Item NewTopping)
        {
            try
            {                
                await MainActivity.main_activity.GetAllData();
                itemExtra = MainActivity.allData.DefaultDataTopping;
                SetExtraData();

                int index = 0;
                index = itemExtra.FindIndex(x => x.SysItemID == NewTopping.SysItemID);
                if (index > -1)
                {
                    lnNoTopping.Visibility = ViewStates.Gone;
                    lnSearchTopping.Visibility = ViewStates.Visible;
                    rcvListTopping.Visibility = ViewStates.Visible;
                    itemExtra.RemoveAt(index);
                    itemExtra.Insert(0, NewTopping);
                    rcvListTopping.SmoothScrollToPosition(0);
                    item_adapter_topping.NotifyItemInserted(0);
                }

                POS_Fragment_Main.fragment_main.OnResume();

                int indexAll = allitemCategory.FindIndex(x => x.SysItemID == NewTopping.SysItemID);
                if (indexAll > -1)
                {
                    allitemCategory[indexAll] = NewTopping;
                }
                else
                {
                    allitemCategory.Add(NewTopping);
                }

                //Stock
                if (NewTopping.FTrackStock == 1)
                {
                    int indexstock = 0;
                    indexstock = itemsStock.FindIndex(x => x.SysItemID == NewTopping.SysItemID);
                    if (indexstock > -1)
                    {
                        itemsStock[indexstock] = NewTopping;
                        item_adapter_stock.NotifyItemChanged(indexstock);
                        lnNoStock.Visibility = ViewStates.Gone;
                        rcvListStock.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        itemsStock.Insert(0, NewTopping);
                        rcvListStock.SmoothScrollToPosition(0);
                        item_adapter_stock.NotifyItemInserted(0);
                        lnNoStock.Visibility = ViewStates.Visible;
                        rcvListStock.Visibility = ViewStates.Gone;
                    }
                    await SetItemOnStock();
                }

                if (NewTopping.SysCategoryID != 0)
                {
                    SetCategoryData();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ReloadTopping at Item_Fragment_Main");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public async void ReloadCategory(Category NewCategory)
        {
            try
            {
                await MainActivity.main_activity.GetAllData();
                lstCategory = MainActivity.allData.DefaultDataCategory;
                SetCategoryData();

                int index = 0;
                index = lstCategory.FindIndex(x => x.SysCategoryID == NewCategory.SysCategoryID);
                if (index > -1)
                {
                    lnNoCategory.Visibility = ViewStates.Gone;
                    lnSearchCategory.Visibility = ViewStates.Visible;
                    rcvListCategory.Visibility = ViewStates.Visible;
                    lstCategory.RemoveAt(index);
                    lstCategory.Insert(0, NewCategory);
                    rcvListCategory.SmoothScrollToPosition(0);
                    item_adapter_category.NotifyItemInserted(0);
                    SetItemData();
                    SetExtraData();
                    await SetItemOnStock();
                }
                POS_Fragment_Main.fragment_main.OnResume();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ReloadCustomer at Item_Fragment_Main");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public  void DeleteItem(Item _DeleteItem)
        {
            try
            {
                int index = 0;
                index = items.FindIndex(x => x.SysItemID == _DeleteItem.SysItemID);
                if (index == -1)
                {
                    return;
                }

                items.RemoveAt(index);
                allitemCategory.RemoveAt(index);
                item_adapter_item.NotifyItemRemoved(index);
                POS_Fragment_Main.fragment_main.OnResume();

                if (items.Count == 0) 
                {
                    lnNoItem.Visibility = ViewStates.Visible;
                    lnSearchItem.Visibility = ViewStates.Gone;
                    rcvListItem.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoItem.Visibility = ViewStates.Gone;
                    lnSearchItem.Visibility = ViewStates.Visible;
                    rcvListItem.Visibility = ViewStates.Visible;
                }

                Item_Fragment_AddItem.fragment_additem.UINewItem();

                if (_DeleteItem.SysCategoryID != 0)
                {
                    SetCategoryData();
                }

                //remove item from list
                if (_DeleteItem.FTrackStock == 1)
                {
                    index = itemsStock.FindIndex(x => x.SysItemID == _DeleteItem.SysItemID);
                    if (index == -1)
                    {
                        return;
                    }

                    itemsStock.RemoveAt(index);
                    item_adapter_stock.NotifyItemRemoved(index);

                    if (itemsStock.Count == 0)
                    {
                        lnNoStock.Visibility = ViewStates.Visible;
                        rcvListStock.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoStock.Visibility = ViewStates.Gone;
                        rcvListStock.Visibility = ViewStates.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeleteItem at Item_Fragment_Main");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public  void DeleteTopping(Item _DeleteTopping)
        {
            try
            {
                int index = 0;
                index = itemExtra.FindIndex(x => x.SysItemID == _DeleteTopping.SysItemID);
                if (index == -1)
                {
                    return;
                }

                itemExtra.RemoveAt(index);
                allitemCategory.RemoveAt(index);
                item_adapter_topping.NotifyItemRemoved(index);
                POS_Fragment_Main.fragment_main.OnResume();

                if (itemExtra.Count == 0)
                {
                    lnNoTopping.Visibility = ViewStates.Visible;
                    lnSearchTopping.Visibility = ViewStates.Gone;
                    rcvListTopping.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoTopping.Visibility = ViewStates.Gone;
                    lnSearchTopping.Visibility = ViewStates.Visible;
                    rcvListTopping.Visibility = ViewStates.Visible;
                }

                Item_Fragment_AddTopping.fragment_addtopping.UINewItem();

                if (_DeleteTopping.SysCategoryID != 0)
                {
                    SetCategoryData();
                }

                //remove item from list
                if (_DeleteTopping.FTrackStock == 1)
                {
                    index = itemsStock.FindIndex(x => x.SysItemID == _DeleteTopping.SysItemID);
                    if (index == -1)
                    {
                        return;
                    }

                    itemsStock.RemoveAt(index);
                    item_adapter_stock.NotifyItemRemoved(index);

                    if (itemsStock.Count == 0)
                    {
                        lnNoStock.Visibility = ViewStates.Visible;
                        rcvListStock.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoStock.Visibility = ViewStates.Gone;
                        rcvListStock.Visibility = ViewStates.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeleteTopping at Item_Fragment_Main");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void DeleteCategory(Category _DeleteCategory)
        {
            try
            {
                int index = 0;
                index = lstCategory.FindIndex(x => x.SysCategoryID == _DeleteCategory.SysCategoryID);
                if (index == -1)
                {
                    return;
                }

                lstCategory.RemoveAt(index);
                item_adapter_category.NotifyItemRemoved(index);
                POS_Fragment_Main.fragment_main.OnResume();
                Item_Fragment_AddCategory.fragment_addcategory.UINewCategory();

                if (lstCategory.Count == 0)
                {
                    lnNoCategory.Visibility = ViewStates.Visible;
                    lnSearchCategory.Visibility = ViewStates.Gone;
                }
                else
                {
                    rcvListCategory.Visibility = ViewStates.Gone;
                    lnSearchCategory.Visibility = ViewStates.Visible;
                }

                SetCategoryData();
                SetItemData();
                SetExtraData();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeleteCategory at Item_Fragment_Main");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

    }
}