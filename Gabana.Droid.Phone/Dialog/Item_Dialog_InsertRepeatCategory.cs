using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class Item_Dialog_InsertRepeatCategory : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        static string categoryname, Detail, Event;
        TextView textconfirm1, textconfirm2;
        CategoryManage CategoryManage = new CategoryManage();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Item_Dialog_InsertRepeatCategory NewInstance(string _categoryname, string _detailinsert, string _event)
        {
            categoryname = _categoryname;
            Detail = _detailinsert;
            Event = _event;
            var frag = new Item_Dialog_InsertRepeatCategory { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.pos_dialog_deleteitem, container, false);
            try
            {
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;

                textconfirm1 = view.FindViewById<TextView>(Resource.Id.textconfirm1);
                textconfirm2 = view.FindViewById<TextView>(Resource.Id.textconfirm2);
                textconfirm1.Text = string.Empty;
                textconfirm2.Text = string.Empty;

                var textItemName = categoryname;
                var text1 = GetText(Resource.String.dialog_addcategory1);
                var text2 = GetText(Resource.String.dialog_additem2);

                textconfirm1.Text = text1 + " " + textItemName + " " + text2;
                textconfirm2.Text = GetString(Resource.String.dialog_additem3);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void Btn_save_Click(object sender, EventArgs e)
        { 
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }
                var DetailCategory = JsonConvert.DeserializeObject<Category>(Detail);
                if (DetailCategory != null)
                {
                    if (Event == "insert")
                    {
                        DetailInsert(DetailCategory);
                    }
                    else
                    {
                        DetailUpdate(DetailCategory);
                    }
                    MainDialog.CloseDialog();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }


        async void DetailInsert(Category category)
        {
            try
            {
                var result = await CategoryManage.InsertCategory(category);
                if (!result)
                {
                    Toast.MakeText(Application.Context, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return;
                }
                else
                {
                    Toast.MakeText(Application.Context, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
                }

                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCatagory((int)category.MerchantID, (int)category.SysCategoryID);
                }
                else
                {
                    category.FWaitSending = 2;
                    await CategoryManage.UpdateCategory(category);
                }

                ItemActivity.SetFocusCategory(category);
                ItemActivity.itemActivity.Resume();
                AddCategoryActivity.addcategory.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailInsert at Item_Dialog_InsertRepeatCategory");
            }
        }

        async void DetailUpdate(Category category)
        {
            try
            {

                var result = await CategoryManage.UpdateCategory(category);
                if (!result)
                {
                    Toast.MakeText(Application.Context, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                Toast.MakeText(Application.Context, GetString(Resource.String.editsucess), ToastLength.Short).Show();

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCatagory((int)category.MerchantID, (int)category.SysCategoryID);
                }
                else
                {
                    category.FWaitSending = 2;
                    await CategoryManage.UpdateCategory(category);
                }
                ItemActivity.SetFocusCategory(category);
                ItemActivity.itemActivity.Resume();
                AddCategoryActivity.addcategory.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailUpdate at item_dialog");
            }
        }
    }
}
