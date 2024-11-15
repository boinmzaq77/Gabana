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
using Gabana.Droid.Tablet.Fragments.Report;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Report_Dialog_Category : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }
        public static Report_Dialog_Category NewInstance()
        {
            var frag = new Report_Dialog_Category { Arguments = new Bundle() };
            return frag;
        }
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.report_dialog_category, container, false);
            try
            {
                CombinUI();
                SetUIEvent();
                SetCategoryData();
                SetShowButton();

                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Report Dialog ChooseCategory");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }

        private void SetUIEvent()
        {
            lnBack.Click += LnBack_Click;
            btnApply.Click += BtnApply_Click;
            btnAll.Click += BtnAll_Click;

            textSearch.TextChanged += TextSearch_TextChanged; ;
            textSearch.KeyPress += TextSearch_KeyPress;
            textSearch.FocusChange += TxtSearch_FocusChange;

        }
        internal static void SetSelectCategory(List<Category> l, string t)
        {
            listChooseCategory = l;
            categorySelect = t;
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
            View view = this.Activity.CurrentFocus;
            if (view != null)
            {
                if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
                {
                    MainActivity.main_activity.CloseKeyboard(view);
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
                GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListcategory.SetAdapter(report_adapter_category);
                rcvListcategory.HasFixedSize = true;
                rcvListcategory.SetLayoutManager(mLayoutManager);
                report_adapter_category.ItemClick += Report_adapter_category_ItemClick;


                if (report_adapter_category.ItemCount == 0 && !string.IsNullOrEmpty(SearchName))
                {
                    rcvListcategory.Visibility = Android.Views.ViewStates.Gone;
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                }
                else
                {
                    rcvListcategory.Visibility = Android.Views.ViewStates.Visible;
                    lnNoDataSearch.Visibility = ViewStates.Gone;
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
        string SearchName;
        private void TextSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchName = textSearch.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                SetCategoryData();
            }
            SetBtnSearch();
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
            GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
            rcvListcategory.SetAdapter(report_adapter_category);
            rcvListcategory.HasFixedSize = true;
            rcvListcategory.SetLayoutManager(mLayoutManager);
            report_adapter_category.ItemClick += Report_adapter_category_ItemClick;
        }
        private async void BtnApply_Click(object sender, EventArgs e)
        {
            if (categorySelect != string.Empty)
            {
                Report_Dialog_Custom.listChooseCategory = listChooseCategory;
                await Report_Dialog_Custom.dialog_Custom.SetDataCategory();
                this.Dialog.Dismiss();
            }
            else
            {
                Toast.MakeText(this.Activity, "กรุณาเลือกหมวดหมู่สินค้า", ToastLength.Short).Show();
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }
        List<Item> allitem = new List<Item>();
        public static List<Category> lstCategory;
        private static ListCategory listCategory;
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
                GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListcategory.SetAdapter(report_adapter_category);
                rcvListcategory.HasFixedSize = true;
                rcvListcategory.SetLayoutManager(mLayoutManager);
                report_adapter_category.ItemClick += Report_adapter_category_ItemClick;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetCategoryData at ReportCategory");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListcategory.SetAdapter(report_adapter_category);
                rcvListcategory.HasFixedSize = true;
                rcvListcategory.SetLayoutManager(mLayoutManager);
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
        private static List<Item> items, itemExtra;
        async Task<List<Item>> GetItemList()
        {
            try
            {
                items = new List<Item>();
                ItemManage itemManage = new ItemManage();
                items = await itemManage.GetAllItem();
                if (items == null)
                {
                    Toast.MakeText(this.Activity, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
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
                    Toast.MakeText(this.Activity, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
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

        public static string categorySelect;
        public static List<Category> listChooseCategory = new List<Category>();
        private void SetShowButton()
        {
            if (categorySelect == "All Category")
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }

            if (listChooseCategory.Count > 0)
            {
                btnApply.Enabled = true;
                btnApply.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.Enabled = false;
                btnApply.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
        }
        LinearLayout lnBack;
        FrameLayout lnSearch;
        ImageButton btnSearch;
        EditText textSearch;
        Button btnAll;
        RecyclerView rcvListcategory;
        LinearLayout lnNoDataSearch;
        Button btnApply;
        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnSearch = view.FindViewById<FrameLayout>(Resource.Id.lnSearch);
            btnSearch = view.FindViewById<ImageButton>(Resource.Id.btnSearch);
            textSearch = view.FindViewById<EditText>(Resource.Id.textSearch);
            btnAll = view.FindViewById<Button>(Resource.Id.btnAll);
            rcvListcategory = view.FindViewById<RecyclerView>(Resource.Id.rcvListcategory);
            lnNoDataSearch = view.FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
            btnApply = view.FindViewById<Button>(Resource.Id.btnApply);
        }
    }
}