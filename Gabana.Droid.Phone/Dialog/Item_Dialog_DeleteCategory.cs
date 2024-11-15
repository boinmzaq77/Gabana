using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;

namespace Gabana.Droid.Phone
{
    public class Item_Dialog_DeleteCategory : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView textconfirm1, textconfirm2;
        static string Page;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Item_Dialog_DeleteCategory NewInstance(string _page)
        {
            Page = _page;
            var frag = new Item_Dialog_DeleteCategory { Arguments = new Bundle() };
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
                textconfirm1.Text = GetString(Resource.String.dialog_delete_category_1);
                textconfirm2.Text = GetString(Resource.String.dialog_delete_category_2);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void Btn_save_Click(object sender, EventArgs e)
        {
            //Delete Item ที่เลือก 
            //Update DataStatus ของ Item เป็น D ,FWaitSending = 2, WaitSendingTime = Now
            try
            {
                btn_save.Enabled = false;
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    btn_save.Enabled = true;
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }

                CategoryManage categoryManage = new CategoryManage();
                ItemManage itemManage = new ItemManage();
                var cateDelte = await categoryManage.GetCategory((int)DataCashingAll.MerchantId, (int)DataCashing.EditSysCategoryID);
                var UpdateItem = await itemManage.GetItembyCategory((int)cateDelte.MerchantID, (int)cateDelte.SysCategoryID);
                if (UpdateItem != null)
                {
                    foreach (var update in UpdateItem)
                    {
                        update.SysCategoryID = null;
                        var resultUpdate = await itemManage.UpdateItem(update);
                    }
                }
                cateDelte.DataStatus = 'D';
                cateDelte.FWaitSending = 2;
                cateDelte.DateModified = DateTime.UtcNow;
                var updateCate = await categoryManage.UpdateCategory(cateDelte);                
                if (!updateCate)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    if (Page == "main")
                    {
                        DataCashingAll.flagCategoryChange = true;
                        ItemActivity.itemActivity.Resume();
                    }
                    btn_save.Enabled = true;
                    return;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCatagory((int)cateDelte.MerchantID, (int)cateDelte.SysCategoryID);
                }
                else
                {
                    cateDelte.FWaitSending = 2;
                    await categoryManage.UpdateCategory(cateDelte);
                }
                DataCashingAll.flagCategoryChange = true;
                if (Page != "main")
                {
                    //หน้า add                       
                    this.Activity.Finish();
                }
                else
                {
                    ItemActivity.itemActivity.Resume();
                }
                MainDialog.CloseDialog();
                btn_save.Enabled = true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                if (Page == "main")
                {
                    ItemActivity.itemActivity.Resume();
                }
                btn_save.Enabled = true;
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

    }
}
