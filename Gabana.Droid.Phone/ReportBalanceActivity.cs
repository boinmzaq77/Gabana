//using Android.App;
//using Android.OS;
//using Android.Support.V4.Widget;
//using Android.Support.V7.Widget;
//using Android.Util;
//using Android.Views;
//using Android.Widget;
//using Gabana.Droid.Adapter;
//using Gabana.Droid.ListData;
//using Gabana.Model;
//using Gabana.ORM.MerchantDB;
//using Gabana.ShareSource;
//using Gabana.ShareSource.Manage;
//using Microcharts;
//using Microcharts.Droid;
//using Newtonsoft.Json;
//using SkiaSharp;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Globalization;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using TinyInsightsLib;
//using Xamarin.Essentials;

//namespace Gabana.Droid
//{
//        [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait ,Exported = false)]
//    public class ReportBalanceActivity : Activity
//    {

//        internal static string branchID;
//        TextView txtTitle, txtBranchName;
//        private ORM.MerchantDB.Branch branch;
//        RecyclerView recyclerHeader, recyclerItem;
//        private string tabSelected;
//        private static List<Item> items, itemExtra;
//        List<Item> allitem;
//        static ListItem listItem, listExtraItem;
//        private static List<Category> lstCategory;
//        private static ListCategory listCategory;
//        int listItemBodyCount;
//        private static List<Item> listChooseItem = new List<Item>();
//        private static List<Item> listChooseItemExtra = new List<Item>();
//        public static List<Category> listChooseCategory = new List<Category>();

//        Button btnAll, btnApply;
//        private bool selcetitemall;
//        private string categorySelect;
//        private string extraSelect;
//        private string itemSelect;

//        List<Model.MenuTab> MenuTab { get; set; }

//        protected async override void OnCreate(Bundle savedInstanceState)
//        {
//            try
//            {
//                base.OnCreate(savedInstanceState);
//                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
//                SetContentView(Resource.Layout.report_activity_balance);

//                LinearLayout lnChooseBranch = FindViewById<LinearLayout>(Resource.Id.lnChooseBranch);
//                lnChooseBranch.Click += LnChooseBranch_Click;

//                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
//                lnBack.Click += LnBack_Click;
//                txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);

//                txtBranchName = FindViewById<TextView>(Resource.Id.txtBranchName);
//                branchID = ReportActivity.branchID;

//                btnAll = FindViewById<Button>(Resource.Id.btnAll);
//                btnAll.Click += BtnAll_Click;

//                btnApply = FindViewById<Button>(Resource.Id.btnApply);
//                btnApply.Click += BtnApply_Click;

//                recyclerHeader = FindViewById<RecyclerView>(Resource.Id.recyclerHeader);
//                recyclerItem = FindViewById<RecyclerView>(Resource.Id.recyclerItem);
//                tabSelected = "Item";
//                selcetitemall = false;
//                GetNameBranch();
//                SetTabMenu();
//                SetTabShowMenu();
//                SetDataItem();
//                SetShowButton();

//            }
//            catch (Exception ex)
//            {
//                await TinyInsights.TrackErrorAsync(ex);
//                _ = TinyInsights.TrackPageViewAsync("at ReportBalance");
//                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
//                return;
//            }
//        }

//        private void BtnApply_Click(object sender, EventArgs e)
//        {

//            switch (tabSelected)
//            {
//                case "Item":
//                    if (listChooseItem.Count == 0) return;
//                    List<int> lstsysitem = new List<int>();
//                    foreach (var item in listChooseItem)
//                    {
//                        lstsysitem.Add((int)item.SysItemID);
//                    }
//                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ShowReportDailySaleActivity)));
//                    ShowReportDailySaleActivity.SetBalanceReport("ReportBalance","I", listChooseBranch, lstsysitem, itemSelect,"i");
//                    break;
//                case "Extra Topping":
//                    if (listChooseItemExtra.Count == 0) return;
//                    List<int> lstsysextra = new List<int>();
//                    foreach (var item in listChooseItemExtra)
//                    {
//                        lstsysextra.Add((int)item.SysItemID);
//                    }
//                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ShowReportDailySaleActivity)));
//                    ShowReportDailySaleActivity.SetBalanceReport("ReportBalance","I", listChooseBranch, lstsysextra,extraSelect,"e");
//                    break;
//                case "Category":
//                    if (listChooseCategory.Count == 0) return;
//                    List<int> lstsyscategory = new List<int>();
//                    foreach (var item in listChooseCategory)
//                    {
//                        lstsyscategory.Add((int)item.SysCategoryID);
//                    }
//                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ShowReportDailySaleActivity)));
//                    ShowReportDailySaleActivity.SetBalanceReport("ReportBalance","C", listChooseBranch, lstsyscategory,categorySelect,"c");
//                    break;
//                default:
//                    break;
//            }

//        }

//        private void BtnAll_Click(object sender, EventArgs e)
//        {
//            if (!selcetitemall)
//            {
//                selcetitemall = true;
//                switch (tabSelected)
//                {
//                    case "Item":
//                        listChooseItem = new List<Item>();
//                        listChooseItem = items;
//                        itemSelect = "All Items";
//                        break;
//                    case "Extra Topping":
//                        listChooseItemExtra = new List<Item>();
//                        listChooseItemExtra = itemExtra;
//                        extraSelect = "All Extra Topping";
//                        break;
//                    case "Category":
//                        listChooseCategory = new List<Category>();
//                        listChooseCategory = lstCategory;
//                        categorySelect = "All Category";
//                        break;
//                    default:
//                        break;
//                }

//            }
//            else
//            {
//                itemSelect = "";
//                extraSelect = "";
//                categorySelect = "";

//                listChooseItem = new List<Item>();
//                listChooseItemExtra = new List<Item>();
//                listChooseCategory = new List<Category>();
//                selcetitemall = false;
//            }
//            SetDataItem();
//            SetShowButton();

//        }
//        private void SetShowButton()
//        {
//            if (!selcetitemall)
//            {
//                btnAll.SetBackgroundResource(Resource.Drawable.btnborderblue);
//                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
//                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
//                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
//            }
//            else
//            {
//                btnAll.SetBackgroundResource(Resource.Drawable.btnblue);
//                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
//                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
//                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
//            }
//        }
//        private void SetDataItem()
//        {
//            switch (tabSelected)
//            {
//                case "Item":
//                    SetListItem();
//                    break;
//                case "Extra Topping":
//                    SetListExtra();
//                    break;
//                case "Category":
//                    SetListCategory();
//                    break;
//                default:
//                    break;
//            }
//            SetShowButton();

//        }

//        private async void SetListCategory()
//        {
//            try
//            {
//                allitem = new List<Item>();

//                List<Item> items = await GetItemList();
//                List<Item> itemExtra = await GetExtraList();

//                allitem.AddRange(items);
//                allitem.AddRange(itemExtra);

//                CategoryManage category = new CategoryManage();
//                lstCategory = await category.GetAllCategory();
//                listCategory = new ListCategory(lstCategory);


//                Report_Adapter_Category report_adapter_category = new Report_Adapter_Category(listCategory, allitem ,listChooseCategory);
//                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
//                recyclerItem.SetAdapter(report_adapter_category);
//                recyclerItem.HasFixedSize = true;
//                recyclerItem.SetLayoutManager(mLayoutManager);
//                report_adapter_category.ItemClick += Report_adapter_category_ItemClick;



//            }
//            catch (Exception ex)
//            {
//                await TinyInsights.TrackErrorAsync(ex);
//                _ = TinyInsights.TrackPageViewAsync("SetListCategory at ReportBalance");
//                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
//                return;
//            }
//        }

//        private async void Report_adapter_category_ItemClick(object sender, int e)
//        {
//            try
//            {
//                var category = listCategory[e];
//                var search = listChooseCategory.FindIndex(x => x.SysCategoryID == category.SysCategoryID && x.MerchantID == DataCashingAll.MerchantId);
//                if (search == -1)
//                {
//                    listChooseCategory.Add(category);
//                }
//                else
//                {
//                    listChooseCategory.RemoveAt(search);
//                }
//                SetListCategory();

//                categorySelect = "";
//                if (listCategory.Count == listChooseCategory.Count)
//                {
//                    categorySelect = "All Category";
//                }
//                else
//                {
//                    foreach (var item in listChooseCategory)
//                    {
//                        if (categorySelect != "")
//                        {
//                            categorySelect += "," + item.Name;
//                        }
//                        else
//                        {
//                            categorySelect = item.Name;
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                await TinyInsights.TrackErrorAsync(ex);
//                _ = TinyInsights.TrackPageViewAsync("Report_adapter_category_ItemClick at ReportBalance");
//                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
//                return;
//            }
//        }

//        private async void SetListExtra()
//        {
//            try
//            {
//                itemExtra = new List<Item>();
//                itemExtra = await GetExtraList();
//                listExtraItem = new ListItem(itemExtra);
//                Report_Adapter_ChooseItem report_aAdapter_chooseItemExtra = new Report_Adapter_ChooseItem(listExtraItem, listChooseItemExtra);
//                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
//                recyclerItem.SetLayoutManager(gridLayoutItem);
//                recyclerItem.HasFixedSize = true;
//                recyclerItem.SetItemViewCacheSize(20);
//                recyclerItem.SetAdapter(report_aAdapter_chooseItemExtra);

//                //Click เพื่อ Update ข้อมูล
//                report_aAdapter_chooseItemExtra.ItemClick += Report_aAdapter_chooseItemExtra_ItemClick;


//            }
//            catch (Exception ex)
//            {
//                await TinyInsights.TrackErrorAsync(ex);
//                _ = TinyInsights.TrackPageViewAsync("SetListExtra at ReportBalance");
//                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
//                return;
//            }
//        }

//        private void Report_aAdapter_chooseItemExtra_ItemClick(object sender, int e)
//        {

//            var extraItem = listExtraItem[e];

//            var search = listChooseItemExtra.FindIndex(x => x.SysItemID == extraItem.SysItemID);
//            if (search == -1)
//            {
//                listChooseItemExtra.Add(extraItem);
//            }
//            else
//            {
//                listChooseItemExtra.RemoveAt(search);
//            }

//            SetListExtra();

//            extraSelect = "";
//            if (listExtraItem.Count == listChooseItemExtra.Count)
//            {
//                extraSelect = "All Extra Topping";
//            }
//            else
//            {
//                foreach (var item in listChooseItemExtra)
//                {
//                    if (extraSelect != "")
//                    {
//                        extraSelect += "," + item.ItemName;
//                    }
//                    else
//                    {
//                        extraSelect = item.ItemName;
//                    }
//                }
//            }
//        }

//        async Task<List<Item>> GetExtraList()
//        {
//            try
//            {
//                itemExtra = new List<Item>();
//                ItemManage itemManage = new ItemManage();
//                itemExtra = await itemManage.GetToppingItem();
//                if (itemExtra == null)
//                {
//                    Toast.MakeText(this, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
//                    return null;
//                }

//                listItemBodyCount = itemExtra.Count();
//                Log.Debug("Insert", JsonConvert.SerializeObject(itemExtra));
//                return itemExtra;
//            }
//            catch (Exception ex)
//            {
//                await TinyInsights.TrackErrorAsync(ex);
//                _ = TinyInsights.TrackPageViewAsync("GetExtraList at ReportBalance");
//                Console.WriteLine(ex.Message);
//                Log.Debug("error", ex.Message);
//                return null;
//            }
//        }
//        private async void SetListItem()
//        {
//            try
//            {
//                items = new List<Item>();
//                items = await GetItemList();
//                listItem = new ListItem(items);
//                Report_Adapter_ChooseItem report_aAdapter_chooseItem = new Report_Adapter_ChooseItem(listItem, listChooseItem);
//                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
//                recyclerItem.SetLayoutManager(gridLayoutItem);
//                recyclerItem.HasFixedSize = true;
//                recyclerItem.SetItemViewCacheSize(20);
//                recyclerItem.SetAdapter(report_aAdapter_chooseItem);
//                report_aAdapter_chooseItem.ItemClick += Report_Adapter_ChooseItem_ItemClick;
//            }
//            catch (Exception ex)
//            {

//                await TinyInsights.TrackErrorAsync(ex);
//                _ = TinyInsights.TrackPageViewAsync("SetListItem at ReportBalance");
//                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
//                return;
//            }

//        }

//        private void Report_Adapter_ChooseItem_ItemClick(object sender, int e)
//        {
//            var item = listItem[e];

//            var search = listChooseItem.FindIndex(x => x.SysItemID == item.SysItemID);
//            if (search == -1)
//            {
//                listChooseItem.Add(item);
//            }
//            else
//            {
//                listChooseItem.RemoveAt(search);
//            }

//            SetListItem();

//            itemSelect = "";
//            if (listItem.Count == listChooseItem.Count)
//            {
//                itemSelect = "All Items";
//            }
//            else
//            {
//                foreach (var i in listChooseItem)
//                {
//                    if (itemSelect != "")
//                    {
//                        itemSelect += "," + i.ItemName;
//                    }
//                    else
//                    {
//                        itemSelect = i.ItemName;
//                    }
//                }
//            }
//        }

//        async Task<List<Item>> GetItemList()
//        {
//            try
//            {
//                items = new List<Item>();
//                ItemManage itemManage = new ItemManage();
//                items = await itemManage.GetAllItem();
//                if (items == null)
//                {
//                    Toast.MakeText(this, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
//                    return null;
//                }

//                listItemBodyCount = items.Count();
//                Log.Debug("Insert", JsonConvert.SerializeObject(items));
//                return items;
//            }
//            catch (Exception ex)
//            {
//                await TinyInsights.TrackErrorAsync(ex);
//                _ = TinyInsights.TrackPageViewAsync("GetItemList at ReportBalance");
//                Console.WriteLine(ex.Message);
//                Log.Debug("error", ex.Message);
//                return null;
//            }
//        }
//        private void LnChooseBranch_Click(object sender, EventArgs e)
//        {
//            //StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportBranchActivity)));
//            //ReportBranchActivity.SetBranch(branchID);
//        }
//        public override void OnBackPressed()
//        {
//            base.OnBackPressed();
//        }
//        private void LnBack_Click(object sender, EventArgs e)
//        {
//            base.OnBackPressed();
//        }
//        protected override void OnResume()
//        {
//            base.OnResume();
//            GetNameBranch();
//        }

//        private async void GetNameBranch()
//        {
//            BranchManage branchManage = new BranchManage();
//            var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
//            branch = lstBranch.Where(x => x.BranchID == branchID).FirstOrDefault();
//            txtBranchName.Text = branch.BranchName?.ToString();
//        }
//        private void SetTabMenu()
//        {
//            MenuTab = new List<Model.MenuTab>
//            {
//                new Model.MenuTab() { NameMenuEn = "Item" , NameMenuTh = "" },
//                new Model.MenuTab() { NameMenuEn = "Extra Topping" , NameMenuTh = "" },
//                new Model.MenuTab() { NameMenuEn = "Category" , NameMenuTh = "" }
//            };
//        }
//        private void SetTabShowMenu()
//        {
//            if (tabSelected == "")
//            {
//                tabSelected = "Item";
//            }

//            GridLayoutManager menuLayoutManager = new GridLayoutManager(this, 3, 1, false);
//            recyclerHeader.HasFixedSize = true;
//            recyclerHeader.SetLayoutManager(menuLayoutManager);
//            Report_Adapter_Header report_adapter_header = new Report_Adapter_Header(MenuTab, tabSelected);
//            recyclerHeader.SetAdapter(report_adapter_header);
//            report_adapter_header.ItemClick += Report_adapter_header_ItemClick;
//        }

//        private void Report_adapter_header_ItemClick(object sender, int e)
//        {
//            tabSelected = MenuTab[e].NameMenuEn;

//            listChooseItem = new List<Item>();
//            listChooseItemExtra = new List<Item>();
//            listChooseCategory = new List<Category>();
//            selcetitemall = false;

//            SetTabShowMenu();
//            SetDataItem();
//            SetShowButton();

//        }
//    }
//}

