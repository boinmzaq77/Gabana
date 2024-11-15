using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views.InputMethods;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyInsightsLib;
using Android.Content;
using System.Linq;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ReportCategoryActivity : AppCompatActivity
    {
        public static ReportCategoryActivity reportcategoryActivity;
        private static ListCategory listCategory;
        LinearLayout lnBack, lnNoDataSearch;
        RecyclerView mRecycleView;
        private static List<Item> items, itemExtra;
        public static List<Category> lstCategory;
        public static List<Category> listChooseCategory = new List<Category>();
        List<Item> allitem;
        Button btnAll, btnApply;
        public static string categorySelect;

        EditText textSearch;
        string SearchName;
        ImageButton btnSearch;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.report_activity_category);
                reportcategoryActivity = this;

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                CheckJwt();
                mRecycleView = FindViewById<RecyclerView>(Resource.Id.recyclerview_listcategory);
                btnApply = FindViewById<Button>(Resource.Id.btnApply);
                btnApply.Click += BtnApply_Click;
                SetCategoryData();
                btnAll = FindViewById<Button>(Resource.Id.btnAll);
                btnAll.Click += BtnAll_Click;
                SetShowButton();
                btnApply.Enabled = false;
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                lnNoDataSearch = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
                textSearch = FindViewById<EditText>(Resource.Id.textSearch);
                textSearch.TextChanged += TextSearch_TextChanged; ;
                textSearch.KeyPress += TextSearch_KeyPress;
                textSearch.FocusChange += TxtSearch_FocusChange;
                btnSearch = FindViewById<ImageButton>(Resource.Id.btnSearch);

                _ = TinyInsights.TrackPageViewAsync("OnCreate : ReportCategoryActivity");
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at ReportCategory");
                Log.Debug("Error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtSearch_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearch.Text.Trim()))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
        }
        private void TextSearch_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            SetBtnSearch();

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
                textSearch.Text += input;
                textSearch.SetSelection(textSearch.Text.Length);
                return;
            }
        }

        private async void SetFilterCategoryData()
        {
            try
            {
                lstCategory = new List<Category>();
                lstCategory = lstCategory.Where(m => m.Name.ToLower().Contains(SearchName.ToLower())).ToList();
                if (lstCategory.Count > 0)
                {
                    lstCategory = lstCategory.OrderBy(x => x.Name).ToList();
                }

                listCategory = new ListCategory(lstCategory);
                Report_Adapter_Category report_adapter_category = new Report_Adapter_Category(listCategory, allitem, listChooseCategory);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleView.SetAdapter(report_adapter_category);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetLayoutManager(mLayoutManager);
                report_adapter_category.ItemClick += Report_adapter_category_ItemClick;


                if (report_adapter_category.ItemCount == 0 && !string.IsNullOrEmpty(SearchName))
                {
                    mRecycleView.Visibility = Android.Views.ViewStates.Gone;
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                }
                else
                {
                    mRecycleView.Visibility = Android.Views.ViewStates.Visible;
                    lnNoDataSearch.Visibility = ViewStates.Gone;
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
        private void TextSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchName = textSearch.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                SetCategoryData();
            }
            SetBtnSearch();
        }
        private void SetClearSearchText()
        {
            SearchName = "";
            textSearch.Text = string.Empty;
            SetBtnSearch();
        }
        private void SetShowButton()
        {
            if (categorySelect == "All Category")
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }

            if (listChooseCategory.Count > 0)
            {
                btnApply.Enabled = true;
                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.Enabled = false;
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
        }
        private void SetBtnSearch()
        {
            if (string.IsNullOrEmpty(SearchName))
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
        private void BtnAll_Click(object sender, EventArgs e)
        {

            if (categorySelect != "All Category" && categorySelect == "")
            {
                categorySelect = "All Category";
                listChooseCategory = new List<Category>();
                listChooseCategory = lstCategory;
            }
            else
            {
                listChooseCategory = new List<Category>();
                categorySelect = "";
            }
            SetShowButton();
            listCategory = new ListCategory(lstCategory);
            Report_Adapter_Category report_adapter_category = new Report_Adapter_Category(listCategory, allitem, listChooseCategory);
            GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
            mRecycleView.SetAdapter(report_adapter_category);
            mRecycleView.HasFixedSize = true;
            mRecycleView.SetLayoutManager(mLayoutManager);
            report_adapter_category.ItemClick += Report_adapter_category_ItemClick;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (categorySelect != string.Empty)
            {
                ReportDailySaleActivity.listChooseCategory = listChooseCategory;
                this.Finish();
            }
            else
            {
                Toast.MakeText(this, "กรุณาเลือกหมวดหมู่สินค้า", ToastLength.Short).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        async void SetCategoryData()
        {
            try
            {
                allitem = new List<Item>();

                List<Item> items = await GetItemList();
                List<Item> itemExtra = await GetExtraList();

                allitem.AddRange(items);
                allitem.AddRange(itemExtra);

                CategoryManage category = new CategoryManage();
                lstCategory = await category.GetAllCategory();
                listCategory = new ListCategory(lstCategory);
                //listChooseCategory = lstCategory;
                Report_Adapter_Category report_adapter_category = new Report_Adapter_Category(listCategory, allitem, listChooseCategory);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleView.SetAdapter(report_adapter_category);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetLayoutManager(mLayoutManager);
                report_adapter_category.ItemClick += Report_adapter_category_ItemClick;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetCategoryData at ReportCategory");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async void Report_adapter_category_ItemClick(object sender, int e)
        {
            try
            {
                var category = listCategory[e];
                var search = listChooseCategory.FindIndex(x => x.SysCategoryID == category.SysCategoryID && x.MerchantID == DataCashingAll.MerchantId);
                if (search == -1)
                {
                    listChooseCategory.Add(category);
                }
                else
                {
                    listChooseCategory.RemoveAt(search);
                }

                categorySelect = "";

                listCategory = new ListCategory(lstCategory);

                if (listCategory.Count == listChooseCategory.Count)
                {
                    categorySelect = "All Category";
                }
                else
                {
                    foreach (var item in listChooseCategory)
                    {
                        if (categorySelect != "")
                        {
                            categorySelect += "," + item.Name;
                        }
                        else
                        {
                            categorySelect = item.Name;
                        }
                    }
                }
                SetShowButton();
                CategoryManage categoryManage = new CategoryManage();
                lstCategory = await categoryManage.GetAllCategory();
                listCategory = new ListCategory(lstCategory);

                Report_Adapter_Category report_adapter_category = new Report_Adapter_Category(listCategory, allitem, listChooseCategory);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleView.SetAdapter(report_adapter_category);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetLayoutManager(mLayoutManager);
                report_adapter_category.ItemClick += Report_adapter_category_ItemClick;

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Report_adapter_category_ItemClick at ReportCategory");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        async Task<List<Item>> GetItemList()
        {
            try
            {
                items = new List<Item>();
                ItemManage itemManage = new ItemManage();
                items = await itemManage.GetAllItem();
                if (items == null)
                {
                    Toast.MakeText(this, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
                    return null;
                }
                Log.Debug("Insert", JsonConvert.SerializeObject(items));
                return items;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetItemList at ReportCategory");
                Console.WriteLine(ex.Message);
                Log.Debug("error", ex.Message);
                return null;
            }
        }
        async Task<List<Item>> GetExtraList()
        {
            try
            {
                itemExtra = new List<Item>();
                ItemManage itemManage = new ItemManage();
                itemExtra = await itemManage.GetToppingItem();
                if (itemExtra == null)
                {
                    Toast.MakeText(this, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
                    return null;
                }

                Log.Debug("Insert", JsonConvert.SerializeObject(itemExtra));
                return itemExtra;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetExtraList at ReportCategory");
                Console.WriteLine(ex.Message);
                Log.Debug("error", ex.Message);
                return null;
            }
        }

        internal static void SetSelectCategory(List<Category> l, string t)
        {
            listChooseCategory = l;
            categorySelect = t;
        }

        protected override void OnResume()
        {
            base.OnResume();
            CheckJwt();
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'ReportCategoryActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'ReportCategoryActivity.openPage' is assigned but its value is never used
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

